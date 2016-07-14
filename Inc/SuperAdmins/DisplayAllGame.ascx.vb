Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports SBCBL

Namespace SBCSuperAdmin
    Partial Class DisplayAllGame
        Inherits SBCBL.UI.CSBCUserControl
        Private _sBookmaker As String = SBCBL.std.GetSiteType & " Bookmaker"
        Private _MainData As DataTable

        Private Sub bindGameType()
            Dim oGameType As New System.Collections.Generic.Dictionary(Of String, String)()

            '' Football
            oGameType("AFL Football") = "AFL Football"
            oGameType("CFL Football") = "CFL Football"
            oGameType("NCAA Football") = "NCAA Football"
            oGameType("NFL Football") = "NFL Football"
            oGameType("NFL Preseason") = "NFL Preseason"
            oGameType("UFL Football") = "UFL Football"

            '' Basketball
            oGameType("NBA Basketball") = "NBA Basketball"
            oGameType("NCAA Basketball") = "NCAA Basketball"
            oGameType("WNBA Basketball") = "WNBA Basketball"
            oGameType("WNCAA Basketball") = "WNCAA Basketball"

            Dim oGameTypes As Dictionary(Of String, String) = oGameType
            ddlGameType.DataSource = oGameTypes
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataBind()
            'ddlGameType.SelectedValue = "NCAA Football"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindGameType()
                'Bindgame()
            End If
        End Sub

        Private Sub Bindgame()

            Dim lstContext As New List(Of String)
            If String.IsNullOrEmpty(ddlContext.SelectedValue) Then
                lstContext.Add("Current")
                lstContext.Add("1H")
                lstContext.Add("2H")
            Else
                lstContext.Add(ddlContext.SelectedValue)
            End If
            rptContext.DataSource = lstContext
            rptContext.DataBind()
        End Sub

        Protected Function FormatValue(ByVal poValue As Object) As String
            If poValue.GetType() Is GetType(DBNull) Then
                Return ""
            Else
                Return "(" & SafeDouble(poValue).ToString() & ")"
            End If
        End Function

        Protected Function FormatPoint(ByVal poValue As Object) As String
            If poValue.GetType() Is GetType(DBNull) Then
                Return ""
            Else
                Return SafeDouble(poValue).ToString()
            End If
        End Function

        Private Function GetGame(ByVal psContext As String) As List(Of String)
            Dim oGamemanager As New CGameManager()
            '' Only get lines of 5Dimes, Pinnacle
            Dim lstBookmaker As New List(Of String)
            lstBookmaker.Add("5Dimes")
            lstBookmaker.Add("Pinnacle")
            _MainData = oGamemanager.GetAllGamesForSuper(ddlGameType.SelectedValue, lstBookmaker, psContext)

            If _MainData IsNot Nothing Then
                Return (From oRow As DataRow In _MainData.Rows _
                Select SafeString(oRow("GameID"))).Distinct.ToList()
            End If

            Return Nothing
        End Function

        Protected Sub rptContext_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptContext.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim sContext As String = CType(e.Item.DataItem, String)
                Dim rptGameLines As Repeater = e.Item.FindControl("rptGameLines")
                Dim oGames As List(Of String) = GetGame(sContext)
                Dim dvLine As HtmlControl = CType(e.Item.FindControl("dvLine"), HtmlControl)

                Select Case sContext
                    Case "Current"
                        dvLine.Style.Add("background-color", "#F9D188")
                    Case "1H"
                        dvLine.Style.Add("background-color", "#B4EEB4")
                    Case "2H"
                        dvLine.Style.Add("background-color", "#F9D188")
                    Case Else
                        dvLine.Style.Add("background-color", "#F9D188")
                End Select
                If oGames IsNot Nothing Then
                    rptGameLines.DataSource = oGames
                    rptGameLines.DataBind()
                End If
            End If
        End Sub

        Protected Sub rptGameLines_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim sGameID As String = CType(e.Item.DataItem, String)

                Dim lblGameInfo As Literal = CType(e.Item.FindControl("lblGameInfo"), Literal)
                Dim lblAwayNumber As Literal = CType(e.Item.FindControl("lblAwayNumber"), Literal)
                Dim lblAwayTeam As Literal = CType(e.Item.FindControl("lblAwayTeam"), Literal)
                Dim lblGameStatus As Literal = CType(e.Item.FindControl("lblGameStatus"), Literal)
                Dim lblHomeNumber As Literal = CType(e.Item.FindControl("lblHomeNumber"), Literal)
                Dim lblHomeTeam As Literal = CType(e.Item.FindControl("lblHomeTeam"), Literal)
                Dim lblHomeScore As Literal = CType(e.Item.FindControl("lblHomeScore"), Literal)
                Dim lblAwayScore As Literal = CType(e.Item.FindControl("lblAwayScore"), Literal)

                '' For 5Dimes
                Dim lblAwayTotalPoints As Literal = CType(e.Item.FindControl("lblAwayTotalPoints"), Literal)
                Dim lblTotalPointsOverMoney As Literal = CType(e.Item.FindControl("lblTotalPointsOverMoney"), Literal)
                Dim lblHomeTotalPoints As Literal = CType(e.Item.FindControl("lblHomeTotalPoints"), Literal)
                Dim lblTotalPointsUnderMoney As Literal = CType(e.Item.FindControl("lblTotalPointsUnderMoney"), Literal)
                Dim lblAwaySpread As Literal = CType(e.Item.FindControl("lblAwaySpread"), Literal)
                Dim lblAwaySpreadMoney As Literal = CType(e.Item.FindControl("lblAwaySpreadMoney"), Literal)
                Dim lblHomeSpread As Literal = CType(e.Item.FindControl("lblHomeSpread"), Literal)
                Dim lblHomeSpreadMoney As Literal = CType(e.Item.FindControl("lblHomeSpreadMoney"), Literal)

                '' For Pinnacle
                Dim lblAwayTotalPoints2 As Literal = CType(e.Item.FindControl("lblAwayTotalPoints2"), Literal)
                Dim lblTotalPointsOverMoney2 As Literal = CType(e.Item.FindControl("lblTotalPointsOverMoney2"), Literal)
                Dim lblHomeTotalPoints2 As Literal = CType(e.Item.FindControl("lblHomeTotalPoints2"), Literal)
                Dim lblTotalPointsUnderMoney2 As Literal = CType(e.Item.FindControl("lblTotalPointsUnderMoney2"), Literal)
                Dim lblAwaySpread2 As Literal = CType(e.Item.FindControl("lblAwaySpread2"), Literal)
                Dim lblAwaySpreadMoney2 As Literal = CType(e.Item.FindControl("lblAwaySpreadMoney2"), Literal)
                Dim lblHomeSpread2 As Literal = CType(e.Item.FindControl("lblHomeSpread2"), Literal)
                Dim lblHomeSpreadMoney2 As Literal = CType(e.Item.FindControl("lblHomeSpreadMoney2"), Literal)

                ' Get 5Dimes Line
                Dim oLine As DataRow = GetGameLine(sGameID, "5Dimes")
                If oLine IsNot Nothing Then
                    ' Set Common Game's Info
                    lblGameInfo.Text = SafeString(oLine("GameDate")) & "<br/>" & _
                    IIf(SafeBoolean(oLine("IsCircle")), "<span style='color:red'>* Game Circled</span>", "") & _
                    IIf(SafeBoolean(oLine("IsAddedGame")), "<span style='color:red'>* Added Game</span>", "") & _
                    IIf(SafeBoolean(oLine("IsWarn")), "<span style='color:red'>** " & SafeString(oLine("WarnReason")) & "</span>", "")

                    lblAwayNumber.Text = SafeString(oLine("AwayRotationNumber"))
                    lblAwayTeam.Text = SafeString(oLine("AwayTeam"))
                    lblGameStatus.Text = SafeString(oLine("GameStatus"))
                    lblHomeNumber.Text = SafeString(oLine("HomeRotationNumber"))
                    lblHomeTeam.Text = SafeString(oLine("HomeTeam"))

                    Select Case UCase(SafeString(oLine("Context")))
                        Case "CURRENT"
                            lblHomeScore.Text = SafeString(oLine("HomeScore"))
                            lblAwayScore.Text = SafeString(oLine("AwayScore"))

                        Case "1H"
                            lblHomeScore.Text = SafeString(oLine("HomeFirstHalfScore"))
                            lblAwayScore.Text = SafeString(oLine("AwayFirstHalfScore"))

                        Case "2H"
                            lblHomeScore.Text = SafeString(SafeDouble(oLine("HomeScore")) - SafeDouble(oLine("HomeFirstHalfScore")))
                            lblAwayScore.Text = SafeString(SafeDouble(oLine("AwayScore")) - SafeDouble(oLine("AwayFirstHalfScore")))

                        Case Else
                            lblHomeScore.Text = SafeString(oLine("HomeScore"))
                            lblAwayScore.Text = SafeString(oLine("AwayScore"))
                    End Select

                    '' Set 5Dimes Line
                    lblAwayTotalPoints.Text = FormatPoint(oLine("TotalPoints"))
                    lblTotalPointsOverMoney.Text = FormatValue(oLine("TotalPointsOverMoney"))
                    lblHomeTotalPoints.Text = FormatPoint(oLine("TotalPoints"))
                    lblTotalPointsUnderMoney.Text = FormatValue(oLine("TotalPointsUnderMoney"))
                    lblAwaySpread.Text = FormatPoint(oLine("AwaySpread"))
                    lblAwaySpreadMoney.Text = FormatValue(oLine("AwaySpreadMoney"))
                    lblHomeSpread.Text = FormatPoint(oLine("HomeSpread"))
                    lblHomeSpreadMoney.Text = FormatValue(oLine("HomeSpreadMoney"))
                End If

                If Not String.IsNullOrEmpty(lblAwayTotalPoints.Text) Then
                    lblAwayTotalPoints.Text = "O" & lblAwayTotalPoints.Text
                    lblHomeTotalPoints.Text = "U" & lblHomeTotalPoints.Text
                End If
                If String.IsNullOrEmpty(lblAwaySpread.Text) Then
                    If Not String.IsNullOrEmpty(lblAwaySpreadMoney.Text) Then
                        lblAwaySpread.Text = "PK"
                    End If
                End If
                If String.IsNullOrEmpty(lblHomeSpread.Text) Then
                    If Not String.IsNullOrEmpty(lblHomeSpreadMoney.Text) Then
                        lblHomeSpread.Text = "PK"
                    End If
                End If

                '' Get Pinnacle Line
                Dim oPinnacle As DataRow = GetGameLine(sGameID, "Pinnacle")
                If oPinnacle IsNot Nothing Then
                    lblAwayTotalPoints2.Text = FormatPoint(oPinnacle("TotalPoints"))
                    lblTotalPointsOverMoney2.Text = FormatValue(oPinnacle("TotalPointsOverMoney"))
                    lblHomeTotalPoints2.Text = FormatPoint(oPinnacle("TotalPoints"))
                    lblTotalPointsUnderMoney2.Text = FormatValue(oPinnacle("TotalPointsUnderMoney"))
                    lblAwaySpread2.Text = FormatPoint(oPinnacle("AwaySpread"))
                    lblAwaySpreadMoney2.Text = FormatValue(oPinnacle("AwaySpreadMoney"))
                    lblHomeSpread2.Text = FormatPoint(oPinnacle("HomeSpread"))
                    lblHomeSpreadMoney2.Text = FormatValue(oPinnacle("HomeSpreadMoney"))
                End If

                If Not String.IsNullOrEmpty(lblAwayTotalPoints2.Text) Then
                    lblAwayTotalPoints2.Text = "O" & lblAwayTotalPoints2.Text
                    lblHomeTotalPoints2.Text = "U" & lblHomeTotalPoints2.Text
                End If
                If String.IsNullOrEmpty(lblAwaySpread2.Text) Then
                    If Not String.IsNullOrEmpty(lblAwaySpreadMoney2.Text) Then
                        lblAwaySpread2.Text = "PK"
                    End If
                End If
                If String.IsNullOrEmpty(lblHomeSpread2.Text) Then
                    If Not String.IsNullOrEmpty(lblHomeSpreadMoney2.Text) Then
                        lblHomeSpread2.Text = "PK"
                    End If
                End If
            End If
        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            Bindgame()
        End Sub

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged
            Bindgame()
        End Sub

        Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
            Bindgame()
        End Sub

        Private Function GetGameLine(ByVal psGameID As String, ByVal psBookmaker As String) As DataRow
            If _MainData IsNot Nothing Then
                Dim oLines As DataRow() = _MainData.Select(String.Format("GameID={0} AND Bookmaker={1}", SQLString(psGameID), SQLString(psBookmaker)))

                If oLines IsNot Nothing AndAlso oLines.Count > 0 Then
                    Return oLines(0)
                End If
            End If

            Return Nothing
        End Function
    End Class
End Namespace
