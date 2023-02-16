using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Modules
{
    public class ListField
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "DisplayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "Format")]
        public string Format { get; set; }
        [XmlAttribute(AttributeName = "Required")]
        public string Required { get; set; }

        [XmlAttribute(AttributeName = "Default")]
        public string Default { get; set; }
    }
    public class Fields
    {
        [XmlElement(ElementName = "Field")]
        public List<ListField> LField { get; set; }
    }
    public class ListTest
    {
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Fields")]
        public Fields Field { get; set; }

        public void CreateNewList(ClientContext context)
        {
            try
            {
                // Create list
                Console.WriteLine($"Create list {Title}...");
                string listName = Title;

                if (context.Site.RootWeb.ListExists(listName))
                {
                    List tempList = context.Web.Lists.GetByTitle(listName);
                    tempList.DeleteObject();
                    context.ExecuteQuery();
                }

                ListTemplateType listTemplate = ListTemplateType.GenericList;
                bool enableVersioning = false;
                List list = context.Site.RootWeb.CreateList(listTemplate, listName, enableVersioning);

                // Add fields to list
                Web web = context.Web;
                List myList = web.Lists.GetByTitle(listName);

                if (myList.FieldExistsByName("Title"))
                {
                    Field titleField = myList.Fields.GetByTitle("Title");
                    context.Load(titleField);
                    context.ExecuteQuery();
                    titleField.SchemaXml =
                        @"<Field
                            Name='Title'
                            DisplayName='Title'
                            Type='Text'
                            Required='FALSE'
                        ></Field>";
                    titleField.Hidden = true;
                    context.ExecuteQuery();
                }

                foreach (ListField field in Field.LField)
                {
                    Console.WriteLine($"Adding field {field.Name} to list...");

                    if (!myList.FieldExistsByName(field.Name))
                    {
                        myList.Fields.AddFieldAsXml(
                            $"<Field Name='{field.Name}' DisplayName='{field.Name}' Type='{field.Type}' " 
                            + (
                                field.Format != null
                                ? $"Format='{field.Format}'"
                                : ""
                               )
                            + $" Required='{field.Required}'>"
                            + (
                                field.Default != null
                                ? $"<Default>{field.Default}</Default>"
                                : ""
                               ) 
                             + "</Field>"
                            , true
                            , AddFieldOptions.DefaultValue
                        );
                        context.ExecuteQuery();
                    }
                    else
                    {
                        Field tempField = myList.Fields.GetByTitle(field.Name);
                        context.Load(tempField);
                        context.ExecuteQuery();
                        tempField.SchemaXml =
                            $"<Field Name='{field.Name}' DisplayName='{field.Name}' Type='{field.Type}' "
                            + (
                                field.Format != null
                                ? $"Format='{field.Format}'"
                                : ""
                               )
                            + $" Required='{field.Required}'>"
                            + (
                                field.Default != null
                                ? $"<Default>{field.Default}</Default>"
                                : ""
                               )
                             + "</Field>";
                        tempField.Hidden = false;
                        context.ExecuteQuery();
                    }
                }
                Console.WriteLine($"Create list {Title} successfully\n");
            } catch (Exception e)
            {
                Console.WriteLine("Error:: " + e.Message);
            }
        }
    }
}
