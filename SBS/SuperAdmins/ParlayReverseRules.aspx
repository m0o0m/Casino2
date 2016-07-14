<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" CodeFile="ParlayReverseRules.aspx.vb" Inherits="SBSSuperAdmins.SBC_SuperAdmins_ParlayReverseRules" %>

<%@ Register Src="~/Inc/Agents/ParlayAndReverseRules.ascx" TagName="ParlayAndReverseRules" TagPrefix="uc" %>
<%@ Register Src="~/Inc/SuperAdmins/ParlayPayout.ascx" TagName="ParlayPayout" TagPrefix="uc" %>
<%@ Register Src="~/Inc/Agents/ParlaySetup.ascx" TagName="ParlaySetup" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">

    <div class="row mbl">
        <div class="col-lg-12">
            <ul id="generalTab" class="nav nav-tabs">
                <li id="tParlaySetup" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabParlaySetup" CommandArgument="PARLAY_SETUP" Text="Parlay Setup"
                        CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tParlayAllow" runat="server">
                    <asp:LinkButton runat="server" ID="lbtParlaysAllowance" CommandArgument="PARLAY_ALLOWANCE" CssClass="selected"
                        Text="Parlays Allowance (in Game)" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tBWParlayAllow" runat="server">
                    <asp:LinkButton runat="server" ID="lbtBWParlaysAllowance" CommandArgument="BW_PARLAY_ALLOWANCE" CssClass="selected"
                        Text="Parlays Allowance (b/w Games)" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
                <li id="tParlayPayout" runat="server">
                    <asp:LinkButton runat="server" ID="lbtTabParlayPayout" CommandArgument="PARLAY_PAYOUT" CssClass="selected"
                        Text="Parlays Payout" CausesValidation="false" OnClick="lbtTab_Click" />
                </li>
            </ul>
            <div class="tab-content responsive" style="border: none;">
                <div class="tab-pane fade active in">
                    <uc1:ParlaySetup ID="ucParlaySetup" runat="server" />
                    <br />
                    <%--<uc:ParlaySetup ID="ucParlaySetup" Title="Parlay Setup" runat="server"></uc:ParlaySetup>--%>
                    <uc:ParlayAndReverseRules ID="ucParlaysAllowance" runat="server" />
                    <uc:ParlayPayout ID="ucParlayPayout" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

