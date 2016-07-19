<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TeaserAllow.ascx.vb" Inherits="SBCSuperAdmins.TeaserAllow" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<div class="form-group">
    <div class="col-lg-12" style="text-align: right;">
        <asp:Button ID="btnUpdate" runat="server" Text="Save" CssClass="btn btn-primary" Style="vertical-align: middle;" />
    </div>
</div>

<div class="form-group" runat="server" id="trAgent">
    <label class="control-label col-md-6">Agents</label>
    <div class="col-md-6">
        <asp:DropDownList ID="ddlAgents" runat="server" AutoPostBack="true" CssClass="form-control">
        </asp:DropDownList>
    </div>
</div>

<table border="0" id="TeaserType" runat="server" class="table table-hover">
    <tr class="offering_pair_odd">
        <td>
            <asp:CheckBox ID="chkTeaser46" runat="server" />
            4/6-Point Teaser
        </td>
        <td></td>
    </tr>
    <tr class="offering_pair_even">
        <td>
            <asp:CheckBox ID="chkTeaser4565" runat="server" />
            4½/6½Point Teaser 
        </td>
        <td></td>
    </tr>
    <tr class="offering_pair_odd">
        <td>
            <asp:CheckBox ID="chkTeaser57" runat="server" />
            5/7-Point Teaser 
        </td>
        <td></td>
    </tr>
    <tr class="offering_pair_even">
        <td>
            <asp:CheckBox ID="chkTeaser5575" runat="server" />
            <nobr>5½/7½-Point Teaser</nobr>
        </td>
        <td></td>
    </tr>
    <tr class="offering_pair_odd">
        <td>
            <nobr><asp:CheckBox ID="chkTeaser38" runat="server" />3T 8-10 SP-Point Teaser</nobr>
        </td>
        <td></td>
    </tr>
    <tr class="offering_pair_even">
        <td>
            <nobr><asp:CheckBox ID="chkTeaser413" runat="server" />4T 13 Point SP Teaser</nobr>
        </td>
        <td></td>
    </tr>
</table>


