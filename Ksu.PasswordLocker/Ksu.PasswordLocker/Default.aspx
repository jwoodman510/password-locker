<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Ksu.PasswordLocker._Default"%>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br/>
    <div class="row" style="padding-top: 10px;">
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
    <div class="jumbotron">
        <h1 class="fa fa-exclamation-triangle">Sorry!</h1>
        <p class="lead">This page is currently under construction...</p>
    </div>
</asp:Content>
