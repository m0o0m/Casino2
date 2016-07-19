<%@ Page Language="VB" AutoEventWireup="false" CodeFile="HistoricalAmount.aspx.vb" Inherits="SBCSuperAdmins.HistoricalAmount" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/Inc/SuperAdmins/YearlyTotalDetail.ascx" TagName="HistoricalAmount" TagPrefix="uc" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Statistical Report</title>
</head>

    <link href="/SBS/Inc/Styles/styles.css" rel="stylesheet" type="text/css" />
    <link href="/SBS/Inc/Styles/tabContainer.css" rel="stylesheet" type="text/css" />

    <script src="/Inc/Scripts/std.js" language="javascript" type="text/javascript"></script>
<body style="background-color: white ;" >
    <form id="form1" runat="server">
    <asp:ScriptManager id="scriptManager" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updatePanel" runat="server">
    <ContentTemplate>
   
    <table>
        <tr>
            <td style="border-bottom: solid 1px #CECECE;">
                <ul class="bottomtab" style="margin-bottom: 0px; margin-left: 0px; padding-left: 0px">
                    <li>
                        <asp:LinkButton runat="server" ID="lbtTabWeekly" CommandArgument="WEEKLY" Text="Weekly"
                            ToolTip="Weekly Statistical Report" CausesValidation="false" OnClick="lbtTab_Click" PostBackUrl="#" />
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="lbtYTD" CommandArgument="YTD" Text="Year To Date"
                             ToolTip="Year to date Statistical Report" CausesValidation="false" OnClick="lbtTab_Click" PostBackUrl="#" />
                    </li>
                     <li>
                        <asp:LinkButton runat="server" ID="lbtYearly" CommandArgument="YEARLY" Text="Yearly"
                             ToolTip="Yearly Statistical Report" CausesValidation="false" OnClick="lbtTab_Click" PostBackUrl="#" />
                    </li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>
                <br />
               <uc:HistoricalAmount ID="ucHistoricalAmount" runat="server" Visible="true" />
                 
            </td>
        </tr>
    </table>
    
     
   
    </ContentTemplate>
    </asp:UpdatePanel> 
    </form>
</body>
</html>
