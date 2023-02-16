using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecondAdvanceProject
{
    class UseParallel
    {
        // # Lập trình async / Dùng TAP / Dùng Parallel / Dùng Parallel.For
        public static void PintInfo(string info) =>
            Console.WriteLine($"{info,10}    task:{Task.CurrentId,3}    " + $"thread: {Thread.CurrentThread.ManagedThreadId}"); // In threadid và taskid hiện tại
        // Vì Parallel For mỗi 1 hàm thực hiện là 1 task riêng

        public static void RunTask(int i)
        {
            PintInfo($"Start {i,3}");
            Task.Delay(1000).Wait(); 
            PintInfo($"Finish {i,3}");
        }

        public static void ParallelFor()
        {
            ParallelLoopResult result = Parallel.For(1, 20, RunTask);
            Console.WriteLine($"All task start and finish: {result.IsCompleted}");
        }
    }
}
