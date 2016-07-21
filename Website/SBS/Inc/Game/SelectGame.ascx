<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SelectGame.ascx.vb" Inherits="SBS_Agents_Inc_Game_SelectGame" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<script type="text/javascript">

    function ShowGame(me, id) {
        //$(me).hide(500);
        $("#" + id).slideDown(500);
    }
    function MiniGame(me, id) {
        //$("#" + id).show(500);
        var $me = $(me);
        $me.parent().next().slideToggle(500);
        if ($me.hasClass('fa-minus')) {
            $me.removeClass('fa-minus');
            $me.addClass('fa-plus');
        } else {
            $me.removeClass('fa-plus');
            $me.addClass('fa-minus');
        }
    }

    function toggleAllGameType(me) {
        var isChecked = $(me).is(':checked');
        $(me).closest('.panel-heading').next().find('input[type="checkbox"]').each(function (index, obj) {
            $(this).attr("checked", isChecked);
        });
    }

    function SelectAll(me) {
        $($(me).parent().parent()).find(".chk").each(function (index, obj) {
            $($(obj).find("input")).attr("checked", true);
        });

    }
    function ClearAll(me) {
        $($(me).parent().parent()).find(".chk").each(function (index, obj) {
            $($(obj).find("input")).attr("checked", false);
        });

    }
    function clearItem() {
        $(".chk").each(function (index, obj) {
            $($(obj).find("input")).attr("checked", false);
        });
    }
    $(document).ready(function () {
        try {
            $(".numgame").each(function (index, obj) {
                $(obj).html("(" + $("#" + $(obj).attr("tid")).find(".chk").length + ")");
            });
        } catch (e) { }

        // 
        $(".divrptGameTypeShort .list-group-item > label").click(function () {
            $(this).toggleClass("highlight");
        });
        $(".panel-heading").click(function () {
            $(this).next(".panel-body").toggleClass("collapsed");
        });

    });



</script>

<div class="row">
    <div class="col-lg-12">

        <div class="page-title-breadcrumb">
            <div class="notif-valid pdL13">
                <asp:Label ID="lbMessage" runat="server" Text=""></asp:Label>
            </div>
            <%-- <div class="col-md-1 pull-left">
                <span class="page-title">WagerType</span>
            </div>--%>
            <%--<div class="col-md-1 pull-left">
                <span class="label label-dark pull-left mtm" style="margin-top: -1px !important; font-size: larger;">
                    <%=IIf(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP", BetType.Replace("BetTheBoard", "Straight").Replace("Reverse", "Action Reverse").Replace("BetIfAll", "Bet The Board"), "Proposition/Future")%>
                </span>
            </div>--%>

            <% If (Not BetType.Equals("Teaser", StringComparison.OrdinalIgnoreCase)) Then%>
            <div class="col-md-4 pull-right">
                <button type="button" class="btn btn-dark pull-right button-style-2 w110px right" style="margin-left: 10px;" onclick='$("#<%=btnContinue.ClientID%>").click()'>
                    Continue
                    <i class="fa fa-forward"></i>
                </button>
                <%-- <button type="button" class="btn btn-red pull-right" onclick='clearItem()'>
                    Clear All
                    <i class="fa fa-times"></i>
                </button>--%>
                <%--<a class="text-red pointer" onclick="clearItem()">Clear</a>--%>
            </div>
            <% End If%>
            <%--<div class="col-md-2 pull-right">
                <a class="text-green ">Step 1: Select Event<asp:Literal ID="lblAcct" runat="server"></asp:Literal></a>
            </div>--%>
            <div class="clearfix"></div>
        </div>
    </div>
</div>

<div class="mbl"></div>

<div class="panel divrptGameTypeShort" id="divrptGameTypeShort" runat="server">
    <div class="panel-body">
        <%--<asp:Repeater runat="server" ID="rptGameTypeShort">
            <ItemTemplate>
                <div id="gametype<%#Container.ItemIndex%>" onclick="ShowGame(this,<%#Container.ItemIndex%>)" class="alert alert-warning col-md-3 mr4">
                    <i class="fa fa-plus pull-right"></i>
                    <strong><%#Container.DataItem%></strong>
                    <span class="numgame" tid="<%#Container.ItemIndex%>"></span>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clearfix"></div>--%>
        <asp:Repeater runat="server" ID="rptGameType">
            <ItemTemplate>
                <div class="col-md-4 game-type" id="<%#Container.ItemIndex%>">
                    <div class="panel panel-grey box-sport-game mgT15">
                        <div class="panel-heading box-title">
                            <%#Container.DataItem%>
                            <%--<span class="" style="margin-left: 7px;" >
                                <input id="<%#Container.ItemIndex%>_cbSelectAll" type="checkbox" onchange="toggleAllGameType(this);" />
                                <label for="<%#Container.ItemIndex%>_cbSelectAll" style="font-size: 14px; color: #fff; position: relative; top: -2px">Select All</label>
                            </span>
                            <i class="fa fa-plus pull-right" onclick="MiniGame(this,'gametype<%#Container.ItemIndex%>')" style="margin-left: 10px;"></i>--%>
                        </div>
                        <div class="panel-body">
                            <%--<div class="col-md-4">
                                <button type="button" class="btn btn-primary" onclick="SelectAll(this)">SELECT ALL</button>
                            </div>
                            <div class="col-md-4">
                                <button type="button" class="btn btn-warning" onclick="ClearAll(this)">CLEAR ALL</button>
                            </div>
                            <div class="col-md-4">
                                <button type="button" class="btn btn-info" onclick='$("#<%=btnContinue.ClientID%>    ").click()'>CONTINUE</button>
                            </div>
                            <div class="clearfix"></div>--%>
                            <%--<div class="mbl"></div>--%>

                            <asp:Repeater ID="rptSubGameType" runat="server" OnItemDataBound="rptSubGameType_ItemDataBound">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div runat="server" id="game" style="max-height: 500px; overflow-y: auto;" class="gamelist">
                                        <asp:Button OnClick="lblGameType_Click" ID="btnGameType" runat="server" Text='<%#Container.DataItem.Replace("Sports","").Trim() %>' Style="display: none" />
                                        <ul class="list-group <%=BetType %>" style="margin-bottom: 0px;">
                                            <li id="lichkCurrent" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkCurrent" class="chk" AutoPostBack="false" Visible="true" runat="server" />
                                                <asp:Label ID="lblGameType" AssociatedControlID="chkCurrent" runat="server" Text='<%#SBCBL.std.SafeString(Container.DataItem).Replace("NCAA Football","College").Replace("CFL Football","Canadian").Replace("AFL Football","Arena").Replace("Football","").Replace("Basketball","").Replace("Baseball","").Replace("Hockey","").Trim() %> '></asp:Label><asp:Literal ID="lblGame" runat="server"></asp:Literal>
                                            </li>
                                            <li id="liOneH" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkOneH" class="chk" AutoPostBack="false" runat="server" />
                                                <asp:Label ID="lblOneH" AssociatedControlID="chkOneH" runat="server" Text="1st"> </asp:Label>
                                            </li>
                                            <li id="liTwoH" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkTwoH" class="chk" AutoPostBack="false" runat="server" Visible="false" />
                                                <asp:Label ID="lblTwoH" AssociatedControlID="chkTwoH" runat="server" Text="2nd" Visible="false"></asp:Label>
                                            </li>
                                            <li id="liQuarter1" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkQuarter1" class="chk" AutoPostBack="false" runat="server" Visible="false" />
                                                <asp:Label ID="lblQuarter1" AssociatedControlID="chkQuarter1" runat="server" Text="Quarter" Visible="false"></asp:Label>
                                            </li>
                                            <li id="liQuarter2" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkQuarter2" class="chk" AutoPostBack="false" runat="server" Visible="false" />
                                                <asp:Label ID="lblQuarter2" AssociatedControlID="chkQuarter2" runat="server" Text="2Q" Visible="false"></asp:Label>
                                            </li>
                                            <li id="liQuarter3" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkQuarter3" class="chk" AutoPostBack="false" runat="server" Visible="false" />
                                                <asp:Label ID="lblQuarter3" AssociatedControlID="chkQuarter3" runat="server" Text="3Q" Visible="false"></asp:Label>
                                            </li>
                                            <li id="liQuarter4" runat="server" class="list-group-item">
                                                <asp:CheckBox ID="chkQuarter4" class="chk" AutoPostBack="false" runat="server" Visible="false" />
                                                <asp:Label ID="lblQuarter4" AssociatedControlID="chkQuarter4" runat="server" Text="4Q" Visible="false"></asp:Label>
                                            </li>
                                        </ul>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                </FooterTemplate>
                            </asp:Repeater>

                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clearfix"></div>
        <% If (Not BetType.Equals("Teaser", StringComparison.OrdinalIgnoreCase) AND (Not BetType.Equals("Parlay", StringComparison.OrdinalIgnoreCase)) ) Then%>
        <div class="col-md-4 pull-right">
            <button type="button" class="btn btn-dark pull-right button-style-2 w110px right" style="margin-left: 10px;" onclick='$("#<%=btnContinue.ClientID%>").click()'>
                Continue
                    <i class="fa fa-forward"></i>
            </button>
        </div>
        <% End If%>
    </div>
</div>

<div id="divTeaserType" runat="server" class="panel panel-grey">
    <div class="panel-heading">Select Option</div>
    <div class="panel-body">
        <table id="TeaserType" runat="server" class="table table-hover table-bordered">
            <tr id="tr46" runat="server" class="offering_pair_odd">
                <td>
                    <asp:LinkButton ID="lbt" runat="server" OnClick="Teaser_Click" Font-Underline="true" CommandArgument="4/6"> 4/6-Point Teaser</asp:LinkButton>
                </td>

                <td>2 - 6 Team 4 pt Basketball & 6 pt Football - Ties & PUSH is No Action - Ties & Lose is a LOSE
                </td>
            </tr>
            <tr id="tr4565" runat="server" class="offering_pair_even">
                <td>
                    <asp:LinkButton ID="LinkButton2" runat="server" OnClick="Teaser_Click" Font-Underline="true" CommandArgument="4.5/6.5"> 4½/6½Point Teaser </asp:LinkButton>
                </td>
                <td>2 - 6 Team 4½ pt Basketball 6½pt Football - Ties & PUSH is No Action - Ties & Lose is a LOSE
                </td>
            </tr>
            <tr id="tr57" runat="server" class="offering_pair_odd">
                <td>
                    <asp:LinkButton ID="LinkButton3" runat="server" OnClick="Teaser_Click" Font-Underline="true" CommandArgument="5/7"> 5/7-Point Teaser </asp:LinkButton>
                </td>
                <td>2 - 6 Team 5 pt Basketball 7 pt Football - Ties & PUSH is No Action - Ties & Lose is a LOSE
                </td>
            </tr>
            <tr id="tr5575" runat="server" class="offering_pair_even">
                <td>
                    <asp:LinkButton ID="LinkButton4" runat="server" OnClick="Teaser_Click" Font-Underline="true" CommandArgument="5.5/7.5"> 5½/7½-Point Teaser</asp:LinkButton>
                </td>
                <td>2 - 6 Team 5½ pt Basketball 7½ pt Football - Ties & PUSH is No Action - Ties & Lose is a LOSE
                </td>
            </tr>
            <tr id="tr38" runat="server" class="offering_pair_odd">
                <td>
                    <asp:LinkButton ID="LinkButton5" runat="server" OnClick="Teaser_Click" Font-Underline="true" CommandArgument="8/10">3T 8-10 SP-Point Tease</asp:LinkButton>
                </td>
                <td>3 Team 8 point Basketball 10 point Football Special Teaser - Ties Lose
                </td>
            </tr>
            <tr id="tr413" runat="server" class="offering_pair_even">
                <td>
                    <asp:LinkButton ID="LinkButton6" runat="server" OnClick="Teaser_Click" Font-Underline="true" CommandArgument="10/13"> 4T 13 Point SP Teaser</asp:LinkButton>
                </td>
                <td>4 Team 13 Point NFL side only Special Teaser - Ties Lose
                </td>
            </tr>
        </table>
    </div>
</div>


<div id="wager" runat="server">
    <div id="betpanel" style="margin-left: 0px; vertical-align: top; padding: 0;">

        <div class="form-group" id="dvSubAgents" runat="server" visible="false">
            <label class="col-md-2 control-label">SubAgents</label>
            <div class="col-md-2">
                <wlb:cdropdownlist id="ddlSubAgents" hasoptionalitem="true" optionaltext="All" cssclass="form-control"
                    optionalvalue="" runat="server" autopostback="true">
                </wlb:cdropdownlist>
            </div>
        </div>
        <div class="form-group" id="dvPlayer" runat="server" visible="false">
            <label class="col-md-2 control-label">Player</label>
            <div class="col-md-2">
                <wlb:cdropdownlist id="ddlPlayers" hasoptionalitem="true" optionaltext="Choose player"
                    cssclass="textInput" optionalvalue="" runat="server" autopostback="false">
                </wlb:cdropdownlist>
            </div>
        </div>
    </div>

    <asp:Literal ID="lbtAgentContinue" runat="server"></asp:Literal>
</div>


<div style="padding-left: 20px; padding-right: 20px;" id="pnSelectGame" runat="server">
    <div style="text-align: center; margin-top: 20px;">
        <span style="font-size: 12pt; color: White; font-weight: bold;">
            <asp:Literal ID="lblBetTheBoard" runat="server"></asp:Literal></span>

    </div>

    <div class="titleSelectgame">
        <asp:Literal ID="lblSelectGameMsg" runat="server"></asp:Literal>
    </div>
    <asp:Button ID="btnContinue" CssClass="continue" Style="display: none" runat="server" Text="Continue" OnClick="btnContinue_Click" />
</div>

<div class="panel panel-grey" id="dvPropGame" runat="server">
    <div class="panel-heading">Select a category below</div>
    <div class="panel-body">
        <asp:Repeater ID="rptPropGame" runat="server">
            <HeaderTemplate>
                <table class="table table-bordered table-hover">
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <div style="height: 5px"></div>
                        <asp:LinkButton ID="lbtPropType" runat="server" Text='<%#Container.DataItem%>' OnClick="lbtPropType_Click"></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>


<script language="javascript">
    function GameTypeClick(btnID) {
        $('#' + btnID).click();
    }
    $(document).ready(function () {
        $('div[id$="divrptGameTypeShort"] .panel .panel-body div').each(function () {
            if ($(this).find('ul li').length == 0)
                $(this).remove();
        });
    });
</script>

