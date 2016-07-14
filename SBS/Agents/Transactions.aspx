<%@ Page Language="VB" MasterPageFile="Agents.master" AutoEventWireup="false" CodeFile="Transactions.aspx.vb"
    Inherits="SBSAgents.Management.Transactions" %>

<%@ Register Src="../../Inc/Transactions.ascx" TagName="PlayerTransactions" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <div id="tab-general">
        <div class="row mbl">
            <div class="col-lg-12">
                <ul id="generalTab" class="nav nav-tabs responsive">
                    <li id="liPLAYERS" runat="server">
                        <asp:LinkButton runat="server" ID="lbtPlayers" CommandArgument="PLAYERS" Text="Players"
                            CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                    <li id="liSUB_AGENTS" runat="server" Visible="False">
                        <asp:LinkButton runat="server" ID="lbtSubAgents" CommandArgument="SUB_AGENTS" Visible="false"
                            Text="Sub Agents" CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                </ul>
                <div id="generalTabContent" class="tab-content responsive" style="border: 0 none; padding: 0">
                    <div id="tabContent1" class="tab-pane fade in" runat="server">
                        <uc:PlayerTransactions ID="ucTransactionPlayer" runat="server" />
                    </div>
                    <div id="tabContent2" class="tab-pane fade in" runat="server">
                        <uc:PlayerTransactions ID="ucTransactionSubAgent" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        $(document).ready(function () {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtPlayers.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=liSUB_AGENTS.UniqueID%>', '');
                });
               
            }, 300);

        });

        $("#<%=lbtPlayers.ClientID%>").click(function () {
            __doPostBack('<%=lbtPlayers.UniqueID%>', '');
        });

        $("#<%=lbtSubAgents.ClientID%>").click(function () {
            __doPostBack('<%=lbtSubAgents.UniqueID%>', '');
        });

    </script>
</asp:Content>
