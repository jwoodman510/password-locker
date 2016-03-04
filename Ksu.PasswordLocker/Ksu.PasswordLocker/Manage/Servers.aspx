<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Servers.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.Servers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Styles/styles.css" rel="stylesheet" type="text/css"/>
    <h1>Servers</h1>
    <br/>
    <div class="row">
        <button runat="server" ID="AddServer" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add Server</span>
        </button>
    </div>
    <div class="row" style="padding-top: 15px;">
    <asp:Panel runat="server" DefaultButton="SearchTextButton">
        <asp:TextBox ID="SearchText" runat="server" CssClass="modalTextInput"></asp:TextBox>
        <asp:Button ID="SearchTextButton" runat="server" Text="Search" style="display:none;" OnClick="SearchTextButton_OnClick" />
    </asp:Panel>
    </div>
    <div class="row text-danger"><h4><asp:Literal runat="server" ID="GridError"></asp:Literal></h4></div>
    <div class="row">
        <asp:GridView runat="server" ID="ServerGrid"
            AutoGenerateColumns="False"
            Visible="False"
            DataKeyNames="ServerId"
            Width="600"
            BackColor="White"
            BorderColor="#999999"
            BorderStyle="Solid"
            BorderWidth="1px"
            CellPadding="3"
            ForeColor="Black"
            GridLines="Vertical"
            OnRowDeleting="ServerGrid_OnRowDeleting"
            OnRowUpdating="ServerGrid_OnRowUpdating"
            OnRowEditing="ServerGrid_OnRowEditing"
            OnRowCancelingEdit="ServerGrid_OnRowCancelingEdit"
            OnRowCommand="ServerGrid_OnRowCommand">
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:BoundField DataField="ServerName" HeaderText="Name"/>
                <asp:ButtonField ButtonType="Button" HeaderText="Departments" Text="Manage Departments" CommandName="ManageDepartments"/>
                <asp:CommandField ShowEditButton="True" />
                <asp:CommandField ShowDeleteButton="True" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
    </div>
    <%--Add Server Modal--%>
    <ajaxToolkit:ModalPopupExtender ID="AddServerPopupExtender"
        runat="server"
        TargetControlID="AddServer" 
        PopupControlID="ServerPanel"
        PopupDragHandleControlID="ServPopupHeader"
        Drag="True"
        BackgroundCssClass="modalBackground"
        BehaviorID="AddServerModal">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel runat="server" ID="ServerPanel" style="display:none" CssClass="modalPopup">
        <div style="text-align: center">
            <asp:Label Font-Bold="True" runat="server" ID="ServPopupHeader">Add Server</asp:Label>
            <i class="fa fa-times-circle" style="float: right; cursor: pointer;" onclick="closeServerModal()"></i>
        </div>
        <hr />
        <div class="text-danger">
            <span id="servError"><asp:Literal runat="server" ID="ServErrorMessage"/></span>
        </div>
        <div>
            <asp:Label AssociatedControlID="ServerNameInput" runat="server" CssClass="control-label">Name:</asp:Label>
            <span id="servNameInput"><asp:TextBox runat="server" ID="ServerNameInput" MaxLength="200" CssClass="modalTextInput"></asp:TextBox></span>
        </div>
        <br/>
        <div class="Controls" style="text-align: right">
            <asp:Button runat="server" ID="addServerSave" Text="Save" OnClick="addServerSave_OnClick"/>
        </div>
    </asp:Panel>
    <script type="text/javascript">
        function closeServerModal() {
            document.getElementById('servError').innerText = "";
            document.getElementById('servNameInput').firstChild.value = "";
            $find('AddServerModal').hide();
        }
    </script>
</asp:Content>
