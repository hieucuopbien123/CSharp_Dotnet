using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UseAsync
{
    public class UseAsyncAwait
    {
        public static async Task<string> Async1(string thamso1, string thamso2)
        {
            Func<object, string> myfunc = (object thamso) => {
                dynamic ts = thamso;
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine($"Task1:: {i,5} {Thread.CurrentThread.ManagedThreadId,3} Param {ts.x} {ts.y}",
                        ConsoleColor.Green);
                    Thread.Sleep(500);
                }
                return $"End Async1! {ts.x}";
            };

            Task<string> task = new Task<string>(myfunc, new { x = thamso1, y = thamso2 });
            task.Start(); 

            await task;

            string ketqua = task.Result;
            Console.WriteLine($"Finish Task1:: {ketqua}"); 

            // Khi hàm có async trả về giá trị kiểu T. C# tự biến giá trị trả về là Task<T>, Hàm async luôn trả về kiểu Task
            return ketqua;
        }
        public static async Task Async2()
        {
            Action myaction = () => {
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine($"Task2:: {i,5} {Thread.CurrentThread.ManagedThreadId,3}", ConsoleColor.Yellow);
                    Thread.Sleep(2000);
                }
            };
            Task task = new Task(myaction);
            task.Start();

            await task;

            Console.WriteLine("Task2 end");
        }
    }
}
