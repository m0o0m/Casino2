<%@ Control Language="VB" AutoEventWireup="false" CodeFile="playerProfile.ascx.vb"
    Inherits="SBCCallCenterAgents.Inc_CallCenter_playerProfile" %>
<%@ Register Src="~/Inc/contentFileDB.ascx" TagName="contentFileDB" TagPrefix="uc1" %>
<table cellpadding="2" cellspacing="2" style="background: url(/SBC/images/header_bg.png);
    background-repeat: repeat-x; color: #000000; font-size: 10px; border: solid 1px #CECECE;"
    width="100%">
    <tr>
        <td valign="middle">
            <table cellpadding="2" cellspacing="2" id="tblLogin" runat="server">
                <tr>
                    <td>
                        PHONE LOGIN:
                    </td>
                    <td>
                        <asp:TextBox ID="txtPhoneLogin" runat="server" CssClass="textInput" MaxLength="50"
                            Width="70" />
                    </td>
                    <td>
                        PHONE PASSWORD:
                    </td>
                    <td>
                        <asp:TextBox ID="txtPhonePassowrd" runat="server" CssClass="textInput" TextMode="Password"
                            Width="70px" MaxLength="10" />
                    </td>
                    <td>
                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" 
                            Width="60" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlProfile" runat="server" Font-Size="12px" Style="vertical-align: middle;">
                <b>
                    <asp:Label ID="lblName" runat="server" />
                    &nbsp;|&nbsp;
                    <asp:LinkButton ID="lbnLogout" runat="server" Text="Log out" ForeColor="Red" ToolTip="Log out this player"
                        Font-Underline="false" />
                </b>
            </asp:Panel>
        </td>
        <td>
            <asp:Panel runat="server" ID="pnlPlayerStatus" Visible="false" Style="font-size:12px;
                display: inline; float: right; margin-right: 20px; 
                width: 100%; clear: both; text-align: right">
                <span style="color: red">Credit Limit:</span> <span style="color: blue;">
                    <asp:Literal ID="lblCreditLimit" runat="server" Text=""></asp:Literal>
                </span>&nbsp; <span style="color: red">Available Balance:</span> <span style="color: blue;" id="lblBalanceAmount">
                    <%=BalanceAmount()%>
                </span>
            </asp:Panel>
        </td>
    </tr>
</table>
