<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LoginControl.ascx.vb"
    Inherits="Login_LoginControl" %>
<%@ Register Src="~/Login/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>
<%--<div id="tblRenewPassword" runat="server" visible="false" style="width: 100%;">
    <asp:Label runat="server" ID="lblPasswordExpired" Font-Bold="true" Visible="false">Your password has expired. Please renew your password.<br /></asp:Label>
    <asp:Label runat="server" ID="lblUserNotFound" Font-Bold="true" ForeColor="red" Visible="false">Unable to find the user account.<br /></asp:Label>
    <asp:Label runat="server" ID="lblAccountExpire" Font-Bold="true" ForeColor="red" Visible="false">This account has expired. Please contact your administrator to renew the account.<br /></asp:Label>
    <div>
        <div>
            <asp:TextBox ID="txtLogin" runat="server" MaxLength="50" />
            <asp:RequiredFieldValidator runat="server" ID="rfvLogin" Display="dynamic" ControlToValidate="txtLogin" ErrorMessage="User name is required">*</asp:RequiredFieldValidator>
        </div>
        <div>
            User Name</div>
    </div>
    <div>
        <div>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="20" onkeypress="checkCapsLock(event, 'divCapsLock2');" />
            <asp:RequiredFieldValidator runat="server" ID="rfvPassword" Display="dynamic" ControlToValidate="txtPassword" ErrorMessage="Old password is required">*</asp:RequiredFieldValidator>
        </div>
        <div>
            Old Password</div>
    </div>
    <div style="left: 110px; height: 100%;">
        <uc1:passwordEditor runat="server" ID="psdEditorLogin" Required="true" PasswordCaption="New" MeterLeft="-18" Left="37" ConfirmPasswordLeft="-10" SetCheckCapsLockClientFunction="checkCapsLock(event, 'divCapsLock2')" />
    </div>
    <div>
        <div style="padding-top: 2px;">
            <asp:Button runat="server" ID="btnUpdatePassword" Text="Renew Password" />
        </div>
        <div>
        </div>
    </div>
    <div style="margin-left: 100px; padding: 0px; width: 100%; height: 100%; overflow: hidden">
        <div id="divCapsLock2" style="color: red; visibility: hidden;">
            <br />
            Caps Lock is ON.</div>
        <asp:ValidationSummary runat="server" ID="vsLogin" DisplayMode="bulletList" />
    </div>
</div>--%>
    <asp:Login ID="Login1" runat="server" DisplayRememberMe="False" TitleText="" PasswordRequiredErrorMessage="Password is required"
        UserNameRequiredErrorMessage="User name is required">
        <LayoutTemplate>
           <%-- <div style="width: 100%; text-align: center;">--%>
           
                <%--<div style="text-align: right; vertical-align: middle; height: 25px; float: left;">--%>
             <a href="Default.aspx" style="color:cyan" class="menuitem">HOME</a>&nbsp;<span style="color:cyan;margin-left:20px;margin-right:20px">|</span>&nbsp;
             <a href="Rules.aspx"  style="color:cyan" class="menuitem">RULES</a>&nbsp;<span  style="color:cyan;margin-left:20px;margin-right:20px">|</span>&nbsp;
             <a href="Odds.aspx"  style="color:cyan" class="menuitem">ODDS &amp; PAYOUT</a>&nbsp;<span  style="color:cyan;margin-left:20px;margin-right:20px">|</span>&nbsp;

             <asp:Label ID="UserNameLabel"  runat="server" ForeColor ="White" AssociatedControlID="UserName" Font-Bold="true" Text="Player"></asp:Label>
             <asp:TextBox ID="UserName" runat="server" MaxLength="50" Width="120" onkeypress="return checkWhiteSpace(event);"
                            onchange="processWhiteSpace(this,'divCapsLock');" CssClass="textInput" />
             <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            ErrorMessage="User name is required" ToolTip="User name is required" ValidationGroup="Login1">*</asp:RequiredFieldValidator>
             <asp:Label ID="PasswordLabel"  ForeColor ="White" runat="server" AssociatedControlID="Password" Font-Bold="true">Password</asp:Label>
             <asp:TextBox ID="Password" runat="server" TextMode="Password" MaxLength="20"  Width="120" onkeypress="checkCapsLock(event, 'divCapsLock');"
                            CssClass="textInput" />
             <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            ErrorMessage="Password is required" ToolTip="Password is required" ValidationGroup="Login1">*</asp:RequiredFieldValidator>
             <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="Login1" style="margin-top:4px"
                            CssClass="loginbuton" />
							
			<input type="button" value="Login Mobile" onclick="window.location='http://m.tigersb.net/'" class="loginbuton" style="margin-left:5px"/>		
             <div id="LoginError" class="loginError">
             <div id="divCapsLock" style="color: Red; visibility: hidden; float: left;">
                    Caps Lock is ON.</div>
                <br />
                <div style="float:left;color:red;text-align:center;">
                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False" />
                </div>
                <%--<div style="margin-left: 170px; white-space: nowrap">
            </div>--%>
               
   
            
           
            
            
           
        </LayoutTemplate>
    </asp:Login>


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

