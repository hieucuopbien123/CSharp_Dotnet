using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseObservableCollection
{
    public class Product
    {
        public int ID { set; get; }
        public string Name { set; get; } 
        public double Price { set; get; }
        public string[] Colors { set; get; } 
        public int Brand { set; get; }   
        public Product(int id, string name, double price, string[] colors, int brand)
        {
            ID = id; Name = name; Price = price; Colors = colors; Brand = brand;
        }
        override public string ToString()
           => $"{ID,3} {Name,12} {Price,5} {Brand,2} {string.Join(",", Colors)}";
    }
    public class Brand
    {
        public string Name { set; get; }
        public int ID { set; get; }
    }
    class UseLINQ
    {
        // # Dùng Linq
        public static void testLINQ()
        {
            var brands = new List<Brand>() {
                new Brand{ID = 1, Name = "Công ty AAA"},
                new Brand{ID = 2, Name = "Công ty BBB"},
                new Brand{ID = 4, Name = "Công ty CCC"},
            };
            var products = new List<Product>()
            {
                new Product(1, "Bàn trà",    400, new string[] {"Xám", "Xanh", "Vàng"},         2),
                new Product(2, "Tranh treo", 400, new string[] {"Vàng", "Xanh"},        1),
                new Product(3, "Đèn trùm",   500, new string[] {"Trắng"},               3),
                new Product(4, "Bàn học",    200, new string[] {"Trắng", "Xanh"},       1),
                new Product(5, "Túi da",     300, new string[] {"Đỏ", "Đen", "Vàng"},   2),
                new Product(6, "Giường ngủ", 500, new string[] {"Trắng"},               2),
                new Product(7, "Túi ngủ",      600, new string[] {"Trắng"},               3),
            };

            // from phức tạp
            var b = from product in products
                    select new
                    {
                        ten = product.Name.ToUpper(),
                        mausac = string.Join(',', product.Colors)
                    };
            Console.WriteLine("FROM Complex::");
            foreach (var item in b) Console.WriteLine(item.ten + " - " + item.mausac);

            // where
            var c = from product in products
                    where (product.Price >= 400 && product.Price < 700)
                    where product.Name.StartsWith("Tranh")
                    select product.Name;
            Console.WriteLine("WHERE::");
            foreach (var product in c) Console.WriteLine(product.ToString());

            // from kết hợp. Thg dùng khi 1 thuộc tính là mảng thì ở đây ta check nếu mảng color có 1 giá trị "Vàng" cũng lấy vì from color in product.Colors là đang xét từng color trong mảng
            var a = from product in products
                    from color in product.Colors
                    where product.Price < 500
                    where color == "Vàng"
                    orderby product.Price descending, product.Name ascending
                    select product;
            Console.WriteLine("FROM combine::");
            foreach (var product in a) Console.WriteLine(product.ToString());

            // group by
            var d = from product in products
                    where product.Price >= 400 && product.Price <= 500
                    group product by product.Price;
            Console.WriteLine("GROUP BY");
            foreach (var group in d)
            {
                Console.WriteLine(group.Key);
                foreach (var product in group)
                {
                    Console.WriteLine($"    {product.Name} - {product.Price}");
                }
            }

            // Lưu câu lệnh vào biến tạm rồi thực hiện tiếp mệnh đề khác trên biến tạm. Thực tế nó k lưu kết quả mà lưu câu lệnh. Khi gọi hay lấy phần tử thì nó mới thực hiện 
            var e = from product in products
                    where product.Price >= 400 && product.Price <= 500
                    group product by product.Price into gr
                    orderby gr.Key descending
                    select gr;

            // Dùng let tạo biến trong linq
            var f = from product in products
                    group product by product.Price into gr
                    let count = gr.Count()
                    select new
                    {
                        price = gr.Key,
                        number_product = count
                    };
            Console.WriteLine("LET::");
            foreach (var item in f)
            {
                Console.WriteLine($"{item.price} - {item.number_product}");
            }

            // Inner join
            var g = from product in products
                    join brand in brands on product.Brand equals brand.ID
                    select new
                    {
                        name = product.Name,
                        brand = brand.Name,
                        price = product.Price
                    };
            Console.WriteLine("INNER JOIN::");
            foreach (var item in g)
            {
                Console.WriteLine($"{item.name,10} {item.price,4} {item.brand,12}");
            }

            // Left join: thực tế lưu câu lệnh vào biến tạm. Khi gọi DefaultIfEmpty thì câu lệnh mới thực hiện với 1 rule mới là: giá trị k khớp 2 bảng sẽ k bỏ qua mà lấy default là null cho nó. 
            var h = from product in products
                    join brand in brands on product.Brand equals brand.ID into t
                    from brand in t.DefaultIfEmpty()
                    select new
                    {
                        name = product.Name,
                        brand = (brand == null) ? "NO-BRAND" : brand.Name,
                        price = product.Price
                    };
            Console.WriteLine("LEFT JOIN::");
            foreach (var item in h)
            {
                Console.WriteLine($"{item.name,10} {item.price,4} {item.brand,12}");
            }
        }
    }
}
