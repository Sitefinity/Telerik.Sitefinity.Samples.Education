<%@ Control Language="C#" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI.PublicControls.BrowseAndEdit" Assembly="Telerik.Sitefinity" %>

<telerik:RadListView id="listsControl" runat="server"
                     ItemPlaceholderId="ListContainer"
                     EnableEmbeddedSkins="false"
                     EnableEmbeddedBaseStylesheet="false">
    <LayoutTemplate>
        <div class="sfexpandedListWrp educationAnnouncements">
            <asp:PlaceHolder id="ListContainer" runat="server" />
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <h2 class="sflistTitle">
            <asp:Literal ID="Literal1" runat="server" Text='<%# Eval("Title") %>' />
        </h2>
        <telerik:RadListView ID="listItemsControl" runat="server" 
                ItemPlaceholderID="ItemsContainer" 
                EnableEmbeddedSkins="false" 
                EnableEmbeddedBaseStylesheet="false">
            <LayoutTemplate>
                <ul class="sflistList">
                    <asp:PlaceHolder ID="ItemsContainer" runat="server" />
                </ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li class="sflistListItem">
                    <h3 class="sflistItemTitle">
                        <asp:Literal ID="Literal2" runat="server" Text='<%# Eval("Title") %>' />
                    </h3>
                    <div class="sflistItemContent">
                        <asp:Literal ID="Literal3" runat="server" Text='<%# Eval("Content") %>' />
                    </div>
                    <sf:ContentBrowseAndEditToolbar ID="BrowseAndEditToolbar" runat="server" Mode="Edit,Delete,Unpublish"></sf:ContentBrowseAndEditToolbar>
                </li>
            </ItemTemplate>
        </telerik:RadListView>
    </ItemTemplate>
</telerik:RadListView>
