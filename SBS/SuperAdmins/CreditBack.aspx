<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreditBack.aspx.vb" Inherits="SBSSuperAdmin.CreditBack" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script language="javascript" type="text/javascript">
// <!CDATA[

        function Button1_onclick() {
            window.close();
        }

// ]]>
    </script>

    <link href="/SBS/Inc/Styles/styles.css" rel="stylesheet" type="text/css" />
    <link href="/SBS/Inc/Styles/tabContainer.css" rel="stylesheet" type="text/css" />

    <script src="/Inc/Scripts/std.js" language="javascript" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; height: 200px; background-color: white">
        <table cellpadding="7" width="100%">
            <tr>
                <td colspan="2" class="tableheading" nowrap="nowrap">
                    Credit Back
                </td>
            </tr>
            <tr>
                <td>
                    Player
                </td>
                <td>
                    <asp:Label ID="lblPlayer" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Amount
                </td>
                <td>
                    <asp:TextBox ID="txtAmount" MaxLength="6" Width="50" CssClass="textInput" runat="server" style="text-align: right;"
                    onkeypress="javascript:return inputNumber(this,event, true);"
                    onblur="javascript:formatNumber(this,2);"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Reason
                </td>
                <td>
                    <asp:TextBox ID="txtDescription" MaxLength="50" Width="200" CssClass="textInput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button ID="btnOK" CssClass="textInput" runat="server" Text="Ok" Width="48px" />
                    &nbsp;
                    <input id="Button1" class="textInput" type="button" value="Cancel" onclick="return Button1_onclick()" />
                    <cc1:CButtonConfirmer ID="CButtonConfirmer1" AttachTo="btnOK" ConfirmExpression="confirm('Are you sure want to credit back amount?')" runat="server">
                    </cc1:CButtonConfirmer>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
