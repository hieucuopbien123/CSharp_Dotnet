using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseStreamProject
{
    class UseBufferedStream
    {
        // VD ta ghi 2000 dòng text vào 1 file. Thay vì liên tục ghi vào file trong vòng for 2000 lần thì ta wrap nó bằng buffered và ghi vào buffered, chỉ khi buffered đầy, nó mới ghi vào file làm giảm thiểu số lần ghi -> tăng hiệu suất
        public static void WriteUseBuffer()
        {
            string path = @".\test.txt";

            FileInfo file = new FileInfo(path);
            file.Directory.Create(); // đảm bảo thư mục tồn tại

            // Lấy FileStream từ FileInfo
            using (FileStream fileStream = file.Create())
            {
                // Tạo một đối tượng BufferedStream bao lấy FileStream.
                // (Chỉ định bộ đệm (buffer) có sức chứa 10000 bytes, mặc định là 4096bytes).
                using (BufferedStream bs = new BufferedStream(fileStream, 10000))
                {
                    int index = 0;
                    for (index = 1; index < 2000; index++)
                    {
                        String s = "This is line " + index + "\n";
                        byte[] bytes = Encoding.UTF8.GetBytes(s);

                        // Ghi vào bộ đệm (buffer), khi bộ đệm đầy nó sẽ tự động đẩy dữ liệu xuống file.
                        bs.Write(bytes, 0, bytes.Length);
                    }
                    // Đẩy các dữ liệu còn lại trên bộ đệm xuống file.
                    bs.Flush();
                }
            }
            Console.WriteLine("Finish");
            Console.Read();
        }
    }
}
