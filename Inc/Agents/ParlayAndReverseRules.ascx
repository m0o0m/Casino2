<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ParlayAndReverseRules.ascx.vb"
    Inherits="SBSAgents.ParlayAndReverseRules" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>

<div class="row">
    <div class="form-group">
        <div class="col-md-12">
            <asp:Button ID="btnSaveAll" OnClick="SaveALLParlaySetting" runat="server" Text="Save All"
                CssClass="btn btn-primary" />
        </div>
    </div>
</div>
<div class="mbxl"></div>
<div class="row" id="dvAgents" runat="server">
    <div class="form-group">
        <label class="control-label col-md-1">Agents</label>
        <div class="col-md-2">
            <cc1:CDropDownList runat="server" ID="ddlAgents" AutoPostBack="true" hasOptionalItem="false" CssClass="form-control">
            </cc1:CDropDownList>
        </div>
    </div>
</div>
<div class="mbxl"></div>
<asp:Repeater ID="rptPRRules" runat="server">
    <ItemTemplate>
        <asp:Panel ID="pnMaxParlayInGame" Visible="false" runat="server">
            <span>Max Spread Allowed </span>
                  <asp:TextBox ID="txtMaxParlayInGame" Text='<%#getMaxParlayInGame(Container.DataItem) %>' runat="server" Style="display: inline-block;" Width="170px"
                      onkeypress="javascript:return inputNumber(this,event, true);" CssClass="form-control"></asp:TextBox>
            <asp:Button CssClass="btn btn-primary" ID="btnSaveMaxParlayInGame" runat="server" Text="Save" CommandName="MAX_PARLAY_IN_GAME" CommandArgument="<%#Container.DataItem%>" />
        </asp:Panel>
        <asp:DataGrid ID="grPRRules" runat="server" AutoGenerateColumns="false"
            OnItemDataBound="grPRRules_ItemDataBound" CssClass="table table-hover table-bordered"
            align="center">
            <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <AlternatingItemStyle HorizontalAlign="Center" />
            <SelectedItemStyle BackColor="YellowGreen" />
            <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
            <Columns>
                <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="110px">
                    <ItemTemplate>
                        <asp:Label ID="lblBetType" runat="server" Text='<%#  Container.DataItem.Value %>' />
                        <asp:HiddenField ID="HFGameType" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn ItemStyle-HorizontalAlign="center" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSpread" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HFSpread" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>Total Points</nobr>" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkTotalPoints" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HFTotalPoints" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>Money Line</nobr>" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkMoneyLine" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HFMoneyLine" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>1H Spread</nobr>" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk1HSpread" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF1HSpread" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>1H Total Points</nobr>" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk1HTotalPoint" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF1HTotalPoint" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>1H Money Line</nobr>" ItemStyle-HorizontalAlign="center">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk1HMoneyline" runat="server"
                            Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF1HMoneyline" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>2H Spread</nobr>" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk2HSpread" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF2HSpread" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="<nobr>2H Total Points</nobr>" HeaderStyle-Width="80px"
                    ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                    <ItemTemplate>
                        <asp:CheckBox ID="chk2HTotalPoints" runat="server" Checked="false" Visible="false" />
                        <asp:HiddenField ID="HF2HTotalPoints" runat="server" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Parlay" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkParlay" runat="server" Checked="false" Visible="true" />
                        <asp:HiddenField ID="HFParlay" runat="server" Value='<%#  Container.DataItem.Key %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Reverse" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center"
                    HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkReverse" runat="server" Checked="false" Visible="true" />
                        <asp:HiddenField ID="HFReverse" runat="server" Value='<%#  Container.DataItem.Key %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </ItemTemplate>
</asp:Repeater>
