Imports SBCBL.std

Partial Class SBS_Players_Casino
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'If UserSession.PlayerUserInfo.RequirePasswordChange Then '' Redirect to change password page
        '    Response.Redirect("AccountStatus.aspx?RequireChangePass=Y")
        'End If
        MenuTabName = "CASINO"
        PageTitle = "Casino"
        DisplaySubTitlle(Me.Master, "Casino")
        CurrentPageName = "Casino"
    End Sub

    Protected Sub lbnCasino_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnCasino.Click
        'update casino amount
        If Not UserSession.PlayerUserInfo.HasCasino Then
            ClientAlert("Please Contact Your Agent ", True)
            Return
        End If
        'Dim oCasinoManager As New SBCBL.Managers.CCasinoMananger()
        'oCasinoManager.UpdateCasinoBalance(UserSession.PlayerUserInfo.BalanceAmount, _
        '                                   UserSession.PlayerUserInfo.Template.CasinoMaxAmount, _
        '                                   UserSession.PlayerUserInfo.Login)

        'encrypt casino password
        Dim oTriple As New SBCBL.CCasinoDES()
        Dim sUserInfo As String = oTriple.Encrypt(UserSession.PlayerUserInfo.Login & SBCBL.std.CASINO_SUFFIX & "|" & UserSession.PlayerUserInfo.Password)

        sUserInfo = HttpUtility.UrlEncode(sUserInfo)
        SBCBL.std.OpenPopupWindow(String.Format("http://casino.tigersb.com/lobby.jsp?alogin=Y&uinfo={0}&" & _
                                                "jsessionid=aM7-boIgvWS6VcBgXG&free=1&h1=600&w1=800", sUserInfo), 800, 600, "", _
                                                False, False, False, False, False, False, True)
    End Sub
End Class
