using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstAdvanceProject
{
    // # Data annotation / Tạo data annotation
    // [AttributeUsage(...)] để set MotaAttribute áp dụng được cho những thành phần nào. Ở đây là cả class, thuộc tính của class, phương thức của class
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public class MotaAttribute : Attribute
    {
        // Phương thức khởi tạo
        public MotaAttribute(string v) => Description = v;
        public string Description { set; get; }
    }
    // Đặt tên thoải mái, class là MotaAttribute thì viết annotation là Mota hay MotaAttribute đều được


    // Dùng thuộc tính Mota cho đối tượng nào thì nó sẽ mang thêm thông tin Description với nội dung thêm vào
    // Bên dưới như gọi phương thức khởi tạo của MotaAttribute
    [Mota("Class represent user")]                          // thêm Attribute cho lớp
    public class User
    {
        [Mota("Save for age")]                             // thêm Attribute cho thuộc tính lớp
        public int age { set; get; }

        [Mota("The method that show user's age")]          // thêm Attribute cho phương thức
        public void ShowAge() { }
    }


    // Dùng Reflection để lấy thuộc tính Mota của class User ez
    class TestReadAttribute
    {
        public static void test()
        {
            var a = new User();

            // Đọc các Attribute của lớp
            // GetCustomAttributes lấy các attributed thêm vào class
            foreach (Attribute attr in a.GetType().GetCustomAttributes(false))
            {
                MotaAttribute mota = attr as MotaAttribute;
                if (mota != null)
                {
                    Console.WriteLine($"{a.GetType().Name,10} : {mota.Description}");
                }
            }

            // Đọc Attribute của từng thuộc tính lớp
            foreach (var thuoctinh in a.GetType().GetProperties())
            {
                foreach (Attribute attr in thuoctinh.GetCustomAttributes(false))
                {
                    MotaAttribute mota = attr as MotaAttribute;
                    if (mota != null)
                    {
                        Console.WriteLine($"{thuoctinh.Name,10} : {mota.Description}");
                    }
                }
            }

            // Đọc Attribute của phương thức
            foreach (var m in a.GetType().GetMethods())
            {
                foreach (Attribute attr in m.GetCustomAttributes(false))
                {
                    MotaAttribute mota = attr as MotaAttribute;
                    if (mota != null)
                    {
                        Console.WriteLine($"{m.Name,10} : {mota.Description}");
                    }
                }
            }
        }
    }
}
