using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPSitesCoreDemo.Modules.CreateListModules
{
    public interface ICreateListModule
    {
        void CreateList(ClientContext context);
    }
}
