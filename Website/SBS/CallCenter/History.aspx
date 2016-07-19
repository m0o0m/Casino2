<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false"
    CodeFile="History.aspx.vb" Inherits="SBSCallCenterAgents.History" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajx" %>
<%@ Register Src="~/SBS/Inc/historyGrid.ascx" TagName="historyGrid" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/DateTime.ascx" TagName="DateTime" TagPrefix="uc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="up1" runat="server">
        <ContentTemplate>
            <div id="historypanel" style="padding: 0; margin: 0 auto;">
                <table id="Table1" cellpadding="0" cellspacing="0" border="0" style="color:Black;margin-left: 10px; text-align: left; margin-bottom: 2px">
                    <tr valign="bottom" >
                        <td width="100px">
                            <span style="font-size: 12px;">CCAgent</span>
                        </td>
                        <td width="450px">
                            <div style="float: left;">
                                <wlb:CDropDownList ID="ddlCAgent" runat="server" CssClass="textInput" hasOptionalItem="true"
                                    AutoPostBack="true" />
                            </div>
                            <div id="Div1" style="margin-right: 10px; text-align: left;">
                                <span style="font-size: 12px; margin-left: 10px; margin-right: 10px">Player</span>
                                <wlb:CDropDownList ID="ddlPlayers" runat="server" CssClass="textInput" hasOptionalItem="true"
                                    AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="dateRange">Date Range:  </span>
                        </td>
                        <td>    
                            <span style="float:left;margin-top:4px;margin-right:10px;">From</span>
                            <div style="margin-bottom:2px;margin-top:4px">
                            <asp:TextBox CssClass="textInput" ID="txtDateFrom" runat="server" Width="60" AutoPostBack="true" />
                            <ajx:CalendarExtender ID="ce4" runat="server" TargetControlID="txtDateFrom" />
                            <span style="margin-left: 10px; margin-right: 10px">To</span>
                            <asp:TextBox CssClass="textInput" ID="txtDateTo" runat="server" Width="60" AutoPostBack="true" />
                            <ajx:CalendarExtender ID="ce5" runat="server" TargetControlID="txtDateTo" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: left">
                            <div style="margin-bottom: 2px; padding: 0;">
                                <span>Has Phone Log</span>
                            </div>
                        </td>
                        <td>
                            <div style="margin-left:-6px">
                            <asp:RadioButton GroupName="HasPhoneLog" ID="rdYes" runat="server" AutoPostBack="true" /><span style="position:relative; bottom:3px">Yes</span>
                            <asp:RadioButton GroupName="HasPhoneLog" ID="rdNo" runat="server" AutoPostBack="true" /><span style="line-height:17px;vertical-align:top">No</span>
                            <asp:RadioButton GroupName="HasPhoneLog" Checked="true" ID="rdDontCare" runat="server" AutoPostBack="true" /><span style="line-height:17px;vertical-align:top">Don't Care</span>
                            </div>
                        </td>
                    </tr>
                </table>
                <uc1:historyGrid ID="ucHistoryGrid" runat="server" ShowPlayerName="true" AssignRecordingLink="AssignRecording.aspx"/>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
