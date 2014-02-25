using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

namespace SitefinityWebApp
{
    public partial class InitializeSample : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        [ScriptMethod]
        public static string SetupSample()
        {            
            CreateImagesModule();

            return "Sample initialized successfully";
        }

        private static void CreateImagesModule()
        {
            SampleUtilities.UploadImages(HttpRuntime.AppDomainAppPath + "Images\\Default", "Some Album");
        }
    }
}