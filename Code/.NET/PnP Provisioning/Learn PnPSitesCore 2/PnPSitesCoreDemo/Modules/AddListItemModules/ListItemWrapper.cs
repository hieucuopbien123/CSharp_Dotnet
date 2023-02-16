using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Modules.AddListItemModules
{
    public class HomeListItem : IAddListItemModule
    {
        [XmlElement(ElementName = "ListItemTest")]
        public List<ListItemTest> ListItemTest { get; set; }

        public void AddListItem(ClientContext context)
        {
            foreach (ListItemTest lit in ListItemTest)
            {
                lit.AddToList(context);
            }
        }
    }

    [XmlRoot(ElementName = "ListItemWrapper")]
    public class ListItemWrapper
    {
        [XmlElement(ElementName = "HomeListItem")]
        public HomeListItem HomeListItem { get; set; }
    }
}
