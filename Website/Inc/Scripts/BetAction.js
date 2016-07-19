
function Factorial(n, r) {
    // calculates n! / r!
    n = parseInt(n);
    r = parseInt(r);
    var result = 1;
    if (r < 2) r = 1;
    for (var i = r + 1; i <= n; i++) {
        result *= i
    }
    return result;
}

function Combinatorial(n, r) {
    // calculates n choose r
    return (Factorial(n, r) / Factorial((n - r), 1));
}

function EnumerateCombinations(n, a, z, D) { // List combinations - D starts empty
    if (n == 0) {
        D[D.length] = z;
        return;
    }
    for (var j = 0; j < a.length; j++) {
        EnumerateCombinations(n - 1, a.slice(j + 1), z + (z != "" ? "|" : "") + a[j], D);
    }
    return;
}

function CalcParlay(t, wager, type, result) {
    try {
        var objs = document.getElementsByTagName('input');

        var BetType = type.value;
        var Bet = parseFloat("0" + t.value.replace(/,/g, ""));
        var MaxRisk = 0;
        var MaxWin = 0;
        var ItemArray = new Array;
        var BuyPoints = new Array;
        var ResultArray = new Array;

        if (Bet != 0) {
            for (var i = 0; i < objs.length; i++) {
                var obj = objs[i];

                if (obj.getAttribute('Wager') == wager) {
                    ItemArray[ItemArray.length] = obj;
                    var buyPoint = obj.parentNode.getElementsByTagName('select');
                    if (buyPoint && buyPoint.length > 0) {
                        if (buyPoint[0].value != "") {
                            BuyPoints[BuyPoints.length] = parseFloat(buyPoint[0].value.split("|")[1]);
                        } else {
                            BuyPoints[BuyPoints.length] = 0
                        }
                    } else {
                        BuyPoints[BuyPoints.length] = 0
                    }
                }
            }

            var NumGames = ItemArray.length;

            if (BetType == "Parlay") {
                BetType = "" + NumGames;
            }

            for (var k = parseInt(BetType.replace(/s/g, "")); k >= 2; k--) {
                var D = new Array;
                var tmpArray = new Array;
                MaxRisk += Bet * Combinatorial(NumGames, k);
                for (var i = 0; i < NumGames; i++) {
                    ResultArray[i] = i;
                }
                EnumerateCombinations(k, ResultArray, tmpArray, D);
                for (var i = 0; i <= D.length - 1; i++) {
                    var DArray = D[i].split("|");
                    var thisMaxParlayLine = 1;
                    var thisParlayLine = 1;
                    var thisParlayRes = 1;
                    for (var j = 0; j <= DArray.length - 1; j++) {
                        var line = CalcRate(parseFloat(ItemArray[DArray[j]].getAttribute('Rate')) + parseFloat(BuyPoints[DArray[j]]));
                        thisMaxParlayLine *= line;
                    }
                    MaxWin += Math.round(CustomParlayAmount(DArray.length, (thisMaxParlayLine - 1) * Bet) * 100) / 100;
                }
                if (BetType.substring(BetType.length - 1) != "s")
                    break;

            }
        }
        result.innerHTML = "Risk/Win: " + Math.round(MaxRisk) + "/" + Math.round(MaxWin);
        formatNumber(t, 2);
    } catch (e) { }
}

function CustomParlayAmount(nItems, nAmount) {

    //var objParlays = ParlayJson;

    if ((nItems < 2) || (nItems > 15)) {
        return nAmount;
    }

    return nAmount * Math.round(parseFloat(objParlays.Parlays[nItems - 2].ParlayPayout) * 10000) / 10000;
}

function CalcWinReverse(t, wager, result) {
    var objs = document.getElementsByTagName('input');

    var Bet = parseFloat("0" + t.value.replace(/,/g, ""));
    var MaxWin = 0;
    var ItemArray = new Array;

    if (Bet != 0) {
        for (var i = 0; i < objs.length; i++) {
            var obj = objs[i];

            if (obj.getAttribute('Wager') == wager) {
                ItemArray[ItemArray.length] = obj;
            }
        }

        var NumGames = ItemArray.length;

        if (NumGames > 1) {
            MaxWin = Bet * 4
        }
    }

    result.value = Math.round(MaxWin);
    formatNumber(result, 2);
    formatNumber(t, 2);



}

function CalcBetReverse(t, wager, result) {
    var objs = document.getElementsByTagName('input');

    var MaxWin = parseFloat("0" + t.value.replace(/,/g, ""));
    var Bet = 0;
    var ItemArray = new Array;

    if (MaxWin != 0) {
        for (var i = 0; i < objs.length; i++) {
            var obj = objs[i];

            if (obj.getAttribute('Wager') == wager) {
                ItemArray[ItemArray.length] = obj;
            }
        }

        var NumGames = ItemArray.length;

        if (NumGames > 1) {
            Bet = MaxWin / 4;
        }
    }

    result.value = Math.round(Bet);
    formatNumber(result, 2);
    formatNumber(t, 2);
}

function CalcWinStraight(t, buyPoint, wager, result) {
    var objs = document.getElementsByTagName('input');

    var Bet = parseFloat("0" + t.value.replace(/,/g, ""));
    var MaxWin = Bet;
    var Rate = 0;

    if (Bet != 0) {
        if (buyPoint && buyPoint.value != "") {
            Rate = parseFloat(buyPoint.value.split("|")[1]);
        }

        for (var i = 0; i < objs.length; i++) {
            var obj = objs[i];

            if (obj.getAttribute('Wager') == wager) {
                Rate += parseFloat(obj.getAttribute('Rate'));
                Rate = CalcRate(Rate);
            }
        }
    }


    result.value = Math.round(MaxWin * Rate - Bet);
    formatNumber(t, 2);
    formatNumber(result, 2);
}

function CalcRiskStraight(t, buyPoint, wager, result) {
    var objs = document.getElementsByTagName('input');

    var MaxWin = parseFloat("0" + t.value.replace(/,/g, ""));
    var MaxRisk = MaxWin;
    var Rate = 2;

    if (MaxWin != 0) {
        if (buyPoint) {
            Rate = parseFloat(buyPoint.value.split("|")[1]);
        }

        for (var i = 0; i < objs.length; i++) {
            var obj = objs[i];

            if (obj.getAttribute('Wager') == wager) {
                if (buyPoint) {
                    Rate += parseFloat(obj.getAttribute('Rate'));
                } else {
                    Rate = parseFloat(obj.getAttribute('Rate'));
                }
                Rate = CalcRate(Rate);
            }
        }
    }

    result.value = Math.round(MaxWin / (Rate - 1));
    formatNumber(t, 2);
    formatNumber(result, 2);
}

function CalcWinTeaser(t, wager, type, result) {
    var objs = document.getElementsByTagName('input');
    //var objTeasers =document.getElementById("TeaserJson").value;

    var Bet = parseFloat("0" + t.value.replace(/,/g, ""));
    var MaxWin = Bet;
    var Rate = 0;
    var BbPoint = 0;
    var FbPoint = 0;
    var MinTeam = 0;
    var MaxTeam = 0;
    var Members = 0;

    // Get Wager's members
    for (var i = 0; i < objs.length; i++) {
        var obj = objs[i];
        if (obj.getAttribute('Wager') == wager) {
            Members++;
        }
    }

    Members = "" + Members;
    // Get Rate
    for (var nRule = 0; nRule < objTeasers.TeaserRules.length; nRule++) {
        if (objTeasers.TeaserRules[nRule].RuleID == type.value) {
            for (var nTeam = 0; nTeam < objTeasers.TeaserRules[nRule].Teams.length; nTeam++) {
                if (objTeasers.TeaserRules[nRule].Teams[nTeam].TeamMember == Members) {
                    Rate = CalcRate(eval(objTeasers.TeaserRules[nRule].Teams[nTeam].Value));
                    BbPoint = parseFloat(objTeasers.TeaserRules[nRule].BasketballPoint);
                    FbPoint = parseFloat(objTeasers.TeaserRules[nRule].FootballPoint);
                    break;
                }
            }
            MinTeam = objTeasers.TeaserRules[nRule].MinTeam;
            MaxTeam = objTeasers.TeaserRules[nRule].MaxTeam;
            break;
        }
    }

    result.value = "0";
    if (Rate != 0) {
        AddTeaserPoint(wager, BbPoint, FbPoint);

        if (Bet != 0) {
            result.value = Math.round(MaxWin * Rate - Bet);
        }
    } else {
        if (MinTeam == MaxTeam) {
            alert("This teaser rule has to have " + MinTeam + " bets.");
        } else {
            alert("This teaser rule has to have from " + MinTeam + " bets to " + MaxTeam + " bets.");
        }
    }

    formatNumber(t, 2);
    formatNumber(result, 2);
}

function CalcRiskTeaser(t, wager, type, result) {
    var objs = document.getElementsByTagName('input');
    var MaxWin = parseFloat("0" + t.value.replace(/,/g, ""));
    var MaxRisk = MaxWin;
    var Rate = 0;
    var BbPoint = 0;
    var FbPoint = 0;
    var MinTeam = 0;
    var MaxTeam = 0;
    var Members = 0;

    // Get Wager's members
    for (var i = 0; i < objs.length; i++) {
        var obj = objs[i];
        if (obj.getAttribute('Wager') == wager) {
            Members++;
        }
    }

    Members = "" + Members;
    // Get Rate
    for (var nRule = 0; nRule < objTeasers.TeaserRules.length; nRule++) {
        if (objTeasers.TeaserRules[nRule].RuleID == type.value) {
            for (var nTeam = 0; nTeam < objTeasers.TeaserRules[nRule].Teams.length; nTeam++) {
                if (objTeasers.TeaserRules[nRule].Teams[nTeam].TeamMember == Members) {
                    Rate = CalcRate(eval(objTeasers.TeaserRules[nRule].Teams[nTeam].Value));
                    BbPoint = parseFloat(objTeasers.TeaserRules[nRule].BasketballPoint);
                    FbPoint = parseFloat(objTeasers.TeaserRules[nRule].FootballPoint);
                    break;
                }
            }
            MinTeam = objTeasers.TeaserRules[nRule].MinTeam;
            MaxTeam = objTeasers.TeaserRules[nRule].MaxTeam;
            break;
        }
    }

    result.value = "0";
    AddTeaserPoint(wager, BbPoint, FbPoint);
    if (Rate != 0) {
        if (MaxWin != 0 && Rate != 1) {
            result.value = Math.round(MaxWin / (Rate - 1));
        }
    } else {
        if (MinTeam == MaxTeam) {
            alert("This teaser rule has to have " + MinTeam + " bets.");
        } else {
            alert("This teaser rule has to have from " + MinTeam + " bets to " + MaxTeam + " bets.");
        }
    }

    formatNumber(t, 2);
    formatNumber(result, 2);
}

function AddTeaserPoint(wager, bbpoint, fbpoint) {
    var objs = document.getElementsByTagName('span');

    for (var i = 0; i < objs.length; i++) {
        var obj = objs[i];
        //alert(obj.getAttribute('Wager'));
        if (obj.getAttribute('Wager') == wager) {
            if (IsBasketball(obj.getAttribute('GameType'))) {
                AddPoint(obj, bbpoint);
            }
            if (IsFootball(obj.getAttribute('GameType'))) {
                AddPoint(obj, fbpoint);
            }
        }
    }
}

function IsFootball(psGameType) {
    switch (psGameType.toUpperCase()) {
        case "AFL FOOTBALL": return true;
        case "CFL FOOTBALL": return true;
        case "NCAA FOOTBALL": return true;
        case "NFL FOOTBALL": return true;
        case "NFL PRESEASON": return true;
        case "UFL FOOTBALL": return true;
    }

    return false;
}

function IsBasketball(psGameType) {
    switch (psGameType.toUpperCase()) {
        case "NBA BASKETBALL": return true;
        case "NCAA BASKETBALL": return true;
        case "WNBA BASKETBALL": return true;
        case "WNCAA BASKETBALL": return true;
        case "NCAA BASKETBALL": return true;
        case "WNCAA BASKETBALL": return true;
    }

    return false;
}


function IsSoccer(psGameType) {
    switch (psGameType.toUpperCase()) {
        case "ARGENTINA": return true;
        case "BRAZIL": return true;
        case "BUNDESLIGA": return true;
        case "LA LIGA": return true;
        case "LIGUE 1": return true;
        case "MEXICAN": return true;
        case "MLS": return true;
        case "NETHERLANDS": return true;
        case "PORTUGAL": return true;
        case "PREMIER": return true;
        case "SCOTLAND": return true;
        case "SERIE A": return true;
        case "WORLD CUP": return true;
        case "EURO CUPS": return true;
        case "SUPER LIGA": return true;
        case "EURO": return true;
        case "CHAMPIONS LEAGUE": return true;
        case "EUROPA LEAGUE": return true;
        case "COPA AMERICA": return true;
        case "CARLING CUP": return true;
        case "FA CUP": return true;
        case "CONCACAF": return true;
    }

    return false;
}


function AddPoint(obj, point) {
    var pnt = obj.getAttribute('Point');
    var dsp = obj.getAttribute('Status');
    var money = obj.getAttribute('Money');

    if (pnt != "") {
        if (dsp == "Over ") {
            pnt = "" + (parseFloat(pnt) - point);
        } else {
            pnt = "" + (parseFloat(pnt) + point);
        }
        dsp += pnt + " (" + money + ")";
    } else {
        dsp += money;
    }
    obj.innerHTML = dsp;
}

function CalcTeaser(type, wager, risk, result) {
    var objs = document.getElementsByTagName('input');
    //var objTeasers = document.getElementById("TeaserJson").value;

    var MaxWin = parseFloat("0" + result.value.replace(/,/g, ""));
    var MaxRisk = parseFloat("0" + risk.value.replace(/,/g, ""));

    if (MaxRisk != 0) {
        CalcWinTeaser(risk, wager, type, result)
    } else {
        CalcRiskTeaser(result, wager, type, risk)
    }
}

function CalcRate(nRate) {
    nRate = parseFloat(nRate);
    if (nRate < 0) {
        nRate = (-nRate + 100) / -nRate;
    } else if (nRate > 0) {
        nRate = (nRate + 100) / 100;
    } else {
        nRate = 0;
    }

    if (nRate != 0) {
        nRate = Math.round(nRate * 10000) / 10000;
    }

    return nRate;
}

window.HighlightIDs = new Array();

function HiglightBlock(ID, classname) {
    var o = document.getElementById(ID);
    if (o == null)
        return;
    o = window.getElementsByClassName(classname, '', o);
    if (o == null || o.length == 0)
        return;

    o = o[0];
    //o.style.backgroundColor='red';

    o.className += " HighLightBlock";
    //o.setAttribute("style","background-color:Red ");
    var timeout = window.setInterval(function() { DeHiglightBlock(o.id, timeout) }, 12000);

}

function DeHiglightBlock(ID, inter) {
    var o = document.getElementById(ID);
    if (o == null)
        return;
    o.className = o.className.replace(" HighLightBlock", "");

    o.style.backgroundColor = '';
    window.clearTimeout(inter);
}

function getElementsByClassName(className, tag, elm) {
    var testClass = new RegExp("(^|\\s)" + className + "(\\s|$)");
    var tag = tag || "*";
    var elm = elm || document;
    var elements = (tag == "*" && elm.all) ? elm.all : elm.getElementsByTagName(tag);
    var returnElements = [];
    var current;
    var length = elements.length;
    for (var i = 0; i < length; i++) {
        current = elements[i];
        if (testClass.test(current.className)) {
            returnElements.push(current);
        }
    }
    return returnElements;
}

function validActionType(t, psActionType, psGameType) {
    switch (psActionType.toUpperCase()) {
        case "TEASER":
            if (!(IsBasketball(psGameType) || IsFootball(psGameType))) {
                alert(psGameType & "doesn't have teaser type.");
                return false;
            }
            //case "PARLAY":
            //            if (IsSoccer(psGameType)) {
            //                t.checked = false;
            //                alert("Soccer doesn't have parlays type.");
            //                return false;
            //            }

    }

    return true;
}

function Betting(t, psGameID, psGameLineID, psGameType, psBookmaker,
psContext, psGameDate, psAwayTeam, psHomeTeam,
psAwayTeamNumber, psHomeTeamNumber, psAwayPitcher,
psHomePitcher, psAwayPitcherRH, psHomePitcherRH,
psBetType, psTeam, pnSpread, pnMoney, psCircled, psFav, psDescription, psBuyPointValue, IsCheckPitcher, pnLineSpread) {
    var bresult = false;
    var sP = document.getElementById("htmlhfselectPlayer").value;
    var sURL = "/Utils/Betting.aspx/SBSBetting";
    var sActionTypeID = document.getElementById("htmlhfBetTypeActive").value;
    var sActionType = document.getElementById(sActionTypeID).value;
    psAwayTeam = psAwayTeam.replace("'", "&quot;");
    psHomeTeam = psHomeTeam.replace("'", "&quot;");

    if (!t.checked) {
        sURL = "/Utils/Betting.aspx/SBSRemoveBet";
    }
    //var ddlBuyPointValue=t.parentNode.nextSibling.nextSibling;
    //var psBuyPointValue;
    //try{
    //psBuyPointValue=ddlBuyPointValue.options[ddlBuyPointValue.selectedIndex].value;
    //}catch(e){psBuyPointValue="";}
    if (psBuyPointValue != "" && document.getElementById(psBuyPointValue) != null) {

        ddlBuyPointValue = document.getElementById(psBuyPointValue);
        psBuyPointValue = psBuyPointValue + "|" + ddlBuyPointValue.selectedIndex; //ddlBuyPointValue.options[ddlBuyPointValue.selectedIndex].value;
    }
    try {
        if (validActionType(t, sActionType, psGameType)) {
            $.ajax({
                type: "POST",
                url: sURL,
                data: "{'psSelectedPlayerID':'" + sP + "','psGameID':'" + psGameID + "','psGameLineID':'" + psGameLineID + "','psActionType':'" + sActionType + "','psGameType':'" + psGameType + "','psBookmaker':'" + psBookmaker + "','psContext':'" + psContext + "','psGameDate':'" + psGameDate + "','psAwayTeam':'" + psAwayTeam + "','psHomeTeam':'" + psHomeTeam + "','psAwayTeamNumber':'" + psAwayTeamNumber + "','psHomeTeamNumber':'" + psHomeTeamNumber + "','psAwayPitcher':'" + psAwayPitcher + "','psHomePitcher':'" + psHomePitcher + "','psAwayPitcherRH':'" + psAwayPitcherRH + "','psHomePitcherRH':'" + psHomePitcherRH + "','psBetType':'" + psBetType + "','psTeam':'" + psTeam + "','pnSpread':'" + pnSpread + "','pnMoney':'" + pnMoney + "','psCircled':'" + psCircled + "','psFav':'" + psFav + "','pschkID':'" + t.id + "','psDescription':'" + psDescription + "','psBuyPointValue':'" + psBuyPointValue + "','pbIsCheckPitcher':'" + IsCheckPitcher + "','pnLineSpread':'" + pnLineSpread + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BettingCallBack
            });
        }
    } catch (e) {
        alert(e);
    }



    return true;
}

function BettingCallBack(result) {

    if ((result != null) || (result != "")) {
        var oJsResult = $.parseJSON(result.d);
        if (oJsResult) {
            if (oJsResult.ErrorMessage != "") {
                var chk = document.getElementById(oJsResult.chkID);
                if (chk) {
                    chk.checked = !chk.checked;
                }

                alert(oJsResult.ErrorMessage);
            } else {
            if (($("#BetTypeActive").html().trim() == "Straight" && document.getElementById(btnContinue).style.display=="none") || $("#BetTypeActive").html().trim() == "If Win or Push" || $("#BetTypeActive").html().trim() == "If Win") {
                    $("#" + btnContinue).click();
                }
            }
        }
    }

    return true;
}

/////////////////////////////////////////new fature bet the board
function showCheckBox(chk) {
    try {
        if (!chk.checked) {
            $(".tmpchk").remove();
            return;
        }
        $("#content table").find("input").each(
            function addCheckBox(index, input) {
                if (input.type == "text" && chk.checked) {
                    $("<input type='checkbox' class='tmpchk' onclick=\"fillValue(this,'#" + input.id + "')\" />").insertBefore(input);
                }

            }
        );
    } catch (e) { alert(e); }
}
function fillValue(chk, txtID) {
    try {
        var nValueBettheBoard = $("#txtMultiBet").val();
        if (nValueBettheBoard == "") {
            alert("An invalid amount for the wager as entered,enter a valid amount to use for all best!!!");
            return;
        }
        if (chk.checked) {
            $(txtID).val(nValueBettheBoard);
        }
        else {
            $(txtID).val("");
        }
    } catch (e) { alert(e); }
}


