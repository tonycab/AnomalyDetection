using AnomalyDetection.Technique.Communications.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Technique.Communications.Requests
{
    public class RequestCalibStep
    {
        public string Command { get; set; } = "/calib_pose";
        public int Ticket { get; set; }
        public int PoseNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
     
        public RequestCalibStep() { }

        public RequestCalibStep(string commande)
        {
            var c = commande.Split(';');

            try
            {
                Ticket = int.Parse(c[1]);
                PoseNumber = int.Parse(c[2]);
                X = double.Parse(c[3],CultureInfo.InvariantCulture);
                Y = double.Parse(c[4], CultureInfo.InvariantCulture);
                Z = double.Parse(c[5], CultureInfo.InvariantCulture);
                Rx = double.Parse(c[6], CultureInfo.InvariantCulture);
                Ry = double.Parse(c[7], CultureInfo.InvariantCulture);
                Rz = double.Parse(c[8], CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                //Erreur d'argument
                throw new InvalidArgRequestException($"{Command} : {e.Message}");
            }
        }

        public RequestCalibStep(int ticket, int numberPose, double x, double y, double z, double rx, double ry, double rz)
        {
            Ticket = ticket;
            PoseNumber = numberPose;
            X = x;
            Y = y;
            Z = z;
            Rx = rx;
            Ry= ry;
            Rz = rz;
        }
    }
}
