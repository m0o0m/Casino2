<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    MaintainScrollPositionOnPostback="true" CodeFile="PresetRules.aspx.vb" Inherits="SBSSuperAdmin.PresetRules"
    Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/Agents/FixedSpreadMoney.ascx" TagName="FixedSpreadMoney"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/GameCircledSettings.ascx" TagName="GameCircledSettings" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/Agents/TimeLineOff.ascx" TagName="TimeLineOff" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/TeaserAllow.ascx" TagName="TeaserAllow" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SuperAdmins/MoneyLineOff.ascx" TagName="MoneyLineOff" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SuperAdmins/JuiceControl.ascx" TagName="JuiceControl" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SuperAdmins/FixSpreadMoney.ascx" TagName="FixSpreadMoney" TagPrefix="uc1" %>
<%@ Register Src="~/Inc/SuperAdmins/TimeOff2H.ascx" TagName="TimeOff2H" TagPrefix="uc" %>
<%@ Register Src="~/Inc/OddsRulesSetup.ascx" TagName="OddsRulesSetup" TagPrefix="uc2" %>
<%@ Register Src="~/Inc/Agents/GamePartTimeDisplaySetup.ascx" TagName="GamePartTimeDisplaySetup"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/Agents/MaxPerGame24h.ascx" TagName="MaxPerGame24h"
    TagPrefix="uc" %>
<%@ Register Src="~/Inc/Agents/QuarterDisplaySetup.ascx" TagName="TeamTotalDisplaySetup" TagPrefix="uc3" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:UpdatePanel ID="aa" runat="server">
        <ContentTemplate>

            <div class="row">
                <div class="col-lg-6">
                    <uc:FixedSpreadMoney ID="ucFixedSpreadMoney" runat="server"></uc:FixedSpreadMoney>
                </div>
                <div class="col-lg-6">
                    <div class="panel panel-grey">
                        <div class="panel-heading">Game Line Restriction</div>
                        <div class="panel-body">
                            <table class="table">
                                <tr>
                                    <td>Sport Type
                                    </td>
                                    <td colspan="2">
                                        <wlb:CDropDownList ID="ddlSportType" runat="server" OptionalText="" OptionalValue=""
                                            AutoPostBack="true" hasOptionalItem="true" CssClass="textInput">
                                        </wlb:CDropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Betting Period
                                    </td>
                                    <td colspan="2">
                                        <asp:DropDownList ID="ddlContext" runat="server" AutoPostBack="true">
                                            <asp:ListItem Text="Choose one" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Game" Value="CURRENT"></asp:ListItem>
                                            <asp:ListItem Text="1H" Value="1H"></asp:ListItem>
                                            <asp:ListItem Text="2H" Value="2H"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Away Spread Money
                                    </td>
                                    <td>Greater than<asp:TextBox ID="txtAwaySpreadGT" Width="50px" onkeypress="javascript:return inputNumber(this,event, true);"
                                        CssClass="textInput" runat="server"></asp:TextBox>
                                    </td>
                                    <td>Lower than<asp:TextBox ID="txtAwaySpreadLT" Width="50px" onkeypress="javascript:return inputNumber(this,event, true);"
                                        CssClass="textInput" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Home Spread Money
                                    </td>
                                    <td>Greater than<asp:TextBox ID="txtHomeSpreadGT" Width="50px" onkeypress="javascript:return inputNumber(this,event, true);"
                                        CssClass="textInput" runat="server"></asp:TextBox>
                                    </td>
                                    <td>Lower than<asp:TextBox ID="txtHomeSpreadLT" Width="50px" onkeypress="javascript:return inputNumber(this,event, true);"
                                        CssClass="textInput" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Total Point
                                    </td>
                                    <td>Greater than<asp:TextBox ID="txtTotalPointGT" onkeypress="javascript:return inputNumber(this,event, true);"
                                        CssClass="textInput" Width="50px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>Lower than<asp:TextBox ID="txtTotalPointLT" onkeypress="javascript:return inputNumber(this,event, true);"
                                        CssClass="textInput" Width="50px" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:Button Style="float: right" ID="btnSave" CssClass="btn btn-primary" runat="server" Text="Save" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                </div>
                <div class="clearfix"></div>
            </div>
            <div class="row">
                <div class="col-lg-6">
                    <uc1:GameCircledSettings ID="ucGameCircledSettings" runat="server"></uc1:GameCircledSettings>
                </div>
                <div class="col-lg-6">
                    <div class="panel panel-grey">
                        <div class="panel-heading">Money Line Off</div>
                        <div class="panel-body">
                            <uc1:MoneyLineOff ID="ucMoneyLineOff" runat="server" />
                        </div>
                    </div>
                </div>
                <div class="clearfix"></div>
            </div>

            <div class="row">
                <div class="col-lg-6">
                    <div class="panel panel-grey">
                        <div class="panel-heading">Total Amount Per Game Risk Control</div>
                        <div class="panel-body">
                            <uc2:OddsRulesSetup ID="OddsRulesSetup1" runat="server" />
                        </div>
                    </div>
                    <div class="panel panel-grey">
                        <div class="panel-heading">Full Time Display Setup</div>
                        <div class="panel-body">
                            <uc1:TimeLineOff ID="ucTimeLineOff" runat="server" ContextDisPlay="Current" />
                        </div>
                    </div>

                    <div class="panel panel-grey">
                        <div class="panel-heading">1H Display Setup</div>
                        <div class="panel-body">
                            <uc1:TimeLineOff ID="ucTimeLineOffHalf" runat="server" ContextDisPlay="FirstHalf" />
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="panel panel-grey">
                        <div class="panel-heading">1H & 2H Display Setup</div>
                        <div class="panel-body">
                            <uc:TimeOff2H ID="ucTimeOff2H" runat="server" />
                            <uc:GamePartTimeDisplaySetup ID="ucGamePartTimeDisplaySetup" runat="server" />
                        </div>
                    </div>
                    <uc3:TeamTotalDisplaySetup ID="ucTeamTotalDisplaySetup1" runat="server" />
                    <uc1:JuiceControl ID="ucJuiceControl" runat="server" />
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="mbxl"></div>
            <div class="row">
                <div class="col-lg-6">
                    <uc:MaxPerGame24h ID="ucMaxPerGame24h" runat="server" />
                </div>
                <div class="col-lg-6">
                    <div class="panel panel-grey">
                        <div class="panel-heading">Teaser Allowed</div>
                        <div class="panel-body">
                            <uc1:TeaserAllow ID="ucTeaserAllow" runat="server" />
                        </div>
                    </div>
                    <h4></h4>
                    <%--<div class="panel panel-grey">
                        <div class="panel-heading">Fixed Spread Money (For Football and Basketball only)</div>
                        <div class="panel-body">
                            <uc1:FixSpreadMoney ID="ucFixSpreadMoney" runat="server" />
                        </div>
                    </div>--%>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="mbxl"></div>

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="up1" runat="server" DisplayAfter="300">
        <ProgressTemplate>
            <div id="divUpdateProgressModal" class="modalBackground">
            </div>
            <div class="ProcessingPanel" id="divProcessPnl" style="left: 400px; top: 200px; position: fixed">
                <img alt="processing" src="/images/processing.gif" style="vertical-align: text-bottom;" />
                &nbsp;&nbsp; <b>Processing. Please wait...</b>
            </div>
            <script type="text/javascript">

                // This help to fill the gray cover to full of the screen
                // 1.get instance of the PageRequestManager
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                // 2.register 'what i'm gonna do is SHOW the gray screen' when init the Request ;)
                prm.add_initializeRequest(InitializeRequest);
                // 3.and 'the gray screen' should disapear when the Request end
                prm.add_endRequest(EndRequest);

                function InitializeRequest(sender, args) {
                    //                    if (Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                    //                        alert('One postback at a time please');
                    //                        args.set_cancel(true);
                    //
                    grayOut(true, "");
                }

                function EndRequest(sender, args) {
                    //debugger;
                    var e = args.get_error();

                    if (e != null) {
                        args.set_errorHandled(true);
                        alert(e.message.replace("Sys.WebForms.PageRequestManagerServerErrorException: ", ""));
                    }

                    grayOut(false, "");
                    setIsGrayOut(true);
                }
                var _IsShowGrayOut = true;
                function setIsGrayOut(val) {
                    _IsShowGrayOut = val;
                }
                function GrayOutAfterRequest() {
                    setTimeout("grayOut(true,'')", 0);
                }
                function grayOut(vis, options) {
                    var optionsoptions = options || {};
                    var zindex = options.zindex || 9;
                    var opacity = options.opacity || 50;
                    var opaque = (opacity / 100);
                    var bgcolor = options.bgcolor || '#5b5a58';
                    var dark = document.getElementById('darkenScreenObject');
                    if (!dark) {
                        // The dark layer doesn't exist, it's never been created.  So we'll    
                        // create it here and apply some basic styles.     
                        var tbody = document.getElementsByTagName("body")[0];
                        var tnode = document.createElement('div');
                        tnode.style.position = 'absolute';
                        tnode.style.top = '0px';
                        tnode.style.left = '0px';
                        tnode.style.overflow = 'hidden';
                        tnode.style.display = 'none';
                        tnode.id = 'darkenScreenObject';
                        tbody.appendChild(tnode);
                        dark = document.getElementById('darkenScreenObject');
                    }

                    if (vis) {
                        dark.style.opacity = opaque;
                        dark.style.MozOpacity = opaque;
                        dark.style.filter = 'alpha(opacity=' + opacity + ')';
                        dark.style.zIndex = zindex;
                        dark.style.backgroundColor = bgcolor;
                        dark.style.width = document.body.scrollWidth + 'px';
                        dark.style.minWidth = '100%';
                        dark.style.height = document.body.scrollHeight + 'px';
                        dark.style.minHeight = '100%';
                        dark.style.display = 'block';
                        $('#' + '<%=up1.ClientID %>').css('display', '');
                        $('#divProcessPnl').css('display', '');
                    }
                    else {
                        dark.style.display = 'none';
                        $('#' + '<%=up1.ClientID %>').css('display', 'none');
                        $('#divProcessPnl').css('display', 'none');
                    }

                    if (!_IsShowGrayOut && vis) {
                        dark.style.display = 'none';
                        $('#' + '<%=up1.ClientID %>').css('display', 'none');
                        $('#divProcessPnl').css('display', 'none');
                    }
                }
            </script>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

