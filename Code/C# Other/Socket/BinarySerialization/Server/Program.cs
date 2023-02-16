using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            var listener = new TcpListener(IPAddress.Any, 1308);
            listener.Start(10);
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var stream = client.GetStream();
                var reader = new BinaryReader(stream);
                var length = reader.ReadInt32();
                var data = reader.ReadBytes(length);
                var student = BinarySerializer.Deserialize(data);
                client.Close();
                Console.WriteLine("Raw byte array:");
                foreach (var b in data)
                    Console.Write($"{b} ");
                Console.WriteLine("\r\nDeserialized object:");
                Console.WriteLine($"Id: {student.Id}\r\nFirst Name: {student.FirstName}\r\nLast Name: {student.LastName}\r\nDate of birth: {student.DateOfBirth.ToShortDateString()}");
            }
        }
    }
}