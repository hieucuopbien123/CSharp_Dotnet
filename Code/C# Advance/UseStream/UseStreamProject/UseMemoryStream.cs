using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseStreamProject
{
    class UseMemoryStream
    {
        public static void UseMemStream()
        {
            // Tạo một đối tượng MemoryStream có dung lượng 100 bytes.
            MemoryStream memoryStream = new MemoryStream(100);

            byte[] javaBytes = Encoding.UTF8.GetBytes("Java");
            byte[] csharpBytes = Encoding.UTF8.GetBytes("CSharp");

            // Ghi các byte vào memoryStream (luồng bộ nhớ). 
            memoryStream.Write(javaBytes, 0, javaBytes.Length);
            memoryStream.Write(csharpBytes, 0, csharpBytes.Length);

            // Ghi ra sức chứa và độ dài của Stream.
            // ==> Capacity: 100, Length: 10.
            Console.WriteLine("Capacity: {0} , Length: {1}",
                                  memoryStream.Capacity.ToString(),
                                  memoryStream.Length.ToString());

            // Lúc này vị trí con trỏ (cursor) đang đứng ở sau ký tự 'p'.
            // ==> 10.
            Console.WriteLine("Position: " + memoryStream.Position);

            // Di chuyển lùi con trỏ lại 6 byte, so với vị trí hiện tại. 
            memoryStream.Seek(-6, SeekOrigin.Current);

            // Lúc này vị trí con trỏ đang đứng ở sau ký tự 'a' và trước 'C'.
            // ==> 4.
            Console.WriteLine("Position: " + memoryStream.Position);

            byte[] vsBytes = Encoding.UTF8.GetBytes(" vs ");

            // Ghi dữ liệu vào memoryStream (Luồng bộ nhớ).
            memoryStream.Write(vsBytes, 0, vsBytes.Length);

            // Lấy ra mảng buffer của nó và in ra
            byte[] allBytes = memoryStream.GetBuffer();
            string data = Encoding.UTF8.GetString(allBytes);

            // ==> Java vs rp
            Console.WriteLine(data);

            Console.WriteLine("Finish!");
            Console.Read();
        }
    }
}
