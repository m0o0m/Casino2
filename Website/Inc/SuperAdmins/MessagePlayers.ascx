<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MessagePlayers.ascx.vb" Inherits="Inc_SuperAdmins_MessagePlayers" %>

<div class="row">
    <div class="col-lg-12">
        <label class="col-lg-2 control-label">
            Web One Time
        </label>
        <asp:TextBox ID="txtOneTime" Rows="7" Columns="50" TextMode="MultiLine"
            runat="server" CssClass="form-control"></asp:TextBox>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <label class="col-lg-2 control-label">Web Permanent</label>
        <asp:TextBox ID="txtPermanent" Rows="7" Columns="50" TextMode="MultiLine"
            runat="server" CssClass="form-control"></asp:TextBox>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>
<div class="row">
    <div class="col-lg-12">
        <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="btn btn-primary" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>
