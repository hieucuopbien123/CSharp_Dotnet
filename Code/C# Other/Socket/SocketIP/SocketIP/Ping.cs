using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace Ping
{
    internal class Ping
    {
        public Action<string> Log { get; set; }
        public void Run(IPAddress address, int loops = 4, int timeout = 3000, string message = "1234567890", int sleep = 1500)
        {
            // Tạo socket thô và endpoint để chuẩn bị ping tới
            var _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
            IPEndPoint remoteEP = new IPEndPoint(address, 0);
            EndPoint dumpEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = new byte[1024];
            var id = (ushort)Process.GetCurrentProcess().Id;

            /*
            Riêng giao thức ICMP yêu cầu phải làm thủ công như này chứ nó k làm hộ ta các trường header như TCP.
            Gói tin ta gửi đi phải có đúng dạng là 1 Icmp packet. Ta tạo ra 1 class chứa đủ các trường của 1 ICMP packet
            cần có và gửi đi thì chuyển sang bytes là được. Để mô phỏng được packet chỉ cần có đủ giá trị và kích thước
            phần giá trị đó, r convert sang bytes ghép lại là gửi đi được:
            Type: 0x08 => 8 là Echo Request, server trả lại Type là 0 là Echo Response
            Code: 0x00 => 0 là Network Unreachable
            Checksum: tính bằng thuật toán riêng ta implement sẵn trong class r
            Identifier: id unique của lệnh ping lấy luôn là thread id
            Sequence: là 1 số chỉ định index thì ta cho tăng dần từ 0 thôi 
            Message: truyền đi 1 string
            */
            var req = new Icmp { Identifier = id, Sequence = 0, Payload = Encoding.ASCII.GetBytes(message) };
            Icmp.SetChecksum(req);
            Log?.Invoke($"Pinging {remoteEP.Address} with {req.PayloadSize} bytes of message, process id={req.Identifier}");

            Stopwatch sw = new Stopwatch(); // Đồng hồ đếm thời gian gửi, có sẵn trong System.Diagnostics
            for (int i = 0; i < loops; i++)
            {
                sw.Start();
                _socket.SendTo(req.GetBytes(), remoteEP);
                try
                {
                    int ipDgramLength = _socket.ReceiveFrom(data, ref dumpEP);
                    sw.Stop();
                    
                    // Khi truyền đi thì ok nhưng khi lấy về thì phải tách ngược lại. Nó chỉ trả ra data dạng bytes full
                    // và kích thước data
                    var res = new Icmp(data, ipDgramLength);
                    Log?.Invoke($"Reply from {remoteEP}: {ipDgramLength - 28} bytes, id={res.Identifier}, seq={res.Sequence}, {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds} ms)");
                }
                catch (SocketException)
                {
                    Log?.Invoke("Timeout");
                }

                req.Sequence++;
                Icmp.SetChecksum(req);
                sw.Reset();
                Thread.Sleep(sleep);
            }
        }
    }
}