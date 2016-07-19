<%@ Page Language="VB" MasterPageFile="../Agents.master" AutoEventWireup="false" CodeFile="PLReport.aspx.vb" Inherits="Agents_PLReport" %>

<%@ Register Src="~/Inc/SuperAdmins/PLReport.ascx" TagName="PLReport" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    
    <div id="tab-general">
        <div class="row mbl">
            <div class="col-lg-12">
                <ul id="generalTab" class="nav nav-tabs responsive">
                    <li id="liDAILY" runat="server">
                        <asp:LinkButton runat="server" ID="lbtTabDaily" CommandArgument="DAILY" Text="Daily"
                            ToolTip="Daily P&L Report" CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                    <li id="liWEEKLY" runat="server">
                        <asp:LinkButton runat="server" ID="lbtTabWeekly" CommandArgument="WEEKLY" Text="Weekly"
                             ToolTip="Weekly P&L report" CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                     <li id="liYTD" runat="server">
                        <asp:LinkButton runat="server" ID="lbtYTD" CommandArgument="YTD" Text="Yearly Total Detail"
                             ToolTip="Year To Date" CausesValidation="false" OnClick="lbtTab_Click" />
                    </li>
                </ul>
                <div id="generalTabContent" class="tab-content responsive" style="border: 0 none">
                    <div id="tabContent1" class="tab-pane fade in" runat="server">
                        <uc:PLReport ID="ucPLReport1" runat="server" />
                    </div>
                    <div id="tabContent2" class="tab-pane fade in" runat="server">
                        <uc:PLReport ID="ucPLReport2" runat="server" />
                    </div>
                    <div id="tabContent3" class="tab-pane fade in" runat="server">
                        <uc:PLReport ID="ucPLReport3" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        $(document).ready(function () {
            setTimeout(function () {
                $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtTabDaily.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtTabWeekly.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent3.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbtYTD.UniqueID%>', '');
                });
            }, 300);

        });

        $("#<%=lbtTabDaily.ClientID%>").click(function () {
            __doPostBack('<%=lbtTabDaily.UniqueID%>', '');
        });

        $("#<%=lbtTabWeekly.ClientID%>").click(function () {
            __doPostBack('<%=lbtTabWeekly.UniqueID%>', '');
        });

        $("#<%=lbtYTD.ClientID%>").click(function () {
            __doPostBack('<%=lbtYTD.UniqueID%>', '');
        });

    </script>

</asp:Content>

<asp:Content ID="Content2" runat="server" contentplaceholderid="head">

    <style type="text/css">
        .style1
        {
            height: 17px;
        }
    </style>

</asp:Content>


