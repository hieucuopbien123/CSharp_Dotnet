using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPSitesCoreDemo.Modules.DeployModules
{
    public interface IDeployModule
    {
        void Deploy(ClientContext context);
    }
}
