<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false"
    CodeFile="OpenBets.aspx.vb" Inherits="SBSCallCenterAgents.OpenBets" %>

<%@ Register Src="~/Inc/Callcenter/playerProfile.ascx" TagName="playerProfile" TagPrefix="uc2" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/SBS/Inc/ticketBetsGridAgent.ascx" TagName="ticketBetsGridAgent" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="Server">
    <uc2:playerProfile ID="ucPlayerProfile" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <div id="historypanel" style="width:100%;margin-top:-16px">
             <table border="0" cellpadding="1" cellspacing="0" style="margin-left: 10px; margin-bottom: 4px;"
            id="tablePlayer" runat="server">
            <tr>
                <td style="padding-left: 10px;color:Black">
                    CCAgent :
                </td>
                <td>
                    <cc1:CDropDownList ID="ddlCAgent" runat="server" CssClass="textInput" hasOptionalItem="true"
                        OptionalText="ALL" AutoPostBack="true" />
                </td>
                <td style="width: 10px;color:Black">
                </td>
                <td>
                   <span style="color:Black"> Player:</span>
                    <cc1:CDropDownList ID="ddlPlayers" hasOptionalItem="true" OptionalText="ALL"
                        AutoPostBack="true" runat="server">
                    </cc1:CDropDownList>
                </td>
                <td>
                    <span style="position: relative; bottom: -2px">Context : </span>
                    <cc1:CDropDownList ID="ddlContext" runat="server" CssClass="textInput" AutoPostBack="true"  Width="150px" >
                         <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Current" Value="Current"></asp:ListItem>
                        <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                        <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                        <asp:ListItem Text="Quarters" Value="Q"></asp:ListItem>
                         <asp:ListItem Text="Proposition" Value="Proposition"></asp:ListItem>
                    </cc1:CDropDownList>
                </td>
            </tr>
        </table>
        <asp:UpdatePanel ID="up1" runat="server">
            <ContentTemplate>
                <uc1:ticketBetsGridAgent ID="ucTicketBetsGridAgent" runat="server" AssignRecordingLink="AssignRecording.aspx" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="clear">
        </div>
    </div>
</asp:Content>
