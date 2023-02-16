using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondIntermediateProject
{
    // # Dùng Collection
    class UseCollections: IFormattable
    {
        public string name { set; get; }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = "O";
            switch (format)
            {
                case "O":
                    return $"This is O format {name}";
                case "N":
                    return $"This is N format {name}";
                default:
                    throw new FormatException("Not support format");
            }
        }

    }
}
