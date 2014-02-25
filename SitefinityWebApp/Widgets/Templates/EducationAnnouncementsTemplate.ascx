<%@ Control Language="C#" %>



<%@ Register Assembly="Telerik.Web.UI, Version=2013.3.1114.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Sitefinity, Version=6.3.5000.0, Culture=neutral, PublicKeyToken=b28c218413bdf563" Namespace="Telerik.Sitefinity.Web.UI.PublicControls.BrowseAndEdit" TagPrefix="sf" %>

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
