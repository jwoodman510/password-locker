<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Departments.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.Departments" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Departments</h1>
    <br/>
    <div class="row">
        <button runat="server" ID="AddDepartment" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add Department</span>
        </button>
    </div>
    <div class="row">
    <asp:Panel runat="server" DefaultButton="SearchTextButton">
        <asp:TextBox ID="SearchText" runat="server" style="float:right;" CssClass="modalTextInput"></asp:TextBox>
        <asp:Button ID="SearchTextButton" runat="server" Text="Search" style="display:none;" OnClick="SearchTextButton_Click" />
    </asp:Panel>
    </div>
    <div class="row text-danger"><h4><asp:Literal runat="server" ID="GridError"></asp:Literal></h4></div>
    <div class="row" style="padding-top: 20px;">
        <asp:GridView runat="server" ID="DeptGrid"
            AutoGenerateColumns="False"
            Visible="False"
            DataKeyNames="DepartmentId"
            Width="1258px"
            BackColor="White"
            BorderColor="#999999"
            BorderStyle="Solid"
            BorderWidth="1px"
            CellPadding="3"
            ForeColor="Black"
            GridLines="Vertical"
            OnRowDeleting="DeptGrid_OnRowDeleting"
            OnRowUpdating="DeptGrid_OnRowUpdating"
            OnRowEditing="DeptGrid_OnRowEditing"
            OnRowCancelingEdit="DeptGrid_OnRowCancelingEdit"
            OnRowCommand="DeptGrid_OnRowCommand">
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:BoundField DataField="DepartmentName" HeaderText="Name"/>
                <asp:ButtonField ButtonType="Button" HeaderText="Servers" Text="Manage Servers" CommandName="ManageServers"/>
                <asp:ButtonField ButtonType="Button" HeaderText="Users" Text="Manage Users" CommandName="ManageUsers"/>
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
    <script type="text/javascript">
        function closeDepartmentModal() {
            document.getElementById('depError').innerText = "";
            document.getElementById('depNameInput').firstChild.value = "";
            $find('AddDepartmentModal').hide();
        }
    </script>
</asp:Content>