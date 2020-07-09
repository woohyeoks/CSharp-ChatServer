using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
           

            Listener listener = new Listener();
            listener.Start();

            while (true)
            {
            }

        }
    }
}
