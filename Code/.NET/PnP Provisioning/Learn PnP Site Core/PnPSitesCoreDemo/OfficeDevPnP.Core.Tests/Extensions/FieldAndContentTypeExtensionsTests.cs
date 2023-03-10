using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using System.Configuration;
using OfficeDevPnP.Core.Tests;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core;

namespace Microsoft.SharePoint.Client.Tests
{
	[TestClass()]
	public class FieldAndContentTypeExtensionsTests
	{
		const string DOC_LIB_TITLE = "Test_Library";
		const string TEST_CATEGORY = "Fields and Content Types";
		const string TEST_CT_PNP = "Test_CT_PNP";
		const string TEST_CT_PNP_ID = "0x01010080BA6ECAEDA6487EAD28FC3C21CA1900";

		#region Test initialize and cleanup
		// **** IMPORTANT ****
		// In order to succesfully clean up after testing, create all artifacts that end up in the test site with a name starting with "Test_"
		// **** IMPORTANT ****
		[TestCleanup]
		public void Cleanup()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var web = clientContext.Web;
				clientContext.Load(web);
				clientContext.ExecuteQueryRetry();
				EmptyRecycleBin(clientContext);

				// delete lists
				var lists = clientContext.LoadQuery(clientContext.Web.Lists);
				clientContext.ExecuteQueryRetry();
				var testLists = lists.Where(l => l.Title.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
				foreach (var list in testLists)
				{
					list.DeleteObject();
				}
				clientContext.ExecuteQueryRetry();

				// first delete content types
				var contentTypes = clientContext.LoadQuery(clientContext.Web.ContentTypes);
				clientContext.ExecuteQueryRetry();
				var testContentTypes = contentTypes.Where(l => l.Name.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
				foreach (var ctype in testContentTypes)
				{
					ctype.DeleteObject();
				}

				clientContext.ExecuteQueryRetry();

				// delete fields
				var fields = clientContext.LoadQuery(clientContext.Web.Fields);
				clientContext.ExecuteQueryRetry();
				var testFields = fields.Where(f => f.InternalName.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
				foreach (var field in testFields)
				{
					field.DeleteObject();
				}
				clientContext.ExecuteQueryRetry();

				// clean recycle bin
				EmptyRecycleBin(clientContext);

			}
		#endregion
		}

		#region Field tests
		[TestMethod()]
		public void CreateFieldTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var fieldName = "Test_" + DateTime.Now.ToFileTime();
				var fieldId = Guid.NewGuid();

				var fieldCI = new FieldCreationInformation(FieldType.Choice)
				{
					Id = fieldId,
					InternalName = fieldName,
					DisplayName = fieldName,
					AddToDefaultView = true,
					Group = "Test fields group"
				};
				var fieldChoice = clientContext.Web.CreateField<FieldChoice>(fieldCI);

				var field = clientContext.Web.Fields.GetByTitle(fieldName);
				clientContext.Load(field);
				clientContext.ExecuteQueryRetry();

				Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
				Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
				Assert.AreEqual("Choice", fieldChoice.TypeAsString, "Failed to create a FieldChoice object.");
			}
		}

		[TestMethod()]
		public void CanAddContentTypeToListByName()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var listName = "Test_" + DateTime.Now.ToFileTime();
				clientContext.Web.CreateList(ListTemplateType.GenericList, listName, enableContentTypes: true, enableVersioning: false);
                var issueContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Issue);
                clientContext.Load(issueContentType, ct => ct.Name);
                clientContext.ExecuteQueryRetry();

                clientContext.Web.AddContentTypeToListByName(listName, issueContentType.Name, defaultContent: true);

				var list = clientContext.Web.GetListByTitle(listName);
				clientContext.Load(list.ContentTypes);
				clientContext.ExecuteQueryRetry();
				var issueContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(issueContentType.Name));

				Assert.AreEqual(1, issueContentTypeCount, "Issue content type was not added.");
			}
		}

		[TestMethod()]
		public void CanRemoveContentTypeFromListByName()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var listName = "Test_" + DateTime.Now.ToFileTime();
				clientContext.Web.CreateList(ListTemplateType.GenericList, listName, enableContentTypes: true, enableVersioning: false);

                var issueContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Issue);
                var taskContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Task);
                var itemContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Item);

                clientContext.Load(issueContentType, ct => ct.Name);
                clientContext.Load(taskContentType, ct => ct.Name);
                clientContext.Load(itemContentType, ct => ct.Name);

                clientContext.ExecuteQueryRetry();

                clientContext.Web.AddContentTypeToListByName(listName, issueContentType.Name, defaultContent: true);
				clientContext.Web.AddContentTypeToListByName(listName, taskContentType.Name, defaultContent: true);
				clientContext.Web.RemoveContentTypeFromListByName(listName, itemContentType.Name);

				var list = clientContext.Web.GetListByTitle(listName);
				clientContext.Load(list.ContentTypes);
				clientContext.ExecuteQueryRetry();
				var issueContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(issueContentType.Name));
				var taskContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(taskContentType.Name));
				var itemContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(itemContentType.Name));

				Assert.AreEqual(1, issueContentTypeCount, "Issue content type was not added. Test is invalid.");
				Assert.AreEqual(1, taskContentTypeCount, "Task content type was not added. Test is invalid.");
				Assert.AreEqual(0, itemContentTypeCount, "Item content type was not removed.");
			}
		}

		[TestMethod()]
		public void CanRemoveContentTypeFromListById()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var listName = "Test_" + DateTime.Now.ToFileTime();
				clientContext.Web.CreateList(ListTemplateType.GenericList, listName, enableContentTypes: true, enableVersioning: false);

                var issueContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Issue);
                var taskContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Task);
                var itemContentType = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Item);

                clientContext.Load(issueContentType, ct => ct.Name);
                clientContext.Load(taskContentType, ct => ct.Name);
                clientContext.Load(itemContentType, ct => ct.Name);

                clientContext.ExecuteQueryRetry();

                clientContext.Web.AddContentTypeToListByName(listName, issueContentType.Name, defaultContent: true);
				clientContext.Web.AddContentTypeToListByName(listName, taskContentType.Name, defaultContent: true);
				clientContext.Web.RemoveContentTypeFromListById(listName, "0x01");

				var list = clientContext.Web.GetListByTitle(listName);
				clientContext.Load(list.ContentTypes);
				clientContext.ExecuteQueryRetry();
				var issueContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(issueContentType.Name));
				var taskContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(taskContentType.Name));
				var itemContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals(itemContentType.Name));

				Assert.AreEqual(1, issueContentTypeCount, "Issue content type was not added. Test is invalid.");
				Assert.AreEqual(1, taskContentTypeCount, "Task content type was not added. Test is invalid.");
				Assert.AreEqual(0, itemContentTypeCount, "Item content type was not removed.");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException), "Field was able to be created twice without exception.")]
		public void CreateExistingFieldTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var fieldName = "Test_ABC123";
				var fieldId = Guid.NewGuid();

				FieldCreationInformation fieldCI = new FieldCreationInformation(FieldType.Choice)
				{
					Id = fieldId,
					InternalName = fieldName,
					AddToDefaultView = true,
					DisplayName = fieldName,
					Group = "Test fields group"
				};
				var fieldChoice1 = clientContext.Web.CreateField<FieldChoice>(fieldCI);
				var fieldChoice2 = clientContext.Web.CreateField<FieldChoice>(fieldCI);

				var field = clientContext.Web.Fields.GetByTitle(fieldName);
				clientContext.Load(field);
				clientContext.ExecuteQueryRetry();
			}
		}

		[TestMethod]
		public void GetContentTypeByIdTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
				var ct = clientContext.Web.GetContentTypeById(TEST_CT_PNP_ID,true);
				Assert.IsInstanceOfType(ct,typeof(ContentType));
			}
		}

		//FIXME: Tests does not revert target to a clean slate after running.
		//FIXME: Tests are tighthly coupled to eachother

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RemoveFieldByInternalNameThrowsOnNoMatchTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var web = clientContext.Web;

				try
				{
					web.RemoveFieldByInternalName("FieldThatDoesNotExistEver");
				}
				catch (ArgumentException ex)
				{
					Assert.AreEqual(ex.Message, "Could not find field with internalName FieldThatDoesNotExistEver");
					throw;
				}
			}
		}

		[TestMethod]
		public void CreateFieldFromXmlTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var fieldId = Guid.NewGuid();
				var fieldXml = string.Format("<Field xmlns='http://schemas.microsoft.com/sharepoint/' ID='{0}' Name='Test_FieldFromXML' StaticName='Test_FieldFromXML' DisplayName='Test Field From XML' Group='Test_Group' Type='Text' Required='TRUE' DisplaceOnUpgrade='TRUE' />", fieldId.ToString("B").ToUpper());

				var field = clientContext.Web.CreateField(fieldXml);

				Assert.IsNotNull(field);
				Assert.IsInstanceOfType(field, typeof(Field));

			}
		}
		#endregion

		#region Contenttype tests
		[TestMethod]
		public void ContentTypeExistsByNameTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
				Assert.IsTrue(clientContext.Web.ContentTypeExistsByName(TEST_CT_PNP));
			}
		}

		[TestMethod]
		public void ContentTypeExistsByIdTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
				Assert.IsTrue(clientContext.Web.ContentTypeExistsById(TEST_CT_PNP_ID));
			}
		}

		[TestMethod]
		public void ContentTypeExistsByNameInSubWebTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);

				string subsiteurl = "Test_Pnp_" + Guid.NewGuid().ToString();
				var subweb = clientContext.Web.Webs.Add(new WebCreationInformation()
				{
					Title = "Test Content type lookups",
					Url = subsiteurl,
				});

				try
				{
					clientContext.Load(subweb);
					clientContext.ExecuteQueryRetry();

					using (var clientContextSub = clientContext.Clone(String.Format("{0}\\{1}", TestCommon.AppSetting("SPODevSiteUrl"), subsiteurl)))
					{
						Assert.IsFalse(clientContextSub.Web.ContentTypeExistsByName(TEST_CT_PNP));
						Assert.IsTrue(clientContextSub.Web.ContentTypeExistsByName(TEST_CT_PNP, true));
					}
				}
				finally
				{
                    // Eat exception to deal with the sporadic error "The object is not associated with an object identity or the object identity is invalid"                    
                    try
                    {
                        subweb.DeleteObject();
                        clientContext.ExecuteQueryRetry();
                    }
                    catch { }
				}
			}
		}

		[TestMethod]
		public void ContentTypeExistsByIdInSubWebTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);

				string subsiteurl = "Test_Pnp_" + Guid.NewGuid().ToString();
				var subweb = clientContext.Web.Webs.Add(new WebCreationInformation()
				{
					Title = "Test Content type lookups",
					Url = subsiteurl,
				});

				try
				{
					clientContext.Load(subweb);
					clientContext.ExecuteQueryRetry();

					using (var clientContextSub = clientContext.Clone(String.Format("{0}\\{1}", TestCommon.AppSetting("SPODevSiteUrl"), subsiteurl)))
					{
						Assert.IsFalse(clientContextSub.Web.ContentTypeExistsById(TEST_CT_PNP_ID));
						Assert.IsTrue(clientContextSub.Web.ContentTypeExistsById(TEST_CT_PNP_ID, true));
					}
				}
				finally
				{
                    // Eat exception to deal with the sporadic error "The object is not associated with an object identity or the object identity is invalid"                    
                    try
                    {
                        subweb.DeleteObject();
                        clientContext.ExecuteQueryRetry();
                    }
                    catch { }
				}
			}
		}

        [TestMethod]
        public void DeleteContentTypeByNameTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
                Assert.IsTrue(clientContext.Web.ContentTypeExistsByName(TEST_CT_PNP));
                // delete the content type
                clientContext.Web.DeleteContentTypeByName(TEST_CT_PNP);
                Assert.IsFalse(clientContext.Web.ContentTypeExistsByName(TEST_CT_PNP));
            }
        }

        [TestMethod]
        public void DeleteContentTypeByIdTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
                Assert.IsTrue(clientContext.Web.ContentTypeExistsById(TEST_CT_PNP_ID));
                // delete the content type
                clientContext.Web.DeleteContentTypeById(TEST_CT_PNP_ID);
                Assert.IsFalse(clientContext.Web.ContentTypeExistsById(TEST_CT_PNP_ID));
            }
        }

        [TestMethod]
		public void ContentTypeExistsByNameSearchInSiteHierarchyTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
				Assert.IsTrue(clientContext.Web.ContentTypeExistsByName(TEST_CT_PNP, true));
			}
		}

		[TestMethod]
		public void ContentTypeExistsByIdSearchInSiteHierarchyTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
				Assert.IsTrue(clientContext.Web.ContentTypeExistsById(TEST_CT_PNP_ID, true));
			}
		}

		[TestMethod]
		public void AddFieldToContentTypeTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);

				var fieldName = "Test_" + DateTime.Now.ToFileTime();
				var fieldId = Guid.NewGuid();

				var fieldCI = new FieldCreationInformation(FieldType.Text)
				{
					Id = fieldId,
					InternalName = fieldName,
					DisplayName = fieldName,
					AddToDefaultView = true,
					Group = "Test fields group"
				};
				var fieldText = clientContext.Web.CreateField<FieldText>(fieldCI);

				clientContext.Web.AddFieldToContentTypeByName(TEST_CT_PNP, fieldId);
				Assert.IsTrue(clientContext.Web.FieldExistsByNameInContentType(TEST_CT_PNP, fieldName));
			}
		}

		[TestMethod]
		public void AddFieldToContentTypeMakeRequiredTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);

				var fieldName = "Test_" + DateTime.Now.ToFileTime();
				var fieldId = Guid.NewGuid();

				var fieldCI = new FieldCreationInformation(FieldType.Text)
				{
					Id = fieldId,
					InternalName = fieldName,
					DisplayName = fieldName,
					AddToDefaultView = true,
					Group = "Test fields group"
				};
				var fieldText = clientContext.Web.CreateField<FieldText>(fieldCI);

				// simply add the field to the content type
				clientContext.Web.AddFieldToContentTypeByName(TEST_CT_PNP, fieldId);

				// add the same field, but now with required setting to true and hidden to true
				clientContext.Web.AddFieldToContentTypeByName(TEST_CT_PNP, fieldId, true);

				// Fetch the created field and verify the state of the hidden and required properties
				ContentType ct = clientContext.Web.GetContentTypeByName(TEST_CT_PNP);
				FieldCollection fields = ct.Fields;
				IEnumerable<Field> results = ct.Context.LoadQuery<Field>(fields.Where(item => item.Id == fieldId));
				ct.Context.ExecuteQueryRetry();
				Assert.IsTrue(results.FirstOrDefault().Required);
			}
		}

        [TestMethod]
        public void SetDefaultContentTypeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                var testList = web.CreateList(ListTemplateType.DocumentLibrary, "Test_SetDefaultContentTypeToListTestList", true, true, "", true);

                var parentCt = web.GetContentTypeById("0x0101");
                var ct = web.CreateContentType("Test_SetDefaultContentTypeToListCt", "Desc", "", "Test_Group", parentCt);
                clientContext.Load(ct);
                clientContext.Load(testList.RootFolder, f => f.ContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                var prevUniqueContentTypeOrder = testList.RootFolder.ContentTypeOrder;

                Assert.AreEqual(1, prevUniqueContentTypeOrder.Count());

                var newContentType = testList.ContentTypes.AddExistingContentType(ct);
                clientContext.Load(newContentType, nct => nct.Id);
                clientContext.ExecuteQueryRetry();

                testList.SetDefaultContentType(newContentType.Id);
                clientContext.Load(testList.RootFolder, f => f.ContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                Assert.AreEqual(2, testList.RootFolder.ContentTypeOrder.Count());
                Assert.IsTrue(testList.RootFolder.ContentTypeOrder.First().StringValue.StartsWith(ct.Id.StringValue, StringComparison.OrdinalIgnoreCase));

                testList.DeleteObject();
                ct.DeleteObject();
                clientContext.ExecuteQueryRetry();
            }
        }

        [TestMethod]
        public void SetDefaultContentTypeToListTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                var testList = web.CreateList(ListTemplateType.DocumentLibrary, "Test_SetDefaultContentTypeToListTestList", true, true, "", true);

                var parentCt = web.GetContentTypeById("0x0101");
                var ct = web.CreateContentType("Test_SetDefaultContentTypeToListCt", "Desc", "", "Test_Group", parentCt);
                clientContext.Load(ct);
                clientContext.Load(testList.RootFolder, f => f.ContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                var prevUniqueContentTypeOrder = testList.RootFolder.ContentTypeOrder;

                Assert.AreEqual(1, prevUniqueContentTypeOrder.Count());

                testList.AddContentTypeToList(ct);

                testList.SetDefaultContentTypeToList(ct);
                clientContext.Load(testList.RootFolder, f => f.ContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                Assert.AreEqual(2, testList.RootFolder.ContentTypeOrder.Count());
                Assert.IsTrue(testList.RootFolder.ContentTypeOrder.First().StringValue.StartsWith(ct.Id.StringValue, StringComparison.OrdinalIgnoreCase));

                testList.DeleteObject();
                ct.DeleteObject();
                clientContext.ExecuteQueryRetry();
            }
        }

        [TestMethod()]
		public void ReorderContentTypesTest()
		{
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var web = clientContext.Web;
                var documentCtype = clientContext.Web.AvailableContentTypes.GetById(BuiltInContentTypeId.Document);
                clientContext.Load(documentCtype, ct => ct.Name);
                clientContext.ExecuteQueryRetry();

                // create content types

				var newCtypeInfo1 = new ContentTypeCreationInformation()
				{
					Name = "Test_ContentType1",
					ParentContentType = documentCtype,
					Group = "Test content types",
					Description = "This is a test content type"
				};
				var newCtypeInfo2 = new ContentTypeCreationInformation()
				{
					Name = "Test_ContentType2",
					ParentContentType = documentCtype,
					Group = "Test content types",
					Description = "This is a test content type"
				};
				var newCtypeInfo3 = new ContentTypeCreationInformation()
				{
					Name = "Test_ContentType3",
					ParentContentType = documentCtype,
					Group = "Test content types",
					Description = "This is a test content type"
				};

				var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
				var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
				var newCtype3 = web.ContentTypes.Add(newCtypeInfo3);
				clientContext.Load(newCtype1);
				clientContext.Load(newCtype2);
				clientContext.Load(newCtype3);
				clientContext.ExecuteQueryRetry();

				var newList = new ListCreationInformation()
				{
					TemplateType = (int)ListTemplateType.DocumentLibrary,
					Title = DOC_LIB_TITLE,
					Url = "TestLibrary"
				};

				var doclib = clientContext.Web.Lists.Add(newList);
				doclib.ContentTypesEnabled = true;
				doclib.ContentTypes.AddExistingContentType(newCtype1);
				doclib.ContentTypes.AddExistingContentType(newCtype2);
				doclib.ContentTypes.AddExistingContentType(newCtype3);
				doclib.Update();
				clientContext.Load(doclib.ContentTypes);
				clientContext.ExecuteQueryRetry();

				var expectedIds = new string[]{
					newCtype3.Name,
					newCtype1.Name,
					newCtype2.Name,
					documentCtype.Name
				};

				doclib.ReorderContentTypes(expectedIds);
				var reorderedCtypes = clientContext.LoadQuery(doclib.ContentTypes);
				clientContext.ExecuteQueryRetry();

				var actualIds = reorderedCtypes.Except(
					// remove the folder content type
										reorderedCtypes.Where(ct => ct.Id.StringValue.StartsWith("0x012000"))
									).Select(ct => ct.Name).ToArray();

				CollectionAssert.AreEqual(expectedIds, actualIds);
			}
		}

		[TestMethod]
		public void CreateContentTypeByXmlTest()
		{
			var xml = @"<ContentType ID=""0x0101000728167cd9c94899925ba69c4af6743e"" Name=""Test_NewContentType"" Group=""Test Group"" Description=""Text Content Type"" Inherits=""TRUE"" Version=""0"">
	<FieldRefs>
	  <!--  Built-in Title field -->
	  <FieldRef ID=""{fa564e0f-0c70-4ab9-b863-0177e6ddd247}"" Name=""Title"" DisplayName=""Test"" Required=""TRUE"" Sealed=""TRUE""/>
	</FieldRefs>
  </ContentType>";
			using (var clientContext = TestCommon.CreateClientContext())
			{
				var web = clientContext.Web;
				var ct = web.CreateContentTypeFromXMLString(xml);
				Assert.IsNotNull(ct);
				clientContext.Load(ct.FieldLinks);
				clientContext.ExecuteQueryRetry();
				Assert.IsTrue(ct.FieldLinks.Count == 8); // Includes default fields

				ct.DeleteObject();
				clientContext.ExecuteQueryRetry();
			}

		}

        [TestMethod]
        public void CanGetContentTypeIdIsChildOf()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                ContentType itemContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Item); //0x01
                ContentType documentContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Document); //0x0101
                ContentType eventContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Event); ////0x0102
                clientContext.Load(itemContentType, ct => ct.Id);
                clientContext.Load(documentContentType, ct => ct.Id);
                clientContext.Load(eventContentType, ct => ct.Id);
                clientContext.ExecuteQueryRetry();
                
                //parent child
                Assert.IsTrue(documentContentType.Id.IsChildOf(itemContentType.Id));
                
                //child parent
                Assert.IsFalse(itemContentType.Id.IsChildOf(documentContentType.Id));
                
                //siblings
                Assert.IsFalse(eventContentType.Id.IsChildOf(documentContentType.Id));
            }
        }

        [TestMethod]

        public void CanGetContentTypeIdIsParentOf()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                ContentType itemContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Item); //0x01
                ContentType documentContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Document); //0x0101
                clientContext.Load(itemContentType, ct => ct.Id);
                clientContext.Load(documentContentType, ct => ct.Id);
                clientContext.ExecuteQueryRetry();
                Assert.IsTrue(itemContentType.Id.IsParentOf(documentContentType.Id));
            }
        }

        [TestMethod]

        public void CanGetContentTypeIdParentIdValue()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                ContentType itemContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Item); //0x01
                ContentType documentContentType = web.AvailableContentTypes.GetById(BuiltInContentTypeId.Document); //0x0101
                clientContext.Load(itemContentType, ct => ct.Id);
                clientContext.Load(documentContentType, ct => ct.Id);
                clientContext.ExecuteQueryRetry();
                string documentContentTypeParentIdValue = documentContentType.Id.GetParentIdValue();
                Assert.AreEqual(itemContentType.Id.StringValue, documentContentTypeParentIdValue);
            }
        }
        #endregion

        [TestMethod]
        public void CanSetDefaultContentTypeWhenContentTypeIsVisibleInNewButtonAndContentTypeOrderIsSet()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);
                doclib.ContentTypesEnabled = true;
                var listContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                clientContext.Load(listContentType1, ct => ct.Id);
                doclib.Update();
                clientContext.Load(doclib.RootFolder, rf => rf.ContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                doclib.RootFolder.UniqueContentTypeOrder = doclib.RootFolder.ContentTypeOrder;
                doclib.RootFolder.Update();
                clientContext.ExecuteQueryRetry();

                var listContentType2 = doclib.ContentTypes.AddExistingContentType(newCtype2);
                clientContext.Load(listContentType2, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                doclib.SetDefaultContentType(listContentType1.Id);

                clientContext.Load(doclib.RootFolder, rf => rf.ContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                var contentTypeOrder = doclib.RootFolder.ContentTypeOrder;

                var actualDefaultContentType = contentTypeOrder[0];

                Assert.AreEqual(2, contentTypeOrder.Count);
                Assert.AreEqual(listContentType1.Id.StringValue, actualDefaultContentType.StringValue);
            }
        }

        [TestMethod]
        public void CanSetDefaultContentTypeWhenNoUniqueOrderIsSet()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                clientContext.Load(newCtype1);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);

                var listContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                clientContext.Load(listContentType1, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                Assert.IsNull(doclib.RootFolder.UniqueContentTypeOrder);

                //Set default content type
                doclib.SetDefaultContentType(listContentType1.Id);

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                clientContext.ExecuteQueryRetry();
                var actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;

                Assert.AreEqual(listContentType1.Id.StringValue, actualContentTypeOrder[0].StringValue);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "A nonexisting content type was incorrectly set as default.")]

        public void ThrowsArgumentOutOfRangeExceptionWhenSettingDefaultContentThatDoesNotExistInList()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                clientContext.Load(newCtype1);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);

                var listContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                clientContext.Load(listContentType1, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                var listDocumentContentType = doclib.ContentTypes.BestMatch(BuiltInContentTypeId.Document);
                // Assert.AreEqual(BuiltInContentTypeId.Document, listDocumentContentType.GetParentIdValue(), true);
                Assert.AreEqual(newCtype1.Id.StringValue, listDocumentContentType.GetParentIdValue(), true);
                doclib.ContentTypes.GetById(listDocumentContentType.StringValue).DeleteObject();
                clientContext.ExecuteQueryRetry();

                //Set default content type
                doclib.SetDefaultContentType(listDocumentContentType);
            }
        }

        [TestMethod]
        public void CanSetDefaultContentTypeWhenContentTypeIsHiddenInNewButton()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);

                var listContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);

                clientContext.Load(listContentType1, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                //Set default content type
                doclib.SetDefaultContentType(listContentType1.Id);

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                clientContext.ExecuteQueryRetry();
                var actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;

                Assert.AreEqual(listContentType1.Id.StringValue, actualContentTypeOrder[0].StringValue);

                //Add content type hidden in the new button (new content types are not automatically added to new button if unique order is set)
                var listContentType2 = doclib.ContentTypes.AddExistingContentType(newCtype2);
                clientContext.Load(listContentType2, ct => ct.Id);
                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;
                bool isContentType2VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(listContentType2.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;

                Assert.IsFalse(isContentType2VisibleInNewButton, "Content type 2 has incorrectly been made visible in the new button");

                //Set default content type
                doclib.SetDefaultContentType(listContentType2.Id);

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;
                isContentType2VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(listContentType2.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;

                Assert.IsTrue(isContentType2VisibleInNewButton, "Content type 2 has not been made visible in the new button");

            }
        }

        [TestMethod]
        public void CanUpdateDefaultContentTypeWithoutModifyingContentTypeNewButtonVisibility()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo3 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType3",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                var newCtype3 = web.ContentTypes.Add(newCtypeInfo3);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.Load(newCtype3);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);

                var listContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                clientContext.Load(listContentType1, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                doclib.SetDefaultContentType(listContentType1.Id);


                //These content types will not be visible unless added to RootFolder.UniqueContentTypeOrder
                var listContentType2 = doclib.ContentTypes.AddExistingContentType(newCtype2);
                var listContentType3 = doclib.ContentTypes.AddExistingContentType(newCtype3);
                doclib.Update();

                clientContext.Load(listContentType2, ct => ct.Id);
                clientContext.Load(listContentType3, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                doclib.SetDefaultContentType(listContentType2.Id);

                clientContext.Load(doclib.ContentTypes);
                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                clientContext.ExecuteQueryRetry();

                var actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;
                bool isContentType3VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(listContentType3.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;

                bool contentType3ExistsInList = doclib.ContentTypeExistsById(newCtype3.Id.StringValue);

                Assert.IsFalse(isContentType3VisibleInNewButton, "Content type 3 has incorrectly been made visible in the new button");
                Assert.IsTrue(contentType3ExistsInList, "Content type 3 should have been added to the list content types");
            }
        }

        [TestMethod]
        public void CanAddContentTypeAndMakeVisibleInNewButton()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo3 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType3",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                var newCtype3 = web.ContentTypes.Add(newCtypeInfo3);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.Load(newCtype3);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);
                doclib.ContentTypesEnabled = true;
                var libContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                var libContentType2 = doclib.ContentTypes.AddExistingContentType(newCtype2);
                doclib.Update();
                clientContext.Load(libContentType1, ct => ct.Id);
                clientContext.Load(libContentType2, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                doclib.RootFolder.UniqueContentTypeOrder = new[] { libContentType1.Id };
                doclib.RootFolder.Update();
                clientContext.ExecuteQueryRetry();

                doclib.AddContentTypeToList(newCtype3, false);

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                var actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;
                var isContentType1VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(libContentType1.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;
                var isContentType2VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(libContentType2.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;
                var isContentType3VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.GetParentIdValue().Equals(newCtype3.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;

                Assert.AreEqual(2, doclib.RootFolder.UniqueContentTypeOrder.Count);
                Assert.IsTrue(isContentType1VisibleInNewButton, "Content type 3 has not been made visible in the new button");
                Assert.IsFalse(isContentType2VisibleInNewButton, "Content type 2 has incorrectly been made visible in the new button");
                Assert.IsTrue(isContentType3VisibleInNewButton, "Content type 3 has not been made visible in the new button");
            }
        }

        [TestMethod]
        public void CanShowContentTypesInNewButton()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo3 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType3",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                var newCtype3 = web.ContentTypes.Add(newCtypeInfo3);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.Load(newCtype3);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);
                doclib.ContentTypesEnabled = true;
                var libContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                var libContentType2 = doclib.ContentTypes.AddExistingContentType(newCtype2);
                doclib.Update();
                clientContext.Load(libContentType1, ct => ct.Id);
                clientContext.Load(libContentType2, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                doclib.ShowContentTypesInNewButton(new[] { libContentType1 });

                //there are no unique order set so all buttons are visible
                Assert.IsNull(doclib.RootFolder.UniqueContentTypeOrder, "UniqueContentTypeOrder should be null since all buttons are visible by default");

                doclib.RootFolder.UniqueContentTypeOrder = new[] { libContentType1.Id };
                doclib.RootFolder.Update();
                clientContext.ExecuteQueryRetry();

                doclib.ShowContentTypesInNewButton(new[] { libContentType2 });

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                var actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;
                var isContentType1VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(libContentType1.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;
                var isContentType2VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(libContentType2.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;

                Assert.AreEqual(2, doclib.RootFolder.UniqueContentTypeOrder.Count);
                Assert.IsTrue(isContentType1VisibleInNewButton, "Content type 1 has not been made visible in the new button");
                Assert.IsTrue(isContentType2VisibleInNewButton, "Content type 2 has not been made visible in the new button");
            }
        }

        [TestMethod]
        public void CanHideContentTypesInNewButton()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                // create content types
                var documentCtype = web.ContentTypes.GetById(BuiltInContentTypeId.Document);
                var newCtypeInfo1 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation()
                {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.ExecuteQueryRetry();

                var newList = new ListCreationInformation()
                {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);
                doclib.ContentTypesEnabled = true;
                var libContentType1 = doclib.ContentTypes.AddExistingContentType(newCtype1);
                var libContentType2 = doclib.ContentTypes.AddExistingContentType(newCtype2);
                doclib.Update();
                clientContext.Load(libContentType1, ct => ct.Id);
                clientContext.Load(libContentType2, ct => ct.Id);
                clientContext.ExecuteQueryRetry();

                doclib.HideContentTypesInNewButton(new[] { libContentType1 });

                clientContext.Load(doclib.RootFolder, rf => rf.UniqueContentTypeOrder);
                var actualContentTypeOrder = doclib.RootFolder.UniqueContentTypeOrder;
                var isDocumentContentTypeVisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.GetParentIdValue().Equals(BuiltInContentTypeId.Document, StringComparison.OrdinalIgnoreCase)) != null;
                var isContentType1VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(libContentType1.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;
                var isContentType2VisibleInNewButton = actualContentTypeOrder.FirstOrDefault(ct => ct.StringValue.Equals(libContentType2.Id.StringValue, StringComparison.OrdinalIgnoreCase)) != null;

                Assert.AreEqual(2, doclib.RootFolder.UniqueContentTypeOrder.Count);
                Assert.IsTrue(isDocumentContentTypeVisibleInNewButton, "Document content type has not been made visible in the new button");
                Assert.IsFalse(isContentType1VisibleInNewButton, "Content type 1 has incorrectly been made visible in the new button");
                Assert.IsTrue(isContentType2VisibleInNewButton, "Content type 2 has not been made visible in the new button");
            }
        }

        #region Helper methods
        void EmptyRecycleBin(ClientContext clientContext)
		{
			var recycleBin = clientContext.Web.RecycleBin;
			clientContext.Load(recycleBin);
			clientContext.ExecuteQueryRetry();

			var items = recycleBin.ToArray();

			for (var i = 0; i < items.Length; i++)
				items[i].DeleteObject();

			clientContext.ExecuteQueryRetry();
		}
		#endregion
	}
}
