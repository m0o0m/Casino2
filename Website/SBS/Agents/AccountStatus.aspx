<%@ Page Language="VB" MasterPageFile="Agents.master" AutoEventWireup="false" ValidateRequest="false" CodeFile="AccountStatus.aspx.vb" Inherits="SBSAgents.AccountStatus" %>

<%@ Register Src="~/Inc/Players/InboxEmail.ascx" TagName="InboxEmail" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Players/ComposeEmail.ascx" TagName="ComposeEmail" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/accountStatus.ascx" TagName="accountStatus" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/changePassword.ascx" TagName="changePassword" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    
    <script>
        function initTabMobi() {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnAccountStatus.UniqueID%>', '');
                });
                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnChangePass.UniqueID%>', '');
                    });
                $(".panel-title a[href='#collapse-" + '<%= tabContent3.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnComposeEmail.UniqueID%>', '');
                    });
                $(".panel-title a[href='#collapse-" + '<%= tabContent4.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnInboxEmail.UniqueID%>', '');
                    });
                $(".panel-title a[href='#collapse-" + '<%= tabContent5.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnReplyEmail.UniqueID%>', '');
                    });
            });
        }

        function reBindTabEvent() {
            initTabMobi();

            $("#<%=lbnAccountStatus.ClientID%>").click(function () {
                __doPostBack('<%=lbnAccountStatus.UniqueID%>', '');
            });

            $("#<%=lbnChangePass.ClientID%>").click(function () {
                __doPostBack('<%=lbnChangePass.UniqueID%>', '');
            });

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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" OnUpdateCompleteClientScript="reBindTabEvent();">
        <contenttemplate>
           <div id="historypanel">
               <div id="tab-general">
                   <div class="row mbl">
                        <div class="col-lg-12">
                            <ul id="generalTab" class="nav nav-tabs responsive">
                               <li id="liACCOUNT_STATUS" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnAccountStatus" CommandArgument="ACCOUNT_STATUS"
                                            ToolTip="Account Status" Text="Account Status" CausesValidation="false" OnClick="lbnTab_Click"
                                       />
                               </li>
                                <li id="liCHANGE_PASSWORD" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnChangePass" CommandArgument="CHANGE_PASSWORD"
                                            ToolTip="Change Password" Text="Change Agent's password" CausesValidation="false" OnClick="lbnTab_Click"
                                          />
                                </li>
                                <li id="liCOMPOSE_EMAIL" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnComposeEmail" CommandArgument="COMPOSE_EMAIL"
                                        ToolTip="Compose Email" Text="Compose Email" CausesValidation="false" OnClick="lbnTab_Click" 
                                         />
                                </li>    
                                <li id="liINBOX_EMAIL" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnInboxEmail" CommandArgument="INBOX_EMAIL"
                                        ToolTip="Inbox Email" Text="Inbox Email" CausesValidation="false" OnClick="lbnTab_Click" 
                                        />
                                </li> 
                                <li id="liREPLY_EMAIL" runat="server">
                                    <asp:LinkButton runat="server" ID="lbnReplyEmail" CommandArgument="REPLY_EMAIL"
                                        ToolTip="Reply Email" Text="Reply Email" CausesValidation="false" OnClick="lbnTab_Click" 
                                        />
                                </li> 
                           </ul> 
                             <div id="generalTabContent" class="tab-content responsive">
                                 <div id="tabContent1" class="tab-pane fade in" runat="server">
                                      <uc1:accountStatus ID="ucAccountStatus" runat="server" />
                                 </div>

                                 <div id="tabContent2" class="tab-pane fade in" runat="server">
                                     <uc1:changePassword ID="ucChangePassword" runat="server" />
                                 </div>
                                  <div id="tabContent3" class="tab-pane fade in" runat="server">
                                       <uc1:ComposeEmail ID="ucComposeEmail" runat="server" />    
                                  </div>
                                 <div id="tabContent4" class="tab-pane fade in" runat="server">
                                        <uc1:InboxEmail ID="ucInboxEmail" runat="server" ReplyEmail="Inbox" /> 
                                 </div>
                                 <div id="tabContent5" class="tab-pane fade in" runat="server">
                                        <uc1:InboxEmail ID="ucReplyEmail" runat="server" ReplyEmail="Reply" />  
                                 </div>
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
