using System;
using System.Threading;
using System.Threading.Tasks;

namespace UseAsync
{
    // # OOP khác / Dùng interface
    // 2 interface cùng hàm được, bù cho thuộc tính của hướng đối tượng.
    interface X
    {
        public void test();
    }
    interface Y
    {
        public void test();
    }
    class A: X, Y
    {
        public void test()
        {
            Console.WriteLine("Call from A");
        }

    }
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Use interface");
            X x = new A();
            x.test();
            ((Y)x).test();

            // # Lập trình async / Dùng TAP / Lớp Task k dùng async await
            Console.WriteLine("Use Class Task");
            Console.WriteLine($"MainThread:: {Thread.CurrentThread.ManagedThreadId,3} ");
            Task<string> t1 = UseTask.Async1("A", "B");
            Task t2 = UseTask.Async2();

            Console.WriteLine("After call 2 task");

            t1.Wait();
            String s = t1.Result;
            Console.WriteLine($"Result::{s}", ConsoleColor.Red);

            // Task 1 chạy song song task 2 -> chờ task 1 chạy xong -> task 2 chưa kết thúc -> ấn phím làm main
            // thread kết thúc -> kéo theo task 2 kết thúc dù chưa xong
            Console.ReadKey();

            // # Lập trình async / Dùng TAP / Dùng async await keyword
            Console.WriteLine("Use Async Await");
            var t11 = UseAsyncAwait.Async1("x", "y");
            var t22 = UseAsyncAwait.Async2();

            Console.WriteLine("Task1, Task2 is running");
            await t11;
            Console.WriteLine("T1 end");
            await t22;

            // Tải file remote dùng async await
            //var taskdonload = DownloadFile.Download("https://github.com/microsoft/vscode/archive/1.48.0.tar.gz");
            //await taskdonload;

            // # Lập trình async / Dùng TAP / Lớp Task / Dùng CancellationToken
            var tokenSource = new CancellationTokenSource(); // Tạo đối tượng phát yêu cầu dừng task
            var token = tokenSource.Token; // Lấy token

            // Tạo task 1 dùng CancellationToken
            Task task1 = new Task(
                () => {
                    for (int i = 0; i < 10000; i++)
                    {
                        if (token.IsCancellationRequested) // Check có yêu cầu dừng thì dừng
                        {
                            Console.WriteLine("TASK1 STOP");
                            token.ThrowIfCancellationRequested();
                            return;
                        }
                        Console.WriteLine("TASK1 runing ... " + i);
                        Thread.Sleep(300);
                    }
                },
                token
            );

            // Tạo task 2 dùng CancellationToken trùng token với task 1
            Task task2 = new Task(
                () => {

                    for (int i = 0; i < 10000; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Console.WriteLine("TASK1 STOP");
                            token.ThrowIfCancellationRequested();
                            return;
                        }
                        Console.WriteLine("TASK2 runing ... " + i);
                        Thread.Sleep(300);
                    }
                },
                token
            );

            // Chạy các task
            task1.Start();
            task2.Start();

            // Chạy song song trong main thread
            while (true)
            {
                var c = Console.ReadKey().KeyChar;
                if (c == 'e') // Bấm e phát yêu cầu dừng task
                {
                    tokenSource.Cancel();
                    break;
                }

            }
            Console.WriteLine("All task stop");
            Console.ReadKey();
        }
    }
}
