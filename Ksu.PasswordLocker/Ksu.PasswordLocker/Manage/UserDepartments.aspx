<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDepartments.aspx.cs" Inherits="Ksu.PasswordLocker.Manage.UserDepartments" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Styles/styles.css" rel="stylesheet" type="text/css"/>
    <div class="row">
        <asp:Label ID="UserName" runat="server" Text="" stye="font-size:24;font-weight:bold;"></asp:Label>
    </div>
    <div class="row">
        <div class="col-lg-5 col-md-5 col-sm-6 col-sm-6">
            <asp:Panel runat="server" DefaultButton="AllSearchTextButton">
                <asp:TextBox ID="AllSearchText" runat="server" CssClass="modalTextInput"></asp:TextBox>
                <asp:Button ID="AllSearchTextButton" runat="server" Text="Search" style="display:none;" OnClick="AllSearchTextButton_OnClick" />
            </asp:Panel>
            <div style="padding-top: 20px;">
                <asp:GridView runat="server" ID="AllGrid"
                    AutoGenerateColumns="False"
                    DataKeyNames="DepartmentId"
                    BackColor="White"
                    BorderColor="#999999"
                    BorderStyle="Solid"
                    BorderWidth="1px"
                    CellPadding="3"
                    ForeColor="Black"
                    GridLines="Vertical"
                    Width="304px"
                    OnRowCommand="AllGrid_OnRowCommand">
                    <AlternatingRowStyle BackColor="#CCCCCC" />
                    <Columns>
                        <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                        <asp:ButtonField HeaderText="" Text="Add"  CommandName="AddDepartment">
                        <ItemStyle Font-Underline="True" />
                        </asp:ButtonField>
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
        </div>
        <div class="col-lg-2 col-md-2 hidden-sm hidden-xs"></div>
        <div class="col-lg-5 col-md-5 col-sm-6 col-xs-6">
            <asp:Panel runat="server" DefaultButton="AddedSearchTextButton">
                <asp:TextBox ID="AddedSearchText" runat="server" CssClass="modalTextInput"></asp:TextBox>
                <asp:Button ID="AddedSearchTextButton" runat="server" Text="Search" style="display:none;" OnClick="AddedSearchTextButton_OnClick" />
            </asp:Panel>
            <div style="padding-top: 20px;">
                <asp:GridView runat="server" ID="AddedGrid"
                    AutoGenerateColumns="false"
                    DataKeyNames="DepartmentId"
                    BackColor="White"
                    BorderColor="#999999"
                    BorderStyle="Solid"
                    BorderWidth="1px"
                    CellPadding="3"
                    ForeColor="Black"
                    GridLines="Vertical"
                    OnRowDeleting="AddedGrid_OnRowDeleting" Width="302px">
                    <AlternatingRowStyle BackColor="#CCCCCC" />
                    <Columns>
                        <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
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
        </div>
    </div>
</asp:Content>