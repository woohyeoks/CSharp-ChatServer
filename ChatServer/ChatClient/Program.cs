using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ipaddress, 8000);
            

            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Thread.Sleep(1000);
                socket.Connect(endPoint);
                byte[] bytes = new byte[1024];
                int numbyte = socket.Receive(bytes);
                Console.WriteLine(Encoding.Default.GetString(bytes));
                socket.Close();
            }
        }
    }
}
