<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.Users" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Styles/styles.css" rel="stylesheet" type="text/css"/>
    <h1>Users</h1>
    <br/>
    <div class="row">
        <button runat="server" ID="AddUser" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add User</span>
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
        <asp:GridView runat="server" ID="UserGrid"
            AutoGenerateColumns="False"
            Visible="False"
            DataKeyNames="Id"
            Width="600"
            BackColor="White"
            BorderColor="#999999"
            BorderStyle="Solid"
            BorderWidth="1px"
            CellPadding="3"
            ForeColor="Black"
            GridLines="Vertical"
            OnRowDeleting="UserGrid_OnRowDeleting"
            OnRowCommand="UserGrid_OnRowCommand">
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:BoundField DataField="UserName" HeaderText="Email"/>
                <asp:ButtonField ButtonType="Button" HeaderText="Departments" Text="Manage Departments" CommandName="ManageDepartments"/>
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
    <%--Add User Modal--%>
    <ajaxToolkit:ModalPopupExtender ID="AddUserPopupExtender"
        runat="server"
        TargetControlID="AddUser" 
        PopupControlID="UserPanel"
        PopupDragHandleControlID="UserPopupHeader"
        Drag="True"
        BackgroundCssClass="modalBackground"
        BehaviorID="AddUserModal">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel runat="server" ID="UserPanel" style="display: none; height: 300px; width: 600px;" CssClass="modalPopup">
        <div style="text-align: center">
            <asp:Label Font-Bold="True" runat="server" ID="UserPopupHeader">Add User</asp:Label>
            <i class="fa fa-times-circle" style="float: right; cursor: pointer;" onclick="closeUserModal()"></i>
        </div>
        <hr />
        <div class="text-danger">
            <span id="userError"><asp:Literal runat="server" ID="UserErrorMessage"/></span>
        </div>
        <div style="text-align: right">
            <asp:Label AssociatedControlID="UserEmailInput" runat="server" CssClass="control-label" style="float: left;">Email:</asp:Label>
            <span id="useEmailInput"><asp:TextBox runat="server" ID="UserEmailInput" MaxLength="200" style="max-width: 400px; width: 400px;"></asp:TextBox></span>
        </div>
        <div style="text-align: right">
            <asp:Label AssociatedControlID="UserPasswordInput" runat="server" CssClass="control-label" style="float: left;">Password:</asp:Label>
            <span id="usePasswordInput"><asp:TextBox runat="server" TextMode="Password" ID="UserPasswordInput" MaxLength="200" style="max-width: 400px; width: 400px;"></asp:TextBox></span>
        </div>
        <div style="text-align: right">
            <asp:Label AssociatedControlID="UserConfirmInput" runat="server" CssClass="control-label" style="float: left;">Confirm Password:</asp:Label>
            <span id="useConfirmInput"><asp:TextBox runat="server" TextMode="Password" ID="UserConfirmInput" MaxLength="200" style="max-width: 400px; width: 400px;"></asp:TextBox></span>
        </div>
        <br/>
        <div class="Controls" style="text-align: right">
            <asp:Button runat="server" ID="addUserSave" Text="Save" OnClick="addUserSave_OnClick"/>
        </div>
    </asp:Panel>
    <script type="text/javascript">
        function closeUserModal() {
            document.getElementById('userError').innerText = "";
            document.getElementById('useEmailInput').firstChild.value = "";
            document.getElementById('usePasswordInput').firstChild.value = "";
            document.getElementById('useConfirmInput').firstChild.value = "";
            $find('AddUserModal').hide();
        }
    </script>
</asp:Content>
