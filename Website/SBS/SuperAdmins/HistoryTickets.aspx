<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="HistoryTickets.aspx.vb" Inherits="SBSSuperAdmin.HistoryTickets" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/SBS/Inc/historyGrid.ascx" TagName="historyGrid" TagPrefix="uc" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    
<table border="0" cellpadding="0" cellspacing="0" style="margin-left: 10px; margin-bottom: 4px;">
        <tr>
            <td valign="bottom" style="padding-right: 10px">
                <span style="font-size: 12px; position: relative; bottom: 0px">Type Of Bet</span>
            </td>
            <td valign="bottom" style="padding-right: 10px">
                <div style="margin-top: 2px">
                    <cc1:CDropDownList ID="ddlTypeOfBet" runat="server" CssClass="textInput" hasOptionalItem="true"
                        OptionalText="All" OptionalValue="All" AutoPostBack="true">
                        <asp:ListItem Text="All" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Internet" Value="Internet"></asp:ListItem>
                        <asp:ListItem Text="Phone" Value="Phone"></asp:ListItem>
                    </cc1:CDropDownList>
                </div>
            </td>
        </tr>
    </table>
    <div style="padding-bottom:10px;width:100%">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc:historyGrid ID="ucHistoryGrid" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
</asp:Content>
