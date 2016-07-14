<%@ Page Language="VB" MasterPageFile="~/SBS/CallCenter/CCAgents.master" AutoEventWireup="false"
    CodeFile="AccountStatus.aspx.vb" Inherits="SBSCallCenterAgents.AccountStatus" %>

<%@ Register Src="~/Inc/changePassword.ascx" TagName="changePassword" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
           
                <div style="background-color: White; margin: 5px;">
                    <table cellpadding="2" cellspacing="2" border="0" style="margin-top:-15px">
                        <tr>
                            <td style="white-space: nowrap; border-bottom: #CCCCCC 1px solid">
                                <ul class="bottomtab" style="margin-bottom: 0px;">
                                    <li>
                                        <asp:LinkButton runat="server" ID="lbnChangePass" CommandArgument="CHANGE_PASSWORD"
                                            ToolTip="Change Password" Text="Change password" CausesValidation="false" OnClick="lbnTab_Click" />
                                    </li>
                                </ul>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 5px;">
                                <uc1:changePassword ID="ucChangePassword" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
        
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
