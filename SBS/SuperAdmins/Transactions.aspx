<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="Transactions.aspx.vb" Inherits="SBSSuperAdmin.Transactions" %>

<%@ Register Src="~/Inc/Transactions.ascx" TagName="Transactions" TagPrefix="uc" %>
<%@ Register Src="~/Inc/SuperAdmins/MaintenanceCharge.ascx" TagName="MaintenanceCharge"
    TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row mbl">
        <div class="col-lg-12">
            <ul id="generalTab" class="nav nav-tabs responsive hidden-xs hidden-sm">
                <li id ="tTrans" runat="server">
                    <asp:LinkButton runat="server" ID="lbtnTransactions" CommandArgument="AGENT_TRANSACTIONS"
                        Text="Agent Transactions" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id ="tPlayerTrans" runat="server">
                    <asp:LinkButton runat="server" ID="lbtnPlayerTransactions" CommandArgument="PLAYER_TRANSACTIONS"
                        Text="Player Transactions" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li  id ="tMainternaceTrans" runat="server">
                    <asp:LinkButton runat="server" ID="lbtnMaintenanceCharge" CommandArgument="MAINTENANCE_CHARGE"
                        CssClass="selected" Text="Weekly Maintenance Charge" CausesValidation="false"
                        OnClick="lbtTab_Click" />
                </li>
            </ul>
            <uc:Transactions ID="ucTransactions" runat="server" UserType="SuperAdmin" />
            <uc:MaintenanceCharge ID="ucMaintenanceCharge" runat="server" />
        </div>

    </div>
</asp:Content>
