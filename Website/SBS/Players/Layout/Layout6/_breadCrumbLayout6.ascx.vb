
Imports SBCBL.CacheUtils
Imports SBCBL.UI

Partial Class SBS_Players_Layout_Layout6_breadCrumbLayout6
    Inherits SBCBL.UI.CSBCUserControl
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Dim userSession As New CSBCSession()
            Dim sUserID As String = userSession.UserID.ToString
            If Not String.IsNullOrEmpty(sUserID) AndAlso userSession.UserType = SBCBL.EUserType.Player Then
                Dim oCacheManager As CCacheManager = New CCacheManager()
                lblAvailableBalance.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID).BalanceAmount, SBCBL.std.GetRoundMidPoint())
            End If
        End If

    End Sub
End Class
