<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCPopup.master" AutoEventWireup="false"
    CodeFile="AssignRecording.aspx.vb" Inherits="SBCCallCenterAgents.AssignRecording" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="2" cellspacing="2" style="color: White;">
                <tr>
                    <td width="100px">
                        Call Agents:
                    </td>
                    <td>
                        <wlb:CDropDownList ID="ddlCCAgents" runat="server" CssClass="textInput" 
                        hasOptionalItem="true" OptionalText="All" OptionalValue="" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Date Range:
                    </td>
                    <td nowrap="nowrap">
                        <table>
                            <tr>
                                <td><uc:DateTime ID="ucDateFrom" runat="server" ShowTime="true" ShowCalendar="false" /></td>
                                <td>~</td>
                                <td><uc:DateTime ID="ucDateTo" runat="server" ShowTime="true" ShowCalendar="false" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="textInput" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:DataGrid ID="grdCallLogs" runat="server" Width="98%" AutoGenerateColumns="false"
                            CellPadding="1" CellSpacing="2" CssClass="gamebox" align="center" BorderWidth="2px"
                            BorderColor="#FFFFFF">
                            <HeaderStyle CssClass="tableheading2" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            <AlternatingItemStyle HorizontalAlign="Center" />
                            <FooterStyle CssClass="tableheading2" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateColumn HeaderText="Call Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCallDate" runat="server" />
                                        <asp:HiddenField ID="hfAsteriskID" runat="server" Value='<%#Container.DataItem("uniqueid") %>' />
                                        <asp:HiddenField ID="hfCallDate" runat="server" Value='<%#Container.DataItem("calldate") %>' />
                                        <asp:HiddenField ID="hfCallerID" runat="server" Value='<%#Container.DataItem("callerid") %>' />
                                        <asp:HiddenField ID="hfCallDuration" runat="server" Value='<%#Container.DataItem("duration") %>' />
                                        <asp:HiddenField ID="hfFileName" runat="server" Value='<%#Container.DataItem("filename") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Ext Phone">
                                    <ItemTemplate>
                                        <asp:Label ID="lblExtPhone" runat="server" Text='<%#Container.DataItem("username") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Duration">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDuration" runat="server" Text='<%#Container.DataItem("duration") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="">
                                    <ItemTemplate>
                                        <asp:Button ID="btnListen" runat="server" CommandName="LISTEN" CommandArgument="" Text="Listen" CssClass="textInput" />|
                                        <asp:Button ID="btnSelected" runat="server" CommandName="SELECT" CommandArgument="" Text="Select" CssClass="textInput" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
