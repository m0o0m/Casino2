<%@ Control Language="VB" AutoEventWireup="false" CodeFile="editPlayerTemplate.ascx.vb"
    Inherits="SBCSuperAdmin.editPlayerTemplate" %>
<table class="table">
    <tr>
        <td>Template Name
        </td>
        <td>
            <asp:TextBox ID="txtTemplateName" runat="server" CssClass="textInput" MaxLength="50"
                Width="360" />
        </td>
    </tr>
    <tr>
        <td>Account Balance
        </td>
        <td>
            <asp:TextBox ID="txtAccountBalance" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="150" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
</table>
<table class="table table-hover">
    <col width="60%" align="left" />
    <col width="40%" align="right" style="padding-right: 10px;" />
    <tr class="tableheading">
        <td></td>
        <td align="right" style="padding-right: 10px;">Credit
        </td>
    </tr>
    <tr>
        <td>Max Credit
        </td>
        <td align="right">
            <asp:TextBox ID="txtmaxcredit" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max Casino Credit</td>
        <td align="right">
            <asp:TextBox ID="txtmaxcasino" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Min Bet Phone
        </td>
        <td align="right">
            <asp:TextBox ID="txtminbetphone" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Min Bet Internet
        </td>
        <td align="right">
            <asp:TextBox ID="txtminbedinternet" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max 1Q
        </td>
        <td align="right">
            <asp:TextBox ID="txtmax1q" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max 2Q
        </td>
        <td align="right">
            <asp:TextBox ID="txtmax2q" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max 3Q
        </td>
        <td align="right">
            <asp:TextBox ID="txtmax3q" runat="server" CssClass="textInput" Style="text-align: right; padding-left: 2px;"
                Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max 4Q</td>
        <td align="right">
            <asp:TextBox ID="txtmax4q" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max 1H</td>
        <td align="right">
            <asp:TextBox ID="txtmax1h" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max 2H</td>
        <td align="right">
            <asp:TextBox ID="txtmax2h" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max Single</td>
        <td align="right">
            <asp:TextBox ID="txtmaxsingle" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max Parlay</td>
        <td align="right">

            <asp:TextBox ID="txtmaxparlay" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max Reverse</td>
        <td align="right">
            <asp:TextBox ID="txtmaxreverse" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
    <tr>
        <td>Max Teaser</td>
        <td align="right">
            <asp:TextBox ID="txtmaxteaser" runat="server" CssClass="textInput"
                Style="text-align: right; padding-left: 2px;" Width="70" onkeypress="javascript:return inputNumber(this,event, false);"
                onblur="javascript:formatNumber(this,2);" />
        </td>
    </tr>
</table>
