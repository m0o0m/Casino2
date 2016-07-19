<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TopMenu.ascx.vb" Inherits="SBSAgents.TopMenu" %>

<script type="text/javascript" language="javascript">
    function btnActive(me) {
        $(me).removeClass("btnGame").addClass("btnGameActive")
    }
    function btnNoActive(me) {
        $(me).removeClass("btnGameActive").addClass("btnGame")
    }
</script>
    <a href="/SBS/CallCenter/default.aspx" id="A1" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Home</a>
      <a href="/SBS/Agents/SelectPlayers.aspx" id="A4" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Game Manager</a>
    <a href="/SBS/CallCenter/OpenBets.aspx" id="board" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">Pending</a>
    <a href="/SBS/CallCenter/History.aspx" id="A2" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White">History</a>
   <a href="/SBS/CallCenter/AccountStatus.aspx" id="A3" onmouseover="btnActive(this)" onmouseout="btnNoActive(this)" class="btnGame" style="color:White"> Account Info</a>
  
   
