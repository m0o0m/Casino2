<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerJuiceControl.ascx.vb" Inherits="SBS_Inc_PlayerJuiceControl" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="panel panel-grey">
    <div class="panel-heading">Player Juice Control</div>
    <div class="panel-body">
        <div id="trAgent" runat="server" class="form-group">
            <label class="control-label col-md-3">Agent</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlAgents" runat="server" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control">
                </wlb:CDropDownList>
            </div>
        </div>
        
        <div class="form-group">
            <label class="control-label col-md-3">Player</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlPlayer" runat="server" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control">
                </wlb:CDropDownList>
            </div>
        </div>

        <div class="form-group">
            <label class="control-label col-md-3">Sport Type</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlSportType" runat="server" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control">
                </wlb:CDropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3">Game Type</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlGameType" runat="server" AutoPostBack="true" hasOptionalItem="True" OptionalText="All" OptionalValue="" CssClass="form-control">
                </wlb:CDropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3">Bet Type</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlBetType" runat="server" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control">
                </wlb:CDropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3">Context</label>
            <div class="col-md-6">
                <wlb:CDropDownList ID="ddlContext" runat="server" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control">
                    <asp:ListItem>Current</asp:ListItem>
                    <asp:ListItem>1H</asp:ListItem>
                    <asp:ListItem>2H</asp:ListItem>
                </wlb:CDropDownList>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3">Juice</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtJuice" MaxLength="3" CssClass="form-control" runat="server" onkeypress="javascript:return inputNumber(this,event, true);"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3"></label>
            <div class="col-md-6">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" />
            </div>
        </div>
    </div>
</div>