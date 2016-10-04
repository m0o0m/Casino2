<%@ Control Language="VB" AutoEventWireup="false" CodeFile="historyGridDetail.ascx.vb"
    Inherits="SBSWebsite.historyGridDetail" %>
<asp:DataGrid ID="grdHistory" runat="server" Width="100%"  AutoGenerateColumns="false"  
     CssClass="table table-hover table-bordered table-style-10" align="center">
    <HeaderStyle CssClass="tableheading row-caption" HorizontalAlign="Center"  />
    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="row-content" />
    <AlternatingItemStyle HorizontalAlign="Center" />
    <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
    <Columns>
        <asp:TemplateColumn HeaderText="**" ItemStyle-HorizontalAlign="Center">
            <HeaderStyle Width="2%"></HeaderStyle>
            <ItemTemplate>
                <span class="btn-show-game-details toggle-detail icon-toggle-1 h14px w14px mgR10" onclick="ShowGameDetail(this);" data-ticket-id="<%# Container.DataItem("TicketID") %>"></span>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Date Time Accepted" ItemStyle-HorizontalAlign="Center">
            <HeaderStyle Width="10%"></HeaderStyle>
            <ItemTemplate>
                <asp:Label ID="lblTicketDate" runat="server" />
                <asp:HiddenField ID="hfTicketID" runat="server" Value='<%# Container.DataItem("TicketID") %>' />
                <asp:HiddenField ID="hfFileName" runat="server" Value='<%#Container.DataItem("RecordingFile") %>' />  
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Ticket #" ItemStyle-HorizontalAlign="Center">
            <HeaderStyle Width="5%"></HeaderStyle>
            <ItemTemplate>
                <b><asp:Label ID="lblTicketNumber" runat="server" /></b>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Description" ItemStyle-HorizontalAlign="Left">
            <HeaderStyle Width="45%" HorizontalAlign="Left"></HeaderStyle>
            <ItemTemplate>
                <div class="gm-type"><asp:Literal ID="ltrIfBet" runat="server"></asp:Literal></div>
                <div class="gm-sportname-team baseline">
                    <asp:Literal ID="ltrSportGameTeam" runat="server"></asp:Literal>
                </div>
                <asp:Literal ID="ltrGameTeam" runat="server"></asp:Literal>
                <div class="baseline fz11">
                    <asp:Literal ID="ltrRiskWin" runat="server"></asp:Literal>
                </div>
                <asp:HiddenField ID="hfBetType" runat="server"/>  
                <asp:HiddenField ID="hfContext" runat="server"/>  
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Type">
            <HeaderStyle Width="10%"></HeaderStyle>
            <ItemTemplate>
                <asp:Label ID="lblWagerType" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Risk">
            <HeaderStyle Width="10%"></HeaderStyle>
            <ItemTemplate>
                <asp:Label ID="lblRisk" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Win">
            <HeaderStyle Width="10%"></HeaderStyle>
            <ItemTemplate>
                <asp:Label ID="lblWin" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Result">
            <HeaderStyle Width="10%"></HeaderStyle>
            <ItemTemplate>
                <asp:Label ID="lblResult" runat="server" CssClass="bold" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>


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
    function ShowGameDetail(element) {
        var
            $this = $(element),
            $td = $this.parent(),
            rowspan = $td.attr("rowspan") || 1,
            $tr = $td.parent(),
            ticketId = $this.data("ticket-id");

        if ($this.hasClass("open")) {
            $this.removeClass("open");
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
                    url: "/SBS/Players/GetGameDetail.aspx/GetGameDetailForHistory",
                    data: '{ticketId: "' + ticketId + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var backgroundColor = $tr.css("background-color");
                        $tr.after('<tr id="game-detail-' + ticketId + '" class="row-detail"><td colspan="9"></td></tr>');
                        $("#game-detail-" + ticketId + " > td").html(response.d).css("background-color", backgroundColor);
                        $this.addClass("open");
                        ticketIds.removeTicketId(ticketId, false);
                    }
                });
            }
        }
    }
</script>