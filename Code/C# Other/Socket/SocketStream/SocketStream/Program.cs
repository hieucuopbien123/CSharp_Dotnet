using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace Client
{
    class Program
    {
        // Socket Stream
        // => K đem lại hiệu quả gì khi dùng stream như dưới
        static void Main(string[] args)
        {
            Console.Title = "Tcp Client";
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("Server Ip: ");
            var address = IPAddress.Parse(Console.ReadLine());
            var serverEndpoint = new IPEndPoint(address, 1308);
            while (true)
            {
                Console.Write("# Command > ");
                var request = Console.ReadLine();
                var client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.Connect(serverEndpoint);
                var stream = new NetworkStream(client);

                var sendBuffer = Encoding.UTF8.GetBytes(request + "\r\n"); // tự thêm \r\n vào cuối chuỗi truy vấn
                // ghi mảng byte vào stream thay vì dùng phương thức Send của Socket
                stream.Write(sendBuffer, 0, sendBuffer.Length);
                
                stream.Flush(); 

                var receiveBuffer = new byte[1024];
                // đọc mảng byte từ stream thay vì dùng phương thức Receive của Socket
                var count = stream.Read(receiveBuffer, 0, 1024);
                var response = Encoding.UTF8.GetString(receiveBuffer, 0, count);

                // cắt bỏ các ký tự thừa \r\n ở cuối chuỗi phản hồi trước khi in vì dùng WriteLine ở server
                Console.WriteLine(response.Trim()); 

                // Có thể gọi lệnh stream.Close() thay vì đóng socket. Khi đóng luồng mạng cũng sẽ tự đóng socket bên dưới
                client.Close();
            }
        }
    }
}