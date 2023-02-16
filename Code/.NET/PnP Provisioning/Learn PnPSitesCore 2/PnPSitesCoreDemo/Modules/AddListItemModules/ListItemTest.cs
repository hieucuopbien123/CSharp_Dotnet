using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Modules.AddListItemModules
{
    public class MetaData
    {
        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }
        [XmlAttribute(AttributeName = "Time")]
        public string Time { get; set; }
        [XmlAttribute(AttributeName = "Answer")]
        public string Answer { get; set; }
        [XmlAttribute(AttributeName = "Question")]
        public string Question { get; set; }
        [XmlAttribute(AttributeName = "Image")]
        public string Image { get; set; }
        [XmlAttribute(AttributeName = "Day")]
        public string Day { get; set; }
    }
    public class ListItemTest
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "Metadata")]
        public List<MetaData> MetaDataOfList { get; set; }
        public void AddToList (ClientContext context)
        {
            Console.WriteLine($"Add item to list {Type}...");
            //switch (Type)
            //{
            //    case "News":
            //    case "Announcement":
            //        AddToAnnouncementTestList(context);
            //        break;
            //    case "EventsForHw5":
            //        AddToEventsTestList(context);
            //        break;
            //    case "HowDoI":
            //        AddToHowDoIList(context);
            //        break;
            //    case "ImageGallery":
            //    case "VideoGallery":
            //        AddToImageGalleryList(context);
            //        break;
            //    default:
            //        throw new Exception("Incorrect Type of list");
            //}

            // Dùng như dưới viết gọn hơn dù k chuẩn OOP Design

            try
            {
                var serverUrl = ConfigurationManager.AppSettings["serverUrl"];

                if (!context.Site.RootWeb.ListExists(Type))
                {
                    throw new Exception($"List {Type} not exist");
                }

                Web web = context.Web;
                List myList = web.Lists.GetByTitle(Type);

                foreach (MetaData md in MetaDataOfList)
                {
                    ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
                    ListItem myItem = myList.AddItem(itemCreationInfo);

                    if (md.Title != null)
                    {
                        myItem["Title"] = md.Title;
                        myItem.Update();
                    }
                    if (md.Description != null)
                    {
                        myItem["Description"] = md.Description;
                        myItem.Update();
                    }
                    if (md.Date != null)
                    {
                        myItem["Date"] = DateTime.Parse(md.Date);
                        myItem.Update();
                    }
                    if (md.Time != null)
                    {
                        myItem["Interval"] = md.Time;
                        myItem.Update();
                    }
                    if(md.Answer != null)
                    {
                        myItem["Answer"] = md.Answer;
                        myItem.Update();
                    }
                    if(md.Question != null)
                    {
                        myItem["Question"] = md.Question;
                        myItem.Update();
                    }
                    if(md.Day != null)
                    {
                        myItem["Date"] = md.Day;
                        myItem.Update();
                    }
                    if (md.Image != null)
                    {
                        Folder folder;

                        if (!context.Site.RootWeb.ListExists("ImageAssets"))
                        {
                            ListTemplateType listTemplate = ListTemplateType.DocumentLibrary;
                            bool enableVersioning = false;
                            List list = context.Site.RootWeb.CreateList(listTemplate, "ImageAssets", enableVersioning);
                        }

                        folder = context.Web.Lists.GetByTitle("ImageAssets").RootFolder;
                        var uploadedFileInfo = folder.UploadFile(md.Image, $"../../Assets/Image/{md.Image}", true);
                        string uploadedFileInfoJson = "{" +
                                                        $"\"serverUrl\":\"{serverUrl}\"," +
                                                        $"\"serverRelativeUrl\":\"{uploadedFileInfo.ServerRelativeUrl}\"}}";
                        myItem["Image"] = uploadedFileInfoJson;
                        myItem.Update();
                    }
                }
                context.ExecuteQuery();
                Console.WriteLine($"Add all item to list {Type} successfully\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:: " + e.Message);
            }
        }
        
        //public void AddToImageGalleryList(ClientContext context)
        //{
        //    try
        //    {
        //        var serverUrl = ConfigurationManager.AppSettings["serverUrl"];

        //        if (!context.Site.RootWeb.ListExists(Type))
        //        {
        //            throw new Exception($"List {Type} not exist");
        //        }

        //        Web web = context.Web;
        //        List myList = web.Lists.GetByTitle(Type);

        //        foreach (MetaData md in MetaDataOfList)
        //        {
        //            ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
        //            ListItem myItem = myList.AddItem(itemCreationInfo);
        //            Folder folder;

        //            if (!context.Site.RootWeb.ListExists("ImageAssets"))
        //            {
        //                ListTemplateType listTemplate = ListTemplateType.DocumentLibrary;
        //                bool enableVersioning = false;
        //                List list = context.Site.RootWeb.CreateList(listTemplate, "ImageAssets", enableVersioning);
        //            }

        //            folder = context.Web.Lists.GetByTitle("ImageAssets").RootFolder;
        //            var uploadedFileInfo = folder.UploadFile(md.Image, $"../../Assets/Image/{md.Image}", true);
        //            string uploadedFileInfoJson = "{" +
        //                                            $"\"serverUrl\":\"{serverUrl}\"," +
        //                                            $"\"serverRelativeUrl\":\"{uploadedFileInfo.ServerRelativeUrl}\"}}";
        //            myItem["Image"] = uploadedFileInfoJson;
        //            myItem.Update();
        //        }
        //        context.ExecuteQuery();
        //        Console.WriteLine($"Add all item to list {Type} successfully\n");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error:: " + e.Message);
        //    }
        //}
        //public void AddToHowDoIList(ClientContext context)
        //{
        //    try
        //    {
        //        if (!context.Site.RootWeb.ListExists(Type))
        //        {
        //            throw new Exception($"List {Type} not exist");
        //        }

        //        Web web = context.Web;
        //        List myList = web.Lists.GetByTitle(Type);

        //        foreach (MetaData md in MetaDataOfList)
        //        {
        //            ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
        //            ListItem myItem = myList.AddItem(itemCreationInfo);
        //            myItem["Answer"] = md.Answer;
        //            myItem.Update();
        //            myItem["Question"] = md.Question;
        //            myItem.Update();
        //        }
        //        context.ExecuteQuery();
        //        Console.WriteLine($"Add all item to list {Type} successfully\n");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error:: " + e.Message);
        //    }
        //}
        //public void AddToEventsTestList(ClientContext context)
        //{
        //    try
        //    {
        //        if (!context.Site.RootWeb.ListExists(Type))
        //        {
        //            throw new Exception($"List {Type} not exist");
        //        }

        //        Web web = context.Web;
        //        List myList = web.Lists.GetByTitle(Type);

        //        foreach (MetaData md in MetaDataOfList)
        //        {
        //            ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
        //            ListItem myItem = myList.AddItem(itemCreationInfo);
        //            myItem["Date"] = md.Day;
        //            myItem.Update();
        //            myItem["Title"] = md.Title;
        //            myItem.Update();
        //            myItem["Interval"] = md.Time;
        //            myItem.Update();
        //        }
        //        context.ExecuteQuery();
        //        Console.WriteLine($"Add all item to list {Type} successfully\n");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error:: " + e.Message);
        //    }
        //}
        //public void AddToAnnouncementTestList(ClientContext context)
        //{
        //    try
        //    {
        //        var serverUrl = ConfigurationManager.AppSettings["serverUrl"];

        //        if (!context.Site.RootWeb.ListExists(Type))
        //        {
        //            throw new Exception($"List {Type} not exist");
        //        }

        //        Web web = context.Web;
        //        List myList = web.Lists.GetByTitle(Type);

        //        foreach(MetaData md in MetaDataOfList)
        //        {
        //            ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
        //            ListItem myItem = myList.AddItem(itemCreationInfo);
        //            myItem["Title"] = md.Title;
        //            myItem.Update();
        //            myItem["Description"] = md.Description;
        //            myItem.Update();
        //            myItem["Date"] = DateTime.Parse(md.Date);
        //            myItem.Update(); 
        //            Folder folder;

        //            if (!context.Site.RootWeb.ListExists("ImageAssets"))
        //            {
        //                ListTemplateType listTemplate = ListTemplateType.DocumentLibrary;
        //                bool enableVersioning = false;
        //                List list = context.Site.RootWeb.CreateList(listTemplate, "ImageAssets", enableVersioning);
        //            }

        //            folder = context.Web.Lists.GetByTitle("ImageAssets").RootFolder;

        //            var uploadedFileInfo = folder.UploadFile(md.Image, $"../../Assets/Image/{md.Image}", true);
        //            string uploadedFileInfoJson = "{" +
        //                                            $"\"serverUrl\":\"{serverUrl}\"," +
        //                                            $"\"serverRelativeUrl\":\"{uploadedFileInfo.ServerRelativeUrl}\"}}";
        //            myItem["Image"] = uploadedFileInfoJson;
        //            myItem.Update();
        //        }
        //        context.ExecuteQuery();
        //        Console.WriteLine($"Add all item to list {Type} successfully\n");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error:: " + e.Message);
        //    }
        //}
    }
}
