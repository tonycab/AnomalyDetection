using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnomalyDetection.Technique.Communications.Results
{
    public class ResultCalibStep : Result
    {
        public string Command { get; set; } = "/calib_pose";
        public int Ticket { get; set; }
        public State State { get; set; }

        public ResultCalibStep() { }

        public ResultCalibStep( int ticket, State state)
        {
            Ticket = ticket;
            State = state;
        }

        public override string ToCsvResult(char delimiter =';')
        {
         
            return $"{Command}{delimiter}" +
                $"{Ticket}{delimiter}" +
                $"{State}{delimiter}";
               
        }
    }
}
