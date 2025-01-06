using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Requests
{

    public class InvalidArgRequestException : Exception
    {
        public InvalidArgRequestException() { }
        public InvalidArgRequestException(string message) : base("Invalid arg :" + message) { }
        public InvalidArgRequestException(string message, Exception inner) : base("Invalid arg :"+ message, inner) { }
        protected InvalidArgRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
    
    
}
