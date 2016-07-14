<%@ Control Language="VB" AutoEventWireup="false" CodeFile="historyGrid.ascx.vb"
    Inherits="SBSWebsite.historyGrid" %>
<asp:DataGrid ID="grdHistory" runat="server" AutoGenerateColumns="false"  
     CssClass="table table-hover table-bordered"  >
    <HeaderStyle CssClass="tableheading"  HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
    <AlternatingItemStyle HorizontalAlign="Center" />
    <SelectedItemStyle BackColor="YellowGreen" />
    <FooterStyle  HorizontalAlign="Center" />
    <Columns>
        <asp:TemplateColumn HeaderText="CAgent">
            <ItemTemplate>
                <asp:Label ID="lblCAgent" runat="server" />
                  <asp:HiddenField ID="hfCAgent" runat="server" Value='<%# BindCAgent(Container.DataItem) %>' />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Method" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
               
                <asp:Label ID="lblUserPhone" runat="server" /> <BR/>
                 <asp:LinkButton ID="btnAssignRecording" runat="server"  
                Text="Assign Recording" Visible="false" /> <BR/>
                <asp:HiddenField ID="hfTicketID" runat="server" Value='<%# Container.DataItem("TicketID") %>' />
                <asp:panel ID="pnlPhoneDetail" runat="server" Visible="false">
                <span style="line-height:25px">Taken By </span> <BR/>
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
        <asp:TemplateColumn HeaderText="Game Date" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblPlaced" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        
       <asp:TemplateColumn HeaderText="Agent"  Visible="false">
            <ItemTemplate>
                <asp:Label ID="lblAgent" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        
        <asp:TemplateColumn HeaderText="Sport Type">
            <ItemTemplate>
                <asp:Label ID="lblSport" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Player">
            <ItemTemplate>
                <asp:Label ID="lblPlayer" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Description">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Status">
            <ItemTemplate>
                <asp:Label ID="lblResult" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Risk / Win" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblRiskWin" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Amount" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblAmount" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Balance" ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblBalance" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Score (A/H)" ItemStyle-HorizontalAlign="Center"
            ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:Label ID="lblScore" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>