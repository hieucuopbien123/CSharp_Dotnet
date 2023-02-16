using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstIntermediateProject
{
    // # Event
    public class MyEventArgs : EventArgs
    {
        public MyEventArgs(string data)
        {
            this.data = data;
        }
        private readonly string data; // K có modifier readonly, nó vẫn tự hiểu là readonly vì k có setter
        public string Data
        {
            get { return data; }
        }
    }
    public class ClassA
    {
        //public event EventHandler event_news; // Có thể thêm modifier event cũng được
        public EventHandler event_news;
        public void Send()
        {
            event_news?.Invoke(this, new MyEventArgs("Có tin moi Abc ..."));
        }
    }

    public class ClassB
    {
        public void Sub(ClassA p)
        {
            p.event_news += ReceiverFromPublisher;
        }
        void ReceiverFromPublisher(object sender, EventArgs e)
        {
            Console.WriteLine("ClassB: " + ((MyEventArgs)e).Data);
        }
    }
    public class ClassC
    {
        public void Sub(ClassA p)
        {
            p.event_news += ReceiverFromPublisher;
        }
        private void ReceiverFromPublisher(object sender, EventArgs e)
        {
            Console.WriteLine("ClassC: " + ((MyEventArgs)e).Data);
        }
    }
}
