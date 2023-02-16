using System;

namespace SecondBasicProject
{
    // # Class / Partial class
    public partial class Product
    {
        partial void myMethod()
        {
            Console.WriteLine("Noi trien khai");
        }
        public int numberBrought()
        {
            return 100;
        }
    }

    // Nested class
    class MobileProduct
    {
        public Manufactory manufactory { set; get; }

        // Lớp Manufactory nằm trong MobileProduct 
        public class Manufactory
        {
            string address;
            public Manufactory(string address)
            {
                this.address = address;
            }
            public void ShowAddress()
            {
                Console.WriteLine(address);
            }
        }

        public void ProductInfo()
        {
            manufactory.ShowAddress();
        }
    }
}
