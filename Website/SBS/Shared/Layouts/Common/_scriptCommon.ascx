<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_scriptCommon.ascx.vb" Inherits="SBS_Shared_Layouts_Common_scriptCommon" %>
<script src="/Content/js/jquery.blockui.js"></script>
<script src="/Content/js/common.js"></script>
<script src="/Inc/scripts/std.js"></script>
<script>
    $("#divLayoutContainer").height($(document).height());

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
        blockUI();
        showLoading();
    });

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        unblockUI();
        hideLoading();
    });


    function detectInMobile() {
        if (navigator.userAgent.match(/Android/i)
        || navigator.userAgent.match(/webOS/i)
        || navigator.userAgent.match(/iPhone/i)
        || navigator.userAgent.match(/iPad/i)
        || navigator.userAgent.match(/iPod/i)
        || navigator.userAgent.match(/BlackBerry/i)
        || navigator.userAgent.match(/Windows Phone/i)
        ) {
            return true;
        }
        else {
            return false;
        }
    }

    $(document).ready(function () {
        if (detectInMobile()) {
            $("#liMode").show();
        } else {
            $("#liMode").hide();
        }
    });

    $(window).resize(function () {
        if (detectInMobile()) {
            $("#liMode").show();
        } else {
            $("#liMode").hide();
        }
    });

    function desktopMode() {
        $.cookie('noneresponsive', 'true', { path: '/' });
        location.reload();
    }

    function responsiveMode() {
        $.removeCookie('noneresponsive', { path: '/' });
        location.reload();
    }
</script>
