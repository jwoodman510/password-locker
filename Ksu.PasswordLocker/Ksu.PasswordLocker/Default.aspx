<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Ksu.PasswordLocker._Default"%>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <div>
        <button runat="server" ID="AddDepartment" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add Department</span>
        </button>
        <button runat="server" ID="AddServer" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add Server</span>
        </button>
        <button runat="server" ID="AddUser" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add User</span>
        </button>
        <asp:Label runat="server" style="float: right; padding-right: 5px;" ID="RoleName" Font-Bold="True">Logged in as: Anonymous User</asp:Label>
    </div>

    <div class="jumbotron">
        <h1 class="fa fa-exclamation-triangle">Sorry!</h1>
        <p class="lead">This page is currently under construction...</p>
    </div>

    <%--Add Department Modal--%>
    <ajaxToolkit:ModalPopupExtender ID="AddDepartmentPopupExtender"
        runat="server"
        TargetControlID="AddDepartment" 
        PopupControlID="DepartmentPanel"
        PopupDragHandleControlID="DepPopupHeader"
        Drag="True"
        BackgroundCssClass="modalBackground"
        BehaviorID="AddDepartmentModal">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel runat="server" ID="DepartmentPanel" style="display:none" CssClass="modalPopup">
        <div style="text-align: center">
            <asp:Label Font-Bold="True" runat="server" ID="DepPopupHeader">Add Department</asp:Label>
            <i class="fa fa-times-circle" style="float: right; cursor: pointer;" onclick="closeDepartmentModal()"></i>
        </div>
        <hr />
        <div class="text-danger">
            <span id="depError"><asp:Literal runat="server" ID="DepErrorMessage"/></span>
        </div>
        <div>
            <asp:Label AssociatedControlID="DepartmentNameInput" runat="server" CssClass="control-label">Name:</asp:Label>
            <span id="depNameInput"><asp:TextBox runat="server" ID="DepartmentNameInput" MaxLength="200" CssClass="modalTextInput"></asp:TextBox></span>
        </div>
        <br/>
        <div class="Controls" style="text-align: right">
            <asp:Button runat="server" ID="addDepartmentSave" Text="Save" OnClick="addDepartmentSave_OnClick"/>
        </div>
    </asp:Panel>

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
        
        function closeDepartmentModal() {
            document.getElementById('depError').innerText = "";
            document.getElementById('depNameInput').firstChild.value = "";
            $find('AddDepartmentModal').hide();
        }

        function closeServerModal() {
            document.getElementById('servError').innerText = "";
            document.getElementById('servNameInput').firstChild.value = "";
            $find('AddServerModal').hide();
        }

        function closeUserModal() {
            document.getElementById('userError').innerText = "";
            document.getElementById('useEmailInput').firstChild.value = "";
            document.getElementById('usePasswordInput').firstChild.value = "";
            document.getElementById('useConfirmInput').firstChild.value = "";
            $find('AddUserModal').hide();
        }
    </script>
</asp:Content>
