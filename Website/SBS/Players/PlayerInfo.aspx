<%@ Page Language="VB" MasterPageFile="~/SBS/Players/Player.master" AutoEventWireup="false" CodeFile="PlayerInfo.aspx.vb" Inherits="SBSPlayer.SBS_Players_PlayerInfo" %>

<%@ Register Src="~/Inc/Players/InboxEmail.ascx" TagName="InboxEmail" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Players/ComposeEmail.ascx" TagName="ComposeEmail" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Players/accountStatus.ascx" TagName="accountStatus" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/changePassword.ascx" TagName="changePassword" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">

    <div id="historypanel" class="roundcorner" style="padding: 5px 5px 5px 5px; width: 98%;">
        <div class="row mbl">
            <div class="col-lg-12">
                <ul id="generalTab" class="nav nav-tabs responsive hidden-xs hidden-sm">
                    <li id="tbChangePw" runat="server" class="active">
                        <asp:LinkButton runat="server" ID="lbnChangePass" CommandArgument="CHANGE_PASSWORD"
                            ToolTip="Change Password" Text="Change Player's password" CausesValidation="false" OnClick="lbnTab_Click" />
                    </li>
                    <li id="tbComposeEm" runat="server">
                        <asp:LinkButton runat="server" ID="lbnComposeEmail" CommandArgument="COMPOSE_EMAIL"
                            ToolTip="Compose Email" Text="Compose Email" CausesValidation="false" OnClick="lbnTab_Click" />
                    </li>
                    <li id="tbInboxEm" runat="server">
                        <asp:LinkButton runat="server" ID="lbnInboxEmail" CommandArgument="INBOX_EMAIL"
                            ToolTip="Inbox Email" Text="Inbox Email" CausesValidation="false" OnClick="lbnTab_Click" />
                    </li>
                    <li  id="tbReplyEm" runat="server">
                        <asp:LinkButton runat="server" ID="lbnReplyEmail" CommandArgument="REPLY_EMAIL"
                            ToolTip="Reply Email" Text="Reply Email" CausesValidation="false" OnClick="lbnTab_Click" />
                    </li>
                </ul>
                <div id="generalTabContent" class="tab-content responsive hidden-xs hidden-sm">
                    <uc1:accountStatus ID="ucAccountStatus" runat="server" />
                    <uc1:changePassword ID="ucChangePassword" runat="server" />
                    <uc1:ComposeEmail ID="ucComposeEmail" runat="server" />
                    <uc1:InboxEmail ID="ucInboxEmail" runat="server" ReplyEmail="Inbox" />
                    <uc1:InboxEmail ID="ucReplyEmail" runat="server" ReplyEmail="Reply" />
                </div>
            </div>
        </div>
    </div>


</asp:Content>

