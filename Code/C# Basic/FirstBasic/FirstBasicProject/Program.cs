using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstBasicProject
{
    class Program
    {
        // # Class
        // Getter setter
        // Constructor
        public string Name
        {
            set
            {
                Name = value;
            }
            get
            {
                return "Tên là: " + Name;
            }
        }
        public String Name2
        {
            set => Name2 = value;
            get => Name2;
        }
        public Program(string _name) => Name = _name;

        // # Basic / Comment
        // # Hàm
        /// <summary>
        /// Tính tổng hai số nguyên
        /// </summary>
        /// <param name="a">số thứ nhất</param>
        /// <param name="b">số thứ hai</param>
        /// <returns>giá trị a + b</returns>
        static int TongHaiSo(int a, int b = 1)
        {
            return a + b;
        }

        // # Hàm
        public static void test(ref int a)
        {
            a++;
        }
        static void Main(string[] args)
        {
            //# Biến và kiểu dữ liệu
            sbyte a = 1; // -128 -> 127
            ushort b = 1; // 16 bit
            double c = 1.2; // 64 bit
            decimal d = (decimal)1.2; // 128 bit
            object e = null;

            // # Basic / Dùng System.Console                             
            Console.ForegroundColor = ConsoleColor.DarkMagenta;          
            Console.WriteLine("XIN CHÀO - CHƯƠNG TRÌNH NHẬP XUẤT DỮ LIỆU");   
            Console.ResetColor();
            Console.WriteLine("Value is {0}", TongHaiSo(1, 2));
            Console.WriteLine($"a = {a}, b = {b}");
            //Console.WriteLine(Console.ReadKey().KeyChar);

            // # Biến và kiểu dữ liệu / Dùng Convert 
            // Từ string đi mọi kiểu
            Console.WriteLine(Convert.ToInt32("32"));
            Console.WriteLine(Convert.ToDouble("3.2"));
            Console.WriteLine(Convert.ToBoolean("false"));

            // Dùng mảng
            // # Biến và kiểu dữ liệu / Keyword as
            double[] g = new double[] { 1.1, 1.2, 1.3 };
            Console.WriteLine(g.Min());
            double[] h = g.Clone() as double[];
            Console.WriteLine(h[0]);

            int[] numbers = { 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            Array.ForEach<int>(numbers, (int n) => {
                Console.WriteLine(n);
            });

            // # Hàm
            int s = 1;
            test(ref s);
            Console.WriteLine("i:: ", s);

            // # Basic / Dùng random
            Random rn = new Random();
            var number = rn.NextDouble() * rn.Next(1, Int32.MaxValue);

            // # Thao tác với string
            string k = "Hello WOrld";
            char l = k[1]; // c= 'i'

            // Keyword @
            string m = "C:\\Abc\\xyz";
            string n = @"C:\Abc\xyz hay ký tự \r"; // nội dung nguyên bản
            string p = @"Sign "" ha"; // Riêng "" thì @ mới viết là chỉ 1 dấu "
            string r = @"Xin chào các bạn
             Tôi đang học C#"; // Thậm chí là nhiều dòng
            Console.WriteLine(p);
            Console.WriteLine(r);

            // Keyword $
            Console.WriteLine($"{"Vong Lap",10} {"Chan/Le",-5}"); 
            for (int i = 8; i < 15; i++)
            {
                string chanle = (i % 2 == 0) ? "Chen" : "Le";
                Console.WriteLine($"{i,10} {chanle,-5}");
            }
            // {"Vong Lap",10} là căn 10 khoảng trống và viết căn lề phải
            // {"Chan/Le",-5} là căn 5 khoảng trống tiếp theo và viết căn lề trái

            // Dùng RegExp
            String text = @"Đây là địa chỉ
        email userabcguest@xuanthulab.net.vn và
        xyz@gmail.com cần trích xuất";
            String pattern = @"(([^\s.]+)@((.[^\s]+)(\..[^\s]+)))"; // RegExp check gmail

            Regex rx = new Regex(pattern);

            // Tìm kiếm.
            MatchCollection matches = rx.Matches(text);

            // In thông báo tìm kiếm.
            Console.WriteLine("Tìm thấy {0} email trong:\n\n  {1}\n\n",
                            matches.Count,
                            text);
            // Xuất kết quả email tìm được
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("{0}", groups[0].Value);
            }

            // # Class / Cast từ nhỏ đến lớn
            Program2 t = new Program2("Hello", "World");
            Program u = (Program2)t;
        }
    }

    // # Class / Kế thừa
    class Program2 : Program
    {
        public Program2(string nameofCategory, string mota) : base(nameofCategory)
        { }
        // Khởi tạo private thg dùng trong các TH đặc biệt như tạo singleton parttern thì bên ngoài sẽ k gọi được
        private Program2() : base("Hello WOrld")
        {
        }

        // # Dùng static / Constructor tĩnh
        // C# hỗ trợ tối đa. Phương thức khởi tạo tĩnh sẽ tự động được gọi chỉ 1 lần khi truy cập thành viên tĩnh lần đầu
        static Program2()
        {
        }
    }
}
