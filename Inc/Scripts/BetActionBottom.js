
//function lbtStraight_Click() {
//    DisplaySubTitle("Straight");
//    LoadBettingData("Straight");

//}
//function lbtParlay_Click() {
//    var spParlay = document.getElementById("spParlay"); //
//    var menuActive ="parplay"; //spParlay.lang;
//    if (menuActive != "disable") {
//        DisplaySubTitle("Parlay");
//        LoadBettingData("Parlay");
//    }

//}
//function lbtTeaser_Click() {
//    var spTeaser = document.getElementById("spTeaser"); //
//    var menuActive = spTeaser.lang;
//    if (menuActive != "disable") {
//        DisplaySubTitle("Teaser");
//        LoadBettingData("Teaser");
//    }


//}
//function lbtReverse_Click() {

//    var spReverse = document.getElementById("spReverse"); //
//    var menuActive = spReverse.lang;
//    if (menuActive != "disable") {
//        DisplaySubTitle("Reverse");
//        LoadBettingData("Reverse");
//    }
//}

//document.getElementById("lbtStraight").href = "#";
//document.getElementById("lbtParlay").href = "#";
//document.getElementById("lbtTeaser").href = "#";
//document.getElementById("lbtReverse").href = "#";

//function changeColor() {
//    try {
//        setActiveColorSubMenu(document.getElementById("hfStraightID").value, "lbtStraight")
//        setActiveColorSubMenu(document.getElementById("hfParlayID").value, "lbtParlay");
//        setActiveColorSubMenu(document.getElementById("hfTeaserID").value, "lbtTeaser");
//        setActiveColorSubMenu(document.getElementById("hfReverseID").value, "lbtReverse");
//    } catch (e) { }
//}
//function setActiveColorSubMenu(BetactionID, SubMenuID) {
//    if (document.getElementById(BetactionID).style.color == "Yellow" || document.getElementById(BetactionID).style.color == "yellow") {
//        document.getElementById(SubMenuID).parentNode.className = "menu_active"; // document.getElementById("<%=Parlay.ClientID %>").style.background;
//        document.getElementById(SubMenuID).parentNode.style.display = "inline";
//        if (document.all) {
//            document.getElementById(SubMenuID).parentNode.style.styleFloat = "left";
//        }
//        else {
//            document.getElementById(SubMenuID).parentNode.style.cssFloat = "left";
//        }
//        document.getElementById(SubMenuID).parentNode.style.height = "28px";
//        document.getElementById(SubMenuID).style.color = "yellow";
//        document.getElementById(SubMenuID + "2").style.color = "yellow";
//    }
//    else {
//        if (document.getElementById(SubMenuID).parentNode.lang != "disable") {
//            document.getElementById(SubMenuID).style.color = "White";
//            document.getElementById(SubMenuID + "2").style.color = "White";
//        }
//        var sColor = document.getElementById(document.getElementById("hfColorID").value).value;
//        if (sColor.toUpperCase() == "RED") {
//            document.getElementById(SubMenuID).parentNode.style.background = "#992929";
//        } else if (sColor.toUpperCase() == "GREEN") {
//            document.getElementById(SubMenuID).parentNode.style.background = "#839300";
//        } else if (sColor.toUpperCase() == "BLACK") {
//            document.getElementById(SubMenuID).parentNode.style.background = "#000";
//        }
//        else if (sColor.toUpperCase() == "SKY") {
//            document.getElementById(SubMenuID).parentNode.style.background = "#09a7d7";
//        } else if (sColor.toUpperCase() == "BLUE") {
//            document.getElementById(SubMenuID).parentNode.style.background = "#2c5296";
//        }

//        document.getElementById(SubMenuID).parentNode.className = "submenu";
//    }
//}

//function DisplaySubTitle(sBettingType) {
//    var odivSubTitle = $get('ctl00_divSubTitle');
//    if (odivSubTitle) {
//        if (sBettingType == 'Straight') {
//            odivSubTitle.innerHTML = 'Straight Bet';
//        }
//        if (sBettingType == 'Parlay') {
//            odivSubTitle.innerHTML = 'Parlay';
//        }
//        if (sBettingType == 'Teaser') {
//            odivSubTitle.innerHTML = 'Teaser';
//        }
//        if (sBettingType == 'Reverse') {
//            odivSubTitle.innerHTML = 'Action Reverse';
//        }
//    }
//}



function LoadBettingData(oBettingType) {
//debugger;
var IsWagers=document.getElementById(document.getElementById("hfIsWagersID").value);
if ( IsWagers.value== "False") 
{
   var hfBetTypeActive = document.getElementById(document.getElementById("htmlhfBetTypeActive").value);
hfBetTypeActive.value = oBettingType;
//ActiveMenu(oBettingType);
//SetControlForSelectedBettingType(oBettingType);
PageMethods.ResetTicket(document.getElementById("htmlhfselectPlayer").value);
return false;
}
//SelectWager(oBettingType)
}

function ActiveMenu(oBettingType) {
//document.getElementById(document.getElementById("hfReverseID").value).style.color = 'white';
//document.getElementById(document.getElementById("hfStraightID").value).style.color = 'white';
//document.getElementById(document.getElementById("hfParlayID").value).style.color = 'white';
//document.getElementById(document.getElementById("hfTeaserID").value).style.color = 'white';
//switch (oBettingType) {
//case "Reverse":
//document.getElementById(document.getElementById("hfReverseID").value).style.color = 'Yellow';
//break;
//case "Parlay":
//document.getElementById(document.getElementById("hfParlayID").value).style.color = 'yellow';
//break;
//case "Teaser":
//document.getElementById(document.getElementById("hfTeaserID").value).style.color = 'yellow';
//break;
//default:
//document.getElementById(document.getElementById("hfStraightID").value).style.color = 'yellow';
//break;
//}
//changeColor();
}

//   
//   
//function SetControlForSelectedBettingType(oBettingType) {
//var inputArr = document.getElementById("content").getElementsByTagName("input");
//var spanArr = document.getElementById("content").getElementsByTagName("span");
//var thArr = document.getElementsByTagName("th");
//var trArr = document.getElementsByTagName("tr");
//var tdArr = document.getElementsByTagName("td");
//var oDisplayTextBox = "none";
//var oDisplayCheckBox = "none";
//if (oBettingType != "Straight")
//oDisplayCheckBox = "block";
//else
//oDisplayTextBox = "block";
//for (var i = 0; i < inputArr.length; i++) 
//{
//if (inputArr[i].type == "text") {
//inputArr[i].style.display = oDisplayTextBox;
//inputArr[i].value="";

//}
//else if (inputArr[i].type == "checkbox")
//{
//inputArr[i].parentNode.style.display = oDisplayCheckBox;
//inputArr[i].checked = false ;


//}
//   
//}
////hidden th team total
// for (var j = 0; j < tdArr.length; j++) 
// {
//if ( tdArr[j].className =="tdTeamTotal")
//{
//   tdArr[j].parentNode.cells[0].colSpan = oDisplayTextBox=="block"?"7":"6";
//}
////hidden td 
//if ( tdArr[j].className =="tdTeamTotalh")
//{
//tdArr[j].style.display = oDisplayTextBox=="block"?"":"none";
//}
// }
//   for (var j = 0; j < thArr.length; j++) 
// {
//if ( thArr[j].className =="tdTeamTotal")
//{
//thArr[j].style.display = oDisplayTextBox=="block"?"block":"none";
//}

// }   
// //show check box tame total
//for (var j = 0; j < spanArr.length; j++) 
// {
//if ( spanArr[j].className =="TeamTotal")
//{
//spanArr[j].style.display = oDisplayTextBox=="block"?"inline-block":"none";;
//   //  spanArr[j].parentNode.parentNode.parentNode.style.display = oDisplayTextBox=="inline-block"?"block":"none";
//}
// }
//  //hidden quarter   
// 
//  for (var j = 0; j < trArr.length; j++) 
// {
//if ( trArr[j].getAttribute("context") =="1Q" ||trArr[j].getAttribute("context") =="2Q"||trArr[j].getAttribute("context") =="3Q"||trArr[j].getAttribute("context") =="4Q")
//{
//trArr[j].style.display = oDisplayTextBox=="block"?"":"none";
//   //  spanArr[j].parentNode.parentNode.parentNode.style.display = oDisplayTextBox=="inline-block"?"block":"none";
//}
// }
//return true;
//}
//SetControlForSelectedBettingType(document.getElementById(document.getElementById("htmlhfBetTypeActive").value).value);
// //window.onload =SetControlForSelectedBettingType(document.getElementById("<%=hfBetTypeActive.ClientID%>").value);
//function ClearWager()
//{
//var inputArr = document.getElementById("content").getElementsByTagName("input");
//for (var i = 0; i < inputArr.length; i++) 
//{
//if (inputArr[i].type == "text") {
//inputArr[i].value="";
//}
//else if (inputArr[i].type == "checkbox")
//{
//inputArr[i].checked = false ;
//}
//}
//return false;
//}

//function teamTotal_onclick(owner, awayID)
//{
//if (owner.checked) document.getElementById(awayID).checked = false;

//}


function showRisk(txtRisk,txtWin,bshowRisk){
   
    document.getElementById(txtRisk).value="";
     document.getElementById(txtWin).value="";
    if(bshowRisk)
    {
        document.getElementById(txtRisk).style.display="";
        document.getElementById(txtWin).style.display="none";
    }
    else
    {
         document.getElementById(txtRisk).style.display="none";
        document.getElementById(txtWin).style.display="";
    }
}


var strGameType = $("#BetTypeActive2").html().trim();

function activeMenu() {
    if (strGameType.indexOf("Board") >= 0) {
        $("#board").removeClass("btnGame").css("color", "#23baff").addClass("btnGameActive2");
        $("#board").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right")
    }
    else if (strGameType.indexOf("Reverse") >= 0) {
        $("#reverse").removeClass("btnGame").css("color", "#23baff").addClass("btnGameActive2");
        $("#reverse").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right")
    }
    else if (strGameType.indexOf("Straight") >= 0) {
        $("#straight").removeClass("btnGame").css("color", "#23baff").addClass("btnGameActive2");
        $("#straight").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
    }
    else if (strGameType.indexOf("Teaser") >= 0) {
        $("#teaser").removeClass("btnGame").css("color", "#23baff").addClass("btnGameActive2");
        $("#teaser").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
    }
    else if (strGameType.indexOf("If Win or Push") >= 0) {
        $("#push").addClass("btnGameActive2").css("color", "#23baff");
       // $("#push").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
    }
    else if (strGameType.indexOf("Prop") >= 0) {
        $("#prop").removeClass("btnGame").css("color", "#23baff").addClass("btnGameActive2");
        $("#prop").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
    }
    else if (strGameType.indexOf("If Win") >= 0) {
        $("#win").addClass("btnGameActive2").css("color", "#23baff");
       // $("#win").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
    }
    else if (strGameType.indexOf("Parlay") >= 0) {
   
        $("#parlay").removeClass("btnGame").css("color", "#23baff").addClass("btnGameActive2");
        $("#parlay").parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
    }
}
activeMenu();
