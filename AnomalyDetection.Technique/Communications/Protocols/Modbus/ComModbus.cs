using AnomalyDetection.Technique.Communications.Requests;
using AnomalyDetection.Technique.Communications.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NModbus;

namespace AnomalyDetection.Technique.Communications.Protocols.Modbus
{
    public class ComModbus : ICom
    {

        public event Action<int, string, string> EvtTrigger2D;
        public event Action<string, string, string> EvtTrigger3D;
        public event Action<string, string, string> EvtTrigger3Drapide;
        public event Action<string, string, string> EvtDataTrigger;
        public event Action<string, string> EvtLoadCampaign;
        public event Action EvtLoadFreeRunStart;
        public event Action EvtLoadFreeRunStop;
        public event Action<string[]> EvtCalibPose;
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

        public event Func<RequestTrigger, ResultTrigger> EvtCmdTrigger3D;
        public event Func<RequestLoad, ResultLoad> EvtCmdLoad;
        public event Func<RequestFreeRun, ResultFreeRun> EvtCmdFreeRun;

        public TcpListener slaveTcpListener { get; set; }
        public IModbusSlaveNetwork Slave { get; set; }
        public IModbusSlave Table { get; set; }

        public string IPadress { get; set; }
        public bool StateModbus { get; set; }

        private ModbusFactory fmodbus = new ModbusFactory();

        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public ComModbus(string ipAddress)
        {
            IPadress = ipAddress;
        }

        public void Start()
        {
            Task.Run(() => StartModbus(IPadress));
        }

        public void Stop()
        {
            cancellationToken.Cancel();
            StateModbus = false;
        }

        private async void StartModbus(string ipAddress)
        {

            StateModbus = true;


            IPAddress address = IPAddress.Parse(ipAddress);
            Byte[] bytes = address.GetAddressBytes();
            IPAddress ipadresse = new IPAddress(bytes);
            IPEndPoint ipEndPoint = new IPEndPoint(ipadresse, 502);
            slaveTcpListener = new TcpListener(ipEndPoint);

            slaveTcpListener.Start();

            //Création d'un SlaveModbus
            Slave = fmodbus.CreateSlaveNetwork(slaveTcpListener);

            //Creation de la table modbus
            Table = fmodbus.CreateSlave(1);

            //Lance une Task de surveillance de la table modbus
            Task.Run(() => viewRegisterValue(Table));

            //Ajout de la table modbus 
            Slave.AddSlave(Table);

            try
            {
                using (cancellationToken.Token.Register(() => StateModbus = false))
                {
                    //Attente de la fin de la connection
                    Slave.ListenAsync(cancellationToken.Token).GetAwaiter().GetResult();
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Contient les valeurs précedente de la table modbus (Sert à détecter les changement de valeurs)
        public  ushort[] Old_RegisterInput;

        public ushort[] RegisterOutput = new ushort[127];

        //Affichage des valeurs de registe
        public async void viewRegisterValue(IModbusSlave slave)
        {

            while (StateModbus)
            {

                ushort[] registreInput = slave.DataStore.HoldingRegisters.ReadPoints(128, 128);

                if (Old_RegisterInput == null)
                {

                    Old_RegisterInput = registreInput;

                }


                //Vision Operationnelle
                RegisterOutput[0].SetBitValue(0);

                //Vision prête 
                RegisterOutput[0].SetBitValue(2);

                slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);


                //Bit de vie
                if (registreInput[0].GetBitValue(15)==true)
                {
                    RegisterOutput[0].SetBitValue(1);
                    slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);
                }
                else
                {
                    RegisterOutput[0].ResetBitValue(1);
                    slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);
                }



                //Trigger localisation pièce
                if (Old_RegisterInput[0].GetBitValue(0) != registreInput[0].GetBitValue(0) && registreInput[0].GetBitValue(0) == true)
                {

                    //Vision non prête 
                    RegisterOutput[0].ResetBitValue(2);

                    //Prise de vue reçu
                    RegisterOutput[0].SetBitValue(0);
                    slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);

 
                    //Traitement
                    ResultTrigger r = EvtCmdTrigger3D?.Invoke(new RequestTrigger { Ticket = registreInput[2], Program = registreInput[1].ToString(), Model = registreInput[3]});


                    //Prise de vue terminé
                    RegisterOutput[0].SetBitValue(4);
                    slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);

                    //Donnee prete
                    RegisterOutput[4] =  (ushort) int.Parse(r.Program);
                    RegisterOutput[5] = (ushort)r.Ticket;
                    
                    RegisterOutput[6] = (ushort)r.Model;
                    RegisterOutput[7] = (ushort)r.State;
                   
                    RegisterOutput[8] = (ushort)Math.Abs(r.X);
                    RegisterOutput[9] = (ushort)((int)Math.Abs(r.X)>>8);

                    RegisterOutput[10] = (ushort)Math.Abs(r.Y);
                    RegisterOutput[11] = (ushort)((int)Math.Abs(r.Y) >> 8);

                    RegisterOutput[12] = (ushort)Math.Abs(r.Z);
                    RegisterOutput[12] = (ushort)((int)Math.Abs(r.Z) >> 8);

                    RegisterOutput[14] = (ushort)Math.Abs(r.Rx);
                    RegisterOutput[14] = (ushort)((int)Math.Abs(r.Rx) >> 8);

                    RegisterOutput[16] = (ushort)Math.Abs(r.Ry);
                    RegisterOutput[16] = (ushort)((int)Math.Abs(r.Ry) >> 8);

                    RegisterOutput[18] = (ushort)Math.Abs(r.Rz);
                    RegisterOutput[18] = (ushort)((int)Math.Abs(r.Rz) >> 8);

                    if (r.X >= 0) { RegisterOutput[23].SetBitValue(0); } else { RegisterOutput[23].ResetBitValue(0);}
                    if (r.Y >= 0) { RegisterOutput[23].SetBitValue(1); } else { RegisterOutput[23].ResetBitValue(1);}
                    if (r.Z >= 0) { RegisterOutput[23].SetBitValue(2); } else { RegisterOutput[23].ResetBitValue(2);} 
                    if (r.Rx >= 0) { RegisterOutput[23].SetBitValue(3); } else { RegisterOutput[23].ResetBitValue(3);}
                    if (r.Ry >= 0) { RegisterOutput[23].SetBitValue(4); } else { RegisterOutput[23].ResetBitValue(4);}
                    if (r.Rz >= 0) { RegisterOutput[23].SetBitValue(5); } else { RegisterOutput[23].ResetBitValue(5);}

                    RegisterOutput[5] = (ushort)r.Score;

                    //Donnée prètes
                    RegisterOutput[0].SetBitValue(5);
                    slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);

                }
                else if(registreInput[0].GetBitValue(0) == false)
                {

                    //Prise de vue reçu
                    RegisterOutput[0].SetBitValue(3);

                    //Fin prise de vue en cours
                    RegisterOutput[0].ResetBitValue(4);
                    
                    //Fin données prêtes
                    RegisterOutput[0].ResetBitValue(5);

                    Console.WriteLine(RegisterOutput[0].GetBitValue(0));

                    slave.DataStore.HoldingRegisters.WritePoints(0, RegisterOutput);

                }


                Old_RegisterInput = registreInput;

                await Task.Delay(100);
            }
        }

        public void SendMessageTCP(string message)
        {
            throw new NotImplementedException();
        }
    }
}
