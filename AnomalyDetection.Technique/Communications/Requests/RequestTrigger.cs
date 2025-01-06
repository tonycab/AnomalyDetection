using AnomalyDetection.Technique.Communications.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Requests
{
    public class RequestTrigger
    {
        public string Command { get; set; } = "/trigger";
        public int Ticket { get; set; }
        public string Program { get; set; }
        public int Model { get; set; }


        public RequestTrigger() { }

        public RequestTrigger(string commande)
        {
            var c = commande.Split(';');
            try
            {

                Ticket = int.Parse(c[1]);

                Program = c[2];
                if (Program == "") throw new Exception();

                Model = int.Parse(c[3]);
            }
            catch (Exception e)
            {

                throw new InvalidArgRequestException($"{Command} : {e.Message}");
            }

        }

        public RequestTrigger(int ticket, string program, int model)
        {
            Ticket = ticket;
            Program = program;
            Model = model;
        }


    }
}
