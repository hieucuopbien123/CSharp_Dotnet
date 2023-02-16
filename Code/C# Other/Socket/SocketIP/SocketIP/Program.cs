using System;
using System.Net;
namespace Ping
{
    internal class Program
    {
        // # Tạo 1 socket IP
        private static void Main(string[] args)
        {
            Console.Title = "My Ping";
            IPAddress getIp(string arg)
            {
                // Lấy địa chỉ IP dạng số từ string
                if (IPAddress.TryParse(arg, out IPAddress ip)) return ip;
                else throw new Exception("Invalid Ip address");
            }
            var ping = new Ping() { Log = Console.WriteLine };
            while (true)
            {
                Console.Write("Ip (ping thu google la 8.8.8.8) > ");
                var ip = Console.ReadLine();
                ping.Run(getIp(ip));
                Console.WriteLine();
            }

        }
    }
}