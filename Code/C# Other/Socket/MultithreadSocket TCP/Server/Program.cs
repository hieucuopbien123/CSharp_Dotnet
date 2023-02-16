using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.ComponentModel;
namespace Server
{
    class Program
    {
        // # Data Serialization / Gửi file
        // Nó k thực sự gửi file mà gửi từng cục data trong file và phía server tạo file mới paste data đó vào
        static readonly string _home = @"C:\Users\Ryan.Nguyen\Desktop";
        static void Serve(TcpClient client)
        {
            var nsStream = client.GetStream();
            var reader = new BinaryReader(nsStream);
            var fileNameLength = reader.ReadInt32();
            var fileDataLength = reader.ReadInt64();
            var fileNameBytes = reader.ReadBytes(fileNameLength);
            var fileName = Encoding.UTF8.GetString(fileNameBytes);
            Console.WriteLine($"File to receive: {fileName}");
            Console.WriteLine($"Bytes to receive: {fileDataLength}");
            var path = Path.Combine(_home, fileName);
            var fStream = File.OpenWrite(path);
            var length = 0L;
            var size = 512;
            var buffer = new byte[size];
            // Làm rất chặt chẽ, đọc từng cục vào buffer rồi gửi tiếp data từ buffer đi
            while (length < fileDataLength)
            {
                var count = nsStream.Read(buffer, 0, size);
                fStream.Write(buffer, 0, count);
                length += count;
            }
            Console.WriteLine($"File saved as: {path}");
            Console.WriteLine($"Bytes received: {fStream.Length}");
            Console.WriteLine("-----------");
            fStream.Close();
            var writer = new StreamWriter(nsStream) { AutoFlush = true };
            writer.WriteLine("200 OK, Thank you!");
            client.Close();
        }

        // # Đa luồng socket / Dùng ThreadPool

        //static void Main(string[] args)
        //{
        //    Console.Title = "File uploader server";
        //    var listener = new TcpListener(IPAddress.Any, 1308);
        //    listener.Start(10);

        //    while (true)
        //    {
        //        var client = listener.AcceptTcpClient(); 
        //        ThreadPool.QueueUserWorkItem(Callback, client);
        //    }
        //}
        //private static void Callback(object state) { 
        //    var client = state as TcpClient; 
        //    Console.WriteLine($"Starting a new thread: {Thread.CurrentThread.ManagedThreadId}"); 
        //    Serve(client); 
        //}

        // # Đa luồng socket / Dùng BackgroundWorker

        //static void Main(string[] args)
        //{
        //    Console.Title = "File uploader server";
        //    var listener = new TcpListener(IPAddress.Any, 1308);
        //    listener.Start(10);
        //    while (true)
        //    {
        //        var client = listener.AcceptTcpClient();
        //        var worker = new BackgroundWorker();
        //        worker.DoWork += Worker_DoWork;
        //        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        //        worker.RunWorkerAsync(client);
        //    }
        //}
        //private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    Console.WriteLine($"Thread execution completed: {e.Result}");
        //}
        //private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    var client = e.Argument as TcpClient;
        //    Console.WriteLine($"Starting a new thread: {Thread.CurrentThread.ManagedThreadId}");
        //    Serve(client);
        //    e.Result = Thread.CurrentThread.ManagedThreadId;
        //}

        // # Đa luồng socket / Dùng Thread
        static void Main(string[] args)
        {
            Console.Title = "File uploader server";
            var listener = new TcpListener(IPAddress.Any, 1308);
            listener.Start(10);
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var thread = new Thread(Start);
                thread.Start(client);
            }
        }
        private static void Start(object state)
        {
            var client = state as TcpClient;
            Console.WriteLine($"Starting a new thread: {Thread.CurrentThread.ManagedThreadId}");
            Serve(client);
        }
    }
}