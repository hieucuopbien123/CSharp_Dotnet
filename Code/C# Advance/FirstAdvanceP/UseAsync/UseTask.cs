using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UseAsync
{
    public class UseTask
    {
        // # Lập trình async / Dùng TAP / Lớp Task
        public static Task<string> Async1(string thamso1, string thamso2)
        {
            Func<object, string> myfunc = (object thamso) => {
                dynamic ts = thamso;
                for (int i = 1; i <= 10; i++)
                {
                    // Lấy ID thread hiện tại
                    Console.WriteLine($"Task1::{i,5} {Task.CurrentId} - {Thread.CurrentThread.ManagedThreadId,3} Params {ts.x} {ts.y}");
                    Thread.Sleep(500);
                }
                return $"End Async1! {ts.x}";
            };

            // Generic theo giá trị trả về
            // Khởi tạo task nhận delegate function, có thể có tham số truyền vào hàm
            Task<string> task = new Task<string>(myfunc, new { x = thamso1, y = thamso2 });
            task.Start();

            Console.WriteLine("After call task1");

            return task;
        }

        // Action k có giá trị trả về thì Task k cần có generic
        public static Task Async2()
        {
            // Action là delegate kiểu trả về là void
            Action myaction = () => {
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine($"Task 2::{i,5} {Task.CurrentId} - {Thread.CurrentThread.ManagedThreadId,3}");
                    Thread.Sleep(2000);
                }
            };
            Task task = new Task(myaction);
            task.Start();

            Console.WriteLine("After call task2");

            return task;
        }

    }
}
