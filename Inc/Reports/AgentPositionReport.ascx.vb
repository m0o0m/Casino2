Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.Managers
Imports System.Collections
Imports System.Data

Namespace SBCWebsite
    Partial Class AgentPositionReport
        Inherits SBCBL.UI.CSBCUserControl
        Private nDiffAmountInit As Double = -1
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"

        Private _sSpread As String = "Spread"
        Private _sAwaySpreadMoney As String = "AwaySpreadMoney"
        Private _sHomeSpreadMoney As String = "HomeSpreadMoney"

        Private _sTotalPoints As String = "TotalPoints"
        Private _sTotalPointsOverMoney As String = "TotalPointsOverMoney"
        Private _sTotalPointsUnderMoney As String = "TotalPointsUnderMoney"


        Private _sMoneyLine As String = "MoneyLine"
        Private _sAwayMoneyLine As String = "AwayMoneyLine"
        Private _sHomeMoneyLine As String = "HomeMoneyLine"

        Private _sDraw As String = "Draw"
        Private _sDrawMoneyLine As String = "DrawMoneyLine"

        Private _odtTicketManager As DataTable

        Private _sBookmakerType As String = SBCBL.std.GetSiteType & " BookmakerType"

        Private ReadOnly Property GameContext() As DataTable
            Get
                Dim sarGameType As String() = New String() {"Current", "1H", "2H"}
                Dim odtGameType As DataTable = New DataTable()
                odtGameType.Columns.Add("GameContext")
                For Each sGameType As String In sarGameType
                    Dim odrGameType As DataRow = odtGameType.NewRow()
                    odrGameType("GameContext") = sGameType
                    odtGameType.Rows.Add(odrGameType)
                Next
                Return odtGameType
            End Get
        End Property

        Protected Property SelectedGameType() As String
            Get
                If ViewState("SelectedGameType") Is Nothing Then
                    ViewState("SelectedGameType") = ""
                End If
                Return CType(ViewState("SelectedGameType"), String)
            End Get
            Set(ByVal value As String)
                ViewState("SelectedGameType") = value
            End Set
        End Property

        Protected Property NumGameRow() As Integer
            Get
                If ViewState("NumGameRow") Is Nothing Then
                    ViewState("NumGameRow") = ""
                End If
                Return SafeDouble(ViewState("NumGameRow"))
            End Get
            Set(ByVal value As Integer)
                ViewState("NumGameRow") = value
            End Set
        End Property

        Protected Property DiffAmount() As Double
            Get
                If ViewState("DiffAmount") Is Nothing Then
                    ViewState("DiffAmount") = ""
                End If
                Return SafeDouble(ViewState("DiffAmount"))
            End Get
            Set(ByVal value As Double)
                ViewState("DiffAmount") = value
            End Set
        End Property

        Protected Property TotalSpread() As Double
            Get
                If ViewState("TotalSpread") Is Nothing Then
                    ViewState("TotalSpread") = ""
                End If
                Return SafeDouble(ViewState("TotalSpread"))
            End Get
            Set(ByVal value As Double)
                ViewState("TotalSpread") = value
            End Set
        End Property

        Protected Property TotalTotal() As Double
            Get
                If ViewState("TotalTotal") Is Nothing Then
                    ViewState("TotalTotal") = ""
                End If
                Return SafeDouble(ViewState("TotalTotal"))
            End Get
            Set(ByVal value As Double)
                ViewState("TotalTotal") = value
            End Set
        End Property

        Protected Property TotalMLine() As Double
            Get
                If ViewState("TotalMLine") Is Nothing Then
                    ViewState("TotalMLine") = ""
                End If
                Return SafeDouble(ViewState("TotalMLine"))
            End Get
            Set(ByVal value As Double)
                ViewState("TotalMLine") = value
            End Set
        End Property

        Private ReadOnly Property SuperID() As String
            Get
                If UserSession.SuperAdminInfo.IsPartner Then
                    Return UserSession.SuperAdminInfo.PartnerOf
                Else
                    Return UserSession.UserID
                End If

            End Get
        End Property

#Region "Bind Data For Agent"
        Private Sub bindAgents()
            Dim oAgentManager As New CAgentManager()
            If UserSession.UserID <> "" Then
                ddlAgents.DataSource = oAgentManager.GetAgentsBySuperID(SuperID, Nothing)
            End If
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub
#End Region


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindGameTypes()
                'ddlContext.DataSource = GameContext
                'ddlContext.DataTextField = "GameContext"
                'ddlContext.DataValueField = "GameContext"
                'ddlContext.DataBind()
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    bindAgents()
                    agentDrop.Visible = True
                End If
            End If
        End Sub

#Region "Game Types"

        Private Sub bindGameTypes()
            Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                                      Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" _
                                                      Order By oSetting.ItemIndex, oSetting.Key _
                                                      Select oSetting).ToList

            Dim olstSportType As New List(Of CSysSetting)

            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Football", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next
            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Basketball", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next

            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Baseball", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next
            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Hockey", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next

            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next

            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Tennis", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next

            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Golf", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If

            Next
            For Each osys As CSysSetting In oGameTypes
                If osys.Key.Equals("Other", StringComparison.CurrentCultureIgnoreCase) Then
                    olstSportType.Add(osys)
                End If
            Next
            rptGameType.DataSource = olstSportType 'oGameTypes
            rptGameType.DataBind()

            '' Disable GameTypes don't have Lines
            Dim oTblGameTypes As DataTable = (New SBCBL.Managers.CGameManager()).GetAvailableGameTypes(GetEasternDate())
            If oTblGameTypes IsNot Nothing Then
                For Each oItems As RepeaterItem In rptGameType.Items
                    If oItems.ItemType = ListItemType.AlternatingItem OrElse oItems.ItemType = ListItemType.Item Then
                        Dim rptSubGameType As Repeater = CType(oItems.FindControl("rptSubGameType"), Repeater)

                        For Each oSubItem As RepeaterItem In rptSubGameType.Items
                            If oSubItem.ItemType = ListItemType.Item OrElse oSubItem.ItemType = ListItemType.AlternatingItem Then
                                'Dim lbtGameType As Li = CType(oSubItem.FindControl("chkGameType"), CheckBox)
                                Dim olbt As LinkButton = CType(oSubItem.FindControl("lbtGameType"), LinkButton)
                                Dim sGameType = olbt.Text
                                Dim sBookMaker As String = UserSession.SysSettings(_sBookmakerType).GetValue(sGameType)
                                '' no filter by bookmaker any more, just validate if gametype has primary bookmaker
                                Dim sWhere As String = String.Format("GameType={0}", SQLString(sGameType))

                                If sBookMaker = "" OrElse SafeInteger(oTblGameTypes.Compute("SUM(Num)", sWhere)) = 0 Then
                                    olbt.Visible = False
                                    CType(oSubItem.FindControl("tdSub"), HtmlTableCell).Visible = False
                                    'olbt.Enabled = False

                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End Sub

        Protected Sub rptGameType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptGameType.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim rptSubGameType As Repeater = CType(e.Item.FindControl("rptSubGameType"), Repeater)
                Dim oSys As CSysSetting = CType(e.Item.DataItem, CSysSetting)
                Dim oListSubSys As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                                      Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = oSys.SysSettingID _
                                                      Order By oSetting.Key _
                                                      Select oSetting).ToList()


                rptSubGameType.DataSource = oListSubSys
                rptSubGameType.DataBind()
            End If
        End Sub

        'reset all memu color to blue when click anoher menu
        Private Sub resetColorSubMenu()
            For i As Integer = 0 To rptGameType.Items.Count - 1
                Dim rptSubGameType As Repeater = CType(rptGameType.Items(i).FindControl("rptSubGameType"), Repeater)
                For k As Integer = 0 To rptSubGameType.Items.Count - 1
                    Dim lbtGameType As LinkButton = CType(rptSubGameType.Items(k).FindControl("lbtGameType"), LinkButton)
                    lbtGameType.ForeColor = Drawing.Color.Blue
                Next
            Next

        End Sub

        'set color in menu selected when bind data for menu repeater
        Private Sub setColorSubMenu()
            For i As Integer = 0 To rptGameType.Items.Count - 1
                Dim rptSubGameType As Repeater = CType(rptGameType.Items(i).FindControl("rptSubGameType"), Repeater)
                For k As Integer = 0 To rptSubGameType.Items.Count - 1
                    Dim lbtGameType As LinkButton = CType(rptSubGameType.Items(k).FindControl("lbtGameType"), LinkButton)
                    If lbtGameType.Text.Equals(SelectedGameType) Then
                        lbtGameType.ForeColor = Drawing.Color.DarkOrange
                    End If

                Next
            Next

        End Sub
#End Region

        Private Sub ShowQuarterGame(ByVal psGameType As String)
            Dim oneQuarter As New ListItem()
            oneQuarter.Text = " <span style='position:relative;top:-3px'> 1Q</span> "
            oneQuarter.Value = "1Q"
            Dim SecondQuarter As New ListItem()
            SecondQuarter.Text = "<span style='position:relative;top:-3px'>  2Q </span>"
            SecondQuarter.Value = "2Q"
            Dim thirdQuarter As New ListItem()
            thirdQuarter.Text = "<span style='position:relative;top:-3px'>  3Q </span>"
            thirdQuarter.Value = "3Q"
            Dim fourQuarter As New ListItem()
            fourQuarter.Text = "<span style='position:relative;top:-3px'>  4Q </span>"
            fourQuarter.Value = "4Q"
            rblContext.Items.Remove(oneQuarter)
            rblContext.Items.Remove(SecondQuarter)
            rblContext.Items.Remove(thirdQuarter)
            rblContext.Items.Remove(fourQuarter)
            If IsFootball(psGameType) OrElse IsBasketball(psGameType) Then
                rblContext.Items.Add(oneQuarter)
                rblContext.Items.Add(SecondQuarter)
                rblContext.Items.Add(thirdQuarter)
                rblContext.Items.Add(fourQuarter)
            End If

        End Sub

        Protected Sub lbtGameType_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim sGameType As String = CType(sender, LinkButton).Text
            ShowQuarterGame(sGameType)
            resetColorSubMenu()
            CType(sender, LinkButton).ForeColor = Drawing.Color.DarkOrange
            SelectedGameType = sGameType
            rblContext.SelectedValue = "Current"
            LoadGames(SelectedGameType, rblContext.SelectedValue)
        End Sub

        Protected Sub rptGameLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                e.Item.FindControl("lblAwayPitcher").Visible = SafeString(e.Item.DataItem("AwayPitcher")) <> ""
                CType(e.Item.FindControl("lblAwayPitcher"), Literal).Text = "<br/>" & e.Item.DataItem("AwayPitcher") & " - "
                e.Item.FindControl("lblAwayPitcherRightHand").Visible = SafeString(e.Item.DataItem("AwayPitcher")) <> ""
                CType(e.Item.FindControl("lblAwayPitcherRightHand"), Literal).Text = SafeString(IIf(SBCBL.std.SafeString(e.Item.DataItem("AwayPitcherRightHand")) = "Y", "R", "L"))

                e.Item.FindControl("lblHomePitcher").Visible = SafeString(e.Item.DataItem("HomePitcher")) <> ""
                CType(e.Item.FindControl("lblHomePitcher"), Literal).Text = "<br/>" & e.Item.DataItem("HomePitcher") & " - "
                e.Item.FindControl("lblHomePitcherRightHand").Visible = SafeString(e.Item.DataItem("HomePitcher")) <> ""
                CType(e.Item.FindControl("lblHomePitcherRightHand"), Literal).Text = SafeString(IIf(SBCBL.std.SafeString(e.Item.DataItem("HomePitcherRightHand")) = "Y", "R", "L"))

                Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)
                Dim sGameID As String = SafeString(oData("GameID"))

                Dim lblAwaySpread, lblAwayTotal, lblAwayMoneyLine, lblHomeSpread, lblHomeTotal, lblHomeMoney, lblDrawMoney, lblDiffSpread, lblDiffTotal, lblDiffMLine As Label
                lblAwaySpread = CType(e.Item.FindControl("lblAwaySpread"), Label)
                lblAwayTotal = CType(e.Item.FindControl("lblAwayTotal"), Label)
                lblAwayMoneyLine = CType(e.Item.FindControl("lblAwayMoneyLine"), Label)
                lblHomeSpread = CType(e.Item.FindControl("lblHomeSpread"), Label)
                lblHomeTotal = CType(e.Item.FindControl("lblHomeTotal"), Label)
                lblHomeMoney = CType(e.Item.FindControl("lblHomeMoney"), Label)
                lblDrawMoney = CType(e.Item.FindControl("lblDrawMoney"), Label)
                lblDiffSpread = CType(e.Item.FindControl("lblDiffSpread"), Label)
                lblDiffTotal = CType(e.Item.FindControl("lblDiffTotal"), Label)
                lblDiffMLine = CType(e.Item.FindControl("lblDiffMLine"), Label)

                DiffAmount = nDiffAmountInit

                lblAwaySpread.Text = CalcPosition(sGameID, _sAwaySpreadMoney, _sSpread, _odtTicketManager)
                lblHomeSpread.Text = CalcPosition(sGameID, _sHomeSpreadMoney, _sSpread, _odtTicketManager)
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    lblDiffSpread.Text = String.Format("<a style='color:blue' href='/SBS/SuperAdmins/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=spread&AgentID={3}'>{2}</a>", sGameID, rblContext.SelectedValue, Math.Abs(DiffAmount), ddlAgents.SelectedValue)
                Else
                    lblDiffSpread.Text = String.Format("<a style='color:blue' href='/SBS/Agents/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=spread'>{2}</a>", sGameID, rblContext.SelectedValue, Math.Abs(DiffAmount))
                End If
                TotalSpread += Math.Abs(DiffAmount)
                DiffAmount = nDiffAmountInit

                lblAwayTotal.Text = CalcPosition(sGameID, _sTotalPointsOverMoney, _sTotalPoints, _odtTicketManager)
                lblHomeTotal.Text = CalcPosition(sGameID, _sTotalPointsUnderMoney, _sTotalPoints, _odtTicketManager)
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    lblDiffTotal.Text = String.Format("<a style='color:blue' href='/SBS/SuperAdmins/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=TotalPoints&AgentID={3}'>{2}</a>", sGameID, rblContext.SelectedValue, Math.Abs(DiffAmount), ddlAgents.SelectedValue)
                Else
                    lblDiffTotal.Text = String.Format("<a style='color:blue' href='/SBS/Agents/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=TotalPoints'>{2}</a>", sGameID, rblContext.SelectedValue, Math.Abs(DiffAmount))
                End If
                TotalTotal += Math.Abs(DiffAmount)
                DiffAmount = nDiffAmountInit

                lblAwayMoneyLine.Text = CalcPosition(sGameID, _sAwayMoneyLine, _sMoneyLine, _odtTicketManager)
                lblHomeMoney.Text = CalcPosition(sGameID, _sHomeMoneyLine, _sMoneyLine, _odtTicketManager)
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    lblDiffMLine.Text = String.Format("<a style='color:blue' href='/SBS/SuperAdmins/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=MoneyLine&AgentID={3}'>{2}</a>", sGameID, rblContext.SelectedValue, Math.Abs(DiffAmount), ddlAgents.SelectedValue)
                Else
                    lblDiffMLine.Text = String.Format("<a style='color:blue' href='/SBS/Agents/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=MoneyLine'>{2}</a>", sGameID, rblContext.SelectedValue, Math.Abs(DiffAmount))
                End If
                TotalMLine += Math.Abs(DiffAmount)
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    lblDrawMoney.Text = String.Format("<a style='color:blue' href='/SBS/SuperAdmins/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=MoneyLine&AgentID={3}'>{2}</a>", sGameID, rblContext.SelectedValue, CalcPosition(sGameID, _sDrawMoneyLine, _sDraw, _odtTicketManager), ddlAgents.SelectedValue)

                Else
                    lblDrawMoney.Text = String.Format("<a style='color:blue' href='/SBS/Agents/DifferenceAmount.aspx?gameid={0}&context={1}&GameType=MoneyLine'>{2}</a>", sGameID, rblContext.SelectedValue, CalcPosition(sGameID, _sDrawMoneyLine, _sDraw, _odtTicketManager))
                End If


                NumGameRow -= 1
                If NumGameRow = 0 Then
                    CType(e.Item.FindControl("lblTotalSpread"), Label).Text = TotalSpread
                    CType(e.Item.FindControl("lblTotalTotal"), Label).Text = TotalTotal
                    CType(e.Item.FindControl("lblTotalMline"), Label).Text = TotalMLine
                    CType(e.Item.FindControl("rowTotal"), HtmlTableRow).Visible = True
                End If

            End If
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            If SelectedGameType <> "" AndAlso ddlAgents.SelectedValue <> "" Then
                LoadGames(SelectedGameType, rblContext.SelectedValue)
            End If
        End Sub

        Protected Sub rblContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblContext.SelectedIndexChanged
            If SelectedGameType <> "" Then
                LoadGames(SelectedGameType, rblContext.SelectedValue)
            End If
        End Sub

        Private Sub LoadGames(ByVal psGameType As String, ByVal psContext As String)
            Dim oToday = GetEasternDate()

            ''Get games
            Dim oGameShows As New Dictionary(Of String, DataRow())
            Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAllAvailableGames(psGameType, oToday)
            _odtTicketManager = LoadBetAmountByAgentID()
            NumGameRow = odtGames.Rows.Count
            TotalTotal = 0
            TotalSpread = 0
            TotalMLine = 0
            rptGameLines.DataSource = odtGames
            rptGameLines.DataBind()

            lblNoGameLine.Visible = odtGames.Rows.Count = 0
            rptGameLines.Visible = odtGames.Rows.Count > 0
        End Sub

        Private Function CalcPosition(ByVal psGameID As String, ByVal psMoney As String, ByVal psBetType As String, ByVal pdtTicketManager As DataTable) As String
            Dim dBetAmount As Double = BetAmount(psGameID, psMoney, psBetType, pdtTicketManager)
            If DiffAmount = nDiffAmountInit Then
                DiffAmount = dBetAmount
            Else
                DiffAmount -= dBetAmount
            End If

            Return SafeString(NumTicket(psGameID, psMoney, psBetType, pdtTicketManager)) & " - " & SafeString(dBetAmount)
        End Function
        'count ticket for game
        Private Function NumTicket(ByVal psGameID As String, ByVal psMoney As String, ByVal psBetType As String, ByVal pdtTicketManager As DataTable) As Integer
            Dim oNumTicket As Object
            oNumTicket = pdtTicketManager.Compute(String.Format("count({0})", psMoney), String.Format(" {0}<>0  and BetType='{1}' and GameID='{2}'", psMoney, psBetType, psGameID))
            Return SafeInteger(oNumTicket)
        End Function

        'sum Betamount for game
        Private Function BetAmount(ByVal psGameID As String, ByVal psMoney As String, ByVal psBetType As String, ByVal pdtTicketManager As DataTable) As Double
            Dim oBetAmount As Object
            oBetAmount = pdtTicketManager.Compute("sum(BetAmount)", String.Format(" {0}<>0 and BetType='{1}' and GameID='{2}'", psMoney, psBetType, psGameID))
            Return SafeDouble(oBetAmount)
        End Function

        Protected Function LoadBetAmountByAgentID() As DataTable
            Dim oLstAgentIDs As New List(Of String)
            Dim bSuperAdmin As Boolean = UserSession.UserType = SBCBL.EUserType.SuperAdmin AndAlso ddlAgents.Value = ""
            Dim sUserID As String = ""
            If ddlAgents.Value <> "" Then
                sUserID = ddlAgents.Value
            Else
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    sUserID = SuperID
                Else
                    sUserID = UserSession.UserID
                End If
            End If

            oLstAgentIDs = (New CAgentManager).GetAllSubAgentIDs(sUserID, bSuperAdmin)

            'Load BetAmount for agent or SuperAgent 
            Return (New SBCBL.Managers.CTicketManager()).GetBetAmountByListAgentID(oLstAgentIDs, rblContext.SelectedValue)

        End Function

        Protected Function GetSpreadTitle(ByVal psTitle As String) As String
            If SelectedGameType = "MLB AL Baseball" Or SelectedGameType = "MLB NL Baseball" Or SelectedGameType = "MLB Baseball" Then
                If psTitle = "Spread" Then
                    Return "Run Line"
                Else
                    Return "Total Runs"
                End If
            End If
            Return psTitle
        End Function

    End Class
End Namespace

