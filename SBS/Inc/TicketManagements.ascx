<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TicketManagements.ascx.vb" Inherits="SBS_Inc_TicketManagements" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="row">
    <div class="col-lg-12">
        <span>Agent</span>
        <wlb:CDropDownList ID="ddlAgents" OptionalText="All" OptionalValue="All" hasOptionalItem="true" Width="230px"
            AutoPostBack="true" runat="server" CssClass="form-control" Style="display: inline-block;">
        </wlb:CDropDownList>
        &nbsp;  &nbsp; 
        <span>Player</span>
        <wlb:CDropDownList ID="ddlPlayer" hasOptionalItem="true" OptionalText="All" OptionalValue=""
            runat="server" CssClass="form-control" Style="display: inline-block;" Width="250px">
        </wlb:CDropDownList>
        &nbsp;  &nbsp; 
        <span>Ticket Number</span>
        <asp:TextBox ID="txtTicket" Width="90" MaxLength="10" runat="server" CssClass="form-control" Style="display: inline-block;"></asp:TextBox>
        &nbsp;  &nbsp; 
        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-default" />
        &nbsp; 
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" />
        &nbsp; 
        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-red" />
        &nbsp; 
        <asp:Button ID="btnUndelete" runat="server" Text="UnDelete" CssClass="btn btn-green" />
        <wlb:CButtonConfirmer ID="cfrmTicket" runat="server" AttachTo="btnDelete" ConfirmExpression="confirm('Are you sure to delete ticket(s)?')">
        </wlb:CButtonConfirmer>
        <wlb:CButtonConfirmer ID="cfUnDelete" runat="server" AttachTo="btnUndelete" ConfirmExpression="confirm('Are you sure to undelete ticket(s)?')">
        </wlb:CButtonConfirmer>
    </div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <asp:DataGrid ID="grdTickets" runat="server" AutoGenerateColumns="false"
            CssClass="table table-hover table-bordered" AllowPaging="false" ShowFooter="true">
            <PagerStyle Mode="NumericPages" />
            <HeaderStyle CssClass="tableheading2" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <AlternatingItemStyle HorizontalAlign="Center" />
            <SelectedItemStyle BackColor="YellowGreen" />
            <FooterStyle CssClass="tableheading2" HorizontalAlign="Center" />
            <Columns>
                <asp:TemplateColumn HeaderText="Player" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblPlayer" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Internet/Phone" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblUserPhone" runat="server" />
                        <asp:HiddenField ID="hfTicketID" runat="server" Value='<%# Container.DataItem("TicketID") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Ticket" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblTicket" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Date" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblTicketDate" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Sport">
                    <ItemTemplate>
                        <asp:Label ID="lblSport" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Description">
                    <ItemTemplate>
                        <asp:Label ID="lblDescription" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Risk / Win" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblRiskWin" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Amount" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblAmount" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Result">
                    <ItemTemplate>
                        <asp:Label ID="lblResult" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Score (Away/Home)" ItemStyle-HorizontalAlign="Center"
                    ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label ID="lblScore" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Game Date" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblPlaced" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-xs btn-blue" CommandName="DELETE" Visible="false"
                            CommandArgument='<%# container.DataItem("TicketID") %>' />
                        <asp:HiddenField ID="HFPlayerID" Value='<%# container.DataItem("PlayerID") %>' runat="server" />
                        <asp:HiddenField ID="HFTicketType" Value='<%# container.DataItem("TicketType") %>'
                            runat="server" />
                        <asp:HiddenField ID="HFTicketNumber" Value='<%# container.DataItem("TicketNumber") %>'
                            runat="server" />
                        <asp:HiddenField ID="HFSubTicketNumber" Value='<%# container.DataItem("SubTicketNumber") %>'
                            runat="server" />
                        <asp:HiddenField ID="HFRiskAmount" Value='<%# container.DataItem("RiskAmount") %>'
                            runat="server" />
                        <asp:HiddenField ID="HFWinAmount" Value='<%# container.DataItem("WinAmount") %>'
                            runat="server" />
                        <asp:HiddenField ID="HFNetAmount" Value='<%# container.DataItem("NetAmount") %>'
                            runat="server" />
                        <asp:CheckBox ID="chkDelete" runat="server" Text="Del" ValidationGroup='<%# container.DataItem("TicketID") %>' Visible='<%# Container.DataItem("TicketStatus").ToString().ToLower() = "open" OrElse Container.DataItem("TicketStatus").ToString().ToLower() = "pending"%>' />
                        <%#IIf(Container.DataItem("TicketBetStatus") = "Deleted", "<b>Deleted</b><br />", "")%>
                        <asp:CheckBox ID="chkUnDelete" runat="server" Text="UnDel" ValidationGroup='<%# container.DataItem("TicketID") %>' Visible='<%# Container.DataItem("TicketBetStatus") = "Deleted"%>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</div>
