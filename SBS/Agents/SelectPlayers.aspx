<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="SelectPlayers.aspx.vb" Inherits="SBSPlayer.SelectPlayers" Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">

    <div class="panel panel-grey">
        <div class="panel-heading"></div>
        <div class="panel-body">
            <div class="form-group">
                <label class="control-label col-md-3">Select Player to bet game</label>
                <div class="col-md-4">
                    <wlb:CDropDownList ID="ddlPlayers" runat="server" CssClass="form-control" hasOptionalItem="false" AutoPostBack="false" />
                </div>
                <div class="col-md-4">
                    <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

