using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;

namespace UseObservableCollection
{
    public class A
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        // # Basic / Dùng yield
        public IEnumerable<int> GetSingleDigitNumbers()
        {
            int index = 0;
            while (index < 10) yield return index++;
            yield return 50;
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            foreach (int i in p.GetSingleDigitNumbers())
            {
                Console.Write(i);
                Console.Write(" ");
            }

            // # Dùng Collection / Dùng ObservableCollection
            ObservableCollection<string> obs = new ObservableCollection<string>();

            obs.CollectionChanged += change;

            obs.Add("ZTest1");
            obs.Add("DTest2");
            obs[1] = "AAAAA";

            obs.RemoveAt(1);
            obs.Clear();

            UseLINQ.testLINQ();

            // # Reflection trong C# / Dùng class Type
            A a = new A
            {
                Name = "HOTEN",
                ID = 10
            };
            foreach (PropertyInfo property in a.GetType().GetProperties())
            {
                string property_name = property.Name; // Lấy tên thuộc tính: Name và ID
                // Cái biến property lúc này chỉ lưu từng thuộc tính của class A chung, muốn lấy giá trị của instance class A nào thì phải gọi GetValue
                object property_value = property.GetValue(a); // Cx gán được với SetValue

                Console.WriteLine($"Thuộc tính {property_name} giá trị là {property_value}");
            }
        }

        private static void change(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (String s in e.NewItems)
                        Console.WriteLine($"Thêm :  {s}");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("Clear");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (String s in e.OldItems)
                        Console.WriteLine($"Remove :  {s}");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine("Repaced - " + e.NewItems[0]);
                    break;
            }
        }
    }
}