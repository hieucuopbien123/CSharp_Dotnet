using System;
using System.IO;

namespace FirstIntermediateProject
{
    class Program
    {
        // # Event
        public static void TestEvent()
        {
            Publisher p = new Publisher();
            SubscriberA sa = new SubscriberA();
            SubscriberB sb = new SubscriberB();

            sa.Sub(p);
            sb.Sub(p);

            p.Send();
        }
        public static void TestEventOfDotNet()
        {
            ClassA p = new ClassA();
            ClassB sa = new ClassB();
            ClassC sb = new ClassC();

            sa.Sub(p);
            sb.Sub(p);

            p.Send();
        }

        // # Class / Destructor
        ~Program()
        {
            Console.WriteLine("Chạy khi gọi GC.Collect() hoặc gán instance = null");
        }
        static void Main(string[] args)
        {
            TestEvent();
            TestEventOfDotNet();

            // # Dùng IDisposable và using
            WriteData writeData = new WriteData("filename.txt");
            writeData.Dispose();

            // # Operator Overloading
            MyVector vec1 = new MyVector(1.2, 2.3);
            MyVector vec2 = new MyVector(3.4, 4.5);
            (vec1 + vec2).ShowXY();
        }
    }
    class MyVector
    {
        // # Operator Overloading
        double x;
        double y;
        public MyVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static MyVector operator +(MyVector a, MyVector b)
        {
            double sx = a.x + b.x;
            double sy = a.x + b.y;
            MyVector v = new MyVector(sx, sy);
            return v;
        }
        public void ShowXY()
        {
            Console.WriteLine("x = " + x);
            Console.WriteLine("y = " + y);
        }
    }

    // # Dùng IDisposable và using
    // Recommend IDisposable pattern 
    public class WriteData : IDisposable
    {
        private bool m_Disposed = false; // Lưu trạng thái dispose
        private StreamWriter stream;
        public WriteData(string filename)
        {
            stream = new StreamWriter(filename, true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Hàm này bảo GC là object đã được cleaned và k cần đi vào finalizer queue nữa (kìm nén qtr finalize lại). Nó đem lại optimization nhưng không đáng kể gì hết. Nó chỉ dùng khi dùng IDisposable như này thôi
        }
        ~WriteData()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // Giải phóng tài nguyên mà phải làm thủ công là gọi các hàm dispose. Chỉ chạy khi gọi dispose
                    stream.Dispose();
                }
                // Giải phóng tài nguyên unmanaged của lớp bằng cách set to null các large field. Luôn chạy khi giải phóng
                m_Disposed = true;
            }
        }
    }
    // Pattern chuẩn: Biến trạng thái đảm bảo tài nguyên thủ công chỉ giải phóng 1 lần dù gọi thế nào
    // Ta thấy k ổn lắm vì chả hiểu biến disposing vai trò làm gì, bỏ đi sẽ chuẩn hơn cho mọi case. Dù quên gọi dispose, gọi dispose nhiều lần hay gọi 1 lần đều chạy đúng.
}
