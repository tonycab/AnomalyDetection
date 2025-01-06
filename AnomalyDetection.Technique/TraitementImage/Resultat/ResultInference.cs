using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.TraitementImage.Resultat
{




    public class ResultInference : Result
    {



        public ResultInference(EnumState state):base(state){ }

        public ResultInference(EnumState state, double score) : base(state, score) { }

        public ResultInference(EnumState state, double score, Bitmap image) : base(state, score, image) { }

    }
}
