using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionProject
{
    // C# Design Pattern / # Basic / Dependency Injection / DI trong ASP.NET / Khởi tạo dịch vụ thông qua Options
    public class MyServiceOptions
    {
        public string data1 { get; set; }
        public int data2 { get; set; }
    }
    public class MyService
    {
        public string data1 { get; set; }
        public int data2 { get; set; }

        public MyService(IOptions<MyServiceOptions> options)
        {
            MyServiceOptions opts = options.Value;
            data1 = opts.data1;
            data2 = opts.data2;
        }
        public void PrintData() => Console.WriteLine($"{data1} / {data2}");
    }
    class CreateServiceByOptions
    {
        public static void createServiceByOptions()
        {
            var services = new ServiceCollection();
            services.Configure<MyServiceOptions>(
                options => {
                    options.data1 = "Xin chao cac ban";
                    options.data2 = 2021;
                }
            );

            // Đảm bảo lớp MyService constructor chỉ nhận vào tham số là kiểu IOptions<> thì có thể đăng ký dịch vụ luôn có tham số mà k cần dùng delegate. Tách các params truyền vào thành 1 class options riêng
            services.AddSingleton<MyService>();
            var provider = services.BuildServiceProvider();

            // Lấy ra instance service bth
            MyService myservice = provider.GetService<MyService>();
            myservice.PrintData();

            // Lấy ra option đã gắn vào biến services
            var config = provider.GetService<IOptions<MyServiceOptions>>();
            MyServiceOptions myServiceOptions = config.Value;
            Console.WriteLine($"{myServiceOptions.data1} / {myServiceOptions.data2}");

            // Dù MyServices hàm khởi tạo nhận kiểu IOptions nhưng k có nghĩa nó chỉ được tạo ra bằng DIContainer, ta vẫn có thể lấy ra IOptions để tạo ra nó thủ công
            var opts = Options.Create(new MyServiceOptions()
            {
                data1 = "DATA-DATA-DATA-DATA-DATA",
                data2 = 12345
            });
            MyService myService = new MyService(opts);
            myService.PrintData();
        }

        // Khởi tạo options lưu trong file riêng
        public static void createServiceByOptionsFile()
        {
            // Nạp file cấu hình 
            var configBuilder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory()+"/../../..") 
                       .AddJsonFile("appsettings.json");
            // Build lấy ra configuration root
            var configurationroot = configBuilder.Build();
            // Truy cập thoải mái
            var cf1 = configurationroot.GetSection("Option2").GetSection("key1").Value; // Test
            var cf2 = configurationroot.GetSection("Option2").GetSection("key2").Value; // 789
            var cf3 = configurationroot.GetSection("Option2").GetSection("key3").Value; // null, không tồn tại
            Console.WriteLine(cf1);
            Console.WriteLine(cf2);
            Console.WriteLine(cf3);

            // Lấy thông tin mục MyServiceOptions trong file json để truyền vào cho dịch vụ khi đăng ký

            ServiceCollection services = new ServiceCollection();

            services.AddOptions();
            services.Configure<MyServiceOptions>(configurationroot.GetSection("MyServiceOptions"));

            services.AddSingleton<MyService>();

            var provider = services.BuildServiceProvider();

            var myservice = provider.GetService<MyService>();
            myservice.PrintData();
        }
    }
}
