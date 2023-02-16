using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Tcp Client";
            Console.Write("Server IP address: ");
            var serverIpStr = Console.ReadLine();
            var serverIp = IPAddress.Parse(serverIpStr);
            var serverPort = 1308;
            var serverEndpoint = new IPEndPoint(serverIp, serverPort);
            var size = 1024;
            while (true)
            {
                Console.Write("# Text >>> ");
                var text = Console.ReadLine();
                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndpoint);
                var sendBuffer = Encoding.ASCII.GetBytes(text);
                socket.Send(sendBuffer);
                socket.Shutdown(SocketShutdown.Send);
                var receiveBuffer = new byte[size];
                var length = socket.Receive(receiveBuffer);
                var result = Encoding.ASCII.GetString(receiveBuffer, 0, length);
                socket.Close();
                Console.WriteLine($">>> {result}");
            }
        }
    }
}