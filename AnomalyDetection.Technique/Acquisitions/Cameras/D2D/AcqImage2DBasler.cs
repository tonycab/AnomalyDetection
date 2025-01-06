using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using AnomalyDetection.Technique.Acquisitions.Discovery;
using AnomalyDetection.Technique.Acquisitions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace AnomalyDetection.Technique.Acquisitions.Cameras.D2D
{
    public class AcqImage2DBasler : AcqDevice, IAcqImage2D
    {

        private HFramegrabber Framegrabber;

        public AcqImage2DBasler(string name) : base(DeviceType.GigeEVision, name) { }

        public AcqImage2DBasler(string name, bool autoConnection) : base(DeviceType.GigeEVision, name, autoConnection) { }

        public override void Disconnect()
        {
            //Caméra en cours de déconnection
            State = DeviceState.Closing;

            try
            {

                Framegrabber?.CloseFramegrabber();

            }
            catch
            {


            }
            finally
            {
                //Caméra déconnecté
                State = DeviceState.Closed;
            }
        }

        public override void Connect()
        {

            if (!(State == DeviceState.Closed) || Detected!=true) return;

            try
            {
                //Connection en cours
                State = DeviceState.Connecting;

                //Connection à la caméra
                Framegrabber = new HFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", Name, 0, -1);

                //Caméra connecté
                State = DeviceState.Connected;

                CheckConnection();
            }
            catch
            {
                State = DeviceState.Closed;
                Detected = false;

            }
        }

        /// <summary>
        /// Retourne la présence ou non de la caméra
        /// </summary>
        /// <returns></returns>
        public override bool GetDetectedDevice()
        {
            //return _Detected;
            if (Detected == true) return true;

            HInfo.InfoFramegrabber(Type.Value, "device", out HTuple valueList);


            List<string> s = new List<string>();

            foreach (var item in (string[])valueList)
            {
                string pattern = @"device:(\w+)";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(item);
                string d = matches[0].Groups[1].Value;
                s.Add(d);

            }

            return s.Contains(Name) ? true : false;

        }

        private bool _Detected;
        private async void CheckConnection()
        {
            HOperatorSet.CreateMessageQueue(out HTuple QueueHandle);

            Detected = true;

            await Task.Run(() =>
            {

                HOperatorSet.SetFramegrabberParam(Framegrabber, "event_selector", "[Device]EventDeviceLost");
                HOperatorSet.SetFramegrabberParam(Framegrabber, "event_notification_helper", "enable");
                HOperatorSet.SetFramegrabberParam(Framegrabber, "event_message_queue", QueueHandle);


                HOperatorSet.DequeueMessage(QueueHandle, "timeout", "infinite", out HTuple MessageHandle);

                HOperatorSet.GetMessageTuple(MessageHandle, "event_name", out HTuple NEventName);
            });


            Detected = false;

        }


        public Image2D GetImage2D()
        {

            if (Detected && State == DeviceState.Connected)
            {
                try
                {
                    // Use using statement to ensure proper disposal of temporary HImage objects
                    HImage image = Framegrabber.GrabImage();

                    return new Image2D(image, $"{Name} - {DateTime.Now}");

                }
                catch (Exception ex)
                {
                    this.Detected = false;
                }
            }

            return null;

        }

        public void init()
        {
            //throw new NotImplementedException();
        }
    }
}
