<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TopMenuPlayer.ascx.vb" Inherits="SBS_Players_Inc_TopMenuPlayer" %>
<style>

    
    
</style>

<script type="text/javascript" language="javascript">
    function btnActive(me) {
        //$(me).removeClass("btnGame2bet").addClass("btnGame2betActive")
        $(me).parent().css("background", "black url(/2bet/wager_center.png) no-repeat right");
        $(me).css("color", "#23baff");
    }
    function btnNoActive(me) {
        // $(me).removeClass("btnGame2betActive").addClass("btnGame2bet")
        if ($(".btnGameActive2").length > 0 && $(".btnGameActive2").attr("id")==$(me).attr("id")) return;
        if ($($(me).parent()).attr("class") == "btnGame2bet"){
        $(me).parent().css("background", "url(/2bet/wager_center.png) no-repeat right");
        $(me).css("color", "white");
        }
    }
</script>

    <div class="btnGame2bet"><a href="Default.aspx?bettype=BetIfAll" id="A1" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" style="font-size:19px;"  > Bet The Board</a></div>
    <div class="btnGame2bet"><a href="Default.aspx?bettype=BetTheBoard" id="board" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)"  style="font-size:19px"> Straight</a></div>
	<div class="btnGame2bet"><a href="Default.aspx?bettype=Parlay" id="parlay" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)"  style="font-size:19px">Parlay</a></div>
	<div class="btnGame2bet"><a href="Default.aspx?bettype=Teaser" id="teaser" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)"  style="font-size:19px">Teaser</a></div>
	
	<div class="btnGame2bet" style="top:-8px"><a style="position:relative;top:3px" href="Default.aspx?bettype=Reverse" id="reverse" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)"  >Action Reverse</a></div>
	<div class="btnGame2bet" style="top:-8px"><a style="position:relative;top:3px" href="Default.aspx?bettype=Prop" id="prop" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)"  >Futures / Props</a></div>
	<div class="btnGame2bet" style="top:-8px"><a style="position:relative;top:3px" href="Casino.aspx?bettype=casino" id="casino" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)"  >Casino</a></div>
	
