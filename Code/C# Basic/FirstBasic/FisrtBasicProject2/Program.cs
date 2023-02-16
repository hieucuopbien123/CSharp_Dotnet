using System;

// # Dùng namespace
// Dùng using với =
// Dùng using static
using newName = A.B;
using static System.Console;

namespace FisrtBasicProject2
{
    // # Class / Keyword sealed
    sealed class Program
    {
        public struct Product
        {
            public string name;  
            public decimal price; 

            // Phương thức sinh ra chuỗi thông tin
            public override string ToString() => $"{name} : {price}$"; // Có sẵn toString nên phải override
        }
        static void Main(string[] args)
        {
            // # Struct và Enum 
            // Struct
            Product productA;
            productA.name = "Iphone";
            productA.price = 1000;

            Product productB = productA; // gán struct, là sao chép giá trị chứ không tham chiếu như lớp
            productB.name = "Laptop";

            Console.WriteLine(productA);
            Console.WriteLine(productB.ToString());

            Product2 product = new("Samsung Abc"); // simplified
            Console.WriteLine(product.ToString());

            // Enum
            int a = (int)HocLuc.Kha;  // cast enum thành int
            WriteLine(a);     // 2

            // # Struct và Enum / Struct
            newName.C.StructC b;
        }

        public struct Product2
        {
            public Product2(string _name)
            {
                name = _name;
                price = 100;
                Description = "Mo ta";
            }
            public string name { set; get; } 
            public decimal price;
            public string Description { set => name = value; get => name; }

            public override string ToString() => $"{name} : {price}$";
        }

        enum HocLuc { Kem, TrungBinh, Kha, Gioi }
    }
}

