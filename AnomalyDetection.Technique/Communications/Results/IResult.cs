using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Results
{
    public abstract class Result
    {
        public virtual string ToCsvResult(char delimiter = ';')
        {
            return "";
        }

    }
}
