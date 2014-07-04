using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data.ContentLinks;
using Telerik.Sitefinity.Data.Metadata;
using Telerik.Sitefinity.Data.OA;
using Telerik.Sitefinity.Ecommerce.Catalog.Model;
using Telerik.Sitefinity.Ecommerce.Orders.Model;
using Telerik.Sitefinity.Ecommerce.Shipping.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Web.UI;
using Telerik.Sitefinity.Model.ContentLinks;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Modules.Blogs.Web.UI;
using Telerik.Sitefinity.Modules.Ecommerce.Catalog;
using Telerik.Sitefinity.Modules.Ecommerce.Catalog.Web.Services;
using Telerik.Sitefinity.Modules.Ecommerce.Catalog.Web.UI;
using Telerik.Sitefinity.Modules.Ecommerce.Catalog.Workflows;
using Telerik.Sitefinity.Modules.Ecommerce.Common;
using Telerik.Sitefinity.Modules.Ecommerce.Configuration;
using Telerik.Sitefinity.Modules.Ecommerce.Orders;
using Telerik.Sitefinity.Modules.Ecommerce.Orders.Interfaces;
using Telerik.Sitefinity.Modules.Ecommerce.Orders.Web.UI;
using Telerik.Sitefinity.Modules.Ecommerce.Orders.Web.UI.ShoppingCartSummaryViews;
using Telerik.Sitefinity.Modules.Ecommerce.Shipping;
using Telerik.Sitefinity.Modules.Events.Web.UI;
using Telerik.Sitefinity.Modules.Forms.Web.UI;
using Telerik.Sitefinity.Modules.Forms.Web.UI.Fields;
using Telerik.Sitefinity.Modules.GenericContent.Web.UI;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.Web.UI.Documents;
using Telerik.Sitefinity.Modules.Libraries.Web.UI.Images;
using Telerik.Sitefinity.Modules.Libraries.Web.UI.Videos;
using Telerik.Sitefinity.Modules.Lists.Web.UI;
using Telerik.Sitefinity.Modules.Lists.Web.UI.Expanded;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.News.Web.UI;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Configuration;
using Telerik.Sitefinity.Publishing.Model;
using Telerik.Sitefinity.Publishing.Pipes;
using Telerik.Sitefinity.Publishing.PublishingPoints;
using Telerik.Sitefinity.Publishing.Web.Services;
using Telerik.Sitefinity.Publishing.Web.Services.Data;
using Telerik.Sitefinity.Samples.Common;
using Telerik.Sitefinity.Security.Web.UI;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments.Proxies;
using Telerik.Sitefinity.Services.Search.Publishing;
using Telerik.Sitefinity.Services.Search.Web.UI.Public;
using Telerik.Sitefinity.Utilities.MS.ServiceModel.Web;
using Telerik.Sitefinity.Web.Model;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.Fields;
using Telerik.Sitefinity.Web.UI.NavigationControls;
using Telerik.Sitefinity.Web.UI.PublicControls;
using Telerik.Sitefinity.Workflow;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {
        private readonly Dictionary<string, string> TagIds = new Dictionary<string, string>
        {
            { "announcement", "913E28AA-0DE2-470d-9C3F-000000000001" },
            { "awards", "913E28AA-0DE2-470d-9C3F-000000000002" },
            { "economics", "913E28AA-0DE2-470d-9C3F-000000000003" },
            { "FIFA", "913E28AA-0DE2-470d-9C3F-000000000004" },
            { "homepage", "913E28AA-0DE2-470d-9C3F-000000000005" },
            { "japan", "913E28AA-0DE2-470d-9C3F-000000000006" },
            { "sports", "913E28AA-0DE2-470d-9C3F-000000000007" },
            { "tourism", "913E28AA-0DE2-470d-9C3F-000000000008" },
        };

        protected void Application_Start(object sender, EventArgs e)
        {
            SystemManager.ApplicationStart += new EventHandler<EventArgs>(this.SystemManager_ApplicationStart);
        }

        private void SystemManager_ApplicationStart(object sender, EventArgs e)
        {
            SystemManager.RunWithElevatedPrivilegeDelegate sampleWorker = new SystemManager.RunWithElevatedPrivilegeDelegate(this.CreateSampleWorker);
            SystemManager.RunWithElevatedPrivilege(sampleWorker);
            SystemManager.RunWithElevatedPrivilegeDelegate searchPipesWorker = new SystemManager.RunWithElevatedPrivilegeDelegate(this.CreateSearchPipes);
            SystemManager.RunWithElevatedPrivilege(searchPipesWorker);
        }

        private void CreateSearchPipes(object[] args)
        {
            this.CreateSearchPublishingPoint();
            this.AddSearchControlToDefaultEducationTemplate();
        }

        private void CreateSampleWorker(object[] args)
        {            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            // Create admin
            var adminId = SampleUtilities.CreateUsersAndRoles();
            //SampleUtilities.FrontEndAuthenticate();

            this.CreateTags();
            this.UploadImages();

            SampleUtilities.SetUserAvatar(adminId, "admin");

            // Users and roles
            this.CreateUsers();
            this.AddUsersToRoles();

            this.CreateEducationThemeAndTemplate();
            this.CreateFacebookThemeAndTemplate();

            this.UploadVideos();

            this.RegisterControls();
            this.RegisterControlTemplates();
            this.RegisterContentViews();
            this.CreateNewsItemCommentItems();
            this.CreateLists();
            this.CreateEvents();
            this.CreateBlogPosts();
            this.CreateContactForms();
            this.CreateDocuments();
            this.CreateFeeds();

            // PAGES
            this.CreateHomePage();

            // About the university pages
            this.CreateAboutTheUniversityPage();
            this.CreateWhyTIUPage();
            this.CreateEventsPage();
            this.CreateScholarshipsPage();
            this.CreateNewsPage();
            this.CreateBlogPage();

            // Academics pages
            this.CreateAcademicsPage();
            this.CreateAthleticsPage();
            this.CreateAcademicFacilitiesPage();
            this.CreateDistantLearningPage();
            this.CreateUndergraduateAndGraduate();

            // Admission pages
            this.CreateAdmissionsPage();
            this.CreateHowToApplyPage();
            this.CreateDiversityStatisticsPage();
            this.CreatePreArrivalInformationPage();
            this.CreateDocumentsBasePage();

            // Campus life pages
            this.CreateCampusLifePage();
            this.CreateCampusRulesPage();
            this.CreateCampusMapPage();
            this.CreateHousingOpportunitiesPage();

            // Internal resources
            this.CreateInternalResourcesGroupPage();
            this.CreateErrorPagesGroupPage();
            this.Create404Page();
            this.CreateErrorPage();

            // Contact us
            this.CreateContactUsPage();

            // Facebook fan page
            this.CreateFacebookFanPagesGroupPage();
            this.CreateTIUFacebookPage();

            this.AddContactUsPhonesToBackend();

            // Create TUI Shop page

            this.CreateShopFAQList();
            this.UploadShopImages();
            this.CreateShopPage();
            this.CreateEcommerceProductTypes();
            this.CreateEcommerceProducts();

            this.CreateEcommerceTax();
            this.CreateEcommercePaymentMethod();
            this.CreateEcommerceShippingMethod();

            this.SetDefaultSMTPSettings();
            this.SetEcommerceMerchantEmail();

            this.CreateShoppingCartPage();
            this.CreateInvoicePage();
            this.CreateCheckoutPage();
            this.CreateOrdersPage();
            this.CreateShopFAQPage();

            this.CreateSearchResultPage();
            this.CreateRegistrationPage();
            this.CreateProfilePage();
            this.CreateLoginPage();

            //resources cleanup
            this.CleanUpResources();
        }

        #region Login

        private void CreateRegistrationPage()
        {
            Guid parentPageID = new Guid(SampleConstants.InternalResourcesGroupPageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.RegistrationPageId), "Registration", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.RegistrationPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Account Registration</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.RegistrationPageId), pageTitle, "content", "Content block", "en");

                RegistrationForm registrationForm = new RegistrationForm();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.RegistrationPageId), registrationForm, "content", "Content block", "en");
            }

            hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.RegistrationPageId), "Registrierung", parentPageID, false, false, "de");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.RegistrationPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Registrierung</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.RegistrationPageId), pageTitle, "content", "Content block", "de");

                RegistrationForm registrationForm = new RegistrationForm();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.RegistrationPageId), registrationForm, "content", "Registrierung", "de");
            }
        }

        private void CreateProfilePage()
        {
            Guid parentPageID = new Guid(SampleConstants.InternalResourcesGroupPageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ProfilePageId), "Profile", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ProfilePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Your Profile</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ProfilePageId), pageTitle, "content", "Content block", "en");

                UserProfileView profile = new UserProfileView();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ProfilePageId), profile, "content", "Profile", "en");
            }

            hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ProfilePageId), "Profil", parentPageID, false, false, "de");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ProfilePageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Ihr Profil</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ProfilePageId), pageTitle, "content", "Content block", "de");

                UserProfileView profile = new UserProfileView();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ProfilePageId), profile, "content", "Profil", "de");
            }
        }

        private void CreateLoginPage()
        {
            Guid parentPageID = new Guid(SampleConstants.InternalResourcesGroupPageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.LoginPageId), "Login", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.LoginPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                // Add layout control
                var mainLayoutControl = new LayoutControl();
                var mainLayoutColumns = new List<ColumnDetails>();

                var mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 15, 0, 0),
                    PlaceholderId = "Left",
                    ColumnWidthPercentage = 50
                };
                mainLayoutColumns.Add(mainLayoutColumn1);

                var mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 15),
                    PlaceholderId = "Right",
                    ColumnWidthPercentage = 50
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), mainLayoutControl, "content", "50% + 50% (custom)", "en");

                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Login</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), pageTitle, "Main_Left", "Content block", "en");

                LoginWidget login = new LoginWidget();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), login, "Main_Left", "Login", "en");

                string registerPageUrl = string.Empty;
                var count = 0;
                App.WorkWith().Pages().Where(pN => pN.Id == new Guid(SampleConstants.RegistrationPageId)).Count(out count);
                if (count != 0)
                {
                    PageNode node = App.WorkWith().Page(new Guid(SampleConstants.RegistrationPageId)).Get();
                    registerPageUrl = node.UrlName;
                }

                ContentBlock registerInfo = new ContentBlock();
                registerInfo.Html =
                    String.Format(@"<div>
                    <div>
                    <p>Please click on ""Register"" and complete the form on the next screen to register for our service. You will receive a confirmation email during the next few minutes. <br />
                    <br />
                    Once registered, you are ready to use our service in full mode.&nbsp;</p>
                    <p><strong>You need to be logged in to:</strong></p>
                    <ul>
                        <li>post in forums</li>
                        <li>manage your account</li>
                        <li>download documents</li>
                    </ul>
                    <a class=""important"" href=""{0}"">Register</a></div>
                    </div>", registerPageUrl);

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), registerInfo, "Main_Right", "Login", "en");
            }

            hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.LoginPageId), "Login", parentPageID, false, false, "de");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.LoginPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                // Add layout control
                var mainLayoutControl = new LayoutControl();
                var mainLayoutColumns = new List<ColumnDetails>();

                var mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 15, 0, 0),
                    PlaceholderId = "Left",
                    ColumnWidthPercentage = 50
                };
                mainLayoutColumns.Add(mainLayoutColumn1);

                var mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 15),
                    PlaceholderId = "Right",
                    ColumnWidthPercentage = 50
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), mainLayoutControl, "content", "50% + 50% (custom)", "de");

                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Login</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), pageTitle, "Main_Left", "Content block", "de");

                LoginWidget login = new LoginWidget();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), login, "Main_Left", "Login", "de");

                string registerPageUrl = string.Empty;
                var count = 0;
                App.WorkWith().Pages().Where(pN => pN.Id == new Guid(SampleConstants.RegistrationPageId)).Count(out count);
                if (count != 0)
                {
                    PageNode node = App.WorkWith().Page(new Guid(SampleConstants.RegistrationPageId)).Get();
                    registerPageUrl = node.UrlName;
                }

                ContentBlock registerInfo = new ContentBlock();
                registerInfo.Html =
                    String.Format(@"<div>
                    <p>Bitte klicken Sie auf ""Registrieren"" und f&uuml;llen Sie das Formular auf der n&auml;chsten Seite, um f&uuml;r unseren Service registrieren. Sie erhalten eine Best&auml;tigung per E-Mail in den n&auml;chsten Minuten erhalten.<br />
                    <br />
                    Einmal registriert, k&ouml;nnen Sie unseren Service in Voll-Modus verwenden.&nbsp;</p>
                    <p><strong>Sie m&uuml;ssen registriert sein, um Folgendes machen zu k&ouml;nnen:</strong></p>
                    <ul>
                        <li>In den Foren schreiben</li>
                        <li>Ihr Account managen</li>
                        <li>Dokumente herunterladen</li>
                    </ul>
                    <p><a href=""{0}"">Registrierung</a></p>
                    </div>", registerPageUrl);

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.LoginPageId), registerInfo, "Main_Right", "Login", "de");
            }
        }

        #endregion

        private void AddSearchControlToDefaultEducationTemplate()
        {
            Guid templateID = new Guid(SampleConstants.EducationTemplateId);

            PageManager pageManager = PageManager.GetManager();
            var template = pageManager.GetTemplates().Where(t => t.Id == templateID).FirstOrDefault();
            var templateControls = template.Controls.Where(t => t.ObjectType == typeof(SearchBox).FullName);
            bool containsSearchBox = false;

            foreach (var control in templateControls)
            {
                var searchBox = pageManager.LoadControl(control) as SearchBox;
                if (searchBox.ResultsPageId == SampleConstants.SearchResultsPageId)
                {
                    containsSearchBox = true;
                }
            }

            if (!containsSearchBox)
            {
                // Search box
                SearchBox searchBox = new SearchBox();
                searchBox.ResultsPageId = SampleConstants.SearchResultsPageId;
                searchBox.WordsMode = WordsMode.AllWords;
                searchBox.SearchIndexPipeId = this.GetDefaultSearchIndexPipeID();

                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), searchBox, "LanguageSearch_Left", "Search box");
            }
        }

        #region Publishing point

        private Guid GetDefaultSearchIndexPipeID()
        {
            List<PublishingPipeViewModel> pipes = this.GetOutgoingPublishingPipes();
            Guid result = (from pipe in pipes
                           where pipe.Title.Equals(SampleConstants.SearchIndexName)
                           select pipe.ID).FirstOrDefault();
            return result;
        }

        private List<PublishingPipeViewModel> GetOutgoingPublishingPipes()
        {
            List<PublishingPipeViewModel> pipesList = new List<PublishingPipeViewModel>();
            var manager = PublishingManager.GetManager(PublishingConfig.SearchProviderName);
            var query = manager.GetPipeSettings().Where(p => p.PipeName == SearchIndexOutboundPipe.PipeName);
            query = query.Where(p => p.IsActive == true && p.IsInbound == false);

            foreach (var item in query)
            {
                PublishingPipeViewModel viewmodel = new PublishingPipeViewModel();
                viewmodel.ID = item.Id;
                if (string.IsNullOrEmpty(item.Title))
                    viewmodel.Title = item.PublishingPoint.Name;
                else
                    viewmodel.Title = item.Title;

                pipesList.Add(viewmodel);
            }

            List<PublishingPipeViewModel> pipes = new List<PublishingPipeViewModel>(pipesList);
            return pipes;
        }

        private void CreateSearchPublishingPoint()
        {
            PublishingPointDetailViewModel pointDetailViewModel = this.CreateSearchPointInfo();
            PublishingAdminService service = new PublishingAdminService();
            Guid itemId = pointDetailViewModel.Id;

            if (!this.PublishingPointExists(pointDetailViewModel, service, itemId))
            {
                PublishingPoint savePoint = this.PublishingMan.CreatePublishingPoint();
                PublishingPointFactory.CreatePublishingPointDataItem(pointDetailViewModel.PublishingPointDefinition, savePoint);

                savePoint.Name = pointDetailViewModel.Title;
                savePoint.Description = pointDetailViewModel.Description;
                savePoint.IsActive = pointDetailViewModel.IsActive;
                savePoint.PublishingPointBusinessObjectName = pointDetailViewModel.PublishingPointBusinessObjectName;
                var fullSet = new List<WcfPipeSettings>();
                fullSet.AddRange(pointDetailViewModel.InboundSettings);
                fullSet.AddRange(pointDetailViewModel.OutboundSettings);
                this.ClearDeletedSettings(savePoint, fullSet);

                this.FillPipeSettings(savePoint, pointDetailViewModel.InboundSettings);
                this.FillPipeSettings(savePoint, pointDetailViewModel.OutboundSettings);

                var pipeSettings = this.PublishingMan.GetPipeSettings<RssPipeSettings>();

                foreach (var currentPipeSettings in savePoint.PipeSettings.Where(ps => ps.GetType().FullName == typeof(RssPipeSettings).FullName))
                {
                    var currentId = currentPipeSettings.Id;
                    var pipeSettingsUrl = ((RssPipeSettings)currentPipeSettings).UrlName;
                    if (pipeSettings.Where(ps => ps.IsInbound == false && ps.UrlName == pipeSettingsUrl && ps.Id != currentId).Any())
                    {
                        this.ThrowDuplicateUrlException(((RssPipeSettings)currentPipeSettings).UrlName);
                    }
                }

                this.PublishingMan.SaveChanges();
                pointDetailViewModel.Id = savePoint.Id;
                MetadataManager.GetManager().SaveChanges(true);
                service.ReschedulePublishingPointPipes(savePoint, string.Empty);

                var pipeSettingsReset = this.PublishingMan.GetPipeSettings<SearchIndexPipeSettings>().Where(ps => ps.PublishingPoint.Id == pointDetailViewModel.Id).FirstOrDefault();
                pipeSettingsReset.CatalogName = System.Text.RegularExpressions.Regex.Replace(SampleConstants.SearchIndexName.ToLowerInvariant(),
                            SampleConstants.SearchIndexFilterExpression, SampleConstants.SearchIndexReplacementString);
                this.PublishingMan.SaveChanges();

                service.ReindexSearchContent(PublishingConfig.SearchProviderName, savePoint.Id.ToString());
            }
        }

        private PublishingPointDetailViewModel CreateSearchPointInfo()
        {
            var model = new PublishingPointDetailViewModel();
            model.Id = Guid.Empty;
            model.Title = SampleConstants.SearchIndexName;
            model.PublishingPointDefinition = this.CreatePublishingPointDefinition();
            model.Description = null;
            model.IsActive = true;
            model.PublishingPointBusinessObjectName = null;
            model.InboundSettings = this.CreatePublishingPointInboundSettings();
            model.OutboundSettings = this.CreatePublishingPointOutboundSettings();
            return model;
        }

        private bool PublishingPointExists(PublishingPointDetailViewModel pointDetailViewModel, PublishingAdminService service, Guid itemId)
        {
            return service.GetPublishingPoints(PublishingConfig.SearchProviderName, string.Empty, -1, -1, string.Empty).Items.Where(ppoint => ppoint.Title.ToLower()
                            == pointDetailViewModel.Title.ToLower() && ppoint.Id != itemId).FirstOrDefault() != null;
        }

        private List<WcfPipeSettings> CreatePublishingPointOutboundSettings()
        {
            List<WcfPipeSettings> outboundSettings = new List<WcfPipeSettings>();

            WcfPipeSettings setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.UIName = "SearchIndexPipe";
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":false,\"MaxItems\":0,\"PipeName\":\"SearchIndex\",\"ResourceClassId\":\"\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"SearchIndexPipe\",\"CatalogName\":null,\"SearchProviderName\":null}";
            setting.PipeName = "SearchIndex";
            setting.IsActive = true;
            setting.IsInbound = false;
            setting.MappingSettings = this.CreateDefaultOutboundMappingSettings();

            outboundSettings.Add(setting);

            return outboundSettings;
        }

        private MappingSettingsViewModel CreateDefaultOutboundMappingSettings()
        {
            var defaultOutboundMappings = SearchIndexOutboundPipe.GetDefaultMappings();
            return this.GetDefaultMappingSettings(defaultOutboundMappings);
        }

        private List<WcfPipeSettings> CreatePublishingPointInboundSettings()
        {
            List<WcfPipeSettings> inboundSettings = new List<WcfPipeSettings>();
            WcfPipeSettings setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = PageInboundPipe.PipeName;
            setting.UIName = "Static HTML in pages";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":\"Visible = true && Status = Live && PublicationDate<= DateTime.UtcNow\",\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"PagePipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"PagePipe\"}";
            inboundSettings.Add(setting);

            setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = ContentInboundPipe.PipeName;
            setting.UIName = "Sitefinity content";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"ContentInboundPipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"ContentPipeName\",\"BackLinksPageId\":null,\"ContentLinks\":[],\"ContentTypeName\":\"Telerik.Sitefinity.News.Model.NewsItem\",\"ImportItemAsPublished\":false,\"ImportedItemParentId\":\"00000000-0000-0000-0000-000000000000\",\"ProviderName\":null}";
            setting.ContentLocationPageID = new Guid(SampleConstants.NewsPageId);
            setting.ContentName = "News";
            inboundSettings.Add(setting);

            setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = ContentInboundPipe.PipeName;
            setting.UIName = "Sitefinity content";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"ContentInboundPipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"ContentPipeName\",\"BackLinksPageId\":null,\"ContentLinks\":[],\"ContentTypeName\":\"Telerik.Sitefinity.Events.Model.Event\",\"ImportItemAsPublished\":false,\"ImportedItemParentId\":\"00000000-0000-0000-0000-000000000000\",\"ProviderName\":null}";
            setting.ContentLocationPageID = new Guid(SampleConstants.EventsPageId);
            setting.ContentName = "Events";
            inboundSettings.Add(setting);

            setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = ContentInboundPipe.PipeName;
            setting.UIName = "Sitefinity content";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"ContentInboundPipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"ContentPipeName\",\"BackLinksPageId\":null,\"ContentLinks\":[],\"ContentTypeName\":\"Telerik.Sitefinity.Blogs.Model.BlogPost\",\"ImportItemAsPublished\":false,\"ImportedItemParentId\":\"00000000-0000-0000-0000-000000000000\",\"ProviderName\":null}";
            setting.ContentLocationPageID = new Guid(SampleConstants.TIUBlogPageId);
            setting.ContentName = "Blog";
            inboundSettings.Add(setting);

            setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = ContentInboundPipe.PipeName;
            setting.UIName = "Sitefinity content";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"ContentInboundPipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"ContentPipeName\",\"BackLinksPageId\":null,\"ContentLinks\":[],\"ContentTypeName\":\"Telerik.Sitefinity.GenericContent.Model.ContentItem\",\"ImportItemAsPublished\":false,\"ImportedItemParentId\":\"00000000-0000-0000-0000-000000000000\",\"ProviderName\":null}";
            setting.ContentLocationPageID = new Guid(SampleConstants.HomePageId);
            setting.ContentName = "Home";
            inboundSettings.Add(setting);

            setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = ContentInboundPipe.PipeName;
            setting.UIName = "Sitefinity content";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"ContentInboundPipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"ContentPipeName\",\"BackLinksPageId\":null,\"ContentLinks\":[],\"ContentTypeName\":\"Telerik.Sitefinity.Lists.Model.ListItem\",\"ImportItemAsPublished\":false,\"ImportedItemParentId\":\"00000000-0000-0000-0000-000000000000\",\"ProviderName\":null}";
            setting.ContentLocationPageID = new Guid(SampleConstants.HomePageId);
            setting.ContentName = "Home";
            inboundSettings.Add(setting);

            setting = new WcfPipeSettings();
            setting.AdditionalSettings = "{}";
            setting.IsActive = true;
            setting.IsInbound = true;
            setting.PipeName = ProductInboundPipe.PipeName;
            setting.UIName = "Sitefinity content";
            setting.MappingSettings = this.CreateDefaultInboundMappingSettings(setting);
            setting.Settings = "{\"ApplicationName\":null,\"Description\":null,\"FilterExpression\":null,\"InvocationMode\":1,\"IsActive\":true,\"IsInbound\":true,\"MaxItems\":0,\"PipeName\":\"ProductInboundPipe\",\"ResourceClassId\":\"PublishingMessages\",\"ScheduleDay\":0,\"ScheduleType\":0,\"Title\":null,\"UIName\":\"ContentPipeName\",\"BackLinksPageId\":null,\"ContentLinks\":[],\"ContentTypeName\":\"Telerik.Sitefinity.Ecommerce.Catalog.Model.Product\",\"ImportItemAsPublished\":false,\"ImportedItemParentId\":\"00000000-0000-0000-0000-000000000000\",\"ProviderName\":null}";
            setting.ContentLocationPageID = new Guid(SampleConstants.ShopBasePageId);
            setting.ContentName = "Products";
            inboundSettings.Add(setting);

            return inboundSettings;
        }

        private MappingSettingsViewModel CreateDefaultInboundMappingSettings(WcfPipeSettings settings)
        {
            var defaultInboundMappings = PublishingSystemFactory.GetPipeMappings(settings.PipeName, settings.IsInbound);
            return this.GetDefaultMappingSettings(defaultInboundMappings);
        }

        private MappingSettingsViewModel GetDefaultMappingSettings(IList<Mapping> defaultMapping)
        {
            MappingSettingsViewModel model = new MappingSettingsViewModel();
            model.Mappings = new List<MappingViewModel>();
            foreach (var item in defaultMapping)
            {
                var mappingTraslations = new List<PipeMappingTranslationViewModel>();
                foreach (var translation in item.Translations)
                {
                    mappingTraslations.Add(new PipeMappingTranslationViewModel()
                    {
                        Id = translation.Id,
                        TranslatorName = translation.TranslatorName,
                        TranslatorSettings = translation.TranslatorSettings
                    });
                }

                model.Mappings.Add(new MappingViewModel()
                {
                    ApplicationName = item.ApplicationName,
                    DefaultValue = item.DefaultValue,
                    DestinationPropertyName = item.DestinationPropertyName,
                    Id = item.Id,
                    IsRequired = item.IsRequired,
                    SourcePropertyNames = item.SourcePropertyNames,
                    Translations = mappingTraslations
                });
            }

            return model;
        }

        private List<SimpleDefinitionField> CreatePublishingPointDefinition()
        {
            List<SimpleDefinitionField> definition = new List<SimpleDefinitionField>();

            var field = new SimpleDefinitionField("ApplicationName", "ApplicationName", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.HideInUI = true;
            field.IsMetaField = false;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Categories", "Categories", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.DBType = "LONGVARCHAR";
            field.SQLDBType = "NVARCHAR(MAX)";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Content", "Content", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.DBType = "LONGVARCHAR";
            field.SQLDBType = "NVARCHAR(MAX)";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("ContentType", "ContentType", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("ExpirationDate", "ExpirationDate", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.DateTime";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Id", "Id", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.Guid";
            field.HideInUI = true;
            field.IsMetaField = false;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("ItemHash", "ItemHash", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("LangId", "LangId", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("LastModified", "LastModified", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.DateTime";
            field.HideInUI = true;
            field.IsMetaField = false;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Link", "Link", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.SQLDBType = "NVARCHAR";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("OriginalItemId", "OriginalItemId", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.Guid";
            field.HideInUI = false;
            field.IsMetaField = false;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("OriginalParentId", "OriginalParentId", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.Guid";
            field.HideInUI = false;
            field.IsMetaField = false;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("OwnerEmail", "OwnerEmail", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.SQLDBType = "NVARCHAR";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("OwnerFirstName", "OwnerFirstName", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.SQLDBType = "NVARCHAR";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("OwnerLastName", "OwnerLastName", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.SQLDBType = "NVARCHAR";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("PipeId", "PipeId", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("PublicationDate", "PublicationDate", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.DateTime";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Summary", "Summary", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.DBType = "LONGVARCHAR";
            field.SQLDBType = "NVARCHAR(MAX)";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Title", "Title", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.SQLDBType = "NVARCHAR";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            field = new SimpleDefinitionField("Username", "Username", true);
            field.AllowMultipleTaxons = false;
            field.ClrType = "System.String";
            field.SQLDBType = "NVARCHAR";
            field.HideInUI = false;
            field.IsMetaField = true;
            field.IsModified = false;
            definition.Add(field);

            return definition;
        }

        private void FillPipeSettings(PublishingPoint modelPublishingPoint, List<WcfPipeSettings> allViewModelSettings)
        {
            var providerName = this.PublishingMan.Provider.Name;
            foreach (var viewModelSetting in allViewModelSettings)
            {
                var vmID = viewModelSetting.Id;
                var tempSetting = viewModelSetting.ConvertToModel(providerName);
                tempSetting.ApplicationName = this.PublishingMan.Provider.ApplicationName;
                if (string.IsNullOrEmpty(tempSetting.Description))
                {
                    tempSetting.Description = modelPublishingPoint.Description;
                }

                tempSetting.FilterExpression = viewModelSetting.AdditionalFilter;
                tempSetting.LanguageIds.Clear();
                if (viewModelSetting.LanguageIds.Count > 0)
                {
                    viewModelSetting.LanguageIds.ForEach(id => tempSetting.LanguageIds.Add(id));
                }

                if (tempSetting is RssPipeSettings)
                {
                    var rssSettings = tempSetting as RssPipeSettings;
                    if (String.IsNullOrEmpty(rssSettings.UrlName))
                    {
                        rssSettings.UrlName = System.Text.RegularExpressions.Regex.Replace(modelPublishingPoint.Name.ToLowerInvariant(),
                            SampleConstants.SearchIndexFilterExpression, SampleConstants.SearchIndexReplacementString);
                    }
                }

                if (tempSetting is SitefinityContentPipeSettings)
                {
                    var contentSettings = tempSetting as SitefinityContentPipeSettings;
                    if (viewModelSetting.ContentLocationPageID.HasValue)
                    {
                        contentSettings.BackLinksPageId = viewModelSetting.ContentLocationPageID;
                    }
                }

                if (tempSetting is SearchIndexPipeSettings)
                {
                    var searchIndexSettings = tempSetting as SearchIndexPipeSettings;
                    searchIndexSettings.Title = modelPublishingPoint.Name;
                    if (string.IsNullOrEmpty(searchIndexSettings.CatalogName))
                    {
                        searchIndexSettings.CatalogName = System.Text.RegularExpressions.Regex.Replace(modelPublishingPoint.Name.ToLowerInvariant(),
                            SampleConstants.SearchIndexFilterExpression, SampleConstants.SearchIndexReplacementString);
                    }
                }

                PipeSettings modelPipeSettings = this.PublishingMan.GetPipeSettings().Where(s => s.Id == vmID).SingleOrDefault();
                if (modelPipeSettings == null)
                {
                    IPipe pipe = null;
                    pipe = PublishingSystemFactory.GetPipe(viewModelSetting.PipeName);
                    modelPipeSettings = this.PublishingMan.CreatePipeSettings(pipe.PipeSettingsType);
                    modelPublishingPoint.PipeSettings.Add(modelPipeSettings);
                }

                tempSetting.CopySettings(modelPipeSettings);
                viewModelSetting.MappingSettings.CopyToModel(this.PublishingMan, modelPipeSettings.Mappings);
            }
        }

        private void ThrowDuplicateUrlException(string url)
        {
            throw new WebProtocolException(System.Net.HttpStatusCode.InternalServerError, String.Format(Res.Get<PublishingMessages>().DuplicateUrlException, url), null);
        }

        private void ClearDeletedSettings(PublishingPoint point, List<WcfPipeSettings> settings)
        {
            List<PipeSettings> toRemove = new List<PipeSettings>();
            foreach (var setting in point.PipeSettings)
            {
                var wcfSetting = settings.SingleOrDefault(p => p.Id == setting.Id);
                if (wcfSetting == null)
                {
                    toRemove.Add(setting);
                }
            }

            foreach (var item in toRemove)
            {
                point.PipeSettings.Remove(item);
                this.PublishingMan.DeletePipeSettings(item);
            }
        }

        #endregion

        private void CreateSearchResultPage()
        {
            Guid parentPageID = new Guid(SampleConstants.InternalResourcesGroupPageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), "Search Results", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                // Add page title
                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Search results</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), pageTitle, "content", "Content block", "en");

                // Add search results
                SearchResults searchResults = new SearchResults();

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), searchResults, "content", "Search results", "en");
            }

            hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), "Suchergebnisse", parentPageID, false, false, "de");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                // Add page title
                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>Suchergebnisse</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), pageTitle, "content", "Content block", "de");

                // Add search results
                SearchResults searchResults = new SearchResults();

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.SearchResultsPageId), searchResults, "content", "Such-Ergebnisse", "de");
            }
        }

        private void AddContactUsPhonesToBackend()
        {
            Guid templateID = SiteInitializer.DefaultBackendTemplateId;

            PageManager pageManager = PageManager.GetManager();
            var template = pageManager.GetTemplates().Where(t => t.Id == templateID).FirstOrDefault();
            var templateControls = template.Controls.Where(t => t.ObjectType == typeof(ContentBlockBase).FullName);
            bool containsPhones = false;

            foreach (var control in templateControls)
            {
                var block = pageManager.LoadControl(control) as ContentBlockBase;
                if (block.Html.Contains("Contact us"))
                {
                    containsPhones = true;
                }
            }

            if (!containsPhones)
            {
                var phonesContentBlock = new ContentBlockBase();
                phonesContentBlock.Html = @"<span class=""phone""><strong>Contact us:</strong> <ul> <li class=""phone-list""><strong>US</strong> +1-888-365-2779</li> <li class=""phone-list""><strong>UK</strong> +44-20-7291-0580</li> <li class=""phone-list""><strong>AU</strong> +61-2-8090-1465</li> <li class=""phone-list""><strong>DE</strong> +49-89-2441642-70</li> </ul> </span>";
                SampleUtilities.AddControlToTemplate(templateID, phonesContentBlock, "Header", "Contact Us");

                var cssControl = new CssEmbedControl();
                cssControl.CustomCssCode = @".phone { font-size: 11px; position: absolute; right: 20px; float: right; width: 200px; padding-top: 5px; color: white; margin-right: 220px; } li.phone-list { line-height: 14px; font-size: 11px; } a.sfMoreDetails, a.sfMoreDetails:link, a.sfMoreDetails:visited, a.sfMoreDetails:hover, a.sfMoreDetails:active { height: 15px; padding: 3px; background-color: #0889DD; color: #ffffff; border-bottom-left-radius: 5px; border-bottom-right-radius: 5px; border-top-left-radius: 5px; border-top-right-radius: 5px; }";

                SampleUtilities.AddControlToTemplate(templateID, cssControl, "Header", "CSS");
            }
        }

        private void CreateUsers()
        {
            //Guid userId = SampleUtilities.CreateUser("admin2", "password", "noreply@telerik.com", "Telerik", "Admin", "default", "default", true);
            //SampleUtilities.SetUserAvatar(userId, "avatarName");
        }

        private void AddUsersToRoles()
        {
            //SampleUtilities.AddUserToApplicationRoles("admin2", new List<string>() { SampleConstants.BackendUsersRoleName, SampleConstants.AuthorsRoleName });
        }

        private void CreateFeeds()
        {
            SampleUtilities.CreateContentFeed("TIU Blog", new Guid(SampleConstants.TIUBlogPageId), typeof(BlogPost), 25, RssContentOutputSetting.TitleAndContent, RssFormatOutputSettings.RssOnly);
        }

        private void CreateNewsItemCommentItems()
        {
            var newsCreated = App.WorkWith().NewsItems().Get().Count() > 0;

            if (!newsCreated)
            {
                //News 1
                var title = "Annual Student Leadership Awards";
                var content = @"<p>The Annual Student Leadership Awards to honor student organizations and leaders on their accomplishments during 2009-10 academic years were held on December 20. This year's Student Leadership honorees included: </p>
<ul>
    <li>Emerging Student Leader: Angel Hawkins </li>
    <li>Persevering Leader: Nathaniel Walsh </li>
    <li>Collaborative Organization Award: Bryant Freeman</li>
    <li>Outstanding Cultural Program: Rafael Weber</li>
    <li>Campus Community Development Program: Josefina Mccoy </li>
    <li>Resident Advisor of the Year: Amelia Hall</li>
    <li>Resident Advisor of the Year: Bob Wilson </li>
    <li>Advisor of the Year: Ira Jacobs </li>
    <li>Senior Athlete of the Year: Glenda Gomez<strong></strong></li>
</ul>";
                var summary = "The Annual Student Leadership Awards to honor student organizations and leaders on their accomplishments during 2009-10 academic years were held on December 20. ";
                var author = "Jordan Angelov";
                var sourceName = "Telerik international university";
                var sourceURL = "http://www.telerik.com";
                var id1 = Guid.NewGuid();

                SampleUtilities.CreateLocalizedNewsItem(id1, title, content, summary, author, sourceName, sourceURL, new List<string>() { "announcement", "awards" }, null, "en");

                title = "Jährliche Student Leadership Awards";
                content = @"<p>Die j&auml;hrlichen Student Leadership Awards wurden an studentischen Organisationen und F&uuml;hrungskr&auml;fte f&uuml;r ihre Leistungen w&auml;hrend 2009-10 Studienjahre vergeben. Die Veranstaltung fand am 20. Dezember statt. Die Preistr&auml;ger sind:</p> <ul style=""list-style-type: disc;""> <li>Neuer Studentenf&uuml;hrer: Angel Hawkins</li> <li>Anerkannter Leader: Nathaniel Walsh</li> <li>Organisation Sonderforschungsbereich Award: Bryant Freeman</li> <li>herausragend kulturelles Programm: Rafael Weber</li> <li>Campus Community Development Program: Josefina Mccoy</li> <li>Berater des Jahres: Amelia Hall</li> <li>Berater des Jahres: Bob Wilson</li> <li>Advisor of the Year: Ira Jacobs</li> </ul>";
                summary = "Die j&auml;hrlichen Student Leadership Awards wurden an studentischen Organisationen und F&uuml;hrungskr&auml;fte f&uuml;r ihre Leistungen w&auml;hrend 2009-10 Studienjahre vergeben. Die Veranstaltung fand am 20";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id1, title, content, summary, author, sourceName, sourceURL, "de");

                //News 2
                var id2 = Guid.NewGuid();
                title = "Economic Historian Jordan Turner Honored for Contributions";
                content = "<p>The research of Turner will be celebrated this weekend with a conference framed by his groundbreaking work studying economic growth in the Americas. </p> <p>Turner's research is acknowledged internationally for its impact on economics and history, especially the history of slavery. Of the 30 books and more than 200 articles he has co-authored or co-edited, his analysis on the economic underpinnings of slavery.</p> <p>&nbsp;</p>";
                summary = "The research of Turner will be celebrated this weekend with a conference framed by his groundbreaking work studying economic growth in the Americas.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id2, title, content, summary, author, sourceName, sourceURL, new List<string>() { "economics", "awards" }, null, "en");

                title = "Wirtschafts-Historiker Jordan Turner erhält Anerkennung für seinen Beitrag";
                content = "<p>Die Forschung der Turner wird dieses Wochenende gefeiert mit einer Konferenz &uuml;ber seine bahnbrechenden Arbeiten in denen er das Wirtschaftswachstum in Amerika studiert hat.</p> <p>Turner-Forschung ist international bekannt und f&uuml;r ihre Auswirkungen auf die Volkswirtschaft und Geschichte, besonders die Geschichte der Sklaverei anerkannt. Als Mitautor von den 30 B&uuml;chern und mehr als 200 Artikeln stammt seine Analyse &uuml;ber die &ouml;konomischen Grundlagen der Sklaverei.</p>";
                summary = "Die Forschung der Turner wird dieses Wochenende gefeiert mit einer Konferenz über seine bahnbrechenden Arbeiten in denen er das Wirtschaftswachstum in Amerika studiert hat.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id2, title, content, summary, author, sourceName, sourceURL, "de");

                var commentContent = "Great, thanks!";
                var commentAuthor = "Stan";
                var commentEmail = "stan@telerik.com";
                var commentIp = "::1";
                var authorProxy1 = new AuthorProxy(commentAuthor, commentEmail);

                var liveId2 = GetLiveVersionIdByMasterId(id2);

                SampleUtilities.CreateNewsItemComment(liveId2, commentContent, authorProxy1, commentIp);

                //News 3
                var id3 = Guid.NewGuid();
                title = "Percent Tuition Decrease for 2010-2011";
                content = @"<p>TIU tuition will go down by 3.1 percent to $13,215 for the academic year 2010-11. The total package (tuition, room and board, and student services fee) will be $31,215, a 3.1 percent decrease over last year.</p>";
                summary = "TIU tuition will go down by 3.1 percent to $13,215 for the academic year 2010-11. The total package (tuition, room and board, and student services fee) will be $31,215, a 3.1 percent decrease over last year.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id3, title, content, summary, author, sourceName, sourceURL, new List<string>() { "announcement" }, null, "en");

                title = "Studiengebührenprozentabnahme für den Zeitraum 2010-2011";
                content = @"<p>TIU Unterricht wird um 3,1 Prozent auf 13.215 $ gehen f&uuml;r das akademische Jahr 2010-11. Das gesamte Paket (Studiengeb&uuml;hren, Unterkunft und Verpflegung und Betreuung der Studierenden gegen Geb&uuml;hr) wird $ 31.215 sein, eine 3,1 Prozent gegen&uuml;ber dem Vorjahr verringern.</p>";
                summary = "TIU Unterricht wird um 3,1 Prozent auf 13.215 $ gehen für das akademische Jahr 2010-11. Das gesamte Paket (Studiengebühren, Unterkunft und Verpflegung und Betreuung der Studierenden gegen Gebühr) wird $ 31.215 sein, eine 3,1 Prozent gegenüber dem Vorjahr verringern.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id3, title, content, summary, author, sourceName, sourceURL, "de");

                //News 4
                var id4 = Guid.NewGuid();
                title = "Staff Recognition Awards shine spotlight on outstanding service";
                content = @"<p>Students from Yamanashi University, Japan, visited TIU last week to gain a valuable insight into a US university and to learn from different practices and technology.</p> <p>Five members of the party were from Japan, one from Shanghai and another from South Korea and they were accompanied by a Yamanashi lecturer.</p> <p>Yamanashi student, Yoko Shimada said: &ldquo;The visit has been extraordinary. Quite life changing and quite different from what we had been expecting. The lectures are amazing. Going into a Media class and a Tourism class is not something we do at Yamanashi.&rdquo;</p> <p>Professor Hristo Borisoff from TIU organized the visit. He said: &ldquo;The 7 students attended lectures and seminars, made presentations and were interviewed for the TIU television. They also met the Vice Chancellor and the Dean of the Medicine School. They were all personable, great fun to be with and have an amazing presentation style.&rdquo;</p>";
                summary = "TIU honored employees for outstanding service and dedication to the university during its annual Staff Recognition Awards ceremony today in Sofia Auditorium.";
                author = "Stanislav Padarev";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id4, title, content, summary, author, sourceName, sourceURL, "en");

                title = "Staff Recognition Awards glänzen Schlaglicht auf hervorragenden Service";
                content = @"<p>TIU geehrt Mitarbeiter f&uuml;r herausragende Leistungen und Engagement f&uuml;r die Universit&auml;t w&auml;hrend der j&auml;hrlichen Mitarbeiter Recognition Awards heute in Sofia Auditorium.<p /> <p>Berufliche Entwicklung und Lernen; Integrit&auml;t und Ethik, Respekt, Vielfalt und Pluralismus, Innovation und Flexibilit&auml;t, und Teamwork und Zusammenarbeit Sch&uuml;ler-Zentriertheit: Auszeichnungen wurden basierend auf TIU Kernwerte vorgestellt.</p>";
                summary = "TIU geehrt Mitarbeiter für herausragende Leistungen und Engagement für die Universität während der jährlichen Mitarbeiter Recognition Awards heute in Sofia Auditorium.";
                author = "Stanislav Padarev";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id4, title, content, summary, author, sourceName, sourceURL, "de");

                //News 5
                var id5 = Guid.NewGuid();
                title = "Term Dates for 2010-2011 and 2011-2012";
                content = @"<p>A full-time project administrator is needed to work in the Sports Science Department on a research project funded by FIFA. The administrator will be responsible for coordinating meetings and communications.</p>";
                summary = "A full-time project administrator is needed to work in the Sports Science Department on a research project funded by FIFA. The administrator will be responsible for coordinating meetings and communications.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id5, title, content, summary, author, sourceName, sourceURL, new List<string>() { "announcement", "FIFA", "sports" }, null, "en");

                title = "Laufzeit Termine für den Zeitraum 2010-2011 und 2011-2012";
                content = @"<p>Ein Vollzeit-Projekt-Administrator ist erforderlich, um in der Sportwissenschaft Abteilung an einem Forschungsprojekt von der FIFA finanziert arbeiten. Der Administrator ist verantwortlich f&uuml;r die Koordinierung der Meetings und Kommunikation.</p>";
                summary = "TIU geehrt Mitarbeiter für herausragende Leistungen und Engagement für die Universität während der jährlichen Mitarbeiter Recognition Awards heute in Sofia Auditorium.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id5, title, content, summary, author, sourceName, sourceURL, "de");

                commentContent = "Thanks for the announcement!";
                commentAuthor = "Anton";
                commentEmail = "anton@telerik.com";

                var authorProxy2 = new AuthorProxy(commentAuthor, commentEmail);

                var liveId5 = GetLiveVersionIdByMasterId(id5);

                SampleUtilities.CreateNewsItemComment(liveId5, commentContent, authorProxy2, commentIp);

                commentContent = "Thank you very much";
                commentAuthor = "Alex";
                commentEmail = "alex@telerik.com";

                var authorProxy3 = new AuthorProxy(commentAuthor, commentEmail);
                SampleUtilities.CreateNewsItemComment(liveId5, commentContent, authorProxy3, commentIp);

                commentContent = "Great! Thanks a lot!";
                commentAuthor = "Stan";
                commentEmail = "stan@telerik.com";
                var authorProxy4 = new AuthorProxy(commentAuthor, commentEmail);
                SampleUtilities.CreateNewsItemComment(liveId5, commentContent, authorProxy4, commentIp);

                //News 6
                var id6 = Guid.NewGuid();
                title = "Visit from Japan";
                content = @"<p>Five members of the party were from Japan, one from Shanghai and another from South Korea and they were accompanied by a Yamanashi lecturer.</p> <p>Yamanashi student, Yoko Shimada said: &ldquo;The visit has been extraordinary. Quite life changing and quite different from what we had been expecting. The lectures are amazing. Going into a Media class and a Tourism class is not something we do at Yamanashi.&rdquo;</p> <p>Professor Hristo Borisoff from TIU organized the visit. He said: &ldquo;The 7 students attended lectures and seminars, made presentations and were interviewed for the TIU television. They also met the Vice Chancellor and the Dean of the Medicine School. They were all personable, great fun to be with and have an amazing presentation style.&rdquo;</p> <p>Students from Yamanashi University, Japan, visited TIU last week to gain a valuable insight into a US university and to learn from different practices and technology.</p>";
                summary = "Students from Yamanashi University, Japan, visited TIU last week to gain a valuable insight into a US university and to learn from different practices and technology.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id6, title, content, summary, author, sourceName, sourceURL, new List<string>() { "announcement", "tourism", "japan" }, null, "en");

                title = "Besuch aus Japan";
                content = @"<p>Studierende der Yamanashi-Universit&auml;t, Japan, besuchten TIU vergangene Woche um einen wertvollen Einblick in einer US-Universit&auml;t zu gewinnen und mehr &uuml;ber unterschiedlichen Praktiken und Technologien zu erfahren.</p> <p>F&uuml;nf Mitglieder der Gruppe waren aus Japan, einer aus Shanghai und die anderen aus S&uuml;dkorea. Sie wurden von einem Yamanashi Dozent begleitet.</p> <p>Yamanashi-Student Yoko Shimada sagte: ""Der Besuch war au&szlig;ergew&ouml;hnlich! Lebensver&auml;ndernd und ganz anders als das, was wir erwartet hatten. Die Vortr&auml;ge sind erstaunlich! Eine Medien- oder eine Tourismus-Vorlesung ist nicht etwas, was wir in Yamanashi haben.""</p> <p>Professor Hristo Borisoff von TIU organisiert den Besuch. Er sagte: ""Die 7 Studenten besuchten Vorlesungen und Seminare, Pr&auml;sentationen und wurden f&uuml;r das TIU-Fernsehen interviewt. Sie trafen auch den Vizekanzler und den Dekan der Medizinischen Fakult&auml;t. Sie alle waren sehr sympathisch, hatten unglaublich viel Spa&szlig; und konnten bei ihren Vortr&auml;gen richtig gl&auml;nzen.""</p>";
                summary = "Studierende der Yamanashi-Universität, Japan, besuchten TIU vergangene Woche um einen wertvollen Einblick in einer US-Universität zu gewinnen und mehr über unterschiedlichen Praktiken und Technologien zu erfahren.";
                author = "Jordan Angelov";
                sourceName = "Telerik international university";
                sourceURL = "http://www.telerik.com";

                SampleUtilities.CreateLocalizedNewsItem(id6, title, content, summary, author, sourceName, sourceURL, "de");

                commentContent = "Thanks";
                commentAuthor = "Stan";
                commentEmail = "stan@telerik.com";
                var authorProxy5 = new AuthorProxy(commentAuthor, commentEmail);

                var liveId6 = GetLiveVersionIdByMasterId(id6);
                SampleUtilities.CreateNewsItemComment(liveId6, commentContent, authorProxy5, commentIp);

                commentContent = "Awesome! I'll be there!";
                commentAuthor = "Greg";
                commentEmail = "greg@telerik.com";
                var authorProxy6 = new AuthorProxy(commentAuthor, commentEmail);
                SampleUtilities.CreateNewsItemComment(liveId6, commentContent, authorProxy6, commentIp);
            }
        }

        private static Guid GetLiveVersionIdByMasterId(Guid masterId)
        {
            var newsManager = NewsManager.GetManager();
            var master = newsManager.GetNewsItem(masterId);
            var liveId = newsManager.Lifecycle.GetLive(master).Id;
            return liveId;
        }

        private void CreateEvents()
        {
            var eventsCreated = App.WorkWith().Events().Get().Count() > 0;

            if (!eventsCreated)
            {
                Guid id = Guid.NewGuid();
                string title = "Digital Signage Technology summit";
                string content = "<p>Transform how you communicate with your employees, students, visitors, customers, and more! Digital signage has become a more efficient and effective way to communicate in university campuses, corporate offices, healthcare facilities, entertainment centers, and more. Now is the time to take your digital signage to the next level. Transform your communications and complement your existing web presence, emails, newsletters, meetings, and posters with digital signage. There is nowhere else you can join real-world practitioners and experts in such an empowering and educational forum.</p>";
                string summary = "Transform how you communicate with your employees, students, visitors, customers, and more! Digital signage has become a more efficient and effective way to communicate in university campuses, corporate offices, healthcare facilities, entertainment centers, and more.";
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now.AddDays(10);
                string street = "3600 Paradise Road";
                string city = "Las Vegas";
                string country = "United States";
                string state = "Nevada";
                string email = "mirara@strategyinstitute.com";
                string website = string.Empty;
                string contactName = "Jennifer Mirara";
                string cellPhone = "1-702-893-8000";
                string phone = string.Empty;

                string titleDE = "Digital Signage-Technologie-Gipfel";
                string contentDE = @"<div id=""gt-src-tools""> <div id=""gt-src-tools-l""></div> <div id=""gt-src-tools-r""></div> </div> <div id=""gt-res-content""> <div dir=""ltr""> <div id=""tts_button""></div>
Transform, wie Sie mit Ihren Mitarbeitern, Studenten, Besucher, Kunden und mehr kommunizieren! Digital Signage ist zu einem effizienteren und effektiveren Weg, um in Universit&auml;ten, B&uuml;ros, Einrichtungen des Gesundheitswesens, Unterhaltungs-und mehr kommunizieren. Jetzt ist die Zeit, Ihre Digital Signage auf die n&auml;chste Stufe zu nehmen. Verwandeln Sie Ihre Kommunikation und erg&auml;nzen Ihre bestehende Web-Pr&auml;senz, E-Mails, Newsletter, Tagungen und Plakate mit Digital Signage. Nirgendwo sonst kann man der realen Welt Praktiker und Experten in einer solchen Erm&auml;chtigung und Bildungs-Forum beitreten.</div> </div>";
                string summaryDE = "Transform, wie Sie mit Ihren Mitarbeitern, Studenten, Besucher, Kunden und mehr kommunizieren! Digital Signage ist zu einem effizienteren und effektiveren Weg, um in Universitäten, Büros, Einrichtungen des Gesundheitswesens, Unterhaltungs-und mehr kommunizieren.";

                SampleUtilities.CreateLocalizedEvent(id, title, content, summary, startDate, endDate, street, city, country, state, email, website, contactName, cellPhone, phone, null, null, "en");
                SampleUtilities.CreateLocalizedEvent(id, titleDE, contentDE, summaryDE, startDate, endDate, string.Empty, string.Empty, string.Empty, string.Empty, email, string.Empty, string.Empty, string.Empty, string.Empty, null, null, "de");

                id = Guid.NewGuid();
                title = "TIU scientists attend world ecology summit";
                content = "<p>Hosted by Urban Ecology Montr&eacute;al, Ecocity World Summit 2011 will build on work of past Ecocity World Summits while adding new conference themes, participatory methods, and projects that will last beyond the life of the conference. Detailed conference content and design will be developed in collaboration with local and international partners, making sure that the particular urban ecological expertise of Montr&eacute;al is highlighted.</p>";
                summary = "Hosted by Urban Ecology Montréal, Ecocity World Summit 2011 will build on work of past Ecocity World Summits while adding new conference themes, participatory methods, and projects that will last beyond the life of the conference.";
                startDate = startDate.AddDays(10);
                endDate = endDate.AddDays(10);
                street = "514.395.1808";
                city = "Montreal";
                country = "Canada";
                state = "Montreal";
                email = "info@ecocity2011.com";
                website = "http://www.ecocity2011.com/";
                contactName = "Jordan Angelov";
                cellPhone = "+359 886 145 221";
                phone = "+359 2 412541";

                titleDE = "TIU Wissenschaftler nehmen weltweit Ökologie-Gipfel";
                contentDE = @"Hosted by Stadt&ouml;kologie Montr&eacute;al, wird Ecocity Weltgipfel 2011 auf der Arbeit der vergangenen Ecocity Weltgipfeln bauen, w&auml;hrend das Hinzuf&uuml;gen neuer Konferenzthemen, partizipative Methoden und Projekte, die &uuml;ber das Leben der Konferenz dauern wird. Detaillierte Konferenz Inhalt und Gestaltung wird in Zusammenarbeit mit lokalen und internationalen Partnern entwickelt werden, um sicherzustellen, dass die besonderen stadt&ouml;kologischen Expertise von Montr&eacute;al markiert ist.";
                summaryDE = "Hosted by Stadtökologie Montréal, wird Ecocity Weltgipfel 2011 auf der Arbeit der vergangenen Ecocity Weltgipfeln bauen, während das Hinzufügen neuer Konferenzthemen, partizipative Methoden und Projekte, die über das Leben der Konferenz dauern wird.";

                SampleUtilities.CreateLocalizedEvent(id, title, content, summary, startDate, endDate, street, city, country, state, email, website, contactName, cellPhone, phone, null, null, "en");
                SampleUtilities.CreateLocalizedEvent(id, titleDE, contentDE, summaryDE, startDate, endDate, string.Empty, string.Empty, string.Empty, string.Empty, email, website, string.Empty, string.Empty, string.Empty, null, null, "de");

                id = Guid.NewGuid();
                title = "West Coast Energy Management Congress";
                content = "<p>The West Coast Energy Management Congress (EMC) is the largest energy conference and technology expo held on the U.S. West Coast specifically for business, industrial and institutional energy users. It brings together the top experts in all areas of the field to help you set a clear, optimum path to energy efficiency, facility optimization and sustainability, as well as innovation solutions to improve your ROI. You can explore promising new technologies, compare energy supply and alternative energy options, and learn about innovative project implementation strategies. The multi-track conference covers a variety of topics, many specific to the region.</p>";
                summary = "The West Coast Energy Management Congress (EMC) is the largest energy conference and technology expo held on the U.S. West Coast specifically for business, industrial and institutional energy users.";
                startDate = startDate.AddDays(10);
                endDate = endDate.AddDays(10);
                street = "Turner blvd.";
                city = "Long Beach";
                country = "USA";
                state = "CA";
                email = "info@aeecenter.org";
                website = "http://www.ecocity2011.com/";
                contactName = "Jordan Angelov";
                cellPhone = "+359 886 454 221";
                phone = "+359 2 871 54 11";

                titleDE = "West Coast Energy Management Congress";
                contentDE = @"Die West Coast Energy Management Congress (EMC) ist der gr&ouml;&szlig;te Energie-Konferenz und Technik expo auf der Westk&uuml;ste der USA speziell f&uuml;r gesch&auml;ftliche, industrielle und institutionelle Energieverbraucher statt. Es vereint die Top-Experten in allen Bereichen des Feldes, um Ihnen ein klares, optimalen Weg zur Steigerung der Energieeffizienz, Anlagenoptimierung und Nachhaltigkeit sowie innovative L&ouml;sungen, um Ihren ROI zu verbessern. Erkunden Sie vielversprechende neue Technologien, zu vergleichen Energieversorgung und alternative Energie-Optionen, und lernen Sie innovative Projekt Umsetzungsstrategien. Die Multi-Track-Konferenz deckt eine Vielzahl von Themen, viele spezifisch f&uuml;r die Region.";
                summaryDE = "Die West Coast Energy Management Congress (EMC) ist der größte Energie-Konferenz und Technik expo auf der Westküste der USA speziell für geschäftliche, industrielle und institutionelle Energieverbraucher statt.";

                SampleUtilities.CreateLocalizedEvent(id, title, content, summary, startDate, endDate, street, city, country, state, email, website, contactName, cellPhone, phone, new List<string>() { "Toronto", "DevTeach", "Conferences", "sustainability", "energy efficiency", "energy" }, new List<string>() { "Conferences", "International events" }, "en");
                SampleUtilities.CreateLocalizedEvent(id, titleDE, contentDE, summaryDE, startDate, endDate, string.Empty, string.Empty, string.Empty, string.Empty, email, website, string.Empty, string.Empty, string.Empty, null, null, "de");
            }
        }

        private void CreateLists()
        {
            var result = SampleUtilities.CreateLocalizedList(new Guid(SampleConstants.AnnouncementsListId), SampleConstants.AnnouncementsListName, SampleConstants.AnnouncementsListName, "en");

            if (result)
            {
                SampleUtilities.CreateLocalizedList(new Guid(SampleConstants.AnnouncementsListId), SampleConstants.AnnouncementsListNameGerman, SampleConstants.AnnouncementsListNameGerman, "de");

                //List item 1
                var parentListId = new Guid(SampleConstants.AnnouncementsListId);
                var title = "TIUNACA strike and impact on Faculty of Law activities";
                var content = "<p>The Telerik International University Non-academic Certified Association (TIUNACA) is currently on strike. TIUNACA represents many of the clerical workers and other support staff who work at the Faculty of Law. Despite the strike and the presence of picket lines, the Faculty remains open. Classes and events will proceed as scheduled. The strike will however have an impact on the Faculty&rsquo;s services.</p>";
                var id = Guid.Empty;
                var owner = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                title = "Die TIUNACA Streik und die Auswirkungen auf Juristischen Fakultät der Aktivitäten";
                content = "<p>Die Telerik International University Nicht-akademische Certified Association (TIUNACA) ist derzeit in den Streik. TIUNACA stellt viele der Angestellten und sonstigen Support-Mitarbeiter, die an der Juristischen Fakult&auml;t der Arbeit. Trotz des Streiks und der Anwesenheit von Streikposten, bleibt der Fakult&auml;t offen. Klassen und Ereignisse werden sich wie geplant. Der Streik wird allerdings Auswirkungen auf die Fakult&auml;t in Anspruch.</p>";

                SampleUtilities.CreateLocalizedListItem(id, title, content, "de");

                //List item 2
                title = "Professor Jordan Angelov made a Fellow of the Emperor Society of Japan";
                content = "<p>The Emperor Society of Japan announced 78 new Fellows this week, including nine TIU researchers and scholars from the faculties of Design, Law and Arts. Among them is Professor Jordan Angelov, who holds the TIU Research Chair in Design and Art. Find out more in the TIU Reporter.</p>";
                owner = Guid.Empty;
                id = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                title = "Professor Jordan Angelov machte ein Fellow des Kaisers Society of Japan";
                content = "<p>Der Kaiser Society of Japan k&uuml;ndigte 78 neue Stipendiaten in dieser Woche, darunter neun TIU Forscher und Wissenschaftler aus den Fakult&auml;ten f&uuml;r Gestaltung, Recht und Kunst. Unter ihnen ist Professor Jordan Angelov, der TIU Research Chair in Design und Kunst h&auml;lt. Erfahren Sie mehr in der TIU Reporter.</p>";

                SampleUtilities.CreateLocalizedListItem(id, title, content, "de");

                //List item 3
                title = "Computing Services support for new students";
                content = "<p>On-campus drop-in clinics to help students connect their laptops to the University network using eduroam and the Halls network using ResNet. Located at the CSD Helpdesk in the Malinov Building (Building number 224 on the University).</p>";
                owner = Guid.Empty;
                id = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                title = "Computing Services Unterstützung für neue Schüler";
                content = "<p>On-Campus-drop-in Kliniken zu helfen Studenten verbinden ihren Laptops an der Universit&auml;t Netzwerk mit eduroam und der Hallen-Netzwerk mit RESNET. Das Hotel liegt an der CSD-Helpdesk in den Malinov (Geb&auml;ude Nummer 224 an der Universit&auml;t).</p>";

                SampleUtilities.CreateLocalizedListItem(id, title, content, "de");
            }
        }

        private void CreateBlogPosts()
        {
            List<string> cultures = new List<string>() { "en", "de" };
            var result = SampleUtilities.CreateLocalizedBlog(new Guid(SampleConstants.TIUBlogId), "TIU Blog", "TIU Blog", cultures);
            Guid userId = SampleUtilities.GetUserIdByUserName("admin");

            if (result)
            {
                // Blog post 1
                var id = Guid.NewGuid();
                var title = "International Understanding and Cooperation";
                var content = @"<p>The TIU Museum, like so many encyclopaedic museums, plays a critical role in fostering international understanding and cooperation. The Museum accomplishes this through insightful exhibitions, collaborating with community partners and sharing expertise and experience with international colleagues. The Cultures collection frequently forms the basis of this work as the 17,000 objects are evidence of intercultural engagement from the 19<sup>th</sup> century onwards.</p><p>Most recently the Museum hosted 3 curators who were participants on the International Training Programme as coordinated by the Bulgarian Museum. The Museum collaborates with the Bulgarian Museum on this programme each year as the chance to converse with international colleagues is invaluable. This years curators were especially interested in the use of post-colonial critique in the Museum&rsquo;s approach to community engagement, exhibition development and the Cultures collection.&nbsp;</p>";

                SampleUtilities.CreateLocalizedBlogPost(id, new Guid(SampleConstants.TIUBlogId), title, content, null, userId, null, null, "en");

                title = @"Internationale Verständigung und Zusammenarbeit";
                content = @"<p>Die TIU Museum, wie so viele enzyklop&auml;dische Museen, spielt eine entscheidende Rolle bei der F&ouml;rderung internationaler Verst&auml;ndigung und Zusammenarbeit. Das Museum erreicht dies durch aufschlussreiche Ausstellungen, die Zusammenarbeit mit kommunalen Partnern und Austausch von Fachwissen und Erfahrung mit internationalen Kollegen. Die Kulturen der Sammlung bildet h&auml;ufig die Grundlage dieser Arbeit, wie die 17.000 Objekte Beweis f&uuml;r interkulturelles Engagement aus dem 19. Jahrhundert sind. </p> <p>In j&uuml;ngster Zeit das Museum gehostet 3 Kuratoren, die Teilnehmer auf dem International Training, wie es von der bulgarischen Museum koordiniert wurden. Das Museum arbeitet mit dem bulgarischen Museum &uuml;ber dieses Programm jedes Jahr die Chance, mit internationalen Kollegen zu unterhalten ist von unsch&auml;tzbarem Wert. In diesem Jahr Kuratoren waren besonders daran interessiert, den Einsatz von postkolonialen Kritik in das Museum der Ansatz, um gesellschaftliches Engagement, Ausstellung Entwicklung und die Kulturen der Sammlung.</p>";

                SampleUtilities.CreateLocalizedBlogPost(id, new Guid(SampleConstants.TIUBlogId), title, content, null, userId, null, null, "de");

                // Blog post 2
                var id2 = Guid.NewGuid();
                title = "Go West";
                content = @"<p>As a young child in the 1980s I have vague memories of watching a Japanese television show called <em>Monkey</em>. The show was an explosion of martial arts, monsters and magic. The electro-psychedelic theme tune by Godiego was particularly catchy. It wasn&rsquo;t until the early 2000s as a student when I rediscovered this cult Japanese show, it was a welcome distraction from late night study. </p> <iframe width=""560"" height=""315"" frameborder=""0"" src=""http://www.youtube.com/embed/Yr5ZWYRaAyw""></iframe>
<p>&nbsp;</p> <p>The show was of course a 1970s interpretation of the 16th century Ming dynasty novel Journey to the West by author Wu Cheng&rsquo;en. The novel details the adventures of the Buddhist monk Tripitaka and his 14 year and 108,000 mile odyssey, with his 3 supernatural companions Monkey, Pigsy and Sandy, to retrieve Buddhist scriptures from the Thunderclap Monastery in India.</p> <p>Having realised that this piece of Japanese pop culture was actually based on a Chinese epic novel I began reading Wu Cheng&rsquo;en&rsquo;s text. Whilst reading volume 3 on a train a young Chinese woman was rather tickled as in her opinion I was reading a children&rsquo;s story. In China the story is very popular amongst the younger generation and many animations have been based on the novel. The story has not only been a stimulus for animators but graphic novelists, computer game designers, and television, film and theatre directors too. The characters were even used by the BBC to advertise their coverage of the 2008 Beijing Olympics.</p>";

                SampleUtilities.CreateLocalizedBlogPost(id2, new Guid(SampleConstants.TIUBlogId), title, content, null, userId, null, null, "en");

                content = @"<p> Als kleines Kind in den 1980er Jahren habe ich noch vage Erinnerungen an Ansehen eines japanischen TV-Show namens <em> Affe <!-- em-->. Die Show war eine Explosion der Kampfkunst, Monster und Magie. Die elektro-psychedelischen Titelmelodie von Godiego war besonders eing&auml;ngig. Erst Anfang der 2000er Jahre als Student, als ich diesen Kult japanische zeigen wiederentdeckt, es ist eine willkommene Ablenkung von Late-Night-Studie war. </em></p> <iframe width=""560"" height=""315"" frameborder=""0"" src=""http://www.youtube.com/embed/Yr5ZWYRaAyw""> </iframe> <p> </p> <p>Die Show war nat&uuml;rlich ein 1970er Interpretation des 16. Jahrhunderts Ming-Dynastie Roman Journey to the West nach Autor Wu Cheng'en. Der Roman beschreibt die Abenteuer der buddhistische M&ouml;nch Tripitaka und seine 14 Jahre und 108.000 Meile Odyssee, mit seinen 3 &uuml;bernat&uuml;rlichen Begleiter Monkey, Pigsy und Sandy, um buddhistische Schriften aus dem Donnerknall Kloster in Indien zu erhalten. </p> <p> Nachdem klar, dass dieses St&uuml;ck der japanischen Popkultur war eigentlich auf einem chinesischen Epos Ich begann zu lesen Wu Cheng'en Text basiert. Beim Durchlesen Band 3 in einem Zug eine junge chinesische Frau eher als in ihrer Meinung, die ich las eine Geschichte f&uuml;r Kinder war gekitzelt. In China ist die Geschichte sehr beliebt bei der j&uuml;ngeren Generation und vielen Animationen auf dem gleichnamigen Roman basiert. Die Geschichte hat nicht nur ein Ansporn f&uuml;r Animatoren, aber Grafik Romanciers, Computerspiel-Designer, und Fernsehen, Film und Theater Regisseure auch schon. Die Charaktere wurden sogar von der BBC verwendet, um die Berichterstattung &uuml;ber die Olympiade 2008 in Peking zu werben.</p>";

                SampleUtilities.CreateLocalizedBlogPost(id2, new Guid(SampleConstants.TIUBlogId), title, content, null, userId, null, null, "de");

                var author = new AuthorProxy("Veronica", "veronica@test.com");

                var blogsManager = BlogsManager.GetManager();
                var master = blogsManager.GetBlogPost(id2);
                var liveId = blogsManager.Lifecycle.GetLive(master).Id;

                SampleUtilities.CreateBlogPostComment(liveId, "Thank you for this awesome post!", author, string.Empty, "::1");
            }
        }

        private void UploadImages()
        {
            List<string> cultures = new List<string>() { "en", "de" };

            SampleUtilities.UploadLocalizedImages(HttpRuntime.AppDomainAppPath + "Images\\Default Images", "Default Images", new Guid(SampleConstants.DefaultImagesAlbumId), cultures);
            SampleUtilities.UploadLocalizedImages(HttpRuntime.AppDomainAppPath + "Images\\Generic Images", "Generic Images", new Guid(SampleConstants.GenericImagesAlbumId), cultures);
            SampleUtilities.UploadLocalizedImages(HttpRuntime.AppDomainAppPath + "Images\\Headers", "Headers", new Guid(SampleConstants.HeadersAlbumId), cultures);
            SampleUtilities.UploadLocalizedImages(HttpRuntime.AppDomainAppPath + "Images\\History", "History", new Guid(SampleConstants.HistoryAlbumId), cultures);
            SampleUtilities.UploadLocalizedImages(HttpRuntime.AppDomainAppPath + "Images\\Users", "Users", new Guid(SampleConstants.UsersAlbumId), cultures);

            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("campus-life", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("sport", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("admissions", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("scholarships_header", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("library", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("graduate", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("diversity", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("campus-header", "en"), new List<string>() { "homepage" });
            SampleUtilities.SetTagsToImage(SampleUtilities.GetLocalizedImageMasterId("academics", "en"), new List<string>() { "homepage" });
        }

        private void UploadVideos()
        {
            List<string> cultures = new List<string>() { "en", "de" };

            Dictionary<string, string> thumbnails = new Dictionary<string, string>();

            string title = "Animate education";
            var imagePath = HttpRuntime.AppDomainAppPath + "Videos\\Thumbnails\\Animate-education.jpg";
            thumbnails.Add(title, imagePath);

            title = "Drive the surprising truth about what motivates us";
            imagePath = HttpRuntime.AppDomainAppPath + "Videos\\Thumbnails\\Drive-the-surprising-truth-about-what-motivates-us.jpg";
            thumbnails.Add(title, imagePath);

            title = "Where good ideas come from";
            imagePath = HttpRuntime.AppDomainAppPath + "Videos\\Thumbnails\\Where-good-ideas-come-from.jpg";
            thumbnails.Add(title, imagePath);

            var libraryId = new Guid(SampleConstants.OnlineCoursesLibraryId);
            SampleUtilities.UploadLocalizedVideos(HttpRuntime.AppDomainAppPath + SampleConstants.VideosFolderName,
                SampleConstants.OnlineCoursesLibraryName, libraryId, cultures, thumbnails);
        }

        private void CreateDocuments()
        {
            List<string> cultures = new List<string>() { "en", "de" };

            SampleUtilities.UploadLocalizedDocuments(HttpRuntime.AppDomainAppPath + "Documents", "Document base", new Guid(SampleConstants.DocumentBaseLibraryId), cultures);
        }

        private void RegisterControls()
        {
            //SampleUtilities.RegisterToolboxWidget("CoverFlow", typeof(CoverFlow), "Custom");
        }

        private void RegisterControlTemplates()
        {
            var templatePath = "SitefinityWebApp.Widgets.Templates.EducationSummary.ascx";
            var assemblyName = "SitefinityWebApp";

            SampleUtilities.RegisterAspNetControlTemplate(SampleConstants.EducationNewsSummaryTemplateName, templatePath, assemblyName, typeof(Telerik.Sitefinity.Modules.News.Web.UI.MasterListView));

            templatePath = "SitefinityWebApp.Widgets.Templates.EducationAnnouncementsTemplate.ascx";

            SampleUtilities.RegisterAspNetControlTemplate(SampleConstants.EducationAnnouncementsTemplateName, templatePath, assemblyName, typeof(ExpandedListItemsMasterView));

            templatePath = "SitefinityWebApp.Widgets.Templates.EducationNewsFullItemTemplate.ascx";
            SampleUtilities.RegisterAspNetControlTemplate(SampleConstants.EducationNewsFullItemTemplateName, templatePath, assemblyName, typeof(Telerik.Sitefinity.Modules.News.Web.UI.DetailsSimpleView));

            templatePath = "SitefinityWebApp.Widgets.Templates.EducationEventDetailView.ascx";
            SampleUtilities.RegisterAspNetControlTemplate(SampleConstants.EducationEventsSingleItemTemplateName, templatePath, assemblyName, typeof(Telerik.Sitefinity.Modules.Events.Web.UI.Public.DetailsView));
        }

        private void RegisterContentViews()
        {
            var templateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.News.Web.UI.MasterListView), SampleConstants.EducationNewsSummaryTemplateName);
            SampleUtilities.RegisterNewsFrontendView(SampleConstants.NewsFrontendContentViewControlName,
                templateKey,
                typeof(Telerik.Sitefinity.Modules.News.Web.UI.MasterListView),
                SampleConstants.EducationNewsSummaryContentViewName,
                6,
                false,
                new Guid(SampleConstants.NewsPageId));

            templateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.News.Web.UI.DetailsSimpleView), SampleConstants.EducationNewsFullItemTemplateName);

            SampleUtilities.RegisterNewsFrontendDetailsView("NewsFrontend", templateKey, typeof(Telerik.Sitefinity.Modules.News.Web.UI.DetailsSimpleView), SampleConstants.EducationNewsItemDetailView, true);

            templateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.Libraries.Web.UI.Videos.MasterThumbnailLightBoxView), SampleConstants.ThumbnailsOverlayLightBoxTemplateName);
            SampleUtilities.RegisterVideosFrontendView(SampleConstants.VideosFrontendContentViewControlName,
                templateKey,
                typeof(Telerik.Sitefinity.Modules.Libraries.Web.UI.Videos.MasterThumbnailLightBoxView),
                SampleConstants.VideosEducationFrontendLightBoxViewName,
                50,
                true,
                new Guid(SampleConstants.OnlineCoursesLibraryId));

            templateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.Libraries.Web.UI.Images.MasterThumbnailLightBoxView), "List of thumbnails and overlay dialog (lightbox)");

            QueryItem q = new QueryItem()
            {
                IsGroup = true,
                Ordinal = 0,
                Join = "AND",
                ItemPath = "_0",
                Value = null,
                Condition = null,
                Name = "Tags"
            };

            QueryItem q1 = new QueryItem()
            {
                IsGroup = false,
                Ordinal = 0,
                Join = "OR",
                ItemPath = "_0_0",
                Condition = new Condition()
                            {
                                FieldName = "Tags",
                                FieldType = "System.Guid",
                                Operator = "Contains",
                            },
                Name = "homepage",
                Value = "913E28AA-0DE2-470d-9C3F-000000000005"
            };

            List<QueryItem> queryItems = new List<QueryItem>();
            queryItems.Add(q);
            queryItems.Add(q1);

            QueryData additionalFilter1 = new QueryData()
            {
                QueryItems = queryItems.ToArray(),
                TypeProperties = new string[0],
                Title = null,
            };

            SampleUtilities.RegisterImagesFrontendView("ImagesFrontend", templateKey, typeof(Telerik.Sitefinity.Modules.Libraries.Web.UI.Images.MasterThumbnailLightBoxView), "ImagesFrontendEducationThumbnailsListLightBox", additionalFilter1);

            templateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.Events.Web.UI.Public.DetailsView), SampleConstants.EducationEventsSingleItemTemplateName);
            SampleUtilities.RegisterEventsFrontendDetailsView("EventsFrontend", templateKey, typeof(Telerik.Sitefinity.Modules.Events.Web.UI.Public.DetailsView), SampleConstants.EducationEventItemDetailView);
        }

        private void CreateEducationThemeAndTemplate()
        {
            SampleUtilities.RegisterTheme(SampleConstants.EducationThemeName, SampleConstants.EducationThemePath);
            SampleUtilities.RegisterTheme(SampleConstants.EducationInGermanThemeName, SampleConstants.EducationInGermanThemePath);

            SampleUtilities.RegisterTemplate(new Guid(SampleConstants.EducationTemplateId), SampleConstants.EducationTemplateName, SampleConstants.EducationTemplateName, SampleConstants.EducationTemplateMasterPage, SampleConstants.EducationThemeName, CultureInfo.GetCultureInfo("en"));
            var result = SampleUtilities.RegisterTemplate(new Guid(SampleConstants.EducationTemplateId), SampleConstants.EducationInGermanTemplateName, SampleConstants.EducationInGermanTemplateName, SampleConstants.EducationTemplateMasterPage, SampleConstants.EducationInGermanThemeName, CultureInfo.GetCultureInfo("de"));

            if (result)
            {
                // Main layout
                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 50),
                    ColumnWidthPercentage = 70,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnWidthPercentage = 30,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);
                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Header";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), mainLayoutControl, "headerInfo", "Header");

                // Logo
                var logoBlock = new ContentBlock();
                logoBlock.Html = string.Format(@"<h1><a href=""[pages]{0}"">Telerik International University</a></h1> <h3>established 2005, Sofia</h3>", SampleConstants.HomePageId);
                logoBlock.CssClass = "sfContentBlock";
                string logoId = SampleUtilities.AddLocalizedControlToTemplate(new Guid(SampleConstants.EducationTemplateId), logoBlock, "Header_Left", "Content block", "en");

                Dictionary<string, object> logoProperties = new Dictionary<string, object>();

                logoProperties.Add("Html", string.Format(@"<h1><a href=""[pages]{0}"">Internationale Universit&auml;t Telerik</a></h1> <h3>gegr&uuml;ndet im 2005, in Sofia</h3>", SampleConstants.HomePageId));
                SampleUtilities.UpdateLocalizedControlInTemplate(logoId, new Guid(SampleConstants.EducationTemplateId), logoProperties, "de");

                // Language/search layout
                var languagesSearchLayoutControl = new LayoutControl();
                List<ColumnDetails> languagesSearchLayoutColumns = new List<ColumnDetails>();
                ColumnDetails languagesSearchLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 46,
                    PlaceholderId = "Left"
                };
                languagesSearchLayoutColumns.Add(languagesSearchLayoutColumn1);
                ColumnDetails languagesSearchLayoutColumn2 = new ColumnDetails()
                {
                    ColumnWidthPercentage = 54,
                    PlaceholderId = "Right"
                };
                languagesSearchLayoutColumns.Add(languagesSearchLayoutColumn2);
                languagesSearchLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(languagesSearchLayoutColumns, string.Empty);
                languagesSearchLayoutControl.ID = "LanguageSearch";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), languagesSearchLayoutControl, "Header_Right", "Language and Search");

                // Language
                var languages = new LanguageSelectorControl();
                languages.SelectorType = LanguageSelectorType.Vertical;
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), languages, "LanguageSearch_Right", "Language selector");

                // Login layout
                //var loginLayoutControl = new LayoutControl();
                //List<ColumnDetails> loginLayoutColumns = new List<ColumnDetails>();
                //ColumnDetails loginLayoutColumn1 = new ColumnDetails()
                //{
                //    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                //    PlaceholderId = "Center"
                //};
                //loginLayoutColumns.Add(loginLayoutColumn1);
                //loginLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(loginLayoutColumns, "login-status");
                //loginLayoutControl.ID = "Login";
                //SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), loginLayoutControl, "Header_Right", "Login");

                //// Login name
                //LoginNameControl loginNameControl = new LoginNameControl();
                //SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), loginNameControl, "Login_Center", "Login name");

                //// Login status
                //LoginStatusControl loginStatusControl = new LoginStatusControl();
                //SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), loginStatusControl, "Login_Center", "Login status");

                // Image
                ImageControl headerImage = new ImageControl();
                headerImage.ImageId = SampleUtilities.GetLocalizedImageId("the_building", "en");
                headerImage.CssClass = "headerImage";
                string controlId = SampleUtilities.AddLocalizedControlToTemplate(new Guid(SampleConstants.EducationTemplateId), headerImage, "image", "Image", "en");

                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add("ImageId", SampleUtilities.GetLocalizedImageId("headerspring1", "de"));
                SampleUtilities.UpdateLocalizedControlInTemplate(controlId, new Guid(SampleConstants.EducationTemplateId), properties, "de");

                // Explore your potentials
                ContentBlockBase exploreContentBlock = new ContentBlockBase();
                exploreContentBlock.Html = @"<h4>Explore your potentials</h4> <p>Share your voice and unlock your potentials while we will equip you with the skills you need for the future!</p>";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), exploreContentBlock, "tagline", "Content block");

                // Header navigation
                var headerNavigationControl = new NavigationControl();
                headerNavigationControl.NavigationMode = NavigationModes.HorizontalSimple;
                headerNavigationControl.SelectionMode = PageSelectionModes.TopLevelPages;
                headerNavigationControl.Skin = "education";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), headerNavigationControl, "header", "Navigation");

                // Footer navigation
                var footerNavigationControl = new NavigationControl();
                footerNavigationControl.NavigationMode = NavigationModes.SiteMapInColumns;
                footerNavigationControl.SelectionMode = PageSelectionModes.TopLevelPages;
                footerNavigationControl.Skin = "footer";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.EducationTemplateId), footerNavigationControl, "footernavigation", "Navigation");
            }
        }

        private void CreateFacebookThemeAndTemplate()
        {
            SampleUtilities.RegisterTheme(SampleConstants.FacebookThemeName, SampleConstants.FacebookThemePath);

            var result = SampleUtilities.RegisterTemplate(new Guid(SampleConstants.FacebookTemplateId), SampleConstants.FacebookTemplateName, SampleConstants.FacebookTemplateName,
                SampleConstants.FacebookTemplateMasterPage, SampleConstants.FacebookThemeName, CultureInfo.GetCultureInfo("en"));

            if (result)
            {
                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 100,
                    PlaceholderId = "Center"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, "header");
                mainLayoutControl.ID = "Header";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.FacebookTemplateId), mainLayoutControl, "content_inside", "100% (custom)");

                // Logo
                var logoBlock = new ContentBlock();
                logoBlock.Html = @"<h1>Telerik International University</h1>";
                logoBlock.CssClass = "sfContentBlock";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.FacebookTemplateId), logoBlock, "Header_Center", "Content block");

                // Image
                ImageControl universityImage = new ImageControl();
                universityImage.ImageId = SampleUtilities.GetLocalizedImageId("university_520", "en");
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.FacebookTemplateId), universityImage, "Header_Center", "Image");

                var contentLayoutControl = new LayoutControl();
                List<ColumnDetails> contentLayoutColumns = new List<ColumnDetails>();
                ColumnDetails contentLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 100,
                    PlaceholderId = "Center"
                };
                contentLayoutColumns.Add(contentLayoutColumn1);
                contentLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(contentLayoutColumns, "content");
                contentLayoutControl.ID = "Content";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.FacebookTemplateId), contentLayoutControl, "Header_Center", "100% (custom)");

                var footerLayoutControl = new LayoutControl();
                List<ColumnDetails> footerLayoutColumns = new List<ColumnDetails>();
                ColumnDetails footerLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 50,
                    PlaceholderId = "Left"
                };
                footerLayoutColumns.Add(footerLayoutColumn1);
                ColumnDetails footerLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 50,
                    PlaceholderId = "Right"
                };
                footerLayoutColumns.Add(footerLayoutColumn2);
                footerLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(footerLayoutColumns, "footer");
                footerLayoutControl.ID = "Footer";
                SampleUtilities.AddControlToTemplate(new Guid(SampleConstants.FacebookTemplateId), footerLayoutControl, "Header_Center", "50% + 50% (custom)");
            }
        }

        // PAGES:

        private void CreateHomePage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.HomePageId), "Home", true, false, "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.HomePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                var mainLayoutControl = new LayoutControl();
                var mainLayoutColumns = new List<ColumnDetails>();

                var mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 15, 0, 0),
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);

                var mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 15),
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), mainLayoutControl, "content", "50% + 50% (custom)", "en");

                //Left content

                #region Latest News Content Block

                var latestNewsBlockLayout = new LayoutControl();
                var latestNewsBlockLayoutColumns = new List<ColumnDetails>();

                var latestNewsBlockLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                latestNewsBlockLayoutColumns.Add(latestNewsBlockLayoutColumn1);

                latestNewsBlockLayout.Layout = SampleUtilities.GenerateLayoutTemplate(latestNewsBlockLayoutColumns, string.Empty);
                latestNewsBlockLayout.ID = "LatestNewsBlockLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), latestNewsBlockLayout, "Main_Left", "100%", "en");

                ContentBlockBase latestNewsBlock = new ContentBlockBase();
                latestNewsBlock.Html = @"<h1>Latest university news</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), latestNewsBlock, "LatestNewsBlockLayout_Content", "Content block", "en");

                #endregion

                #region Latest News View

                var latestNewsViewLayout = new LayoutControl();
                var latestNewsViewLayoutColumns = new List<ColumnDetails>();

                var latestNewsViewLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                latestNewsViewLayoutColumns.Add(latestNewsViewLayoutColumn1);

                latestNewsViewLayout.Layout = SampleUtilities.GenerateLayoutTemplate(latestNewsViewLayoutColumns, string.Empty);
                latestNewsViewLayout.ID = "LatestNewsViewLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), latestNewsViewLayout, "Main_Left", "100%", "en");

                NewsView latestNews = new NewsView();
                latestNews.MasterViewName = SampleConstants.EducationNewsSummaryContentViewName;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), latestNews, "LatestNewsViewLayout_Content", "News", "en");

                #endregion

                #region Videos Content Block

                var videosBlockLayout = new LayoutControl();
                var videosBlockLayoutColumns = new List<ColumnDetails>();

                var videosBlockLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                videosBlockLayoutColumns.Add(videosBlockLayoutColumn1);

                videosBlockLayout.Layout = SampleUtilities.GenerateLayoutTemplate(videosBlockLayoutColumns, string.Empty);
                videosBlockLayout.ID = "VideosBlockLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), videosBlockLayout, "Main_Left", "100%", "en");

                ContentBlockBase videosBlock = new ContentBlockBase();
                videosBlock.Html = @"<h1>Videos</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), videosBlock, "VideosBlockLayout_Content", "Content block", "en");

                #endregion

                #region Videos view

                var videosViewLayout = new LayoutControl();
                var videosViewLayoutColumns = new List<ColumnDetails>();

                var videosViewLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                videosViewLayoutColumns.Add(videosViewLayoutColumn1);

                videosViewLayout.Layout = SampleUtilities.GenerateLayoutTemplate(videosViewLayoutColumns, string.Empty);
                videosViewLayout.ID = "VideosViewLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), videosViewLayout, "Main_Left", "100%", "en");

                VideosView videoGallery = new VideosView();
                videoGallery.MasterViewName = SampleConstants.VideosEducationFrontendLightBoxViewName;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), videoGallery, "VideosViewLayout_Content", "Video gallery", "en");

                #endregion

                #region Meet us Image

                var meetUsImageLayout = new LayoutControl();
                var meetUsImageLayoutColumns = new List<ColumnDetails>();

                var meetUsImageLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                meetUsImageLayoutColumns.Add(meetUsImageLayoutColumn1);

                meetUsImageLayout.Layout = SampleUtilities.GenerateLayoutTemplate(meetUsImageLayoutColumns, string.Empty);
                meetUsImageLayout.ID = "MeetUsImageLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), meetUsImageLayout, "Main_Left", "100%", "en");

                ImageControl meetUsImage = new ImageControl();
                meetUsImage.CssClass = "sfimageWrp bottomImage";
                meetUsImage.ImageId = SampleUtilities.GetLocalizedImageId("1meet_us", "en");
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), meetUsImage, "MeetUsImageLayout_Content", "Image", "en");

                #endregion

                //Right content

                #region Upcoming events block

                var upcomingEventsBlockLayout = new LayoutControl();
                var upcomingEventsBlockLayoutColumns = new List<ColumnDetails>();

                var upcomingEventsBlockLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                upcomingEventsBlockLayoutColumns.Add(upcomingEventsBlockLayoutColumn1);

                upcomingEventsBlockLayout.Layout = SampleUtilities.GenerateLayoutTemplate(upcomingEventsBlockLayoutColumns, string.Empty);
                upcomingEventsBlockLayout.ID = "UpcomingEventsBlockLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), upcomingEventsBlockLayout, "Main_Right", "100%", "en");

                ContentBlockBase upcomingEventsBlock = new ContentBlockBase();
                upcomingEventsBlock.Html = @"<h1>Upcoming events</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), upcomingEventsBlock, "UpcomingEventsBlockLayout_Content", "Content block", "en");

                #endregion

                #region Upcoming events image

                var upcomingEventsImageLayout = new LayoutControl();
                var upcomingEventsImageLayoutColumns = new List<ColumnDetails>();

                var upcomingEventsImageLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                upcomingEventsImageLayoutColumns.Add(upcomingEventsImageLayoutColumn1);

                upcomingEventsImageLayout.Layout = SampleUtilities.GenerateLayoutTemplate(upcomingEventsImageLayoutColumns, string.Empty);
                upcomingEventsImageLayout.ID = "UpcomingEventsImageLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), upcomingEventsImageLayout, "Main_Right", "100%", "en");

                ImageControl upcomingEventsImage = new ImageControl();
                upcomingEventsImage.ImageId = SampleUtilities.GetLocalizedImageId("event", "en");
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), upcomingEventsImage, "UpcomingEventsImageLayout_Content", "Image", "en");

                #endregion

                #region Welcome block

                var welcomeBlockLayout = new LayoutControl();
                var welcomeBlockLayoutColumns = new List<ColumnDetails>();

                var welcomeBlockLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                welcomeBlockLayoutColumns.Add(welcomeBlockLayoutColumn1);

                welcomeBlockLayout.Layout = SampleUtilities.GenerateLayoutTemplate(welcomeBlockLayoutColumns, string.Empty);
                welcomeBlockLayout.ID = "WelcomeBlockLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), welcomeBlockLayout, "Main_Right", "100%", "en");

                ContentBlockBase welcomeBlock = new ContentBlockBase();
                Guid deanImageId = SampleUtilities.GetLocalizedImageId("dadean", "en");
                //string deanImageUrl = SampleUtilities.GetLocalizedImageDefaultUrl("dadean", "en");

                welcomeBlock.Html = string.Format(@"<h1>Welcome from the dean</h1> <p><img alt="""" src=""[images]{0}"" /><em>Welcome to Telerik International University. As dean of the university, I am proud of what we have to offer, and I am delighted that you are interested in learning more about us. Our region has a fascinating history, a creative arts community and fantastic recreational opportunities.</em><br /> <br /> </p>", deanImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), welcomeBlock, "WelcomeBlockLayout_Content", "Content block", "en");

                #endregion

                #region Photos block

                var photosBlockLayout = new LayoutControl();
                var photosBlockLayoutColumns = new List<ColumnDetails>();

                var photosBlockLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                photosBlockLayoutColumns.Add(photosBlockLayoutColumn1);

                photosBlockLayout.Layout = SampleUtilities.GenerateLayoutTemplate(photosBlockLayoutColumns, string.Empty);
                photosBlockLayout.ID = "PhotosBlockLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), photosBlockLayout, "Main_Right", "100%", "en");

                ContentBlockBase photosBlock = new ContentBlockBase();
                photosBlock.Html = @"<h1>Photos</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), photosBlock, "PhotosBlockLayout_Content", "Content block", "en");

                #endregion

                #region Images view

                var photosGalleryLayout = new LayoutControl();
                var photosGalleryLayoutColumns = new List<ColumnDetails>();

                var photosGalleryLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                photosGalleryLayoutColumns.Add(photosGalleryLayoutColumn1);

                photosGalleryLayout.Layout = SampleUtilities.GenerateLayoutTemplate(photosGalleryLayoutColumns, string.Empty);
                photosGalleryLayout.ID = "PhotosGalleryLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), photosGalleryLayout, "Main_Right", "100%", "en");

                ImagesView gallery = new ImagesView();
                gallery.MasterViewName = "ImagesFrontendEducationThumbnailsListLightBox";

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), gallery, "PhotosGalleryLayout_Content", "Images gallery", "en");

                #endregion

                #region Announcements list

                var announcementsListLayout = new LayoutControl();
                var announcementsListLayoutColumns = new List<ColumnDetails>();

                var announcementsListLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    PlaceholderId = "Content"
                };
                announcementsListLayoutColumns.Add(announcementsListLayoutColumn1);

                announcementsListLayout.Layout = SampleUtilities.GenerateLayoutTemplate(announcementsListLayoutColumns, string.Empty);
                announcementsListLayout.ID = "AnnouncementsListLayout";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), announcementsListLayout, "Main_Right", "100%", "en");

                Telerik.Sitefinity.Modules.Lists.Web.UI.ListView announcements = new Telerik.Sitefinity.Modules.Lists.Web.UI.ListView();
                announcements.Mode = ListViewMode.Expanded;
                announcements.SelectedListText = "Announcements";
                announcements.SelectedListIds = string.Format(@"[""{0}""]", SampleConstants.AnnouncementsListId);
                announcements.CurrentMasterTemplateId = new Guid(SampleUtilities.GetControlTemplateKey(typeof(ExpandedListItemsMasterView), SampleConstants.EducationAnnouncementsTemplateName));

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), announcements, "AnnouncementsListLayout_Content", "List items", "en");
                #endregion
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.HomePageId), "Startseite", false, false, "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.HomePageId), new Guid(SampleConstants.EducationTemplateId), "de");

                var mainLayoutControl = new LayoutControl();
                var mainLayoutColumns = new List<ColumnDetails>();

                var mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 15, 0, 0),
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);

                var mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 15),
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), mainLayoutControl, "content", "50% + 50% (custom)", "de");

                //Lates news block
                ContentBlockBase latestNewsBlock = new ContentBlockBase();
                latestNewsBlock.Html = @"<h1>Universit&auml;tsnachrichten</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), latestNewsBlock, "Main_Left", "Content block", "de");

                //Latest  news view
                NewsView latestNews = new NewsView();
                latestNews.MasterViewName = SampleConstants.EducationNewsSummaryContentViewName;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), latestNews, "Main_Left", "News", "de");

                //Meet us image
                ImageControl meetUsImage = new ImageControl();
                meetUsImage.ImageId = SampleUtilities.GetLocalizedImageId("besuchen", "de");
                meetUsImage.CssClass = "sfimageWrp bottomImage";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), meetUsImage, "Main_Left", "Image", "de");

                //Upcoming events
                ContentBlockBase upcomingEventsBlock = new ContentBlockBase();
                upcomingEventsBlock.Html = @"<h1>Veranstaltungen</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), upcomingEventsBlock, "Main_Right", "Content block", "de");

                //Upcoming events image
                ImageControl upcomingEventsImage = new ImageControl();
                upcomingEventsImage.ImageId = SampleUtilities.GetLocalizedImageId("event", "de");
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), upcomingEventsImage, "Main_Right", "Image", "de");

                //Welcome block
                ContentBlockBase welcomeBlock = new ContentBlockBase();
                Guid deanImageId = SampleUtilities.GetLocalizedImageId("dadean", "de");

                welcomeBlock.Html = string.Format(@"<h1>Begr&uuml;&szlig;ung des Dekans</h1> <p><img alt="""" src=""[images]{0}"" /><em>Herzlich willkommen auf die Telerik International University. Als Dekan der Universit&auml;t bin ich stolz auf das, was wir zu bieten haben, und ich freue mich, dass Sie an weiteren Informationen &uuml;ber uns interessiert sind. Unsere Region hat eine faszinierende Geschichte, eine kreative Kunst Community und fantastische M&ouml;glichkeiten zur Freizeitgestaltung.</em><br /><br /></p>",
                    deanImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), welcomeBlock, "Main_Right", "Content block", "de");

                Telerik.Sitefinity.Modules.Lists.Web.UI.ListView announcements = new Telerik.Sitefinity.Modules.Lists.Web.UI.ListView();
                announcements.Mode = ListViewMode.Expanded;
                announcements.SelectedListText = "Announcements";
                announcements.SelectedListIds = string.Format(@"[""{0}""]", SampleConstants.AnnouncementsListId);
                announcements.CurrentMasterTemplateId = new Guid(SampleUtilities.GetControlTemplateKey(typeof(ExpandedListItemsMasterView), SampleConstants.EducationAnnouncementsTemplateName));

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HomePageId), announcements, "Main_Right", "List items", "de");
            }
        }

        #region About the university

        private void CreateAboutTheUniversityPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), "About the university", "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase aboutBlock = new ContentBlockBase();
                aboutBlock.Html = @"<h1>About the university</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), aboutBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var about_tiuImageId = SampleUtilities.GetLocalizedImageId("about_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Tradition combined with innovation</h3> <p>Telerik International University, is the newest university in the English-speaking world. The university traces its roots back to the beginning of 2006, although the exact date of foundation remains unclear. Its founders came together after realizing that no places of higher education existed within a fifty mile radius of their home town.</p> <p>The university is dedicated to teaching a wide number of subjects and encouraging all its students to participate in the many extra-curricular pursuits that its student societies put on during term. &ldquo;Work hard, play hard&rdquo; is its motto and those who come to TIU couldn&rsquo;t agree more. Whether you prefer extreme sports, tiddlywinks, developing software, defending people who can&rsquo;t defend themselves, working for charities or running events, there is a society for you and if there isn&rsquo;t, we actively support those who want to start their own.</p> <p> </p> <h3>Young, open, international </h3> <p>Admission to TIU is based equally on academic potential and your resume of sports and activities. There is no weighing in favor of any particular thing. We are interested to make sure that our students continue to develop to their true potential - academically, socially, mentally and physically. All potential students should visit us on an open day before getting application forms and starting to work through the entry process. Because of the high volume of applications and the direct involvement of the faculty in admissions, students may be called to interview any time between March and July.</p> <h3>Ideal conditions for studying</h3> <p>If it takes more than 15 minutes to get to the university from the main train station, the central bus and tram stop, then something has gone wrong somewhere. Trams leave the main station every five minutes and stop right in the middle of the campus. The university registration fee includes a &ldquo;Semester Ticket&rdquo; that gives students free travel on public transport within Sofia. And living in Sofia is almost as cheap: even in the charming city center, the rents are affordable. In addition to this, there are more than 2000 rooms and apartments available in the university's halls of residence.</p>", about_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), "Über uns", false, "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase aboutBlockDE = new ContentBlockBase();
                aboutBlockDE.Html = @"<h1>Über die Universität</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), aboutBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var about_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("about_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Tradition mit Innovation</h3> <p>Telerik International University ist die j&uuml;ngste Universit&auml;t in der englischsprachigen Welt. Die Universit&auml;t f&uuml;hrt ihre Wurzeln zur&uuml;ck an den Anfang des Jahres 2006, obwohl das genaue Datum der Gr&uuml;ndung, bleibt unklar. Die Gr&uuml;nder kamen zusammen, nachdem er feststellte, dass keine Pl&auml;tze der Hochschulbildung in eine f&uuml;nfzig Meile Radius von ihrer Heimatstadt gab.</p> <p>Die Universit&auml;t ist die Lehre eine gro&szlig;e Anzahl von Themen und die F&ouml;rderung aller Sch&uuml;ler in den vielen au&szlig;erschulischen Besch&auml;ftigungen, die ihre Sch&uuml;ler Gesellschaften w&auml;hrend des Semesters gestellt teilnehmen gewidmet. ""Work hard, play hard"" ist sein Motto und diejenigen, die TIU kommen konnte nicht mehr zustimmen. Ob Extremsport, tiddlywinks, Software-Entwicklung, Verteidigung von Menschen, die sich nicht wehren k&ouml;nnen, die sich f&uuml;r Wohlt&auml;tigkeitsorganisationen oder Laufveranstaltungen bevorzugen, gibt es eine Gesellschaft f&uuml;r Sie und wenn es nicht unterst&uuml;tzen wir aktiv die, die ihre eigenen beginnen m&ouml;chten .</p> <h3>Jung, offen, international</h3> <p>Zulassung zum TIU ist gleicherma&szlig;en auf wissenschaftliche Potenzial und Ihren Lebenslauf an Sport-und Aktivit&auml;ten. Es gibt keine W&auml;gung zu Gunsten einer bestimmten Sache. Wir sind daran interessiert, sicherzustellen, dass unsere Sch&uuml;ler, ihr wahres Potential weiterentwickeln - fachlich, sozial, geistig und k&ouml;rperlich. Alle potenziellen Studenten sollten uns auf Tag der offenen T&uuml;r, bevor sie Antragsformulare und ab durch den Eintrag Prozess der Arbeit besuchen. Aufgrund der hohen Anzahl der eingehenden Bewerbungen und die direkte Beteiligung der Fakult&auml;t bei der Zulassung k&ouml;nnen die Sch&uuml;ler aufgefordert, jederzeit zwischen M&auml;rz und Juli zu interviewen.</p> <h3>Ideale Bedingungen f&uuml;r ein Studium</h3> <p>Wenn es mehr als 15 Minuten dauert, bis die Universit&auml;t aus dem Hauptbahnhof, dem zentralen Bus-und Stra&szlig;enbahnhaltestelle zu bekommen, dann ist etwas schief gegangen ist irgendwo. Stra&szlig;enbahnen verlassen den Hauptbahnhof alle f&uuml;nf Minuten und halten direkt in der Mitte des Campus. Die Universit&auml;t Teilnahmegeb&uuml;hr beinhaltet ein ""Semesterticket"", dass die Sch&uuml;ler freie Fahrt gibt im &ouml;ffentlichen Nahverkehr innerhalb Sofia. Und er lebt in Sofia ist fast so billig: Selbst in der reizvollen Innenstadt sind die Mieten erschwinglich. Dar&uuml;ber hinaus gibt es mehr als 2000 Zimmer und Appartements zur Verf&uuml;gung in der Universit&auml;t Wohnheim.</p>", about_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AboutTheUniversityPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateWhyTIUPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), "Why TIU", new Guid(SampleConstants.AboutTheUniversityPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlock whyBlock = new ContentBlock();
                whyBlock.Html = @"<h2>Why TIU?</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), whyBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlock contentBlock = new ContentBlock();
                var students_tiuImageId = SampleUtilities.GetLocalizedImageId("students_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img class=""special"" alt=""Lorem ipsum dolor sit amet"" src=""[images]{0}"" style=""width: 350px; height: 273px; float: left; margin-right: 15px; margin-bottom: 5px;"" longdesc=""Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."" /></p> <h3>Know how to live!</h3> <p> At TIU we believe in teaching you the knowhow of life. In order to lead a good life, one must excel in education, succeed in his profession and enjoy the free time in between. This is why life at TIU encompasses not only academics, but also activities that will shape your character and lead you to success. </p> <h3>Excellence </h3> <p>In order to expect you to excel, we must excel at what we do. We believe in making a real impact on the world by constantly growing, hiring the best staff and investing in the newest technology. You will have access to an impressive range of resources to support your studies including an outstanding academic library and some of the best computing resources. </p> <h3>Careers </h3> <p>Education is the prelude to profession. What use is a diploma unless it makes you highly demanded by the world of employers? TIU&rsquo;s reputation in the business world combined with its contacts and internship programs is a guarantee for your ability to land directly into your dream job right after graduation.</p>", students_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), "Warum TIU", false, "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlock whyBlockDE = new ContentBlock();
                whyBlockDE.Html = @"<h2>Warum TIU?</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), whyBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlock contentBlockDE = new ContentBlock();
                var students_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("students_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Wissen, wie man lebt!</h3> <p> Am TIU wir glauben, in der Lehre Sie das Know-how des Lebens. Um ein gutes Leben zu f&uuml;hren, muss man excel im Bildungswesen, in seinem Beruf erfolgreich zu sein und genie&szlig;en die freie Zeit dazwischen. Dies ist, warum das Leben in TIU nicht nur Akademiker umfasst, sondern auch Aktivit&auml;ten, die Ihren Charakter Form und f&uuml;hren Sie zum Erfolg wird.</p> <h3>Exzellenzinitiative</h3> <p>Um Ihnen zu &uuml;bertreffen erwarten, m&uuml;ssen wir, was wir tun Excel. Wir glauben an einen echten Einfluss auf die Welt durch st&auml;ndig wachsende,, die besten Mitarbeiter und investieren in die neueste Technologie. Sie haben Zugriff auf eine beeindruckende Vielfalt an Ressourcen, um Ihre Studien mit einem hervorragenden akademischen Bibliothek und einige der besten IT-Ressourcen zu unterst&uuml;tzen.</p> <h3>Karriere</h3> <p>Bildung ist der Auftakt zu Beruf. Was n&uuml;tzt ein Diplom, es sei denn es macht dich stark nachgefragten durch die Welt der Arbeitgeber? TIU Ruf in der Gesch&auml;ftswelt mit ihren Kontakten und Praktika in Kombination ist ein Garant f&uuml;r Ihre F&auml;higkeit, direkt Land in Ihren Traumjob nach dem Studium.</p>", students_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.WhyTIUPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateEventsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.EventsPageId), "Events", new Guid(SampleConstants.AboutTheUniversityPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.EventsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                var headerLayoutControl = new LayoutControl();
                List<ColumnDetails> headerLayoutColumns = new List<ColumnDetails>();
                ColumnDetails headerLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 100,
                    PlaceholderId = "Center"
                };
                headerLayoutColumns.Add(headerLayoutColumn1);
                headerLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(headerLayoutColumns, string.Empty);
                headerLayoutControl.ID = "Header";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), headerLayoutControl, "content", "100%", "en");

                ContentBlockBase eventsBlock = new ContentBlockBase();
                eventsBlock.Html = @"<h2>Events</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), eventsBlock, "Header_Center", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);
                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), navigationControl, "Main_Left", "Navigation", "en");

                EventsView eventsControl = new EventsView();
                eventsControl.DetailViewName = SampleConstants.EducationEventItemDetailView;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), eventsControl, "Main_Right", "Events", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.EventsPageId), "Veranstaltungen", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.EventsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                var headerLayoutControl = new LayoutControl();
                List<ColumnDetails> headerLayoutColumns = new List<ColumnDetails>();
                ColumnDetails headerLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 100,
                    PlaceholderId = "Center"
                };
                headerLayoutColumns.Add(headerLayoutColumn1);
                headerLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(headerLayoutColumns, string.Empty);
                headerLayoutControl.ID = "Header";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), headerLayoutControl, "content", "100%", "de");

                ContentBlockBase eventsBlock = new ContentBlockBase();
                eventsBlock.Html = @"<h2>Veranstaltungen</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), eventsBlock, "Header_Center", "Content block", "de");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);
                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), mainLayoutControl, "content", "25% + 75%", "de");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), navigationControl, "Main_Left", "Navigation", "de");

                EventsView eventsControl = new EventsView();
                eventsControl.DetailViewName = SampleConstants.EducationEventItemDetailView;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.EventsPageId), eventsControl, "Main_Right", "Events", "de");
            }
        }

        private void CreateScholarshipsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), "Innovation scholarships", new Guid(SampleConstants.AboutTheUniversityPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase scholarshipsBlock = new ContentBlockBase();
                scholarshipsBlock.Html = @"<h2>TIU Scholarships</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), scholarshipsBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var scholarship_tiuImageId = SampleUtilities.GetLocalizedImageId("scholarship_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Need-based Scholarships &amp; Grants</h3> <p>Federal grant programs include the Federal Pell Grant and the Federal Supplemental Educational Opportunity Grant. The University also offers a number of privately endowed need-based scholarships and grant funds. </p> <h3>Federal Grant Programs for Undergraduate Students </h3> <p><em>The Federal Pell Grant </em>is financial aid based on financial need. It does not have to be repaid. It is federally funded and is considered the foundation of financial aid packages. Pell Grant awards are based on the EFC (expected family contribution), as calculated by the FAFSA formula and after the verification process is completed. </p> <h3>TIU Grants &amp; Scholarships</h3> <p> The University offers a number of privately endowed need-based scholarship and grant funds administered by Student Financial Services. Most of these awards were established by individuals or foundations and are intended for students who meet specific criteria. </p> <p>To qualify for any University endowed or restricted scholarships, students must demonstrate outstanding academic achievement, have financial need, and meet all federal and University eligibility requirements. In most instances, students do not need to complete a separate application form but are considered automatically. </p> <p>Many scholarship awards offered to students are funded by the generous gifts of University benefactors. In addition to financial need and/or academic achievement, a number of scholarships have specific donor requirements that are not easily identifiable. Students awarded a named scholarship will receive written notification of their selection. </p> <h3>State Grants &amp; Scholarships</h3> <p><em>Robert C. Byrd Honors Scholarship:</em> The Byrd Scholarship is awarded to students for their academic achievement and excellence. Recipients may receive up to $1,500 for their first year, and scholarships are renewable for up to three years. To apply for the scholarship, students should contact the Department of Education in their home state. </p>", scholarship_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), "Innovative Stipendien", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase stipendienBlockDE = new ContentBlockBase();
                stipendienBlockDE.Html = @"<h2>TIU Stipendien</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), stipendienBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var scholarship_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("scholarship_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Stipendien &amp; F&ouml;rderungen</h3> <p>Federal F&ouml;rderprogramme umfassen die Federal Pell Grant und der Bundesrepublik Supplemental Bildungschancen Grant. Die Universit&auml;t bietet auch eine Reihe von privat dotiert bedarfsgerechte Stipendien und Zusch&uuml;sse.</p> <h3>Federal Stipendienprogramme f&uuml;r Studenten</h3> <p>Die Federal Pell Grant ist die finanzielle Hilfe auf finanzielle Notwendigkeit basiert. Es muss nicht zur&uuml;ckgezahlt werden. Es wird vom Bund finanziert und gilt als die Grundlage der finanziellen Hilfspakete. Pell Grant Auszeichnungen werden auf der EFC (erwartete Beitrag von Familienmitgliedern), ge&auml;ndert durch die FAFSA Formel berechnet und nach dem Pr&uuml;fverfahren abgeschlossen ist Basis.</p> <h3>TIU Stipendien und Zusch&uuml;sse</h3> <p>Die Universit&auml;t bietet eine Reihe von privat dotiert bedarfsgerechte Stipendien und Zusch&uuml;sse von Student Financial Services verwaltet. Die meisten dieser Auszeichnungen wurden von Privatpersonen oder Stiftungen gegr&uuml;ndet und sind f&uuml;r Studenten, die bestimmte Kriterien erf&uuml;llen soll.</p> <p>Um in den Genuss einer Universit&auml;t verleiht, oder beschr&auml;nkt Stipendien, m&uuml;ssen die Studenten demonstrieren herausragende akademische Leistungen, haben finanzielle Not, und erf&uuml;llen alle Bundes-und Universit&auml;tsbibliothek Anspruchsvoraussetzungen. In den meisten F&auml;llen die Sch&uuml;lerinnen und Sch&uuml;ler nicht unbedingt ein eigenes Antragsformular vollst&auml;ndig, sondern werden automatisch ber&uuml;cksichtigt.</p> <p>Viele Stipendien f&uuml;r Studierende werden durch die gro&szlig;z&uuml;gige Wohlt&auml;ter der Universit&auml;t finanziert. Neben der finanziellen Not und / oder akademischen Leistungen, haben eine Reihe von Stipendien bestimmten Spenders Anforderungen, die nicht leicht zu identifizieren. Die Studierenden erhalten eine benannte Stipendium schriftlichen Mitteilung &uuml;ber die Auswahl zu erhalten.</p> <h3>Staatliche Stipendien und Zusch&uuml;sse</h3> <p>Robert C. Byrd Honors Scholarship: Die Byrd-Stipendium richtet sich an Studierende f&uuml;r ihre akademischen Leistungen und Exzellenz ausgezeichnet. Empf&auml;nger k&ouml;nnen bis zu $ ​​1.500 f&uuml;r das erste Jahr, und Stipendien sind verl&auml;ngerbar bis zu drei Jahren. Um sich f&uuml;r das Stipendium sollen Studierende der Abteilung f&uuml;r Bildung in ihren Heimatstaat wenden.</p>", scholarship_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ScholarshipsPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateNewsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.NewsPageId), "News", new Guid(SampleConstants.AboutTheUniversityPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.NewsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase newsBlock = new ContentBlockBase();
                newsBlock.Html = @"<h2>News</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), newsBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), navigationControl, "Main_Left", "Navigation", "en");

                NewsView latestNews = new NewsView();
                latestNews.MasterViewName = SampleConstants.EducationNewsSummaryContentViewName;
                latestNews.DetailViewName = SampleConstants.EducationNewsItemDetailView;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), latestNews, "Main_Right", "News", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.NewsPageId), "Nachrichten", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.NewsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase newsBlock = new ContentBlockBase();
                newsBlock.Html = @"<h2>Nachrichten</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), newsBlock, "content", "Content block", "de");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), mainLayoutControl, "content", "25% + 75%", "de");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), navigationControl, "Main_Left", "Navigation", "de");

                NewsView latestNews = new NewsView();
                latestNews.MasterViewName = SampleConstants.EducationNewsSummaryContentViewName;
                latestNews.DetailViewName = SampleConstants.EducationNewsItemDetailView;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.NewsPageId), latestNews, "Main_Right", "News", "de");
            }
        }

        private void CreateBlogPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), "Blog", new Guid(SampleConstants.AboutTheUniversityPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase tiuBlogBlock = new ContentBlockBase();
                tiuBlogBlock.Html = @"<h2>TIU Blog</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), tiuBlogBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), navigationControl, "Main_Left", "Navigation", "en");

                var blogsControl = new BlogPostView();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), blogsControl, "Main_Right", "Blog posts", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), "Blog", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase tiuBlogBlockDE = new ContentBlockBase();
                tiuBlogBlockDE.Html = @"<h2>TIU Blog</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), tiuBlogBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), navigationControl, "Main_Left", "Navigation", "de");

                var blogsControl = new BlogPostView();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUBlogPageId), blogsControl, "Main_Right", "Blog posts", "de");
            }
        }

        #endregion

        #region Academics

        private void CreateAcademicsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AcademicsPageId), "Academics", "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase academicsBlock = new ContentBlockBase();
                academicsBlock.Html = @"<h1>Academics</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), academicsBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var academics_tiuImageId = SampleUtilities.GetLocalizedImageId("academics_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Interdisciplinary, practice-oriented, responsible </h3> <p>Striving to become a leading university, TIU invests in academics.&nbsp; We bring together world-renowned faculty and students from all over the region, the nation, and the world.&nbsp; We also count on our bright students to participate in groundbreaking research and contribute their ideas to our overall academic atmosphere.</p> <h3>Information-oriented business sciences&nbsp; </h3> <p>For our Undergraduate and Graduate students we have established stimulating courses in over 150 majors and programs.&nbsp; The courses often incorporate different activities and research projects, because we believe that doing is a major part of learning and we practice what we preach! </p> <h3>Excellent further education opportunities </h3> <p>In order to enlarge our student base and to welcome all of those who are unable to commute to our campus, we have created an innovative and highly efficient base for Distant Learning.&nbsp; Our online students can benefit from an extensive list of courses and contribute to our academic society just as much as the other students.</p>", academics_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AcademicsPageId), "Studieninteressenten", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase academicsBlockDE = new ContentBlockBase();
                academicsBlockDE.Html = @"<h1> </h1> <h1>Studieninteressenten</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), academicsBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var academics_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("academics_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Interdisziplin&auml;re, praxisorientierte, verantwortlich</h3> <p>Das Streben nach einer f&uuml;hrenden Universit&auml;t zu werden, investiert TIU in Akademiker. Wir bringen weltweit renommierten Dozenten und Studenten aus der ganzen Region, der Nation und der Welt. Wir haben auch auf unserer hellen Studenten z&auml;hlen, um in bahnbrechende Forschung zu beteiligen und ihre Ideen f&uuml;r unsere gesamte akademische Atmosph&auml;re.</p> <h3>Informations-orientierten Wirtschaftswissenschaften</h3> <p>F&uuml;r unsere Undergraduate-und Graduate Studenten haben wir anregende Kurse in &uuml;ber 150 Majors und Programme etabliert. Die Kurse beinhalten h&auml;ufig verschiedene Aktivit&auml;ten und Forschungsprojekte, weil wir, dass dabei ein wichtiger Teil des Lernens ist und wir glauben, was wir predigen!</p> <h3>Hervorragende Weiterbildungsm&ouml;glichkeiten</h3> <p>Um unseren Studenten Basis zu erweitern und alle diejenigen, die nicht in der Lage zu pendeln, um unseren Campus sind willkommen, haben wir eine innovative und hocheffiziente Basis f&uuml;r Distant Learning erstellt. Unser Online-Studenten k&ouml;nnen aus einer umfangreichen Liste von Kursen profitieren und dazu beitragen, unseren akademischen Gesellschaft ebenso wie die anderen Sch&uuml;ler.</p>", academics_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicsPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateAthleticsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AthleticsPageId), "Athletics", new Guid(SampleConstants.AcademicsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase athleticsBlock = new ContentBlockBase();
                athleticsBlock.Html = @"<h2>Athletics</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), athleticsBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var sport_tiuImageId = SampleUtilities.GetLocalizedImageId("sport_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Campus Sport </h3> <p>Just across the street from the Admission Office Building lies the Telerik Sports Center, the main building for our athletics and wellness programs on campus. At TSC, you&rsquo;ll find a comprehensive lineup of aquatic, fitness, sports and wellness activities. Most of these services are available at no charge to TIU students and at highly reduced rates for TIU employees, as well as alumni and their family members.Our teams are young and we work pretty hard every season to improve our results. With the help of some generous investors we were able to attract great coaches for our Basketball and American Football teams and we are thrilled to see the results at the end of this season.</p> <h3>Sport in the city</h3> <p>The city of Sofia offers a wide range of possiblities for sports enthousiasts. The most popular sports attraction&nbsp;of the city is the local soccer club ""CSKA Sofia"", which won the national championships 31 times. Those who want to get active by themselves will find a wide range of sport&nbsp; courses at&nbsp; the university's Sports Institute.</p>", sport_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AthleticsPageId), "Uni-Sport", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase athleticsBlockDE = new ContentBlockBase();
                athleticsBlockDE.Html = @"<h2>Uni-Sport</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), athleticsBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var sport_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("sport_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Campus-Sport </h3> <p>Nur &uuml;ber die Stra&szlig;e von der Zulassungsstelle B&uuml;rogeb&auml;ude liegt die Telerik Sports Center, das Hauptgeb&auml;ude f&uuml;r unsere Leichtathletik-und Wellness-Programme auf campus.At TSC, finden Sie eine umfassende Palette von Wasserpflanzen, Fitness-, Sport-und Wellness-Aktivit&auml;ten. Die meisten dieser Dienste sind kostenlos, um TIU Studenten und bei stark reduzierten Preise f&uuml;r TIU Mitarbeiter sowie Alumni und ihrer Familienangeh&ouml;rigen members.Our Teams verf&uuml;gbar sind jung und wir arbeiten ziemlich hart zu jeder Jahreszeit, unsere Ergebnisse zu verbessern. Mit der Hilfe einiger gro&szlig;z&uuml;giger Investoren konnten wir tolle Trainer f&uuml;r unsere Basketball und American Football-Teams zu gewinnen, und wir sind begeistert, dass die Ergebnisse am Ende dieser Saison zu sehen. </p> <h3>Sport in der Stadt</h3> <p>Die Stadt Sofia bietet ein breites Spektrum an M&ouml;glichkeiten f&uuml;r Sport-Liebhaber alter. Die beliebtesten Sportarten Attraktion der Stadt ist die &ouml;rtliche Fu&szlig;ballverein ""CSKA Sofia"", die den nationalen Meisterschaften 31 Mal gewonnen. Diejenigen, die sich von selbst aktiv werden wollen finden Sie eine breite Palette von Sport-Kurse an der Uni-Sport-Institut.</p>", sport_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AthleticsPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateAcademicFacilitiesPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), "Academic facilities", new Guid(SampleConstants.AcademicsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase academicFacilitiesBlock = new ContentBlockBase();
                academicFacilitiesBlock.Html = @"<h2>Academic facilities</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), academicFacilitiesBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var facilities_tiuImageId = SampleUtilities.GetLocalizedImageId("facilities_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Seven faculties</h3> <p>The TUI provides the ideal conditions for completing a degree program successfully and within a reasonable time frame. Ever since its foundation, the university has been committed to reform. Seven faculties have been established, each with long-term strategies for development and using the most up-to-date facilities available. </p> <p>As a student at TIU, you have direct, unfettered access to scientific resources and you can use to help your self-education plan.</p> <br style=""clear: left;""/> <p>Among the most important scientific institutions on campus are:</p> <ul> <li>Data Center</li> <li>Center for Art and Music</li> <li>Languages ​​and Education Center</li> <li>Center for Mathematics and Science</li> <li>Law Library</li> <li>Merriam-Webster Library</li> <li>""Books and Coffee"" Internet Caf&eacute;</li> <li>Brook's theater</li> </ul> <p>For those of you who are involved in sports, whether professional or novice, we have invested in a state-of-the-art sports facility called The Dome. There you will find a fully equipped fitness center, basketball court, indoor pool, indoor and outdoor tennis courts and an outdoor soccer field.</p>", facilities_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), "Einrichtungen", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase academicFacilitiesBlockDE = new ContentBlockBase();
                academicFacilitiesBlockDE.Html = @"<h2>Einrichtungen</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), academicFacilitiesBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var facilities_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("facilities_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Sieben Fakult&auml;ten</h3> <p>Die TUI bietet ideale Voraussetzungen f&uuml;r den Abschluss eines Studiengangs erfolgreich und innerhalb eines angemessenen Zeitrahmens. Seit ihrer Gr&uuml;ndung hat die Universit&auml;t begangen zu reformieren. Sieben Fakult&auml;ten wurden eingerichtet, die jeweils mit langfristigen Strategien f&uuml;r die Entwicklung und Nutzung der meisten up-to-date Einrichtungen zur Verf&uuml;gung.&nbsp;</p> <p>Als Student an TIU haben Sie direkten, freien Zugang zu wissenschaftlichen Ressourcen und k&ouml;nnen Sie benutzen, um Ihre Selbst-Bildung zu planen.</p> <br style=""clear: left;""/> <p>Zu den wichtigsten wissenschaftlichen Einrichtungen auf dem Campus sind:</p> <ul> <li>Data Center</li> <li>Zentrum f&uuml;r Kunst und Musik</li> <li>Sprachen-und Bildungszentrum</li> <li>Center for Mathematics and Science</li> <li> Law Library</li> <li>Merriam-Webster-Bibliothek</li> <li>""Books and Coffee"" Internet Caf&eacute;</li> <li>Brook-Theater</li> </ul> <p>F&uuml;r diejenigen unter Ihnen, die im Sport involviert sind, ob Profi oder Anf&auml;nger, wir haben in einer state-of-the-art Sportanlage investiert namens The Dome. Dort finden Sie ein voll ausgestattetes Fitness-Center, Basketballplatz, Hallenbad, Indoor-und Outdoor-Tennispl&auml;tze und ein Outdoor-Fu&szlig;ballfeld.</p>", facilities_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AcademicFacilitiesPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateDistantLearningPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), "Distant learning", new Guid(SampleConstants.AcademicsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase distantLearningBlock = new ContentBlockBase();
                distantLearningBlock.Html = @"<h2>Distant Learning</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), distantLearningBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var distant_tiuImageId = SampleUtilities.GetLocalizedImageId("distant_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Cultural Studies and Educational Science</h3> <p>We believe in education and we really think that education should be accessible for everyone. This is why we started our Distant Learning program back in 2007 according to the latest standards and using the newest technologies in the telecommunications to make education affordable for people anywhere in the world.</p> <h3>Ideal conditions for studying natural sciences</h3> <p>We already support Web-based VoIP, Videoconferencing, Web conferencing, Internet radio and Live streaming. This year we invested $2.6 million in creating a much bigger asynchronous technology base to support people, who cannot use the latest technologies in their region. We transferred 94% of our materials on Audiocassettes, VHS, CD and DVD, and we are almost finished with a massive print materials section that can be ordered very easy per Mail or sent to a local copy shop and printed there!</p> <h3>Integrative environmental research</h3> <p>Our online education program offers both Undergraduate and Graduate programs for those students who are not able to joins us on campus.</p>", distant_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), "Fernstudium", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase distantLearningBlockDE = new ContentBlockBase();
                distantLearningBlockDE.Html = @"<h2>Fernstudium</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), distantLearningBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var distant_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("distant_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" src=""[images]{0}"" /></p> <h3>Cultural Studies und P&auml;dagogik&nbsp;</h3> <p>Wir glauben an Bildung und wir wirklich denken, dass Bildung f&uuml;r jeden zug&auml;nglich sein. Deshalb haben wir unsere Distant Learning-Programm begann im Jahr 2007 nach den neuesten Standards und mit den neuesten Technologien in der Telekommunikation, Bildung und Ausbildung erschwinglich f&uuml;r Menschen &uuml;berall in der Welt.</p> <h3>Ideale Bedingungen f&uuml;r Studium der Naturwissenschaften&nbsp;</h3> <p>Wir unterst&uuml;tzen bereits Web-basierte VoIP, Videoconferencing, Web-Conferencing, Internet Radio und Live-Streaming. Dieses Jahr haben wir investiert $ 2.600.000 bei der Schaffung eines viel gr&ouml;&szlig;eren asynchrone technologische Basis, um Menschen, die nicht die neuesten Technologien in ihrer Region unterst&uuml;tzen k&ouml;nnen. Wir &uuml;bertragen 94% unserer Materialien auf Audiokassetten, VHS, CD und DVD, und wir sind fast mit einem massiven Druckmaterialien Abschnitt, der bestellt einfach kann per Mail oder an einen Copyshop und dort gedruckt fertig!</p> <h3>Integrative Umweltforschung&nbsp;</h3> <p>Unser Online-Bildungsprogramm bietet sowohl Undergraduate-und Graduate-Programme f&uuml;r diejenigen Studierenden, die nicht imstande sind, gesellt sich zu uns auf dem Campus.</p>", distant_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DistantLearningPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateUndergraduateAndGraduate()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), "Undergraduate and Graduate", new Guid(SampleConstants.AcademicsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase graduateBlock = new ContentBlockBase();
                graduateBlock.Html = @"<h2>Undergraduate and Graduate</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), graduateBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                contentBlock.Html = @"Undergraduates and graduates at TIU have the opportunity to choose from a wide range of courses and to comprise their schedules in order to obtain their Majors and Minors in the different schools at TIU.";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), contentBlock, "Main_Right", "Content block", "en");

                var innerLayoutControl = new LayoutControl();
                List<ColumnDetails> innerLayoutColumns = new List<ColumnDetails>();
                ColumnDetails innerLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 50,
                    PlaceholderId = "Left"
                };
                innerLayoutColumns.Add(innerLayoutColumn1);
                ColumnDetails innerLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 50,
                    PlaceholderId = "Right"
                };
                innerLayoutColumns.Add(innerLayoutColumn2);

                innerLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(innerLayoutColumns, string.Empty);
                innerLayoutControl.ID = "Inner";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), innerLayoutControl, "Main_Right", "50% + 50%", "en");

                ContentBlockBase leftBlock = new ContentBlockBase();

                leftBlock.Html = @"<h3>Graduate </h3> <ul> <li>Art History</li> <li>Asian Studies</li> <li>Astronomy</li> <li>Biochemistry and Molecular Biology</li> <li>Biology</li> <li>Chemistry</li> <li>Chinese</li> <li>Classics</li> <li>Computer Science</li> <li>Economics</li> <li>English</li> <li>Entrepreneurship</li> </ul>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), leftBlock, "Inner_Left", "Content block", "en");

                ContentBlockBase rightBlock = new ContentBlockBase();
                rightBlock.Html = @"<h3>Under graduate</h3> <ul> <li>Geography</li> <li>German</li> <li>Greek</li> <li>History</li> <li>Japanese</li> <li>Law and Society</li> <li>Mathematics</li> <li>Management</li> <li>Music</li> <li>Philosophy</li> <li>Physics</li> <li>Psychology</li> <li>Sociology</li> <li>Spanish</li> <li>Theater Arts</li> <li>Urban Development and Social Change</li> </ul>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), rightBlock, "Inner_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), "Bachelor und Master", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase graduateBlockDE = new ContentBlockBase();
                graduateBlockDE.Html = @"<h2>Bachelor und Master</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), graduateBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                contentBlockDE.Html = @"<p>Studenten und Absolventen bei TIU haben die M&ouml;glichkeit, aus einer breiten Palette von Kursen zu w&auml;hlen und ihre Zeitpl&auml;ne umfassen, um ihre Haupt und Nebenf&auml;cher in den verschiedenen Schulen in TIU erhalten.</p>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), contentBlockDE, "Main_Right", "Content block", "de");

                var innerLayoutControlDE = new LayoutControl();
                List<ColumnDetails> innerLayoutColumns = new List<ColumnDetails>();
                ColumnDetails innerLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 50,
                    PlaceholderId = "Left"
                };
                innerLayoutColumns.Add(innerLayoutColumn1);
                ColumnDetails innerLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 50,
                    PlaceholderId = "Right"
                };
                innerLayoutColumns.Add(innerLayoutColumn2);

                innerLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(innerLayoutColumns, string.Empty);
                innerLayoutControlDE.ID = "Inner";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), innerLayoutControlDE, "Main_Right", "50% + 50%", "de");

                ContentBlockBase leftBlockDE = new ContentBlockBase();
                leftBlockDE.Html = @"<h3>Masterstudieng&auml;nge</h3> <ul> <li>Kunstgeschichte</li> <li>Asiatische Lehren</li> <li>Astronomie</li> <li>Biochemie</li> <li>Biologie</li> <li>Chemie</li> <li>Chinesisch</li> <li>Klassische Lehren </li> <li>Informatik</li> <li>Wirtschaft</li> <li>Englisch </li> <li>Entrepreneurship</li> </ul>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), leftBlockDE, "Inner_Left", "Content block", "de");

                ContentBlockBase rightBlockDE = new ContentBlockBase();
                rightBlockDE.Html = @"<h3>Bachelorstudieng&auml;nge</h3> <ul> <li>Geographie</li> <li>Deutsch </li> <li>Griechisch</li> <li>Geschichte </li> <li>Japanisch</li> <li>Jura</li> <li>Mathematik</li> <li>Management</li> <li>Musik</li> <li>Philosophie</li> <li>Physik</li> <li>Psychologie</li> <li>Sociologie</li> <li>Spanisch</li> <li>Kunst</li> </ul>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.UndergraduateAndGraduatePageId), rightBlockDE, "Inner_Right", "Content block", "de");
            }
        }

        #endregion

        #region Admissions

        private void CreateAdmissionsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), "Admissions", "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase admissionsBlock = new ContentBlockBase();
                admissionsBlock.Html = @"<h1>Admissions</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), admissionsBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var admission_tiuImageId = SampleUtilities.GetLocalizedImageId("admission_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Applying</h3> <p>TIU believes that diversity is the key to constant growth and experiencing life.&nbsp; We encourage students from all over the world, from different backgrounds, ethnicities, cultures and religions to apply and to become part of our academic family, thus contributing to each other&rsquo;s cultural enrichment.&nbsp; We travel to many different parts of the world in order to meet with potential students and to learn about ways in which to make the admissions process easier and more diversity friendly.&nbsp; </p> <p>Admission to TIU, however, is also highly dependent on your previous academic achievements and merits.&nbsp; In order to keep our reputation of a school for smart and ambitious young men and women, we are selective and inspective of your past education.&nbsp; We take into consideration not only your grades but also your personal goals, achievement and your ability to overcome difficulties and shine!</p>", admission_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), "Zulassung", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase admissionsBlockDE = new ContentBlockBase();
                admissionsBlockDE.Html = @"<h1>Zulassung </h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), admissionsBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var admission_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("admission_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>Bewerben </h3> <p>TIU glaubt, dass die Vielfalt ist der Schl&uuml;ssel zu stetigem Wachstum und das Leben erfahren. Wir ermutigen die Sch&uuml;ler aus der ganzen Welt, von unterschiedlicher Herkunft, Ethnien, Kulturen und Religionen zu &uuml;bernehmen und einen Teil unserer akademischen Familie zu werden, damit einen Beitrag zur gegenseitigen kulturellen Bereicherung. Wir fahren zu vielen verschiedenen Teilen der Welt, um mit potenziellen Studenten zu treffen und sich &uuml;ber M&ouml;glichkeiten, um die Zulassungsverfahren vereinfachen und mehr Vielfalt freundlich zu lernen.</p> <p>Zulassung zum TIU, ist aber auch stark abh&auml;ngig von Ihrer bisherigen wissenschaftlichen Leistungen und Verdienste. Um unseren Ruf einer Schule f&uuml;r intelligente und ambitionierte junge M&auml;nner und Frauen zu halten, werden wir selektiv und inspective Ihrer Vergangenheit Bildung. Wir ber&uuml;cksichtigen nicht nur Ihre Noten, sondern auch Ihre pers&ouml;nlichen Ziele, Leistungen und Ihre F&auml;higkeit, Schwierigkeiten zu &uuml;berwinden und Glanz!</p>", admission_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.AdmissionsPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateHowToApplyPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), "How to apply", new Guid(SampleConstants.AdmissionsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase howToApplyBlock = new ContentBlockBase();
                howToApplyBlock.Html = @"<h2>How to apply?</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), howToApplyBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var howto_tiuImageId = SampleUtilities.GetLocalizedImageId("howto_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <h3>No assessment tests for University Entrance</h3> <p>TIU is a modern, fast-moving environment and our energetic staff does not like to waste time with endless bureaucratic ordeals.&nbsp; This is why our application process is very simple, accurate and rational.&nbsp; There are three easy steps you need to follow in order to let yourself known to us and we will do the rest.&nbsp; All due dates and deadlines for the upcoming semester will be posted and updated on our website.</p> <br /> <br style=""clear: left"" /> <ul> <li>First &ndash; fill out the online Application Form in its entirety and send it to us with the click of a button. You will receive an e-mail from us, confirming that we have received your application.</li> <li>Second &ndash; you have 14 calendar days from the date of the application confirmation e-mail to send us all the original documents listed in the Application Form. You will receive an e-mail from us, confirming that we have received your documents.</li> <li>Third &ndash; we will contact you within 14 calendar days from the date of the document confirmation e-mail in order to let you know our decision. You will receive an e-mail, a phone call and a letter with our decision.&nbsp; </li> </ul>", howto_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), "Bewerbung", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase howToApplyBlockDE = new ContentBlockBase();
                howToApplyBlockDE.Html = @"<h2>Bewerbung</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), howToApplyBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var howto_tiuImageId = SampleUtilities.GetLocalizedImageId("howto_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <p>TIU ist ein modernes, sich schnell bewegende Umwelt und unsere energetische Personal nicht gerne Zeit mit endlosen b&uuml;rokratischen Pr&uuml;fungen Abfall. Deshalb ist unsere Anwendung ist sehr einfach, genau und rational. Es gibt drei einfache Schritte, die Sie folgen m&uuml;ssen, um lassen Sie uns bekannt und wir werden den Rest erledigen. Alle f&auml;lligen Termine und Fristen f&uuml;r das kommende Semester wird ver&ouml;ffentlicht und aktualisiert werden auf unserer Website.</p> <br /> <br style=""clear: left"" /> <ul> <li> First - f&uuml;llen Sie das Online-Bewerbungsformular in seiner Gesamtheit und schicken Sie es uns mit dem Klick auf eine Schaltfl&auml;che. Sie erhalten eine E-Mail von uns erhalten, die best&auml;tigt, dass wir Ihre Anmeldung erhalten haben. </li> <li>Zweitens - Sie haben 14 Kalendertagen ab dem Datum der Antragstellung per E-Mail an uns alle Originaldokumente im Antragsformular aufgef&uuml;hrt. Sie erhalten eine E-Mail von uns erhalten, die best&auml;tigt, dass wir Ihre Unterlagen erhalten. </li> <li>Drittens - wir werden Sie innerhalb von 14 Kalendertagen Kontakt ab dem Datum des Dokuments per E-Mail, um Ihnen mitzuteilen, unsere Entscheidung. Sie erhalten eine E-Mail, ein Anruf und einen Brief mit unserer Entscheidung.</li> </ul>", howto_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HowToApplyPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateDiversityStatisticsPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), "Diversity statistics", new Guid(SampleConstants.AdmissionsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase diversityStatisticsBlock = new ContentBlockBase();
                diversityStatisticsBlock.Html = @"<h2>Diversity Statistics</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), diversityStatisticsBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var diversityImageId = SampleUtilities.GetLocalizedImageId("diversity_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" src=""[images]{0}"" /></p> <p>TIU is the Benetton of universities.&nbsp; Our colors are many and bright.&nbsp; Our diversity statistics show that we the educational institution of choice for students of 57 different nationalities.&nbsp; Here are our most recent stats.</p>", diversityImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), "Multikulturelle Vielfalt", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase diversityStatisticsBlockDE = new ContentBlockBase();
                diversityStatisticsBlockDE.Html = @"<h2>Multikulturelle Vielfalt</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), diversityStatisticsBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var diversityImageIdDE = SampleUtilities.GetLocalizedImageId("diversity_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" src=""[images]{0}"" /></p> TIU ist die Benetton von Universit&auml;ten. Unsere Farben sind vielf&auml;ltig und hell ist. Unsere Vielfalt Statistiken zeigen, dass wir die Bildungseinrichtung der Wahl f&uuml;r Studenten von 57 verschiedenen Nationalit&auml;ten. Hier sind unsere j&uuml;ngsten Statistik.", diversityImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DiversityStatisticsPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreatePreArrivalInformationPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), "Pre-arrival information", new Guid(SampleConstants.AdmissionsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase titleBlock = new ContentBlockBase();
                titleBlock.Html = @"<h2>Pre-arrival Information</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), titleBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlock contentBlock = new ContentBlock();
                var prearrivalImageId = SampleUtilities.GetLocalizedImageId("prearrival", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{3}"" /></p> <h3>Pre-arrival</h3> <p>College is a step towards the beginning of your new life.&nbsp; We will try to make getting there as easy as possible for you.&nbsp; The information in this section of our website is intended to be used before you arrive on campus. Once you are on campus we will be here to meet and greet you and to make your experience here unforgettable and beneficial.&nbsp; If you have a specific question that is not answered here please feel free to contact us. We are very pleased that you are interested in studying at the Telerik International University.</p> <br /> <br style=""clear: left"" /> <p>Here are some helpful links on different topics:</p> <ul> <li><a href=""[pages]{0}"" >Contact us</a></li> <li><a href=""[pages]{1}"" >Housing opportunities</a></li> <li><a href=""[pages]{2}"" >Documents base</a></li> </ul>", SampleConstants.ContactUsPageId, SampleConstants.HousingOpportunitiesPageId, SampleConstants.DocumentsBasePageId, prearrivalImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), "Vor der Ankunft", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlock titleBlockDE = new ContentBlock();
                titleBlockDE.Html = @"<h2>Vor der Ankunft</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), titleBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlock contentBlockDE = new ContentBlock();
                var prearrivalImageIdDE = SampleUtilities.GetLocalizedImageId("prearrival", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{3}"" /></p> <h3>Vor der Ankunft</h3> <p>College ist ein Schritt in Richtung der Beginn Ihres neuen Lebens. Wir werden versuchen, dorthin zu gelangen so einfach wie m&ouml;glich f&uuml;r Sie. Die Informationen in diesem Bereich unserer Website soll verwendet werden, bevor Sie auf dem Campus zu gelangen. Sobald Sie sich auf dem Campus werden wir hier zu treffen und gr&uuml;&szlig;en Sie und machen Ihre Erfahrungen hier unvergessliche und n&uuml;tzlich. Wenn Sie eine spezifische Frage, die hier nicht beantwortet haben, z&ouml;gern Sie nicht uns zu kontaktieren.</p> <br /> <br style=""clear: left"" /> <p>Hier sind einige hilfreiche Links zu verschiedenen Themen:</p> <ul> <li><a href=""[pages]{0}"" >Kontakt</a></li> <li><a href=""[pages]{1}"" >Wohnmöglichkeiten</a></li> <li><a href=""[pages]{2}"" >Downloads</a></li> </ul>", SampleConstants.ContactUsPageId, SampleConstants.HousingOpportunitiesPageId, SampleConstants.DocumentsBasePageId, prearrivalImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.PreArrivalInformationPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateDocumentsBasePage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), "Document base", new Guid(SampleConstants.AdmissionsPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase documentsBaseBlock = new ContentBlockBase();
                documentsBaseBlock.Html = @"<h2>Document base</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), documentsBaseBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), navigationControl, "Main_Left", "Navigation", "en");

                DownloadListView downloads = new DownloadListView();
                downloads.MasterViewName = "MasterTableView";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), downloads, "Main_Right", "Download list", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), "Downloads", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase documentsBaseBlockDE = new ContentBlockBase();
                documentsBaseBlockDE.Html = @"<h2>Downloads</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), documentsBaseBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), navigationControlDE, "Main_Left", "Navigation", "de");

                DownloadListView downloadsDE = new DownloadListView();
                downloadsDE.MasterViewName = "MasterTableView";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.DocumentsBasePageId), downloadsDE, "Main_Right", "Download list", "de");
            }
        }

        #endregion

        #region Campus life

        private void CreateCampusLifePage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.CampusLifePageId), "Campus life", "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase campusLifeBlock = new ContentBlockBase();
                campusLifeBlock.Html = @"<h1>Campus life</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), campusLifeBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var campuslife_tiuImageId = SampleUtilities.GetLocalizedImageId("campuslife_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" src=""[images]{0}"" /></p> <h3>Ideal conditions for studying</h3> <p>Campus life is all about comfort, fun and interaction.&nbsp; We have invested in creating a &ldquo;small town&rdquo; atmosphere, where on the one hand you can find everything you need in order to live comfortably and on the other &ndash; you can easily get to know everyone around.&nbsp; </p> <p>There are many facilities, besides the educational ones, that our students can use for their everyday needs, including gyms, theater halls, shops and coffee shops, dining halls, recreation rooms, media halls and even child care facilities. </p> <p>We also very proud of our Activities Center where students can sign up for different activities, sports and hobbies or even start up their own recreational class and promote it across campus.</p>", campuslife_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.CampusLifePageId), "Leben auf dem Campus", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase campusLifeBlockDE = new ContentBlockBase();
                campusLifeBlockDE.Html = @"<h1>Leben auf dem Campus</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), campusLifeBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageChildren;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var campuslife_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("campuslife_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" src=""[images]{0}"" /></p> <p>Campus Leben dreht sich alles um Komfort, Spa&szlig; und Interaktion. Sie k&ouml;nnen leicht f&uuml;r jedermann rund wissen - Wir haben bei der Schaffung einer ""Kleinstadt"" Atmosph&auml;re, wo auf der einen Seite alles, was Sie brauchen, um bequem zu leben und auf der anderen Seite finden investiert.</p> <p>Es gibt viele Einrichtungen, neben den p&auml;dagogischen sind, dass unsere Sch&uuml;ler f&uuml;r ihre t&auml;glichen Bed&uuml;rfnisse, einschlie&szlig;lich Turnhallen, Theatern, Gesch&auml;ften und Caf&eacute;s, Kantinen, Aufenthaltsr&auml;ume, Medien Hallen und Einrichtungen zur Kinderbetreuung zu verwenden.</p> <p>Wir haben auch sehr stolz auf unsere Activities Center, wo die Studenten anmelden k&ouml;nnen f&uuml;r verschiedene Aktivit&auml;ten, Sport und Hobbys oder sogar anfangen, ihre eigenen Erholungs-Klasse und f&ouml;rdern es &uuml;ber den Campus.</p>", campuslife_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusLifePageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateCampusRulesPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), "Campus rules", new Guid(SampleConstants.CampusLifePageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase campusRulesBlock = new ContentBlockBase();
                campusRulesBlock.Html = @"<h2>Campus rules</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), campusRulesBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                contentBlock.Html = @"<p>We are a fun place to be, but no one said that there will be no rules!&nbsp; Rules are what make us strong and durable, so we believe that in order to be the best at what we do, we must not only follow a few general rules, but also create our own.&nbsp; You can download our Campus Rules and Regulations brochure from here &ndash; it is an easy and fun read!</p>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), "Campus-Regeln", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase campusRulesBlockDE = new ContentBlockBase();
                campusRulesBlockDE.Html = @"<h2>Campus-Regeln</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), campusRulesBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                contentBlockDE.Html = @"<p>Wir sind ein angenehmer Ort zu sein, aber niemand gesagt, dass es keine Regeln! Regeln sind das, was uns stark und langlebig, so glauben wir, dass, um die am besten, was wir tun werden, m&uuml;ssen wir nicht nur folgen ein paar allgemeine Regeln, sondern auch unsere eigene. Sie k&ouml;nnen unsere Campus Regeln und Vorschriften Brosch&uuml;re hier herunterladen - es ist ein einfach und macht Spa&szlig; zu lesen!</p>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusRulesPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        private void CreateCampusMapPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.CampusMapPageId), "Campus map", new Guid(SampleConstants.CampusLifePageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase campusMapBlock = new ContentBlockBase();
                campusMapBlock.Html = @"<h2>Campus map</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), campusMapBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), navigationControl, "Main_Left", "Navigation", "en");

                ImageControl campusMapImage = new ImageControl();
                campusMapImage.ImageId = SampleUtilities.GetLocalizedImageId("campus", "en");
                campusMapImage.Width = 676;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), campusMapImage, "Main_Right", "Image", "en");

                var legendHeaderLayoutControl = new LayoutControl();
                List<ColumnDetails> legendHeaderLayoutColumns = new List<ColumnDetails>();
                ColumnDetails legendHeaderLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 100,
                    PlaceholderId = "Center"
                };
                legendHeaderLayoutColumns.Add(legendHeaderLayoutColumn1);

                legendHeaderLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(legendHeaderLayoutColumns, string.Empty);
                legendHeaderLayoutControl.ID = "LegendHeader";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), legendHeaderLayoutControl, "Main_Right", "100%", "en");

                ContentBlock legendBlock = new ContentBlock();
                legendBlock.Html = @"<h3>Legend</h3>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), legendBlock, "LegendHeader_Center", "Content block", "en");

                var legendLayoutControl = new LayoutControl();
                List<ColumnDetails> legendLayoutColumns = new List<ColumnDetails>();
                ColumnDetails legendLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 33,
                    PlaceholderId = "Left33"
                };
                legendLayoutColumns.Add(legendLayoutColumn1);
                ColumnDetails legendLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 34,
                    PlaceholderId = "Middle34"
                };
                legendLayoutColumns.Add(legendLayoutColumn2);
                ColumnDetails legendLayoutColumn3 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 33,
                    PlaceholderId = "Right33"
                };
                legendLayoutColumns.Add(legendLayoutColumn3);

                legendLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(legendLayoutColumns, string.Empty);
                legendLayoutControl.ID = "Legend";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), legendLayoutControl, "Main_Right", "33% + 34% + 33%", "en");

                ContentBlockBase contentBlockLeft = new ContentBlockBase();
                contentBlockLeft.Html = @"1. Dorm A (Ryan Mcguire)<br /> 2. Fine Arts building<br /> 3. Computing center<br /> 4. Dorm B (Tomas Newman)";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), contentBlockLeft, "Legend_Left33", "Content block", "en");

                ContentBlockBase contentBlockMiddle = new ContentBlockBase();
                contentBlockMiddle.Html = @"5. Biology faculty<br /> 6. Chemistry building<br /> 7. Dorm C (Micheal Burgess)<br /> 8. Administration center";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), contentBlockMiddle, "Legend_Middle34", "Content block", "en");

                ContentBlockBase contentBlockRight = new ContentBlockBase();
                contentBlockRight.Html = @"9. Athletics facilities<br /> 10.Main Building<br /> 11.Liberal arts building<br /> 12. Dorm D (Melvin Richards)";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), contentBlockRight, "Legend_Right33", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.CampusMapPageId), "Campus-Karte", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase campusMapBlockDE = new ContentBlockBase();
                campusMapBlockDE.Html = @"<h2>Campus-Karte</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), campusMapBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ImageControl campusMapImageDE = new ImageControl();
                campusMapImageDE.ImageId = SampleUtilities.GetLocalizedImageId("campus", "de");
                campusMapImageDE.Width = 676;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), campusMapImageDE, "Main_Right", "Image", "de");

                var legendHeaderLayoutControlDE = new LayoutControl();
                List<ColumnDetails> legendHeaderLayoutColumns = new List<ColumnDetails>();
                ColumnDetails legendHeaderLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 100,
                    PlaceholderId = "Center"
                };
                legendHeaderLayoutColumns.Add(legendHeaderLayoutColumn1);

                legendHeaderLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(legendHeaderLayoutColumns, string.Empty);
                legendHeaderLayoutControlDE.ID = "LegendHeader";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), legendHeaderLayoutControlDE, "Main_Right", "100%", "de");

                ContentBlock legendBlockDE = new ContentBlock();
                legendBlockDE.Html = @"<h3>Legend</h3>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), legendBlockDE, "LegendHeader_Center", "Content block", "de");

                var legendLayoutControlDE = new LayoutControl();
                List<ColumnDetails> legendLayoutColumns = new List<ColumnDetails>();
                ColumnDetails legendLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 33,
                    PlaceholderId = "Left33"
                };
                legendLayoutColumns.Add(legendLayoutColumn1);
                ColumnDetails legendLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 34,
                    PlaceholderId = "Middle34"
                };
                legendLayoutColumns.Add(legendLayoutColumn2);
                ColumnDetails legendLayoutColumn3 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 33,
                    PlaceholderId = "Right33"
                };
                legendLayoutColumns.Add(legendLayoutColumn3);

                legendLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(legendLayoutColumns, string.Empty);
                legendLayoutControlDE.ID = "Legend";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), legendLayoutControlDE, "Main_Right", "33% + 34% + 33%", "de");

                ContentBlockBase contentBlockLeftDE = new ContentBlockBase();
                contentBlockLeftDE.Html = @"1. Dorm A (Ryan Mcguire)<br /> 2. Fine Arts building<br /> 3. Computing center<br /> 4. Dorm B (Tomas Newman)";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), contentBlockLeftDE, "Legend_Left33", "Content block", "de");

                ContentBlockBase contentBlockMiddleDE = new ContentBlockBase();
                contentBlockMiddleDE.Html = @"5. Biology faculty<br /> 6. Chemistry building<br /> 7. Dorm C (Micheal Burgess)<br /> 8. Administration center";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), contentBlockMiddleDE, "Legend_Middle34", "Content block", "de");

                ContentBlockBase contentBlockRightDE = new ContentBlockBase();
                contentBlockRightDE.Html = @"9. Athletics facilities<br /> 10.Main Building<br /> 11.Liberal arts building<br /> 12. Dorm D (Melvin Richards)";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.CampusMapPageId), contentBlockRightDE, "Legend_Right33", "Content block", "de");
            }
        }

        private void CreateHousingOpportunitiesPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), "Housing opportunities", new Guid(SampleConstants.CampusLifePageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase housingOpportunitiesBlock = new ContentBlockBase();
                housingOpportunitiesBlock.Html = @"<h2>Housing Opportunities</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), housingOpportunitiesBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), mainLayoutControl, "content", "25% + 75%", "en");

                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.VerticalSimple;
                navigationControl.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControl.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), navigationControl, "Main_Left", "Navigation", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                var housing_tiuImageId = SampleUtilities.GetLocalizedImageId("housing_tiu", "en");

                contentBlock.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <p>Telerik International University offers an American-style liberal arts education with accommodation available in the surrounding area. The following accommodation is currently available to students.</p>", housing_tiuImageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), contentBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), "Wohnmöglichkeiten", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase housingOpportunitiesBlockDE = new ContentBlockBase();
                housingOpportunitiesBlockDE.Html = @"<h2>Wohnmöglichkeiten</h2>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), housingOpportunitiesBlockDE, "content", "Content block", "de");

                var mainLayoutControlDE = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 25,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 75,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControlDE.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControlDE.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), mainLayoutControlDE, "content", "25% + 75%", "de");

                var navigationControlDE = new NavigationControl();
                navigationControlDE.NavigationMode = NavigationModes.VerticalSimple;
                navigationControlDE.SelectionMode = PageSelectionModes.CurrentPageSiblings;
                navigationControlDE.Skin = "left";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), navigationControlDE, "Main_Left", "Navigation", "de");

                ContentBlockBase contentBlockDE = new ContentBlockBase();
                var housing_tiuImageIdDE = SampleUtilities.GetLocalizedImageId("housing_tiu", "de");

                contentBlockDE.Html = string.Format(@"<p><img alt="""" style=""float: left; margin-right: 15px; margin-bottom: 5px;"" src=""[images]{0}"" /></p> <p>Telerik International University bietet eine American-style geisteswissenschaftliche Ausbildung mit &Uuml;bernachtungsm&ouml;glichkeiten in der Umgebung. Die folgende Unterkunft ist derzeit f&uuml;r Studierende.</p>", housing_tiuImageIdDE);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.HousingOpportunitiesPageId), contentBlockDE, "Main_Right", "Content block", "de");
            }
        }

        #endregion

        #region Shop

        private void CreateEcommerceTax()
        {
            string defaultTaxTitle = "Texas State Sales Tax";

            var tax = this.OrdersMan.GetTaxes().Where(t => t.Title == defaultTaxTitle).FirstOrDefault();
            if (tax == null)
            {
                var newTax = this.OrdersMan.CreateTax();
                newTax.Title = defaultTaxTitle;
                newTax.ApplyTaxToShipping = true;
                newTax.StandardTaxRate = 6.25m;
                newTax.Country = "United States";
                newTax.State = "Texas";
                this.OrdersMan.SaveChanges();
            }
        }

        private void CreateEcommercePaymentMethod()
        {
            string title = SampleConstants.PaymentMethodName;
            this.OrdersMan.Provider.SuppressSecurityChecks = true;

            var method = this.OrdersMan.GetPaymentMethods().Where(m => m.Title == title).FirstOrDefault();
            if (method == null)
            {
                PaymentMethod paymentMethod = this.OrdersMan.CreatePaymentMethod();
                paymentMethod.Title = title;
                paymentMethod.PaymentMethodType = PaymentMethodType.Offline;
                paymentMethod.IsActive = true;

                this.OrdersMan.SaveChanges();
                this.OrdersMan.Provider.SuppressSecurityChecks = false;
            }
        }

        private void CreateEcommerceShippingMethod()
        {
            string title = SampleConstants.ShippingMethodUSAName;
            decimal amount = 10m;

            ShippingManager shippingManager = ShippingManager.GetManager();
            shippingManager.Provider.SuppressSecurityChecks = true;

            var method = shippingManager.GetShippingMethods().Where(m => m.Title == title).FirstOrDefault();
            if (method == null)
            {
                ShippingMethod shippingMethod = shippingManager.CreateShippingMethod();
                shippingMethod.ShippingPrice = String.Format("fixedPrice|||{0}", amount);
                shippingMethod.Area = @"{""Area"":""1"",""Countries"":""""}";
                shippingMethod.Name = title.ToLower();
                shippingMethod.Title = title;
                shippingMethod.IsActive = true;

                shippingManager.SaveChanges();
                shippingManager.Provider.SuppressSecurityChecks = false;
            }

            title = SampleConstants.ShippingMethodEuropeName;
            amount = 60m;

            shippingManager.Provider.SuppressSecurityChecks = true;
            method = shippingManager.GetShippingMethods().Where(m => m.Title == title).FirstOrDefault();
            if (method == null)
            {
                ShippingMethod shippingMethod = shippingManager.CreateShippingMethod();
                shippingMethod.ShippingPrice = String.Format("fixedPrice|||{0}", amount);
                shippingMethod.Area = @"{""Area"":""2"",""Countries"":""""}";
                shippingMethod.Name = title.ToLower();
                shippingMethod.Title = title;
                shippingMethod.IsActive = true;

                shippingManager.SaveChanges();
                shippingManager.Provider.SuppressSecurityChecks = false;
            }
        }

        private void SetDefaultSMTPSettings()
        {
            var config = this.ConfigMan.GetSection<SystemConfig>();

            config.SmtpSettings.Host = "127.0.0.1";
            config.SmtpSettings.Port = 25;
            config.SmtpSettings.UserName = "smtp_username";
            config.SmtpSettings.Password = "smtp_password";
            config.SmtpSettings.DefaultSenderEmailAddress = "tuishop@telerikuniversity.com";

            this.ConfigMan.SaveSection(config);
        }

        private void SetEcommerceMerchantEmail()
        {

            var config = this.ConfigMan.GetSection<EcommerceConfig>();
            config.MerchantEmail = "tiu@telerik.com";
            this.ConfigMan.SaveSection(config);
        }

        private void CreateShoppingCartPage()
        {
            Guid parentPageID = new Guid(SampleConstants.ShopBasePageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ShopShoppingCartPageId), "Shopping cart", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ShopShoppingCartPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ShoppingCart shoppingCart = new ShoppingCart();

                shoppingCart.ContinueShoppingPageId = new Guid(SampleConstants.ShopBasePageId);
                shoppingCart.ContinueShoppingUrl = "~/tiu-shop";

                shoppingCart.CheckoutPageId = new Guid(SampleConstants.ShopCheckoutPageId);
                shoppingCart.CheckoutUrl = "~/tiu-shop/checkout";

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopShoppingCartPageId), shoppingCart, "content", "Shopping cart", "en");
            }
        }

        private void CreateInvoicePage()
        {
            Guid parentPageID = new Guid(SampleConstants.ShopBasePageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ShopInvoicePageId), "Invoice", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ShopInvoicePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                OrderInvoiceView orderInvoiceView = new OrderInvoiceView();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopInvoicePageId), orderInvoiceView, "content", "Order invoice", "en");
            }
        }

        private void CreateCheckoutPage()
        {
            Guid parentPageID = new Guid(SampleConstants.ShopBasePageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ShopCheckoutPageId), "Checkout", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ShopCheckoutPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                Checkout checkout = new Checkout();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopCheckoutPageId), checkout, "content", "Checkout", "en");
            }
        }

        private void CreateOrdersPage()
        {
            Guid parentPageID = new Guid(SampleConstants.ShopBasePageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ShopOrdersPageId), "Orders", parentPageID, false, false, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ShopOrdersPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                OrdersList ordersList = new OrdersList();
                ordersList.InvoicePageId = new Guid(SampleConstants.ShopInvoicePageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopOrdersPageId), ordersList, "content", "Orders list", "en");
            }
        }

        private void CreateShopFAQPage()
        {
            Guid parentPageID = new Guid(SampleConstants.ShopBasePageId);
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ShopFAQPageId), "Shop FAQ", parentPageID, false, true, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ShopFAQPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                Telerik.Sitefinity.Modules.Lists.Web.UI.ListView faqList = new Telerik.Sitefinity.Modules.Lists.Web.UI.ListView();
                faqList.SelectedListIds = "[\"" + SampleConstants.ShopFAQListId + "\"]";
                faqList.SelectedListText = "Shop FAQ";
                faqList.Mode = Telerik.Sitefinity.Modules.Lists.Web.UI.ListViewMode.Expandable;
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopFAQPageId), faqList, "content", "List items", "en");
            }
        }

        private void CreateEcommerceProducts()
        {
            Dictionary<string, Guid> imageIDs = new Dictionary<string, Guid>();

            using (var fluent = App.WorkWith())
            {
                var images = fluent.Images().Where(i => i.Album.Id == new Guid(SampleConstants.ShopAlbumId) &&
                                                        i.ApprovalWorkflowState == "Published" && i.Status == ContentLifecycleStatus.Live).Get().ToList();
                if (images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        imageIDs.Add(image.Title, image.Id);
                    }
                }
            }

            // Create Alumni watch
            string productTypeName = SampleConstants.ShopProductTypeWatch;
            string title = "Alumni watch";
            string description =
            @"<div>
                <h2>Official Telerik University Alumni Watches</h2>
                <p>Men's exclusive Swiss watch with a variety of personalisation options and luxury features:</p>
                <ul>
                    <li>Silver case with rhodium plating to help prevent tarnishing&nbsp;and scratches.</li>
                    <li>Gold plated movements.</li>
                    <li>Scratch proof dome shaped sapphire crystal.</li>
                    <li>Natural blue sapphire gem stone on the crown.</li>
                    <li>Leather strap in navy blue or black, featuring the University of Oxford crest and 'University of Oxford Collection' on the buckle.</li>
                    <li>The back of the case features the University of Oxford crest and a serial number unique to the individual watch.</li>
                    <li>Smooth or shiny finish to the case.</li>
                    <li>5 year guarantee for manufacturing faults.</li>
                </ul>
                <p><strong>Personalization options:</strong></p>
                <ul>
                    <li>The front of the watch can be personalized with a combination of your name, College, and affiliation.</li>
                    <li>The back of the watch can be personalized with a combination of your name or College and your year of graduation.</li>
                </ul>
                <p>Presented in a luxury leather box featuring the Telerik University crest.</p>
                <p>Size: Approx. 3.5cm diameter watch face.<br />
                Strap: Length 115/75mm, Width: 20/18mm</p>
                <p><strong>Due to the customised nature of this item, please allow up to 25 working days on top of the normal delivery time (approximately 10 days).</strong></p>
                <p>Please note that the lower of the two prices listed is the one for Alumni</p>
                </div>";

            double weight = 0.3;
            decimal price = 355;
            string sku = "15001c";
            DateTime expirationalDate = DateTime.Now.AddYears(1);

            Guid productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            Guid imageID = imageIDs["watch"];
            this.LinkProductToImage(productID, imageID);

            // Create Blue Enamel Swivel Lid Clock
            productTypeName = SampleConstants.ShopProductTypeWatch;
            title = "Blue Enamel Swivel Lid Clock";
            description = @"<div>Silver plated and lacquered round swivel lid alarm clock. Decorated with blue enamel and engraved with the TIU logo on the lid. Size approx. 5cm diameter. Presentation boxed.</div>";

            weight = 2;
            price = 44;
            sku = "15654c";

            productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            imageID = imageIDs["watch2"];
            this.LinkProductToImage(productID, imageID);

            // Create Grey Multi Chunky Knit Beanie
            productTypeName = SampleConstants.ShopProductTypeClothing;
            title = "Grey Multi Chunky Knit Beanie";
            description = @"<div>If you have been looking for that beanie that guarantees to keep you warm while showing off your style and won't cause you to break the bank to buy it, then look no further because the Telerik University Beanie is exactly what you have been looking for. This beanie is tight fitting so you can wear it comfortably underneath a helmet or all by itself. The style and comfort that if offered from the Telerik University Beanie makes it a great addition to any collection.</div>";

            weight = 1;
            price = 14.95m;
            sku = "14453h";

            productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            imageID = imageIDs["hat"];
            this.LinkProductToImage(productID, imageID);

            // Create Silver Plate USB Stick
            productTypeName = SampleConstants.ShopProductTypeGift;
            title = "Silver Plate USB Stick";
            description = @"<div>For the executive who has everything... A beautifully simple silver plated USB Flash Drive perfect for corporate promotions. Perfect for branding with an engraved company logo or customised message to ensure your promotion stays visible each time this practical business gift is used. Packaged in a black gift box that can be customised with a blocked logo or personalised message to ensure maximum exposure of your company. This promotional gift is ideal for anyone looking for that modern and classic promotional gift for there corporate event.</div>";

            weight = 0.1;
            price = 33m;
            sku = "3232u";

            productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            imageID = imageIDs["usb"];
            this.LinkProductToImage(productID, imageID);

            // Create Teddy bear
            productTypeName = SampleConstants.ShopProductTypeGift;
            title = "Teddy bear";
            description = @"<div>Graduation Teddy Bear with cap and gown and shirt embroidered with the Telerik University logo. Approx. 8 inches high in sitting position. Exclusive to the Telerik University Shop.</div>";

            weight = 0.1;
            price = 12m;
            sku = "1111t";

            productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            imageID = imageIDs["teddy"];
            this.LinkProductToImage(productID, imageID);

            // Create Telerik University Backpack
            productTypeName = SampleConstants.ShopProductTypeBag;
            title = "Telerik University Backpack";
            description = @"<div>This stylish backpack includes a mobile phone pocket, a computer compartment, an airflow back system, shoulder straps, a water bottle pocket, audio interface organizer pockets for school accessories, and a sunglasses holder.</div>";

            weight = 0.7;
            price = 100m;
            sku = "1432b";

            productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            imageID = imageIDs["bag"];
            this.LinkProductToImage(productID, imageID);

            imageID = imageIDs["bag2"];
            this.LinkProductToImage(productID, imageID);

            imageID = imageIDs["bag3"];
            this.LinkProductToImage(productID, imageID);

            // Create T-shirt
            productTypeName = SampleConstants.ShopProductTypeClothing;
            title = "T-shirt";
            description = @"<div>100% Cotton Jersey Polo Shirt in mid blue with narrow white stripes and the TIU logo embroidered in blue in the left chest. Available in sizes XS - XXL.</div>";

            weight = 0.3;
            price = 23m;
            sku = "4521t";

            productID = this.CreateProduct(productTypeName, title, description, sku, weight, expirationalDate, true, price, true);

            imageID = imageIDs["shirt"];
            this.LinkProductToImage(productID, imageID);
        }

        public Guid CreateProduct(string productTypeName, string title, string description, string sku, double? weight, DateTime expirationDate,
            bool isShippable, decimal price, bool isActive)
        {
            CatalogManager manager = CatalogManager.GetManager();
            var ecommerceManager = EcommerceManager.GetManager();

            if (manager.GetProducts().Where(t => t.Title == title).SingleOrDefault() != null)
            {
                return Guid.Empty;     // Product already exists
            }

            ProductType productType = ecommerceManager.GetProductTypes().Where(t => t.Title == productTypeName).SingleOrDefault();
            if (productType == null)
            {
                return Guid.Empty;     // Product Type does not exist
            }

            Product product = manager.CreateProduct(productType.ClrType);
            product.ClrType = productType.ClrType;

            product.Title = title;
            product.AssociateBuyerWithRole = Guid.Empty;
            product.DateCreated = DateTime.Now;
            product.Description = description;
            product.IsShippable = isShippable;
            product.Price = price;
            product.Sku = sku;
            product.UrlName = Regex.Replace(title.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
            product.Visible = true;
            product.Weight = weight;

            product.Status = ContentLifecycleStatus.Master;

            manager.Provider.RecompileItemUrls(product);
            manager.SaveChanges();

            var contextBag = new Dictionary<string, string>();
            contextBag.Add("ContentType", product.GetType().FullName);

            string workflowOperation = "Publish";

            WorkflowManager.MessageWorkflow(
                                            product.Id,
                                            product.GetType(),
                                            "OpenAccessDataProvider",
                                            workflowOperation,
                                            false,
                                            contextBag);
            return product.Id;
        }

        private void LinkProductToImage(Guid productId, Guid imageId)
        {
            CatalogManager catManager = CatalogManager.GetManager();
            Product p = catManager.GetProducts().Where(x => x.Id == productId).SingleOrDefault();
            if (p == null)
            {
                return; // Product does not exist
            }

            LibrariesManager librariesManager = LibrariesManager.GetManager();
            Telerik.Sitefinity.Libraries.Model.Image image = librariesManager.GetImage(imageId);

            if (image == null)
            {
                return; // Image does not exist
            }

            // Create, populate and save a ProductImage
            ProductImage pi = new ProductImage();
            pi.AlbumId = image.Album.Id;
            pi.Album = image.Album.Title.Value;
            pi.Id = image.Id;
            pi.Width = image.Width;
            pi.Height = image.Height;
            pi.Title = image.Title;
            pi.Url = image.Url;
            pi.AlternativeText = image.AlternativeText;
            pi.FileName = image.FilePath;
            pi.FileSize = image.TotalSize.ToString();

            // Add the ProductImage to the product's list of images
            p.Images.Add(pi);

            // Save the product
            catManager.SaveChanges();

            // Create a content link between the product and the image

            ContentLinksManager contentLinksManager = ContentLinksManager.GetManager();
            IEnumerable<ContentLink> contentLinks = contentLinksManager.GetContentLinks()
                                                                       .Where(cl => cl.ParentItemId == p.Id && cl.ComponentPropertyName == "ProductImage")
                                                                       .ToList();

            IEnumerable<Guid> persistedIds = contentLinks.Select(cl => cl.ChildItemId);
            List<ProductImage> imagesToAdd = p.Images.Where(i => !persistedIds.Contains(i.Id)).ToList();

            var createdContentLinks = new List<ContentLink>();

            foreach (ProductImage productImage in imagesToAdd)
            {
                Telerik.Sitefinity.Libraries.Model.Image img2 = librariesManager.GetImage(productImage.Id);
                ContentLink contentLink = contentLinksManager.CreateContentLink("ProductImage", p, img2);
                createdContentLinks.Add(contentLink);
            }

            // Save the content link(s)
            contentLinksManager.SaveChanges();
        }

        private void CreateEcommerceProductTypes()
        {
            this.CreateProductTypeIfDoesntExist(SampleConstants.ShopProductTypeBag, SampleConstants.ShopProductTypeBagPlural, ProductDeliveryType.Shippable);
            this.CreateProductTypeIfDoesntExist(SampleConstants.ShopProductTypeClothing, SampleConstants.ShopProductTypeClothingPlural, ProductDeliveryType.Shippable);
            this.CreateProductTypeIfDoesntExist(SampleConstants.ShopProductTypeGift, SampleConstants.ShopProductTypeGiftPlural, ProductDeliveryType.Shippable);
            this.CreateProductTypeIfDoesntExist(SampleConstants.ShopProductTypeWatch, SampleConstants.ShopProductTypeWatchPlural, ProductDeliveryType.Shippable);
            SystemManager.ClearCurrentTransactions();
        }

        public void CreateProductTypeIfDoesntExist(string titleSingular, string titlePlural, ProductDeliveryType deliveryType)
        {
            MetadataManager metadataManager = MetadataManager.GetManager();
            CatalogDefinitionManager catalogDefinitionManager = new CatalogDefinitionManager();

            using (EcommerceManager ecommerceManager = EcommerceManager.GetManager())
            {
                int productTypesCount = ecommerceManager.GetProductTypes().Count();
                if (!ecommerceManager.GetProductTypes().Any(p => p.Title == titleSingular))
                {
                    ProductType productType = ecommerceManager.CreateProductType();
                    productType.Title = titleSingular;
                    productType.TitlePlural = titlePlural;
                    productType.ProductDeliveryType = deliveryType;

                    string productClrType;
                    ecommerceManager.CreateProductTypePersistance(productType, metadataManager, out productClrType);
                    productType.ClrType = productClrType;

                    catalogDefinitionManager.AddProductTypeDefinition(productType, ecommerceManager.Provider.Name);

                    if (productTypesCount == 1)
                    {
                        var singleType = ecommerceManager.GetProductTypes().Single();
                        catalogDefinitionManager.AdjustForMultipleProductTypes(singleType);
                    }

                    ecommerceManager.SaveChanges();
                    metadataManager.SaveChanges(true);

                    //Starting 6.0 we have workflows for products so whenever you are creating a product you have to create a associated workflow
                    var productTypeWorkFlowInstaller = new ProductTypeWorkflowInstaller();
                    productTypeWorkFlowInstaller.InstallWorkflowForProductType(productType);

                    ProductTypeResolver.RestartProductServiceHost();
                }
            }
        }

        public ProductType GetProductTypeByTitle(string title)
        {
            ProductType productType = this.EcommerceMan.GetProductTypes().Where(x => x.Title == title).SingleOrDefault();

            return productType;
        }

        private void CreateShopFAQList()
        {
            var hasCreatedList = SampleUtilities.CreateLocalizedList(new Guid(SampleConstants.ShopFAQListId), SampleConstants.ShopFAQListName, SampleConstants.ShopFAQListName, "en");
            if (hasCreatedList)
            {
                //List item 1
                var parentListId = new Guid(SampleConstants.ShopFAQListId);
                var title = "What is the process for returning my TIU product?";
                var content = "<p>If you have received damaged, defective or incorrectly shipped merchandise, please notify Customer Service within 5 days.</p>";
                var id = Guid.Empty;
                var owner = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                //List item 2
                title = "Where can I find support for my TIU product?";
                content = "<p>For support of your TIU product, please visit Quantum's Support Page or call TIU Customer Service.</p>";
                owner = Guid.Empty;
                id = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                //List item 3
                title = "Can I change, upgrade or cancel my TIU order?";
                content = "<p>Our goal is to ship orders as fast as possible. This leaves very little time for us to cancel or change an order. All upgrades, changes or cancellations must be requested by phone within 1 hour of the order being placed.</p>";
                owner = Guid.Empty;
                id = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                //List item 4
                title = "How do I place an order online with TIU?";
                content = @"To order products online, simply browse for your items, put them in your shopping cart and check out. You can add and remove products as you choose. <br/>Here are the steps: <br/> Place an item in your cart by clicking ""Add To Cart."" You can open your shopping cart at any time to view all of your selected items by simply clicking on ""View Cart"" in the upper right hand corner of the webpage. You can add additional items to your cart at any time. Click on ""Continue Shopping"" in the shopping cart and select the additional products that you would like to purchase by clicking ""Add to Cart"" on the product pages.To continue through the checkout process, choose your service plan and installation option. Select ""Continue.""Change the quantity of an item by typing the number in the quantity field and clicking on ""update."" Remove an item from your cart by clicking ""Remove."" Verify your order and click ""Proceed To Checkout.""Login to your current account or create an accout to place your order.Verify your order and click ""Proceed To Checkout.""Choose your shipping method, verify your mailing address, enter any promotional codes (including gift cards) and enter your billing and payment information. Click ""Review Order.""Double-check your order and confirm quantities, then click ""Place Order.""You will be taken to a confirmation page, containing your order confirmation number, and order details. Be sure to print this page for your records. This document serves as your receipt from Quantum.";
                owner = Guid.Empty;
                id = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");

                //List item 5
                title = "What is the TIU tax policy?";
                content = "TIU automatically computes and adds sales tax in compliance with government tax laws; TIU charges sales tax for orders shipped to all countries";
                owner = Guid.Empty;
                id = Guid.Empty;

                id = SampleUtilities.CreateLocalizedListItem(id, parentListId, title, content, owner, "en");
            }
        }

        private void UploadShopImages()
        {
            List<string> cultures = new List<string>() { "en", "de" };

            SampleUtilities.UploadLocalizedImages(HttpRuntime.AppDomainAppPath + "Images\\Shop Images", "Shop Images", new Guid(SampleConstants.ShopAlbumId), cultures);
        }

        private void CreateShopPage()
        {
            var hasCreatedPage = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ShopBasePageId), "TIU Shop", false, true, "en");
            if (hasCreatedPage)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ShopBasePageId), new Guid(SampleConstants.EducationTemplateId), "en");

                // Add page title
                ContentBlockBase pageTitle = new ContentBlockBase();
                pageTitle.Html = "<h1>University shop</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopBasePageId), pageTitle, "content", "Title", "en");

                // Add layout control
                var mainLayoutControl = new LayoutControl();
                var mainLayoutColumns = new List<ColumnDetails>();

                var mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 15, 0, 0),
                    PlaceholderId = "Left",
                    ColumnWidthPercentage = 75
                };
                mainLayoutColumns.Add(mainLayoutColumn1);

                var mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 15),
                    PlaceholderId = "Right",
                    ColumnWidthPercentage = 25
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopBasePageId), mainLayoutControl, "content", "75% + 25% (custom)", "en");

                ProductsView productsList = new ProductsView();
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopBasePageId), productsList, "Main_Left", "Product list", "en");

                ShoppingCartSummary shoppingCartSummary = new ShoppingCartSummary();

                shoppingCartSummary.CheckoutData = @"{""Id"":""" + SampleConstants.ShopCheckoutPageId + @""",""Title"":""Checkout""}";
                shoppingCartSummary.CheckoutUrl = "~/tiu-shop/checkout";

                shoppingCartSummary.ContinueShoppingData = @"{""Id"":""" + SampleConstants.ShopBasePageId + @""",""Title"":""TIU Shop""}";
                shoppingCartSummary.ContinueShoppingUrl = "~/tiu-shop";

                shoppingCartSummary.ShoppingCartData = @"{""Id"":""" + SampleConstants.ShopShoppingCartPageId + @""",""Title"":""Shopping cart""}";
                shoppingCartSummary.ShoppingCartUrl = "~/tiu-shop/shopping-cart";

                // set the Simple list template!
                shoppingCartSummary.TemplateKey = this.GetShoppingCartSummaryTemplateKey();

                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ShopBasePageId), shoppingCartSummary, "Main_Right", "Shopping cart summary", "en");
            }
        }

        private string GetShoppingCartSummaryTemplateKey()
        {
            string key = String.Empty;
            var myItem = this.PageMan.GetPresentationItems<ControlPresentation>().Where(p => p.Name == "Simple Link").FirstOrDefault();
            if (myItem != null)
                key = myItem.Id.ToString();
            return key;
        }

        #endregion

        #region Managers

        public EcommerceManager EcommerceMan
        {
            get
            {
                if (this.ecommerceManager == null)
                    this.ecommerceManager = EcommerceManager.GetManager();
                return this.ecommerceManager;
            }
        }

        public MetadataManager MetadataMan
        {
            get
            {
                if (this.metadataManager == null)
                    this.metadataManager = MetadataManager.GetManager();
                return this.metadataManager;
            }
        }

        public PublishingManager PublishingMan
        {
            get
            {
                if (this.publishingManager == null)
                    this.publishingManager = PublishingManager.GetManager(PublishingConfig.SearchProviderName);
                return this.publishingManager;
            }
        }

        public PageManager PageMan
        {
            get
            {
                if (this.pageManager == null)
                    this.pageManager = PageManager.GetManager();
                return this.pageManager;
            }
        }

        public OrdersManager OrdersMan
        {
            get
            {
                if (this.ordersManager == null)
                    this.ordersManager = OrdersManager.GetManager();
                return this.ordersManager;
            }
        }

        public ConfigManager ConfigMan
        {
            get
            {
                if (this.configManager == null)
                    this.configManager = ConfigManager.GetManager();
                return this.configManager;
            }
        }

        private EcommerceManager ecommerceManager;
        private CatalogManager catalogManager;
        private MetadataManager metadataManager;
        private PageManager pageManager;
        private PublishingManager publishingManager;
        private OrdersManager ordersManager;
        private ConfigManager configManager;

        #endregion

        private void CreateContactUsPage()
        {
            var pageId = new Guid(SampleConstants.ContactUsPageId);

            var result = SampleUtilities.CreateLocalizedPage(pageId, "Contact us", "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(pageId, new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase titleBlock = new ContentBlockBase();
                titleBlock.Html = @"<h1>Contact us</h1>";
                SampleUtilities.AddControlToLocalizedPage(pageId, titleBlock, "content", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 71,
                    PlaceholderId = "Left"
                };

                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 29,
                    PlaceholderId = "Right"
                };

                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(pageId, mainLayoutControl, "content", "25% + 75%", "en");

                FormsControl contactUsForm = new FormsControl();

                contactUsForm.FormId = new Guid(SampleConstants.ContactUsFormId);

                SampleUtilities.AddControlToLocalizedPage(pageId, contactUsForm, "Main_Left", "Form", "en");

                ContentBlockBase contentBlock = new ContentBlockBase();
                contentBlock.Html = @"<p><strong>Address:</strong><br /> Telerik International University<br /> Malinov str. 33<br /> 8803 Sofia<br /> Phone: +359 44 724 90 90</p>";
                SampleUtilities.AddControlToLocalizedPage(pageId, contentBlock, "Main_Right", "Content block", "en");

                ContentBlockBase mapBlock = new ContentBlockBase();
                mapBlock.Html = @"<iframe width=""250"" scrolling=""no"" height=""250"" frameborder=""0"" src=""http://maps.google.com/?ie=UTF8&amp;t=m&amp;vpsrc=6&amp;ll=42.656829,23.381782&amp;spn=0.003945,0.005386&amp;z=16&amp;output=embed"" marginwidth=""0"" marginheight=""0""></iframe><br />
<small><a style=""color: #0000ff; text-align: left;"" href=""http://maps.google.com/?ie=UTF8&amp;t=m&amp;vpsrc=6&amp;ll=42.656829,23.381782&amp;spn=0.003945,0.005386&amp;z=16&amp;source=embed"">View Larger Map</a></small>";

                SampleUtilities.AddControlToLocalizedPage(pageId, mapBlock, "Main_Right", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(pageId, SampleConstants.ContactUsPageNameGerman, "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(pageId, new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase titleBlock = new ContentBlockBase();
                titleBlock.Html = @"<h1>Kontakt</h1>";
                SampleUtilities.AddControlToLocalizedPage(pageId, titleBlock, "content", "Content block", "de");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 0),
                    ColumnWidthPercentage = 71,
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 29,
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(pageId, mainLayoutControl, "content", "25% + 75%", "de");

                FormsControl contactUsForm = new FormsControl();

                contactUsForm.FormId = new Guid(SampleConstants.ContactUsFormId);

                SampleUtilities.AddControlToLocalizedPage(pageId, contactUsForm, "Main_Left", "Form", "de");

                ContentBlockBase contentBlock = new ContentBlockBase();
                contentBlock.Html = @"<p><strong>Adresse:</strong><br /> Telerik International University<br /> Malinov Str. 33<br /> 8803 Sofia<br /> Tel.: +359 44 724 90 90</p>";
                SampleUtilities.AddControlToLocalizedPage(pageId, contentBlock, "Main_Right", "Content block", "de");

                ContentBlockBase mapBlock = new ContentBlockBase();
                mapBlock.Html = @"<iframe width=""250"" scrolling=""no"" height=""250"" frameborder=""0"" src=""http://maps.google.com/?ie=UTF8&amp;t=m&amp;vpsrc=6&amp;ll=42.656829,23.381782&amp;spn=0.003945,0.005386&amp;z=16&amp;output=embed"" marginwidth=""0"" marginheight=""0""></iframe><br />
<small><a style=""color: #0000ff; text-align: left;"" href=""http://maps.google.com/?ie=UTF8&amp;t=m&amp;vpsrc=6&amp;ll=42.656829,23.381782&amp;spn=0.003945,0.005386&amp;z=16&amp;source=embed"">View Larger Map</a></small>";

                SampleUtilities.AddControlToLocalizedPage(pageId, mapBlock, "Main_Right", "Content block", "de");
            }
        }

        private void CreateContactForms()
        {
            var formId = new Guid(SampleConstants.ContactUsFormId);
            Dictionary<string, object> localizedProperties = new Dictionary<string, object>();

            var result = SampleUtilities.CreateLocalizedForm(formId, SampleConstants.ContactUsFormName, SampleConstants.ContactUsFormTitle, SampleConstants.ContactUsFormSuccessMessage, "en");

            if (result)
            {
                SampleUtilities.CreateLocalizedForm(new Guid(SampleConstants.ContactUsFormId), string.Empty, SampleConstants.ContactUsFormTitleGerman, SampleConstants.ContactUsFormSuccessMessageGerman, "de");

                #region Instructional text

                FormInstructionalText instructionalText = new FormInstructionalText();
                instructionalText.Html = @"Send us a message below and we will get back to you as soon as possible (* denotes required responses in the contact form)";

                var controlID = SampleUtilities.AddControlToLocalizedForm(formId, instructionalText, "Body", "Instructional text", "en");

                instructionalText.Html = @"Senden Sie uns eine Nachricht aus und wir werden uns umgehend mit Ihnen so bald wie m&ouml;glich (* kennzeichnet erforderliche Reaktionen in das Kontaktformular)";

                localizedProperties.Clear();
                localizedProperties.Add("Html", @"Senden Sie uns eine Nachricht aus und wir werden uns umgehend mit Ihnen so bald wie m&ouml;glich (* kennzeichnet erforderliche Reaktionen in das Kontaktformular)");

                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region Main layout

                LayoutControl mainLayout = new LayoutControl();

                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();

                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 15, 0, 0),
                    PlaceholderId = "Left"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);

                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 15),
                    PlaceholderId = "Right"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);

                mainLayout.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayout.ID = "Main";
                SampleUtilities.AddControlToLocalizedForm(formId, mainLayout, "Body", "50% + 50% (custom)", "en");

                #endregion

                #region Name box

                FormTextBox nameBox = new FormTextBox();
                nameBox.Title = "Your Name*";
                nameBox.TextBoxSize = FormControlSize.Medium;
                nameBox.ValidatorDefinition.Required = true;
                nameBox.ValidatorDefinition.MaxLength = 40;
                controlID = SampleUtilities.AddControlToLocalizedForm(formId, nameBox, "Main_Left", "Textbox", "en");

                localizedProperties.Clear();
                localizedProperties.Add("Title", "Ihr Name*");
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region Email box
                FormTextBox emailBox = new FormTextBox();
                emailBox.Title = "Your Email*";
                emailBox.TextBoxSize = FormControlSize.Medium;
                emailBox.ValidatorDefinition.Required = true;
                emailBox.ValidatorDefinition.MaxLength = 40;
                controlID = SampleUtilities.AddControlToLocalizedForm(formId, emailBox, "Main_Left", "Textbox", "en");

                localizedProperties.Clear();
                localizedProperties.Add("Title", "Ihr Email*");
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region Categories list
                FormDropDownList categoriesList = new FormDropDownList();
                categoriesList.Title = "Category";
                categoriesList.Choices.Clear();

                ChoiceItem athleticsItem = new ChoiceItem()
                {
                    Text = "Athletics",
                    Value = "Athletics",
                    Description = "Athletics"
                };
                categoriesList.Choices.Add(athleticsItem);

                ChoiceItem admissionsItem = new ChoiceItem()
                {
                    Text = "Admissions",
                    Value = "Admissions",
                    Description = "Admissions"
                };
                categoriesList.Choices.Add(admissionsItem);

                ChoiceItem shopItem = new ChoiceItem()
                {
                    Text = "Shop",
                    Value = "Shop",
                    Description = "Shop"
                };
                categoriesList.Choices.Add(shopItem);

                controlID = SampleUtilities.AddControlToLocalizedForm(formId, categoriesList, "Main_Left", "Dropdown list", "en");

                athleticsItem.Text = "Sport";
                athleticsItem.Description = "Sport";
                admissionsItem.Text = "Zulassung";
                admissionsItem.Description = "Zulassung";
                shopItem.Text = "E-Shop";
                shopItem.Description = "E-Shop";

                localizedProperties.Clear();
                localizedProperties.Add("Title", "Kategorie");
                localizedProperties.Add("Choices", categoriesList.Choices);
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region Message box

                FormParagraphTextBox messageBox = new FormParagraphTextBox();
                messageBox.Title = "Message*";
                messageBox.ParagraphTextBoxSize = FormControlSize.Medium;
                messageBox.ValidatorDefinition.Required = true;
                messageBox.ValidatorDefinition.MaxLength = 400;

                controlID = SampleUtilities.AddControlToLocalizedForm(formId, messageBox, "Main_Left", "Paragraph textbox", "en");

                localizedProperties.Clear();
                localizedProperties.Add("Title", "Meldung*");
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region SubmitButton

                FormSubmitButton submitButton = new FormSubmitButton();
                submitButton.Text = "Send";
                submitButton.ButtonSize = FormControlSize.Small;

                controlID = SampleUtilities.AddControlToLocalizedForm(formId, submitButton, "Main_Left", "Submit button", "en");

                submitButton.Text = "Senden";

                localizedProperties.Clear();
                localizedProperties.Add("Text", "Senden");
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region Multiple choice

                FormMultipleChoice genderMultipleChoice = new FormMultipleChoice();
                genderMultipleChoice.Title = "Sex";
                genderMultipleChoice.Choices.Clear();

                ChoiceItem maleItem = new ChoiceItem()
                {
                    Text = "Male",
                    Value = "Male",
                    Description = "Male"
                };
                genderMultipleChoice.Choices.Add(maleItem);

                ChoiceItem femaleItem = new ChoiceItem()
                {
                    Text = "Female",
                    Value = "Female",
                    Description = "Female"
                };
                genderMultipleChoice.Choices.Add(femaleItem);

                controlID = SampleUtilities.AddControlToLocalizedForm(formId, genderMultipleChoice, "Main_Right", "Multiple choice", "en");

                genderMultipleChoice.Title = "Geschlecht";
                maleItem.Text = "Männlich";
                maleItem.Description = "Männlich";
                femaleItem.Text = "Weiblich";
                femaleItem.Description = "Weiblich";

                localizedProperties.Clear();
                localizedProperties.Add("Title", "Geschlecht");
                localizedProperties.Add("Choices", genderMultipleChoice.Choices);
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");

                #endregion

                #region Major checkboxes

                FormCheckboxes majorCheckboxes = new FormCheckboxes();
                majorCheckboxes.Title = "What major are you interested in?";
                majorCheckboxes.Choices.Clear();

                ChoiceItem artHistory = new ChoiceItem()
                {
                    Text = "Art History",
                    Value = "Art History",
                    Description = "Art History"
                };
                majorCheckboxes.Choices.Add(artHistory);

                ChoiceItem asianStudies = new ChoiceItem()
                {
                    Text = "Asian Studies",
                    Value = "Asian Studies",
                    Description = "Asian Studies"
                };
                majorCheckboxes.Choices.Add(asianStudies);

                ChoiceItem astronomy = new ChoiceItem()
                {
                    Text = "Astronomy",
                    Value = "Astronomy",
                    Description = "Astronomy"
                };
                majorCheckboxes.Choices.Add(astronomy);

                ChoiceItem biochemistry = new ChoiceItem()
                {
                    Text = "Biochemistry and Molecular Biology",
                    Value = "Biochemistry and Molecular Biology",
                    Description = "Biochemistry and Molecular Biology"
                };
                majorCheckboxes.Choices.Add(biochemistry);

                ChoiceItem biology = new ChoiceItem()
                {
                    Text = "Biology",
                    Value = "Biology",
                    Description = "Biology"
                };
                majorCheckboxes.Choices.Add(biology);

                ChoiceItem chemistry = new ChoiceItem()
                {
                    Text = "Chemistry",
                    Value = "Chemistry",
                    Description = "Chemistry"
                };
                majorCheckboxes.Choices.Add(chemistry);

                ChoiceItem chinese = new ChoiceItem()
                {
                    Text = "Chinese",
                    Value = "Chinese",
                    Description = "Chinese"
                };
                majorCheckboxes.Choices.Add(chinese);

                ChoiceItem classics = new ChoiceItem()
                {
                    Text = "Classics",
                    Value = "Classics",
                    Description = "Classics"
                };
                majorCheckboxes.Choices.Add(classics);

                ChoiceItem computerScience = new ChoiceItem()
                {
                    Text = "Computer Science",
                    Value = "Computer Science",
                    Description = "Computer Science"
                };
                majorCheckboxes.Choices.Add(computerScience);

                ChoiceItem economics = new ChoiceItem()
                {
                    Text = "Economics",
                    Value = "Economics",
                    Description = "Economics"
                };
                majorCheckboxes.Choices.Add(economics);

                controlID = SampleUtilities.AddControlToLocalizedForm(formId, majorCheckboxes, "Main_Right", "Checkboxes", "en");

                majorCheckboxes.Title = "Welche Fächer interessieren Sie?";

                artHistory.Text = "Kunstgeschichte";
                artHistory.Description = "Kunstgeschichte";
                asianStudies.Text = "Asiatische Lehren";
                asianStudies.Description = "Asiatische Lehren";
                astronomy.Text = "Astronomie";
                astronomy.Description = "Astronomie";
                biology.Text = "Biologie";
                biology.Description = "Biologie";
                biochemistry.Text = "Biochemie und Molekularbiologie";
                biochemistry.Description = "Biochemie und Molekularbiologie";
                chinese.Text = "Chinesisch";
                chinese.Description = "Chinesisch";
                classics.Text = "Klassische Lehren";
                classics.Description = "Klassische Lehren";
                computerScience.Text = "Informatik";
                computerScience.Description = "Informatik";
                economics.Text = "Wirtschaft";
                economics.Description = "Wirtschaft";
                chemistry.Text = "Chemie";
                chemistry.Description = "Chemie";

                localizedProperties.Clear();
                localizedProperties.Add("Title", "Welche Fächer interessieren Sie?");
                localizedProperties.Add("Choices", majorCheckboxes.Choices);
                SampleUtilities.UpdateControlInLocalizedForm(controlID, formId, localizedProperties, "de");
                #endregion
            }
        }

        #region Internal resources

        private void CreateInternalResourcesGroupPage()
        {
            SampleUtilities.CreatePageGroup(new Guid(SampleConstants.InternalResourcesGroupPageId), Guid.Empty, "Internal Resources", "en");
        }

        private void CreateErrorPagesGroupPage()
        {
            SampleUtilities.CreatePageGroup(new Guid(SampleConstants.ErrorPagesGroupPageId), new Guid(SampleConstants.InternalResourcesGroupPageId), "Error pages", "en");
        }

        private void Create404Page()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.Error404PageId), "404", new Guid(SampleConstants.ErrorPagesGroupPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.Error404PageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase pageNotFoundBlock = new ContentBlockBase();
                pageNotFoundBlock.Html = string.Format(@"<h1>Page not found!</h1> The page you have requested was not found on this server! Please check the URL and retry! <p>Or you can go back to the <a href=""[pages]{0}"">home page</a>.</p>", SampleConstants.HomePageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.Error404PageId), pageNotFoundBlock, "content", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.Error404PageId), "404", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.Error404PageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase pageNotFoundBlock = new ContentBlockBase();
                pageNotFoundBlock.Html = string.Format(@"<h1>Seite nicht gefunden!</h1> Der von Ihnen angeforderte Seite wurde nicht auf diesem Server gefunden! Bitte &uuml;berpr&uuml;fen Sie die URL und versuchen!<br /> <br /> Oder Sie k&ouml;nnen zur&uuml;ck auf die <a href=""[pages]{0}"">Startseite</a>.", SampleConstants.HomePageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.Error404PageId), pageNotFoundBlock, "content", "Content block", "de");
            }
        }

        private void CreateErrorPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ErrorPageId), "Error", new Guid(SampleConstants.ErrorPagesGroupPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ErrorPageId), new Guid(SampleConstants.EducationTemplateId), "en");

                ContentBlockBase requestProblemBlock = new ContentBlockBase();
                requestProblemBlock.Html = string.Format(@"<h1>There was a problem with your request!</h1> <p>You are either trying to access functionality not supported by your license or there is a temporary server issue.</p> <p>Please note that the Ecommerce feature of this website does not run on a Sitefinity Small Business Edition. It requires Standard or higher Edition. </p> <p>You can go back to the <a href=""[pages]{0}"">home page</a>.</p>", SampleConstants.HomePageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ErrorPageId), requestProblemBlock, "content", "Content block", "en");
            }

            result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.ErrorPageId), "Fehler", "de");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.ErrorPageId), new Guid(SampleConstants.EducationTemplateId), "de");

                ContentBlockBase requestProblemBlock = new ContentBlockBase();
                requestProblemBlock.Html = string.Format(@"<h1></h1> <h1>Es gab ein Problem mit Ihrer Anfrage!</h1> Sie sind entweder versuchen, um die Funktionalit&auml;t Zugang nicht durch Ihre Lizenz unterst&uuml;tzt, oder es ist eine tempor&auml;re Server-Problem.<br /> <br /> Bitte beachten Sie, dass die E-Commerce-Funktion der Website nicht auf einem Sitefinity Small Business Edition laufen. Es erfordert Standard oder h&ouml;her Edition.<br /> <br /> Sie k&ouml;nnen zur&uuml;ck auf die <a href=""[pages]{0}"">Startseite</a>.", SampleConstants.HomePageId);
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.ErrorPageId), requestProblemBlock, "content", "Content block", "de");
            }
        }

        #endregion

        #region Facebook fan pages

        private void CreateFacebookFanPagesGroupPage()
        {
            SampleUtilities.CreatePageGroup(new Guid(SampleConstants.FacebookFanPagesGroupPageId), Guid.Empty, "Facebook fan pages", "en");
        }

        private void CreateTIUFacebookPage()
        {
            var result = SampleUtilities.CreateLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), "TIU Facebook", new Guid(SampleConstants.FacebookFanPagesGroupPageId), "en");

            if (result)
            {
                SampleUtilities.SetTemplateToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), new Guid(SampleConstants.FacebookTemplateId), "en");

                ImageControl fansImage = new ImageControl();
                fansImage.ImageId = SampleUtilities.GetLocalizedImageId("fans", "en");
                fansImage.CssClass = "sfimageWrp";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), fansImage, "Content_Center", "Image", "en");

                ContentBlockBase welcomeBlock = new ContentBlockBase();
                welcomeBlock.Html = @"<h1>Welcome to TIU</h1>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), welcomeBlock, "Content_Center", "Content block", "en");

                var mainLayoutControl = new LayoutControl();
                List<ColumnDetails> mainLayoutColumns = new List<ColumnDetails>();
                ColumnDetails mainLayoutColumn1 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 10),
                    ColumnWidthPercentage = 33,
                    PlaceholderId = "Left33"
                };
                mainLayoutColumns.Add(mainLayoutColumn1);
                ColumnDetails mainLayoutColumn2 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 0, 0, 12),
                    ColumnWidthPercentage = 34,
                    PlaceholderId = "Middle34"
                };
                mainLayoutColumns.Add(mainLayoutColumn2);
                ColumnDetails mainLayoutColumn3 = new ColumnDetails()
                {
                    ColumnSpaces = new ColumnSpaces(0, 10, 0, 12),
                    ColumnWidthPercentage = 33,
                    PlaceholderId = "Right33"
                };
                mainLayoutColumns.Add(mainLayoutColumn3);
                mainLayoutControl.Layout = SampleUtilities.GenerateLayoutTemplate(mainLayoutColumns, string.Empty);
                mainLayoutControl.ID = "Main";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), mainLayoutControl, "Content_Center", "33% + 34% + 33% (custom)", "en");

                ContentBlockBase leftHeaderBlock = new ContentBlockBase();
                leftHeaderBlock.Html = @"<h3>The Campus</h3>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), leftHeaderBlock, "Main_Left33", "Content block", "en");

                ContentBlockBase middleHeaderBlock = new ContentBlockBase();
                middleHeaderBlock.Html = @"<h3>The University</h3>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), middleHeaderBlock, "Main_Middle34", "Content block", "en");

                ContentBlockBase rightHeaderBlock = new ContentBlockBase();
                rightHeaderBlock.Html = @"<h3>Sofia</h3>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), rightHeaderBlock, "Main_Right33", "Content block", "en");

                ImageControl leftImage = new ImageControl();
                leftImage.ImageId = SampleUtilities.GetLocalizedImageId("campus_small", "en");
                leftImage.CssClass = "sfimageWrp";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), leftImage, "Main_Left33", "Image", "en");

                ImageControl middleImage = new ImageControl();
                middleImage.ImageId = SampleUtilities.GetLocalizedImageId("uni_small", "en");
                middleImage.CssClass = "sfimageWrp";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), middleImage, "Main_Middle34", "Image", "en");

                ImageControl rightImage = new ImageControl();
                rightImage.ImageId = SampleUtilities.GetLocalizedImageId("sofia1_small", "en");
                rightImage.CssClass = "sfimageWrp";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), rightImage, "Main_Right33", "Image", "en");

                ContentBlockBase leftBlock = new ContentBlockBase();
                leftBlock.Html = @"<p>The Telerik International University campus is one of the most congenial places to study. Situated near the city centre of Sofia, the campus is spacious without being overwhelming.</p>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), leftBlock, "Main_Left33", "Content block", "en");

                ContentBlockBase middleBlock = new ContentBlockBase();
                middleBlock.Html = @"<p>The Telerik International University was founded in 2008. It is one of the new, modern universities in Bulgaria, and with approximately 1,500 students it is of a manageable size.</p>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), middleBlock, "Main_Middle34", "Content block", "en");

                ContentBlockBase rightBlock = new ContentBlockBase();
                rightBlock.Html = @"<p>Sofia is one of the oldest cities in Europe. The Romans founded the city in 29 B.C. In the late Middle Ages and the Renaissance, the city developed into a trade and finance metropolis.</p>";
                SampleUtilities.AddControlToLocalizedPage(new Guid(SampleConstants.TIUFacebookPageId), rightBlock, "Main_Right33", "Content block", "en");
            }
        }

        #endregion

        private void CreateTags()
        {
            foreach (KeyValuePair<string, string> tag in this.TagIds)
            {
                SampleUtilities.CreateTag(tag.Key, new Guid(tag.Value));
            }
        }

        private void CleanUpResources()
        {
            var manager = PageManager.GetManager();

            var allBackend = manager.GetPageNodes().Where(pn => pn.RootNodeId == SiteInitializer.BackendRootNodeId);
            foreach (var page in allBackend)
            {
                var currentTitle = page.Title;
                var currentUrlName = page.UrlName;
                var a = "$Resources: ";
                if (currentTitle.Contains(a))
                {

                    var trimResources = currentTitle.ToString().Replace(a, string.Empty);
                    var resClass = trimResources.Split(',')[0];
                    var resKey = trimResources.Split(',')[1];
                    var resManager = ResourceManager.GetManager();
                    var resource = resManager.GetResourceOrEmpty(CultureInfo.InvariantCulture, resClass, resKey);
                    if (resource != null)
                    {
                        page.Title = resource.Value;
                        manager.SaveChanges();
                    }
                }
            }
        }

        #region Application methods

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
        //Handle the ConfigManager.Executed event with this method, if you use VirtualPathProvider.
        //private void ConfigManager_Executed(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs args)
        //{
        //    if (args.CommandName == "SaveSection")
        //    {
        //        var section = args.CommandArguments as VirtualPathSettingsConfig;
        //        if (section != null)
        //        {
        //            // Reset the Virtual path manager, whenever the section of the VirtualPathSettingsConfig is saved.
        //            // This is needed so that the prefixes for templates in our module assembly are taken into account.
        //            VirtualPathManager.Reset();
        //        }
        //    }
        //}

        #endregion
    }
}