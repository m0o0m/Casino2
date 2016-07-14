<%@ Control Language="VB" AutoEventWireup="false" CodeFile="layout5LoginForm.ascx.vb" Inherits="SBS_Inc_Login_layout5LoginForm" %>

<section class="main-content">
    <header>
        <div class="container">
            <div class="logo">
                <img id="imgLogo" runat="server" src="/images/Winsb.png" visible="False" />
                <asp:Literal ID="ltrLogoName" runat="server">Sport <span>LOGO</span></asp:Literal>
            </div>
        </div>
    </header>
    <div class="container">
        <div class="login-wrap">
            <h3>// Member <span>Login</span></h3>
            <form runat="server" id="form1">
                <%--<div class="form-group">
                    <label class="control-label">Username</label>
                    <input class="form-control" type="text" name="username">
                </div>
                <div class="form-group">
                    <label class="control-label">Password</label>
                    <input class="form-control" type="password" name="">
                </div>
                <div class="remember-box">
                    <input type="checkbox" name="">
                    Remember me</div>
                <div class="submit-btn-box">
                    <input type="submit" class="submit-btn" value="enter"></div>
                <div class="login-links"><a href="">Forgot password</a> | <a href="">Register</a></div>--%>
                <asp:Login ID="Login1" runat="server" DisplayRememberMe="False" TitleText="" PasswordRequiredErrorMessage="Password is required"
                    UserNameRequiredErrorMessage="User name is required" Width="100%">
                    <LayoutTemplate>

                        <div class="form-group">
                            <label class="control-label">Username</label>
                            <asp:TextBox ID="UserName" runat="server" MaxLength="50" onkeypress="return checkWhiteSpace(event);"
                                onchange="processWhiteSpace(this,'divCapsLock');" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                ErrorMessage="User name is required" ToolTip="User name is required" ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label class="control-label">Password</label>
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" MaxLength="20" onkeypress="checkCapsLock(event, 'divCapsLock');"
                                CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                ErrorMessage="Password is required" ToolTip="Password is required" ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                        </div>
                        <div class="submit-btn-box">
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Enter" ValidationGroup="Login1" CssClass="submit-btn" />
                        </div>
                        <div id="LoginError" class="loginError">
                            <div id="divCapsLock" style="color: Red; visibility: hidden; float: left;">
                                Caps Lock is ON.
                            </div>
                            <br />
                            <div style="float: left; color: red; text-align: center;">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False" />
                            </div>
                        </div>
                    </LayoutTemplate>
                </asp:Login>
            </form>
        </div>
    </div>
    <!--container-->
</section>
<!---main-content-->
<section class="info">
    <div class="container">
        <ul class="sport-icons">
            <li>
                <img src="/Content/login/images/icon-nfl.png"></li>
            <li>
                <img src="/Content/login/images/icon-ncaa.png"></li>
            <li>
                <img src="/Content/login/images/icon-nba.png"></li>
            <li>
                <img src="/Content/login/images/icon-mlb.png"></li>
            <li>
                <img src="/Content/login/images/icon-nhl.png"></li>
        </ul>
    </div>
</section>
<footer>
    <div class="container">
        <div class="copyright">
            <asp:Literal ID="ltrCopywright" runat="server">Copyright &copy;2016. All rights reserved.</asp:Literal></div>
    </div>
</footer>
<script type="text/javascript">
    function checkCapsLock(e, divAlert) {
        var key = e.keyCode ? e.keyCode : e.which;
        var shift = e.shiftKey ? e.shiftKey : ((key == 16) ? true : false);
        if (((key >= 65 && key <= 90) && !shift) || ((key >= 97 && key <= 122) && shift)) {
            document.getElementById(divAlert).innerHTML = 'Caps Lock is ON.';
            document.getElementById(divAlert).style.visibility = 'visible';
        }
        else
            document.getElementById(divAlert).style.visibility = 'hidden';
    }

    function checkWhiteSpace(e) {
        var key = e.keyCode ? e.keyCode : e.which;
        if (key == 32) return false;
        return true;
    }

    function processWhiteSpace(e, divAlert) {
        if (e.value.indexOf(" ") >= 0) {
            document.getElementById(divAlert).innerHTML = 'User name don\'t allow spaces';
            document.getElementById(divAlert).style.visibility = 'visible';
        }
        else
            document.getElementById(divAlert).style.visibility = 'hidden';
    }


</script>
