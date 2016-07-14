<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LottoSumGame.ascx.vb" Inherits="SBCSuperAdmins.LottoSumGame" %>

<table cellpadding="5" cellspacing="0">
    <tr><td colspan="4"><strong>The LOTTOSUM Game : Sum of State Lottery Numbers </strong></td></tr>
   <%-- <tr><td colspan="4">Sum of  5 numbers ( Bonus number is not included )</td></tr>--%>
    <tr align="left" valign="middle">
        <td>
        For Mega:   
        </td>
        <td>
            from 1 to 56 numbers
        </td>
        <td>
        142
        <asp:TextBox ID="txtMegaOver" runat="server" size="4" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                            AutoPostBack="false" />
        Over(Juice)                                                    
        </td>
        <td>
         <asp:TextBox ID="txtMegaUnder" runat="server" size="4" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                            AutoPostBack="false" />
        Under(Juice)
        </td>
    </tr>
    <tr>
        <td>
            For SuperLotto:  
        </td>
        <td>
            from 1 to 47 numbers
        </td>
        <td>
        120
        <asp:TextBox ID="txtSuperLottoOver" runat="server" size="4" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                            AutoPostBack="false" />
        Over(Juice)                                                    
        </td>
        <td>
         <asp:TextBox ID="txtSuperLottoUnder" runat="server" size="4" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                            AutoPostBack="false" />
        Under(Juice)
        </td>
    </tr>
     <tr>
        <td>
            For Fantasy Five:
        </td>
        <td>
            from 1 to 39 numbers
        </td>
        <td>
        100
        <asp:TextBox ID="txtFantasyOver" runat="server" size="4" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                            AutoPostBack="false" />
        Over(Juice)                                                    
        </td>
        <td>
         <asp:TextBox ID="txtFantasyUnder" runat="server" size="4" MaxLength="5" onkeypress="javascript:return inputNumber(this,event, false);"
                                                            AutoPostBack="false" />
        Under(Juice)
        </td>
    </tr>
    <tr>
        <td colspan="4" align="right" > 
            <asp:Button ID="btnSave" runat="server" Text="Save" />
        </td>
    </tr>
</table>











































