using System;
using System.IO;
using System.Text;

namespace UseStreamProject
{
    class UseFileStream
    {
        // String <-> bytes
        public static void TestStreamWrite()
        {
            string path = @".\test.txt";

            // FileMode.Create: Tạo file mới để ghi, nếu file đã tồn tại ghi đè file này.
            Stream writingStream = new FileStream(path, mode: FileMode.Create);

            try
            {
                // Một mảng các byte (1byte < 2^8).
                // Mảng này tương ứng với: {'H','e','l','l','o',' ','W','o','r','l','d'}.
                byte[] bytes = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100 };
                // Ánh xạ ASCII là thấy. Thực tế ta phải chuyển tự động qua Encoding.ASCII.GetBytes(...)

                if (writingStream.CanWrite)
                {
                    writingStream.Write(bytes, 0, bytes.Length);

                    writingStream.WriteByte(33); // Ghi thêm một byte (33 = '!')
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e);
            }
            finally
            {
                writingStream.Close();
            }
        }
        public static void TestStreamRead()
        {
            string path = @".\test.txt";

            if (!File.Exists(path))
            {
                Console.WriteLine("File " + path + " does not exists!");
                return;
            }
            using (Stream readingStream = new FileStream(path, FileMode.Open))
            {
                // Ta giả sử đang có 1 cục đệm là 10bytes và buộc phải đọc vào đây
                byte[] temp = new byte[10];
                UTF8Encoding encoding = new UTF8Encoding(true);

                int len = 0;

                while ((len = readingStream.Read(temp, 0, temp.Length)) > 0)
                {
                    // Chuyển thành chuỗi (String).
                    // ('len' phần tử bắt đầu từ vị trí 0).
                    String s = encoding.GetString(temp, 0, len);
                    Console.WriteLine(s);
                }
                // Do cụm Hello World là 12 bytes nên phải đọc 2 lần
            }
        }
    }
}
