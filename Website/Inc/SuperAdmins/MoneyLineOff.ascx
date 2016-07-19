<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MoneyLineOff.ascx.vb"
    Inherits="SBCSuperAdmins.MoneyLineOff" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<table class="table table-hover">
    <tr>
        <td>
            Sport Type
        </td>
        <td>
            <wlb:CDropDownList ID="ddlSportType" runat="server" OptionalText="" OptionalValue=""
                AutoPostBack="true" hasOptionalItem="true" CssClass="textInput">
            </wlb:CDropDownList>
        </td>
    </tr>
    <tr>
        <td>
            Away Spread greater than
        </td>
        <td>
            <asp:TextBox ID="txtAwaySpreadGT" onkeypress="javascript:return inputNumber(this,event, true);"
                Width="50" CssClass="textInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            or Away Spread less than
        </td>
        <td>
            <asp:TextBox ID="txtAwaySpreadLT" onkeypress="javascript:return inputNumber(this,event, true);"
                Width="50" CssClass="textInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Inactive money line when away spread over the limitation.
        </td>
    </tr>
</table>
