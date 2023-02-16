using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// .NET / # Entity Framework / # Tương tác DB
namespace EntityFrameworkProject1
{
    class Program
    {
        // Tạo DB
        public static async Task CreateDatabase()
        {
            // Khởi tạo DB theo config đã set trong class. Mỗi lần dùng using là 1 connection mới
            using (var dbcontext = new ProductContext())
            {
                string databasename = dbcontext.Database.GetDbConnection().Database; // mydata

                Console.WriteLine("Create " + databasename);

                bool result = await dbcontext.Database.EnsureCreatedAsync(); // Tạo nếu chưa có

                Console.WriteLine($"Create CSDL {databasename} : {result}");
            }
        }

        // Xóa DB
        public static async Task DeleteDatabase()
        {
            using (var context = new ProductContext())
            {
                String databasename = context.Database.GetDbConnection().Database;
                Console.Write($"Confirm delete {databasename} (y) ? ");
                string input = Console.ReadLine();

                if (input.ToLower() == "y")
                {
                    bool deleted = await context.Database.EnsureDeletedAsync();
                    Console.WriteLine($"Deleted {databasename}: {deleted}");
                }
            }
        }

        // Thêm vào db
        public static async Task InsertProduct()
        {
            using (var context = new ProductContext())
            {
                await context.products.AddAsync(new Product
                {
                    Name = "Product 1",
                    Provider = "Comp 1"
                });
                await context.AddAsync(new Product()
                {
                    Name = "Product 2",
                    Provider = "Comp 2"
                });

                var p1 = new Product() { Name = "Product 3", Provider = "CTY A" };
                var p2 = new Product() { Name = "Product 4", Provider = "CTY B" };
                await context.AddRangeAsync(new object[] { p1, p2 });

                // Lưu thay đổi
                int rows = await context.SaveChangesAsync();
                Console.WriteLine($"Save {rows} products");
            }
        }

        // Lấy data từ db
        public static async Task ReadProducts()
        {
            using (var context = new ProductContext())
            {
                var products = await context.products.ToListAsync();

                Console.WriteLine("list product");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.ProductId,2} {product.Name,10} - {product.Provider}");
                }
                Console.WriteLine();
                
                // Dùng LINQ để truy vấn đến DbSet products (bảng product)
                // Lấy các sản phẩm cung cấp bởi CTY A 
                products = await (from p in context.products
                                  where (p.Provider == "CTY A")
                                  select p
                                 )
                                .ToListAsync();

                // Nếu không dùng bất đồng bộ chỗ này, có thể dùng
                 //var pros = from p in context.products where (p.Provider == "CTY A") select p;

                Console.WriteLine("Product CTY A");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.ProductId,2} {product.Name,10} - {product.Provider}");
                }
            }
        }

        // Update table data
        public static async Task RenameProduct(int id, string newName)
        {
            using (var context = new ProductContext())
            {
                // Tìm product
                var product = await (from p in context.products where (p.ProductId == id) select p).FirstOrDefaultAsync();

                // Đổi tên và cập nhạt
                if (product != null)
                {
                    product.Name = newName;
                    Console.WriteLine($"{product.ProductId,2} new name = {product.Name,10}");
                    await context.SaveChangesAsync();  //Thi hành cập nhật
                }
            }
        }
        public static void UpdateDBIndependent()
        {
            using (var context = new ProductContext())
            {
                // Tạo 1 biến độc lập k được theo dõi
                var pr = new Product()
                {
                    ProductId = 4,
                    Name = "Abc"
                };

                // Bảo EF theo dõi với EntityEntry
                // Gắn pr vào context để theo dõi, nó trả vể đối tượng EntityEntry<Product>
                EntityEntry<Product> pr_e = context.Attach(pr);

                // Lấy thuộc tính Name của Product và thiết lập nó cần cập nhật với IsModified  = true;
                pr_e.Property(p => p.Name).IsModified = true;

                // Lưu thay đổi các thứ đã theo dõi
                context.SaveChanges();

                // VD dùng bỏ theo dõi 1 record
                EntityEntry<Product> eProduct = context.Entry(pr);
                eProduct.State = EntityState.Detached;
            }
        }
        public static async Task DeleteProduct(int id)
        {
            using (var context = new ProductContext())
            {
                var product = await (from p in context.products where (p.ProductId == id) select p).FirstOrDefaultAsync();

                if (product != null)
                {
                    context.Remove(product);
                    Console.WriteLine($"Delete {product.ProductId}");
                    await context.SaveChangesAsync();
                }
            }
        }

        static async Task Main(string[] args)
        {
            await CreateDatabase();

            //await InsertProduct();

            //await RenameProduct(1, "Hello World");
            //UpdateDBIndependent();
            //await DeleteProduct(4);

            //await ReadProducts();

            //await DeleteDatabase();
        }
    }
}
