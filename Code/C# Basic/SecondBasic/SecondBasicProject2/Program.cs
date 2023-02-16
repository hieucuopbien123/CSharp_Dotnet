using System;

namespace SecondBasicProject2
{
    // # OOP khác / Tính đa hình
    interface X { }
    interface Y { }
    class Product: X, Y
    {
        protected double price = 0;
        public virtual void ProductInfo()
        {
            Console.WriteLine($"Giá sản phẩm {price}");
        }
        public void TestProduct()
        {
            this.ProductInfo();
        }
    }
    class Iphone : Product
    {
        public Iphone()
        {
            price = 500;
        }
        public override void ProductInfo()
        {
            Console.WriteLine($"Điện thoại Iphone");
            base.ProductInfo();
        }
    }

    class Program
    {
        // # Dùng delegate
        public delegate void ShowLog(string message);

        static public void Info(string s) // Phương thức tương đồng với ShowLog (tham số, kiểu trả về)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("Info: {0}", s));
            Console.ResetColor();
        }
        static public void Warning(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("Waring: {0}", s));
            Console.ResetColor();
        }
        static int Sum(int x, int y)
        {
            return x + y;
        }

        // Dùng delegate làm callback
        static void TinhTong(int a, int b, Action<string> callback)
        {
            int c = a + b;
            // Gọi callback
            callback(c.ToString());
        }
        public static void TestTinhTong()
        {
            TinhTong(5, 6, (x) => Console.WriteLine($"Tong hai số là: {x}"));
        }


        static void Main(string[] args)
        {
            Product p1 = new Iphone();
            p1.TestProduct(); // Iphone coi như k có hàm ProductInfo của base luôn

            // # Dùng delegate
            // # Hàm / Lamba function
            ShowLog showLog;

            showLog = Info; 
            showLog("Thong bao");
            if (showLog == Info)
            {
                showLog = Warning;
            }
            showLog?.Invoke("Thong bao");

            //Bây giờ nó đang chỉ lưu Warning
            showLog += Info;
            showLog("Test"); // gọi cả 2 hàm theo thứ tự
            showLog -= Warning;

            // Nhưng thg dùng để gán lamba trong thực tế
            Console.WriteLine("TestLamba\n");
            showLog += (x) => Console.WriteLine(string.Format("===>{0}<===", x));
            ShowLog showLogTest = Warning;
            showLog += showLogTest;
            showLog("Test");

            // Func và Action
            Func<int, int, int> tinhtong;
            tinhtong = Sum;
            Console.WriteLine(tinhtong(1, 2));
            Action<string> showLog4 = Warning;

            TestTinhTong();
        }

        // # Hàm / Lamba function
        int Tong(int x, int y) => x + y;
    }
}
