<%@ Control Language="C#" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI" Assembly="Telerik.Sitefinity" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI.ContentUI" Assembly="Telerik.Sitefinity" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI.PublicControls.BrowseAndEdit" Assembly="Telerik.Sitefinity" %>
<%@ Import Namespace="Telerik.Sitefinity" %>	

<telerik:RadListView ID="SingleItemContainer" ItemPlaceholderID="ItemContainer" AllowPaging="False" runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false">
    <LayoutTemplate>
        <div class="sfeventDetails">
            <div class="sfeventLinksWrp">
                <sf:MasterViewHyperLink ID="MasterViewHyperLink1" class="sfeventBack" Text="<%$ Resources:EventsResources, AllEvents %>" runat="server" />
            </div>
            <asp:PlaceHolder ID="ItemContainer" runat="server" />
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <h1 class="sfeventTitle">
            <asp:Literal ID="Literal1" Text='<%# Eval("Title") %>' runat="server" />
        </h1>
        <sf:ContentBrowseAndEditToolbar ID="BrowseAndEditToolbar" runat="server" Mode="Edit,Delete,Unpublish"></sf:ContentBrowseAndEditToolbar>
        <ul class="sfeventDatesLocationContacts">
            <sf:FieldListView ID="EventDates" runat="server" WrapperTagName="li" />
            <sf:FieldListView ID="Location" runat="server" 
                Text="<%$ Resources:EventsResources, Where %>" Properties="City, State, Country" 
                WrapperTagName="li"
            />
            <sf:FieldListView ID="Street" runat="server" 
                Text="<%$ Resources:EventsResources, Address %>" Properties="Street" 
                WrapperTagName="li"
            />
            <sf:FieldListView ID="ContactName" runat="server" 
                Text="<%$ Resources:EventsResources, ContactName %>" Properties="ContactName" 
                WrapperTagName="li"
            />
            <sf:FieldListView ID="ContactEmail" runat="server" 
                Text="<%$ Resources:EventsResources, ContactEmail %>" Properties="ContactEmail" 
                WrapperTagName="li"
            />
            <sf:FieldListView ID="Web" runat="server" 
                Text="<%$ Resources:EventsResources, WebSite %>" Properties="ContactWeb" 
                WrapperTagName="li"
            />
            <sf:FieldListView ID="Phone" runat="server" 
                Text="<%$ Resources:EventsResources, ContactPhone %>" Properties="ContactPhone, ContactCell" 
                WrapperTagName="li"
            />
        </ul>
        <div class="sfeventContent">
            <asp:Literal ID="Literal4" Text='<%# Eval("Content") %>' runat="server" />
        </div>
        <sf:ContentView 
             id="commentsListView" 
             ControlDefinitionName="EventsCommentsFrontend" 
             DetailViewName="CommentsMasterView"
             ContentViewDisplayMode="Master"
             runat="server" />
        <comments:CommentsWidget runat="server"
               ThreadKey='<%# ControlUtilities.GetLocalizedKey((Guid)Eval("Id")) %>'
               AllowComments='true'
               ThreadTitle='<%# Eval("Title") %>'
               ThreadType='<%# Container.DataItem.GetType().FullName %>'
               GroupKey='<%# ControlUtilities.GetUniqueProviderKey("Telerik.Sitefinity.Modules.Events.EventsManager", Eval("Provider.Name").ToString()) %>' />
    </ItemTemplate>
</telerik:RadListView>