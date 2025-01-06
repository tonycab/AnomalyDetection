using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnomalyDetection.Technique.Communications.Results
{
    public class ResultTrigger : Result
    {
        public string Command { get; set; } = "/trigger";
        public int Ticket { get; set; }
        public string Program { get; set; }
        public int Model { get; set; }
        public State State { get; set; }
        public double Score { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
        public List<ExtensionResult> Extensions { get; set; }

        public ResultTrigger() { }

        public ResultTrigger(int ticket, string program, int model, State state)
        {
            Ticket = ticket;
            Program = program;
            Model = model;
            State = state;
        }

        public ResultTrigger(int ticket, string program, int model, State state, double score, double x, double y, double z, double rx, double ry, double rz, List<ExtensionResult> extension = null )
        {
            Ticket = ticket;
            Program = program;
            Model = model;
            State = state;
            Score = score;
            X = x;
            Y = y;
            Z = z;
            Rx = rx;
            Ry = ry;
            Rz = rz;

            if ( extension != null )
            {

            }
        }

        public override string ToCsvResult(char delimiter =';')
        {
            string strExtensions = "";
            if (Extensions != null)
                 strExtensions = String.Join(delimiter.ToString(), Extensions?.Select((p) => p.ToCsv()).ToArray());

            return $"{Command}{delimiter}" +
                $"{Ticket}{delimiter}" +
                $"{Program}{delimiter}" +
                $"{Model}{delimiter}" +
                $"{State}{delimiter}" +
                $"{Score}{delimiter}" +
                $"{X}{delimiter}" +
                $"{Y}{delimiter}" +
                $"{Z}{delimiter}" +
                $"{Rx}delimiter" +
                $"{Ry}delimiter" +
                $"{Rz}delimiter" +
                $"{strExtensions}";
        }
        //public void Invoke(Action<ResultTrigger> action)
        //{
        //    if (action== null)
        //        action(this);
        //}
    }
}
