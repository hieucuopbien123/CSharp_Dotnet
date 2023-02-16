using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteServer();
        }
        // # Socket / Dùng class Socket
        public static void ExecuteServer()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10); // Tại 1 thời điểm có nhiều yêu cầu kết nối từ các client nhưng server chỉ xử lý 1 kết
                // nối 1 lúc. Các yêu cầu còn lại sẽ nằm trong hàng đợi, ta set max 10 cái trong hàng đợi. Khi có client sẽ
                // tạo 1 bản sao client tương tác với client thật dùng bằng Accept
                // Trong UDP éo chia giới hạn bnh client vì k thiết lập connection và k cần gọi hàm listen

                while (true)
                {
                    Console.WriteLine("Waiting connection ... ");
                    Socket clientSocket = listener.Accept(); // Wait connection mới
                    // Nếu có sẽ khởi tạo 1 instance client 3 bước hoàn chỉnh

                    byte[] bytes = new Byte[1024]; // Data buffer
                    string data = null;

                    // Ở đây đang k dùng Stream và cũng implement buffer k chuẩn. Là vì clientSocket gửi 1 lượng bytes lần lượt thì ta bắt lưu vào buffer liên tục đến khi hết data, sau đó mới xử lý. Thực tế sẽ phải lưu vào buffer liên tục bất đồng bộ, trong lúc đó thì server xử lý data luôn, xử lý đến đâu thì xóa khỏi buffer đến đó. Thì vc data đi ra liên tục tuần tự và có 1 buffer dự trữ như v mới là stream + buffer. Lớp socket stream chính là lo hết điều này.
                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes); // Client nhận đươc lưu vào buffer
                        data += Encoding.ASCII.GetString(bytes, 0, numByte);
                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }
                    clientSocket.Shutdown(SocketShutdown.Receive); // K nhận dữ liệu nữa

                    Console.WriteLine("Text received -> {0} ", data);
                    byte[] message = Encoding.ASCII.GetBytes("Test Server");

                    clientSocket.Send(message);

                    // Lệnh Shutdown Send như dưới k ngắt kết nối TCP. Nếu còn data trong buffer chưa gửi nó sẽ cố gửi hết
                    // và k nhận thêm data vào buffer nữa. Rồi phát đi 1 chuỗi byte đặc biệt tới bên kia bảo rằng qtr
                    // truyền đã kết thúc và kết thúc dòng byte dữ liệu. Điều này qtrong khi dữ liệu kthuoc lớn
                    clientSocket.Shutdown(SocketShutdown.Send);
                    clientSocket.Close();
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
