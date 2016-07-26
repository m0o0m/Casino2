<%@ Control Language="VB" AutoEventWireup="false" CodeFile="historyGrid.ascx.vb"
    Inherits="SBSWebsite.historyGrid" %>
<asp:DataGrid ID="grdHistory" runat="server" Width="100%"  AutoGenerateColumns="false"  
     CssClass="table table-hover table-bordered table-style-1" align="center">
    <HeaderStyle CssClass="tableheading row-caption" HorizontalAlign="Center"  />
    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="row-content" />
    <AlternatingItemStyle HorizontalAlign="Center" />
    <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
    <Columns>
        <asp:TemplateColumn HeaderText="CAgent">
            <ItemStyle Width="5%" />
            <ItemTemplate>
                <asp:Label ID="lblCAgent" runat="server" />
                  <asp:HiddenField ID="hfCAgent" runat="server" Value='<%# BindCAgent(Container.DataItem) %>' />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="**" ItemStyle-HorizontalAlign="Center">
            <ItemStyle Width="3%" />
            <ItemTemplate>
                <span class="btn-show-game-details toggle-detail icon-toggle-1 h14px w14px mgR10" onclick="ShowGameDetail(this);" data-ticket-id="<%# Container.DataItem("TicketID") %>"></span>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Date Time Accepted" ItemStyle-HorizontalAlign="Center">
            <ItemStyle Width="10%"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblTicketDate" runat="server" />
                <asp:Label ID="lblUserPhone" runat="server" />
                <asp:HiddenField ID="hfTicketID" runat="server" Value='<%# Container.DataItem("TicketID") %>' />
                <asp:panel ID="pnlPhoneDetail" runat="server" Visible="false">
                <span>Taken By </span> <BR/>
                <asp:Label ID="lblCAgentName" runat="server"  ></asp:Label> <BR />
                <asp:LinkButton ID="lbtRecord" runat ="server" Visible="false" Text ="Recording"></asp:LinkButton>
                </asp:panel>
                <asp:HiddenField ID="hfFileName" runat="server" Value='<%#Container.DataItem("RecordingFile") %>' />  
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Ticket #" ItemStyle-HorizontalAlign="Center">
            <ItemStyle Width="8%"></ItemStyle>
            <ItemTemplate>
                <b><asp:Label ID="lblTicketNumber" runat="server" /></b>
            </ItemTemplate>
        </asp:TemplateColumn>
         <asp:TemplateColumn HeaderText="Player">
             <ItemStyle Width="10%"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblPlayer" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Agent"  Visible="false">
            <ItemTemplate>
                <asp:Label ID="lblAgent" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Taken" ItemStyle-HorizontalAlign="Center">
            <ItemStyle Width="5%"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblMethod" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Status">
             <ItemStyle Width="5%" HorizontalAlign="Left"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblTicketStatus" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Game">
            <ItemStyle Width="45%" HorizontalAlign="Left"></ItemStyle>
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
    </Columns>
</asp:DataGrid>


<script>
    function ShowGameDetail(element) {
        var
            $this = $(element),
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

                $.ajax({
                    type: "POST",
                    url: "/SBS/Players/GetGameDetail.aspx/GetGameDetailForHistory",
                    data: '{ticketId: "' + ticketId + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var borderColor = $tr.css("background-color");
                        $tr.after('<tr id="game-detail-' + ticketId + '" class="row-detail"><td colspan="9"></td></tr>');
                        $("#game-detail-" + ticketId + " > td").html(response.d).css("border-color", borderColor);
                        $("#game-detail-" + ticketId + " > td td, #game-detail-" + ticketId + " > td table").css("border-color", borderColor);
                        $this.addClass("open");
                        console.log(borderColor);
                    }
                });
            }
        }
    }
</script>