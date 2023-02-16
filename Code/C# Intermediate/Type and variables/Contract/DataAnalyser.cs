using System;
using System.Collections.Generic;
using System.Text;

namespace Contract
{
    public interface IDataAnalyser
    {
        /// <summary>
        /// The path of folder which stores data files
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Analyse data and 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTopTenStrings();
    }
}
