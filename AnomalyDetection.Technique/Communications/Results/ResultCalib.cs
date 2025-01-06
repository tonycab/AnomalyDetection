using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnomalyDetection.Technique.Communications.Results
{
    public class ResultCalib : Result
    {
        public string Command { get; set; } = "/calib";
        public bool Activate { get; set; }
        public State State { get; set; }

        public ResultCalib() { }

        public ResultCalib( bool activate, State state)
        {
            Activate = activate;
            State = state;
        }

        public override string ToCsvResult(char delimiter =';')
        {
            string mode = Activate == true ? "START" : "STOP";

            return $"{Command}{delimiter}" +
                $"{mode}{delimiter}" +
                $"{State}{delimiter}";
               
        }
    }
}
