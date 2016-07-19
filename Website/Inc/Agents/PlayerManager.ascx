<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerManager.ascx.vb"
    Inherits="SBSAgents.PlayerManager" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/Inc/PlayerEdit.ascx" TagName="PlayerEdit" TagPrefix="uc1" %>
<script language="javascript">
    function resetAllField() {
        var obj = document.getElementerdById('<%=txtNameOrLogin.ClientID%>');
        alert(obj);
        if (obj) {
            obj.value = '';
            obj.focus();
        }
    }
</script>

<div class="panel panel-grey">
    <div class="panel-heading">Manage Player</div>
    <div class="panel-body">

        <asp:UpdatePanel runat="server" ID="upPlayers">
            <contenttemplate>
        <asp:Panel ID="pnPlayerManager" runat="server">
            <div class="form-group">
                 <div class="col-md-1 w10">
                     <asp:RadioButton ID="rdAllAcct" runat="server" GroupName="Acct" AutoPostBack="true" />
                 </div>
                <div class="col-md-2">
                    <label class="control-label pt2">All Acct</label>
                </div>
                <div class="col-md-1 w10">
                    <asp:RadioButton ID="rdLockAcct" runat="server" GroupName="Acct" AutoPostBack="true" />
                </div>
                <div class="col-md-2">
                    <asp:Label ID="lblLockAcct" CssClass="control-label pt2" runat="server"
                            Text="Locked Acct"></asp:Label>
                </div>
                <div class="col-md-1 w10">
                    <asp:RadioButton ID="rdLockBetAcct" runat="server" GroupName="Acct" AutoPostBack="true" />
                </div>
                <div class="col-md-2">
                    <asp:Label ID="lblLockBetAcct" runat="server"
                            Text="Betting Locked Acct"></asp:Label>
                </div>
            </div>
             <div class="form-group">
                 <div class="col-md-2">
                    <label class="control-label">Name or Login</label>
                 </div>
                 <div class="col-md-2">
                     <asp:TextBox ID="txtNameOrLogin" runat="server" CssClass="form-control"></asp:TextBox>
                 </div>
                  <div class="col-md-2 w140">
                    <label class="control-label">Agent</label>
                 </div>
                 <div class="col-md-3">
                     <cc1:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control"/>
                 </div>
                 <div class="col-md-1">
                     <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-success"/>
                 </div>
                 <div class="col-md-2">
                     <asp:Button CausesValidation="false" CssClass="btn btn-default" Text="Reset" runat="server" ID="btnreset" />
                 </div>
             </div>
            <div class="form-group">
                 <div class="col-md-2">
                     <label class="control-label">Player Template</label>
                 </div>
                <div class="col-md-2">
                    <cc1:CDropDownList ID="ddlplayerTemplate" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-2 w140">
                    <label class="control-label">Player Accounts</label>
                </div>
                <div class="col-md-3">
                    <asp:RadioButtonList ID="rdlNumAcc" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="rdo-group" >
                        <asp:ListItem Value="1" Selected="True">1Acct</asp:ListItem>
                        <asp:ListItem Value="5" Selected="True">5Accts</asp:ListItem>
                        <asp:ListItem Value="10">10Accts</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div class="col-md-2">
                     <asp:Button ID="btnCreate" runat="server" Text="Add New Players" CssClass="btn btn-primary" />
                </div>
            </div>
            
            <asp:DataGrid ID="dgPlayers" runat="server" Width="100%" AutoGenerateColumns="false"
                    CssClass="table table-hover table-bordered" AllowPaging="True"
                PageSize="30">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                <PagerStyle Font-Names="tahoma" HorizontalAlign="Right" Mode="NumericPages" />
                <AlternatingItemStyle HorizontalAlign="Left" />
                <SelectedItemStyle BackColor="YellowGreen" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="left" HeaderText="Name (Login)">
                        <ItemTemplate>
                            <nobr>
                <asp:HiddenField ID="hfPlayerTemplateID" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("PlayerTemplateID")) & "|" & SBCBL.std.SafeString(Container.DataItem("DefaultPlayerTemplateID")) %>' />
                <asp:LinkButton ID="lbtEdit" CssClass="itemplayer" runat="server" CommandArgument='<%#Container.DataItem("PlayerID") %>' CommandName="EditUser" Text='<%#Container.DataItem("Login") & " (" & Container.DataItem("Name")  & ")" %>'></asp:LinkButton> </nobr>
                            <asp:HiddenField ID="hfLock" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("IsLocked")) %>'>
                            </asp:HiddenField>
                            <asp:HiddenField ID="hfBettingLock" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("IsBettingLocked")) %>'>
                            </asp:HiddenField>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Block">
                        <ItemTemplate>
                            <nobr>
            <asp:LinkButton  CssClass="itemplayer" runat="server" style="text-decoration:none"  id="lbnLock" Text='<%#IIf(SBCBL.std.SafeString(Container.DataItem("IsLocked")).Equals("Y"),"Y","N")   %>'  CommandName="LOCK" CommandArgument='<%#  sbcbl.std.safestring(Container.DataItem("PlayerID"))  & "|" & Container.DataItem("Login")  %>' ></asp:LinkButton>
                        
            </nobr>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderText="Block Bet">
                        <ItemTemplate>
                            <nobr>
            <asp:LinkButton  CssClass="itemplayer" style="text-decoration:none"  runat="server" id="lbnBettingLock" Text='<%#IIf(SBCBL.std.SafeString(Container.DataItem("IsBettingLocked")).Equals("Y"),"Y","N")   %>'  CommandName="Betting Lock" CommandArgument='<%#  sbcbl.std.safestring(Container.DataItem("PlayerID"))  & "|" & Container.DataItem("Login")  %>' ></asp:LinkButton>

            </nobr>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="PassWord" HeaderText="PassWord" ItemStyle-HorizontalAlign="Center">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
                        <HeaderTemplate>
                            Account Status
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#IIf(SBCBL.std.SafeString(Container.DataItem("IsLocked")).Equals("Y"), "Locked", "Active")%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
                        <HeaderTemplate>
                            Last Log in
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#formatDate(Eval("LastLoginDate"))%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            Casino
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#IIf(SBCBL.std.SafeString(Container.DataItem("HasCasino")).Equals("Y"), "Yes", "No")%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
                        <HeaderTemplate>
                            Limit
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblLimit" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
                        <HeaderTemplate>
                            Balance
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblBalance" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
                        <HeaderTemplate>
                            Pending
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblPending" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>

            <asp:Label runat="server" ID="lblmsg" ForeColor="Red" Visible="false" Text="No result was found. Please try again later."></asp:Label>
        </asp:Panel>
        <div style="margin-left: 10px; margin-top: 10px" id="PlayerEdit" runat="server" visible="false">
            <uc1:PlayerEdit ID="ucPlayerEdit" runat="server" />
        </div>
    </contenttemplate>
            <triggers>
        <asp:AsyncPostBackTrigger ControlID="btnfilter" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="rdLockAcct" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID="rdLockBetAcct" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID="rdAllAcct" EventName="CheckedChanged" />
       <%-- <asp:AsyncPostBackTrigger ControlID="btnaddnewplayer" EventName="Click" />--%>
        <asp:AsyncPostBackTrigger ControlID="btnreset" EventName="Click" />
    </triggers>
        </asp:UpdatePanel>
    </div>
</div>


<script language="javascript" type="text/javascript">
    Sys.Browser.WebKit = {};
    if (navigator.userAgent.indexOf('WebKit/') > -1) {
        Sys.Browser.agent = Sys.Browser.WebKit;
        Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
        Sys.Browser.name = 'WebKit';
    }


    var newWidth;
    var divPasswordChild;

    function passwordStrength(passwordInputID, divPasswordParentID, divPasswordChildID) {
        var score = 0
        var p = document.getElementById(passwordInputID).value;

        var nScore = calcPasswordcStrength(p);

        // Set new width
        var nRound = Math.round(nScore * 2);

        if (nRound > 100) {
            nRound = 100;
        }

        divPasswordChild = document.getElementById(divPasswordChildID);
        var maxWidth = document.getElementById(divPasswordParentID).offsetWidth - 2;
        //var maxWidth = divPasswordParent.offsetWidth - 2;
        var scoreWidth = (maxWidth / 100) * nRound;
        newWidth = scoreWidth;

        if (nScore < 20)
            document.getElementById(divPasswordChildID).title = "Your password is very weak!";
        else if (nScore > 20 && nScore < 40)
            document.getElementById(divPasswordChildID).title = "Your password is medium strength.";
        else if (nScore > 40)
            document.getElementById(divPasswordChildID).title = "Strong password!";

        document.getElementById(passwordInputID).title = document.getElementById(divPasswordChildID).title;
        setPasswordMeterWidth();
    }

    function setPasswordMeterWidth() {
        //var div = document.getElementById("divPasswordChild");
        var oldWidth = parseInt(divPasswordChild.style.width.replace("px", ""));

        if (navigator.appName != "IE") {
            divPasswordChild.style.width = (oldWidth + 1) + "px";
        }
        else {
            divPasswordChild.style.width = oldWidth + 1;
        }

        oldWidth = oldWidth + 1;
        if (newWidth > oldWidth)
            setTimeout(setPasswordMeterWidth, 20);
        else {
            if (navigator.appName != "IE") {
                divPasswordChild.style.width = newWidth + "px";
            }
            else {
                divPasswordChild.style.width = newWidth;
            }
        }
    }

    function calcPasswordcStrength(p) {
        if (p.length == 0) {
            return 0;
        }
        var intScore = 0;

        // PASSWORD LENGTH
        intScore += p.length;

        if (p.length > 0 && p.length <= 4) {                    // length 4 or less
            intScore += p.length;
        }
        else if (p.length >= 5 && p.length <= 7) {	// length between 5 and 7
            intScore += 6;
        }
        else if (p.length >= 8 && p.length <= 15) {	// length between 8 and 15
            intScore += 12;
            //alert(intScore);
        }
        else if (p.length >= 16) {               // length 16 or more
            intScore += 18;
            //alert(intScore);
        }

        // LETTERS (Not exactly implemented as dictacted above because of my limited understanding of Regex)
        if (p.match(/[a-z]/)) {              // [verified] at least one lower case letter
            intScore += 1;
        }
        if (p.match(/[A-Z]/)) {              // [verified] at least one upper case letter
            intScore += 5;
        }
        // NUMBERS
        if (p.match(/\d/)) {             	// [verified] at least one number
            intScore += 5;
        }
        if (p.match(/.*\d.*\d.*\d/)) {            // [verified] at least three numbers
            intScore += 5;
        }

        // SPECIAL CHAR
        if (p.match(/[!,@,#,$,%,^,&,*,?,_,~]/)) {           // [verified] at least one special character
            intScore += 5;
        }
        // [verified] at least two special characters
        if (p.match(/.*[!,@,#,$,%,^,&,*,?,_,~].*[!,@,#,$,%,^,&,*,?,_,~]/)) {
            intScore += 5;
        }

        // COMBOS
        if (p.match(/(?=.*[a-z])(?=.*[A-Z])/)) {        // [verified] both upper and lower case
            intScore += 2;
        }
        if (p.match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])/)) { // [verified] both letters and numbers
            intScore += 2;
        }
        // [verified] letters, numbers, and special characters
        if (p.match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!,@,#,$,%,^,&,*,?,_,~])/)) {
            intScore += 2;
        }

        return intScore;

    }


</script>
<%--Processing--%>
<asp:UpdateProgress ID="up1" runat="server" DisplayAfter="300">
    <progresstemplate>
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
                controlProfile('<%= SBCBL.std.GetBettingProfileSubAgent(AgentID) %>');
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
    </progresstemplate>
</asp:UpdateProgress>
<script type="text/javascript">

    function controlProfile(enable) {
        if (enable == "False") {
            $child = $("#fsRight").find('input');
            try {
                for (i = 0; i < $child.length; i++) {

                    var sID = $child[i].id;
                    if ($child[i].parentNode.parentNode.parentNode.parentNode.id == "creditLimit" && sID.indexOf("txtAccountBalance") <= 0) {
                        var bdisable;
                        if ("<%= SBCBL.std.GetHasCrediLimitSetting(AgentID) %>" == "False") {
                            bdisable = true;
                        }
                        else {

                            bdisable = false;

                        }

                        $child[i].disabled = bdisable;

                    }
                    else {
                        $child[i].disabled = "false";
                    }
                }
            } catch (e) { alert(e); }
            $child = $("#playerPropfile").find('input');
            for (i = 0; i < $child.length; i++) {
                $child[i].disabled = "false";
            }
        }
        else {

            $child = $("#fsRight").find('input');
            try {
                for (i = 0; i < $child.length; i++) {

                    var sID = $child[i].id;
                    if ($child[i].parentNode.parentNode.parentNode.parentNode.id == "creditLimit" && sID.indexOf("txtAccountBalance") <= 0) {

                        var bdisable;
                        if ("<%= SBCBL.std.GetHasCrediLimitSetting(AgentID) %>" == "False") {
                            bdisable = true;
                        }
                        else {

                            bdisable = false;

                        }

                        $child[i].disabled = bdisable;

                    }
                }
            } catch (e) { alert(e); }


        }
    }
</script>
