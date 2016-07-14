<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ParlayAndReverseRules.ascx.vb"
    Inherits="SBCSuperAdmin.ParlayAndReverseRules" %>
<div style="width:98%;margin:0 auto">
    <asp:Button ID="btnSaveAll" OnClick="SaveALLParlaySetting" runat="server" Text="Save All" style="Float:right;margin:5px;margin-right:0" />
</div>
    
<asp:Repeater ID="rptPRRules" runat="server">
    <ItemTemplate>
        <asp:DataGrid ID="grPRRules" runat="server" Width="98%" AutoGenerateColumns="false"
            OnItemDataBound="grPRRules_ItemDataBound" CellPadding="1" CellSpacing="2" CssClass="gamebox"
            align="center" BorderWidth="2px">
            <HeaderStyle CssClass="tableheading2" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <AlternatingItemStyle HorizontalAlign="Center" />
            <SelectedItemStyle BackColor="YellowGreen" />
            <FooterStyle CssClass="tableheading2" HorizontalAlign="Center" />
            <Columns>
                <asp:TemplateColumn ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:Label ID="lblBetType" runat="server" Text='<%#  Container.DataItem.Value %>' />
                        <asp:HiddenField ID="HFGameType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSpread" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HFSpread" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Total Points" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkTotalPoints" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HFTotalPoints" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Money Line" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkMoneyLine" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HFMoneyLine" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="1H Spread" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk1HSpread" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF1HSpread" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="1H Total Points" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk1HTotalPoint" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF1HTotalPoint" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="1H Money Line" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk1HMoneyline" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF1HMoneyline" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="2H Spread" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk2HSpread" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF2HSpread" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="2H Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk2HTotalPoints" runat="server" 
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF2HTotalPoints" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Parlay" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkParlay" runat="server" 
                            Checked="false" Visible="true" />
                        <asp:HiddenField ID="HFParlay" runat="server" Value='<%#  Container.DataItem.Key %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Reverse" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkReverse" runat="server" 
                            Checked="false" Visible="true" />
                        <asp:HiddenField ID="HFReverse" runat="server" Value='<%#  Container.DataItem.Key %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </ItemTemplate>
</asp:Repeater>
