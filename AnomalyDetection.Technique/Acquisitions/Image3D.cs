using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace AnomalyDetection.Technique.Acquisitions
{
    public class Image3D
    {

        #region property
        public HObject Xyz { get; set; }
        public HObject Texture { get; set; }
        public HObject NormalMap { get; set; }
        public HTuple ObjectModel3D {get;set;}

        #endregion

        #region contructeur
        public Image3D(){}

        public Image3D(HObject xyz) : this( xyz,null, null){}

        public Image3D(HObject xyz, HObject normalMap):this(null, xyz,  normalMap){}

        public Image3D(HObject xyz, HObject normalMap, HObject texture)
        {
            Xyz = xyz;
            Texture = texture;
            NormalMap = normalMap;
            ObjectModel3D = GetObjectImage3D();
        }




        public Image3D(HTuple objectModel3D)
        {
            ObjectModel3D = objectModel3D;
        }

        #endregion

        #region methodes
        public static Image3D ImportNewImage3DFile(string filename)
        {
            try
            {
                HOperatorSet.ReadObject(out HObject _objectVal, filename);
                HOperatorSet.SelectObj(_objectVal, out HObject _XYZ, 1);
                HOperatorSet.SelectObj(_objectVal, out HObject _Normals, 2);
                HOperatorSet.SelectObj(_objectVal, out HObject _Texture, 3);

                Image3D image3D = new Image3D(_XYZ, _Normals, _Texture);

                return image3D;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool ImportImage3DFile(string filename)
        {
            try
            {
                HOperatorSet.ReadObject(out HObject _objectVal, filename);
                HOperatorSet.SelectObj(_objectVal, out HObject _XYZ, 1);
                HOperatorSet.SelectObj(_objectVal, out HObject _Normals, 2);
                HOperatorSet.SelectObj(_objectVal, out HObject _Texture, 3);

                this.Xyz = _XYZ;
                this.NormalMap = _Normals;
                this.Texture = _Texture;
                this.ObjectModel3D = this.GetObjectImage3D();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void ImportToSTLFile(string path)
        {

            HObjectModel3D cad = new HObjectModel3D();
            cad.ReadObjectModel3d(path, 1, "file_type", "stl");
            ObjectModel3D = cad;

        }


        public static bool ExportImage3DFile(Image3D image, string filename)
        {
            try
            {
                HOperatorSet.ConcatObj(image.Xyz, image.NormalMap, out HObject objectConcat);
                HOperatorSet.ConcatObj(objectConcat, image.Texture, out HObject objectConcat2);

                HOperatorSet.WriteObject(objectConcat2, filename);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ExportImage3DFile(string filename) => ExportImage3DFile(this, filename);


        private HTuple GetObjectImage3D()
        {

            if (Xyz != null)
            {
                //Décomposition des cannaux
                HOperatorSet.Decompose3(Xyz, out HObject x, out HObject y, out HObject z);

                //Creation du ObjectModel3D
                HOperatorSet.XyzToObjectModel3d(x, y, z, out HTuple objectModel3D);

                //Ajout des normals
                if (NormalMap != null)
                {
                    HOperatorSet.Decompose3(NormalMap, out HObject nx, out HObject ny, out HObject nz);
                    HOperatorSet.GetRegionPoints(NormalMap, out HTuple Rows, out HTuple Columns);
                    HOperatorSet.GetGrayval(nx, Rows, Columns, out HTuple NX);
                    HOperatorSet.GetGrayval(ny, Rows, Columns, out HTuple NY);
                    HOperatorSet.GetGrayval(nz, Rows, Columns, out HTuple NZ);
                    HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, new HTuple("point_normal_x").TupleConcat("point_normal_y").TupleConcat("point_normal_z"), new HTuple(),
                            NX.TupleConcat(NY).TupleConcat(NZ));
                }
                if (Texture != null)
                {
                    //Texture
                    HOperatorSet.GetRegionPoints(Texture, out HTuple Rows, out HTuple Columns);
                    HOperatorSet.GetGrayval(Texture, Rows, Columns, out HTuple T);
                    HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "&intensity", "points", T);
                }
                return objectModel3D;
            }
            return null;
        }

        #endregion

        #region destructeur
        public void Dispose()
        {

            if (this.Texture != null)
            {
                HOperatorSet.ClearObj(this.Texture);
                this.Texture = (HObject)null;
            }
            if (this.NormalMap != null)
            {
                HOperatorSet.ClearObj(this.NormalMap);
                this.NormalMap = (HObject)null;
            }
            if (this.Xyz != null)
            {
                HOperatorSet.ClearObj(this.Xyz);
                this.Xyz = (HObject)null;
            }

        }
        #endregion

    }


}

