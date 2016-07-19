Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports SBCBL
Imports SBCBL.CacheUtils
Namespace SBCSuperAdmin

    Partial Class AddNewGame
        Inherits SBCBL.UI.CSBCUserControl
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim odic As Dictionary(Of String, String) = New Dictionary(Of String, String)
                Dim odicTmp As Dictionary(Of String, String) = GetGameType()
                For Each strGameType As String In odicTmp.Keys
                    If IsSoccer(strGameType) Then
                        odic(strGameType) = strGameType
                    End If
                Next
                odic("MLB Baseball Live") = "MLB Baseball Live"
                odic("NFL Football Live") = "NFL Football Live"
                odic("WNBA Basketball Live") = "WNBA Basketball Live"
                ddlGameType.DataSource = odic
                ddlGameType.DataTextField = "Key"
                ddlGameType.DataValueField = "Value"
                ddlGameType.DataBind()
                bindData()
            End If
        End Sub


#Region "Bind Data"
        Private Sub bindData()
            Dim oToday = UserSession.ConvertToEST(DateTime.Now.ToUniversalTime()).AddHours(-12)
            Dim odtGames = New SBCBL.Managers.CGameManager().GetAgentAvailableNewGames(ddlGameType.SelectedValue, oToday, "pinnacle2")
            rptGameAdd.DataSource = odtGames
            rptGameAdd.DataBind()
        End Sub

#End Region



        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            AddGame()
        End Sub

        Private Function checkValidGame() As Boolean
            If String.IsNullOrEmpty(txtGameDate.Value) Then
                ClientAlert("Please, input GameDate", True)
                Return False
            ElseIf String.IsNullOrEmpty(txtAwayRotationNumber.Text) Then
                ClientAlert("Please, input AwayRotationNumber", True)
                Return False
            ElseIf String.IsNullOrEmpty(txtHomeRotationNumber.Text) Then
                ClientAlert("Please, input HomeRotationNumber", True)
                Return False
            ElseIf String.IsNullOrEmpty(txtDrawnRotationNumber.Text) Then
                ClientAlert("Please, input DrawnRotationNumber", True)
                Return False
            ElseIf String.IsNullOrEmpty(txtAwayTeam.Text) Then
                ClientAlert("Please, input AwayTeam", True)
                Return False
            ElseIf String.IsNullOrEmpty(txtHomeTeam.Text) Then
                ClientAlert("Please, input HomeTeam", True)
                Return False
            ElseIf String.IsNullOrEmpty(txtSecondHalfTime.Value) Then
                ClientAlert("Please, input SecondHalfTime", True)
                Return False
                'ElseIf (Math.Abs(SafeDouble(txtAwaySpread.Text)) > Math.Abs(SafeDouble(txtHomeSpread.Text))) OrElse (Math.Abs(SafeDouble(txtAwaySpread.Text)) < Math.Abs(SafeDouble(txtHomeSpread.Text))) Then
                '    ClientAlert("Spread is wrong", True)
                '    Return False
            End If
            Return True
        End Function

        Private Sub resetData()
            '' txtGameDate.Value = ""
            txtAwayRotationNumber.Text = ""
            txtHomeRotationNumber.Text = ""
            txtAwayTeam.Text = ""
            txtHomeTeam.Text = ""
            txtAwaySpread.Text = ""
            txtHomeSpread.Text = ""
            txtHomeMoney.Text = ""
            txtHomeSpreadMoney.Text = ""
            txtAwaySpreadMoney.Text = ""
            txtAwayMoneyLine.Text = ""
            txtAwayTotal.Text = ""
            txtTotalPointsOverMoney.Text = ""
            txtTotalPointsUnderMoney.Text = ""
            txtDrawMoney.Text = ""
            txtDrawnRotationNumber.Text = ""
        End Sub

        Private Sub AddGame()
            Dim oGameLineManager = New CGameLineManager()
            If checkValidGame() Then
                Dim sGameID As String = oGameLineManager.SaveNewGame(txtGameDate.Value, txtSecondHalfTime.Value, txtAwayRotationNumber.Text, txtHomeRotationNumber.Text, txtDrawnRotationNumber.Text, txtAwayTeam.Text, txtHomeTeam.Text, ddlGameType.SelectedValue)
                Dim oGameLine As New CGameLine()
                oGameLine.GameID = sGameID
                oGameLine.Context = ddlContext.SelectedValue
                oGameLine.FeedSource = SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER
                oGameLine.Bookmaker = "pinnacle2"

                oGameLine.HomeSpread = -SafeDouble(txtAwaySpread.Text)
                oGameLine.HomeSpreadMoney = SafeDouble(txtHomeSpreadMoney.Text)

                oGameLine.AwaySpread = SafeDouble(txtAwaySpread.Text)
                oGameLine.AwaySpreadMoney = SafeDouble(txtAwaySpreadMoney.Text)

                oGameLine.HomeMoneyLine = SafeDouble(txtHomeMoney.Text)
                oGameLine.AwayMoneyLine = SafeDouble(txtAwayMoneyLine.Text)

                oGameLine.TotalPoints = SafeDouble(txtAwayTotal.Text)
                oGameLine.TotalPointsOverMoney = SafeDouble(txtTotalPointsOverMoney.Text)
                oGameLine.TotalPointsUnderMoney = SafeDouble(txtTotalPointsUnderMoney.Text)
                oGameLine.DrawMoneyLine = SafeDouble(txtDrawMoney.Text)
                If oGameLineManager.SaveManualLine(oGameLine) Then
                    ClientAlert("Successfully Updated", True)
                    bindData()
                    resetData()
                Else
                    ClientAlert("Unsuccessfully Updated", True)
                End If
            End If
        End Sub

        Protected Function FormatValue(ByVal poValue As Object) As String
            If poValue.GetType() Is GetType(DBNull) Then
                Return ""
            Else
                Return SafeDouble(poValue).ToString()
            End If
        End Function

        Protected Sub rptGameAdd_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles rptGameAdd.Init

        End Sub

        Protected Sub rptGameAdd_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptGameAdd.ItemCommand
            If e.CommandName.Equals("UPDATE", StringComparison.CurrentCultureIgnoreCase) Then
                Dim hfGameID = CType(e.Item.FindControl("hfGameID"), HiddenField)
                Dim txtGameDateUP = CType(e.Item.FindControl("txtGameDate"), SBCWebsite.Inc_DateTime)
                Dim ddlGameContext = CType(e.Item.FindControl("ddlContext"), DropDownList)

                Dim txtAwayRotationNumberUP = CType(e.Item.FindControl("txtAwayRotationNumber"), TextBox)
                Dim txtHomeRotationNumberUP = CType(e.Item.FindControl("txtHomeRotationNumber"), TextBox)
                Dim txtAwayTeamUp = CType(e.Item.FindControl("txtAwayTeam"), TextBox)
                Dim txtHomeTeamUp = CType(e.Item.FindControl("txtHomeTeam"), TextBox)
                Dim txtAwaySpreadUp = CType(e.Item.FindControl("txtAwaySpread"), TextBox)
                Dim txtHomeSpreadUp = CType(e.Item.FindControl("txtHomeSpread"), TextBox)
                Dim txtAwaySpreadMoneyUP = CType(e.Item.FindControl("txtAwaySpreadMoney"), TextBox)
                Dim txtHomeSpreadMoneyUP = CType(e.Item.FindControl("txtHomeSpreadMoney"), TextBox)
                Dim txtAwayTotalUP = CType(e.Item.FindControl("txtAwayTotal"), TextBox)
                Dim txtTotalPointsOverMoneyUP = CType(e.Item.FindControl("txtTotalPointsOverMoney"), TextBox)
                Dim txtTotalPointsUnderMoneyUP = CType(e.Item.FindControl("txtTotalPointsUnderMoney"), TextBox)
                Dim txtAwayMoneyLineUP = CType(e.Item.FindControl("txtAwayMoneyLine"), TextBox)
                Dim txtHomeMoneyLineUP = CType(e.Item.FindControl("txtHomeMoney"), TextBox)
                Dim txtAwayScore = CType(e.Item.FindControl("txtAwayScore"), TextBox)
                Dim txtHomeScore = CType(e.Item.FindControl("txtHomeScore"), TextBox)
                Dim txtDrawMoneyLineUP = CType(e.Item.FindControl("txtDrawMoney"), TextBox)
                Dim txtDrawnRotationNumberUP = CType(e.Item.FindControl("txtDrawnRotationNumber"), TextBox)
                Dim txtSecondHalfTimeUP = CType(e.Item.FindControl("txtSecondHalfTime"), SBCWebsite.Inc_DateTime)
                Dim oGameLine As New CGameLine()
                If String.IsNullOrEmpty(txtGameDateUP.Value) Then
                    ClientAlert("Please, input GameDate", True)
                    Return
                ElseIf String.IsNullOrEmpty(txtSecondHalfTimeUP.Value) Then
                    ClientAlert("Please, input SecondHalfTime", True)
                    Return
                ElseIf String.IsNullOrEmpty(txtAwayRotationNumberUP.Text) Then
                    ClientAlert("Please, input AwayRotationNumber", True)
                    Return
                ElseIf String.IsNullOrEmpty(txtHomeRotationNumberUP.Text) Then
                    ClientAlert("Please, input HomeRotationNumber", True)
                    Return
                ElseIf String.IsNullOrEmpty(txtAwayTeamUp.Text) Then
                    ClientAlert("Please, input AwayTeam", True)
                    Return
                ElseIf String.IsNullOrEmpty(txtHomeTeamUp.Text) Then
                    ClientAlert("Please, input HomeTeam", True)
                    Return
                ElseIf String.IsNullOrEmpty(txtSecondHalfTimeUP.Value) Then
                    ClientAlert("Please, input SecondHalfTime", True)
                    Return
                End If
                Dim oGameLineManager = New CGameLineManager()
                If Not txtAwayScore.Text.Equals("-1") AndAlso Not String.IsNullOrEmpty(txtHomeScore.Text) AndAlso Not String.IsNullOrEmpty(txtAwayScore.Text) Then
                    oGameLineManager.UpdateGame(hfGameID.Value, txtGameDateUP.Value, txtSecondHalfTimeUP.Value, txtAwayRotationNumberUP.Text, txtHomeRotationNumberUP.Text, txtDrawnRotationNumberUP.Text, txtAwayTeamUp.Text, txtHomeTeamUp.Text, ddlGameType.SelectedValue, txtAwayScore.Text, txtHomeScore.Text, ddlGameContext.SelectedValue)
                ElseIf txtAwayScore.Text.Equals("-1") Then
                    Dim oGame = New CGameManager()
                    oGame.UpdateGameStatus2H(hfGameID.Value)
                End If

                oGameLine.GameID = hfGameID.Value
                oGameLine.Context = ddlGameContext.SelectedValue
                oGameLine.FeedSource = SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER
                oGameLine.Bookmaker = "pinnacle2"
                oGameLine.HomeSpread = -SafeDouble(txtAwaySpreadUp.Text)
                oGameLine.HomeSpreadMoney = SafeDouble(txtHomeSpreadMoneyUP.Text)
                oGameLine.AwaySpread = SafeDouble(txtAwaySpreadUp.Text)
                oGameLine.AwaySpreadMoney = SafeDouble(txtAwaySpreadMoneyUP.Text)
                oGameLine.HomeMoneyLine = SafeDouble(txtHomeMoneyLineUP.Text)
                oGameLine.AwayMoneyLine = SafeDouble(txtAwayMoneyLineUP.Text)
                oGameLine.TotalPoints = SafeDouble(txtAwayTotalUP.Text)
                oGameLine.TotalPointsOverMoney = SafeDouble(txtTotalPointsOverMoneyUP.Text)
                oGameLine.TotalPointsUnderMoney = SafeDouble(txtTotalPointsUnderMoneyUP.Text)
                oGameLine.DrawMoneyLine = SafeDouble(txtDrawMoneyLineUP.Text)
                If oGameLineManager.SaveManualLine(oGameLine) Then
                    ClientAlert("Successfully Updated", True)
                    bindData()
                Else
                    ClientAlert("Unsuccessfully Updated", True)
                End If
            End If
        End Sub
  
        Protected Sub rptGameAdd_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptGameAdd.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim ddlGameContext = CType(e.Item.FindControl("ddlContext"), DropDownList)
                Dim Scontext As String = CType(e.Item.DataItem, DataRowView)("Context")
                ddlGameContext.SelectedValue = Scontext

                Dim txtAwayScore = CType(e.Item.FindControl("txtAwayScore"), TextBox)
                Dim txtHomeScore = CType(e.Item.FindControl("txtHomeScore"), TextBox)
                If Scontext.Equals("1H") AndAlso Not IsDBNull(CType(e.Item.DataItem, DataRowView)("AwayFirstHalfScore")) Then
                    txtAwayScore.Text = SafeInteger(CType(e.Item.DataItem, DataRowView)("AwayFirstHalfScore"))
                    txtHomeScore.Text = SafeInteger(CType(e.Item.DataItem, DataRowView)("HomeFirstHalfScore"))
                End If
                If Scontext.Equals("2H") AndAlso Not IsDBNull(CType(e.Item.DataItem, DataRowView)("AwaySecondQScore")) Then
                    txtAwayScore.Text = SafeInteger(CType(e.Item.DataItem, DataRowView)("AwaySecondQScore"))
                    txtHomeScore.Text = SafeInteger(CType(e.Item.DataItem, DataRowView)("HomeSecondQScore"))
                End If
                If Scontext.Equals("Current") AndAlso Not IsDBNull(CType(e.Item.DataItem, DataRowView)("AwayScore")) Then
                    txtAwayScore.Text = SafeInteger(CType(e.Item.DataItem, DataRowView)("AwayScore"))
                    txtHomeScore.Text = SafeInteger(CType(e.Item.DataItem, DataRowView)("HomeScore"))
                End If
            End If
        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            bindData()
        End Sub
    End Class
End Namespace
