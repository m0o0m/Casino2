<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DisplayRule.aspx.vb" 
    MasterPageFile="~/SBS/Agents/Agents.master"
    Inherits="SBSAgents.SBS_Agents_Management_Settings_DisplayRule" %>

<%@ Register Src="~/Inc/TeaserAllow.ascx" TagName="TeaserAllow" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/GamePartTimeDisplaySetup.ascx" TagName="GamePartTimeDisplaySetup"
    TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/TimeLineOff.ascx" TagName="TimeLineOff" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/QuarterDisplaySetup.ascx" TagName="TeamTotalDisplaySetup" TagPrefix="uc3" %>


<asp:content id="Content2" contentplaceholderid="cphBody" runat="Server">
    <div class="col-md-6">
        <div class="panel panel-grey">
            <div class="panel-heading">1H , 2H & Teamtotal Display Setup</div>
            <div class="panel-body">
                <uc1:GamePartTimeDisplaySetup ID="ucGamePartTimeDisplaySetup" runat="server" />
            </div>
        </div>
        
        <uc3:teamtotaldisplaysetup id="TeamTotalDisplaySetup1" runat="server" />
        <div class="panel panel-grey">
            <div class="panel-heading">Teaser Allow Display Setup</div>
            <div class="panel-body">
                <uc1:TeaserAllow ID="ucTeaserAllow" runat="server" />
            </div>
        </div>
    </div>
    <div class="col-md-6">
        <div class="panel panel-grey">
            <div class="panel-heading">Full Game Display Setup</div>
            <div class="panel-body">
                <uc1:TimeLineOff ID="ucTimeLineOff" runat="server" ContextDisPlay="Current" />
            </div>
        </div>
        <div class="panel panel-grey">
            <div class="panel-heading">1H Display Setup</div>
            <div class="panel-body">
                <uc1:TimeLineOff ID="TimeLineOff1" runat="server" ContextDisPlay="FirstHalf" />
            </div>
        </div>
    </div>


</asp:content>
