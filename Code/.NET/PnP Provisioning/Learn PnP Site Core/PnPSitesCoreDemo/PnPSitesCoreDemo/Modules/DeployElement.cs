using PnPSitesCoreDemo.Modules.DeployModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Modules
{
    public abstract class IElements { }
    public class DeployElements : IElements
    {
        [XmlElement(ElementName = "SiteColumnModule")]
        public SiteColumnModule siteColumnModule { set; get; }
    }

    [XmlRoot(ElementName = "Elements")]
    public class Elements
    {
        [XmlElement(ElementName = "HomeElements")]
        public DeployElements DeployElements { get; set; }
    }
}
