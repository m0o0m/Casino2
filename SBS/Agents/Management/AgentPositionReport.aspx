<%@ Page Language="VB" MasterPageFile="../Agents.master" AutoEventWireup="false"  ValidateRequest ="false"
    CodeFile="AgentPositionReport.aspx.vb" Inherits="Agents_AgentPositionReport" %>
<%@ Register Src="~/Inc/Reports/NewAgentPositionReport.ascx" TagName="NewAgentPositionReport"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/Reports/AgentPositionReport.ascx" TagName="AgentPositionReport"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <style type="text/css">
        #LineType
        {
            color: #f7a30b;
        }
    </style>
    
    <div id="tab-general">
        <div class="row mbl">
            <div class="col-lg-12">
                <ul id="generalTab" class="nav nav-tabs responsive">
                    <li id="liALL" runat="server">
                        <asp:LinkButton runat="server" ID="lbtAllPlayer" CommandArgument="ALL" 
                            Text="All" CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                    <li id="liDETAILS" runat="server">
                        <asp:LinkButton runat="server" ID="lbtPlayers" CommandArgument="DETAILS" Text="Details"
                            CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                </ul>
                <div id="generalTabContent" class="tab-content responsive" style="border: 0 none">
                    <div id="tabContent1" class="tab-pane fade in" runat="server">
                        <uc:NewAgentPositionReport ID="ucNewAgentPositionReport" runat="server" Title="AgentPositionReport" />
                    </div>
                    <div id="tabContent2" class="tab-pane fade in" runat="server">
                        <uc:AgentPositionReport ID="ucAgentPositionReport" Visible="false" runat="server" Title="AgentPositionReport" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        $(document).ready(function () {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtAllPlayer.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtPlayers.UniqueID%>', '');
                });
            }, 300);

        });

        $("#<%=lbtAllPlayer.ClientID%>").click(function () {
            __doPostBack('<%=lbtAllPlayer.UniqueID%>', '');
        });

        $("#<%=lbtPlayers.ClientID%>").click(function () {
            __doPostBack('<%=lbtPlayers.UniqueID%>', '');
        });
    </script>
</asp:Content>
