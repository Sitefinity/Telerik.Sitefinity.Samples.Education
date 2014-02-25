namespace CoverFlowSilverlight
{
    /// <summary>
    /// Class that contains the Title and the Url of an images passed by Sitefinity.
    /// </summary>
    public class SitefinityImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SitefinityImage"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL of the image.</param>
        public SitefinityImage(string title, string url)
        {
            this.Title = title;
            this.URL = url;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL of the image.</value>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title of the image.</value>
        public string Title { get; set; }
    }
}