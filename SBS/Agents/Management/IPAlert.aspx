<%@ Page Language="VB" MasterPageFile="../Agents.master"
    AutoEventWireup="false" CodeFile="IPAlert.aspx.vb" Inherits="SBSAgents.IPAlert" %>

<%@ Register Src="~/Inc/Reports/IPAlert.ascx" TagName="IPAlert" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
     <div style="padding-top: 20px">
        <table>
            <tr>
                <td>
                    <div style="margin-left: 190px;">
                        <uc:IPAlert ID="ucIPAlert" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>    
</asp:Content>
