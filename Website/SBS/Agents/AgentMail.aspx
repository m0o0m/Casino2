<%@ Page Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="AgentMail.aspx.vb" Inherits="SBSAgents.AgentMail" %>

<%@ Register Src="~/Inc/Players/InboxEmail.ascx" TagName="InboxEmail" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Players/ComposeEmail.ascx" TagName="ComposeEmail" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    
    
    <script>
        function initTabMobi() {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                     __doPostBack('<%=lbnComposeEmail.UniqueID%>', '');
                });

                 $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                     __doPostBack('<%=lbnInboxEmail.UniqueID%>', '');
                });

                 $(".panel-title a[href='#collapse-" + '<%= tabContent3.ClientID%>' + "']").mousedown(function () {
                     __doPostBack('<%=lbnReplyEmail.UniqueID%>', '');
                });
             }, 300);
        }

        function reBindTabEvent() {
            initTabMobi();

            $("#<%=lbnComposeEmail.ClientID%>").click(function () {
                __doPostBack('<%=lbnComposeEmail.UniqueID%>', '');
             });

             $("#<%=lbnInboxEmail.ClientID%>").click(function () {
                __doPostBack('<%=lbnInboxEmail.UniqueID%>', '');
            });

            $("#<%=lbnReplyEmail.ClientID%>").click(function () {
                __doPostBack('<%=lbnReplyEmail.UniqueID%>', '');
            });

            
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            reBindTabEvent();
            fakewaffle.responsiveTabs(['xs', 'sm']);
        });

    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <contenttemplate>
            <div id="tab-general">
                <div class="row mbl">
                    <div id="historypanel" class="col-lg-12">
                        <ul id="generalTab" class="nav nav-tabs responsive">
                            <li id="liCOMPOSE_EMAIL" runat="server">
                                <asp:LinkButton runat="server" ID="lbnComposeEmail" CommandArgument="COMPOSE_EMAIL"
                                    ToolTip="Compose Email" Text="Compose Email" CausesValidation="false" OnClick="lbnTab_Click" />
                            </li>    
                            <li id="liINBOX_EMAIL" runat="server">
                                <asp:LinkButton runat="server" ID="lbnInboxEmail" CommandArgument="INBOX_EMAIL"
                                    ToolTip="Inbox Email" Text="Inbox Email" CausesValidation="false" OnClick="lbnTab_Click" />
                            </li> 
                            <li id="liREPLY_EMAIL" runat="server">
                                <asp:LinkButton runat="server" ID="lbnReplyEmail" CommandArgument="REPLY_EMAIL"
                                    ToolTip="Reply Email" Text="Reply Email" CausesValidation="false" OnClick="lbnTab_Click" />
                            </li> 
                        </ul>
                        <div id="generalTabContent" class="tab-content responsive" style="border: 0 none">
                            <div id="tabContent1" class="tab-pane fade in" runat="server">
                                <uc1:ComposeEmail ID="ucComposeEmail" runat="server" />       
                            </div>
                            <div id="tabContent2" class="tab-pane fade in" runat="server">
                                <uc1:InboxEmail ID="ucInboxEmail" runat="server" ReplyEmail="Inbox" />     
                            </div>
                            <div id="tabContent3" class="tab-pane fade in" runat="server">
                                <uc1:InboxEmail ID="ucReplyEmail" runat="server" ReplyEmail="Reply" />       
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </contenttemplate>
    </asp:UpdatePanel>
    
    <script>
        $(document).ready(function () {
            reBindTabEvent();
        });
    </script>

</asp:Content>

