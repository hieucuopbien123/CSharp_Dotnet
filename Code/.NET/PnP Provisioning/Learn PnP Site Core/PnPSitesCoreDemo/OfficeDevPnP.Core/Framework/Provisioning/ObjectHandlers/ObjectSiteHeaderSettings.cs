#if !ONPREMISES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectSiteHeaderSettings : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Site Header"; }
        }

        public override string InternalName => "SiteHeader";

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            using (var scope = new PnPMonitoredScope(this.Name))
            {
                web.EnsureProperties(w => w.HeaderEmphasis, w => w.HeaderLayout, w => w.MegaMenuEnabled);
                var header = new SiteHeader();
                header.MenuStyle = web.MegaMenuEnabled ? SiteHeaderMenuStyle.MegaMenu : SiteHeaderMenuStyle.Cascading;
                switch (web.HeaderLayout)
                {
                    case HeaderLayoutType.Compact:
                        {
                            header.Layout = SiteHeaderLayout.Compact;
                            break;
                        }
                    case HeaderLayoutType.Standard:
                    default:
                        {
                            header.Layout = SiteHeaderLayout.Standard;
                            break;
                        }
                }

                if (Enum.TryParse<Emphasis>(web.HeaderEmphasis.ToString(), out Emphasis backgroundEmphasis))
                {
                    header.BackgroundEmphasis = backgroundEmphasis;
                }

                template.Header = header;
            }
            return template;
        }

        public override TokenParser ProvisionObjects(Web web, ProvisioningTemplate template, TokenParser parser, ProvisioningTemplateApplyingInformation applyingInformation)
        {
            using (var scope = new PnPMonitoredScope(this.Name))
            {
                web.EnsureProperties(w => w.Url);

                if (template.Header != null)
                {
                    switch (template.Header.Layout)
                    {
                        case SiteHeaderLayout.Compact:
                            {
                                web.HeaderLayout = HeaderLayoutType.Compact;
                                break;
                            }
                        case SiteHeaderLayout.Standard:
                            {
                                web.HeaderLayout = HeaderLayoutType.Standard;
                                break;
                            }
                    }
                    web.HeaderEmphasis = (SPVariantThemeType)Enum.Parse(typeof(SPVariantThemeType), template.Header.BackgroundEmphasis.ToString());
                    web.MegaMenuEnabled = template.Header.MenuStyle == SiteHeaderMenuStyle.MegaMenu;

                    var jsonRequest = new
                    {
                        headerLayout = web.HeaderLayout,
                        headerEmphasis = web.HeaderEmphasis,
                        megaMenuEnabled = web.MegaMenuEnabled,
                    };

                    web.ExecutePostAsync("/_api/web/SetChromeOptions", System.Text.Json.JsonSerializer.Serialize(jsonRequest)).GetAwaiter().GetResult();


                    //if (PnPProvisioningContext.Current != null)
                    //{
                    //    // Get an Access Token for the SetChromeOptions request
                    //    var spoResourceUri = new Uri(web.Url).Authority;
                    //    var accessToken = PnPProvisioningContext.Current.AcquireToken(spoResourceUri, null);

                    //    if (accessToken != null)
                    //    {
                    //        // Prepare the JSON request for SetChromeOptions
                    //        var jsonRequest = new
                    //        {
                    //            headerLayout = web.HeaderLayout,
                    //            headerEmphasis = web.HeaderEmphasis,
                    //            megaMenuEnabled = web.MegaMenuEnabled,
                    //        };

                    //        // Build the URL of the SetChromeOptions API
                    //        var setChromeOptionsApiUrl = $"{web.Url}/_api/web/SetChromeOptions";

                    //        // Make the POST request to the SetChromeOptions API
                    //        // and fail in case of any exception
                    //        HttpHelper.MakePostRequest(setChromeOptionsApiUrl,
                    //            jsonRequest,
                    //            "application/json",
                    //            accessToken);
                    //    }
                    //}
                    //else
                    //{
                    //    web.Update();
                    //    web.Context.ExecuteQueryRetry();
                    //}
                }
            }
            return parser;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return true;
        }

        public override bool WillProvision(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation)
        {
            return template.Header != null;
        }
    }
}
#endif