namespace CoverFlowSilverlight
{
    using System.Windows.Browser;
    using System.Windows.Controls;

    /// <summary>
    /// The user control that is used to load the coverflow widget.
    /// </summary>
    public partial class MainPage : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="title">The title of the image.</param>
        /// <param name="url">The URL of the image.</param>
        [ScriptableMember]
        public void SetItem(string title, string url)
        {
            this.coverFlow.Items.Add(new SitefinityImage(title, url));
        }
    }
}