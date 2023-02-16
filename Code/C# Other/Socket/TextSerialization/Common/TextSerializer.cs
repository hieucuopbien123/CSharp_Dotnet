using System;
using System.Collections.Generic;
namespace Common
{
    public class TextSerializer
    {
        // Chuyển đổi một object của Student sang chuỗi ký tự. 
        // Chuỗi kết quả có hình thức tương tự chuỗi tham số của Http get.
        public static string Serialize(Student obj)
        {
            return $"Id = {obj.Id} " +
                $"& FirstName = {obj.FirstName} " +
                $"& LastName = {obj.LastName} " +
                $"& DateOfBirth = {obj.DateOfBirth.Ticks}";
        }
        // Chuyển đổi một chuỗi trở lại thành object kiểu Student
        public static Student Deserialize(string data)
        {
            var dict = new Dictionary<string, string>();
            var pairs = data.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var p = pair.Split('='); // cắt mỗi phần tử lấy mốc là ký tự =
                if (p.Length == 2) // một cặp khóa = giá_trị đúng sau khi cắt sẽ phải có 2 phần
                {
                    var key = p[0].Trim(); // phần tử thứ nhất là khóa
                    var value = p[1].Trim(); // phần tử thứ hai là giá trị
                    dict[key] = value; // lưu cặp khóa-giá trị này lại sử dụng phép toán indexing                    
                }
            }
            var obj = new Student();
            if (dict.ContainsKey("Id"))
            {
                obj.Id = int.Parse(dict["Id"]);
            }
            if (dict.ContainsKey("FirstName"))
            {
                obj.FirstName = dict["FirstName"];
            }
            if (dict.ContainsKey("LastName"))
            {
                obj.LastName = dict["LastName"];
            }
            if (dict.ContainsKey("DateOfBirth"))
            {
                obj.DateOfBirth = new DateTime(long.Parse(dict["DateOfBirth"]));
            }
            return obj;
        }
    }
}