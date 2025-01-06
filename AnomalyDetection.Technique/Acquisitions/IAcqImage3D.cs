using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Acquisitions
{       /// <summary>
        /// Interface for get image 
        /// </summary>
     public interface IAcqImage3D
    {
        /// <summary>
        /// Return a image direct methode
        /// </summary>
        /// <returns>Htupe Image model3D</returns>
        Image3D GetImage3D();
        /// <summary>
        /// Event to progress capturing image
        /// </summary>
        event EventHandler Progress;

        /// <summary>
        /// Event capturing image termined
        /// </summary>
        event EventHandler End;

        /// <summary>
        /// Return a image asynchrone methode
        /// </summary>
        void  GetImage3DAsync(Action<Image3D> CallBackImage );


        void init();
    }
}
