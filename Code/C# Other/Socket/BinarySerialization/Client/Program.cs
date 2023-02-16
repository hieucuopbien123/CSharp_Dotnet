using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            while (true)
            {
                Console.Write("Press enter to send ... ");
                Console.ReadLine();
                var student = new Student
                {
                    Id = 1,
                    FirstName = "Nguyen Van",
                    LastName = "A",
                    DateOfBirth = new DateTime(1990, 12, 30)
                };
                var client = new TcpClient();
                client.Connect(IPAddress.Loopback, 1308);
                var stream = client.GetStream();
                var writer = new BinaryWriter(stream);
                var data = BinarySerializer.Serialize(student);
                writer.Write(data.Length);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                client.Close();
            }
        }
    }
}