using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Server
{
    internal class Program
    {
        // # Đa luồng socket // Bất đồng bộ socket / Dùng mô hình APM trong socket
        // Ở đây gọi đệ quy. 1 connection sinh ra sẽ chạy để tiếp tục chờ connection tiếp theo vào trên 1 luồng riêng
        private static void Main(string[] args)
        {
            Console.Title = "Tcp Server";
            var listener = new TcpListener(IPAddress.Any, 1308);
            listener.Start();
            listener.BeginAcceptSocket(AcceptCallback, listener);
            
            Console.ReadLine(); // phải có cái này tránh dừng mẹ ct
        }
        private static readonly int _size = 1024;
        private static readonly byte[] _buffer = new byte[_size];
        private static void AcceptCallback(IAsyncResult ar)
        {
            var listener = ar.AsyncState as TcpListener;
            listener.BeginAcceptSocket(AcceptCallback, listener);
            var socket = listener.EndAcceptSocket(ar);
            socket.BeginReceive(_buffer, 0, _size, SocketFlags.None, ReceiveCallback, socket);
        }
        private static void ReceiveCallback(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            int count = socket.EndReceive(ar);
            var request = Encoding.ASCII.GetString(_buffer, 0, count);
            Console.WriteLine($"Received: {request}");
            var response = request.ToUpper();
            var buffer = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, socket);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            int count = socket.EndSend(ar);
            Console.WriteLine($"{count} bytes have been sent to client");
        }
    }
}