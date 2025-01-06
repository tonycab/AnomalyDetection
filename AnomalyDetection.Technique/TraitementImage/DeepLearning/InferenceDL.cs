using AnomalyDetection.Technique.Acquisitions;
using AnomalyDetection.Technique.ModelsDL;
using AnomalyDetection.Technique.TraitementImage.Resultat;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Metier.TraitementImage.DeepLearning
{
    public class InferenceDL
    {
        static Random random = new Random();
        public static ResultInference Execute (Image2D image, Model model)
        {

            //Fonction inférence à implémenter


            
            //Simulation du resultat
            var score = Math.Round(random.NextDouble(), 2);

            return new ResultInference(score > 0.5 ? EnumState.PASS : EnumState.FAIL, score, image.ToBitmap(45));
        }

    }
}
