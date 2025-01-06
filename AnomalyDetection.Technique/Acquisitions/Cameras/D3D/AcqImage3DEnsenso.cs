using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using AnomalyDetection.Technique.Acquisitions.Discovery;
using AnomalyDetection.Technique.Acquisitions;

namespace AnomalyDetection.Technique.Acquisitions.Cameras.D3D
{

    class AcqImage3DEnsenso : AcqDevice, IAcqImage3D
    {

        #region Proporties
        // Local iconic variables
        private HObject ho_ImageData;
        private HObject ho_TextureImage;
        private HObject ho_outXYZ, ho_Disparity, ho_NormalMap, ho_DepthMap, ho_ConfidenceMap;
        private HObject ho_Region, ho_Contours;
        private HTuple hv_AcqHandle;
        private HTuple hv_Data;
        private HTuple hv_GrabDataItems;

        /// <summary>
        /// Event start capturing
        /// </summary>
        public event EventHandler Progress;
        /// <summary>
        /// Event capturing termined
        /// </summary>
        public event EventHandler End;


        private static readonly object padlock = new object();

        #endregion

        #region "Constructeur"
        /// <summary>
        /// Constructor device phoxi
        /// </summary>
        /// <param name="name">Nome unique de la caméra</param>
        public AcqImage3DEnsenso(string name):base(DeviceType.Ensenso,name){}
        /// <summary>
        /// Constructor device phoxi
        /// </summary>
        /// <param name="name">Device name</param>
        /// /// <param name="autoConnection">Device auto connection </param>
        public AcqImage3DEnsenso(string name,bool autoConnection):base(DeviceType.Ensenso, name,autoConnection){}

        #endregion

        /// <summary>
        /// Open the connection with the device
        /// </summary>
        public override async void Connect()
        {
            Boolean resultat = await Task<bool>.Run(() =>
                {
                    try
                    {
                        if(State != DeviceState.Closed) return false;

                        State = DeviceState.Connecting;

                        if (hv_AcqHandle != null) hv_AcqHandle.Dispose();
                        // Connexion au scanner phoxi

                        HOperatorSet.OpenFramegrabber(Type.Value, 0, 0,0, 0, 0,0, "default", 0, "Raw",  -1, "false", "Stereo", Name,0, 0, out hv_AcqHandle);

                        hv_GrabDataItems = new HTuple();
                        hv_GrabDataItems[0] = "Images/PointMap";
                        hv_GrabDataItems[1] = "Images/Rectified/Right";
                        hv_GrabDataItems[2] = "Images/Normals";

                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, (HTuple)"grab_data_items", hv_GrabDataItems);

                        //Connected = true;
                        State = DeviceState.Connected;
                        return true;

                    }
                    catch (Exception ex)
                    {
                        State = DeviceState.Closed;
                        return false;
                    }                   
                }
                );
        }

        /// <summary>
        /// Close the connection with the device
        /// </summary>
        public override async void Disconnect()
        {
            await Task.Run(() =>
           {
               try
               {                   

                   if(State == DeviceState.Connected)
                   {
                       State = DeviceState.Closing;
                       HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                   }
                   State = DeviceState.Closed;
               }
               catch(Exception ex)
               {
                   State = DeviceState.Closed;
                   return;
               }
               finally
               {

                   if (hv_AcqHandle != null) hv_AcqHandle.Dispose();
               }
           });

        }
        /// <summary>
        /// Return a image of the device
        /// </summary>
        public Image3D GetImage3D()
        {

            lock (padlock)
            {

                if (State != DeviceState.Connected) return null;

                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_TextureImage);
                HOperatorSet.GenEmptyObj(out ho_ImageData);
                HOperatorSet.GenEmptyObj(out ho_Region);
                HOperatorSet.GenEmptyObj(out ho_Contours);
                HOperatorSet.GenEmptyObj(out ho_outXYZ);
                HOperatorSet.GenEmptyObj(out ho_Disparity);
                HOperatorSet.GenEmptyObj(out ho_NormalMap);
                HOperatorSet.GenEmptyObj(out ho_DepthMap);
                HOperatorSet.GenEmptyObj(out ho_ConfidenceMap);

                try
                {
                    Progress?.Invoke(this, new EventArgs());

                    ho_TextureImage.Dispose();
                    ho_ImageData.Dispose();
                    ho_Region.Dispose();
                    ho_Contours.Dispose();

                    //no supported
                    //HOperatorSet.GrabDataAsync(out ho_ImageData, out ho_Region, out ho_Contours, hv_AcqHandle, 0, out hv_Data);

                    HOperatorSet.GrabData(out ho_ImageData, out ho_Region, out ho_Contours, hv_AcqHandle, out hv_Data);

                    ////Selection des composantes (Nuage de points, normal et texture etc)
                    HOperatorSet.SelectObj(ho_ImageData, out ho_outXYZ, 1);
                    HOperatorSet.SelectObj(ho_ImageData, out ho_Disparity, 2);
                    HOperatorSet.SelectObj(ho_ImageData, out ho_NormalMap, 3);
                 
                    //Trigger
                    HOperatorSet.GrabImage(out ho_TextureImage, hv_AcqHandle);
                    return new Image3D(ho_outXYZ, ho_NormalMap, ho_TextureImage);

                }
                catch (HalconException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {

                    End?.Invoke(this, new EventArgs());

                    //ho_TextureImage.Dispose();
                    //ho_ImageData.Dispose();
                    //ho_Region.Dispose();
                    //ho_Contours.Dispose();
                    //ho_outXYZ.Dispose();
                    //ho_Disparity.Dispose();
                    //ho_NormalMap.Dispose();
                    //ho_DepthMap.Dispose();
                    //ho_ConfidenceMap.Dispose();
                    GC.Collect();

                }
            }

        }
        /// <summary>
        /// Acquiring image file asynchrone methode
        /// </summary>
        /// <param name="CallBackImage"></param>
        public async void GetImage3DAsync(Action<Image3D> CallBackImage)
        {
            await Task.Run(() =>
            {

                CallBackImage(GetImage3D());


            });
        }

        public void init()
        {
            throw new NotImplementedException();
        }
    }
}
