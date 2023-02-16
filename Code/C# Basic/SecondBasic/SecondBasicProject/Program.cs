using System;

namespace SecondBasicProject
{
    class Program
    {
        // # Hàm / Generic
        static X MyFunction<X, Y>(X x, ref Y y)
        {
            return x;
        }

        static void Main(string[] args)
        {
            Product a = new Product();
            Console.WriteLine($"{a.Order()} - {a.numberBrought()}");

            MobileProduct product = new MobileProduct();
            product.manufactory = new MobileProduct.Manufactory("Abc ...");
            product.ProductInfo();

            string b = "2";
            int rs = MyFunction<int, string>(1, ref b);

            MyClass<double> myClass = new MyClass<double>(123.123);
            myClass.TestMethod(123);

            // # Biến và kiểu dữ liệu
            // Kiểu vô danh
            var myProfile = new
            {
                name = "XuanThuLab",
                age = 20,
                skill = "ABC"
            };
            Console.WriteLine(myProfile.name);

            // Keyword dynamic
            dynamic myvar;
            dynamic d4 = System.Diagnostics.Process.GetProcesses(); // gán bất cứ kiểu gì như var

            // Keyword ?
            int? bienkieuint; // Hoặc Nullable<int> bienkieuint;
            bienkieuint = null;
        }
        // # Biến và kiểu dữ liệu / Keyword dynamic
        static void TestFunc(dynamic dvar)
        {
            Console.WriteLine(dvar.age); // Ở thời điểm biên dịch - không biết dvar có thuộc tính age hay không, nhưng nó vẫn biên dịch. Luôn chạy thành công
        }
    }
    class MyClass<T>
    {
        private T bien;
        public MyClass(T value)
        {
            bien = value;
        }
        public T TestMethod(T pr)
        {
            Console.WriteLine(pr);
            return bien;
        }
    }
}
