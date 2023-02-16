using System;

namespace ExtensionMethod
{
    // # Extension Method
    public static class MyExtensionMethod
    {
        public static void Print(this string s, ConsoleColor color = ConsoleColor.Yellow)
        {
            ConsoleColor lastColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(s);
            Console.ForegroundColor = lastColor;
        }
    }

    class Program
    {
        // # Operator overloading / Đánh indexer 
        string ho = "Nguyễn";
        string ten = "Nam";
        public string this[int index]
        {
            // Đọc dữ liệu theo chỉ mục
            get
            {
                if (index == 0) return ho;
                else if (index == 1) return ten;
                else throw new Exception("Index not exist");
            }

            // Gán dữ liệu theo chỉ mục
            set
            {
                if (index == 0) ho = value;
                else if (index == 1) ten = value;
                else throw new Exception("Index not exist");
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p[0] = "Hello";
            Console.WriteLine(p[0] + " - " + p[1]);

            // # Dùng try catch
            try
            {
                int b = 0;
                int a = 1 / b; // Có sẵn
                Exception myexception = new Exception("Not divide for 0"); // Custom message
                throw myexception;
                throw new DataTooLongExeption(); // Tạo riêng class
            } catch(DivideByZeroException e)
            {
                Console.WriteLine(e.StackTrace);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { }
        }
    }
    public class DataTooLongExeption : Exception
    {
        const string erroMessage = "Too long";
        public DataTooLongExeption() : base(erroMessage) { }
    }
}
