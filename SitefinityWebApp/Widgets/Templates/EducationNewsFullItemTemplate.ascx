<%@ Control Language="C#" %>



<%@ Register Assembly="Telerik.Web.UI, Version=2013.3.1114.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Sitefinity, Version=6.3.5000.0, Culture=neutral, PublicKeyToken=b28c218413bdf563" Namespace="Telerik.Sitefinity.Web.UI" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity, Version=6.3.5000.0, Culture=neutral, PublicKeyToken=b28c218413bdf563" Namespace="Telerik.Sitefinity.Web.UI.ContentUI" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity, Version=6.3.5000.0, Culture=neutral, PublicKeyToken=b28c218413bdf563" Namespace="Telerik.Sitefinity.Web.UI.PublicControls.BrowseAndEdit" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity, Version=6.3.5000.0, Culture=neutral, PublicKeyToken=b28c218413bdf563" Namespace="Telerik.Sitefinity.Modules.Comments.Web.UI.Frontend" TagPrefix="comments" %>

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
