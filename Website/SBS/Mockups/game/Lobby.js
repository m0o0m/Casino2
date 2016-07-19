
// provides access to querystring collection
function Widgets_QueryString(){var aQS;var sQS = window.location.search.toString();sQS = sQS.substring(1, sQS.length);if (sQS != null && sQS != ""){aQS = sQS.split("&");for (var i = 0; i < aQS.length; i++){aQS[i] = aQS[i].split("=");}}else{aQS = new Array(0);}this.Get = function (sKey){sKey = escape(sKey);for (var i = 0; i < aQS.length; i++){if (aQS[i][0] == sKey){return unescape(aQS[i][1]);}}return null;};this.Count = aQS.length;};
function Widgets_Request(){this.QueryString = new Widgets_QueryString;};
var Request = new Widgets_Request();

// deprecated syntax support
function CasinoGameOpenDemo() { CasinoGameOpen(arguments[0], -1); }
function CasinoGameOpenLogin() { CasinoGameOpen(arguments[0], 1); }
function OnGameClicked() { CasinoGameOpen(arguments[0]); }
function OnPlayClicked() { CasinoGameOpen(arguments[0],arguments[1]); }

// game launch
function CasinoGameOpen(nCasinoGameId, nAccountId, sGameCode)
{
   var LaunchForm = document.forms["LaunchForm"];
   var LobbyForm = document.forms["LobbyForm"];
   if (LaunchForm == null){alert("Launch Form not found"); return;}
   if (LobbyForm == null){alert("Lobby Form not found"); return;}
   
   if (nAccountId != null) LaunchForm["AccountId"].value = nAccountId;
   
   // verify login
   var token = LaunchForm["Token"].value;
   var nAccountId = LaunchForm["AccountId"].value;
   if (nAccountId == "1" && token == "") return;
   
   LaunchForm["CasinoGameId"].value = nCasinoGameId;
   if (LaunchForm["GameCode"] != null) LaunchForm["GameCode"].value = sGameCode;
   
   var windowname = "Casino";
   if (nCasinoGameId != null && nCasinoGameId > 0) windowname += nCasinoGameId;
   if (sGameCode != null) windowname += sGameCode;
   if (nAccountId == -1) windowname += "aDemo";
   else if (nAccountId != null) windowname += "a" + nAccountId;
   windowname += "Sol";
   
   LaunchForm.target = windowname;
   
   var wWindow = popupGame(windowname);
   if (wWindow == null)
   {
      alert("Unable to open game window. You may have a pop-up blocker enabled.");
      return;
   }
   try 
   {
      wWindow.moveTo(0,0);
   } 
   catch(e){}
   try 
   {
      wWindow.resizeTo(screen.width,screen.height);
      wWindow.focus();
   } 
   catch(e){}
   LaunchForm.submit();
}

// lobby functions

function OnCategoryClicked(CategoryId, deprecated, TabCode)
{
   var LobbyForm = document.forms["LobbyForm"];
   LobbyForm["catid"].value = CategoryId;
   LobbyForm["tabcode"].value = TabCode;
   LobbyForm.submit();
}

function RefreshLobby()
{
   document.forms["LobbyForm"].submit();
}

function OnLobbyLoad()
{
   if (document.location.search != null && document.location.search.length > 2)
   {
      if (Request.QueryString.Get("CasinoGameId") != null) 
      CasinoGameOpen(Request.QueryString.Get("CasinoGameId"));
      if (Request.QueryString.Get("GameCode") != null)
      CasinoGameOpen(0,null,Request.QueryString.Get("GameCode"));
      RefreshLobby();
   }
}

function Logout()
{
   var LobbyForm = document.forms["LobbyForm"];
   LobbyForm["token"].value = "";
	LobbyForm["action"].value = "LOGOUT";
	LobbyForm.submit();
}

function popupNews(url, width, height)
{
   try
   {
      var settings="toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width="+width+",height="+height;
      winpops=window.open(url,"",settings);
   }
   catch(e){return;}
}

function popupGame(windowname)
{
   try
   {
      if (windowname == null) windowname = "casinosol";
      var settings="toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=yes,width="+screen.width+",height="+screen.height;
      return window.open("",windowname,settings);
   }
   catch(e)
   {
      alert(e.message + "\n" + windowname + "\n" + settings);
      return null;
   }
}

function CallCashOut(queryString)
{
	var request;
	if (!window.ActiveXObject)
		request = new XMLHttpRequest();
	else if (navigator.userAgent.toLowerCase().indexOf('msie 5') == -1)
		request = new ActiveXObject("Msxml2.XMLHTTP");
	else
		request = new ActiveXObject("Microsoft.XMLHTTP");

	request.open("GET", "../chiptransfer/cashout.aspx?" + queryString);
	request.send(null);
}
