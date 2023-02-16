using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
namespace Server
{
    internal class Program
    {
        //Các giao thức bậc cao wrap Socket
        private static void Main()
        {
            var listener = new TcpListener(IPAddress.Any, 1308);
            listener.Start(10);
            while (true)
            {
                // Nhận 1 đoạn text xong đóng connection luôn
                Console.WriteLine("New Connection");
                var client = listener.AcceptTcpClient();
                var stream = client.GetStream();
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream) { AutoFlush = true };
                var text = reader.ReadLine();
                var response = text.ToUpper();
                writer.WriteLine(response);
                client.Close();
            }
        }
    }
}