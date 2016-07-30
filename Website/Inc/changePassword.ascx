<%@ Control Language="VB" AutoEventWireup="false" CodeFile="changePassword.ascx.vb"
    Inherits="SBCWebsite.Inc_changePassword" %>
<%@ Register Src="~/Inc/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>

<div class="panel panel-grey panel-style-1">
    <div class="panel-heading">Change Password</div>
    <div class="panel-body pdTB10">


        <div class="ly-fixed pd10">
            <div class="ly-w-1:6 v-top-i pdT5">
                <label class="control-label col-md-2">Current Password</label>
            </div>
            <div class="ly-w-1:6 v-top-i">
                <asp:TextBox CssClass="form-control input-field-2 h28px full-w" ID="txtCurrentPassword" runat="server" TextMode="Password"
                    MaxLength="10" onkeypress="capsLock(event, 'divCapsLock')" />
            </div>
            <div></div>
        </div>

        <div class="ly-fixed pd10">
            <div class="ly-w-1:6 v-top-i pdT5">
                <label class="block control-label col-md-2">New Password</label>
                <label class="block control-label col-md-2 mgT30">Confirm New Password</label>
            </div>
            <div class="ly-w-1:6 v-top-i">
                <uc1:passwordEditor runat="server" ID="psdPassword" Required="false" HorizontalAlign="false"
                    TextVisible="false" SetCheckCapsLockClientFunction="capsLock(event, 'divCapsLock')" />
                <asp:HiddenField ID="hfdPassword" runat="server" />
            </div>
            <div></div>
        </div>

        <div class="ly-fixed pd10">
            <div class="ly-w-1:6">
            </div>
            <div>
                <asp:Button ID="btnChangePassword" runat="server" CssClass="btn btn-grey button-style-2 h26px w120px" Text="Save Change"
                    ToolTip="Change Password" />
                <div id="divCapsLock" style="color: red; display: none;">
                    Caps Lock is ON.
                </div>
            </div>
        </div>

        <div class="form-group">
            <div class="col-md-2 col-lg-offset-2">
                
            </div>
        </div>
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
