using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirstBasic
{
    class Program
    {
        // # Thao tác với System.IO / File / Directory
        class IO
        {
            public void testfile()
            {
                // ghi file txt
                string filepath = @"./test.txt";
                if (!File.Exists(filepath))
                {
                    using (StreamWriter sw = File.CreateText(filepath)) // k có tự tạo
                    {
                        sw.WriteLine("Hello");
                        sw.WriteLine("World");
                    }
                }
                // đọc file txt
                using (StreamReader sr = File.OpenText(filepath))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            // move directory
            public void testDirectory()
            {
                string sourceDirectory = @".";
                string archiveDirectory = @"..\..\..";
                try
                {
                    var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt"); // phải tồn tại directory
                    foreach (string currentFile in txtFiles)
                    {
                        string fileName = currentFile.Substring(sourceDirectory.Length + 1);
                        Directory.Move(currentFile, Path.Combine(archiveDirectory, fileName));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            // FileInfo lấy file temp
            public void testFileInfo()
            {
                string path = Path.GetTempFileName();
                var fi1 = new FileInfo(path);
                Console.WriteLine(path);
                Console.WriteLine(fi1);
            }
            // DirectoryInfo tạo và xóa directory
            public void testDirectoryInfo()
            {
                DirectoryInfo d1 = new DirectoryInfo(@"./Test");
                try
                {
                    if (d1.Exists)
                    {
                        Console.WriteLine("Exist");
                        return;
                    }
                    d1.Create();
                    Console.WriteLine("Created");
                    d1.Delete();
                    Console.WriteLine("Deleted");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally { }
            }
        }
        static void Main(string[] args)
        {
            // # Basic
            // Chạy FirstProject.exe "a" "b" để test
            if (args.Length > 1)
            {
                Console.WriteLine("args[0]::" + args[0]);
                Console.WriteLine("args[1]::" + args[1]);
            }
            
            // cast lớn sang nhỏ được
            double d = 12.34;
            int castedI = (int)d;

            // # Biến và kiểu dữ liệu / Dùng mảng 2 chiều
            int[] arr1 = new int[] { 1, 2, 3, 4, 5 };
            int[,] arr2dimension = new int[2, 3];
            int[,] arr2dimension2 = { { 1, 2, 3 }, { 4, 5, 6 } };

            int[][] jaggedArr = new int[6][];
            jaggedArr[0] = new int[4] { 1, 2, 3, 4 };

            // Dùng StringReader StringWriter
            StringWriter str = new StringWriter();
            str.WriteLine("Hello, this message is read by StringReader class");
            str.Close();
            using (var reader = new StringReader(str.ToString()))
            {
                while (reader.Peek() > -1)
                {
                    Console.WriteLine(reader.ReadLine());
                }
            }

            // # Thao tác với System.IO
            IO io = new IO();
            io.testfile();
            io.testDirectory();
            io.testFileInfo();
            io.testDirectoryInfo();

            // Dùng ?? và ??=
            int? a = null;
            int? b = 1;
            Console.WriteLine(a ?? (b ?? 2));
            a ??= (b ??= 2); // Đầu tiên a là null nên tính vế phải. b khác null nên bỏ qua 2 mà gán a = b = 1
            Console.WriteLine(a);
            Console.WriteLine(b);

            // # Dùng class library
            ClassLibrary1.Class1.test();

            Console.ReadKey();
        }
    }
    // # OOP khác / Dùng interface
    interface IEquatable<T>
    {
        bool Equals(T obj);
    }
    class Test1 : IEquatable<Test1>
    {
        public string Model { get; private set; }
        public String name
        {
            get => name;
            set => name = value;
        }
        public bool Equals(Test1 test1)
        {
            return this.Model == test1.Model;
        }
    }
    // Abstract class
    public abstract class Motorcycle
    {
        public void StartEngine() { }
        protected virtual int Drive(int miles) { return 1; }
        public virtual int Drive(int speed, string name) { return speed; }
        public abstract double getTopSpeed();
    }
    // Kế thừa
    public class ChildOfMotorCycle : Motorcycle
    {
        public override double getTopSpeed()
        {
            return 1.2;
        }
        protected override int Drive(int miles) { return 2; }
    }
    // # Dùng static / static class
    static class StaticClass
    {
        public static int a = 10;
        private static float b = 1.2F;
    }
    // # Dùng delegate
    public class TestEvent
    {
        public delegate string EventType(string str);
        public static event EventType eventInstance;
        // Lúc này, eventInstance là 1 hàm số nhận str và trả ra 1 string
        public void triggerEvent()
        {
            Console.WriteLine(eventInstance?.Invoke("Hello World"));
        }
    }
}