using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Acquisitions
{
    public interface IAcqImage2D
    {
        Image2D GetImage2D();

        void init();
    }
}
