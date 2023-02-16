using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPSitesCoreDemo.Modules.AddListItemModules
{
    public interface IAddListItemModule
    {
        void AddListItem(ClientContext context);
    }
}
