<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GameCircledSettings.ascx.vb"
    Inherits="SBCAgents.Inc_GameCircledSettings" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wbl" %>

<div class="panel panel-grey">
    <div class="panel-heading">Total Allowed For Circled Games</div>
    <div class="panel-body">

        <div runat="server" id="trAgents" class="form-group">
            <label class="control-label col-md-3">Agents</label>
            <div class="col-md-6">
                <wbl:CDropDownList runat="server" ID="ddlAgents" hasOptionalItem="false" AutoPostBack="true" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3">Straight</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtStraight" Width="70" CssClass="form-control" runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                    Style="text-align: right"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3">Parlay & Reverse</label>
            <div class="col-md-6">
                <asp:TextBox ID="txtPnR" Width="70" CssClass="form-control" runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                    Style="text-align: right"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3"></label>
            <div class="col-md-6">
                <asp:Button ID="btnSaveCircled" runat="server" CssClass="btn btn-primary"
                    Text="Save" />
            </div>
        </div>

    </div>
</div>
