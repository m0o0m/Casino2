<%@ Control Language="VB" AutoEventWireup="false" CodeFile="passwordEditor.ascx.vb"
    Inherits="CCWebsite.PasswordEditor" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!--link href="http://testcases.pagebakers.com/PasswordMeter/passwordmeter.css" rel="stylesheet" type="text/css" /-->
<div style="width: 200px;" runat="Server" id="divPasswordEditor">
    <asp:Literal runat="server" ID="ltrPasswordCaption">Password</asp:Literal>
    <asp:TextBox ID="txtPassword" runat="server" 
        Width="70px" MaxLength="10"></asp:TextBox>
    <asp:Label ID="lblAlert" runat="server" ForeColor="red" Font-Bold="true" Text="*"
        Visible="false"></asp:Label>
    <cc1:TextBoxWatermarkExtender ID="txtwePassword" runat="server" TargetControlID="txtPassword"
        WatermarkText="Change Password" WatermarkCssClass="watermarked" Enabled="false">
    </cc1:TextBoxWatermarkExtender>
    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required"
        Display="dynamic" ControlToValidate="txtPassword">*</asp:RequiredFieldValidator><asp:CustomValidator
            ID="custvPassword" runat="server" ErrorMessage="Password must be at least 6 chars long"
            Display="dynamic" ControlToValidate="txtPassword" ClientValidationFunction="valPasswordLength">*</asp:CustomValidator><asp:CustomValidator
                ID="custvPasswordPattern" runat="server" ClientValidationFunction="valPasswordPattern"
                ControlToValidate="txtPassword" Display="dynamic" ErrorMessage="Password must contain at least one letter and one number">*</asp:CustomValidator>
    <div id="div1" width="200" runat="server" title="Password Strengh Meter">
        <div style="width: 0" id="div2" runat="server">
        </div>
    </div>
    <span style="position: relative;" runat="server" id="spanConfirmPassword">
        <asp:Literal runat="server" ID="ltrConfirmPasswordCaption">Confirm Password</asp:Literal>
        <asp:TextBox ID="txtPasswordConfirm" runat="server" 
            Width="70px" MaxLength="10"></asp:TextBox>
        <asp:CompareValidator runat="server" Enabled="false" ID="cvPassword" ControlToCompare="txtPassword"
            ControlToValidate="txtPasswordConfirm" Display="dynamic" ErrorMessage="passwords don't not match">*</asp:CompareValidator><asp:CustomValidator
                ID="custvComparePassword" runat="server" ClientValidationFunction="valComparePassword"
                ControlToValidate="txtPassword" Display="dynamic" ErrorMessage="Passwords do not match">*</asp:CustomValidator>
    </span>
</div>

<script>
    function valComparePassword(sender, args) {
        args.IsValid = true;

        var password = document.getElementById(sender.id.replace("custvComparePassword", "txtPassword"));
        var confirmPassword = document.getElementById(sender.id.replace("custvComparePassword", "txtPasswordConfirm"));

        if (password.value != confirmPassword.value) {
            args.IsValid = false;
        }
        //alert("passwords match compare:" + args.IsValid);
    }
    function valPasswordLength(sender, args) {
        args.IsValid = true;
        var password = args.Value;

        if (password.length < 6)
            args.IsValid = false;
    }

    function valPasswordPattern(sender, args) {
        args.IsValid = true;
        var password = args.Value;

        // NUMBERS
        //alert(!password.match(/\d/));
        if (!password.match(/\d/)) {             	// [verified] at least one number
            args.IsValid = false;
        }
        //alert(!password.match(/[a-z]/) && !password.match(/[A-Z]/));
        if (!password.match(/[a-z]/) && !password.match(/[A-Z]/)) {              // [verified] at least one lower case letter or one upper case letter
            args.IsValid = false;
        }
    }
</script>

<script language="javascript">
    var newWidth;
    var div2;

    function CheckPsdStrength(password, div1ID, div2ID, passwordInputID) {
        var score = 0
        var p = password;

        var nScore = calcStrength(p);

        // Set new width
        var nRound = Math.round(nScore * 2);

        if (nRound > 100) {
            nRound = 100;
        }

        div2 = document.getElementById(div2ID);
        var maxWidth = document.getElementById(div1ID).offsetWidth - 2;
        //var maxWidth = div1.offsetWidth - 2;
        var scoreWidth = (maxWidth / 100) * nRound;
        newWidth = scoreWidth;

        if (nScore < 20)
            document.getElementById(div2ID).title = "Your password is very weak!";
        else if (nScore > 20 && nScore < 40)
            document.getElementById(div2ID).title = "Your password is medium strength.";
        else if (nScore > 40)
            document.getElementById(div2ID).title = "Strong password!";
        document.getElementById(passwordInputID).title = document.getElementById(div2ID).title;
        SetWidth();
    }

    function SetWidth() {
        //var div = document.getElementById("div2");
        var oldWidth = parseInt(div2.style.width.replace("px", ""));

        div2.style.width = oldWidth + 1;
        oldWidth = oldWidth + 1;
        if (newWidth > oldWidth)
            setTimeout(SetWidth, 20);
        else
            div2.style.width = newWidth;
    }

    function calcStrength(p) {
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

