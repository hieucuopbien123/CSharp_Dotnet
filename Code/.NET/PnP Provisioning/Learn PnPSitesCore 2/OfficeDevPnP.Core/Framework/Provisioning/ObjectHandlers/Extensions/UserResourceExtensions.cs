using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.Extensions
{
    internal static class UserResourceExtensions
    {
#if !SP2013
        public static ProvisioningTemplate SaveResourceValues(ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            var tempFolder = Path.GetTempPath();
            List<Tuple<string, int, string>> resourceTokens = creationInfo.ResourceTokens;

            IEnumerable<int> languages = resourceTokens.Select(t => t.Item2).Distinct();
            foreach (int language in languages)
            {
                var culture = new CultureInfo(language);

                var resourceFileName = Path.Combine(tempFolder, $"{creationInfo.ResourceFilePrefix}.{culture.Name}.resx");
                if (System.IO.File.Exists(resourceFileName))
                {
                    // Read existing entries, if any
#if !NETSTANDARD2_0
                    using (ResXResourceReader resxReader = new ResXResourceReader(resourceFileName))
#else
                    using (ResourceReader resxReader = new ResourceReader(resourceFileName))
#endif
                    {
                        foreach (DictionaryEntry entry in resxReader)
                        {
                            // find if token is already there
                            var existingToken = resourceTokens.FirstOrDefault(t => t.Item1 == entry.Key.ToString() && t.Item2 == language);
                            if (existingToken == null)
                            {
                                resourceTokens.Add(new Tuple<string, int, string>(entry.Key.ToString(), language, entry.Value as string));
                            }
                        }
                    }
                }

                // Create new resource file
#if !NETSTANDARD2_0
                using (ResXResourceWriter resx = new ResXResourceWriter(resourceFileName))
#else
                using (ResourceWriter resx = new ResourceWriter(resourceFileName))
#endif
                {
                    foreach (var token in resourceTokens.Where(t => t.Item2 == language))
                    {

                        resx.AddResource(token.Item1, token.Item3);
                    }
                }

                template.Localizations.Add(new Localization() { LCID = language, Name = culture.NativeName, ResourceFile = $"{creationInfo.ResourceFilePrefix}.{culture.Name}.resx" });

                // Persist the file using the connector
                using (FileStream stream = System.IO.File.Open(resourceFileName, FileMode.Open))
                {
                    creationInfo.FileConnector.SaveFileStream($"{creationInfo.ResourceFilePrefix}.{culture.Name}.resx", stream);
                }
                // remove the temp resx file
                System.IO.File.Delete(resourceFileName);
            }
            return template;
        }

        public static bool SetUserResourceValue(this UserResource userResource, string tokenValue, TokenParser parser)
        {
            bool isDirty = false;

            if (userResource != null && !String.IsNullOrEmpty(tokenValue))
            {
                var resourceValues = parser.GetResourceTokenResourceValues(tokenValue);
                foreach (var resourceValue in resourceValues)
                {
                    userResource.SetValueForUICulture(resourceValue.Item1, resourceValue.Item2);
                    isDirty = true;
                }
            }

            return isDirty;
        }

        public static bool PersistResourceValue(UserResource userResource, string token, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            bool returnValue = false;
            List<Tuple<string, int, string>> resourceTokens = creationInfo.ResourceTokens;

            foreach (var language in template.SupportedUILanguages)
            {
                var culture = new CultureInfo(language.LCID);

                var value = userResource.GetValueForUICulture(culture.Name);
                userResource.Context.ExecuteQueryRetry();
                if (!string.IsNullOrEmpty(value.Value))
                {
                    returnValue = true;
                    resourceTokens.Add(new Tuple<string, int, string>(token, language.LCID, value.Value));
                }
            }

            return returnValue;
        }

        public static bool PersistResourceValue(string token, int lcid, string title, ProvisioningTemplateCreationInformation creationInfo)
        {
            bool returnValue = false;

            if (!string.IsNullOrWhiteSpace(title))
            {
                returnValue = true;
                creationInfo.ResourceTokens.Add(new Tuple<string, int, string>(token, lcid, title));
            }

            return returnValue;
        }

        public static bool PersistResourceValue(List siteList, Guid viewId, string token, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            bool returnValue = false;
            var clientContext = siteList.Context;
            List<Tuple<string, int, string>> resourceTokens = creationInfo.ResourceTokens;

            foreach (var language in template.SupportedUILanguages)
            {
                var currentView = siteList.GetViewById(viewId);
                clientContext.Load(currentView, cc => cc.Title);
                var acceptLanguage = clientContext.PendingRequest.RequestExecutor.WebRequest.Headers["Accept-Language"];
                clientContext.PendingRequest.RequestExecutor.WebRequest.Headers["Accept-Language"] = new CultureInfo(language.LCID).Name;
                clientContext.ExecuteQueryRetry();

                if (!string.IsNullOrWhiteSpace(currentView.Title))
                {
                    returnValue = true;
                    resourceTokens.Add(new Tuple<string, int, string>(token, language.LCID, currentView.Title));
                }

                clientContext.PendingRequest.RequestExecutor.WebRequest.Headers["Accept-Language"] = acceptLanguage;

            }
            return returnValue;
        }

#endif
        public static bool ContainsResourceToken(this string value)
        {
            if (value != null)
            {
                return Regex.IsMatch(value, "\\{(res|loc|resource|localize|localization):(.*?)(\\})", RegexOptions.IgnoreCase);
            }
            else
            {
                return false;
            }
        }
    }
}
