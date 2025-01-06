using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Acquisitions.Discovery
{    /// <summary>
     /// Custum EventArgrs for event device discovery
     /// </summary>
    public class DiscoveryEventArgs : EventArgs
    {
        public DeviceType DeviceType { get; set; }
        public string Name { get; set; }

        public DiscoveryEventArgs()
        {

        }
        public DiscoveryEventArgs(DeviceType deviceType, string name)
        {
            DeviceType = deviceType;
            Name = name;
        }

        public override string ToString()
        {
            return $"{DeviceType} - {Name}" ;
        }


    }
}
