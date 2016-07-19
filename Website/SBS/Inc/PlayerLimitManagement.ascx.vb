Imports WebsiteLibrary.DBUtils
Imports SBCBL.std
Imports System.Data
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Namespace SBCWebsite
    Partial Class JuiceManagement
        Inherits SBCBL.UI.CSBCUserControl

        Dim oPlayerLimitManager As New CPlayerLimitManager()
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Dim oGameTypes As New List(Of String)
        Dim oBetType As New System.Collections.Generic.Dictionary(Of String, String)()
        Dim oPlayerLimitSetting As New CPlayerTemplateLimitList
        Dim sCol As String = ""
        Dim oData As New DataTable
        Dim CurrRepeater As Repeater

#Region "Property"
        Dim GameTypes As Dictionary(Of String, String)
       
        Protected ReadOnly Property ListGameType() As List(Of String)
            Get
                Dim lstGameType As New List(Of String)
                lstGameType.Add("Football")
                'lstGameType.Add("Basketball")
                'lstGameType.Add("Hockey")
                'lstGameType.Add("Baseball")
                'lstGameType.Add("Soccer")
                Return lstGameType
            End Get
        End Property

        Protected Property BetType() As Dictionary(Of String, String)
            Set(ByVal value As Dictionary(Of String, String))
                oBetType = value
            End Set
            Get
                If oBetType.Count = 0 Then
                    oBetType("MaxSingle") = "Max Single"
                    oBetType("MaxParlay") = "Max Parlay"
                    oBetType("MaxTeaser") = "Max Teaser"
                    oBetType("MaxReverse") = "Max Reverse"
                End If
                Return oBetType
            End Get
        End Property

        Public Property GameType() As String
            Get
                Return CType(ViewState("GameType"), String)
            End Get
            Set(ByVal value As String)
                ViewState("GameType") = value
            End Set
        End Property

        Public Property SportType() As String
            Get
                If hfSportType.Value = "" Then
                    hfSportType.Value = "Football"
                End If
                Return hfSportType.Value
            End Get
            Set(ByVal value As String)
                hfSportType.Value = value
            End Set
        End Property

#End Region

#Region "Page Events"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        End Sub
#End Region

#Region "Load"
        Public Sub LoadControlForSportType()
            InvisibleControls()
            ' ClientAlert(SportType)
            CurrRepeater.Visible = True
            divMaxTeaser.Visible = SportType.Equals("Football") OrElse SportType.Equals("Basketball")

            ' tdTeamTotalPoint.Visible = SportType <> "Soccer"
        End Sub

        Public Sub InvisibleControls()
            rptPlayerLimitBaseball.Visible = False
            rptPlayerLimitBasketball.Visible = False
            rptPlayerLimitFootBall.Visible = False
            rptPlayerLimitHockey.Visible = False
            rptPlayerLimitSoccer.Visible = False
            rptPlayerLimitOther.Visible = False
            rptPlayerLimitGolf.Visible = False
            rptPlayerLimitTennis.Visible = False
            divSoccer.Visible = False
            divMaxTeaser.Visible = False
        End Sub

        Public Sub LoadPlayerSettingData(ByVal poPlayerTemplateID As String)
            hfPlayerTemplateID.Value = poPlayerTemplateID
            'If SportType = "Other" Then
            '    CType(Me.FindControl("PlayerLimitManager"), HtmlControl).Visible = False
            'Else
            LoadPlayerTemplateLimit()
            'End If
            LoadJuiceManagement()
        End Sub

        Public Sub LoadPlayerTemplateLimit()
            ' ClientAlert("rptPlayerLimit" & SportType)
            CurrRepeater = Me.FindControl("rptPlayerLimit" & SportType)
            LoadControlForSportType()
            BindGameTypes()
            BindPlayerLimit()
        End Sub

        Public Sub LoadJuiceManagement()
            Dim oJuiceManagementForSportType As CPlayerJuiceList = (New CPlayerJuiceManager).GetPlayerJuice(hfPlayerTemplateID.Value, SportType)
            BindPlayerJuice(oJuiceManagementForSportType)
        End Sub
#End Region

#Region "Get Data"
        Public Function GetListOfGame(ByVal psKey As String) As Dictionary(Of String, String)
            Dim odt As DataTable = oPlayerLimitManager.GetGameType(psKey)
            Dim oGameType As New Dictionary(Of String, String)
            For Each oDR As DataRow In odt.Rows
                If psKey = "Football" Then ''football
                    Select Case SafeString(oDR("Key"))
                        Case "NFL Preseason"
                            oGameType.Add(oDR("key"), "Pre-Season NFL")
                        Case Else
                            oGameType.Add(oDR("key"), SafeString(oDR("key")).Replace("Football", ""))
                    End Select
                ElseIf psKey <> "Soccer" Then
                    oGameType.Add(oDR("key"), SafeString(oDR("key")).Split(" ")(0))
                Else
                    oGameType.Add(oDR("key"), oDR("key"))
                End If
                oGameTypes.Add(oDR("key"))
            Next
          
            Return oGameType
        End Function

#End Region

#Region "bind data"
        Public Sub BindPlayerJuice(ByVal poJuiceList As CPlayerJuiceList)
            Try
                'Dim oJuice As CPlayerJuice = poJuiceList.Find(Function(x) x.JuiceType = "Favorite")
                'If oJuice IsNot Nothing Then
                '    hfJuice_FavID.Value = oJuice.PlayerJuiceManagementID
                '    txtFSpread.Text = oJuice.SpreadSetup
                '    txtF1HSpread.Text = oJuice._1stHSpreadSetup
                '    txtF2HSpread.Text = oJuice._2ndHSPreadSetup
                '    txtFTotal.Text = oJuice.TotalPointSetup
                '    txtF1HTotal.Text = oJuice._1stHTotalPointSetup
                '    txtF2HTotal.Text = oJuice._2ndHTotalPointSetup
                '    txtFMLine.Text = oJuice.MLineSetup
                '    txtF1HMLine.Text = oJuice._1stHMLineSetup
                '    txtF2HMLine.Text = oJuice._2ndMLineSetup
                '    txtFTeam.Text = oJuice.TeamTotalPointSetup
                'Else
                '    hfJuice_FavID.Value = ""
                '    txtFSpread.Text = "0"
                '    txtF1HSpread.Text = "0"
                '    txtF2HSpread.Text = "0"
                '    txtFTotal.Text = "0"
                '    txtF1HTotal.Text = "0"
                '    txtF2HTotal.Text = "0"
                '    txtFMLine.Text = "0"
                '    txtF1HMLine.Text = "0"
                '    txtF2HMLine.Text = "0"
                '    txtFTeam.Text = "0"
                'End If

                'oJuice = poJuiceList.Find(Function(x) x.JuiceType = "Underdog")
                'If oJuice IsNot Nothing Then
                '    hfJuice_UndID.Value = oJuice.PlayerJuiceManagementID
                '    txtUSpread.Text = oJuice.SpreadSetup
                '    txtU1HSpread.Text = oJuice._1stHSpreadSetup
                '    txtU2HSpread.Text = oJuice._2ndHSPreadSetup
                '    txtUTotal.Text = oJuice.TotalPointSetup
                '    txtU1HTotal.Text = oJuice._1stHTotalPointSetup
                '    txtU2HTotal.Text = oJuice._2ndHTotalPointSetup
                '    txtUMLine.Text = oJuice.MLineSetup
                '    txtU1HMLine.Text = oJuice._1stHMLineSetup
                '    txtU2HMLine.Text = oJuice._2ndMLineSetup
                '    txtUTeam.Text = oJuice.TeamTotalPointSetup
                'Else
                '    hfJuice_UndID.Value = ""
                '    txtUSpread.Text = "0"
                '    txtU1HSpread.Text = "0"
                '    txtU2HSpread.Text = "0"
                '    txtUTotal.Text = "0"
                '    txtU1HTotal.Text = "0"
                '    txtU2HTotal.Text = "0"
                '    txtUMLine.Text = "0"
                '    txtU1HMLine.Text = "0"
                '    txtU2HMLine.Text = "0"
                '    txtUTeam.Text = "0"
                'End If

            Catch ex As Exception
                LogError(log, "BindPlayerJuice ", ex)
            End Try
        End Sub

        Public Sub BindGameTypes()
            GameTypes = GetListOfGame(SportType)
            'If SportType.Equals("Other", StringComparison.CurrentCultureIgnoreCase) Then
            '    GameTypes("Golf") = "Golf"
            '    GameTypes("Tennis") = "Tennis"
            'End If
            'GameTypesFooball = GetListOfGame("Football")
            'GameTypesBasketball = GetListOfGame("Basketball")
            'GameTypesHockey = GetListOfGame("Hockey")
            'GameTypesBaseball = GetListOfGame("Baseball")
            'GameTypesSoccer = GetListOfGame("Soccer")
        End Sub

        Public Sub BindPlayerLimit()
            oData = oPlayerLimitManager.GetDataTable(hfPlayerTemplateID.Value, oGameTypes)
            CurrRepeater.DataSource = GameTypes
            CurrRepeater.DataBind()
            'lblrptSoccerID.Text = CurrRepeater.ClientID
        End Sub

        Public Function CheckExistColumn(ByVal sColName As String) As Boolean
            If sCol.Contains(sColName) Then Return True
            Return False
        End Function

        Public Function bindColumn(ByVal sColName As String, ByVal container As System.Data.DataRowView) As String
            If CheckExistColumn(sColName) = True Then
                Return container.Item(sColName)
            Else
                Return 0
            End If
        End Function

        Public Function mapColumnName(ByVal i As Integer) As String
            Select Case i
                Case 0
                    Return "MaxSingle"
                Case 1
                    Return "MaxParlay"
                Case 2
                    Return "MaxReverse"
                Case Else
                    Return "MaxTeaser"
            End Select
        End Function

        Public Function GetTemplateLimitDataOfGameType(ByVal sGameType As String) As DataTable
            sCol = ""
            Dim oRows = From row In oData Where row("GameType") = sGameType Order By row("Context")
            Dim odtTmp As New DataTable
            odtTmp.Columns.Add("Limit")
            For Each row As DataRow In oRows
                If Not odtTmp.Columns.Contains(SafeString(row("Context"))) Then
                    odtTmp.Columns.Add(SafeString(row("Context")))
                    sCol += SafeString(row("Context")) + ","
                End If

            Next
            For i As Integer = 0 To 3
                Dim r As DataRow = odtTmp.NewRow
                For z As Integer = 1 To odtTmp.Columns.Count - 1 'index 0 : col "Name"
                    'r(odtTmp.Columns(z).ColumnName) = SafeRound(oRows(z - 1)(i))
                    r(z) = SafeRound(oRows(z - 1)(mapColumnName(i)))
                Next
                Select Case i
                    Case 0
                        r("Limit") = "Max Single"
                    Case 1
                        r("Limit") = "Max Parlay"
                    Case 2
                        r("Limit") = "Max Reverse"
                    Case 3
                        r("Limit") = "Max Teaser"
                End Select
                odtTmp.Rows.Add(r)
            Next
            If SportType = "Other" Or SportType = "Golf" Or SportType = "Tennis" Or SportType = "Baseball" Or SportType = "Hockey" Or SportType = "Soccer" Then
                odtTmp.Rows(3).Delete()
            End If

            Return odtTmp
        End Function

        Public Sub SetDefaultLimitValues(ByVal poDefaultValues As Dictionary(Of String, String))
            CurrRepeater = Me.FindControl("rptPlayerLimit" & SportType)
            If SportType = "Soccer" Then
                divSoccer.Visible = True
            End If
            Dim row As Integer = 2
            If SportType = "Football" Or SportType = "Basketball" Then
                row = 3
            End If

            For Each item As RepeaterItem In CurrRepeater.Items
                Dim grLimitSetting As DataGrid = CType(item.FindControl("grPlayerLimit" & SportType), DataGrid)
                For iCol As Integer = 1 To grLimitSetting.Columns.Count - 1
                    With grLimitSetting.Columns(iCol)
                        If .HeaderText = "Full Game" Or .HeaderText = "Money Line" Or .HeaderText = "Total Points" Or _
                            .HeaderText = "Run Line" Or .HeaderText = "Puck Line" Then

                            For i As Integer = 0 To row
                                Dim lbl As Label = CType(grLimitSetting.Items(i).FindControl("lblLimit"), Label)
                                CType(grLimitSetting.Items(i).FindControl("txt" + .HeaderText.Replace(" ", "")), TextBox).Text = poDefaultValues.Item(lbl.Text)
                            Next
                        Else
                            For i As Integer = 0 To row
                                CType(grLimitSetting.Items(i).FindControl("txt" + .HeaderText), TextBox).Text = poDefaultValues.Item(.HeaderText)
                            Next
                        End If
                    End With
                Next
            Next
        End Sub

#End Region

#Region "controls event"
        Protected Sub rptPlayerLimitFootBall2_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)

            'Dim data As DataRowView = CType(e.Item.DataItem, DataRowView)
            'If SafeString(data("Limit")).Equals("Max Teaser") Then
            '    Dim txtFullGame As TextBox = CType(e.Item.FindControl("txtFullGame"), TextBox)
            '    Dim txtMoneyLine As TextBox = CType(e.Item.FindControl("txtMoneyLine"), TextBox)
            '    Dim txtTotalPoints As TextBox = CType(e.Item.FindControl("txtTotalPoints"), TextBox)
            '    ' txtFullGame.CssClass = "textInput FormatTextRight Teaser"
            '    'txtMoneyLine.CssClass = "textInput FormatTextRight Teaser"
            '    ' txtTotalPoints.CssClass = "textInput FormatTextRight Teaser"
            'End If

        End Sub
        Protected Sub rptPlayerLimitFootBall_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitFootBall As DataGrid = CType(e.Item.FindControl("grPlayerLimitFootBall"), DataGrid)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitFootBall.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            grPlayerLimitFootBall.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitFootBall.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitFootBall)

        End Sub

        Protected Sub rptPlayerLimitBasketball_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitBasketball As DataGrid = CType(e.Item.FindControl("grPlayerLimitBasketball"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitBasketball.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitBasketball.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitBasketball.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitBasketball)
        End Sub

        Protected Sub rptPlayerLimitHockey_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitHockey As DataGrid = CType(e.Item.FindControl("grPlayerLimitHockey"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitHockey.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitHockey.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitHockey.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitHockey)
        End Sub

        Protected Sub rptPlayerLimitTennis_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitTennis As DataGrid = CType(e.Item.FindControl("grPlayerLimitTennis"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitTennis.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitTennis.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitTennis.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitTennis)
        End Sub
        Protected Sub rptPlayerLimitOther_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitTennis As DataGrid = CType(e.Item.FindControl("grPlayerLimitOther"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitTennis.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitTennis.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitTennis.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitTennis)
        End Sub

        Protected Sub rptPlayerLimitGolf_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitTennis As DataGrid = CType(e.Item.FindControl("grPlayerLimitGolf"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitTennis.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitTennis.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitTennis.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitTennis)
        End Sub
        Protected Sub rptPlayerLimitBaseball_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitBaseball As DataGrid = CType(e.Item.FindControl("grPlayerLimitBaseball"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            lblrptFootBallID.Text = grPlayerLimitBaseball.ClientID.Trim()
            lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitBaseball.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitBaseball.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitBaseball)
        End Sub

        Protected Sub rptPlayerLimitSoccer_DataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim grPlayerLimitSoccer As DataGrid = CType(e.Item.FindControl("grPlayerLimitSoccer"), DataGrid)
            Dim hfGameType As HiddenField = CType(e.Item.FindControl("hfGameType"), HiddenField)
            'Dim lblrptFootBallID As Literal = CType(e.Item.FindControl("lblrptFootBallID"), Literal)
            'Dim lbltxtFootBallID As Literal = CType(e.Item.FindControl("lbltxtFootBallID"), Literal)
            'Dim txtGameTypeLimit As TextBox = CType(e.Item.FindControl("txtGameTypeLimit"), TextBox)
            'lblrptFootBallID.Text = grPlayerLimitSoccer.ClientID.Trim()
            'lbltxtFootBallID.Text = txtGameTypeLimit.ClientID.Trim()
            grPlayerLimitSoccer.DataSource = GetTemplateLimitDataOfGameType(hfGameType.Value)
            grPlayerLimitSoccer.DataBind()
            DisableTextBox(hfGameType.Value, grPlayerLimitSoccer, "Soccer")
        End Sub

        Public Sub DisableTextBox(ByVal sGameType As String, ByVal grdData As DataGrid, Optional ByVal sSportType As String = "")
            Dim sDisableField1() As String = {"1Q", "2Q", "3Q", "4Q", "1H", "2H"}
            Dim sDisableField2() As String = {"1H", "2H"}
            Dim sDisableField3() As String = {"1H", "2H", IIf(sGameType.Contains("Baseball"), "RunLine", "PuckLine"), "MoneyLine", "TotalPoints"}
            Dim sDisableField4() As String = {"1H", "2H", "MoneyLine", "TotalPoints", "FullGame"}
            Select Case sGameType.Contains("Football") Or sGameType.Contains("Basketball") Or sGameType.Contains("NFL Preseason")
                Case True
                    For i As Integer = 1 To 3
                        For Each s As String In sDisableField1
                            If grdData.Items.Count < i Then
                                Dim textbox As TextBox = CType(grdData.Items(i).FindControl("txt" + s), TextBox)
                                If textbox IsNot Nothing Then
                                    textbox.Visible = False
                                    If (i = 1 And (s = "1H" Or s = "2H")) Or (i = 2 And (s = "1H" Or s = "2H")) Then
                                        textbox.Visible = True
                                    End If
                                End If
                            End If
                        Next
                    Next
                Case Else
                    If sGameType.Contains("Baseball") Or sGameType.Contains("Hockey") Then
                        'For Each s As String In sDisableField3
                        '    Dim textbox As TextBox = CType(grdData.Items(2).FindControl("txt" + s), TextBox)
                        '    textbox.Visible = False
                        'Next
                        For i As Integer = 1 To 2
                            Dim textbox As TextBox = CType(grdData.Items(i).FindControl("txt" + IIf(sGameType.Contains("Baseball"), "RunLine", "PuckLine")), TextBox)
                            textbox.Visible = False
                        Next
                    End If
                    For Each s As String In sDisableField2
                        Dim textbox As TextBox = CType(grdData.Items(2).FindControl("txt" + s), TextBox)
                        textbox.Visible = False
                    Next
            End Select
            'If SportType = "Soccer" Then
            '    For Each s As String In sDisableField4
            '        Dim textbox As TextBox = CType(grdData.Items(2).FindControl("txt" + s), TextBox)
            '        textbox.Visible = False
            '    Next
            'End If
        End Sub

#End Region

#Region "Save"
        Dim sContext() As String = {"Max Single", "Max Parlay", "Max Teaser", "Max Reverse"}
        Dim sColumn() As String = {"1Q", "2Q", "3Q", "4Q", "1H", "2H", "Full Game"}

        Public Function SaveCopy(ByVal psPlayerTemplateID As String) As Boolean
            hfPlayerTemplateID.Value = psPlayerTemplateID
            Return Save()
        End Function

        Public Function Save() As Boolean
            Dim oSuccess As Boolean = False
            Try
                CurrRepeater = Me.FindControl("rptPlayerLimit" & SportType)
                BindGameTypes()
                oSuccess = SaveLimitTemplate() And SavePlayerJuice()
            Catch ex As Exception
                LogError(log, "Player limits save error", ex)
            End Try
            Return oSuccess
        End Function

        Public Function SavePlayerJuice() As Boolean
            Dim bSuccess As Boolean = False
            Try
                ''Favorite
                'Dim oJuice As New CPlayerJuice
                'oJuice.SpreadSetup = SafeDouble(txtFSpread.Text)
                'oJuice._1stHSpreadSetup = SafeDouble(txtF1HSpread.Text)
                'oJuice._2ndHSPreadSetup = SafeDouble(txtF2HSpread.Text)
                'oJuice.TotalPointSetup = SafeDouble(txtFTotal.Text)
                'oJuice._1stHTotalPointSetup = SafeDouble(txtF1HTotal.Text)
                'oJuice._2ndHTotalPointSetup = SafeDouble(txtF2HTotal.Text)
                'oJuice.MLineSetup = SafeDouble(txtFMLine.Text)
                'oJuice._1stHMLineSetup = SafeDouble(txtF1HMLine.Text)
                'oJuice._2ndMLineSetup = SafeDouble(txtF2HMLine.Text)
                'oJuice.TeamTotalPointSetup = SafeDouble(txtFTeam.Text)
                'oJuice.JuiceType = "Favorite"
                'oJuice.SportType = SportType
                'oJuice.playerTemplateID = hfPlayerTemplateID.Value
                'If hfJuice_FavID.Value <> "" Then
                '    bSuccess = (New CPlayerJuiceManager()).UpdatePlayerJuiceSetup(hfJuice_FavID.Value, oJuice)
                'Else
                '    bSuccess = (New CPlayerJuiceManager()).InsertPlayerJuiceSetup(oJuice)
                'End If
                ' ''Underdog
                'oJuice = New CPlayerJuice
                'oJuice.SpreadSetup = SafeDouble(txtUSpread.Text)
                'oJuice._1stHSpreadSetup = SafeDouble(txtU1HSpread.Text)
                'oJuice._2ndHSPreadSetup = SafeDouble(txtU2HSpread.Text)
                'oJuice.TotalPointSetup = SafeDouble(txtUTotal.Text)
                'oJuice._1stHTotalPointSetup = SafeDouble(txtU1HTotal.Text)
                'oJuice._2ndHTotalPointSetup = SafeDouble(txtU2HTotal.Text)
                'oJuice.MLineSetup = SafeDouble(txtUMLine.Text)
                'oJuice._1stHMLineSetup = SafeDouble(txtU1HMLine.Text)
                'oJuice._2ndMLineSetup = SafeDouble(txtU2HMLine.Text)
                'oJuice.TeamTotalPointSetup = SafeDouble(txtUTeam.Text)
                'oJuice.JuiceType = "Underdog"
                'oJuice.SportType = SportType
                'oJuice.playerTemplateID = hfPlayerTemplateID.Value
                'If hfJuice_UndID.Value <> "" Then
                '    bSuccess = (New CPlayerJuiceManager()).UpdatePlayerJuiceSetup(hfJuice_UndID.Value, oJuice)
                'Else
                '    bSuccess = (New CPlayerJuiceManager()).InsertPlayerJuiceSetup(oJuice)
                'End If

            Catch ex As Exception
                LogError(log, "SavePlayerJuice", ex)
                Return False
            End Try
            Return bSuccess
        End Function

        Public Function SaveLimitTemplate() As Boolean
            Try
                Dim odtSaveTable As DataTable = CreateSaveDataTable()
                CreateSaveData(odtSaveTable, hfPlayerTemplateID.Value, CurrRepeater)
                Dim list As New List(Of String)(GameTypes.Keys)
                Return oPlayerLimitManager.SaveData(odtSaveTable, hfPlayerTemplateID.Value, "'" & Join(list.ToArray, "','"))
            Catch ex As Exception
                LogError(log, "Save Limit Template", ex)
            End Try
            Return False
        End Function

        Public Function CreateSaveDataTable() As DataTable
            Dim odt As New DataTable
            odt.Columns.Add("PlayerTemplateLimitID")
            odt.Columns.Add("PlayerTemplateID")
            odt.Columns.Add("GameType")
            odt.Columns.Add("Context")
            odt.Columns.Add("MaxSingle")
            odt.Columns.Add("MaxParlay")
            odt.Columns.Add("MaxTeaser")
            odt.Columns.Add("MaxReverse")
            Return odt
        End Function

        Public Function mapHeaderToContext(ByVal sHeader As String) As String
            Select Case sHeader
                Case "Full Game", "Puck Line", "Run Line"
                    Return "Current"
                Case "Money Line", "Total Points"
                    Return sHeader.Replace(" ", "")
                Case Else
                    Return sHeader
            End Select
        End Function

        Public Function CreateSaveData(ByRef odtLimitSettingData As DataTable, ByVal poPlayerTemplateID As String, ByVal poRepeaterGame As Repeater) As Boolean
            Try
                Dim row As Integer = 2
                If SportType = "Football" Or SportType = "Basketball" Then
                    row = 3
                End If
                For Each item As RepeaterItem In poRepeaterGame.Items
                    Dim oGameType As HiddenField = CType(item.FindControl("hfGameType"), HiddenField)
                    Dim grLimitSetting As DataGrid = CType(item.FindControl("grPlayerLimit" & SportType), DataGrid)
                    For iCol As Integer = 1 To grLimitSetting.Columns.Count - 1
                        Dim oContext As String = grLimitSetting.Columns(iCol).HeaderText
                        Dim oRow As DataRow = odtLimitSettingData.NewRow
                        oRow("PlayerTemplateLimitID") = newGUID()
                        oRow("PlayerTemplateID") = poPlayerTemplateID
                        oRow("GameType") = oGameType.Value
                        oRow("Context") = mapHeaderToContext(oContext)
                        oRow("MaxSingle") = CType(grLimitSetting.Items(0).FindControl("txt" + oContext.Replace(" ", "")), TextBox).Text
                        oRow("MaxParlay") = CType(grLimitSetting.Items(1).FindControl("txt" + oContext.Replace(" ", "")), TextBox).Text
                        If SportType = "Football" Or SportType = "Basketball" Then
                            oRow("MaxTeaser") = CType(grLimitSetting.Items(row).FindControl("txt" + oContext.Replace(" ", "")), TextBox).Text
                        Else
                            oRow("MaxTeaser") = 0
                        End If
                        oRow("MaxReverse") = CType(grLimitSetting.Items(2).FindControl("txt" + oContext.Replace(" ", "")), TextBox).Text

                        odtLimitSettingData.Rows.Add(oRow)
                    Next
                Next
            Catch ex As Exception
                LogError(log, "Save Data", ex)
                Return False
            End Try
            Return True
        End Function

        Public Function SavePlayerSettingData() As Boolean

        End Function
#End Region
    End Class

End Namespace
