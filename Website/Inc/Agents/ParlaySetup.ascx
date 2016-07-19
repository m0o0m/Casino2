<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ParlaySetup.ascx.vb" Inherits="SBSAgents.ParlaySetup" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="row">
    <div id="trAgents" runat="server" visible="false" class="form-group">
        <label class="control-label col-md-2">Agents</label>
        <div class="col-md-2">
            <wlb:CDropDownList runat="server" ID="ddlAgents" CssClass="form-control" AutoPostBack="true" hasOptionalItem="true" OptionalText="" OptionalValue=""></wlb:CDropDownList>
        </div>
    </div>
</div>
<div class="row">
    <div class="form-group">
        <label class="control-label col-md-2">Max Selection</label>
        <div class="col-md-2">
            <asp:TextBox ID="txtMaxSelection" runat="server" MaxLength="10" CssClass="form-control"
                onkeypress="javascript:return inputNumberOnly(event);" onblur="javascript:formatNumber(this,0);"></asp:TextBox>
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="btn btn-primary" />
        </div>
    </div>
</div>
<div class="mbxl"></div>
<div class="row">
    <div class="form-group">
        <label class="control-label col-md-2">Round Robin Allowed</label>
        <div class="col-md-2">
            <asp:CheckBox ID="chkRoundRobin" runat="server" />
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnSaveRoundRobin" runat="server" Text="Save" CssClass="btn btn-primary" />
        </div>
    </div>
</div>
