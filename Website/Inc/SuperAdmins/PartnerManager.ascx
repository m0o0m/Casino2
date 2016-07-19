<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PartnerManager.ascx.vb"
    Inherits="SBCSuperAdmin.PartnerManager" %>
<%@ Register Src="~/Inc/SuperAdmins/PartnerEdit.ascx" TagName="PartnerEdit" TagPrefix="uc" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<script type="text/javascript" language="javascript">
    function CheckAllPartner(checkAll) {
        var oGrid = document.getElementById('<%=dgPartners.ClientID%>');
        var oInputs = oGrid.getElementsByTagName("INPUT");

        for (var index = 0; index < oInputs.length; index++) {
            if (oInputs[index].type.toUpperCase() == "CHECKBOX") {
                oInputs[index].checked = checkAll.checked;
            }
        }

        if (checkAll.checked) {
            checkAll.title = "Unselect all"
        }
        else {
            checkAll.title = "Select all"
        }
    }

</script>

<div class="row">
    <div class="col-lg-12">
         <asp:Button ID="btnLock" runat="server" Text="Lock" ToolTip="Lock Partners" CssClass="btn btn-default" />
            <asp:Button ID="btnViewLock" runat="server" Text="View Lock" ToolTip="View lock Partners" CssClass="btn btn-default" />
    </div>
</div>
<div class="mbxl"></div>
<div class="row">
    <div class="col-lg-6">
        <asp:DataGrid ID="dgPartners" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                <AlternatingItemStyle HorizontalAlign="Left" />
                <SelectedItemStyle BackColor="YellowGreen" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20">
                        <HeaderTemplate>
                            <input id="chkChoice" type="checkbox" onclick="CheckAllPartner(this);" title="Select all"
                                runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox type="checkbox" ID="chkID" runat="server" value='<%# SBCBL.std.SafeString(Container.DataItem("SuperAdminID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>'
                                name='Chosen_<%#Container.DataItem("SuperAdminID") %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Login (Name)" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtEdit" runat="server" CommandArgument='<%# SBCBL.std.SafeString(Container.DataItem("SuperAdminID")) & "|" & SBCBL.std.SafeString(Container.DataItem("Login")) %>'
                                CommandName="EditUser" Text='<%#Container.DataItem("Login") & " (" & Container.DataItem("Name")  & ")" %>' CssClass="itemplayer" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="IsLocked" HeaderText="Locked" ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" />
                    <asp:TemplateColumn HeaderText="Last Login" ItemStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#IIf(SBCBL.std.SafeString(Container.DataItem("LastLoginDate")) <> "", UserSession.ConvertToEST(SBCBL.std.SafeString(Container.DataItem("LastLoginDate"))), "").ToString%>&nbsp;
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
    </div>
    <div class="col-lg-6" id="tdPartnerEdit" runat="server">
        <uc:PartnerEdit ID="ucPartnerEdit" runat="server" />
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
