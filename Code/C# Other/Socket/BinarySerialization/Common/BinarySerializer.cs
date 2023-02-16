using System;
using System.Collections.Generic;
using System.Text;
namespace Common
{
    public class BinarySerializer
    {
        // # Data Serialization
        public static byte[] Serialize(Student obj)
        {
            // Dùng BitConverter giúp chuyển đổi một biến (thuộc các kiểu cơ sở, trừ kiểu string) về mảng byte.
            // Dùng stream + bytes thì phải tự code ngăn ranh giới string vì kích thước nó biến động, vì nó gửi thành 1
            // chuỗi dài liên tiếp nhau k ngăn cách gì mà.
            var data = new List<byte>();

            // chuyển Id thành mảng byte và copy vào data
            data.AddRange(BitConverter.GetBytes(obj.Id));

            // đếm số byte của FirtName, chuyển thành mảng byte và copy vào data
            data.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(obj.FirstName)));
            data.AddRange(Encoding.UTF8.GetBytes(obj.FirstName));

            data.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(obj.LastName)));
            data.AddRange(Encoding.UTF8.GetBytes(obj.LastName));

            // Date time tương tự ta lưu thành dạng số long là ticks
            data.AddRange(BitConverter.GetBytes(obj.DateOfBirth.Ticks));

            return data.ToArray(); // chuyển sang đúng byte[]
        }
        public static Student Deserialize(byte[] data)
        {
            var obj = new Student();
            int offset = 0;

            // Số int hay long thì biết trước kích thước có thể lấy được bằng ToX với X là type của .NET được
            // Các kiểu như string mới cần lấy length ra rồi GetString đúng lượng length
            obj.Id = BitConverter.ToInt32(data, offset);
            offset += 4;

            var length1 = BitConverter.ToInt32(data, offset);
            offset += 4;
            obj.FirstName = Encoding.UTF8.GetString(data, offset, length1);
            offset += length1;

            var length2 = BitConverter.ToInt32(data, offset);
            offset += 4;
            obj.LastName = Encoding.UTF8.GetString(data, offset, length2);
            offset += length2;

            obj.DateOfBirth = new DateTime(BitConverter.ToInt64(data, offset));
            
            return obj;
        }
    }
}