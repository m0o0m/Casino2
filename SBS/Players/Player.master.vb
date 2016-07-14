Imports SBCBL.UI
Imports SBCBL.CacheUtils
Imports System.Data
Imports SBCBL.Managers

Partial Class SBS_Players_Player
    Inherits System.Web.UI.MasterPage

#Region "Event"

    Dim defaultConyRight As String = ""

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
        oSetting = oSetting.LoadByUrl(Request.Url.Host)

        If oSetting IsNot Nothing Then
            Dim themeName As String = ""

            If Not String.IsNullOrEmpty(oSetting.ColorScheme) Then
                themeName = oSetting.ColorScheme.ToLower().Trim()
            End If

            Select Case themeName
                Case "layout1"
                    _cssLayout1.Visible = True
                    _headScriptsLayout1.Visible = True
                    _scriptsLayout1.Visible = True
                Case "layout4"
                    InitDefaultTheme()
                Case "layout5"
                    _cssLayout5.Visible = True
                    _headScriptsLayout5.Visible = True
                    _scriptsLayout5.Visible = True
                Case Else
                    InitDefaultTheme()
            End Select
            ' set background

            If Not String.IsNullOrWhiteSpace(oSetting.BackgroundImage) Then
                bdPlayer.Style.Add("background-image", oSetting.BackgroundImage)
            End If

            If Not String.IsNullOrWhiteSpace(oSetting.CopyrightName) Then
                defaultConyRight = oSetting.CopyrightName
            End If

        Else
            InitDefaultTheme()
        End If

        ltrCopyRight.Text = defaultConyRight
    End Sub

    Public Function DisableMenu() As String
        Dim userSession As New CSBCSession()
        If userSession.UserType = SBCBL.EUserType.Agent Then
            Return "Onclick='return false;'"
        End If
        Return ""
    End Function

#End Region

    'Public Sub DisplayInfo()
    '    Dim userSession As New CSBCSession()
    '    Dim sUserID As String = userSession.UserID.ToString
    '    If Not String.IsNullOrEmpty(sUserID) Then
    '        Dim oCacheManager As CCacheManager = New CCacheManager()
    '        ltlBalance.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID).BalanceAmount, SBCBL.std.GetRoundMidPoint())
    '        ltlCreditLimit.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID).Template.CreditMaxAmount, SBCBL.std.GetRoundMidPoint())
    '        ltlPendingAmount.Text = FormatNumber(oCacheManager.GetPlayerInfo(sUserID).PendingAmount, SBCBL.std.GetRoundMidPoint())
    '        ltlLoginID.Text = oCacheManager.GetPlayerInfo(sUserID).Login
    '        pnPlayerInfo.Visible = True
    '    Else
    '        pnPlayerInfo.Visible = False
    '    End If
    'End Sub

    Private Sub InitDefaultTheme()
        _cssLayout4.Visible = True
        _headScriptsLayout4.Visible = True
        _scriptsLayout4.Visible = True
        _headerTopBarLayout4.Visible = True
        _mainMenuLayout4.Visible = True
        _breadCrumbLayout4.Visible = True
    End Sub
End Class

