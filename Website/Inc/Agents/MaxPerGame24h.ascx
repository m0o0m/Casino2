<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MaxPerGame24h.ascx.vb" Inherits="SBSAgents.MaxPerGame24h" %>

<div class="panel panel-grey">
    <div class="panel-heading">Max Per Game 24h</div>
    <div class="panel-body">

        <div id="trAgent" runat="server" class="form-group">
            <label class="control-label col-md-6">Agents</label>
            <div class="col-md-6">
                <asp:DropDownList ID="ddlAgents" runat="server" AutoPostBack="true" CssClass="form-control">
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Sport Type</label>
            <div class="col-md-6">
                <asp:DropDownList ID="ddlSportType" runat="server" AutoPostBack="true" CssClass="form-control">
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Time befor game</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtTime" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6">Max per game 24h</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtMaxPerGame" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-6"></label>
            <div class="col-md-6">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" />
            </div>
        </div>

    </div>
</div>
