using Microsoft.SharePoint.Client;
using System;
using System.Net;
using OfficeDevPnP.Core.Pages;
using OfficeDevPnP.Core.ALM;
using System.Collections.Generic;

namespace PnPSitesCoreDemo
{
    // ## PnP Provisioning
    // # PnP Site Core
    public class Program 
    {
        static void Main(string[] args)
        {
            try {
                // Connect với sharepoint
                using (ClientContext context = new ClientContext("https://vndevcore2.sharepoint.com/sites/Test25") { Credentials = new SharePointOnlineCredentials("ryan.nguyen@vndevcore2.onmicrosoft.com", new NetworkCredential("", "Hieucuopbien123!").SecurePassword) })
                {
                    //// Lấy thông tin của web và site
                    //var web = context.Web;
                    //var site = context.Site;
                    //context.Load(site, s => s.Id);
                    //context.Load(site, s => s.ServerRelativeUrl);
                    //context.Load(web, w => w.Id, w => w.Url);
                    //context.ExecuteQuery();
                    //Console.WriteLine(site.ServerRelativeUrl);
                    //Console.WriteLine(web.Url);

                    //// Thao tác với Page
                    //string pageName = "Home.aspx";
                    //ClientSidePage newPage = ClientSidePage.Load(context, pageName);
                    //newPage.Publish();
                    //newPage.ClearPage();
                    //newPage.Save();
                    //newPage.Publish();

                    //// Thêm canvas vao page, thêm webpart mặc định vào canvas
                    //CanvasSection section = new CanvasSection(newPage, CanvasSectionTemplate.TwoColumn, 0);
                    //newPage.AddSection(section);
                    //ClientSideText webpart1 = new ClientSideText()
                    //{
                    //    Text = "This is a text"
                    //};
                    //newPage.AddControl(webpart1, section.Columns[0]);
                    //ClientSideWebPart webpart2 = newPage.InstantiateDefaultWebPart(DefaultClientSideWebParts.List);
                    //webpart2.Title = "This is a doc lib";
                    //webpart2.PropertiesJson = "{\"isDocumentLibrary\":true,\"showDefaultDocumentLibrary\":true,\"webpartHeightKey\":4";
                    //newPage.AddControl(webpart2, section.Columns[1]);
                    //newPage.Save();
                    //newPage.Publish();

                    //// Lấy các app đang có trong catalog
                    //var relativePageUrl = "/sites/Test25/SitePages/Home.aspx";
                    //var appManager = new AppManager(context);
                    //var available = appManager.GetAvailable(OfficeDevPnP.Core.Enums.AppCatalogScope.Tenant);
                    //foreach(var app in available)
                    //{
                    //    Console.WriteLine($"{app.Id} - {app.Title} - {app.AppCatalogVersion}");
                    //}

                    ////Add vào catalog
                    //var newApp = appManager.Add(".....sppkg", true, OfficeDevPnP.Core.Enums.AppCatalogScope.Tenant);

                    ////Add vào site 
                    //if (appManager.Install(newApp, OfficeDevPnP.Core.Enums.AppCatalogScope.Tenant))
                    //{
                    //    if (appManager.Deploy(newApp, false, OfficeDevPnP.Core.Enums.AppCatalogScope.Tenant))
                    //    {
                    //        Console.WriteLine("Deploy Successfull");
                    //        // Tương tự uninstall: https://www.youtube.com/watch?v=112PJHWTlLw&t=10s
                    //    }
                    //}

                    // Thêm 1 webpart component tạo bằng react vào page
                    //ClientSidePage newPage = ClientSidePage.Load(context, "Home.aspx");
                    //var components = newPage.AvailableClientSideComponents();
                    //foreach (var comp in components)
                    //{
                    //    Console.WriteLine($"{comp.Name} - {comp.ComponentType} - {comp.Id}");
                    //    if (comp.Name == "HowDoI")
                    //    {
                    //        CanvasSection section = new CanvasSection(newPage, CanvasSectionTemplate.ThreeColumn, 0);
                    //        newPage.AddSection(section);
                    //        ClientSideWebPart webpart = new ClientSideWebPart(comp);
                    //        newPage.AddControl(webpart, section.Columns[2]);
                    //        newPage.Save();
                    //        newPage.Publish();
                    //        Console.WriteLine("ok");
                    //    }
                    //}

                    //// Add list 2 cách
                    //// Add list C1
                    //string listName = "TestList";
                    //if (!context.Site.RootWeb.ListExists(listName))
                    //{
                    //    ListTemplateType listTemplate = ListTemplateType.GenericList;
                    //    bool enableVersioning = false; // No versioning
                    //    List list = context.Site.RootWeb.CreateList(listTemplate, listName, enableVersioning);
                    //    Console.WriteLine("Created list: " + list.Title);
                    //}

                    //// Add list C2
                    //Web web = context.Web;
                    //ListCreationInformation listInfo = new ListCreationInformation();
                    //listInfo.Title = "ListNormal";
                    //listInfo.Description = "This is description";
                    //listInfo.TemplateType = (int)ListTemplateType.GenericList; // Nh loại list
                    //List myList = web.Lists.Add(listInfo);
                    //myList.OnQuickLaunch = true;
                    //myList.Update();
                    //context.ExecuteQuery();

                    //// Add field to list: add nhiều field trùng tên được
                    //Web web = context.Web;
                    //List myList = web.Lists.GetByTitle("TestList");
                    //myList.Fields.AddFieldAsXml(@"<Field
                    //   Name='DateOpened'
                    //   DisplayName='Date Opened'
                    //   Type='DateTime'
                    //   Format='DateOnly'
                    //   Required='FALSE'
                    //>
                    // <Default>[today]</Default>
                    //</Field>", true, AddFieldOptions.DefaultValue);
                    //context.ExecuteQuery();

                    // Add item to list: vid youtube
                    // Add multiple items to list
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
