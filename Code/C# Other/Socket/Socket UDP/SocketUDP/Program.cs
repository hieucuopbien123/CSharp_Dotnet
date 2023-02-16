using System;
using System.Net; // để sử dụng lớp IPAddress, IPEndPoint
using System.Net.Sockets; // để sử dụng lớp Socket
using System.Text; // để sử dụng lớp Encoding
namespace Client
{
    internal class Program
    {
        // # Socket / Dùng class Socket
        // # Basic / Dùng ref
        private static void Main(string[] args)
        {
            Console.Title = "Udp Client";
            Console.Write("Server IP address: ");
            var serverIpStr = Console.ReadLine();
            var serverIp = IPAddress.Parse(serverIpStr);
            Console.Write("Server port: ");
            var serverPortStr = Console.ReadLine();
            var serverPort = int.Parse(serverPortStr);

            var serverEndpoint = new IPEndPoint(serverIp, serverPort);
            var size = 1024; // quá 1024 bytes sẽ bị cut
            var receiveBuffer = new byte[size];    

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("# Text >>> ");
                Console.ResetColor();
                var text = Console.ReadLine();

                var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                var sendBuffer = Encoding.ASCII.GetBytes(text);
                socket.SendTo(sendBuffer, serverEndpoint); // Gắn vào data chuỗi header 8 bytes r truyền tới chương trình 
                // chạy giao thức IP là dạng datagram

                // Socket phía client ở đây k cần phải bind để bắt UDP package từ mọi port như là server vì khi client tạo connection, 1 instance UDP mới sinh ra phía client, khi server nó vừa send gửi lại đúng cái remote endpoint mà socket này cung cấp, nó về đúng socket này luôn chứ client k cần phải bind. Còn phía server chả cần tạo 1 socket instance của client như TCP mà nhận từ đâu thì gửi lại đúng remote endpoint đó mà thôi
                EndPoint dummyEndpoint = new IPEndPoint(IPAddress.Any, 0);
                // Đối số 2 lưu địa chỉ của server nhưng k cần thiết ở đây vì biết bên trên r
                var length = socket.ReceiveFrom(receiveBuffer, ref dummyEndpoint);
                var result = Encoding.ASCII.GetString(receiveBuffer, 0, length);
                Array.Clear(receiveBuffer, 0, size); // Xóa bộ đệm (để lần sau sử dụng cho yên tâm)
                socket.Close();
                Console.WriteLine($">>> {result}");
            }
        }
    }
}