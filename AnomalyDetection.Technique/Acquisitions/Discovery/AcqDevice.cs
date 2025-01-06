using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace for acquering image 
/// </summary>
namespace AnomalyDetection.Technique.Acquisitions.Discovery
{
    /// <summary>
    /// Abstract class device aquisition vision
    /// </summary>
    public abstract class AcqDevice 
    {
        #region Enum
        /// <summary>
        /// Enum state device
        /// Connecting = 1,
        /// Connected = 2,
        /// Closing = 3,
        /// Closed = 4,
        /// </summary>
        public enum DeviceState
        {
            Connecting = 1,
            Connected = 2,
            Closing = 3,
            Closed = 4,
        }
        #endregion

        #region Propeties

        /// <summary>
        /// State to device
        /// </summary>
        private DeviceState state = DeviceState.Closed;
        public virtual DeviceState State
        {
            get => state;

            set
            {
                if (state != value)
                {
                    state = value;

                    StateChange();
                }
            }
        }

        /// <summary>
        /// Device name
        /// </summary>
        public DeviceType Type { set; internal get; }

        /// <summary>
        /// Device name
        /// </summary>
        private string name;

        public string Name
        {
            get => name;

            set
            {
                if (name != value)
                {
                    name = value;
                    Detected = false;
                    State = DeviceState.Closed;
                    StateChange();
                    Dispose();
                }
            }
        }

        ///// <summary>
        ///// Device is recconnected after detection 
        ///// </summary>
        private bool autoConnection;
        public bool AutoConnection
        {
            get => autoConnection;

            set
            {

                if (autoConnection != value)
                {
                    autoConnection = value;
                }
            }
        }

        /// <summary>
        /// Device is detected
        /// </summary>
        private bool detected = false;
        public virtual bool Detected
        {
            get => detected;

            set
            {
                if (detected != value)
                {
                    detected = value;

                    if (detected == true & AutoConnection)
                    {
                        Connect();
                    }

                    //StateChange();
                }
            }
        }
        #endregion

        #region Event

        /// <summary>
        /// The device state has changed
        /// </summary>
        public virtual event EventHandler<DeviceEventArg> EventState;

        #endregion

        #region Collection
        /// <summary>
        /// Device collection available
        /// </summary>
        static private List<AcqDevice> Devices = new List<AcqDevice>();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor class device
        /// </summary>
        /// <param name="type">Type device</param>
        /// <param name="name">Name device</param>
        public AcqDevice(DeviceType type, string name)
        {
            
            Type = type;
            Name = name;
            AutoConnection = true;
            DeviceDiscovery.Instance(this);
           
        }
        /// <summary>
        /// Constructor class device
        /// </summary>
        /// <param name="type">Type device</param>
        /// <param name="name">Name device</param>
        /// <param name="autoConnection">Autoconnection device after detection</param>
        public AcqDevice(DeviceType type, string name, bool autoConnection)
        {
            
            Type = type;
            Name = name;
            AutoConnection = autoConnection;
            DeviceDiscovery.Instance(this);
            
        }
        #endregion

        #region Methode
        /// <summary>
        /// Methode call event
        /// </summary>
        protected void StateChange()
        {
            EventState?.Invoke(this, new DeviceEventArg(this));
        }

        /// <summary>
        /// Methode for open connection with the device
        /// </summary>
        /// 
        abstract public void Connect();
        /// <summary>
        /// Methode for close connection with the device
        /// </summary>
        abstract public void Disconnect();


        /// <summary>
        /// Methode who returne presence or not presence camera
        /// </summary>
        /// <returns></returns>
        public virtual bool GetDetectedDevice()
        {

            HInfo.InfoFramegrabber(Type.Value, "device", out HTuple valueList);

            return valueList.SArr.Contains(Name)?true:false;

        }

        /// <summary>
        /// Return the name
        /// </summary>
        /// <returns>Returns the device type and name</returns>
        public override string ToString()
        {
            return $"{Type.Value}-{Name}-{(Detected?"Detected":"NoDetected")}-{State}";
        }

        /// <summary>
        /// Dispose methode
        /// </summary>
        public void Dispose()
        {
            if (Detected | State == DeviceState.Connected)
            {
                Disconnect();

            }
        }
        #endregion
    }
}
