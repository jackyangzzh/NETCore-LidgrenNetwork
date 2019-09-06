using System;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.StartClient();
            Console.WriteLine("Welcome to Holo's chat room!");

            //Threading to check for new messages
            Thread checkMessage = new Thread(client.ConstantUpdateMsg);
            checkMessage.Start();


            while (true)
            {

                
                Console.WriteLine("Type your message:");

                var text = Console.ReadLine();
                client.SendMessage(text);


                if (text == "exit")
                {
                    break;
                }

                //client.GotMessage();


            }

          

            client.Disconnect();
        }

    }
}
