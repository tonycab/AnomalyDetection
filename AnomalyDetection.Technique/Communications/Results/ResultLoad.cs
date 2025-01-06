using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnomalyDetection.Technique.Communications.Results
{
    public class ResultLoad : Result
    {
        public string Command { get; set; } = "/ref";
        public string Program { get; set; }
        public int Model { get; set; }
        public State State { get; set; }

        public ResultLoad() { }

        public ResultLoad( string program, int model, State state)
        {
            Program = program;
            Model = model;
            State = state;
        }

        public override string ToCsvResult(char delimiter =';')
        {

            return $"{Command}{delimiter}" +
                $"{Program}{delimiter}" +
                $"{Model}{delimiter}" +
                $"{State}{delimiter}";
               
        }
    }
}
