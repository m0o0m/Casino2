<%@ Page Language="VB" MasterPageFile="~/SBS/Players/Player.master" AutoEventWireup="false" CodeFile="PlayerEmail.aspx.vb" Inherits="SBSPlayers.PlayerEmail" %>
    <%@ Register Src="~/Inc/Players/InboxEmail.ascx" TagName="InboxEmail" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Players/ComposeEmail.ascx" TagName="ComposeEmail" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" Runat="Server">

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="historypanel" class="roundcorner" style="padding: 5px 5px 5px 5px; width: 98%">
                <div style="background-color: White; margin: 5px 5px 5px 5px; width: 99%">
                    <table cellpadding="2" cellspacing="2" width="100%" border="0">
                        <tr>
                            <td style="white-space: nowrap; border-bottom: #CCCCCC 1px solid">
                                <ul class="bottomtab" style="margin-bottom: 0px;">
                                    <li>
                                        <asp:LinkButton runat="server" ID="lbnComposeEmail" CommandArgument="COMPOSE_EMAIL"
                                            ToolTip="Compose Email" Text="Compose Email" CausesValidation="false" OnClick="lbnTab_Click" />
                                    </li>  
                                     <li>
                                        <asp:LinkButton runat="server" ID="lbnInboxEmail" CommandArgument="INBOX_EMAIL"
                                            ToolTip="Inbox Email" Text="Inbox Email" CausesValidation="false" OnClick="lbnTab_Click" />
                                    </li>
                                     <li>
                                        <asp:LinkButton runat="server" ID="lbnReplyEmail" CommandArgument="REPLY_EMAIL"
                                            ToolTip="Reply Email" Text="Reply Email" CausesValidation="false" OnClick="lbnTab_Click" />
                                    </li>                                
                                </ul>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 5px;">
                                <uc1:ComposeEmail ID="ucComposeEmail" runat="server" />   
                                 <uc1:InboxEmail ID="ucInboxEmail" runat="server" ReplyEmail="Inbox" />   
                                   <uc1:InboxEmail ID="ucReplyEmail" runat="server" ReplyEmail="Reply" />                               
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

