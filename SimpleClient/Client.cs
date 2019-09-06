using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Client
{
    class Client
    {

        private NetClient client;

        public void StartClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("chat");
            config.AutoFlushSendQueue = false;

            client = new NetClient(config);
            client.Start();
            //Insert server IP address below: 10.0.1.8
            client.Connect("10.0.1.8", 14242);
        }

        public void SendMessage(string text)
        {
            NetOutgoingMessage message = client.CreateMessage(text);

            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
        }

        public void ConstantUpdateMsg()
        {
            while (true)
            {
                GotMessage();
            }
        }

        public void GotMessage()
        {
            NetIncomingMessage message;
            Console.Write("");

 
                    message = client.ReadMessage();
                    if (message != null)
                    {
                        switch (message.MessageType)
                        {
                            case NetIncomingMessageType.DebugMessage:
                            case NetIncomingMessageType.ErrorMessage:
                            case NetIncomingMessageType.WarningMessage:
                            case NetIncomingMessageType.VerboseDebugMessage:
                            case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                                Console.WriteLine("Status changed to " + status.ToString());
                                break;
                            case NetIncomingMessageType.Data:
                                {
                                    var data = message.ReadString();
                                    Console.WriteLine(data);

                                    if (data == "exit")
                                    {
                                        //TODO: exit later
                                    }

                                    break;
                                }
                            default:
                                Console.WriteLine("Unhandled message type: {message.MessageType}");
                                break;
                        }
                    }
         }
            

        public void Disconnect()
        {
            client.Disconnect("Bye");
        }
    }
}

