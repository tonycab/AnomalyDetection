using AnomalyDetection.Technique.Acquisitions;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnomalyDetection.Technique.ModelsDL
{
    public class Model
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string ModelHDLDirectory { get; set; }
        public double SegmentationThreshold { get; set; }
        public double ClassificationThreshold { get; set; }
        private HTuple modelHDL { get; set; }

        public Model() { }

        public Model(string name, string description,string  modelHDLDirectory, double segmentationThreshold,double classificationThreshold)
        {
            Name = name;
            Description = description;
            ModelHDLDirectory = modelHDLDirectory;
            SegmentationThreshold = segmentationThreshold;
            ClassificationThreshold = classificationThreshold;
        }


        public Model GetModelHDL(bool Reload = false)
        {
            if(modelHDL == null || Reload)
                try { 

                //Simulation chargement de modele
                Thread.Sleep(4000);

                HOperatorSet.ReadDlModel(ModelHDLDirectory, out HTuple modelHDL);

                }catch(Exception e) {
                    throw new Exception("Erreur fichier modele");
                }

            return this;
        }

        public XElement ToXElement()
        {
            XElement xElement = new XElement(nameof(Model));
            xElement.SetElementValue(nameof(Name), Name);
            xElement.SetElementValue(nameof(Description), Description);
            xElement.SetElementValue(nameof(ModelHDLDirectory), ModelHDLDirectory);
            xElement.SetElementValue(nameof(SegmentationThreshold), SegmentationThreshold.ToString(CultureInfo.InvariantCulture));
            xElement.SetElementValue(nameof(ClassificationThreshold), ClassificationThreshold.ToString(CultureInfo.InvariantCulture));

            return xElement;
        }

        public static Model FromXElement(XElement xElement)
        {
            Model model = new Model();

            model.Name = xElement.Element(nameof(Name))?.Value;
            model.Description = xElement.Element(nameof(Description))?.Value;
            model.ModelHDLDirectory = xElement.Element(nameof(ModelHDLDirectory))?.Value;
            model.SegmentationThreshold = double.Parse(xElement.Element(nameof(SegmentationThreshold))?.Value, CultureInfo.InvariantCulture);
            model.ClassificationThreshold = double.Parse(xElement.Element(nameof(ClassificationThreshold))?.Value, CultureInfo.InvariantCulture);

            return model;
        }

        public static implicit operator string(Model model) => $"{model.Name} - {model.Description}";

    }
}
