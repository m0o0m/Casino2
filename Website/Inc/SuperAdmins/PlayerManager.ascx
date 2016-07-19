<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerManager.ascx.vb"
    Inherits="SBCSuperAdmin.PlayerManager" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/SuperAdmins/editPlayerTemplate.ascx" TagName="editPlayerTemplate"
    TagPrefix="uc1" %>
<%@ Register Src="~/Inc/PlayerEdit.ascx" TagName="PlayerEdit" TagPrefix="uc3" %>

<script type="text/javascript" language="javascript">
    function CheckAllPlayers(checkAll) {
        var oGrid = document.getElementById('<%= dgPlayers.ClientID %>');
        var oInputs = oGrid.getElementsByTagName('INPUT');
        for (var index = 0; index < oInputs.length; index++) {
            if (oInputs[index].type.toUpperCase() == 'CHECKBOX') {
                oInputs[index].checked = checkAll.checked;
            }
        }
        if (checkAll.checked) {
            checkAll.title = 'Unselect all'
        }
        else {
            checkAll.title = 'Select all'
        }
    }
    function resetAllField() {
        var obj = document.getElementById('<%=txtNameOrLogin.ClientID %>');
        if (obj) {
            obj.value = '';
            obj.focus();
        }
    }
</script>

<div class="row">
    <div class="col-lg-12 pull-left">
        <label class="control-label">Name or Login</label>
        &nbsp;
    <asp:TextBox ID="txtNameOrLogin" CssClass="form-control" Style="display: inline-block;" MaxLength="50" Width="130px" runat="server" />
        &nbsp;
        <label class="control-label">Agent</label>&nbsp; 
            <wlb:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control" Style="display: inline-block;"
                AutoPostBack="false" hasOptionalItem="false" Width="190px" />
        &nbsp; 
        <label class="control-label">Template</label>&nbsp; 
            <wlb:CDropDownList ID="ddlTemplates" runat="server" CssClass="form-control" hasOptionalItem="true"
                AutoPostBack="false" Style="display: inline-block;" Width="190px" />
        &nbsp;
    <asp:Button ID="btnFilter" runat="server" Text="Filter" ToolTip="Filter" Width="60" CssClass="btn btn-green" />
        &nbsp;
    <input type="button" value="Reset" class="btn btn-default" onclick="resetAllField()" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <span style="margin-right: 26px">Player Template :
            <wlb:CDropDownList ID="ddlplayerTemplate" runat="server" CssClass="form-control" Style="display: inline-block;" Width="190px" />
        </span>
        <span style="margin-right: 10px">Player Accounts :</span>
        <asp:RadioButtonList ID="rdlNumAcc" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
            <asp:ListItem Value="5" Selected="True">5Accts</asp:ListItem>
            <asp:ListItem Value="10">10Accts</asp:ListItem>
        </asp:RadioButtonList>
        <asp:Button ID="btnCreate" runat="server" Text="Add New Players" CssClass="btn btn-primary" Style="margin-left: 17px" />
    </div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <asp:Button ID="btnLock" runat="server" Text="Lock" ToolTip="Lock Player" CssClass="btn btn-default" />
        <asp:Button ID="btnBettingLock" runat="server" Text="Betting Lock" ToolTip="Betting lock Player"
            CssClass="btn btn-default" />
        <asp:Button ID="btnViewLock" runat="server" Text="View Lock" ToolTip="View locked Player"
            CssClass="btn btn-default" />
        <asp:Button ID="btnViewBettingLock" runat="server" Text="View Betting Lock" ToolTip="View locked Betting Player"
            CssClass="btn btn-default" />
        &nbsp;<asp:Button ID="btnAddnewplayer" runat="server" Visible="False" CssClass="btn btn-default" Text="Add new player" />
    </div>
</div>
<div class="mbxl"></div>


<div class="row">
    <div class="col-lg-12">
        <asp:Panel ID="pnPlayerManager" runat="server">
            <asp:DataGrid ID="dgPlayers" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                <AlternatingItemStyle HorizontalAlign="Left" />
                <SelectedItemStyle BackColor="YellowGreen" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20">
                        <HeaderTemplate>
                            <input id="chkChoice" type="checkbox" onclick="CheckAllPlayers(this);" title="Select all"
                                runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox type="checkbox" ID="chkID" runat="server" value='<%# SBCBL.std.SafeString(Container.DataItem("PlayerID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login"))  %>'
                                name='Chosen_<%#Container.DataItem("PlayerTemplateID")%>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Login (Name)" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:HiddenField ID="hfPlayerTemplateID" runat="server" Value='<%# SBCBL.std.SafeString(Container.DataItem("PlayerTemplateID")) & "|" & SBCBL.std.SafeString(Container.DataItem("DefaultPlayerTemplateID")) %>' />
                            <asp:LinkButton ID="lbtnEdit" runat="server" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("PlayerID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>'
                                CommandName="EditPlayer" Text='<%#Container.DataItem("Login") & " (" & Container.DataItem("Name") & ")" %>'
                                CssClass="itemplayer" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="IsBettingLocked" HeaderText="Betting Locked" ItemStyle-Width="85"
                        ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundColumn DataField="IsLocked" HeaderText="Locked" ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" />
                    <asp:TemplateColumn HeaderText="Last Login" ItemStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#formatDate(Eval("LastLoginDate"))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid><br />
        </asp:Panel>
    </div>
</div>

<div class="row">
    <div class="col-lg-12" id="divPlayerEdit" runat="server" visible="false">
        <uc3:PlayerEdit ID="PlayerEdit1" runat="server" />
    </div>
</div>
<div class="mbxl"></div>


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
