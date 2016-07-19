<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentEdit.ascx.vb" Inherits="SBCSuperAdmin.AgentEdit" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>

<script type="text/javascript">
    function CheckPreset(objSource, objID) {
        if (objSource.checked) {
            var o = document.getElementById(objID);
            o.checked = false;
        }
    }
</script>

<table class="table table-hover">         
    <tr>
        <td class="tableheading" align="left" colspan="2" style="padding-left:5px;">
            Agent Info
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
            <asp:UpdatePanel ID="pnl1" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="txtName" EventName="TextChanged" />
                </Triggers>
                <ContentTemplate>
                    <asp:TextBox ID="txtLogin" CssClass="textInput" MaxLength="50" runat="server" Width="142px" />
                </ContentTemplate>
            </asp:UpdatePanel>
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
            <uc1:passwordEditor runat="server" ID="psdPassword" Required="false" 
                HorizontalAlign="false" ShowPassword="True" 
                TextVisible="false" 
                SetCheckCapsLockClientFunction="capsLock(event, 'divCapsLock')" />
            <asp:HiddenField ID="hfdPassword" runat="server" />
            <div id="divCapsLock" style="color: red; display: none;">
                Caps Lock is ON.</div>
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Is Child Of
        </td>
        <td>
            <wlb:CDropDownList ID="ddlPAgents" runat="server" CssClass="textInput" hasOptionalItem="false" AutoPostBack="true" />
            <asp:HiddenField ID="hfPAgentID" runat="server" />
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
            P/L Percentage
        </td>
        <td>
            <asp:TextBox ID="txtProfitPercentage" runat="server" CssClass="textInput" onkeypress="javascript:return inputNumberOnly(event);"
                MaxLength="3" Style="text-align: right; width: 50px"></asp:TextBox>&nbsp;%
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Gross To Net %
        </td>
        <td>
            <asp:TextBox ID="txtGrossPercentage" runat="server" CssClass="textInput" onkeypress="javascript:return inputNumberOnly(event);"
                MaxLength="3" Style="text-align: right; width: 50px"></asp:TextBox>&nbsp;%
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Assign Group Letters
        </td>
        <td>
            <asp:TextBox ID="txtSpecialKey" runat="server" CssClass="textInput" MaxLength="4"
                Style="width: 50px"></asp:TextBox>&nbsp;(max 4)
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Starting Number
        </td>
        <td>
            <asp:TextBox ID="txtCurrentPlayerNumber" runat="server" CssClass="textInput" onkeypress="javascript:return inputNumberOnly(event);"
                MaxLength="5" Style="text-align: right; width: 50px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Preset Player Accounts
        </td>
        <td>
            <asp:CheckBox ID="chk10Acc" onclick="javascript:CheckPreset(this,chk20Acc)" Text="10 Accts"
                runat="server" />
            <asp:CheckBox ID="chk20Acc" onclick="javascript:CheckPreset(this, chk10Acc)" Text="20 Accts"
                runat="server" />
        </td>
    </tr>
    <tr>
        <td>
          Enable Player Template
        </td>
        <td>
            <asp:CheckBox ID="chkEnablePlayerTemplate" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
     <tr>
        <td>
          Enable Player Block
        </td>
        <td>
            <asp:CheckBox ID="chkEnablePlayerBlock" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr>
        <td>
          Enable Betting Profile
        </td>
        <td>
            <asp:CheckBox ID="chkIsEnableBettingProfile" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr>
        <td>
          Max Credit Limit
        </td>
        <td>
            <asp:CheckBox ID="chkMaxCreditSetting" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr>
        <td>
          Sub-Agent Enable
        </td>
        <td>
            <asp:CheckBox ID="chkSubAgentEnable" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr>
        <td>
          Enable Change Bookmaker
        </td>
        <td>
            <asp:CheckBox ID="chkIsEnableChangeBookmaker" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
     <tr id="trCasino" runat="Server">
        <td>
         Casino
        </td>
        <td>
            <asp:CheckBox ID="chkCasino" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr id="trHasGameManagement" runat="Server">
        <td>
         Has GameManagement
        </td>
        <td>
            <asp:CheckBox ID="chkHasGameManagement" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr id="trHasSystemManagement" runat="Server" >
        <td>
         Has User Management 
        </td>
        <td>
            <asp:CheckBox ID="chkHasSystemManagement" runat="server" AutoPostBack="false"  />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Preset Player Template
        </td>
        <td>
            <wlb:CDropDownList ID="ddlTemplates" runat="server" CssClass="textInput" />
        </td>
    </tr>
    <tr id="trRequireChangePass" runat="server" visible="false">
        <td class="fieldTitle">
            <b>Require Change Password?</b>
        </td>
        <td>
            <asp:CheckBox ID="chkRequireChangePass" Font-Bold="true" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            <b>Betting Locked?</b>
        </td>
        <td>
            <asp:CheckBox ID="chkIsBettingLocked" runat="server" />
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
