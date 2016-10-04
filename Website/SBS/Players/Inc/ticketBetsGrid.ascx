<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ticketBetsGrid.ascx.vb"
    Inherits="SBSPlayer.ticketBetsGrid" %>
<div id="open-bet-print">
    <asp:DataGrid ID="dgTicketBets" runat="server" Width="100%" AutoGenerateColumns="false"
        CssClass="table table-hover table-bordered table-style-10 open-bet-print" align="center">
        <HeaderStyle CssClass="tableheading row-caption" HorizontalAlign="Center" />
        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="row-content" />
        <AlternatingItemStyle HorizontalAlign="Center" />
        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
        <Columns>
            <asp:TemplateColumn HeaderText="**" ItemStyle-HorizontalAlign="Center" Visible="False">
                <ItemStyle Width="3%" />
                <ItemTemplate>
                    <span class="btn-show-game-details toggle-detail icon-toggle-1 h14px w14px mgR10" data-ticket-id="<%# Container.DataItem("TicketID") %>"></span>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Date Time Accepted" ItemStyle-HorizontalAlign="Center">
                <ItemStyle Width="10%"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblTicketDate" runat="server" />
                    <asp:Label ID="lblUserPhone" runat="server" />
                    <asp:HiddenField ID="hfTicketID" runat="server" Value='<%# Container.DataItem("TicketID") %>' />
                    <asp:Panel ID="pnlPhoneDetail" runat="server" Visible="false">
                        <span>Taken By </span>
                        <br />
                        <asp:Label ID="lblCAgentName" runat="server"></asp:Label>
                        <br />
                        <asp:LinkButton ID="lbtRecord" runat="server" Visible="false" Text="Recording"></asp:LinkButton>
                    </asp:Panel>
                    <asp:HiddenField ID="hfFileName" runat="server" Value='<%#Container.DataItem("RecordingFile") %>' />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Ticket #" ItemStyle-HorizontalAlign="Center">
                <ItemStyle Width="8%"></ItemStyle>
                <ItemTemplate>
                    <b>
                        <asp:Label ID="lblTicketNumber" runat="server" /></b>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Player">
                <ItemStyle Width="10%"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblPlayer" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Taken" ItemStyle-HorizontalAlign="Center">
                <ItemStyle Width="5%"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblMethod" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Game">
                <ItemStyle Width="45%" HorizontalAlign="Left" CssClass="td-rowspan"></ItemStyle>
                <ItemTemplate>
                    <div class="gm-type">
                        <asp:Literal ID="ltrIfBet" runat="server"></asp:Literal>
                    </div>
                    <div class="gm-sportname-team baseline">
                        <asp:Literal ID="ltrSportGameTeam" runat="server"></asp:Literal>
                    </div>
                    <asp:Literal ID="ltrGameTeam" runat="server"></asp:Literal>
                    <div class="baseline fz11">
                        <asp:Literal ID="ltrRiskWin" runat="server"></asp:Literal>
                    </div>
                    <asp:HiddenField ID="hfBetType" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Wager Type">
                <ItemStyle Width="10%"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblWagerType" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Risk">
                <ItemStyle Width="10%"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblRisk" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Win">
                <ItemStyle Width="10%"></ItemStyle>
                <ItemTemplate>
                    <asp:Label ID="lblWin" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Status" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblAction" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="<nobr>Score (A/H)</nobr>" ItemStyle-HorizontalAlign="Center"
                ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblScore" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</div>
<!-- /#open-bet-print-->
<table id="pnColor" visible="False" runat="server" class="table table-hover table-bordered">
    <tr>
        <td style="background: #FFCC66; width: 20px"></td>
        <td>From 300 to 499
        </td>
    </tr>
    <tr>
        <td style="background: #FF9900"></td>
        <td>From 500 to 999
        </td>
    </tr>
    <tr>
        <td style="background: #FF6600"></td>
        <td>From 1000 to 1999
        </td>
    </tr>
    <tr>
        <td style="background: #FF3300"></td>
        <td>From 2000 to 2999
        </td>
    </tr>
    <tr>
        <td style="background: #00DD00"></td>

        <td>From 3000 to 3999
        </td>
    </tr>
    <tr>
        <td style="background: #008800"></td>

        <td>From 4000 to 4999
        </td>
    </tr>
    <tr>
        <td style="background: #FFCC66"></td>
        <td>From 5000 to 5999
        
        </td>
    </tr>
    <tr>
        <td style="background: #0000FF"></td>
        <td>From 6000 to max
        
        </td>
    </tr>
</table>

<script>
    Array.prototype.removeTicketId = function (ticketId, all) {
        for (var i = this.length - 1; i >= 0; i--) {
            if (this[i] === ticketId) {
                this.splice(i, 1);
                if (!all)
                    break;
            }
        }
        return this;
    };

    Array.prototype.isExistedTicketId = function (ticketId) {
        var isExisted = false;
        for (var i = this.length - 1; i >= 0; i--) {
            if (this[i] === ticketId) {
                isExisted = true;
                break;
            }
        }

        return isExisted;
    };

    var ticketIds = new Array();
    $(".btn-show-game-details").click(function () {
        var
            $this = $(this),
            $td = $this.parent(),
            rowspan = $td.attr("rowspan") || 1,
            $tr = $td.parent(),
            ticketId = $this.data("ticket-id");

        if ($(this).hasClass("open")) {
            $(this).removeClass("open");
            $("#game-detail-" + ticketId).fadeOut();

            return;
        } else {
            var detailBox = $("#game-detail-" + ticketId);

            if (detailBox.length > 0) {
                detailBox.slideDown();
                $this.addClass("open");
            } else {
                for (var i = 1; i < rowspan; ++i)
                    $tr = $tr.next();

                if (ticketIds.isExistedTicketId(ticketId)) return;

                ticketIds.push(ticketId);

                $.ajax({
                    type: "POST",
                    url: "/SBS/Players/GetGameDetail.aspx/GetGameDetail",
                    data: '{ticketId: "' + ticketId + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var backgroundColor = $tr.css("background-color");
                        $tr.after('<tr id="game-detail-' + ticketId + '" class="row-detail"><td colspan="8"></td></tr>');
                        $("#game-detail-" + ticketId + " > td").css("background-color", backgroundColor);
                        $("#game-detail-" + ticketId + " > td").html(response.d);
                        $this.addClass("open");
                        ticketIds.removeTicketId(ticketId, false);
                    }
                });


            }
        }
    });


</script>
