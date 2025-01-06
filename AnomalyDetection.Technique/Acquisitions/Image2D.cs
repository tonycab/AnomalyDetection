using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace AnomalyDetection.Technique.Acquisitions
{
    public class Image2D
    {
        public HObject Image { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        private Image2D() { }

        public Image2D(HObject image)
        {
            Image = image;
            Date = DateTime.Now;

        }
        public Image2D(HObject image, string name)
        {
            Image = image;
            Date = DateTime.Now;
            Name = name;

        }

        public static implicit operator HImage(Image2D image) => (HImage)image.Image;

        public static implicit operator string(Image2D image) => image.Name;
        public Bitmap ToBitmap(int height)
        {
            HImage im = (HImage)Image;

            IntPtr r = IntPtr.Zero;
            IntPtr g = IntPtr.Zero;
            IntPtr b = IntPtr.Zero;
            int w, h;

            //Redimmensionnement de l'image

                im.GetImageSize(out w,out  h);

                w = w*height/h;
                h = height;

                im = im.ZoomImageSize(w,h, "nearest_neighbor");


            //Image couleur 
            if (im.CountChannels() == 3)
            {
                //Recupération des 3 canaux de couleur
                im.GetImagePointer3(out r, out g, out b, out string type, out w, out h);

                byte[] red = new byte[w * h];
                byte[] green = new byte[w * h];
                byte[] blue = new byte[w * h];

                Marshal.Copy(r, red, 0, w * h);
                Marshal.Copy(g, green, 0, w * h);
                Marshal.Copy(b, blue, 0, w * h);

                Bitmap bitmap2 = new Bitmap(w, h, PixelFormat.Format32bppRgb);
                Rectangle rect2 = new Rectangle(0, 0, w, h);
                BitmapData bitmapData2 = bitmap2.LockBits(rect2, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                unsafe
                {
                    byte* bptr2 = (byte*)bitmapData2.Scan0;
                    for (int i = 0; i < w * h; i++)
                    {
                        bptr2[i * 4] = blue[i];
                        bptr2[i * 4 + 1] = green[i];
                        bptr2[i * 4 + 2] = red[i];
                        bptr2[i * 4 + 3] = 255;
                    }
                }
                bitmap2.UnlockBits(bitmapData2);

                return bitmap2;

            }
            //Image Gray
            else
            {

                b = im.GetImagePointer1(out string type, out w, out h);

                byte[] gray = new byte[w * h];

                Marshal.Copy(b, gray, 0, w * h);

                int graycount = gray.Length;

                byte min = gray.Min();

                Bitmap bitmap2 = new Bitmap(w, h, PixelFormat.Format32bppRgb);
                Rectangle rect2 = new Rectangle(0, 0, w, h);
                BitmapData bitmapData2 = bitmap2.LockBits(rect2, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

                unsafe
                { 
                    byte* bptr2 = (byte*)bitmapData2.Scan0;

                    for (int i = 0; i < w * h; i++) 
                    {
                        byte _gray = (byte)(gray[i]);

                        bptr2[i * 4 ] = (byte)(_gray - min);
                        bptr2[i * 4 + 1] = (byte)(_gray - min); ;
                        bptr2[i * 4 + 2] = (byte)(_gray - min);
                        bptr2[i * 4 + 3] = 255;
                    }
                }

                bitmap2.UnlockBits(bitmapData2);

                return bitmap2;

            }
        }
    }
}
