using System;
using System.Threading.Tasks;

namespace SecondAdvanceProject
{
    // # OOP khác / Tính đa hình / Phân biệt override và new
    class BaseClass
    {
        public virtual void DoSomeThing()
        {
            Console.WriteLine("I'm solokop");
        }
        public virtual void DoSomeThing2()
        {
            Console.WriteLine("I'm solokop");
        }
    }
    class DerivedClass : BaseClass
    {
        public new void DoSomeThing()
        {
            Console.WriteLine("I'm koploso");
        }
        public override void DoSomeThing2()
        {
            Console.WriteLine("I'm koploso");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            BaseClass b = new DerivedClass();
            DerivedClass d = new DerivedClass();
            b.DoSomeThing();
            d.DoSomeThing();
            b.DoSomeThing2();
            d.DoSomeThing2();


            // # Lập trình async / Dùng TAP / Dùng Parallel
            // K có task nào khởi tạo trong main thread nên k có taskid nào
            Console.WriteLine(Task.CurrentId);

            //UseParallel.ParallelFor();
            //UseParallelAsync.ParallelFor();
            UseParallelInvoke.ParallelInvoke();

            // # Using DLL
            MathLib.BasicFunc a = new MathLib.BasicFunc();
            Console.WriteLine(a.Add(1, 2));
        }
    }
}
