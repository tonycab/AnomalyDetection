using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Acquisitions.Discovery
{
    /// <summary>
    /// Type of device halcon discovery
    /// </summary>
    public class DeviceType
    {
        private DeviceType(string value) { Value = value; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// Device type for device phoxi for photoneo
        /// </summary>
        public static DeviceType Phoxi { get { return new DeviceType("Phoxi"); } }
        /// <summary>
        /// Device type for device Ensenso
        /// </summary>
        public static DeviceType Ensenso { get { return new DeviceType("Ensenso-NxLib"); } }
        /// <summary>
        /// Device type for device with interface GigeEvision
        /// </summary>
        public static DeviceType GigeEVision { get { return new DeviceType("GigEVision2"); } }
        /// <summary>
        /// Device type for device with interface USB3Vision
        /// </summary>
        public static DeviceType USB3Vision { get { return new DeviceType("USB3Vision"); } }

    }
}
