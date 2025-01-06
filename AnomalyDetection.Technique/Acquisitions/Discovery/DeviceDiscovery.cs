using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HalconDotNet;

namespace AnomalyDetection.Technique.Acquisitions.Discovery
{

    /// <summary>
    /// Discover the camera peripherals available on the system
    /// </summary>
    public class DeviceDiscovery : List<AcqDevice>
    {

        #region Properties
        /// <summary>
        /// control frequency device 
        /// </summary>
        public int PollTime { get; set; }

        /// <summary>
        /// Device collection scanned
        /// </summary>
        private Dictionary<string, DeviceType> HashsetDevicesScanned = new Dictionary<string, DeviceType>();
        /// <summary>
        /// Device collection connected
        /// </summary>
        private Dictionary<string, DeviceType> HashsetDevicesDetected = new Dictionary<string, DeviceType>();
        /// <summary>
        /// Collection type device
        /// </summary>
        private HashSet<DeviceType> CollectionDeviceType = new HashSet<DeviceType>();

        /// <summary>
        /// Instance singleton
        /// </summary>
        private static DeviceDiscovery instance = null;

        /// <summary>
        /// lock singleton
        /// </summary>
        private static readonly object padlock = new object();
        #endregion

        #region Event
        /// <summary>
        /// New device detected
        /// </summary>
        static public event EventHandler<DiscoveryEventArgs> EventNewDevice;

        /// <summary>
        /// New device not detected
        /// </summary>
        static public event EventHandler<DiscoveryEventArgs> EventRemoveDevice;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new HDeviceDiscovery
        /// </summary>
        /// <param name="deviceType">Collection of deviceType/param>
        /// <param name="pollTime">poll time ms</param>
        private DeviceDiscovery(int pollTime)
        {

            PollTime = pollTime;
            GetDevicesAsync2();
        }

        #endregion

        #region Methode
        /// <summary>
        /// Get instance Singleton and add the device for update state
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static DeviceDiscovery Instance(AcqDevice device)
        {

            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new DeviceDiscovery(1000);
                }

                instance.Add(device);

                return instance;
            }
        }
        /// <summary>
        /// Add device for update state
        /// </summary>
        /// <param name="device"></param>
        public new void Add(AcqDevice device)
        {

            CollectionDeviceType.Add(device.Type);

            base.Add(device);
        }
        /// <summary>
        /// Remove device
        /// </summary>
        /// <param name="device"></param>
        public new void Remove(AcqDevice device)
        {

            CollectionDeviceType.Remove(device.Type);

            base.Remove(device);
        }
        /// <summary>
        /// Get the device present
        /// </summary>
        /// <param name="deviceType">Type device checked</param>
        /// <returns></returns>
        private string[] GetDevices(DeviceType deviceType) => GetListDevices(deviceType);



        /// <summary>
        /// Get the device present
        /// </summary>
        /// <param name="deviceType">Type device checked</param>
        /// <returns></returns>
        public static string[] GetListDevices(DeviceType deviceType)
        {
            try
            {
                HInfo.InfoFramegrabber(deviceType.Value, "device", out HTuple valueList);



                if (deviceType.Value == DeviceType.GigeEVision.Value)
                {

                    List<string> s = new List<string>();

                    foreach (var item in (string[])valueList)
                    {
                        string pattern = @"device:(\w+)";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(item);
                        string d = matches[0].Groups[1].Value;
                        s.Add(d);

                    }

                    return s.ToArray();


                }


                return valueList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>
        /// Asynchrone methode for check presence device and update state collection device
        /// </summary>
        /// <returns>task</returns>
        private async Task GetDevicesAsync()
        {
            await Task.Run(() =>
            {
                do
                {
                    //Clear old device scanned
                    HashsetDevicesScanned.Clear();

                    //Scan device connected
                    foreach (DeviceType deviceType in new HashSet<DeviceType>(CollectionDeviceType))
                    {

                        //New device detection 
                        foreach (string str in GetDevices(deviceType))
                        {
                            HashsetDevicesScanned.Add(str, deviceType);

                            if (!HashsetDevicesDetected.ContainsKey(str))
                            {
                                HashsetDevicesDetected.Add(str, deviceType);
                                EventNewDevice?.Invoke(this, new DiscoveryEventArgs(deviceType, str));
                            }
                        }

                    }
                    //Remove device detection
                    foreach (var str in new Dictionary<string, DeviceType>(HashsetDevicesDetected))
                    {

                        if (!HashsetDevicesScanned.ContainsKey(str.Key))
                        {

                            if (str.Value.Value != DeviceType.GigeEVision.Value)
                            {
                                HashsetDevicesDetected.Remove(str.Key);

                                DiscoveryEventArgs e = new DiscoveryEventArgs();
                                e.DeviceType = str.Value;
                                e.Name = str.Key;
                                EventRemoveDevice?.Invoke(this, e);
                            }
                        }
                    }

                    //Update the state device used in device scanned
                    foreach (AcqDevice device in this)
                    {

                        if (HashsetDevicesDetected.ContainsKey(device.Name) && device.Detected == false)
                        {
                            device.Detected = true;
                        }
                        else if (!HashsetDevicesDetected.ContainsKey(device.Name) && device.Detected == true)
                        {
                            device.Detected = false;
                            device.State = AcqDevice.DeviceState.Closed;
                        }

                    }

                    //Time pause
                    System.Threading.Thread.Sleep(PollTime);

                } while (true);

            });
        }


        /// <summary>
        /// Asynchrone methode for check presence device and update state collection device
        /// </summary>
        /// <returns>task</returns>
        private async Task GetDevicesAsync2()
        {
            await Task.Run(() =>
            {
                do
                {

                    //Update the state device used in device scanned
                    foreach (AcqDevice device in this)
                    {
                        //Device detected
                        if (device.GetDetectedDevice())
                        {

                            if (!HashsetDevicesDetected.ContainsKey(device.Name))
                            {
                                HashsetDevicesDetected.Add(device.Name, device.Type);

                                EventNewDevice?.Invoke(this, new DiscoveryEventArgs(device.Type, device.Name));

                                device.Detected = true;

                            }

                        }

                        //Device no detected
                        else
                        {
                            if (HashsetDevicesDetected.ContainsKey(device.Name))
                            {
                                HashsetDevicesDetected.Remove(device.Name);

                                DiscoveryEventArgs e = new DiscoveryEventArgs();
                                e.DeviceType = device.Type;
                                e.Name = device.Name;
                                EventRemoveDevice?.Invoke(this, e);

                                device.Detected = false;
                                device.Disconnect();
                                device.State = AcqDevice.DeviceState.Closed;

                            }
                        }
                    }


                    //Time pause
                    System.Threading.Thread.Sleep(PollTime);

                } while (true);

            });
        }





        #endregion
    }

}