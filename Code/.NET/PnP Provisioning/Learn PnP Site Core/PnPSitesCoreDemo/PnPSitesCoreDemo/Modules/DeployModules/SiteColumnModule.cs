using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PnPSitesCoreDemo.Modules.DeployModules
{
    public class SiteColumnModule : IDeployModule
    {
        [XmlElement(ElementName = "Field")]
        public List<SiteField> fields { get; set; }

        public void Deploy(ClientContext context)
        {
            foreach (SiteField sfield in this.fields)
            {
                switch (sfield.Type)
                {
                    case "TaxonomyFieldType":
                        CreateTaxonomyField(context, sfield);
                        break;
                    case "Note":
                        CreateNoteField(context, sfield);
                        break;
                    case "DateTime":
                        CreateDateField(context, sfield);
                        break;
                    case "Choice":
                        CreateChoiceField(context, sfield);
                        break;
                    case "Number":
                        CreateNumberField(context, sfield);
                        break;
                    case "HTML":
                        CreateHTMLField(context, sfield);
                        break;
                }
            }
        }

        private void CreateHTMLField(ClientContext context, SiteField sfield)
        {
            try
            {
                context.Load(context.Web.Fields);
                context.ExecuteQuery();

                Field field = context.Web.Fields.Where(x => x.InternalName == sfield.Name).FirstOrDefault();
                if (field != null)
                {
                    if (Boolean.Parse(sfield.OverWrite))
                    {
                        DeleteField(context, field);
                        HTMLField(context, sfield);
                    }
                }
                else
                {
                    HTMLField(context, sfield);
                }
            }
            catch (Exception ex)
            {
                //exception handler
            }
        }

        private void HTMLField(ClientContext context, SiteField sfield)
        {
            string columnHTMLSchema = @"<Field Type='HTML' Name='{0}' Group='Custom Columns' DisplayName='{1}' UnlimitedLengthInDocumentLibrary='TRUE' RichText='TRUE' RichTextMode = 'FullHtml' />";
            string HTMLSchema = string.Format(columnHTMLSchema, sfield.Name, sfield.DisplayName);
            Field Field = context.Web.Fields.AddFieldAsXml(HTMLSchema, true, AddFieldOptions.DefaultValue);
            context.Load(Field);
            context.ExecuteQuery();
        }

        private void CreateNumberField(ClientContext context, SiteField sfield)
        {
            try
            {
                context.Load(context.Web.Fields);
                context.ExecuteQuery();

                Field field = context.Web.Fields.Where(x => x.InternalName == sfield.Name).FirstOrDefault();
                if (field != null)
                {
                    if (Boolean.Parse(sfield.OverWrite))
                    {
                        DeleteField(context, field);
                        NumberField(context, sfield);
                    }
                }
                else
                {
                    NumberField(context, sfield);
                }
            }
            catch (Exception ex)
            {
                //exception handler
            }
        }

        private void NumberField(ClientContext context, SiteField sfield)
        {
            string columnNumberSchema = @"<Field Type='Number' Name='{0}' Group='Custom Columns' DisplayName='{1}' />";
            string NumberSchema = string.Format(columnNumberSchema, sfield.Name, sfield.DisplayName);
            Field Field = context.Web.Fields.AddFieldAsXml(NumberSchema, true, AddFieldOptions.DefaultValue);
            context.Load(Field);
            context.ExecuteQuery();
        }

        private void CreateChoiceField(ClientContext context, SiteField sfield)
        {
            try
            {
                context.Load(context.Web.Fields);
                context.ExecuteQuery();

                Field field = context.Web.Fields.Where(x => x.InternalName == sfield.Name).FirstOrDefault();
                if (field != null)
                {
                    if (Boolean.Parse(sfield.OverWrite))
                    {
                        DeleteField(context, field);
                        ChoiceField(context, sfield);
                    }
                }
                else
                {
                    ChoiceField(context, sfield);
                }
            }
            catch (Exception ex)
            {
                //exception handler
            }
        }

        private void ChoiceField(ClientContext context, SiteField sfield)
        {
            string columnSchema = @"<Field Type='Choice' Name='{0}' DisplayName='{1}' Group='Custom Columns' Format='RadioButtons'><Default>" + sfield.Choice + "</Default><CHOICES>{2}</CHOICES></Field>";
            string choices = "";
            foreach (ChoiceOption value in sfield.Choices)
            {
                choices = choices + string.Format("<CHOICE>{0}</CHOICE>", value.Value);
            }
            string ChoiceSchema = string.Format(columnSchema, sfield.Name, sfield.DisplayName, choices);
            Field Field = context.Web.Fields.AddFieldAsXml(ChoiceSchema, true, AddFieldOptions.DefaultValue);
            context.Load(Field);
            context.ExecuteQuery();
        }

        private void CreateDateField(ClientContext context, SiteField sfield)
        {
            try
            {
                context.Load(context.Web.Fields);
                context.ExecuteQuery();

                Field field = context.Web.Fields.Where(x => x.InternalName == sfield.Name).FirstOrDefault();
                if (field != null)
                {
                    if (Boolean.Parse(sfield.OverWrite))
                    {
                        DeleteField(context, field);
                        DateField(context, sfield);
                    }
                }
                else
                {
                    DateField(context, sfield);
                }
            }
            catch (Exception ex)
            {
                //exception handler
            }
        }

        private void DateField(ClientContext context, SiteField sfield)
        {
            string columnDateSchema = @"<Field Type='DateTime' Name='{0}' Format='DateOnly' Group='Custom Columns' DisplayName='{1}' />";
            string DateSchema = string.Format(columnDateSchema, sfield.Name, sfield.DisplayName);
            Field Field = context.Web.Fields.AddFieldAsXml(DateSchema, true, AddFieldOptions.DefaultValue);
            context.Load(Field);
            context.ExecuteQuery();
        }

        private void CreateNoteField(ClientContext context, SiteField sfield)
        {
            try
            {
                context.Load(context.Web.Fields);
                context.ExecuteQuery();

                Field field = context.Web.Fields.Where(x => x.InternalName == sfield.Name).FirstOrDefault();
                if (field != null)
                {
                    if (Boolean.Parse(sfield.OverWrite))
                    {
                        DeleteField(context, field);
                        NoteField(context, sfield);
                    }
                }
                else
                {
                    NoteField(context, sfield);
                }
            }
            catch (Exception ex)
            {
                //exception handler
            }
        }

        private void NoteField(ClientContext context, SiteField sfield)
        {
            string columnNoteSchema = @"<Field Type='Note' Name='{0}' Group='Custom Columns' DisplayName='{1}' UnlimitedLengthInDocumentLibrary='TRUE' />";
            string NoteSchema = string.Format(columnNoteSchema, sfield.Name, sfield.DisplayName);
            Field Field = context.Web.Fields.AddFieldAsXml(NoteSchema, true, AddFieldOptions.DefaultValue);
            context.Load(Field);
            context.ExecuteQuery();
        }

        private void CreateTaxonomyField(ClientContext context, SiteField sfield)
        {
            try
            {
                context.Load(context.Web.Fields);
                context.ExecuteQuery();

                Field field = context.Web.Fields.Where(x => x.InternalName == sfield.Name).FirstOrDefault();
                if (field != null)
                {
                    if (Boolean.Parse(sfield.OverWrite))
                    {
                        DeleteField(context, field);
                        TaxonomyField(context, sfield);
                    }
                }
                else
                {
                    TaxonomyField(context, sfield);
                }
            }
            catch (Exception ex)
            {
                //exception handler
            }
        }

        private void TaxonomyField(ClientContext context, SiteField sfield)
        {
            string columnTaxonomyGenericSchema = @"<Field Type='TaxonomyFieldType' Group='Custom Columns' Name='{0}' DisplayName='{1}' />";
            string columnTaxonomySchema = string.Format(columnTaxonomyGenericSchema, sfield.Name, sfield.DisplayName);

            //var session = TaxonomySession.GetTaxonomySession(client.ClientContext);
            //var store = session.TermStores.GetByName(sfield.TermStore);
            //var group = store.Groups.GetByName(sfield.GroupName);
            //var set = group.TermSets.GetByName(sfield.TermSet);

            //client.ClientContext.Load(store, s => s.Id);
            //client.ClientContext.Load(set, s => s.Id);
            //client.ClientContext.ExecuteQuery();


            Field Field = context.Web.Fields.AddFieldAsXml(columnTaxonomySchema, true, AddFieldOptions.DefaultValue);
            context.Load(Field);
            context.ExecuteQuery();
            //TaxonomyField taxField = client.ClientContext.CastTo<TaxonomyField>(Field);
            //taxField.SspId = store.Id;
            //taxField.TermSetId = set.Id;
            //taxField.AllowMultipleValues = false;
            //taxField.Open = true;

            //taxField.TargetTemplate = string.Empty;
            //taxField.AnchorId = Guid.Empty;
            //taxField.Update();
            //client.ClientContext.Web.Update();
            //client.ClientContext.ExecuteQuery();
        }

        private static void DeleteField(ClientContext context, Field field)
        {
            context.Load(field);
            context.ExecuteQuery();
            field.DeleteObject();
            context.ExecuteQuery();
        }
    }

    public class SiteField
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "DisplayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "TermStore")]
        public string TermStore { get; set; }
        [XmlAttribute(AttributeName = "GroupName")]
        public string GroupName { get; set; }

        [XmlAttribute(AttributeName = "TermSet")]
        public string TermSet { get; set; }
        [XmlAttribute(AttributeName = "OverWrite")]
        public string OverWrite { get; set; }
        [XmlAttribute(AttributeName = "DefaultChoice")]
        public string Choice { get; set; }
        [XmlElement(ElementName = "Choice")]
        public List<ChoiceOption> Choices { get; set; }
    }
    public class ChoiceOption
    {
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }
    }
}
