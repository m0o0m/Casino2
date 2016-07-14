<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false" CodeFile="GameManager.aspx.vb" Inherits="SBSCallCenterAgents.SBS_CallCenter_GameManager" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<style type="text/css">
    .MenuItem
    {
        display: block;
        width: 190px;
        height: 25px;
        text-align: center;
        background: url(/SBS/images/tab.png) no-repeat;
        text-align:left;
        margin: 0 auto;
        text-indent:10px;
        margin-top:10px;
    }
   table .MenuItem a
    {
        font-weight:900;
        color:Yellow;
        line-height: 23px;
        text-decoration: none;
    }
    .MenuItem a, .MenuItem a:visited
    {
        color: #222;
        text-decoration: none;
    }
    .MenuItem a:hover
    {
      color:#fff;
    }
</style>
<table width="100%" border="0" style="text-align: center">
    <tr>
        <td>
            <span class="MenuItem">
                <asp:LinkButton ID="lbnLinesMonitor"  runat="server" PostBackUrl="/SBS/CallCenter/LinesMonitor.aspx">Lines Monitor</asp:LinkButton>
            </span>
        </td>
        <td>
            <span class="MenuItem">
                <asp:LinkButton ID="lbnAgentBalanceReports" runat="server" PostBackUrl="/SBS/CallCenter/SetupQuarter.aspx">Setup Quater Lines</asp:LinkButton>
            </span>
        </td>
    </tr>
    <tr>
        <td>
            <span class="MenuItem">
                <asp:LinkButton ID="lbnAgents" runat="server"   PostBackUrl="/SBS/CallCenter/UpdateGameScores.aspx">Update Game Scores</asp:LinkButton>
            </span>
        </td>
        <td>
            <span class="MenuItem">
                <asp:LinkButton ID="lbnPlayerBalanceReports"   runat="server" PostBackUrl="/SBS/CallCenter/UpdateQuarterScores.aspx">Update Quater Scores</asp:LinkButton>
            </span>
        </td>
    </tr>
    </table>

</asp:Content>

