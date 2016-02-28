<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.Users" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <div>
        <button runat="server" ID="AddUser" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add User</span>
        </button>
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
