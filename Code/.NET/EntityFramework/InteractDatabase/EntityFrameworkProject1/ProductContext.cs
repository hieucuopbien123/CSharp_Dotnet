using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkProject1
{
    // ConnectDB: Mọi tương tác db đều phải thông qua class config
    class ProductContext: DbContext
    {
        // DbSet<Product> trong DbContext chính là 1 bảng trong DB. Khi khai báo kiểu cho nó trong class 
        // thì coi như nó là 1 phần của config. Khi tạo database sẽ tự tạo table là nó luôn
        public DbSet<Product> products { set; get; }

        // Phải nạp chồng onConfiguring để config
        private const string connectionString = @"
                Data Source=localhost,1433;
                Initial Catalog=mydata;
                User ID=sa;Password=123456;
            ";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString).UseLoggerFactory(loggerFactory);
        }

        // Dùng logging
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
            builder
                //.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Warning)
                //.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Debug)
                .AddConsole();
        });
    }
}
