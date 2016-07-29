<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ticketBetsGrid.ascx.vb"
    Inherits="SBSPlayer.ticketBetsGrid" %>
      
<asp:DataGrid ID="dgTicketBets" runat="server" Width="100%" AutoGenerateColumns="false" 
    CssClass="table table-hover table-bordered table-style-1" align="center">
    <HeaderStyle CssClass="tableheading row-caption" HorizontalAlign="Center"  />
    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="row-content" />
    <AlternatingItemStyle HorizontalAlign="Center" />
    <%--<SelectedItemStyle BackColor="#e6e6e6" />--%>
    <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
    <Columns>
        <asp:TemplateColumn HeaderText="**" ItemStyle-HorizontalAlign="Center">
            <ItemStyle Width="3%" />
            <ItemTemplate>
                <span class="btn-show-game-details toggle-detail icon-toggle-1 h14px w14px mgR10" data-ticket-id="<%# Container.DataItem("TicketID") %>"></span>
                <%--<span onclick="ShowDetailWager(event, '24445906','W')" class="icon minus">+</span>--%>
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
            <ItemStyle Width="9%"></ItemStyle>
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
        <asp:TemplateColumn HeaderText="Taken" ItemStyle-HorizontalAlign="Center">
            <ItemStyle Width="5%"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="lblMethod" runat="server" />
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
                <%--<div class="baseline">
                    <b class="gm-number">[25810]</b>&nbsp;<b class="gm-team">Porland Timber - La Galaxy</b>&nbsp;
                    <span class="gm-date">02/23/2016</span>&nbsp;<span class="gm-time">(12:30 PM)</span>&nbsp;
                    <span class="gm-status">(Pending)</span>
                </div>
                <div class="baseline">
                    <asp:Literal ID="ltrGameBet" runat="server"></asp:Literal>
                </div>--%>
                <div class="baseline fz11">
                    <asp:Literal ID="ltrRiskWin" runat="server"></asp:Literal>
                </div>
                <%--<asp:Label ID="lblIfBet" runat="server" />
                <asp:Label ID="lblGameType" runat="server" />
                <asp:Label ID="lblAwayTeam" runat="server" />
                <asp:Label ID="lblRiskWin" runat="server" />--%>
                <asp:HiddenField ID="hfBetType" runat="server"/>  
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
        
<%--        <asp:TemplateColumn HeaderText="Method" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
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
        <asp:TemplateColumn HeaderText="Ticket" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblTicket" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Date Bet" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblTicketDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Player">
            <ItemTemplate>
                <asp:Label ID="lblPlayer" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Game Date" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblPlaced" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Sport Type">
            <ItemTemplate>
                <asp:Label ID="lblSport" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Description">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Status" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblAction" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Risk / Win" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblRiskWin" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="<nobr><a href='#'>Score (A/H)</a></nobr>"   ItemStyle-HorizontalAlign="Center"
            ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblScore" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>--%>
    </Columns>
</asp:DataGrid>
<table id="pnColor" visible="False"  runat="server" class="table table-hover table-bordered">
    <tr>
        <td style="background:#FFCC66; width: 20px"></td>
        <td>
           From 300 to 499
        </td>
    </tr>
     <tr>
        <td style="background:#FF9900"></td>
        <td>
         From 500 to 999
        </td>
    </tr>
    <tr>
        <td style="background:#FF6600"></td>
        <td>
         From 1000 to 1999
        </td>
    </tr>
    <tr>
        <td style="background:#FF3300"></td>
        <td>
         From 2000 to 2999
        </td>
    </tr>
    <tr>
        <td style="background:#00DD00"></td>
        
        <td>
          From 3000 to 3999
        </td>
    </tr>
    <tr>
        <td style="background:#008800"></td>
       
        <td>
            From 4000 to 4999
        </td>
    </tr>
    <tr>
        <td style="background:#FFCC66"></td>
        <td>
            From 5000 to 5999
        
        </td>
    </tr>
      <tr>
        <td style="background:#0000FF"></td>
        <td>
         From 6000 to max
        
        </td>
    </tr>
</table>

<script>
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

                $.ajax({
                    type: "POST",
                    url: "/SBS/Players/GetGameDetail.aspx/GetGameDetail",
                    data: '{ticketId: "' + ticketId + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var borderColor = $tr.css("background-color");
                        $tr.after('<tr id="game-detail-' + ticketId + '" class="row-detail"><td colspan="8"></td></tr>');
                        $("#game-detail-" + ticketId + " > td").html(response.d).css("border-color", borderColor);
                        $("#game-detail-" + ticketId + " > td td, #game-detail-" + ticketId + " > td table").css("border-color", borderColor);
                        $this.addClass("open");
                        console.log(borderColor);
                    }
                });
            }

            
        }
        
    });
</script>