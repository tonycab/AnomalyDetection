using SiiF_VISION.Technique.Communications.Requests;
using SiiF_VISION.Technique.Communications.Results;
using SimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiiF_VISION.Technique.Communications
{
    public interface ICom
    {

        event Action<int, string, string> EvtTrigger2D;

        event Action<string, string, string> EvtTrigger3D;

        //Pas prioritaire
        event Action<string, string, string> EvtTrigger3Drapide;
        event Action<string, string, string> EvtDataTrigger;

        event Action<string, string> EvtLoadCampaign;
        event Action EvtLoadFreeRunStart;
        event Action EvtLoadFreeRunStop;
        event Action<String[]> EvtCalibPose;
        event Action EvtCalibStart;
        event Action EvtCalibStop;

        
        event Action<string, string> EvtReconstruct3DStart;
        event Action<string, string> EvtReconstructInPos;
        event Action EvtReconstructStop;
        event Action<string, string, string, string[]> EvtReconstructAdd;
        event Action<string, string, string> EvtCalibAxeExtInPos;
        event Action EvtCalibAxeExtStart;
        event Action EvtCalibAxeExtStop;

        event Action EvtConnectCam;
        event Action EvtDisconnectCam;

        event Action EvtGetTempPhoxi;

        //supp
        event Action EvtGetVisionState;

        //supp
        event Action EvtStateAcknoledgement;

        //sipp
        event Action EvtRepetabiliteStart;
        event Action EvtRepetabiliteStop;
        //supp
        event Action EvtBackgroundLearning;

        //ALC 
        event Func<RequestTrigger, ResultTrigger> EvtCmdTrigger3D;
        event Func<RequestLoad, ResultLoad> EvtCmdLoad;
        event Func<RequestFreeRun, ResultFreeRun> EvtCmdFreeRun;


        void Start();
        void Stop();



        [Obsolete]
         void SendMessageTCP(string message);
  

    }
}
