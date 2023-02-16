using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Utility
{
    public class XmlUtility
    {
        public static T ToObject<T>(string xml)
        {
            object obj = ToObject(xml, typeof(T));
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }

        public static Object ToObject(string data, Type type)
        {
            Object o = new Object();
            using (StringReader sr = new StringReader(data))
            {
                XmlSerializer seria = new XmlSerializer(type);
                try
                {
                    o = seria.Deserialize(sr);
                }
                catch (Exception e)
                {

                }
            }
            return o;
        }
    }
}
