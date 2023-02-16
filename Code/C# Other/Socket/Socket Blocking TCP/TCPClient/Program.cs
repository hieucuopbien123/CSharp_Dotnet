using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    internal class Program
    {
        //Các giao thức bậc cao wrap Socket
        private static void Main()
        {
            Console.Write("Server Ip: ");
            var ip = IPAddress.Parse(Console.ReadLine());
            while (true)
            {
                // Mỗi 1 lần gửi text là tạo ra 1 connection mới
                Console.Write("# Text >>> ");
                var text = Console.ReadLine();
                var client = new TcpClient();
                client.Connect(ip, 1308);

                // Nhờ có buffer nên TCP mới chơi kiểu luồng được, UDP thì k
                var stream = client.GetStream();
                var writer = new StreamWriter(stream) { AutoFlush = true };
                var reader = new StreamReader(stream);
                writer.WriteLine(text);
                var response = reader.ReadLine();
                client.Close();
                Console.WriteLine(response);
            }
        }
    }
}