using Microsoft.SharePoint.Client;
using PnPSitesCoreDemo.Modules.CreateListModules;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Modules
{
    public class HomeElementTest: ICreateListModule
    {
        [XmlElement(ElementName = "ListTest")]
        public List<ListTest> ListTestElements { get; set; }

        public void CreateList(ClientContext context)
        {
            foreach(ListTest lt in ListTestElements)
            {
                lt.CreateNewList(context);
            }
        }
    }

    [XmlRoot(ElementName = "ElementTest")]
    public class ElementTest
    {
        [XmlElement(ElementName = "HomeElementTest")]
        public HomeElementTest HomeElementTest { get; set; }
    }
}
