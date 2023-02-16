using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionProject
{
    interface IClassC
    {
        public void ActionC();
    }
    class ClassC: IClassC
    {
        public ClassC()
        {
            Console.WriteLine("Create Class C");
        }
        public void ActionC()
        {
            Console.WriteLine("Action in class C");
        }
    }
    class ClassB: IClassC
    {
        public ClassB()
        {
            Console.WriteLine("Create Class B");
        }
        public void ActionC()
        {
            Console.WriteLine("Action in class B");
        }
    }
    class ClassA
    {
        public ClassA()
        {
            Console.WriteLine("Create Class A");
        }
        public virtual void ActionA()
        {
            Console.WriteLine("Action in class A");
        }
    }
}
