using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.TraitementImage.Resultat
{

    public enum EnumState
    {

        NONE = 0,
        FAIL = 1,
        PASS = 2,
    }

    public abstract class Result
    {

        public string Date { get; set; }
        public EnumState State { get; set; }
        public double Score { get; set; }
        public Bitmap Image { get; set; }


        public Result(EnumState state)
        {
            Date = DateTime.Now.ToString();
            State = state;
        }

        public Result(EnumState state, double score)
        {
            Date = DateTime.Now.ToString();
            State = state;
            Score = score;
        }

        public Result(EnumState state, double score, Bitmap image)
        {
            Date = DateTime.Now.ToString();
            State = state;
            Score = score;
            Image = image;
        }

    }
}
