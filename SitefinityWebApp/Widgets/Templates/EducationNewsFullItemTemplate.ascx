<%@ Control Language="C#" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI" Assembly="Telerik.Sitefinity" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI.ContentUI" Assembly="Telerik.Sitefinity" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI.PublicControls.BrowseAndEdit"
    Assembly="Telerik.Sitefinity" %>
<%@ Register TagPrefix="comments" Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Modules.Comments.Web.UI.Frontend" %>

<telerik:RadListView ID="DetailsView" ItemPlaceholderID="ItemContainer" AllowPaging="False"
    runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false">
    <LayoutTemplate>
        <div class="sfnewsDetails">
            <div class="sfnewsLinksWrp">
                <sf:MasterViewHyperLink ID="MasterViewHyperLink1" class="sfnewsBack" Text="<%$ Resources:NewsResources, AllNews %>"
                    runat="server" />
            </div>
            <asp:PlaceHolder ID="ItemContainer" runat="server" />
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <h1 class="sfnewsTitle">
            <asp:Literal ID="Literal1" Text='<%# Eval("Title") %>' runat="server" />
        </h1>
        <sf:ContentBrowseAndEditToolbar ID="BrowseAndEditToolbar" runat="server" Mode="Edit,Delete,Unpublish">
        </sf:ContentBrowseAndEditToolbar>
        <br />
        <sf:FieldListView ID="summary" runat="server" Text="{0}" Properties="Summary" WrapperTagName="div"
            WrapperTagCssClass="sfnewsSummary" />
        <div class="sfnewsContent">
            <asp:Literal ID="Literal3" Text='<%# Eval("Content") %>' runat="server" />
            <br />
            <asp:PlaceHolder ID="socialOptionsContainer" runat="server"></asp:PlaceHolder>
            <br />
            <br />
            <br />
        </div>
        <sf:ContentView ID="commentsListView" ControlDefinitionName="NewsCommentsFrontend"
            MasterViewName="CommentsMasterView" ContentViewDisplayMode="Master" runat="server" />     
        <comments:CommentsWidget runat="server"
               ThreadKey='<%# ControlUtilities.GetLocalizedKey((Guid)Eval("Id")) %>'
               AllowComments='true'
               ThreadTitle='<%# Eval("Title") %>'
               ThreadType='<%# Container.DataItem.GetType().FullName %>'
               GroupKey='<%# ControlUtilities.GetUniqueProviderKey("Telerik.Sitefinity.Modules.News.NewsManager", Eval("Provider.Name").ToString()) %>' />
    </ItemTemplate>
</telerik:RadListView>
