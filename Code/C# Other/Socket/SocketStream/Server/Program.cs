using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Program
    {
        // Socket Stream
        // Cơ chế: Tạo NetworkStream cho backing store là socket, Dùng StreamReader và StreamWriter làm adapter stream giúp đọc ghi văn bản
        // Tức có 2 cách: 1 là tạo instance socket client phía server rồi gọi Send Receive các thứ; 2 là tạo ra stream từ instance của socket client rồi chỉ cần thao tác với stream
        static void Main(string[] args)
        {
            Console.Title = "Tcp Server";
            var listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 1308));
            listener.Listen(10);
            Console.WriteLine($"Server started at {listener.LocalEndPoint}");
            while (true)
            {
                var worker = listener.Accept(); // trả ra instance của socket client
                
                // Dùng listener.GetStream tương tự
                var stream = new NetworkStream(worker); // khởi tạo object của NetworkStream từ tcp socket

                // Các thể loại stream khi đọc ghi đều tự giúp chuyển qua lại bytes <-> string nên chỉ cần thao tác như bth
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream) { AutoFlush = true }; // AutoFlush để tự động flush luồng NetworkStream

                // ReadLine của StreamReader sẽ tự làm việc với NetworkStream bên trong để đọc ra chuỗi byte, sau đó tự động
                // biến đổi thành chuỗi utf-8.
                // lưu ý: lệnh ReadLine sẽ đọc đến khi nào nhìn thấy cặp ký tự \r\n thì sẽ dừng lại. Trong kết quả trả về,
                // ReadLine sẽ xóa bỏ hai ký tự thừa này. Vì vậy, ở client chúng ta phải tự bổ sung cặp \r\n vào cuối chuỗi
                // truy vấn. Nếu không làm như vậy, ReadLine sẽ không dừng việc đọc.
                var request = reader.ReadLine();
                var response = string.Empty;
                switch (request.ToLower())
                {
                    case "date": response = DateTime.Now.ToLongDateString(); break;
                    case "time": response = DateTime.Now.ToLongTimeString(); break;
                    case "year": response = DateTime.Now.Year.ToString(); break;
                    case "month": response = DateTime.Now.Month.ToString(); break;
                    case "day": response = DateTime.Now.Day.ToString(); break;
                    case "dow": response = DateTime.Now.DayOfWeek.ToString(); break;
                    case "doy": response = DateTime.Now.DayOfYear.ToString(); break;
                    default: response = "UNKNOW COMMAND"; break;
                }

                // Có thể ghi thẳng chuỗi utf-8 vào luồng bằng phương thức WriteLine của StreamWriter thay vì tự biến đổi chuỗi
                // sang mảng byte.
                // Tương tự phương thức WriteLine tự thêm cặp \r\n vào cuối chuỗi, tự động biến đổi chuỗi này thành mảng byte
                // và ghi vào stream. Vì lý do này, bên client sẽ phải tự mình cắt bỏ chuỗi \r\n trước khi in ra.
                // nếu sử dụng ReadLine của StreamReader thì không cần tự cắt bỏ \r\n vì ReadLine sẽ tự động xóa bỏ cặp ký tự này giúp. Còn dùng Write bth thì phải chủ động thêm
                
                writer.WriteLine(response);

                //writer.Flush(); // nếu không sử dụng AutoFlush thì phải tự mình gọi lệnh Flush
                
                worker.Close(); // tự đóng hết các stream liên quan
            }
        }
    }
}