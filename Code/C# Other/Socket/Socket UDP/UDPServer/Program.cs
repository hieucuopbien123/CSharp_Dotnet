using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Server
{
    internal class Program
    {
        //Các giao thức bậc cao wrap Socket
        private static void Main()
        {
            var server = new UdpClient(1308); // tự động bind với cổng
            while (true)
            {
                var remoteEp = new IPEndPoint(0, 0);
                var buffer = server.Receive(ref remoteEp);
                var text = Encoding.ASCII.GetString(buffer);
                var response = text.ToUpper(); // Convert sang string để xử lý upppercase r lại convert lại bytes
                buffer = Encoding.ASCII.GetBytes(response);
                server.Send(buffer, buffer.Length, remoteEp);
            }
        }
    }
}