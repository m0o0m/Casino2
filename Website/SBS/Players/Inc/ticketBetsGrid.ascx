<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ticketBetsGrid.ascx.vb"
    Inherits="SBSPlayer.ticketBetsGrid" %>
      
<asp:DataGrid ID="dgTicketBets" runat="server" Width="98%" AutoGenerateColumns="false" 
    CssClass="table table-hover table-bordered" align="center">
    <HeaderStyle CssClass="tableheading" HorizontalAlign="Center"  />
    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
    <AlternatingItemStyle HorizontalAlign="Center" />
    <SelectedItemStyle BackColor="#e6e6e6" />
    <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
    <Columns>
        <asp:TemplateColumn HeaderText="Method" ItemStyle-HorizontalAlign="Center">
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
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<table id="pnColor" visible=false  runat="server" class="table table-hover table-bordered">
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