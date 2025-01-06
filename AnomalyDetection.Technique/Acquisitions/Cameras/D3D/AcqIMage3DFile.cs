using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace AnomalyDetection.Technique.Acquisitions.Cameras.D3D
{
    /// <summary>
    /// Class acquisition image file
    /// </summary>
    public class AcqIMage2DFile : IAcqImage3D
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
        public virtual Image3D GetImage3D()
        {

            if (FileDirectory == null) return null;

            Progress?.Invoke(this, new EventArgs());

            HObject Objet3D = new HObject();

            //Lecture du fichier HOBJ
            Objet3D.ReadObject(FileDirectory);

            //Selection des composantes (Nuage de points, normal et texture)
            HObject xyz = Objet3D.SelectObj(1);
            HObject normal = Objet3D.SelectObj(2);
            HObject texture = Objet3D.SelectObj(3);
            Objet3D.Dispose();

            //Décomposition des cannaux
            //HOperatorSet.Decompose3(xyz, out HObject x, out HObject y, out HObject z);
            //HOperatorSet.Decompose3(normal, out HObject nx, out HObject ny, out HObject nz);

            ////Creation du ObjectModel3D
            //HObjectModel3D model3D = new HObjectModel3D();
            //HOperatorSet.XyzToObjectModel3d(x, y, z, out HTuple objectModel3D);

            ////Ajout des normals
            //HOperatorSet.GetRegionPoints(normal, out HTuple Rows, out HTuple Columns);
            //HOperatorSet.GetGrayval(nx, Rows, Columns, out HTuple NX);
            //HOperatorSet.GetGrayval(ny, Rows, Columns, out HTuple NY);
            //HOperatorSet.GetGrayval(nz, Rows, Columns, out HTuple NZ);


            //HOperatorSet.SetObjectModel3dAttribMod(
            //        objectModel3D,
            //        ((new HTuple("point_normal_x")).TupleConcat("point_normal_y")).TupleConcat("point_normal_z"), new HTuple(),
            //        ((NX.TupleConcat(NY))).TupleConcat(NZ));


            ////Texture
            //HOperatorSet.GetGrayval(texture, Rows, Columns, out HTuple T);
            //HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "&intensity", "points", T);

            End?.Invoke(this, new EventArgs());

            return new Image3D(xyz, normal, texture);

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
        public async void GetImage3DAsync(Action<Image3D> CallBackImage)
        {
            await Task.Run(() =>
            {
                CallBackImage(GetImage3D());
            });
         }

        public void init()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
