<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TeaserRuleEdit.ascx.vb"
    Inherits="SBCSuperAdmin.TeaserRuleEdit" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<table class="table table-hover" >
    <tr>
        <td align="left" colspan="4">
            <strong>Teaser Rule</strong>
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Name
        </td>
        <td colspan="3">
            <asp:TextBox ID="txtName" CssClass="textInput" MaxLength="50" runat="server" Width="200" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            From Teams
        </td>
        <td>
            <wlb:CDropDownList ID="ddlMinTeam" runat="server" CssClass="textInput" hasOptionalItem="true"
                OptionalText="" OptionalValue="" />
        </td>
        <td class="fieldTitle">
            To Teams
        </td>
        <td>
            <wlb:CDropDownList ID="ddlMaxTeam" runat="server" CssClass="textInput" hasOptionalItem="true"
                OptionalText="" OptionalValue="" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            Basketball Point
        </td>
        <td>
            <wlb:CDropDownList ID="ddlBasketballPoint" runat="server" CssClass="textInput" hasOptionalItem="true"
                OptionalText="" OptionalValue="" />
        </td>
        <td class="fieldTitle">
            Football Point
        </td>
        <td>
            <wlb:CDropDownList ID="ddlFootballPoint" runat="server" CssClass="textInput" hasOptionalItem="true"
                OptionalText="" OptionalValue="" />
        </td>
    </tr>
    <tr>
        <td class="fieldTitle">
            <b>Ties Lose?</b>
        </td>
        <td colspan="3">
            <asp:CheckBox ID="chkIsTiesLose" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td colspan="3">
            <asp:Button ID="btnSave" Text="Save" runat="server" CssClass="btn btn-primary" />
            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default" Text="Cancel" />
        </td>
    </tr>
</table>
