using System;
using System.Net;
using System.Collections.Generic;
namespace OtherLib
{
    class Program
    {
        static void Main(string[] args)
        {
            // Lấy tên máy cục bộ
            var hostName = Dns.GetHostName();
            Console.WriteLine($"Local host name: {hostName}");

            // Lấy address v4 và v6 của host
            var addresses = Dns.GetHostAddresses("google.com");
            Console.WriteLine("Addresses:");
            foreach (var a in addresses)
            {
                Console.WriteLine(a);
            }

            // Lấy tất cả dạng object về 1 host name: {HostName, AddressList, Aliases}
            var entry = Dns.GetHostEntry("google.com");

            Console.WriteLine("HostEntry of google.com");
            Console.WriteLine($"Host name: {entry.HostName}");
            Console.WriteLine("Addresses:");
            foreach (var a in entry.AddressList)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine("Aliases");
            foreach (var s in entry.Aliases)
            {
                Console.WriteLine(s);
            }


            short aShort = 256;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(aShort));
            Console.WriteLine($"short: {aShort}, {sizeof(short)} byte(s)");
            Console.WriteLine(bytes.ToArray().Length); // Socket send hay Receive đều là byte[]


            Console.ReadLine();
        }
    }
}