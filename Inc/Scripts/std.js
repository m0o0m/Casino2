function capsLock(e, divAlert) {
    var key = e.keyCode ? e.keyCode : e.which;
    var shift = e.shiftKey ? e.shiftKey : ((key == 16) ? true : false);

    //    var bCapsLock = false;
    //    if (shift)
    //    {
    //        bCapsLock = (key >= 65 && key <= 90)||(key >= 97 && key <= 122);
    //    }
    //    else
    //    {
    //        bCapsLock = (key >= 65 && key <= 90); 
    //    }

    if (((key >= 65 && key <= 90) && !shift) || ((key >= 97 && key <= 122) && shift))
        document.getElementById(divAlert).style.display = '';
    else
        document.getElementById(divAlert).style.display = 'none';
}

function toggleDisplay(psIDToHide, poToggleImg) {
    var el = document.getElementById(psIDToHide);
    if (el.style.display != 'none') {
        el.style.display = 'none';
    }
    else {
        el.style.display = '';
    }

    poToggleImg.src = '/LML/images/' +
                (poToggleImg.src.match('minimize.gif') ?
                'maximize.gif' : 'minimize.gif');
}

function focusFirstTextbox() {
    for (var i = 0; i < document.forms[0].elements.length; i++) {
        if ((document.forms[0].elements[i].tagName == 'TEXTAREA' || document.forms[0].elements[i].type == 'text') &&
            (document.forms[0].elements[i].name.indexOf('txtFind') == -1)) {

            if (!document.forms[0].elements[i].disabled) {
                try {
                    document.forms[0].elements[i].focus();
                    break;
                }
                catch (err) { }
            }
        }
    }
}

function openModalDialog(sURL, height, width, resizeable) {
    //the guid forces the page to refresh
    if (sURL.indexOf("?") > -1)
        sURL += "&";
    else
        sURL += "?";
    sURL += "GUID=" + newGUID();

    return showModalDialog(sURL, "", "dialogWidth:" + width + "px; dialogHeight:" + height + "px;center:yes;help:no;status:no;scroll:yes;resizable:" + resizeable);
}

function openDialog(sURL, height, width, resize) {
    openDialog(sURL, height, width, resize, true);
}

function openDialog(sURL, height, width, resize, scrollbar) {
    var sResize = (resize) ? 1 : 0;
    var sScrollbar = (scrollbar) ? 1 : 0;
    var sFeature = 'status=0,toolbar=0,location=0,menubar=0,resizable=' + sResize + ',width=' + width + ',height=' + height + ',scrollbars=' + sScrollbar;

    var centerWidth = (window.screen.width - width) / 2;
    var centerHeight = (window.screen.height - height) / 2;

    window.open(sURL, '', sFeature);
}

function newGUID() {
    var result, i, j;
    result = '';
    for (j = 0; j < 32; j++) {
        if (j == 8 || j == 12 || j == 16 || j == 20)
            result = result + '-';
        i = Math.floor(Math.random() * 16).toString(16).toUpperCase();
        result = result + i;
    }
    return result
}

function HighlightActiveMenu() {
    var skmMnu = $get('activemenuid');
    if (skmMnu) {
        skmMnu = skmMnu.parentNode;
        skmMnu.className = 'menumouseover';
        skmMnu.onmouseout = null;
    }
}

function MM_preloadImages() { //v3.0
    var d = document; if (d.images) {
        if (!d.MM_p) d.MM_p = new Array();
        var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
            if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; }
    }
}
function MM_swapImgRestore() { //v3.0
    var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
}
function MM_findObj(n, d) { //v4.01
    var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
        d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
    }
    if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
    for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
    if (!x && d.getElementById) x = d.getElementById(n); return x;
}

function MM_swapImage() { //v3.0
    var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
        if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
}

function getTime() {
    var curtime = new Date();
    var curhour = curtime.getHours();
    var curmin = curtime.getMinutes();
    var cursec = curtime.getSeconds();
    var time = "";

    var curr_date = curtime.getDate();
    var curr_month = curtime.getMonth() + 1;
    var curr_year = curtime.getFullYear();

    if (curhour == 0) curhour = 12;
    time = (curhour > 12 ? curhour - 12 : curhour) + ":" +
         (curmin < 10 ? "0" : "") + curmin + ":" +
         (cursec < 10 ? "0" : "") + cursec + " " +
         (curhour > 12 ? "PM" : "AM");

    return curr_month + "/" + curr_date + "/" + curr_year + " " + time;
}

function checknumber(psValue) {

    var anum = /(^\-?\d+$)|(^\-?\d*\.\d+$)|(^\-?\d+\.\d*$)/;
    if (anum.test(psValue)) {
        return true;
    }
    return false;
}

function disableEnterKey(pe) {

    var keycode;

    if (window.event) keycode = window.event.keyCode;
    else if (pe) keycode = pe.which;

    if (keycode == 13) {
        return false;
    }
}

function inputNumber(pobj, pe, pblnNegative) {

    var keycode;

    if (window.event) keycode = window.event.keyCode;
    else if (pe) keycode = pe.which;

    //if (keycode == 45) {
    //    if (pobj.value != "" && pblnNegative) {
    //        var nNumber = parseFloat(getNumberString(pobj.value));

    //        if (nNumber > 0) {
    //            pobj.value = "-" + nNumber;
    //        } else {
    //            pobj.value = "" + (-nNumber);
    //        }
    //    }
    //    return false;
    //}
    //if ((keycode >= 48 && keycode <= 57) || keycode == 46 || keycode == 8 || keycode == 0) {

    //    if (keycode == 46) {
    //        if (pobj.value.indexOf(".") >= 0) {
    //            return false;
    //        }
    //    }
    //    return true;
    //}
    
    var re = /^\d*\.?\d{0,2}$/;
    if (pblnNegative)
        re = /^-?\d*\.?\d{0,2}$/;
    
    var text = pobj.value + String.fromCharCode(keycode);

    var isValid = (text.match(re) !== null);

    return isValid;
}

function inputColorCode(pe) {

    var keycode;
    var keyCodeIsWhich = false; //use for firefox

    if (window.event)
        keycode = window.event.keyCode;
    else if (pe) {
        keycode = pe.which;
        keyCodeIsWhich = true;
    }
    //0->9, A->F, a-f        
    if ((keycode >= 48 && keycode <= 57) || (keycode >= 65 && keycode <= 70) || (keycode >= 97 && keycode <= 102)) {
        return true;
    }
    if (keyCodeIsWhich && keycode == 0) //use for firefox
    {
        return true;
    }

    return false;
}

function formatNumber(pObj, pdecimals) {
    var sNum = pObj.value.replace(/,/g, "");
    if (checknumber(sNum)) {
        var sNum = parseFloat(sNum).toFixed(pdecimals);
        var arrNum = sNum.split(".");
        var sNegative = "";
        if (arrNum[0] == "") {
            arrNum[0] = "0";
        }
        if (arrNum[0].substring(0, 1) == "-") {
            sNegative = "-";
        }

        var nValue = parseInt(arrNum[0]);

        nValue = Math.abs(nValue);

        var nTemp = 0;
        sNum = "";
        while (nValue > 999) {
            nValue = Math.round(nValue);
            if (parseInt((nValue / 1000)) > 0) {
                nTemp = nValue % 1000;
                if (sNum == "") {
                    sNum = addZero(3 - ("" + nTemp).length) + nTemp;
                } else {
                    sNum = addZero(3 - ("" + nTemp).length) + nTemp + "," + sNum;
                }
            }
            nValue = parseInt((nValue / 1000));
        }

        if (nValue > 0 && sNum != "") {
            sNum = nValue + "," + sNum;
        } else {
            sNum = nValue + "";
        }

        arrNum[0] = sNum;
        pObj.value = sNegative + arrNum.join(".");
    } else {
        if (pdecimals > 0) {
            pObj.value = "0." + addZero(pdecimals);
        } else {
            pObj.value = "0";
        }
    }
}

function addZero(pnNum) {
    var sValue = "";
    for (var i = 0; i < pnNum; i++) {
        sValue += "0";
    }
    return sValue;
}

function selectedAll(pObj) {
    pObj.select();
}

function inputNumberOnly(evt) {

    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function showUserLinks(Favorites) {
    var oDiv = document.getElementById("Favorites");
    oDiv.style.position = "absolute";
    oDiv.style.display = "";
    oDiv.style.left = getLeft(Favorites) - 20;
    oDiv.style.top = getTop(Favorites) + 20;
}
function showUserLinks2() {
    var oDiv = document.getElementById("Favorites");
    oDiv.style.display = "";
}
function hideUserLinks() {
    var oDiv = document.getElementById("Favorites");
    oDiv.style.display = "none";
}

function getLeft(e) {
    var offset = e.offsetLeft;
    if (e.offsetParent != null) offset += getLeft(e.offsetParent);
    return offset;
}
function getTop(e) {
    var offset = e.offsetTop;
    if (e.offsetParent != null) offset += getTop(e.offsetParent);
    return offset;
}

function getNumberString(psNumber) {
    if (psNumber == "") return "0";

    var sNumber = psNumber.replace("$", "");
    while (sNumber.indexOf(',') != -1) {
        sNumber = sNumber.replace(",", "");
    }

    return sNumber;
}

function formatNumber2(pNum, pdecimals) {
    var sNum = pNum;
    if (checknumber(sNum)) {
        var sNum = parseFloat(sNum).toFixed(pdecimals);
        var arrNum = sNum.split(".");
        var sNegative = "";
        if (arrNum[0] == "") {
            arrNum[0] = "0";
        }
        if (arrNum[0].substring(0, 1) == "-") {
            sNegative = "-";
        }

        var nValue = parseInt(arrNum[0]);

        nValue = Math.abs(nValue);

        var nTemp = 0;
        sNum = "";
        while (nValue > 999) {
            nValue = Math.round(nValue);
            if (parseInt((nValue / 1000)) > 0) {
                nTemp = nValue % 1000;
                if (sNum == "") {
                    sNum = addZero(3 - ("" + nTemp).length) + nTemp;
                } else {
                    sNum = addZero(3 - ("" + nTemp).length) + nTemp + "," + sNum;
                }
            }
            nValue = parseInt((nValue / 1000));
        }

        if (nValue > 0 && sNum != "") {
            sNum = nValue + "," + sNum;
        } else {
            sNum = nValue + "";
        }

        arrNum[0] = sNum;

        return sNegative + arrNum.join(".");
    } else {
        if (pdecimals > 0) {
            return "0." + addZero(pdecimals);
        } else {
            return "0";
        }
    }
}

function showJSPopup(holderID, bgID) {
    // hide all dropdown, IE 6 fix
    for (f = 0; f < document.forms.length; f++) {
        var elements = document.forms[f].elements;
        // looping through all elements on certain form

        for (e = 0; e < elements.length; e++) {
            if (elements[e].type == "select-one") {
                elements[e].style.display = 'none';
            }
        }
    }

    // show popup layer
    document.getElementById(bgID).style.width = document.body.offsetWidth - 20;
    document.getElementById(bgID).style.height = document.body.offsetHeight - 5;
    document.getElementById(holderID).style.display = 'block';
    // set resize to popup layer
    window.onresize = function() { resizeJSPopup(bgID); }
}
function hideJSPopup(holderID) {
    // reshow all dropdown
    for (f = 0; f < document.forms.length; f++) {
        var elements = document.forms[f].elements;
        for (e = 0; e < elements.length; e++) {
            if (elements[e].type == "select-one") {
                elements[e].style.display = 'block';
            }
        }
    }

    document.getElementById(holderID).style.display = 'none';
    window.onresize = function() { }
}
function resizeJSPopup(bgID) {
    var modal = document.getElementById(bgID);
    if (modal != null) {
        modal.style.width = document.body.offsetWidth - 20;
        modal.style.height = document.body.offsetHeight - 5;
    }
}

function LookupZipInfo(txtzipcode, txtCity, ddlState) {
    window.CURRENT_TXT_ZIP = txtzipcode;
    window.CURRENT_TXT_CITY = txtCity;
    window.CURRENT_DLL_STATE = ddlState;
    txtzipcode.className = 'textInput ZipLookupLoading';
    txtzipcode.disabled = true;
    PageMethods.ZipLookup(txtzipcode.value, LookupZipInfoCallBack);
}

function LookupZipInfoCallBack(result) {
    CURRENT_TXT_ZIP.className = 'textInput';
    CURRENT_TXT_ZIP.disabled = false;
    if (result != null && result.indexOf("FOUND_ZIP:") == 0) {
        var sContent = result.replace("FOUND_ZIP:", "");
        var oReults = sContent.split('||');
        var sCity = oReults[0];
        var sState = oReults[2];
        CURRENT_TXT_CITY.value = sCity;
        if (CURRENT_DLL_STATE != null && CURRENT_DLL_STATE != undefined) {
            CURRENT_DLL_STATE.value = sState;
        }
        return true;
    }
    else {
        CURRENT_TXT_CITY.value = "";
        if (CURRENT_DLL_STATE != null && CURRENT_DLL_STATE != undefined) {
            CURRENT_DLL_STATE.value = "";
        }
        return false;
    }

}

function checkNumberBetweenValues(poTextBox, pnMinValue, pnMaxValue, psDefaultValue, psTitle) {
    var nValue = parseFloat(getNumberString(poTextBox.value));
    if (nValue < pnMinValue || nValue > pnMaxValue) {
        alert(psTitle + ' must be between ' + pnMinValue.toString() + ' - ' + pnMaxValue.toString() + '.');
        poTextBox.value = psDefaultValue;
        poTextBox.focus();
    }
}
function checkNumberGreaterOrEqualZero(poTextBox, psDefaultValue, psTitle) {
    var nValue = parseFloat(getNumberString(poTextBox.value));
    if (nValue < 0) {
        alert(psTitle + ' must be greater then or equal to 0.');
        poTextBox.value = psDefaultValue;
        poTextBox.focus();
    }
}

function checkAllInGird(psGridClientID, pbCheck) {
    var oGird = document.getElementById(psGridClientID);
    if (!oGird) return;

    var oInputs = oGird.getElementsByTagName("INPUT");
    if (!oInputs) return;

    for (var index = 0; index < oInputs.length; index++) {
        if (oInputs[index].type.toUpperCase() == "CHECKBOX") {
            oInputs[index].checked = pbCheck;
        }
    }
}

function GetWidth() {
    var x = 0;
    if (self.innerWidth) {
        x = self.innerWidth;
    }
    else if (document.documentElement && document.documentElement.clientWidth) {
        x = document.documentElement.clientWidth;
    }
    else if (document.body) {
        x = document.body.clientWidth;
    }
    return x;
}

function GetHeight() {
    var y = 0;
    if (self.innerHeight) {
        y = self.innerHeight;
    }
    else if (document.documentElement && document.documentElement.clientHeight) {
        y = document.documentElement.clientHeight;
    }
    else if (document.body) {
        y = document.body.clientHeight;
    }
    return y;
}

function setTableHeight(poTable, pnOffset) {
    try {
        poTable.style.height = (GetHeight() - pnOffset).toString() + 'px';
    }
    catch (e) {
        //window resized to an invalid value, ignore;
    }
}
function setTableWidth(poTable, pnOffset) {
    try {
        poTable.style.width = (GetWidth() - pnOffset).toString() + 'px';
    }
    catch (e) {
        //window resized to an invalid value, ignore;
    }
}

function resizeMainTable() {
    //note side menu div height is a slightly off from 
    //main body div height. don't know why :T
    setTableHeight($get('divLeftMenu'), 113);

    //set the height of the main body table so it constrains
    //both the side nav and the body content
    setTableHeight($get('divScrollContent'), 125);

    //restrict the width of the body content only since 
    //sidenav is a constant width
    if (CurrentExpandLeftMenuLML) {
        setTableWidth($get('divScrollContent'), 305);
    }
    else {
        setTableWidth($get('divScrollContent'), 50);
    }

    return true;
}

//this function is used in the APPLICATION master file since
//its tabs requires a different offset to fit the app on screen
function resizeMainTableApp() {
    //note side menu div height is a slightly off from 
    //main body div height. don't know why :T
    setTableHeight($get('divLeftMenu'), 113);

    //set the height of the main body table so it constrains
    //both the side nav and the body content
    setTableHeight($get('divScrollContent'), 142);

    //restrict the width of the body content only since 
    //sidenav is a constant width
    if (CurrentExpandLeftMenuLML)
        setTableWidth($get('divScrollContent'), 305);
    else
        setTableWidth($get('divScrollContent'), 60);

    return true;
}


//this function is used in the ADMIN master file since
//its tabs requires a different offset to fit the app on screen
function resizeMainTableAdmin() {
    //note side menu div height is a slightly off from 
    //main body div height. don't know why :T
    setTableHeight($get('divLeftMenu'), 113);

    //set the height of the main body table so it constrains
    //both the side nav and the body content
    setTableHeight($get('divScrollContent'), 142);

    //restrict the width of the body content only since 
    //sidenav is a constant width
    if (CurrentExpandLeftMenuLML)
        setTableWidth($get('divScrollContent'), 305);
    else
        setTableWidth($get('divScrollContent'), 60);

    return true;
}

// save Cookie 
function setCookie(name, value) {
    var expires = new Date();
    expires.setTime(expires.getTime() + (1000 * 86400 * 365));
    document.cookie = name + "=" + escape(value) + "; expires=" + expires.toGMTString() + "; path=/";
}

// get Cookie 
function getCookie(name) {
    var cookie_name = name + "=";
    var cookie_length = document.cookie.length;
    var cookie_begin = 0;
    while (cookie_begin < cookie_length) {
        var value_begin = cookie_begin + cookie_name.length;
        if (document.cookie.substring(cookie_begin, value_begin) == cookie_name) {
            var value_end = document.cookie.indexOf(";", value_begin);
            if (value_end == -1) {
                value_end = cookie_length;
            }
            return unescape(document.cookie.substring(value_begin, value_end));
        }
        cookie_begin = document.cookie.indexOf(" ", cookie_begin) + 1;
        if (cookie_begin == 0) {
            break;
        }
    }
    return true;
}

//Menu begin///

function menuActive(menu_a) {
    var sScheme = document.getElementById("hfMenu").value;
   // alert(sScheme);
    menu_a.style.borderLeft = "#FFF solid 1px";
    if (sScheme.toUpperCase() == "RED") {
        menu_a.style.background = "url(/SBS/images/menu_active_red.png) repeat-x  top";
    } else if (sScheme.toUpperCase() == "SKY") {
        menu_a.style.background = "url(/SBS/images/menu_active_sky.png) repeat-x  top";
    } else if (sScheme.toUpperCase() == "BLUE") {
        menu_a.style.background = "url(/SBS/images/menu_active_blue.png) repeat-x  top";
    }
    else if (sScheme.toUpperCase() == "BLACK") {
        menu_a.style.background = "url(/SBS/images/menu_active_black.png) repeat-x  top";
    }
    else if (sScheme.toUpperCase() == "GREEN") {
        menu_a.style.background = "url(/SBS/images/bg_active_green.jpg) repeat-x  top";
    }
}
function menuNotActive(menu_a) {
    if (menu_a.className == "menu_active sfHover" || menu_a.className == "menu_active")
        return
    else
        menu_a.style.background = "";
}
function subMenuActive(menu_a) {
    var sScheme = document.getElementById("hfMenu").value;
    menu_a.style.borderLeft = "#FFF solid 1px";
    if (sScheme.toUpperCase() == "RED") {
        menu_a.style.background = "url(/SBS/images/menu_active_red.png) repeat-x  top";
    } else if (sScheme.toUpperCase() == "SKY") {
    menu_a.style.background = "url(/SBS/images/menu_active_sky.png) repeat-x  top";
    } else if (sScheme.toUpperCase() == "BLUE") {
    menu_a.style.background = "url(/SBS/images/menu_active_blue.png) repeat-x  top";
    }
    else if (sScheme.toUpperCase() == "BLACK") {
    menu_a.style.background = "url(/SBS/images/menu_active_black.png) repeat-x  top";
    }
    else if (sScheme.toUpperCase() == "GREEN") {
    menu_a.style.background = "url(/SBS/images/bg_active_green.jpg) repeat-x  top";
}
    
   
}

function subMenuNotActive(menu_a) {
    if (menu_a.className == "menu_active sfHover" || menu_a.className == "menu_active")
        return
    else {
        var sScheme = document.getElementById("hfMenu").value;
        // menu_a.style.borderLeft = "#FFF solid 1px";
        if (sScheme.toUpperCase() == "RED") {
            // menu_a.style.background = "url(/SBS/images/menu_active_red.png) repeat-x  top";
            menu_a.style.background = "#992929"
        } else if (sScheme.toUpperCase() == "SKY") {
            // menu_a.style.background = "url(/SBS/images/menu_active_sky.png) repeat-x  top";
        menu_a.style.background = "#09a7d7"
        } else if (sScheme.toUpperCase() == "BLUE") {
            // menu_a.style.background = "url(/SBS/images/menu_active_blue.png) repeat-x  top";
            menu_a.style.background = "#2c5296"
        }
        else if (sScheme.toUpperCase() == "BLACK") {
            // menu_a.style.background = "url(/SBS/images/menu_active_black.png) repeat-x  top";
            menu_a.style.background = "#000"
        }
        else if (sScheme.toUpperCase() == "GREEN") {
            // menu_a.style.background = "url(/SBS/images/bg_active_green.png) repeat-x  top";
            menu_a.style.background = "#839300"
        }


        // menu_a.style.background = "#f5803f";

        //  menu_a.style.background = "";
    }
}
//menu end//
function initMenuColor() {
    try {
        var spTeaser = document.getElementById("spTeaser");
        var spParlay = document.getElementById("spParlay");
        var spReverse = document.getElementById("spReverse");
        var hfTeaser = document.getElementById("hfTeaser");
        var hfQuarterOnly = document.getElementById("hfQuarterOnly");
        if (hfQuarterOnly.value == "true") {

            //spTeaser.firstChild.style.color = "#b0afae";

            spTeaser.childNodes[0].style.color = "#b0afae";
            spParlay.childNodes[0].style.color = "#b0afae";
            spReverse.childNodes[0].style.color = "#b0afae";


            document.getElementById("lbtParlay2").style.color = "#b0afae";
            document.getElementById("lbtReverse2").style.color = "#b0afae";
            document.getElementById("lbtTeaser2").style.color = "#b0afae";


            document.getElementById("lbtParlay2").onclick = "";
            document.getElementById("lbtReverse2").onclick = "";
            document.getElementById("lbtTeaser2").onclick = "";

            spTeaser.lang = "disable";
            spParlay.lang = "disable";
            spReverse.lang = "disable";
        }
        if (hfTeaser.value == "disable") {
            spTeaser.childNodes[0].style.color = "#b0afae";
            spTeaser.lang = "disable";
            document.getElementById("lbtTeaser2").style.color = "#b0afae";
            document.getElementById("lbtTeaser2").onclick = "";
        }
    } catch (e) { }

}
/*
  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-25971372-9']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();
*/