<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GameCircledSettings.ascx.vb"
    Inherits="SBS_Agents_Management_Inc_GameCircledSettings" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wbl" %>
<fieldset style="padding: 5px;">
    <legend style="margin-left: 20px; padding: 0 5px 0 5px;">Specific Game Circled Setting
    </legend>
    <div style="margin: 10px">
        <table class="gamebox" cellpadding="5" cellspacing="0" style="width: 100%; margin-top: 12px;
            font-size: 12px;">
            <tr>
                <td>
                </td>
                <td colspan="2" style="white-space: nowrap">
                    Max Allowed:
                </td>
            </tr>
            <tr runat="server" id="trAgents">
                <td>
                </td>
                <td style="padding-left: 15px">
                    Agents
                </td>
                <td style="text-align: left">
                    <wbl:CDropDownList runat="server" ID="ddlAgents" AutoPostBack="true" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="padding-left: 15px">
                    Straight
                </td>
                <td style="text-align: left">
                    <asp:TextBox ID="txtStraight" Width="70" CssClass="textInput" runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                        Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="padding-left: 15px">
                    Parlay & Reverse
                </td>
                <td style="text-align: left">
                    <asp:TextBox ID="txtPnR" Width="70" CssClass="textInput" runat="server" onkeypress="javascript:return inputNumber(this,event, true);"
                        Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                    <asp:Button ID="btnSaveCircled" runat="server" CssClass="button" Style="float: right;"
                        Text="Save" />
                </td>
            </tr>
        </table>
    </div>
</fieldset>
