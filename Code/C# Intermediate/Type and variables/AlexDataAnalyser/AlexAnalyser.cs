using Contract;
using System;
using System.Collections.Generic;

namespace AlexDataAnalyser
{
    public class AlexAnalyser : IDataAnalyser
    {
        public string Path { get; set; }

        public IEnumerable<string> GetTopTenStrings()
        {
            string[] result = { "aa", "bb", "cc", "dd" };
            return result;
        }
    }
}
