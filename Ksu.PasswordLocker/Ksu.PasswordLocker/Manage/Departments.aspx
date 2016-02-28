<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Departments.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.Departments" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <div>
        <button runat="server" ID="AddDepartment" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add Department</span>
        </button>
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