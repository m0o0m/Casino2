<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerLimitManagement.ascx.vb"
    Inherits="SBCWebsite.JuiceManagement" %>
<style type="text/css">
    .FormatTextRight { text-align: right !important; }

    .Juice { text-align: center; border-style: solid; border-width: 1px; }
</style>
<asp:HiddenField ID="hfSportType" runat="server" />
<asp:HiddenField ID="hfPlayerTemplateID" runat="server" />

<div id="PlayerLimitManager" runat="server" class="panel panel-grey">
    <div class="panel-heading">Limit for each <%=SportType%> type</div>
    <div class="panel-body">
        <div id="divMaxTeaser" runat="server" class="form-group">
            <label class="control-label col-md-2">Max for Teaser</label>
            <div class="col-md-2">
                <asp:TextBox ID="txtMaxForTeaser" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <input id="btnTeaser" type="button" value="Set" class="btn btn-primary" 
                    onclick="$('#pnSoccer').find('.Teaser').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<%=txtMaxForTeaser.ClientID%>').val());}});" />
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-2">Max for Parlay & Reverse</label>
            <div class="col-md-2">
                <asp:TextBox ID="txtMaxForParlay" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <input id="btnSave" type="button" value="Set" class="btn btn-primary" 
                    onclick="$('#pnSoccer').find('.Parlay').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<%=txtMaxForParlay.ClientID%>').val());}});" />
            </div>
        </div>
        <div id="divSoccer" runat="server" class="form-group">
            <label class="control-label col-md-2">All Soccer</label>
            <div class="col-md-2">
                <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <input type="button" value="SET" class="btn btn-primary" 
                    onclick="$('#pnSoccer').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<%=txtGameTypeLimit.ClientID %>').val());}});" />
            </div>
        </div>
        <div class="clearfix"></div>
        <div class="form-group"></div>
        <div id="pnSoccer">
            <asp:Repeater ID="rptPlayerLimitFootBall" runat="server" OnItemDataBound="rptPlayerLimitFootBall_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%#  Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>
                    <asp:DataGrid ID="grPlayerLimitFootBall" OnItemDataBound="rptPlayerLimitFootBall2_DataBound" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered <%#  Container.DataItem.Key%>"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="100px">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%# Container.DataItem.Item("Limit")%>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1Q">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1Q" runat="server" Width="95%" MaxLength="5" Text='<%# bindColumn("1Q", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF1Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2Q">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2Q" runat="server" Width="95%" MaxLength="5" Text='<%# bindColumn("2Q", Container.DataItem)%>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF2Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="3Q" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt3Q" runat="server" Width="95%" MaxLength="5" Text='<%# bindColumn("3Q", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF3Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="4Q" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt4Q" runat="server" Width="95%" MaxLength="5" Text='<%# bindColumn("4Q", Container.DataItem)  %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF4Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="1H" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem)  %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stH" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="2H" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%#bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndH" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Full Game" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtFullGame" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="97%" MaxLength="5" Text='<%#bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="97%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center" HeaderStyle-Wrap="true">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="97%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptPlayerLimitBasketball" runat="server" OnItemDataBound="rptPlayerLimitBasketball_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%#  Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>
                    <asp:DataGrid ID="grPlayerLimitBasketball" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="center"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="100px">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#  Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1Q">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1Q" runat="server" Width="95%" MaxLength="5" Text='<%# bindColumn("1Q", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF1Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2Q">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2Q" runat="server" Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2Q", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF2Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="3Q" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt3Q" runat="server" Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("3Q", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF3Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="4Q" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt4Q" runat="server" Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("4Q", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'></asp:TextBox>
                                    <asp:HiddenField ID="HF4Q" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="1H" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stH" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="2H" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndH" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Full Game" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtFullGame" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptPlayerLimitHockey" runat="server" OnItemDataBound="rptPlayerLimitHockey_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%# Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>

                    <asp:DataGrid ID="grPlayerLimitHockey" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="left"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="50px">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#  Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Puck Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtPuckLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <div class="form-group">
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">1H= 1st Period
                            </span>
                        </div>
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">2H= 2nd + 3rd Period</span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptPlayerLimitBaseball" runat="server" OnItemDataBound="rptPlayerLimitBaseball_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%#  Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                        </div>
                    </div>
                    <asp:DataGrid ID="grPlayerLimitBaseball" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="left"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#   Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1st5innings" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFLast4innings" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Run Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtRunLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current" , Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <div class="form-group">
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">1H= First 5 innings</span>
                        </div>
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">2H= Last 4 innings</span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Repeater ID="rptPlayerLimitSoccer" runat="server" OnItemDataBound="rptPlayerLimitSoccer_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%#  Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>
                    <asp:DataGrid ID="grPlayerLimitSoccer" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="left"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#  Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="left" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stH" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndH" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Full Game" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtFullGame" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))  %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptPlayerLimitTennis" runat="server" OnItemDataBound="rptPlayerLimitTennis_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%# Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>

                    <asp:DataGrid ID="grPlayerLimitTennis" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="left"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="50px">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#  Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Puck Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtPuckLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <div class="form-group">
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">1H= 1st Period</span>
                        </div>
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">2H= 2nd + 3rd Period</span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptPlayerLimitOther" runat="server" OnItemDataBound="rptPlayerLimitOther_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%# Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>

                    <asp:DataGrid ID="grPlayerLimitOther" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="left"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="50px">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#  Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Puck Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtPuckLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <div class="form-group">
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">1H= 1st Period</span>
                        </div>
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">2H= 2nd + 3rd Period</span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptPlayerLimitGolf" runat="server" OnItemDataBound="rptPlayerLimitGolf_DataBound">
                <ItemTemplate>
                    <div class="form-group">
                        <asp:Label ID="lblGameType" runat="server" CssClass="control-label col-md-2" Text='<%# Container.DataItem.Value %>' />
                        <div class="col-md-2">
                            <asp:TextBox ID="txtGameTypeLimit" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <input id="btnSave" type="button" value="Set For Game Type" class="btn btn-pink" onclick="$('#<asp:Literal ID='lblrptFootBallID' runat='server'></asp:Literal>').find('input').each(function addCheckBox(index, input) {if (input.type == 'text') {$(input).val($('#<asp:Literal ID='lbltxtFootBallID' runat='server'></asp:Literal>').val());}});" />
                            <asp:HiddenField ID="hfGameType" runat="server" Value='<%#Container.DataItem.Key %>' />
                        </div>
                    </div>
                    <asp:DataGrid ID="grPlayerLimitGolf" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered" align="left"
                        GridLines="none">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <AlternatingItemStyle HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <FooterStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderStyle-Width="50px">
                                <ItemTemplate>
                                    <nobr> <asp:Label ID="lblLimit" runat="server" Text='<%#  Container.DataItem.Item("Limit") %>' /></nobr>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="1H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt1H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("1H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF1stPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="2H">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt2H" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>' Width="95%"
                                        MaxLength="5" Text='<%# bindColumn("2H", Container.DataItem) %>' onkeypress="javascript:return inputNumber(this,event, false);"
                                        onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HF2ndPeriod" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Puck Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtPuckLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%# bindColumn("Current", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                    <asp:HiddenField ID="HFFullGame" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Money Line" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMoneyLine" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("MoneyLine", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Points" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTotalPoints" runat="server" CssClass='<%#IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Reverse"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Parlay"),"Parlay form-control FormatTextRight",IIF(Container.DataItem.Item("Limit").ToString().Equals("Max Teaser"),"form-control FormatTextRight Teaser","form-control FormatTextRight")))   %>'
                                        Width="95%" MaxLength="5" Text='<%#bindColumn("TotalPoints", Container.DataItem) %>'
                                        onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:return formatNumber(this,2);"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <div class="form-group">
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">1H= 1st Period</span>
                        </div>
                        <div class="col-md-12">
                            <span style="color: Maroon;" class="pull-left">2H= 2nd + 3rd Period</span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
