using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjectionProject
{
    // C# Design Pattern / # Basic / Dependency Injection / DI trong ASP.NET
    interface IClassB
    {
        public void ActionB();
    }
    class ClassB2 : IClassB
    {
        IClassC c_dependency;
        string message;
        public ClassB2(IClassC classc, string mgs)
        {
            c_dependency = classc;
            message = mgs;
            Console.WriteLine("ClassB2 is created");
        }
        public void ActionB()
        {
            Console.WriteLine(message);
            c_dependency.ActionC();
        }
    }
    class Program
    {
        // C# Design Pattern / # Basic / Dependency Injection / DI trong ASP.NET
        static public void useSingletonService()
        {
            // Khởi tạo đối tượng dịch vụ
            var services = new ServiceCollection();
            services.AddSingleton<IClassC, ClassC>();

            // Dùng dịch vụ 3 lần
            ServiceProvider provider = services.BuildServiceProvider();
            for (int i = 0; i < 3; i++)
            {
                var instanceOfService = provider.GetService<IClassC>();
                // Lúc gọi thế này nó sẽ gọi hàm khởi tạo của dịch vụ để tạo ra đối tượng chuẩn bị cung ra. Do là singleton nên chỉ tạo 1 lần
                Console.WriteLine(instanceOfService.GetHashCode());
            }
        }
        static public void useTransientService()
        {
            // Khởi tạo đối tượng dịch vụ
            var services = new ServiceCollection();
            services.AddTransient<IClassC, ClassC>();

            // Dùng dịch vụ 3 lần
            ServiceProvider provider = services.BuildServiceProvider();
            for (int i = 0; i < 3; i++)
            {
                var instanceOfService = provider.GetService<IClassC>();
                Console.WriteLine(instanceOfService.GetHashCode());
            }
        }
        static public void useScopedService()
        {
            // Khởi tạo đối tượng dịch vụ
            var services = new ServiceCollection();
            services.AddScoped<IClassC, ClassC>();

            // Dùng 3 lần dịch vụ trong scope toàn cục, vì gọi GetService trong scope toàn cục là cả hàm này
            ServiceProvider provider = services.BuildServiceProvider();
            for (int i = 0; i < 3; i++)
            {
                var instanceOfService = provider.GetService<IClassC>();
                Console.WriteLine(instanceOfService.GetHashCode());
            }

            using (var scope = provider.CreateScope())
            {
                // Lấy dịch vụ trong scope con
                for (int i = 0; i < 3; i++)
                {
                    var service = scope.ServiceProvider.GetService<IClassC>();
                    Console.WriteLine(service.GetHashCode());
                }
            }

        }
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<ClassA, ClassA>(); // Kiểu lớp dịch vụ và kiểu triển khai dịch vụ. Kiểu triển khai phải kế thừa hoặc implements dịch vụ

            // Khai báo thêm dịch vụ IClassC và triển khai bằng ClassC
            services.AddSingleton<IClassC, ClassC>();
            // Đổi thành ClassB triển khai dịch vụ IClassC, k dùng được với ClassC nữa
            services.AddSingleton<IClassC, ClassB>();

            var provider = services.BuildServiceProvider();

            ClassB service_b = (ClassB)provider.GetService<IClassC>(); // Lấy lớp nào triển khai dịch vụ ClassA thì gọi
            service_b.ActionC();
            ClassA service_a = provider.GetService<ClassA>();
            service_a.ActionA();

            // Đăng ký dịch vụ bằng hàm delegate
            // Đăng ký dịch vụ IClassC vào ClassB2
            // ClassB2 hàm khởi tạo nhận string, nếu dùng mặc định k dùng delegate sẽ thiếu nó dẫn tới lỗi. Hoặc có thể dùng khởi tạo bằng Options
            services.AddSingleton<IClassB>((IServiceProvider serviceprovider) => {
                var service_c = provider.GetService<IClassC>();
                var sv = new ClassB2(service_c, "Thực hiện trong ClassB2");
                return sv;
            });
            // Khi dùng dịch vụ IClassB từ provider(đang là singleton) or inject nó vào 1 dịch vụ khác, nếu dịch vụ này từng đươc tạo r thì lấy, nếu k sẽ tạo dịch vụ bằng delegate

            // Tách thành factory: 1 hàm riêng cung cơ chế tạo đối tượng mong muốn dùng đk dịch vụ
            services.AddSingleton<IClassB>(CreateB2Factory);

            CreateServiceByOptions.createServiceByOptions();
            CreateServiceByOptions.createServiceByOptionsFile();
        }
        public static ClassB2 CreateB2Factory(IServiceProvider serviceprovider)
        {
            var service_c = serviceprovider.GetService<IClassC>();
            var sv = new ClassB2(service_c, "Do in ClassB2");
            return sv;
        }
    }
}
