using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Acquisitions.Discovery
{
    /// <summary>
    /// Custum event args for device detected halcom
    /// </summary>
    public class DeviceEventArg : EventArgs
    {
        /// <summary>
        /// Type device
        /// </summary>
        public DeviceType DeviceType { get; set; }
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Device detected
        /// </summary>
        public bool Detected { get; set; }
        /// <summary>
        /// State device connection
        /// </summary>
        public AcqDevice.DeviceState State { get; set; }
        
        #region Constructor        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">AcqDevice</param>
        public DeviceEventArg()
        {
 
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device"></param>
        public DeviceEventArg(AcqDevice device)
        {
            DeviceType = device.Type;
            Name = device.Name;
            Detected = device.Detected;

            State = device.State;
        }

        /// <summary>
        /// Return the name
        /// </summary>
        /// <returns>Returns the device type and name</returns>
        public override string ToString()
        {
            return $"{DeviceType.Value}-{Name}-{(Detected ? "Detected" : "NoDetected")}-{State}";
        }



        #endregion
    }
}
