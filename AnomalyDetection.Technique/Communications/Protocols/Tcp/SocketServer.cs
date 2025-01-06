using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AnomalyDetection.Technique.Communications.Protocols.Tcp
{
    /// <summary>
    /// Class permettant de lancer un server TCP
    /// </summary>
    public class SocketServer
    {
        /// <summary>
        /// Etat du server
        /// </summary>
        public bool ServerState { get; private set; }


        /// <summary>
        /// Appeler lorsque des données on été recu
        /// </summary>
        public event Func<object, String, String> DataRecieve;
        /// <summary>
        /// Appeler lorsque qu'un client est connecté
        /// </summary>
        public event Action<Socket> AddClient;
        /// <summary>
        /// Appeler lorqu'un client est déconnecté
        /// </summary>
        public event Action<Socket> RemoveClient;
        /// <summary>
        /// Appeler lorsque le status du server change
        /// </summary>
        public event Action<bool> StateServer;

        private List<Socket> listSocketClient = new List<Socket>();
        /// <summary>
        /// Liste de client connecté
        /// </summary>
        public ReadOnlyCollection<Socket> ListSocketClient => listSocketClient.ToList().AsReadOnly();

        private Socket socket;

        private IPEndPoint localEndPoint;


        /// <summary>
        /// Methode pour lance un server TCP
        /// </summary>
        /// <param name="adresseIP"></param>
        /// <param name="Port"></param>
        public async void StartServer(string adressIP, int Port)
        {
            if (ServerState) return;

            IPAddress IPadress = IPAddress.Parse(adressIP);

            //Creation du socket
            localEndPoint = new IPEndPoint(IPadress, Port);
            socket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            //new Thread(() => { AcceptClient(listener); }).Start();
            await Task.Run(() => AcceptClient(socket));

        }

        /// <summary>
        /// Methode pour arrêter le server TCP
        /// </summary>
        public void StopServer()
        {
            try
            {

                foreach (var c in listSocketClient)
                {
                    c.Close();
                }

                socket?.Close();
                ServerState = false;

            }
            catch { }
        }

        private void AcceptClient(Socket socketServer)
        {
            try
            {
                //Server démarrer
                ServerState = true;
                StateServer?.Invoke(ServerState);

                socketServer.Bind(localEndPoint);
                socketServer.Listen(10);

                while (ServerState)
                {

                    //Attente d'un nouveau client
                    Socket socketClient = socketServer.Accept();

                    listSocketClient.Add(socketClient);

                    //Lance un task d'échange avec le client
                    Task.Run(() => Client(socketClient));
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);

            }
            finally
            {

                socketServer.Close();

                ServerState = false;
                StateServer?.Invoke(ServerState);
            }
        }

        private void Client(Socket socketClient)
        {

            //Appel évennement nouveau client
            AddClient?.Invoke(socketClient);

            try
            {
                while (ServerState)
                {

                    string data = null;

                    //Reception des données jusqu'a détection du ; et retour à la ligne
                    while (socketClient.Connected)
                    {
                        byte[] bytes = new byte[1024];

                        int bytesRec = socketClient.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        if (data.Length == 0) break /* throw new Exception()*/;


                        if (data.EndsWith(";"))
                        {
                            break;
                        }

                        //if (data.EndsWith(";\r\n"))
                        //{
                        //    break;
                        //}
                    }

                    //Appel évenement donnée reçu
                    string result = DataRecieve?.Invoke((object)socketClient, data);

                    if (result == null) result = string.Empty;

                    byte[] msg = Encoding.ASCII.GetBytes(result);

                    socketClient.Send(msg);

                }
            }
            catch { }
            finally
            {

                listSocketClient.Remove(socketClient);
                RemoveClient?.Invoke(socketClient);
            }

        }

    }
}
