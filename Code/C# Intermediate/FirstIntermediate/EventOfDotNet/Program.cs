using System;

namespace EventOfDotNet
{
    class Program
    {
        public class MyEventArgs : EventArgs
        {
            public MyEventArgs(string data)
            {
                this.data = data;
            }
            private string data;
            public string Data
            {
                get { return data; }
            }
        }

        public class ClassA
        {
            public event EventHandler event_news;
            public void Send()
            {
                event_news?.Invoke(this, new MyEventArgs("Có tin mới Abc ..."));
            }
        }

        public class ClassB
        {
            public void Sub(ClassA p)
            {
                p.event_news += ReceiverFromPublisher;
            }

            private void ReceiverFromPublisher(object sender, MyEventArgs e)
            {
                Console.WriteLine("ClassB: " + e.Data);
            }
        }
        public class ClassC
        {
            public void Sub(ClassA p)
            {
                p.event_news += ReceiverFromPublisher;
            }

            private void ReceiverFromPublisher(object sender, MyEventArgs e)
            {
                Console.WriteLine("ClassC: " + e.Data);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
