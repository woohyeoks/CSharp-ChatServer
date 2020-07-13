using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Program
    {

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();
                session.Start(clientSocket);

                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");

                Console.WriteLine("[TO Client] Welcome to MMORPG Server!");
                session.Send(sendBuffer);

                Thread.Sleep(1000);
                session.Disconnect();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
           

            Listener listener = new Listener();
            listener.Start(OnAcceptHandler);

            while (true)
            {
            }

        }
    }
}
