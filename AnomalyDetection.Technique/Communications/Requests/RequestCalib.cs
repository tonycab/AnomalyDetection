using AnomalyDetection.Technique.Communications.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Requests
{
    public class RequestCalib
    {
        public string Command { get; set; } = "/calib";
        public bool Activate { get; set; }
     
        public RequestCalib() { }

        public RequestCalib(string commande)
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

        public RequestCalib(bool activate)
        {
            Activate = Activate;
        }


    }
}
