using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecondAdvanceProject
{
    class UseParallelAsync
    {
        // # Lập trình async / Dùng TAP / Dùng Parallel / Dùng Parallel.For 
        // Parallel For mà dùng async Action
        public static void PintInfo(string info) =>
            Console.WriteLine($"{info,10}    task:{Task.CurrentId,3}    " + $"thread: {Thread.CurrentThread.ManagedThreadId}");

        public static async void RunTask(int i)
        {
            PintInfo($"Start {i,3}");
            await Task.Delay(1); // Task delay thực chất là 1 Task nên có thể await 
            PintInfo($"Finish {i,3}");
        }

        public static void ParallelFor()
        {
            ParallelLoopResult result = Parallel.For(1, 20, RunTask);
            Console.WriteLine($"All task start and finish: {result.IsCompleted}");
        }
    }
}
