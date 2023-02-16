using System;
using System.Collections.Generic;
using System.Net;
namespace Ping
{
    internal class Icmp
    {
        public byte Type { get; set; } = 0x08;
        public byte Code { get; set; } = 0x00;
        public ushort Checksum { get; set; } = 0;
        private ushort _identifier;
        public ushort Identifier
        {
            get => (ushort)IPAddress.NetworkToHostOrder((short)_identifier);
            set => _identifier = (ushort)IPAddress.HostToNetworkOrder((short)value);
        }
        private ushort _sequence;
        public ushort Sequence
        {
            get => (ushort)IPAddress.NetworkToHostOrder((short)_sequence);
            set => _sequence = (ushort)IPAddress.HostToNetworkOrder((short)value);
        }
        public byte[] Payload { get; set; }
        public int PayloadSize => Payload.Length;
        public Icmp()
        {
        }
        public Icmp(byte[] ipDgram, int ipDgramLength)
        {
            // Vc tách yêu cầu ta phải nắm rõ kích thước từng biến. Khi trả về nhận được cả kích thước chứa IP header
            // tốn 20 bytes nên các trường bắt đầu lấy từ buffer[20] trở đi
            Type = ipDgram[20]; // Thành công trả ra Echo Reply Type là 0
            Code = ipDgram[21]; // Code là 0
            Checksum = BitConverter.ToUInt16(ipDgram, 22);
            _identifier = BitConverter.ToUInt16(ipDgram, 24);
            _sequence = BitConverter.ToUInt16(ipDgram, 26);
            var payloadSize = ipDgramLength - 28;
            Payload = new byte[payloadSize];
            Array.Copy(ipDgram, 28, Payload, 0, payloadSize);
        }
        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            //Console.WriteLine("OK");
            //Console.WriteLine(Type);
            //Console.WriteLine(Code);
            //Console.WriteLine(Checksum);
            //Console.WriteLine(_identifier);
            //Console.WriteLine(_sequence);
            bytes.Add(Type);
            bytes.Add(Code);
            bytes.AddRange(BitConverter.GetBytes(Checksum));
            bytes.AddRange(BitConverter.GetBytes(_identifier));
            bytes.AddRange(BitConverter.GetBytes(_sequence));
            bytes.AddRange(Payload);
            return bytes.ToArray();
        }
        public static void SetChecksum(Icmp packet)
        {
            packet.Checksum = 0;
            uint checksum = 0;
            byte[] data = packet.GetBytes();
            for (int i = 0; i < data.Length; i += 2)
            {
                checksum += Convert.ToUInt32(BitConverter.ToUInt16(data, i));
            }
            checksum = (checksum >> 16) + (checksum & 0xffff);
            checksum += (checksum >> 16);
            packet.Checksum = (ushort)(~checksum);
        }
    }
}