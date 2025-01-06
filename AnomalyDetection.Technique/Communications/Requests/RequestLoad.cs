using AnomalyDetection.Technique.Communications.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Requests
{
    public class RequestLoad
    {
        public string Command { get; set; } = "/ref";
        public string Program { get; set; }
        public int Model { get; set; }


        public RequestLoad() { }

        public RequestLoad(string commande)
        {
            var c = commande.Split(';');
            try
            {

                Program = c[1];
                if (Program == "") throw new Exception();

                Model = int.Parse(c[2]);
            }
            catch (Exception e)
            {

                throw new InvalidArgRequestException($"{Command} : {e.Message}");
            }

        }

        public RequestLoad(int ticket, string program, int model)
        {
            Program = program;
            Model = model;
        }


    }
}
