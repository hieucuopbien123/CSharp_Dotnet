using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "Server";
            var listener = new TcpListener(IPAddress.Any, 1308);
            listener.Start(10);
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var stream = client.GetStream();
                var reader = new StreamReader(stream);
                var data = reader.ReadLine();
                var student = TextSerializer.Deserialize(data);
                Console.WriteLine($"Raw data:\r\n{data}\r\n");
                Console.WriteLine("Deserialized object:");
                Console.WriteLine($"Id: {student.Id}\r\nFirst Name: {student.FirstName}\r\nLast Name: {student.LastName}\r\nDate of birth: {student.DateOfBirth.ToShortDateString()}");
                client.Close();
            }
        }
    }
}