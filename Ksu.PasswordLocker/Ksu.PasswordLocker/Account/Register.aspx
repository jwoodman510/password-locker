<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Ksu.PasswordLocker.Account.Register" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>
    
    <div class="form-horizontal">
        <h4>Create a new account</h4>
        <hr />
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <div class="form-group">
          <div class="col-md-10">
            <asp:Label runat="server" AssociatedControlID="Company" CssClass="control-label" style="width:200px;text-align:left">Company</asp:Label>
            <asp:TextBox runat="server" ID="Company" CssClass="form-control" MaxLength="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="Company"
                CssClass="text-danger" ErrorMessage="The company field is required." />
          </div>
        </div>
        <div class="form-group">
          <div class="col-md-10">
            <asp:Label runat="server" AssociatedControlID="Email" CssClass="control-label" style="width:200px;text-align:left">Email</asp:Label>
              <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
              <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                  CssClass="text-danger" ErrorMessage="The email field is required." />
          </div>
        </div>
        <div class="form-group">
          <div class="col-md-10">
            <asp:Label runat="server" AssociatedControlID="Password" CssClass="control-label" style="width:200px;text-align:left">Password</asp:Label>
            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                CssClass="text-danger" ErrorMessage="The password field is required." />
          </div>
        </div>
        <div class="form-group">
          <div class="col-md-10">
            <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="control-label" style="width:200px;text-align:left">Confirm password</asp:Label>
            <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword"
                CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
            <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
          </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" OnClick="CreateUser_Click" Text="Register" CssClass="btn btn-default" />
            </div>
        </div>
    </div>
</asp:Content>
