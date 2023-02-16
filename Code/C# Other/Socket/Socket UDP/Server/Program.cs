using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Server
{
    internal class Program
    {
        // # Socket / Dùng class Socket
        private static void Main(string[] args)
        {
            Console.Title = "Udp Server";
            var localIp = IPAddress.Any; // Any ứng với Ip của tất cả các giao diện mạng trên máy
            // Có Loopback
            Console.WriteLine(localIp); // 0.0.0.0 

            var localPort = 1308;
            var localEndPoint = new IPEndPoint(localIp, localPort);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localEndPoint);
            
            Console.WriteLine($"Local socket bind to {localEndPoint}. Waiting for request ...");

            var size = 1024;
            var receiveBuffer = new byte[size];
            while (true)
            {
                // Khởi tạo remote endpoint có thể là bất cứ địa chỉ nào và bất cứ cổng nào
                EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

                // Khi có client connect thì server nhận về data rồi lưu thông tin client vào remoteEndpoint
                var length = socket.ReceiveFrom(receiveBuffer, ref remoteEndpoint);

                Console.WriteLine("Catch");
                var text = Encoding.ASCII.GetString(receiveBuffer, 0, length);
                Console.WriteLine($"Received from {remoteEndpoint}: {text}");
                var result = text.ToUpper();
                var sendBuffer = Encoding.ASCII.GetBytes(result);

                socket.SendTo(sendBuffer, remoteEndpoint);
                Array.Clear(receiveBuffer, 0, size);
            }
        }
    }
}