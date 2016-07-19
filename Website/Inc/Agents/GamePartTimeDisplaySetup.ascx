<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GamePartTimeDisplaySetup.ascx.vb" Inherits="SBSAgents.GamePartTimeDisplaySetup" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="panel">
    <%--<div class="panel-heading"></div>--%>
    <div class="panel-body">
        <div class="form-group" runat="server" id="trAgents" visible="false">
            <label class="control-label col-md-6">Agents</label>
            <div class="col-md-6">
                <asp:DropDownList runat="server" ID="ddlAgents" AutoPostBack="true" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Game Type</label>
            <div class="col-md-6">
                <wlb:CDropDownList runat="server" ID="ddlGameType" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">1H Off Line</label>
            <div class="col-md-6">
                <asp:CheckBox ID="chk1HOFF" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">2H Off Line</label>
            <div class="col-md-6">
                <asp:CheckBox ID="chk2HOFF" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Team Total Off Line</label>
            <div class="col-md-6">
                <asp:CheckBox ID="chkTeamtotalOFF" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6"></label>
            <div class="col-md-6">
                <asp:Button ID="btnSaveOffLine" runat="server" Text="Save" CssClass="btn btn-primary" />
            </div>
        </div>
    </div>
</div>
