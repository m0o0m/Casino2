<%@ Control Language="VB" AutoEventWireup="false" CodeFile="layout6LoginForm.ascx.vb" Inherits="SBS_Inc_Login_layout6LoginForm" %>



<form id="form2" runat="server" class="document layout-small">
    <div>
        <!-- Header -->
        <div class="header">
            <div class="logo-login-info-area clear h110px overhidden pdLR10">
                <div class="logo-main h110px left">
                    <a class="block full" href="#">
                        <img id="imgLogo" runat="server" src="/Content/themes/agent/layout6/images/icons/logo_77ebet_h.png" visible="False"/>
                        <asp:Literal ID="ltrLogoName" runat="server">Sport <span>LOGO</span></asp:Literal>
                    </a>
                </div>
                <div class="login-main-area right mgT40 pdR10">
                    <asp:Login ID="Login1" runat="server" DisplayRememberMe="False" TitleText="" PasswordRequiredErrorMessage="Password is required"
                        UserNameRequiredErrorMessage="User name is required" Width="100%">
                        <LayoutTemplate>
                            <%--<input class="input-style-1 w145px h30px mgR5" type="text" placeholder="USERNAME" />
                            <input class="input-style-1 w150px h30px mgR5" type="password" placeholder="PASSWORD" />--%>
                            <asp:TextBox ID="UserName" runat="server" MaxLength="50" onkeypress="return checkWhiteSpace(event);"
                                onchange="processWhiteSpace(this,'divCapsLock');" CssClass="input-style-1 w145px h30px mgR5" placeholder="USERNAME" />
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" MaxLength="20" onkeypress="checkCapsLock(event, 'divCapsLock');"
                                CssClass="input-style-1 w145px h30px mgR5" placeholder="PASSWORD" />
                            <%--<button class="button-style-4 h30px pdLR10">Login</button>--%>
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Login" ValidationGroup="Login1" CssClass="button-style-4 h30px pdLR10" />
                            <div class="errortxt">
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"  ForeColor="#FFFFFF" 
                            ErrorMessage="User name is required" ToolTip="User name is required" ValidationGroup="Login1" CssClass="lblipmsg"></asp:RequiredFieldValidator>
                                <br/>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ForeColor="#FFFFFF" 
                            ErrorMessage="Password is required" ToolTip="Password is required" ValidationGroup="Login1" CssClass="lblipmsg"></asp:RequiredFieldValidator>
                            </div>
                        </LayoutTemplate>
                    </asp:Login>
                </div>
            </div>
        </div>

        <div id="wrapper">
            <!-- Main menu -->
            <div class="navigation-area pdLR10">
                <div class="main-menu-area">
                    <ul class="main-menu menu-style-3">
                        <li class="selected"><a href="#">Home</a></li>
                        <li><a href="#">Sportbook</a></li>
                        <li><a href="#">Racebook</a></li>
                        <li><a href="#">Online Casino</a></li>
                        <li><a href="#">Live Casino</a></li>
                    </ul>
                </div>
            </div>
            <div id="page-wrapper">
                <!-- breadcrums-->
                <div class="page-content">

                    <!-- Content -->
                    <div class="content">
                        <div class="main-menu-slider-area overhidden mgT15">
                            
                            <div id="wowslider-container1">
                                <div class="ws_images">
                                    <ul>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr1.jpg" alt="bnr1" title="bnr1" id="wows1_0" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr2.jpg" alt="bnr6" title="bnr6" id="wows1_1" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr3.jpg" alt="bnr5" title="bnr5" id="wows1_2" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr4.jpg" alt="bnr2" title="bnr2" id="wows1_3" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr5.jpg" alt="bnr7" title="bnr7" id="wows1_4" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr6.jpg" alt="bnr3" title="bnr3" id="wows1_5" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr7.jpg" alt="bnr4" title="bnr4" id="wows1_6" /></li>
                                        <li>
                                            <img src="/Content/themes/agent/layout6/images/slides/bnr8.jpg" alt="bnr8" title="bnr8" id="wows1_7" /></li>
                                    </ul>
                                </div>
                                <div class="ws_bullets">
                                    <div>
                                        <a href="#" title="bnr1">1</a>
                                        <a href="#" title="bnr6">2</a>
                                        <a href="#" title="bnr5">3</a>
                                        <a href="#" title="bnr2">4</a>
                                        <a href="#" title="bnr7">5</a>
                                        <a href="#" title="bnr3">6</a>
                                        <a href="#" title="bnr4">7</a>
                                        <a href="#" title="bnr8">8</a>
                                    </div>
                                </div>
                                <div class="ws_shadow"></div>
                            </div>
                            
                        </div>

                        <div class="clear pd5">
                            <div class="ly-w-1:3 left pd5">
                                <div class="box-present-1 pd10">
                                    <div class="thumnail">
                                        <img class="full-w" src="/Content/themes/agent/layout6/images/boxgame/sports.jpg" />
                                    </div>
                                    <div class="ly-fixed pdT10">
                                        <div>
                                            <div class="fz11 bold clr-white uppercase">
                                                sports wagering
                                            </div>
                                            <div class="fz10 clr-blue-1 mgT2">> Find out more about Sports Wagering</div>
                                        </div>
                                        <div class="w70px text-right">
                                            <button class="button-style-5">More</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="ly-w-1:3 left pd5">
                                <div class="box-present-1 pd10">
                                    <div class="thumnail">
                                        <img class="full-w" src="/Content/themes/agent/layout6/images/boxgame/casino.jpg" />
                                    </div>
                                    <div class="ly-fixed pdT10">
                                        <div>
                                            <div class="fz11 bold clr-white uppercase">
                                                online casino games
                                            </div>
                                            <div class="fz10 clr-blue-1 mgT2">> Find out more about Sports Wagering</div>
                                        </div>
                                        <div class="w70px text-right">
                                            <button class="button-style-5">More</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="ly-w-1:3 left pd5">
                                <div class="box-present-1 pd10">
                                    <div class="thumnail">
                                        <img class="full-w" src="/Content/themes/agent/layout6/images/boxgame/horse.jpg" />
                                    </div>
                                    <div class="ly-fixed pdT10">
                                        <div>
                                            <div class="fz11 bold clr-white uppercase">
                                                horse wagering
                                            </div>
                                            <div class="fz10 clr-blue-1 mgT2">> Find out more about Sports Wagering</div>
                                        </div>
                                        <div class="w70px text-right">
                                            <button class="button-style-5">More</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%--<div class="clear pdLR10 mgT10">
                            <div class="ly-w-1:3 left">
                                <img src="/Content/themes/agent/layout6/images/contact.jpg" />
                            </div>
                            <div class="ly-w-2:3 left text-right">
                                <img src="/Content/themes/agent/layout6/images/mobcomp.jpg" />
                            </div>
                        </div>--%>


                    </div>

                </div>
                <div id="footer">
                    <div class="">
                        <div class="sp-1 mgT20"></div>
                        <div class="clear h120px pdT20">
                            <div class="left pdL10">
                                <div class="link-nav-1">
                                    <a href="">home</a> <span>|</span>
                                    <a href="">sportsbook</a> <span>|</span>
                                    <a href="">racebook</a> <span>|</span>
                                    <a href="">online casino</a>
                                </div>
                            </div>
                            <div class="right text-right pdR10">
                                <div class="fz10 clr-gray-1">Copyright © 2010. Online Casino & Racebook</div>
                                <div class="fz10 clr-gray-1 mgT1">All Rights Reserved</div>
                            </div>
                        </div>
                    </div>
                    <div class="copyright">
                        <a href="#">
                            <asp:Literal ID="ltrCopyRight" runat="server"></asp:Literal></a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</form>





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
