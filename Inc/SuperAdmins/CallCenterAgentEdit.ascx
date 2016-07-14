<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CallCenterAgentEdit.ascx.vb"
    Inherits="SBCSuperAdmin.CallCenterAgentEdit" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>
<table class="table table-hover">
    <tr>
        <td class="tableheading" align="left" colspan="2" style="padding-left:5px;">
            Call Center Agent Info
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Name
        </td>
        <td>
            <asp:TextBox ID="txtName" CssClass="textInput" MaxLength="50" runat="server" AutoPostBack="true" />
            <asp:ImageButton ID="ibtGenerateLogin" runat="server" ToolTip="Generate login" ImageUrl="~/images/cancel.gif"
                ImageAlign="AbsMiddle" Style="height: 16px" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Login
        </td>
        <td>
            <asp:TextBox ID="txtLogin" CssClass="textInput" MaxLength="50" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Password
            <br />
            <br />
            <br />
            Confirm Password
        </td>
        <td class="field">
            <uc1:passwordEditor runat="server" ID="psdPassword" Required="false" HorizontalAlign="false" ShowPassword="true" 
                TextVisible="false" SetCheckCapsLockClientFunction="capsLock(event, 'divCapsLock')" />
            <asp:HiddenField ID="hfdPassword" runat="server" />
            <div id="divCapsLock" style="color: red; display: none;">
                Caps Lock is ON.</div>
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            TimeZone
        </td>
        <td>
            <wlb:CDropDownList ID="ddlTimeZone" runat="server" CssClass="textInput" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Phone Extension
        </td>
        <td>
            <asp:TextBox ID="txtPhoneExt" CssClass="textInput" MaxLength="4" Width="60" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            <b>Locked?</b>
        </td>
        <td>
            <asp:CheckBox ID="chkIsLocked" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Lock Reason
        </td>
        <td>
            <asp:TextBox CssClass="textInput" ID="txtLockReason" TextMode="MultiLine" MaxLength="100"
                Height="50" Width="150" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Button ID="btnSave" Text="Save" runat="server" CssClass="btn btn-primary" />
            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-primary" Text="Cancel" />
        </td>
    </tr>
</table>
