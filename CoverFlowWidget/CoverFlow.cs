using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Modules.Pages.Web.UI;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web;

namespace CoverFlowWidget
{
    /// <summary>
    /// CoverFlow widget class that inherits from SimpleView that is registered in Sitefinity's toolbox.
    /// </summary>
    [RequireScriptManager]
    public class CoverFlow : SimpleView
    {
        private HiddenField imagesField;
        private string albumTitle;

        /// <summary>
        /// Gets or sets the album title for which the widget retrieves images from.
        /// </summary>
        /// <value>The title of the album in which your images are placed.</value>
        public string AlbumTitle
        {
            get
            {
                return this.albumTitle;
            }

            set
            {
                this.albumTitle = value;
            }
        }

        /// <summary>
        /// Gets the name of the embedded layout template.
        /// </summary>
        /// <value>Full path of the template name as a string.</value>
        /// <remarks>
        /// Override this property to change the embedded template to be used with the dialog
        /// </remarks>
        protected override string LayoutTemplateName
        {
            get { return "CoverFlowWidget.CoverFlowTemplate.ascx"; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> value that corresponds to this Web server control. This property is used primarily by control developers.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> enumeration values.</returns>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                ////Use div wrapper tag to make easier common styling. This will surround the layout template with a div tag.
                return HtmlTextWriterTag.Div;
            }
        }

        /// <summary>
        /// Initializes the controls part of your template.
        /// </summary>
        /// <param name="container">Template container for all controls.</param>
        /// <remarks>
        /// Initialize your controls in this method. Do not override CreateChildControls method.
        /// </remarks>
        protected override void InitializeControls(GenericContainer container)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Hashtable> images = this.GetImages();
            string serializedCollection = serializer.Serialize(images);

            this.imagesField = container.GetControl<HiddenField>("imagesField", true);
            this.imagesField.Value = serializedCollection;
        }

        private List<Hashtable> GetImages()
        {
            List<Hashtable> results = new List<Hashtable>();
            string title = this.AlbumTitle;

            ////get IQueryable of images from the Fluent API.
            var images = App.WorkWith().Images()
                .Where(
                (w) => w.Parent.Title == title &&
                       w.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live)
                 .Get();

            foreach (Telerik.Sitefinity.Libraries.Model.Image v in images)
            {
                Hashtable table = new Hashtable();
                table.Add("Url", RouteHelper.GetAbsoluteUrl("~" + v.MediaUrl));
                table.Add("Title", v.Title.ToString());
                results.Add(table);
            }

            return results;
        }
    }
}
