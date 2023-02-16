using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstIntermediateProject
{
    // # Event 
    public class Publisher
    {
        public delegate void NotifyNews(object data);
        public NotifyNews event_news;
        public void Send()
        {
            event_news?.Invoke("Co tin moi");
        }
    }
    public class SubscriberA
    {
        public void Sub(Publisher p)
        {
            p.event_news += ReceiverFromPublisher;
        }
        void ReceiverFromPublisher(object data)
        {
            Console.WriteLine("SubscriberA: " + data.ToString());
        }
    }
    public class SubscriberB
    {
        public void Sub(Publisher p)
        {
            p.event_news = null;  // Hủy các đối tượng khác nhận sự kiện
            p.event_news += ReceiverFromPublisher;
        }
        void ReceiverFromPublisher(object data)
        {
            Console.WriteLine("SubscriberB: " + data.ToString());
        }
    }

}
