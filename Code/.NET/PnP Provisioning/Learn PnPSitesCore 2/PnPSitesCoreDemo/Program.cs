using Microsoft.SharePoint.Client;
using System;
using System.Configuration;
using System.Security;
using System.Xml;
using PnPSitesCoreDemo.Modules;
using System.Reflection;
using PnPSitesCoreDemo.Utility;
using System.Diagnostics;
using OfficeDevPnP.Core.ALM;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Pages;
using PnPSitesCoreDemo.Modules.AddListItemModules;

namespace PnPSitesCoreDemo
{
    class Program
    {
        // ## PnP Provisioning
        // # PnP Site Core
        static void Main(string[] _)
        {

            /*  ╔══════════════════════════════╗
                ║     Connect to sharepoint    ║
                ╚══════════════════════════════╝ */
            try
            {
                var siteUrl = ConfigurationManager.AppSettings["siteUrl"];
                var username = ConfigurationManager.AppSettings["accountName"];
                Console.WriteLine($"SharePoint Online User Name: {username}");
                Console.WriteLine("Please input user's password: ");
                var password = GetPassword();
                using (ClientContext context = new ClientContext(siteUrl) { Credentials = new SharePointOnlineCredentials(username, password) })
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    /*  ╔═══════════════════════════════════════════╗
                        ║   Install package to app catalog và page  ║
                        ╚═══════════════════════════════════════════╝ */
                    // SetupPackage(context);

                    /*  ╔═════════════════════════════════════╗
                        ║   Tạo và khai báo giá trị cho list  ║
                        ╚═════════════════════════════════════╝ */
                    CreateList(context);
                    Console.WriteLine("");
                    AddItemToList(context);
                    Console.WriteLine("");

                    /*  ╔══════════════╗
                        ║   Tạo page   ║
                        ╚══════════════╝ */
                    ClientSidePage newPage = CreatePage(context);
                    Console.WriteLine("");

                    // Thao tác với layout
                    /*  ╔════════════════════════════════════╗
                        ║   Add layout và webpart vào page   ║
                        ╚════════════════════════════════════╝ */
                    BuildPage(newPage);
                    Console.WriteLine("");

                    stopwatch.Stop();
                    Console.WriteLine(string.Format("Total time: {0} s", stopwatch.ElapsedTicks / 10000000));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error::" + e.Message);
            }
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();
            try
            {
                Console.Write("SharePoint Password : ");

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        sStrPwd.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                sStrPwd = null;
                Console.WriteLine("Error:: " + e.Message);
            }

            return sStrPwd;
        }
        private static void SetupPackage(ClientContext context)
        {
            // Overwrite app catalog
            Console.WriteLine("\n");
            Console.WriteLine("Waiting to add package to app catalog...");
            var packageName = ConfigurationManager.AppSettings["packageName"];
            ListItem listItem = WebExtensions.DeployApplicationPackageToAppCatalog(context.Web, packageName, "../../Assets/SPPKGFiles/", true, true);
            Console.WriteLine("Add package to app catalog successfully");
            Console.WriteLine("");

            // Add to page if it haven't been added
            Console.WriteLine("Add package to site page...");
            var appManager = new AppManager(context);
            var availableApp = appManager.GetAvailable((Guid)listItem["UniqueId"], AppCatalogScope.Tenant);
            if (availableApp.InstalledVersion == null)
            {
                appManager.Install(availableApp, OfficeDevPnP.Core.Enums.AppCatalogScope.Tenant);
            }
            Console.WriteLine("Add package to site page successfully");
            Console.WriteLine("\n");
        }

        static private void CreateList(ClientContext context)
        {
            try
            {
                var constConfig = new XmlDocument();
                constConfig.Load(@"../../Configuration/ListConfig.xml");

                ElementTest configrations = XmlUtility.ToObject<ElementTest>(constConfig.InnerXml);

                using (context)
                {
                    Type xmlType = configrations.GetType();
                    foreach (PropertyInfo property in xmlType.GetProperties())
                    {
                        if (property.PropertyType.GetInterface("ICreateListModule") != null)
                        {
                            MethodInfo mi = property.PropertyType.GetMethod("CreateList");
                            if (mi != null && property.GetValue(configrations) != null)
                            {
                                mi.Invoke(property.GetValue(configrations), new object[] { context });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error::" + e.Message);
            }
        }

        static private void AddItemToList(ClientContext context)
        {
            try
            {
                var constConfig = new XmlDocument();
                constConfig.Load(@"../../Configuration/ListItemConfig.xml");

                ListItemWrapper configrations = XmlUtility.ToObject<ListItemWrapper>(constConfig.InnerXml);

                using (context)
                {
                    Type xmlType = configrations.GetType();
                    foreach (PropertyInfo property in xmlType.GetProperties())
                    {
                        if (property.PropertyType.GetInterface("IAddListItemModule") != null)
                        {
                            MethodInfo mi = property.PropertyType.GetMethod("AddListItem");
                            if (mi != null && property.GetValue(configrations) != null)
                            {
                                mi.Invoke(property.GetValue(configrations), new object[] { context });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error::" + e.Message);
            }
        }
        static private ClientSidePage CreatePage(ClientContext context)
        {
            ClientSidePage newPage = null;
            try
            {
                var pageName = ConfigurationManager.AppSettings["pageName"];
                Console.WriteLine($"Start to create page {pageName}...");
                bool pageMissing = false;
                try
                {
                    newPage = ClientSidePage.Load(context, pageName);
                }
                catch (Exception _)
                {
                    pageMissing = true;
                }
                if (pageMissing == true)
                {
                    newPage = new ClientSidePage(context)
                    {
                        LayoutType = ClientSidePageLayoutType.Home
                    };
                    newPage.Save(pageName);
                }
                newPage.ClearPage();
                newPage.DisableComments();
                newPage.Save();
                newPage.Publish();
                Console.WriteLine($"Created page {pageName} successfully");
            } catch(Exception e)
            {
                Console.WriteLine("Error:: " + e.Message);
            }
            return newPage;
        }
        static private void BuildPage(ClientSidePage page)
        {
            try
            {
                Console.WriteLine($"Start to build webpart for the page...");
                CanvasSection section = new CanvasSection(page, CanvasSectionTemplate.TwoColumnLeft, 0);
                page.AddSection(section);

                var Announcement = page.AvailableClientSideComponents("Announcement");
                foreach (var comp in Announcement)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[0]);
                }
                var News = page.AvailableClientSideComponents("News");
                foreach (var comp in News)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[0]);
                }
                var ImageGallery = page.AvailableClientSideComponents("ImageGallery");
                foreach (var comp in ImageGallery)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[0]);
                }
                var VideoGallery = page.AvailableClientSideComponents("VideoGallery");
                foreach (var comp in VideoGallery)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[0]);
                }

                var QuickLinks = page.AvailableClientSideComponents("QuickLinks");
                foreach (var comp in QuickLinks)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[1]);
                }
                var Events = page.AvailableClientSideComponents("Events");
                foreach (var comp in Events)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[1]);
                }
                var HowDoI = page.AvailableClientSideComponents("HowDoI");
                foreach (var comp in HowDoI)
                {
                    ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    page.AddControl(webpart, section.Columns[1]);
                }

                Console.WriteLine($"Build page successfully");
                page.Save();
                page.Publish();
                Console.WriteLine($"Page published successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error::" + e.Message);
            }
        }
    }
}

/*
 * Nếu tên list bị thay đổi: sẽ phải đổi trong ListConfig.xml, ListItemConfig.xml, ListItemTest.cs
 */