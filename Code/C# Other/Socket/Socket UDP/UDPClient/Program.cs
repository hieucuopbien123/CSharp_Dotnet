using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    internal class Program
    {
        // Các giao thức bậc cao wrap Socket / UdpClient
        private static void Main()
        {
            Console.Write("Server Ip: ");
            var ip = IPAddress.Parse(Console.ReadLine());
            var client = new UdpClient();
            client.Connect(ip, 1308);
            while (true)
            {
                Console.Write("# Text >>> ");
                var text = Console.ReadLine();
                var buffer = Encoding.ASCII.GetBytes(text);
                client.Send(buffer, buffer.Length);
                var dumpEp = new IPEndPoint(0, 0);
                buffer = client.Receive(ref dumpEp);
                var response = Encoding.ASCII.GetString(buffer);
                Console.WriteLine(response);
            }
        }
    }
}