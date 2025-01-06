using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Results
{
    public abstract class ExtensionResult
    {
        public string Name { get; set; }
        public virtual string ToCsv(char delimiter =';')
        {
            return "";
        }

    }

    public  class ExtensionResultAttaque : ExtensionResult
    {
        public int Number { get; set; }

        public ExtensionResultAttaque(int number)
        {
            Name = "ATTAQUE";

            Number = number;
        }

        public override string ToCsv(char delimiter = ';')
        {
            return $"{Name}delimiter{Number}";
        }

    }



}
