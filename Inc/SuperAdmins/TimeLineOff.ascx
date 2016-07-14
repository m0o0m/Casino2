<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TimeLineOff.ascx.vb"
    Inherits="SBCSuperAdmins.TimeLineOff" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<table class="gamebox" cellpadding="5" cellspacing="0" style="width: 100%">
    <tr>
        <td colspan="4" class="tableheading">
            Display Game Lines
        </td>
    </tr>
    <tr>
        <td>
            Off Before 2H Time (Minutes)<br />
            (for Footbal, Basketball, Soccer)
        </td>
        <td>
            <asp:TextBox ID="txt2H" onkeypress="javascript:return inputNumber(this,event, true);"
                Width="30" CssClass="textInput" runat="server"></asp:TextBox>
        </td>
    </tr>
     <tr>
        <td>
        </td>
        <td>
            <asp:Button ID="btn2HSave" runat="server" CssClass="textInput" Text="Save" />
        </td>
    </tr>
    <tr>
        <td style="white-space: nowrap">
            Game Type
        </td>
        <td>
            <wlb:CDropDownList ID="ddlGameType" runat="server" OptionalText="" OptionalValue=""
                hasOptionalItem="true" CssClass="textInput" AutoPostBack="true">
            </wlb:CDropDownList>
        </td>
    </tr>
    <tr>
        <td style="white-space: nowrap">
            Off Before Game Time (Minutes)
        </td>
        <td>
            <asp:TextBox ID="txtOffMinutes" MaxLength="3" Width="30" CssClass="textInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td style="white-space: nowrap">
            Display Lines Before Game Time (Hours)
        </td>
        <td>
            <asp:TextBox ID="txtDisplayHours" MaxLength="3" Width="30" CssClass="textInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Button ID="btnSave" runat="server" CssClass="button" Text="Save" />
        </td>
    </tr>
</table>
<asp:DataGrid runat="server" ID="dtgTimeOff" AutoGenerateColumns="false" HeaderStyle-CssClass="tableheading"
    CellPadding="5" CellSpacing="0" Width="100%" HeaderStyle-HorizontalAlign="Center">
    <Columns>
        <asp:BoundColumn HeaderText="Game Type" DataField="Key" HeaderStyle-HorizontalAlign="Left">
        </asp:BoundColumn>
        <asp:BoundColumn HeaderText="Off Before" DataField="Value" ItemStyle-HorizontalAlign="Center"
            HeaderStyle-Width="70"></asp:BoundColumn>
        <asp:BoundColumn HeaderText="Display Before" DataField="SubCategory" ItemStyle-HorizontalAlign="Center"
            HeaderStyle-Width="100"></asp:BoundColumn>
    </Columns>
</asp:DataGrid>
