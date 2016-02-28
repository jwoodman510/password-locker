<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Servers.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.Servers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <div>
        <button runat="server" ID="AddServer" class="btn btn-mini" Visible="False">
            <i class="fa fa-plus-square"></i>
            <span>Add Server</span>
        </button>
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
