using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecondAdvanceProject
{
    class UseParallelInvoke
    {
        public static void PintInfo(string info) =>
            Console.WriteLine($"{info,10}    task:{Task.CurrentId,3}    " + $"thread: {Thread.CurrentThread.ManagedThreadId}");

        public static async void RunTask(string s)
        {
            PintInfo($"Start {s,10}");
            await Task.Delay(1);
            PintInfo($"Finish {s,10}");
        }

        public static void actionA()
        {
            PintInfo($"Finish {"ActionA",10}");
        }

        public static void actionB()
        {
            PintInfo($"Finish {"ActionB",10}");
        }

        public static void ParallelInvoke()
        {
            Action action1 = () => {
                RunTask("Action1");
            };

            // Tùy loại hàm mà nhận vào kiểu delegate có param và giá trị trả về tương ứng. Ở đây Invoke nhận vào Action<void> mà RunTask nhận string nên ta buộc phải lưu nó thành 1 biến Action thỏa mãn.
            Parallel.Invoke(action1, actionA, actionB);
        }
    }
}
