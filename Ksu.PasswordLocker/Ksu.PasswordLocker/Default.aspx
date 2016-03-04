<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Ksu.PasswordLocker._Default"%>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <div class="row">
        <asp:Label runat="server" style="padding-left: 15px;" ID="RoleName" Font-Bold="True">Logged in as: Anonymous User</asp:Label>
    </div>
    <div class="row" style="padding-top: 10px;"></div>
    <div class="row">
        <div class="col-lg-2 col-md-2 col-sm-3 col-xs-3">
            <button runat="server" ID="ManageDepartments" class="btn btn-mini" Visible="False">
                <i class="fa fa-exchange"></i>
                <span>Departments</span>
            </button>
        </div>
        <div class="col-lg-2 col-md-2 col-sm-3 col-xs-3">
            <button runat="server" ID="ManageServers" class="btn btn-mini" Visible="False">
                <i class="fa fa-exchange"></i>
                <span>Servers</span>
            </button>
        </div>
        <div class="col-lg-8 col-md-8 col-sm-6 col-xs-6">
            <button runat="server" ID="ManageUsers" class="btn btn-mini" Visible="False">
                <i class="fa fa-exchange"></i>
                <span>Users</span>
            </button>
        </div>
    </div>
    <div class="row" style="padding-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <h1>Server Logins</h1>
        </div>
    </div>
    <div class="row" style="padding-top: 10px;">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <button runat="server" ID="AddLogin" class="btn btn-mini">
                    <i class="fa fa-plus-square"></i>
                    <span>Create Login</span>
            </button>
        </div>
    </div>
    <div class="row" style="padding-top: 15px;">
        <asp:Panel runat="server" DefaultButton="SearchTextButton">
            <asp:TextBox ID="SearchText" runat="server" CssClass="modalTextInput"></asp:TextBox>
            <asp:Button ID="SearchTextButton" runat="server" Text="Search" style="display:none;" OnClick="SearchTextButton_OnClick" />
        </asp:Panel>
    </div>
    <div class="row text-danger"><h4><asp:Literal runat="server" ID="GridError"></asp:Literal></h4></div>
    <div class="row">
        <asp:GridView runat="server" ID="LoginGrid"
            AutoGenerateColumns="False"
            DataKeyNames="ServerLoginId"
            Width="800px"
            BackColor="White"
            BorderColor="#999999"
            BorderStyle="Solid"
            BorderWidth="1px"
            CellPadding="3"
            ForeColor="Black"
            GridLines="Vertical"
            OnRowDeleting="LoginGrid_OnRowDeleting"
            OnRowUpdating="LoginGrid_OnRowUpdating"
            OnRowEditing="LoginGrid_OnRowEditing"
            OnRowCancelingEdit="LoginGrid_OnRowCancelingEdit">
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:BoundField DataField="UserName" HeaderText="User Name"/>
                <asp:BoundField DataField="Password" HeaderText="Password"/>
                <asp:BoundField DataField="DepartmentName" HeaderText="Department" ReadOnly="True"/>
                <asp:BoundField DataField="ServerName" HeaderText="Server" ReadOnly="True"/>
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
    <%--Add Login Modal--%>
    <ajaxToolkit:ModalPopupExtender ID="AddLoginPopupExtender"
        runat="server"
        TargetControlID="AddLogin" 
        PopupControlID="LoginPanel"
        PopupDragHandleControlID="LoginPopupHeader"
        Drag="True"
        BackgroundCssClass="modalBackground"
        BehaviorID="AddLoginModal">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel runat="server" ID="LoginPanel" style="display: none; height: 375px; width: 650px;" CssClass="modalPopup">
        <div style="text-align: center">
            <asp:Label Font-Bold="True" runat="server" ID="LoginPopupHeader">Create Login</asp:Label>
            <i class="fa fa-times-circle" style="float: right; cursor: pointer;" onclick="closeUserModal()"></i>
        </div>
        <hr />
        <div>
            <asp:Label runat="server" Font-Bold="True">Server:</asp:Label>
            <asp:DropDownList runat="server" ID="ServerDropDown" DataTextField="ServerName" DataValueField="ServerId"></asp:DropDownList>
            <asp:Label runat="server" Font-Bold="True" style="padding-left: 10px;">Department:</asp:Label>
            <asp:DropDownList runat="server" ID="DepartmentDropDown" DataTextField="DepartmentName" DataValueField="DepartmentId"></asp:DropDownList>
        </div>
        <hr />
        <div style="text-align: right; padding-top: 20px;">
            <asp:Label AssociatedControlID="UserNameInput" runat="server" CssClass="control-label" style="float: left;">User Name:</asp:Label>
            <span id="useNameInput"><asp:TextBox runat="server" ID="UserNameInput" MaxLength="200" style="max-width: 400px; width: 400px;"></asp:TextBox></span>
        </div>
        <div style="text-align: right; padding-top: 10px;">
            <asp:Label AssociatedControlID="UserPasswordInput" runat="server" CssClass="control-label" style="float: left;">Password:</asp:Label>
            <span id="useNamePasswordInput"><asp:TextBox runat="server" TextMode="Password" ID="UserPasswordInput" MaxLength="200" style="max-width: 400px; width: 400px;"></asp:TextBox></span>
        </div>
        <div style="text-align: right; padding-top: 10px;">
            <asp:Label AssociatedControlID="UserConfirmInput" runat="server" CssClass="control-label" style="float: left;">Confirm Password:</asp:Label>
            <span id="useNameConfirmInput"><asp:TextBox runat="server" TextMode="Password" ID="UserConfirmInput" MaxLength="200" style="max-width: 400px; width: 400px;"></asp:TextBox></span>
        </div>
        <br/>
        <hr />
        <div class="Controls" style="text-align: right">
            <asp:Button runat="server" ID="addLoginSave" Text="Save" OnClick="addLoginSave_OnClick"/>
        </div>
        <div class="text-danger">
            <span id="userNameError"><asp:Literal runat="server" ID="LoginErrorMessage"/></span>
        </div>
    </asp:Panel>
    <script type="text/javascript">
        function closeUserModal() {
            document.getElementById('userNameError').innerText = "";
            document.getElementById('useNameInput').firstChild.value = "";
            document.getElementById('useNamePasswordInput').firstChild.value = "";
            document.getElementById('useNameConfirmInput').firstChild.value = "";
            $find('AddLoginModal').hide();
        }
    </script>
</asp:Content>
