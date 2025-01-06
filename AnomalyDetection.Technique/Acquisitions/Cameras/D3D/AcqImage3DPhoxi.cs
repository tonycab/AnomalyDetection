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

    public class AcqImage3DPhoxi : AcqDevice, IAcqImage3D
    {

        #region Proporties
        // Local iconic variables
        private HObject ho_ImageData;
        private HObject ho_TextureImage;
        private HObject ho_Texture, ho_PointCloud, ho_Normals, ho_DepthMap, ho_ConfidenceMap;
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
        public AcqImage3DPhoxi(string name) : base(DeviceType.Phoxi, name) { }

        /// <summary>
        /// Constructor device phoxi
        /// </summary>
        /// <param name="name">Device name</param>
        /// /// <param name="autoConnection">Device auto connection </param>
        public AcqImage3DPhoxi(string name, bool autoConnection) : base(DeviceType.Phoxi, name, autoConnection) { }

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
                        if (State != DeviceState.Closed) return false;

                        State = DeviceState.Connecting;

                        if (hv_AcqHandle != null) hv_AcqHandle.Dispose();
                        // Connexion au scanner phoxi
                        HOperatorSet.OpenFramegrabber(Type.Value, 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", Name, -1, -1, out hv_AcqHandle);

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

                   if (State == DeviceState.Connected)
                   {
                       State = DeviceState.Closing;
                       HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                   }
                   State = DeviceState.Closed;
               }
               catch (Exception ex)
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
                HOperatorSet.GenEmptyObj(out ho_Texture);
                HOperatorSet.GenEmptyObj(out ho_PointCloud);
                HOperatorSet.GenEmptyObj(out ho_Normals);
                HOperatorSet.GenEmptyObj(out ho_DepthMap);
                HOperatorSet.GenEmptyObj(out ho_ConfidenceMap);

                try
                {
                    Progress?.Invoke(this, new EventArgs());

                    ho_TextureImage.Dispose();
                    ho_ImageData.Dispose();
                    ho_Region.Dispose();
                    ho_Contours.Dispose();

                    //Trigger
                    HOperatorSet.GrabImageAsync(out ho_TextureImage, hv_AcqHandle, 0);
                    HOperatorSet.GrabDataAsync(out ho_ImageData, out ho_Region, out ho_Contours, hv_AcqHandle, 0, out hv_Data);

                    ////Selection des composantes (Nuage de points, normal et texture etc)
                    HOperatorSet.SelectObj(ho_ImageData, out ho_Texture, 1);
                    HOperatorSet.SelectObj(ho_ImageData, out ho_PointCloud, 2);
                    HOperatorSet.SelectObj(ho_ImageData, out ho_Normals, 3);

                    //HOperatorSet.SelectObj(ho_ImageData, out ho_DepthMap, 4);
                    //HOperatorSet.SelectObj(ho_ImageData, out ho_ConfidenceMap, 5);

                    //Décomposition des cannaux
                    //HOperatorSet.Decompose3(ho_PointCloud, out HObject x, out HObject y, out HObject z);
                    //HOperatorSet.Decompose3(ho_Normals, out HObject nx, out HObject ny, out HObject nz);

                    //Creation du ObjectModel3D
                    //HObjectModel3D model3D = new HObjectModel3D();
                    //HOperatorSet.XyzToObjectModel3d(x, y, z, out HTuple objectModel3D);

                    //Ajout des normals
                    //HOperatorSet.GetRegionPoints(ho_Normals, out HTuple Rows, out HTuple Columns);
                    //HOperatorSet.GetGrayval(nx, Rows, Columns, out HTuple NX);
                    //HOperatorSet.GetGrayval(ny, Rows, Columns, out HTuple NY);
                    //HOperatorSet.GetGrayval(nz, Rows, Columns, out HTuple NZ);

                    //HOperatorSet.SetObjectModel3dAttribMod(objectModel3D,new HTuple("point_normal_x").TupleConcat("point_normal_y").TupleConcat("point_normal_z"), new HTuple(),
                    //        NX.TupleConcat(NY).TupleConcat(NZ));

                    //Texture
                    //HOperatorSet.GetGrayval(ho_Texture, Rows, Columns, out HTuple T);
                    //HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "&intensity", "points", T);

                    return new Image3D(ho_PointCloud, ho_Normals, ho_Texture);

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
                    //ho_Texture.Dispose();
                    //ho_PointCloud.Dispose();
                    //ho_Normals.Dispose();
                    //ho_DepthMap.Dispose();
                    //ho_ConfidenceMap.Dispose();
                    //GC.Collect();

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
