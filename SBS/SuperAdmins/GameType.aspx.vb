Imports SBCBL.Managers
Imports System.Web.Services
Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class GameType
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Game Type Setup"
            SideMenuTabName = "GAME_TYPE"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Game Type Setup")
        End Sub

        <WebMethod()> _
      Public Shared Sub UpdateGameType(ByVal psSysSettingID As String, ByVal psKey As String, ByVal psValue As String)
            Dim oSysManager As New CSysSettingManager
            Dim sGameType As String = SBCBL.std.GetSiteType & " GameType"

            If oSysManager.UpdateKeyValue(psSysSettingID, psKey, psValue) Then
                '' Clear cache
                HttpContext.Current.Cache.Remove("SBC_ALL_SYS_SETTINGS_" & sGameType)
            End If
        End Sub
    End Class
End Namespace