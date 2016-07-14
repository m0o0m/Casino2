<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="NewAgentPositionReport.aspx.vb" Inherits="SBS_Agents_Management_NewAgentPositionReport" %>
<%@ Register Src="~/Inc/Reports/NewAgentPositionReport.ascx" TagName="NewAgentPositionReport"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">
<style type="text/css">
        #LineType
        {
            color: #f7a30b;
        }
    </style>
<div style="padding-top:0px">
    <table>
        <tr>
            <td>
                <uc:NewAgentPositionReport ID="ucNewAgentPositionReport" runat="server" Title="AgentPositionReport" />
            </td>
        </tr>
    </table>
    </div>
</asp:Content>

