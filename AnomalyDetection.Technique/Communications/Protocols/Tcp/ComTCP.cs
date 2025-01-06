using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnomalyDetection.Technique.Communications.Requests;
using AnomalyDetection.Technique.Communications.Results;
using AnomalyDetection.Technique.Communications.Protocols.Tcp;
using AnomalyDetection.Technique.Communications;


namespace AnomalyDetection .Technique.Communications.Protocols.Tcp
{
    /// <summary>
    /// Class de communication Vision robot en TCP standard SIIF
    /// </summary>
    public class ComTCP : ICom
    {
        public SocketServer ServerTCP;

        public event Action<int, string, string> EvtTrigger2D;
        public event Action<string, string, string> EvtTrigger3D;
        public event Action<string, string, string> EvtTrigger3Drapide;
        public event Action<string, string, string> EvtDataTrigger;
        public event Action<string, string> EvtLoadCampaign;
        public event Action EvtLoadFreeRunStart;
        public event Action EvtLoadFreeRunStop;
        public event Action<String[]> EvtCalibPose;
        public event Action EvtCalibStart;
        public event Action EvtCalibStop;
        public event Action<string, string> EvtReconstruct3DStart;
        public event Action<string, string> EvtReconstructInPos;
        public event Action EvtReconstructStop;
        public event Action<string, string, string, string[]> EvtReconstructAdd;
        public event Action<string, string, string> EvtCalibAxeExtInPos;
        public event Action EvtCalibAxeExtStart;
        public event Action EvtCalibAxeExtStop;
        public event Action EvtConnectCam;
        public event Action EvtDisconnectCam;
        public event Action EvtGetTempPhoxi;
        public event Action EvtGetVisionState;
        public event Action EvtStateAcknoledgement;
        public event Action EvtRepetabiliteStart;
        public event Action EvtRepetabiliteStop;
        public event Action EvtBackgroundLearning;

        //ALC 
        public event Func<RequestTrigger, ResultTrigger> EvtCmdTrigger3D;
        public event Func<RequestLoad, ResultTrigger> EvtCmdLoad;
        public event Func<RequestFreeRun, ResultFreeRun> EvtCmdFreeRun;


        ///// <summary>
        ///// Evenement appelé lors de la réception d'une demande de chargement de programme
        ///// </summary>
        //public event Func<RequestLoad, ResultLoad> OnLoad;
        ///// <summary>
        ///// Evennement appelé lors de la réception d'une demande calibration
        ///// </summary>
        //public event Func<RequestCalib, ResultCalib> OnCalib;
        ///// <summary>
        ///// Evennement appelé lors de la réception d'une pose robot pour la calibration
        ///// </summary>
        //public event Func<RequestCalibPose, ResultCalibPose> OnCalibPose;

        private string ipAdress;
        /// <summary>
        /// Adresse IP utilisé pour la liaison
        /// </summary>
        public string IPAdress
        {
            get { return ipAdress; }
            set
            {
                ipAdress = value;
                ResetCom();
            }
        }
        private int port;

        /// <summary>
        /// Port utilisé pour la liaison
        /// </summary>
        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                ResetCom();
            }
        }
        private void ResetCom()
        {
            if (ServerTCP.ServerState)
            {
                ServerTCP?.StopServer();
                ServerTCP?.StartServer(IPAdress, Port);
            }
        }
        /// <summary>
        /// Instanciation d'un server TCP d'échange Vision robot
        /// </summary>
        /// <param name="ipAdress">Adresse IP de la liaison</param>
        /// <param name="port">Port de la liaison</param>
        public ComTCP(string ipAdress, int port)
        {
            ServerTCP = new SocketServer();

            IPAdress = ipAdress;
            Port = port;

            ServerTCP.DataRecieve += Request;

        }

        /// <summary>
        /// Stop le serveur TCP
        /// </summary>
        public void Stop()
        {
            if (ServerTCP.ServerState)
            {
                ServerTCP?.StopServer();
            }
        }
        /// <summary>
        /// Demarrer le server TCP
        /// </summary>
        public void Start()
        {
            ServerTCP.StartServer(IPAdress, Port);
        }

        /// <summary>
        /// Instanciation d'un server TCP d'échange Vision robot
        /// </summary>
        /// <param name="ipAdress"></param>
        /// <param name="port"></param>
        public ComTCP() : this("127.0.0.1", 1100) { }

        event Func<RequestLoad, ResultLoad> ICom.EvtCmdLoad
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        private string Request(object Sender, string Commande)
        {
            var cmd = Commande.Split(';')[0];
            
            try { 
            switch (cmd)
            {
                
                case "/trigger": return EvtCmdTrigger3D?.Invoke(new RequestTrigger(Commande)).ToCsvResult();
                case "/ref": return EvtCmdLoad?.Invoke(new RequestLoad(Commande)).ToCsvResult();
                case "/free_run": return EvtCmdFreeRun?.Invoke(new RequestFreeRun(Commande)).ToCsvResult();

                    //case "/calib": return OnCalib?.Invoke(new RequestCalib(Commande)).ToCsv();
                    //case "/calib_pose": return OnCalibPose?.Invoke(new RequestCalibPose(Commande)).ToCsv();

                    default: return $"Invalide command : {cmd}";
            }

            }catch(Exception e)
            {
                return $"{e.Message}";

            }
            

        }


        [Obsolete]
        public void SendMessageTCP(string message)
        {
            throw new NotImplementedException();
        }
    }
}
