<%@ Control Language="VB" AutoEventWireup="false" CodeFile="accountStatus.ascx.vb"
    Inherits="SBSAgents.Inc_Agents_accountStatus" %>
    
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<table class="table table-hover table-bordered">
    <tr class="tableheading">
        <td colspan="3">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            Players
        </td>
        <td>
            :
        </td>
        <td align="right">
            <cc1:CDropDownList ID="ddlPlayers" runat="server" CssClass="textInput" hasOptionalItem="true"
                    AutoPostBack="true" />
        </td>
    </tr>
    <tr>
        <td>
            Credit Limit
        </td>
        <td>
            :
        </td>
        <td align="right">
            <asp:Label ID="lblCreditLimit" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Available Balance
        </td>
        <td>
            :
        </td>
        <td align="right">
            <asp:Label ID="lblAvailableBalance" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Last Login Date
        </td>
        <td>
            :
        </td>
        <td align="right">
            <asp:Label ID="lblLastLoginDate" runat="server" />
        </td>
    </tr>
</table>
