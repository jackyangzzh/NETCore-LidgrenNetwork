using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Lidgren.Network;

namespace Server
{
    class Server
    {
        public NetServer server;
        private List<NetPeer> clients;

        public void StartServer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("chat");
            config.Port = 14242;
            server = new NetServer(config);
            server.Start();

            if (server.Status == NetPeerStatus.Running)
            {
                Console.WriteLine("Server is running on port " + config.Port);
            }
            else
            {
                Console.WriteLine("Server not started...");
            }
            clients = new List<NetPeer>();

        }

        public void ReadMessages()
        {
            NetIncomingMessage message;
            var stop = false;

            while (!stop)
            {
                message = server.ReadMessage();
                Console.Write("");
                if (message != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            {
                                Console.WriteLine(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " said: ");
                                var data = message.ReadString();
                                Console.WriteLine(data);

                                List<NetConnection> all = server.Connections;
                                all.Remove(message.SenderConnection);

                                if(all.Count > 0)
                                {
                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " said: " + data);
                                    server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);

                                }
                            
                                if (data == "exit")
                                {
                                    stop = true;
                                }

                                break;
                            }
                        case NetIncomingMessageType.DebugMessage:
                            Console.WriteLine(message.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            Console.WriteLine(message.SenderConnection.Status);
                            if (message.SenderConnection.Status == NetConnectionStatus.Connected)
                            {
                                clients.Add(message.SenderConnection.Peer);
                                Console.WriteLine("{0} has connected.", message.SenderConnection.Peer.Configuration.LocalAddress);
                            }
                            if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                            {
                                clients.Remove(message.SenderConnection.Peer);
                                Console.WriteLine("{0} has disconnected.", message.SenderConnection.Peer.Configuration.LocalAddress);
                            }
                            break;
                        default:
                            Console.WriteLine("Unhandled message type: {message.MessageType}");
                            break;
                    }
                    server.Recycle(message);
                }
            }

            Console.WriteLine("Shutdown package \"exit\" received. Press any key to finish shutdown");
            Console.ReadKey();
        }
    }
}
