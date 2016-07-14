<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HTMLContent.ascx.vb" Inherits="Inc_SuperAdmins_HTMLContent" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fck" %>
<script type="text/javascript" language="javascript">
    function FCKFix() {
        for (var i = 0; i < parent.frames.length; ++i) {
            if (parent.frames[i].FCK)
                parent.frames[i].FCK.UpdateLinkedField();
        }
        return true;
    }
</script>
<fck:FCKeditor runat="server" ID="fckContent" Width="100%" Height="500" />
<div class="mbxl"></div>
<asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClientClick="return FCKFix()" />
