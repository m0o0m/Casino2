<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ListenMedia.aspx.vb" Inherits="ListenMedia" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="margin:0;padding:0">
    <form id="form1" runat="server">
    <object id="player" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" name="player"
        width="400" height="315">
        <param name="movie" value="../Inc/Scripts/flvplayer.swf" />
        <param name="allowfullscreen" value="true" />
        <param name="allowscriptaccess" value="always" />
        <param name="flashvars" value='file=/Utils/MediaFille.ashx?fname=<%= request("fname") %>' />
        <embed type="application/x-shockwave-flash" id="player2" name="player2" src="../Inc/Scripts/flvplayer.swf"
        width="400" height="315" allowscriptaccess="always" allowfullscreen="true" flashvars='file=/Utils/MediaFille.ashx?fname=<%= request("fname") %>' />
    </object>
    <div>
    </div>
    </form>
</body>
</html>
