<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SuperEdit.ascx.vb" Inherits="SBCSuperAdmin.SuperEdit" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>
 <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<style type="text/css">
    .style1
    {
        font-weight: bold;
    }
</style>
<table class="table table-hover">
<colgroup>
    <col align="left" width="60%" />
    <col align="left" width="40%" />
</colgroup>
    <tr>
        <td class="tableheading" align="left" colspan="2" style="padding-left:5px;">
            Super Admin Info
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Name
        </td>
        <td>
            <asp:TextBox ID="txtName" CssClass="textInput" MaxLength="50" runat="server" />
            <asp:ImageButton ID="ibtGenerateLogin" runat="server" ToolTip="Generate login" ImageUrl="~/images/cancel.gif"
                ImageAlign="AbsMiddle" Style="height: 16px" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Login
        </td>
        <td>
            <asp:TextBox ID="txtAdminLogin" CssClass="textInput" MaxLength="50" runat="server"/>
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
            <uc1:passwordEditor runat="server" ID="psdAdminPassword" Required="false" HorizontalAlign="false" ShowPassword="true" 
                TextVisible="false" SetCheckCapsLockClientFunction="capsLock(event, 'divCapsLock')" />
            <asp:HiddenField ID="hfdPassword" runat="server" />
            <div id="divCapsLock" style="color: red; display: none;">
                Caps Lock is ON.</div>
        </td>
    </tr>
    <tr id="tdWagering" runat="server" visible ="false">
        <td>
            Wagering
        </td>
        <td>
            <asp:TextBox ID="txtWagering" runat="server"  CssClass="textInput"></asp:TextBox>
            <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txtWagering" ClearMaskOnLostFocus="False" MaskType="Number" Mask="999-999-9999" />
        </td>
    </tr>
    <tr id="tdCustomerService" visible ="false" runat="server">
        <td>
           Customer Service 
        </td>
        <td>
            <asp:TextBox ID="txtCustomerService" runat="server"  CssClass="textInput"></asp:TextBox>
            <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txtCustomerService" ClearMaskOnLostFocus="False" MaskType="Number" Mask="999-999-9999" />
        </td>
    </tr>
    <tr>
        <td>
            Site URL
        </td>
        <td>
           <wlb:CDropDownList ID="ddlUrls" runat="server" CssClass="textInput" />
        </td>
    </tr>
    <tr>
        <td>
            Site URL Backup
        </td>
        <td>
           <wlb:CDropDownList ID="ddlUrls2" runat="server" CssClass="textInput" />
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
            <b>Locked?</b>
        </td>
        <td>
            <asp:CheckBox ID="chkIsLocked" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="style1">
            Is Manager
        </td>
        <td>
            <asp:CheckBox ID="chkManager" runat="server" />
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
