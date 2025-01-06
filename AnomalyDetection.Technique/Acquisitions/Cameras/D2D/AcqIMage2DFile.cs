using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace AnomalyDetection.Technique.Acquisitions.Cameras.D2D
{
    /// <summary>
    /// Class acquisition image file
    /// </summary>
    public class AcqIMage2DFile : IAcqImage2D
    {
        #region "Properties"
        /// <summary>
        /// Directory file
        /// </summary>
        public string FileDirectory { get; set; }
        #endregion

        #region Event
        /// <summary>
        /// Acquisition progress
        /// </summary>
        public event EventHandler Progress;

        /// <summary>
        /// Acqusition end
        /// </summary>
        public event EventHandler End;
        #endregion

        #region "Contructor"
        /// <summary>
        /// Constructor
        /// </summary>
        public  AcqIMage2DFile() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileDirectory">File directory</param>
        public AcqIMage2DFile(string fileDirectory) {FileDirectory = fileDirectory; }
        #endregion

        #region "Methode"
        /// <summary>
        /// Acquisition image synchrone
        /// </summary>
        /// <returns>Image</returns>
        public virtual Image2D GetImage2D()
        {

            if (FileDirectory == null) return null;

            Progress?.Invoke(this, new EventArgs());

            //HOperatorSet.ReadImage(out HObject image, FileDirectory);

            GC.Collect();

            HImage himage = new HImage();

            himage.ReadImage(FileDirectory);

            End?.Invoke(this, new EventArgs());
      
            return new Image2D(himage, FileDirectory);

        }

        /// <summary>
        /// Return the name file
        /// </summary>
        /// <returns>Name file</returns>
        public override string ToString() => FileDirectory;

        /// <summary>
        /// Acquiring image file asynchrone methode
        /// </summary>
        /// <param name="CallBackImage">Callback methode</param>
        public async void GetImage2DAsync(Action<Image2D> CallBackImage)
        {
            await Task.Run(() =>
            {
                CallBackImage(GetImage2D());
            });
         }

        public void init()
        {
           
        }
        #endregion
    }
}
