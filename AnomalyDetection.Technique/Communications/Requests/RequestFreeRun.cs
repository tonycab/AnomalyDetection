using AnomalyDetection.Technique.Communications.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Requests
{
    public class RequestFreeRun
    {
        public string Command { get; set; } = "/free_run";
        public bool Activate { get; set; }
     

        public RequestFreeRun() { }

        public RequestFreeRun(string commande)
        {
            var c = commande.Split(';');

            try
            {
                if (c[1]=="START")
                {
                    Activate = true;
                }
                else if (c[1]=="STOP")
                {
                    Activate = false;
                }
                else
                {
                    throw new Exception();
                }
              
            }
            catch (Exception e)
            {
                //Erreur d'argument
                throw new InvalidArgRequestException($"{Command} : {e.Message}");
            }
        }

        public RequestFreeRun(bool activate)
        {
            Activate = Activate;
        }


    }
}
