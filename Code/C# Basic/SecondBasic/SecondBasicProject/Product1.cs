using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondBasicProject
{
    // # Class / Partial class
    public partial class Product
    {
        partial void myMethod(); // khai báo
        public string Name { set; get; }
        public bool Order(int number = 0)
        {
            return true;
        }

    }
}