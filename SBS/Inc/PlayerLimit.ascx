
<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerLimit.ascx.vb"
    Inherits="SBCWebsite.PlayerLimit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc" %>
<%@ Register Src="~/SBS/Inc/PlayerLimitManagement.ascx" TagName="PlayerManagement"
    TagPrefix="uc" %>

<div class="panel panel-grey">
    <div class="panel-heading">Player limit setting</div>
    <div class="panel-body">
        <div class="form-group">
            <div class="col-md-12">
                <input type="button" class="btn btn-red pull-right ml4" value="Back To PlayerList" onclick="$('input[id$=btnCancelInfo]').click();" />
                <asp:Button runat="server" ID="btnUpdateLimitTop" CssClass="btn btn-primary pull-right " Text="Save setting" />
            </div>
        </div>
        <asp:HiddenField ID="hfPlayerTemplateID" runat="server" />
        <asp:HiddenField ID="hfClickedTabIndex" runat="server" />
        <div id="tab-general">
            <div class="row mbl">
                <div class="col-lg-12">
                    <ul id="generalTab" class="nav nav-tabs responsive">
                        <li id="liFOOTBALL" runat="server">
                            <asp:LinkButton runat="server" ID="lbnFootball" CommandArgument="FOOTBALL" Text="Football"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liBASKETBALL" runat="server">
                            <asp:LinkButton runat="server" ID="lbnBasketball" CommandArgument="BASKETBALL" Text="Basketball"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liBASEBALL" runat="server">
                            <asp:LinkButton runat="server" ID="lbnBaseball" CommandArgument="BASEBALL" Text="Baseball"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liHOCKEY" runat="server">
                            <asp:LinkButton runat="server" ID="lbnHockey" CommandArgument="HOCKEY" Text="Hockey"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liSOCCER" runat="server">
                            <asp:LinkButton runat="server" ID="lbnSoccer" CommandArgument="SOCCER" Text="Soccer"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liGOLF" runat="server">
                            <asp:LinkButton runat="server" ID="lbnGolf" CommandArgument="GOLF" Text="Golf"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liTENNIS" runat="server">
                            <asp:LinkButton runat="server" ID="lbnTennis" CommandArgument="TENNIS" Text="Tennis"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                        <li id="liOTHER" runat="server">
                            <asp:LinkButton runat="server" ID="lbnOther" CommandArgument="OTHER" Text="Other"
                                CausesValidation="false" OnClick="lbnTab_Click" />
                        </li>
                    </ul>
                    <div id="generalTabContent" class="tab-content responsive" style="border: 0 none">
                        <div id="tabContent1" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucFootball" runat="server" />
                        </div>
                        <div id="tabContent2" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucBasketball" runat="server" />
                        </div>
                        <div id="tabContent3" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucBaseball" runat="server" />
                        </div>
                        <div id="tabContent4" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucHockey" runat="server" />
                        </div>
                        <div id="tabContent5" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucSoccer" runat="server" />
                        </div>
                        <div id="tabContent6" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucGolf" runat="server" />
                        </div>
                        <div id="tabContent7" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucTennis" runat="server" />
                        </div>
                        <div id="tabContent8" class="tab-pane fade in" runat="server">
                            <uc:PlayerManagement ID="ucOther" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-12">
                <asp:Button runat="server" ID="btnUpdateLimitBottom" CssClass="btn btn-primary pull-right" Text="Save setting" />
            </div>
        </div>
    </div>
</div>

<script>
    $(document).ready(function () {
        setTimeout(function () {
            $(".panel-title a[href='#collapse-" + '<%= tabContent1.ClientID %>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnFootball.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent2.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnBasketball.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent3.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnBaseball.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent4.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnHockey.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent5.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnSoccer.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent6.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnGolf.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent7.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnTennis.UniqueID%>', '');
                });

                $(".panel-title a[href='#collapse-" + '<%= tabContent8.ClientID%>' + "']").mousedown(function () {
                    __doPostBack('<%=lbnOther.UniqueID%>', '');
                });

            }, 300);

        });

        $("#<%=lbnFootball.ClientID%>").click(function () {
        __doPostBack('<%=lbnFootball.UniqueID%>', '');
        });

        $("#<%=lbnBasketball.ClientID%>").click(function () {
        __doPostBack('<%=lbnBasketball.UniqueID%>', '');
        });

        $("#<%=lbnBaseball.ClientID%>").click(function () {
        __doPostBack('<%=lbnBaseball.UniqueID%>', '');
        });

        $("#<%=lbnHockey.ClientID%>").click(function () {
            __doPostBack('<%=lbnHockey.UniqueID%>', '');
        });

        $("#<%=lbnSoccer.ClientID%>").click(function () {
            __doPostBack('<%=lbnSoccer.UniqueID%>', '');
        });

        $("#<%=lbnGolf.ClientID%>").click(function () {
            __doPostBack('<%=lbnGolf.UniqueID%>', '');
        });

        $("#<%=lbnTennis.ClientID%>").click(function () {
            __doPostBack('<%=lbnTennis.UniqueID%>', '');
        });

        $("#<%=lbnOther.ClientID%>").click(function () {
            __doPostBack('<%=lbnOther.UniqueID%>', '');
        });

    </script>

<script type="text/javascript" language="javascript">
    function clientActiveTabChanged(sender, args) {
        var currentTabIndex = $get('<%=hfClickedTabIndex.ClientID %>');
        currentTabIndex.value = sender.get_activeTabIndex();
    }
</script>

