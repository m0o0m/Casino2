<%@ Control Language="VB" AutoEventWireup="false" CodeFile="passwordEditor.ascx.vb"
    Inherits="SBCWebsite.Inc_PasswordEditor" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!--link href="http://testcases.pagebakers.com/PasswordMeter/passwordmeter.css" rel="stylesheet" type="text/css" /-->
<style>
    .x-form-element .strengthMeter
    {
        border: 1px solid #B5B8C8;
        margin: 3px 0 3px 0;
        background-image: url(/images/passwordmeter_background.gif);
    }
    .x-form-element .strengthMeter-focus
    {
        border: 1px solid #000;
    }
    .x-form-element .scoreBar
    {
        background-image: url(/images/passwordmeter.gif);
        height: 10px;
        width: 0;
        line-height: 1px;
        font-size: 1px;
    }
</style>
<div class="x-form-element" runat="Server" id="divPasswordEditor">
    <asp:Literal runat="server" ID="ltrPasswordCaption">Password</asp:Literal>
    <asp:TextBox CssClass="form-control" ID="txtPassword" runat="server" 
        MaxLength="10"></asp:TextBox>
    <asp:Label ID="lblAlert" runat="server" ForeColor="red" Font-Bold="true" Text="*"
        Visible="false"></asp:Label>
    <cc1:TextBoxWatermarkExtender ID="txtwePassword" runat="server" TargetControlID="txtPassword"
        WatermarkText="Change Password" WatermarkCssClass="watermarked" Enabled="false">
    </cc1:TextBoxWatermarkExtender>
    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required"
        Display="dynamic" ControlToValidate="txtPassword">*</asp:RequiredFieldValidator><asp:CustomValidator
            ID="custvPassword" runat="server" ErrorMessage=" (minimum 3 characters required)"
            Display="dynamic" ControlToValidate="txtPassword" ClientValidationFunction="valPasswordLength">*</asp:CustomValidator>
    <div class="strengthMeter" id="div1" runat="server" title="Password Strengh Meter">
        <div class="scoreBar" style="width: 0" id="div2" runat="server">
        </div>
    </div>
    <span style="position: relative;" runat="server" id="spanConfirmPassword">
        <asp:Literal runat="server" ID="ltrConfirmPasswordCaption">Confirm Password</asp:Literal>
        <asp:TextBox CssClass="form-control" ID="txtPasswordConfirm" runat="server" 
             MaxLength="10"></asp:TextBox>
        <asp:CompareValidator runat="server" Enabled="false" ID="cvPassword" ControlToCompare="txtPassword"
            ControlToValidate="txtPasswordConfirm" Display="dynamic" ErrorMessage="passwords don't not match">*</asp:CompareValidator><asp:CustomValidator
                ID="custvComparePassword" runat="server" ClientValidationFunction="valComparePassword"
                ControlToValidate="txtPassword" Display="dynamic" ErrorMessage="Passwords do not match">*</asp:CustomValidator>
        <asp:ValidationSummary ID="validSummary" runat="server" ShowMessageBox="true" DisplayMode="List"
            ShowSummary="false" />
    </span>
</div>
