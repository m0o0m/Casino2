Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.Managers
Imports System.Collections
Imports System.Data


Namespace SBCWebsite
    Partial Class NewAgentPositionReport
        Inherits SBCBL.UI.CSBCUserControl
        Private _RISK As Double = 0
        Private _WIN As Double = 0
        Private _BET As Double = 0
        Private bShow1H As Boolean = False
        Private bShow2H As Boolean = False

#Region "Property"
        Public ReadOnly Property AgentID() As String
            Get
                Return IIf(UserSession.UserType = SBCBL.EUserType.Agent, UserSession.AgentSelectID, ddlAgents.SelectedValue)
            End Get

        End Property

        Public Property Risk() As Double
            Get
                Return _RISK
            End Get
            Set(ByVal value As Double)
                _RISK = value
            End Set
        End Property

        Public Property Win() As Double
            Get
                Return _WIN
            End Get
            Set(ByVal value As Double)
                _WIN = value
            End Set
        End Property

        Public Property Bet() As Double
            Get
                Return _BET
            End Get
            Set(ByVal value As Double)
                _BET = value
            End Set
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    bindAgents()
                Else
                    ddlAgents.Visible = False
                    divAgents.Visible = False
                End If

                BindSportType()
                getTotal()
            End If
        End Sub

#Region "Bind Data"

        Private Sub BindSportType()
            rptPortType.DataSource = getAllGameType()
            rptPortType.DataBind()
        End Sub

        Private Sub bindAgents()
            Dim oAgentManager As New CAgentManager()
            If UserSession.UserID <> "" Then
                ddlAgents.DataSource = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            End If
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub

        Private Sub BindAgentPosition(ByVal rptOpenBet As Repeater, ByVal psGameType As String, ByVal lblGameType As Label)
            Dim oTicketManager As New CTicketManager()

            ''ucAgentPositionReport.LoadTicketBets((New CTicketManager).GetOpenTicketsByAllSubAgent(getListSubAgentID(UserSession.UserID), GetEasternDate()))

            Dim dicAgentPosition As New Dictionary(Of String, CAgentPosition)
            Dim odt = oTicketManager.GetPositionReportByAllSubAgent(getListSubAgentID(AgentID), GetEasternDate(), psGameType, Not chkAllOpen.Checked)
            If odt IsNot Nothing OrElse odt.Rows.Count > 0 Then
                For Each odr As DataRow In odt.Rows
                    Dim oAgentPosition As New CAgentPosition(odr)
                    Dim sKey As String = oAgentPosition.AwayTeam & oAgentPosition.HomeTeam & oAgentPosition.AwayRotationNumber & oAgentPosition.HomeRotationNumber
                    'If dicAgentPosition.ContainsKey(sKey) Then
                    If dicAgentPosition.ContainsKey(sKey) Then
                        oAgentPosition = dicAgentPosition(sKey)
                    End If
                    Select Case UCase(SafeString(odr("BetType")))
                        Case "DRAW"
                            getDetailByMoneyLine(oAgentPosition, odr)
                        Case "SPREAD"
                            Select Case UCase(SafeString(odr("Context")))
                                Case "CURRENT"
                                    getDetailBySpread(oAgentPosition, odr)
                                Case "1H"
                                    bShow1H = True
                                    getDetailBySpread(oAgentPosition, odr)
                                Case "2H"
                                    bShow2H = True
                                    getDetailBySpread(oAgentPosition, odr)
                                Case "1Q"
                                    getDetailBySpread(oAgentPosition, odr)
                                Case "2Q"
                                    getDetailBySpread(oAgentPosition, odr)
                                Case "3Q"
                                    getDetailBySpread(oAgentPosition, odr)
                                Case "4Q"
                                    getDetailBySpread(oAgentPosition, odr)
                            End Select
                        Case "TOTALPOINTS"
                            'oAgentPosition = dicAgentPosition(sKey)
                            Select Case UCase(SafeString(odr("Context")))
                                Case "CURRENT"
                                    getDetailByTotalPoints(oAgentPosition, odr)
                                Case "1H"
                                    bShow1H = True
                                    getDetailByTotalPoints(oAgentPosition, odr)
                                Case "2H"
                                    bShow2H = True
                                    getDetailByTotalPoints(oAgentPosition, odr)
                                Case "1Q"
                                    getDetailByTotalPoints(oAgentPosition, odr)
                                Case "2Q"
                                    getDetailByTotalPoints(oAgentPosition, odr)
                                Case "3Q"
                                    getDetailByTotalPoints(oAgentPosition, odr)
                                Case "4Q"
                                    getDetailByTotalPoints(oAgentPosition, odr)
                            End Select
                        Case "MONEYLINE"
                            ' oAgentPosition = dicAgentPosition(sKey)
                            Select Case UCase(SafeString(odr("Context")))
                                Case "CURRENT"
                                    getDetailByMoneyLine(oAgentPosition, odr)
                                Case "1H"
                                    bShow1H = True
                                    getDetailByMoneyLine(oAgentPosition, odr)
                                Case "2H"
                                    bShow2H = True
                                    getDetailByMoneyLine(oAgentPosition, odr)
                                Case "1Q"
                                    getDetailByMoneyLine(oAgentPosition, odr)
                                Case "2Q"
                                    getDetailByMoneyLine(oAgentPosition, odr)
                                Case "3Q"
                                    getDetailByMoneyLine(oAgentPosition, odr)
                                Case "4Q"
                                    getDetailByMoneyLine(oAgentPosition, odr)
                            End Select
                    End Select
                    'Else
                    '    dicAgentPosition(sKey) = oAgentPosition
                    'End If
                    dicAgentPosition(sKey) = oAgentPosition
                Next
            End If
            If dicAgentPosition.Count > 0 Then
                lblGameType.Text = "<br/>" & psGameType.Replace("NCAA Football", "College Football").Replace("CFL Football", "Canadian").Replace("AFL Football", "Arena").Trim() & "<br/>"
                rptOpenBet.DataSource = dicAgentPosition
                rptOpenBet.DataBind()
            Else
                rptOpenBet.Visible = False
                lblGameType.Visible = False
            End If

        End Sub
#End Region

        Private Sub getDetailBySpread(ByRef poAgentPosition As CAgentPosition, ByVal poTicketBet As DataRow)
            Dim nHomeSpreadMoney As Double = SafeDouble(poTicketBet("HomeSpreadMoney"))
            Dim nAwaySpreadMoney As Double = SafeDouble(poTicketBet("AwaySpreadMoney"))
            Select Case UCase(SafeString(poTicketBet("Context")))
                Case "CURRENT"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.BetCurrentSpreadHome = 0 Then
                        '    poAgentPosition.BetCurrentSpreadHome = 1
                        'End If

                        poAgentPosition.BetCurrentSpreadHome += 1
                        poAgentPosition.RiskAmountCurrentSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmountCurrentSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmountCurrentSpreadHome = SafeRound(poAgentPosition.RiskAmountCurrentSpreadHome)
                        poAgentPosition.WinAmountCurrentSpreadHome = SafeRound(poAgentPosition.WinAmountCurrentSpreadHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.BetCurrentSpreadAway = 0 Then
                        '    poAgentPosition.BetCurrentSpreadAway = 1
                        'End If
                        poAgentPosition.BetCurrentSpreadAway += 1
                        poAgentPosition.RiskAmountCurrentSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmountCurrentSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmountCurrentSpreadAway = SafeRound(poAgentPosition.RiskAmountCurrentSpreadAway)
                        poAgentPosition.WinAmountCurrentSpreadAway = SafeRound(poAgentPosition.WinAmountCurrentSpreadAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "1H"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.Bet1HSpreadHome = 0 Then
                        '    poAgentPosition.Bet1HSpreadHome = 1
                        'End If
                        poAgentPosition.Bet1HSpreadHome += 1
                        poAgentPosition.RiskAmount1HSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1HSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1HSpreadHome = SafeRound(poAgentPosition.RiskAmount1HSpreadHome)
                        poAgentPosition.WinAmount1HSpreadHome = SafeRound(poAgentPosition.WinAmount1HSpreadHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.Bet1HSpreadAway = 0 Then
                        '    poAgentPosition.Bet1HSpreadAway = 1
                        'End If
                        poAgentPosition.Bet1HSpreadAway += 1
                        poAgentPosition.RiskAmount1HSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1HSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1HSpreadAway = SafeRound(poAgentPosition.RiskAmount1HSpreadAway)
                        poAgentPosition.WinAmount1HSpreadAway = SafeRound(poAgentPosition.WinAmount1HSpreadAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "2H"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.Bet2HSpreadHome = 0 Then
                        '    poAgentPosition.Bet2HSpreadHome = 1
                        'End If
                        poAgentPosition.Bet2HSpreadHome += 1
                        poAgentPosition.RiskAmount2HSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2HSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2HSpreadHome = SafeRound(poAgentPosition.RiskAmount2HSpreadHome)
                        poAgentPosition.WinAmount2HSpreadHome = SafeRound(poAgentPosition.WinAmount2HSpreadHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.Bet2HSpreadAway = 0 Then
                        '    poAgentPosition.Bet2HSpreadAway = 1
                        'End If
                        poAgentPosition.Bet2HSpreadAway += 1
                        poAgentPosition.RiskAmount2HSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2HSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2HSpreadAway = SafeRound(poAgentPosition.RiskAmount2HSpreadAway)
                        poAgentPosition.WinAmount2HSpreadAway = SafeRound(poAgentPosition.WinAmount2HSpreadAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "1Q"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.Bet1QSpreadHome = 0 Then
                        '    poAgentPosition.Bet1QSpreadHome = 1
                        'End If
                        poAgentPosition.Bet1QSpreadHome += 1
                        poAgentPosition.RiskAmount1QSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1QSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1QSpreadHome = SafeRound(poAgentPosition.RiskAmount1QSpreadHome)
                        poAgentPosition.WinAmount1QSpreadHome = SafeRound(poAgentPosition.WinAmount1QSpreadHome)
                    Else
                        If poAgentPosition.Bet1QSpreadAway = 0 Then
                            poAgentPosition.Bet1QSpreadAway = 1
                        End If
                        poAgentPosition.Bet1QSpreadAway += 1
                        poAgentPosition.RiskAmount1QSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1QSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1QSpreadAway = SafeRound(poAgentPosition.RiskAmount1QSpreadAway)
                        poAgentPosition.WinAmount1QSpreadAway = SafeRound(poAgentPosition.WinAmount1QSpreadAway)
                    End If
                Case "2Q"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.Bet2QSpreadHome = 0 Then
                        '    poAgentPosition.Bet2QSpreadHome = 1
                        'End If
                        poAgentPosition.Bet2QSpreadHome += 1
                        poAgentPosition.RiskAmount2QSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2QSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2QSpreadHome = SafeRound(poAgentPosition.RiskAmount2QSpreadHome)
                        poAgentPosition.WinAmount2QSpreadHome = SafeRound(poAgentPosition.WinAmount2QSpreadHome)
                    Else
                        'If poAgentPosition.Bet2QSpreadAway = 0 Then
                        '    poAgentPosition.Bet2QSpreadAway = 1
                        'End If
                        poAgentPosition.Bet2QSpreadAway += 1
                        poAgentPosition.RiskAmount2QSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2QSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2QSpreadAway = SafeRound(poAgentPosition.RiskAmount2QSpreadAway)
                        poAgentPosition.WinAmount2QSpreadAway = SafeRound(poAgentPosition.WinAmount2QSpreadAway)
                    End If
                Case "3Q"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.Bet3QSpreadHome = 0 Then
                        '    poAgentPosition.Bet3QSpreadHome = 1
                        'End If
                        poAgentPosition.Bet3QSpreadHome += 1
                        poAgentPosition.RiskAmount3QSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount3QSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount3QSpreadHome = SafeRound(poAgentPosition.RiskAmount3QSpreadHome)
                        poAgentPosition.WinAmount3QSpreadHome = SafeRound(poAgentPosition.WinAmount3QSpreadHome)
                    Else
                        'If poAgentPosition.Bet3QSpreadAway = 0 Then
                        '    poAgentPosition.Bet3QSpreadAway = 1
                        'End If
                        poAgentPosition.Bet3QSpreadAway += 1
                        poAgentPosition.RiskAmount3QSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount3QSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount3QSpreadAway = SafeRound(poAgentPosition.RiskAmount3QSpreadAway)
                        poAgentPosition.WinAmount3QSpreadAway = SafeRound(poAgentPosition.WinAmount3QSpreadAway)
                    End If
                Case "4Q"
                    If nHomeSpreadMoney <> 0 Then
                        'If poAgentPosition.Bet4QSpreadHome = 0 Then
                        '    poAgentPosition.Bet4QSpreadHome = 1
                        'End If
                        poAgentPosition.Bet4QSpreadHome += 1
                        poAgentPosition.RiskAmount4QSpreadHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount4QSpreadHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount4QSpreadHome = SafeRound(poAgentPosition.RiskAmount4QSpreadHome)
                        poAgentPosition.WinAmount4QSpreadHome = SafeRound(poAgentPosition.WinAmount4QSpreadHome)
                    Else
                        'If poAgentPosition.Bet4QSpreadAway = 0 Then
                        '    poAgentPosition.Bet4QSpreadAway = 1
                        'End If
                        poAgentPosition.Bet4QSpreadAway += 1
                        poAgentPosition.RiskAmount4QSpreadAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount4QSpreadAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount4QSpreadAway = SafeRound(poAgentPosition.RiskAmount4QSpreadAway)
                        poAgentPosition.WinAmount4QSpreadAway = SafeRound(poAgentPosition.WinAmount4QSpreadAway)
                    End If
            End Select


        End Sub

        Private Sub getDetailByTotalPoints(ByRef poAgentPosition As CAgentPosition, ByVal poTicketBet As DataRow)
            Dim nOverMoney As Double = SafeRound(poTicketBet("TotalPointsOverMoney"))
            Dim nUnderMoney As Double = SafeRound(poTicketBet("TotalPointsUnderMoney"))

            Select Case UCase(SafeString(poTicketBet("Context")))
                Case "CURRENT"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.BetCurrentTotalPointAway = 0 Then
                        '    poAgentPosition.BetCurrentTotalPointAway = 1
                        'End If
                        poAgentPosition.BetCurrentTotalPointAway += 1
                        poAgentPosition.RiskAmountCurrentTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmountCurrentTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmountCurrentTotalPointAway = SafeRound(poAgentPosition.RiskAmountCurrentTotalPointAway)
                        poAgentPosition.WinAmountCurrentTotalPointAway = SafeRound(poAgentPosition.WinAmountCurrentTotalPointAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.BetCurrentTotalPointHome = 0 Then
                        '    poAgentPosition.BetCurrentTotalPointHome = 1
                        'End If
                        poAgentPosition.BetCurrentTotalPointHome += 1
                        poAgentPosition.RiskAmountCurrentTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmountCurrentTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmountCurrentTotalPointHome = SafeRound(poAgentPosition.RiskAmountCurrentTotalPointHome)
                        poAgentPosition.WinAmountCurrentTotalPointHome = SafeRound(poAgentPosition.WinAmountCurrentTotalPointHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "1H"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.Bet1HTotalPointAway = 0 Then
                        '    poAgentPosition.Bet1HTotalPointAway = 1
                        'End If
                        poAgentPosition.Bet1HTotalPointAway += 1
                        poAgentPosition.RiskAmount1HTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1HTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1HTotalPointAway = SafeRound(poAgentPosition.RiskAmount1HTotalPointAway)
                        poAgentPosition.WinAmount1HTotalPointAway = SafeRound(poAgentPosition.WinAmount1HTotalPointAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.Bet1HTotalPointHome = 0 Then
                        '    poAgentPosition.Bet1HTotalPointHome = 1
                        'End If
                        poAgentPosition.Bet1HTotalPointHome += 1
                        poAgentPosition.RiskAmount1HTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1HTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1HTotalPointHome = SafeRound(poAgentPosition.RiskAmount1HTotalPointHome)
                        poAgentPosition.WinAmount1HTotalPointHome = SafeRound(poAgentPosition.WinAmount1HTotalPointHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "2H"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.Bet2HTotalPointAway = 0 Then
                        '    poAgentPosition.Bet2HTotalPointAway = 1
                        'End If
                        poAgentPosition.Bet2HTotalPointAway += 1
                        poAgentPosition.RiskAmount2HTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2HTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2HTotalPointAway = SafeRound(poAgentPosition.RiskAmount2HTotalPointAway)
                        poAgentPosition.WinAmount2HTotalPointAway = SafeRound(poAgentPosition.WinAmount2HTotalPointAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.Bet2HTotalPointHome = 0 Then
                        '    poAgentPosition.Bet2HTotalPointHome = 1
                        'End If
                        poAgentPosition.Bet2HTotalPointHome += 1
                        poAgentPosition.RiskAmount2HTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2HTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2HTotalPointHome = SafeRound(poAgentPosition.RiskAmount2HTotalPointHome)
                        poAgentPosition.WinAmount2HTotalPointHome = SafeRound(poAgentPosition.WinAmount2HTotalPointHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "1Q"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.Bet1QTotalPointAway = 0 Then
                        '    poAgentPosition.Bet1QTotalPointAway = 1
                        'End If
                        poAgentPosition.Bet1QTotalPointAway += 1
                        poAgentPosition.RiskAmount1QTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1QTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1QTotalPointAway = SafeRound(poAgentPosition.RiskAmount1QTotalPointAway)
                        poAgentPosition.WinAmount1QTotalPointAway = SafeRound(poAgentPosition.WinAmount1QTotalPointAway)
                    Else
                        'If poAgentPosition.Bet1QTotalPointHome = 0 Then
                        '    poAgentPosition.Bet1QTotalPointHome = 1
                        'End If
                        poAgentPosition.Bet1QTotalPointHome += 1
                        poAgentPosition.RiskAmount1QTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1QTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1QTotalPointHome = SafeRound(poAgentPosition.RiskAmount1QTotalPointHome)
                        poAgentPosition.WinAmount1QTotalPointHome = SafeRound(poAgentPosition.WinAmount1QTotalPointHome)
                    End If
                Case "2Q"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.Bet2QTotalPointAway = 0 Then
                        '    poAgentPosition.Bet2QTotalPointAway = 1
                        'End If
                        poAgentPosition.Bet2QTotalPointAway += 1
                        poAgentPosition.RiskAmount2QTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2QTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2QTotalPointAway = SafeRound(poAgentPosition.RiskAmount2QTotalPointAway)
                        poAgentPosition.WinAmount2QTotalPointAway = SafeRound(poAgentPosition.WinAmount2QTotalPointAway)
                    Else
                        'If poAgentPosition.Bet2QTotalPointHome = 0 Then
                        '    poAgentPosition.Bet2QTotalPointHome = 1
                        'End If
                        poAgentPosition.Bet2QTotalPointHome += 1
                        poAgentPosition.RiskAmount2QTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2QTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2QTotalPointHome = SafeRound(poAgentPosition.RiskAmount2QTotalPointHome)
                        poAgentPosition.WinAmount2QTotalPointHome = SafeRound(poAgentPosition.WinAmount2QTotalPointHome)
                    End If
                Case "3Q"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.Bet3QTotalPointAway = 0 Then
                        '    poAgentPosition.Bet3QTotalPointAway = 1
                        'End If
                        poAgentPosition.Bet3QTotalPointAway += 1
                        poAgentPosition.RiskAmount3QTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount3QTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount3QTotalPointAway = SafeRound(poAgentPosition.RiskAmount3QTotalPointAway)
                        poAgentPosition.WinAmount3QTotalPointAway = SafeRound(poAgentPosition.WinAmount3QTotalPointAway)
                    Else
                        'If poAgentPosition.Bet3QTotalPointHome = 0 Then
                        '    poAgentPosition.Bet3QTotalPointHome = 1
                        'End If
                        poAgentPosition.Bet3QTotalPointHome += 1
                        poAgentPosition.RiskAmount3QTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount3QTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount3QTotalPointHome = SafeRound(poAgentPosition.RiskAmount3QTotalPointHome)
                        poAgentPosition.WinAmount3QTotalPointHome = SafeRound(poAgentPosition.WinAmount3QTotalPointHome)
                    End If
                Case "4Q"
                    If nOverMoney <> 0 Then
                        'If poAgentPosition.Bet4QTotalPointAway = 0 Then
                        '    poAgentPosition.Bet4QTotalPointAway = 1
                        'End If
                        poAgentPosition.Bet4QTotalPointAway += 1
                        poAgentPosition.RiskAmount4QTotalPointAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount4QTotalPointAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount4QTotalPointAway = SafeRound(poAgentPosition.RiskAmount4QTotalPointAway)
                        poAgentPosition.WinAmount4QTotalPointAway = SafeRound(poAgentPosition.WinAmount4QTotalPointAway)
                    Else
                        'If poAgentPosition.Bet4QTotalPointHome = 0 Then
                        '    poAgentPosition.Bet4QTotalPointHome = 1
                        'End If
                        poAgentPosition.Bet4QTotalPointHome += 1
                        poAgentPosition.RiskAmount4QTotalPointHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount4QTotalPointHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount4QTotalPointHome = SafeRound(poAgentPosition.RiskAmount4QTotalPointHome)
                        poAgentPosition.WinAmount4QTotalPointHome = SafeRound(poAgentPosition.WinAmount4QTotalPointHome)
                    End If
            End Select
        End Sub

        Private Sub getDetailByMoneyLine(ByRef poAgentPosition As CAgentPosition, ByVal poTicketBet As DataRow)
            Dim nHomeMoneyLine As Double = SafeDouble(poTicketBet("HomeMoneyLine"))
            Dim nAwayMoneyLine As Double = SafeDouble(poTicketBet("AwayMoneyLine"))

            Select Case UCase(SafeString(poTicketBet("Context")))
                Case "CURRENT"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.BetCurrentMLHome = 0 Then
                        '    poAgentPosition.BetCurrentMLHome = 1
                        'End If
                        poAgentPosition.BetCurrentMLHome += 1
                        poAgentPosition.RiskAmountCurrentMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmountCurrentMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmountCurrentMLHome = SafeRound(poAgentPosition.RiskAmountCurrentMLHome)
                        poAgentPosition.WinAmountCurrentMLHome = SafeRound(poAgentPosition.WinAmountCurrentMLHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.BetCurrentMLAway = 0 Then
                        '    poAgentPosition.BetCurrentMLAway = 1
                        'End If
                        poAgentPosition.BetCurrentMLAway += 1
                        poAgentPosition.RiskAmountCurrentMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmountCurrentMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmountCurrentMLAway = SafeRound(poAgentPosition.RiskAmountCurrentMLAway)
                        poAgentPosition.WinAmountCurrentMLAway = SafeRound(poAgentPosition.WinAmountCurrentMLAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "1H"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.Bet1HMLHome = 0 Then
                        '    poAgentPosition.Bet1HMLHome = 1
                        'End If
                        poAgentPosition.Bet1HMLHome += 1
                        poAgentPosition.RiskAmount1HMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1HMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1HMLHome = SafeRound(poAgentPosition.RiskAmount1HMLHome)
                        poAgentPosition.WinAmount1HMLHome = SafeRound(poAgentPosition.WinAmount1HMLHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.Bet1HMLAway = 0 Then
                        '    poAgentPosition.Bet1HMLAway = 1
                        'End If
                        poAgentPosition.Bet1HMLAway += 1
                        poAgentPosition.RiskAmount1HMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1HMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1HMLAway = SafeRound(poAgentPosition.RiskAmount1HMLAway)
                        poAgentPosition.WinAmount1HMLAway = SafeRound(poAgentPosition.WinAmount1HMLAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "2H"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.Bet2HMLHome = 0 Then
                        '    poAgentPosition.Bet2HMLHome = 1
                        'End If
                        poAgentPosition.Bet2HMLHome += 1
                        poAgentPosition.RiskAmount2HMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2HMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2HMLHome = SafeRound(poAgentPosition.RiskAmount2HMLHome)
                        poAgentPosition.WinAmount2HMLHome = SafeRound(poAgentPosition.WinAmount2HMLHome)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    Else
                        'If poAgentPosition.Bet2HMLAway = 0 Then
                        '    poAgentPosition.Bet2HMLAway = 1
                        'End If
                        poAgentPosition.Bet2HMLAway += 1
                        poAgentPosition.RiskAmount2HMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2HMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2HMLAway = SafeRound(poAgentPosition.RiskAmount2HMLAway)
                        poAgentPosition.WinAmount2HMLAway = SafeRound(poAgentPosition.WinAmount2HMLAway)
                        Risk += SafeDouble(poTicketBet("RiskAmount"))
                        Win += SafeDouble(poTicketBet("WinAmount"))
                        Bet += 1
                    End If
                Case "1Q"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.Bet1QMLHome = 0 Then
                        '    poAgentPosition.Bet1QMLHome = 1
                        'End If
                        poAgentPosition.Bet1QMLHome += 1
                        poAgentPosition.RiskAmount1QMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1QMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1QMLHome = SafeRound(poAgentPosition.RiskAmount1QMLHome)
                        poAgentPosition.WinAmount1QMLHome = SafeRound(poAgentPosition.WinAmount1QMLHome)
                    Else
                        'If poAgentPosition.Bet1QMLAway = 0 Then
                        '    poAgentPosition.Bet1QMLAway = 1
                        'End If
                        poAgentPosition.Bet1QMLAway += 1
                        poAgentPosition.RiskAmount1QMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount1QMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount1QMLAway = SafeRound(poAgentPosition.RiskAmount1QMLAway)
                        poAgentPosition.WinAmount1QMLAway = SafeRound(poAgentPosition.WinAmount1QMLAway)
                    End If
                Case "2Q"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.Bet2QMLHome = 0 Then
                        '    poAgentPosition.Bet2QMLHome = 1
                        'End If
                        poAgentPosition.Bet2QMLHome += 1
                        poAgentPosition.RiskAmount2QMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2QMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2QMLHome = SafeRound(poAgentPosition.RiskAmount2QMLHome)
                        poAgentPosition.WinAmount2QMLHome = SafeRound(poAgentPosition.WinAmount2QMLHome)
                    Else
                        'If poAgentPosition.Bet2QMLAway = 0 Then
                        '    poAgentPosition.Bet2QMLAway = 1
                        'End If
                        poAgentPosition.Bet2QMLAway += 1
                        poAgentPosition.RiskAmount2QMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount2QMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount2QMLAway = SafeRound(poAgentPosition.RiskAmount2QMLAway)
                        poAgentPosition.WinAmount2QMLAway = SafeRound(poAgentPosition.WinAmount2QMLAway)
                    End If
                Case "3Q"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.Bet3QMLHome = 0 Then
                        '    poAgentPosition.Bet3QMLHome = 1
                        'End If
                        poAgentPosition.Bet3QMLHome += 1
                        poAgentPosition.RiskAmount3QMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount3QMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount3QMLHome = SafeRound(poAgentPosition.RiskAmount3QMLHome)
                        poAgentPosition.WinAmount3QMLHome = SafeRound(poAgentPosition.WinAmount3QMLHome)
                    Else
                        'If poAgentPosition.Bet3QMLAway = 0 Then
                        '    poAgentPosition.Bet3QMLAway = 1
                        'End If
                        poAgentPosition.Bet3QMLAway += 1
                        poAgentPosition.RiskAmount3QMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount3QMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount3QMLAway = SafeRound(poAgentPosition.RiskAmount3QMLAway)
                        poAgentPosition.WinAmount3QMLAway = SafeRound(poAgentPosition.WinAmount3QMLAway)
                    End If
                Case "4Q"
                    If nHomeMoneyLine <> 0 Then
                        'If poAgentPosition.Bet4QMLHome = 0 Then
                        '    poAgentPosition.Bet4QMLHome = 1
                        'End If
                        poAgentPosition.Bet4QMLHome += 1
                        poAgentPosition.RiskAmount4QMLHome += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount4QMLHome += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount4QMLHome = SafeRound(poAgentPosition.RiskAmount4QMLHome)
                        poAgentPosition.WinAmount4QMLHome = SafeRound(poAgentPosition.WinAmount4QMLHome)
                    Else
                        'If poAgentPosition.Bet4QMLAway = 0 Then
                        '    poAgentPosition.Bet4QMLAway = 1
                        'End If
                        poAgentPosition.Bet4QMLAway += 1
                        poAgentPosition.RiskAmount4QMLAway += SafeDouble(poTicketBet("RiskAmount"))
                        poAgentPosition.WinAmount4QMLAway += SafeDouble(poTicketBet("WinAmount"))
                        poAgentPosition.RiskAmount4QMLAway = SafeRound(poAgentPosition.RiskAmount4QMLAway)
                        poAgentPosition.WinAmount4QMLAway = SafeRound(poAgentPosition.WinAmount4QMLAway)
                    End If
            End Select

        End Sub

        Private Function getListSubAgentID(ByVal psAgentID As String) As List(Of String)
            Dim olstSubAgent As List(Of String) = New List(Of String)
            If psAgentID <> "" Then
                Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsByAgent(psAgentID, Nothing)
                For Each odrSubAgent As DataRow In odtSubAgent.Rows
                    olstSubAgent.Add(SafeString(odrSubAgent("AgentID")))
                Next
                olstSubAgent.Add(psAgentID)
            End If
            Return olstSubAgent
        End Function

        Private Sub setLinkOpenBet(ByVal lbl As Label, ByVal poAgentPosition As CAgentPosition, ByVal psBetType As String, ByVal psBetAway As String, ByVal psContext As String)
            Dim sLink As String
            Dim oDate As Date = GetEasternDate()
           
            sLink = String.Format("window.location='/SBS/Agents/OpenBets.aspx?AgentID={0}&AwayTeam={1}&HomeTeam={2}&BetType={3}&Context={4}&BetAway={5}", _
             SafeString(AgentID), poAgentPosition.AwayTeam, poAgentPosition.HomeTeam, psBetType, psContext, psBetAway)
            lbl.Style.Add("cursor", "pointer")
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                sLink = sLink.Replace("Agents", "SuperAdmins").Replace("OpenBets", "PendingTickets")
            End If
            lbl.Attributes.Add("onClick", String.Format(sLink & "&EDate={0}&bToday={1}';", _
                                                             Format(oDate, "MM/dd/yyyy"), chkAllOpen.Checked))
        End Sub


        Protected Sub rptOpenBet_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim lblTransactionDate, lblAwayRotationNumber, lblAwayTeam, lblCurrentBetAwaySpread, lblRiskAmountCurrentAwaySpread As Label
            Dim lblHomeRotationNumber, lblHomeTeam, lblCurrentBetHomeSpread, lblRiskAmountCurrentHomeSpread As Label
            Dim lblCurrentBetAwayML, lblRiskAmountCurrentAwayML, lblCurrentBetAwayTotalPoint, lblRiskAmountCurrentTotalPointOver As Label
            Dim lblCurrentBetHomeML, lblRiskAmountCurrentHomeML, lblCurrentBetHomeTotalPoint, lblRiskAmountCurrentTotalPointUnder As Label

            Dim lbl1HBetAwaySpread, lblRiskAmount1HAwaySpread, lbl1HBetAwayML, lblRiskAmount1HAwayML, lbl1HBetAwayTotalPoint, lblRiskAmount1HTotalPointOver As Label
            Dim lbl1HBetHomeSpread, lblRiskAmount1HHomeSpread, lbl1HBetHomeML, lblRiskAmount1HHomeML, lbl1HBetHomeTotalPoint, lblRiskAmount1HTotalPointUnder As Label

            Dim lbl2HBetAwaySpread, lblRiskAmount2HAwaySpread, lbl2HBetAwayML, lblRiskAmount2HAwayML, lbl2HBetAwayTotalPoint, lblRiskAmount2HTotalPointOver As Label
            Dim lbl2HBetHomeSpread, lblRiskAmount2HHomeSpread, lbl2HBetHomeML, lblRiskAmount2HHomeML, lbl2HBetHomeTotalPoint, lblRiskAmount2HTotalPointUnder As Label

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                If SafeString(ViewState("OddEven")) = "odd" Then
                    ViewState("OddEven") = "even"
                Else
                    ViewState("OddEven") = "odd"
                End If
                lblTransactionDate = e.Item.FindControl("lblTransactionDate")
                lblAwayRotationNumber = e.Item.FindControl("lblAwayRotationNumber")
                lblAwayTeam = e.Item.FindControl("lblAwayTeam")
                lblHomeRotationNumber = e.Item.FindControl("lblHomeRotationNumber")
                lblHomeTeam = e.Item.FindControl("lblHomeTeam")

                lblCurrentBetAwaySpread = e.Item.FindControl("lblCurrentBetAwaySpread")
                lblCurrentBetHomeSpread = e.Item.FindControl("lblCurrentBetHomeSpread")
                lblRiskAmountCurrentAwaySpread = e.Item.FindControl("lblRiskAmountCurrentAwaySpread")
                lblRiskAmountCurrentHomeSpread = e.Item.FindControl("lblRiskAmountCurrentHomeSpread")
                lblCurrentBetAwayML = e.Item.FindControl("lblCurrentBetAwayML")
                lblRiskAmountCurrentAwayML = e.Item.FindControl("lblRiskAmountCurrentAwayML")
                lblCurrentBetAwayTotalPoint = e.Item.FindControl("lblCurrentBetAwayTotalPoint")
                lblRiskAmountCurrentTotalPointOver = e.Item.FindControl("lblRiskAmountCurrentTotalPointOver")
                lblCurrentBetHomeML = e.Item.FindControl("lblCurrentBetHomeML")
                lblRiskAmountCurrentHomeML = e.Item.FindControl("lblRiskAmountCurrentHomeML")
                lblCurrentBetHomeTotalPoint = e.Item.FindControl("lblCurrentBetHomeTotalPoint")
                lblRiskAmountCurrentTotalPointUnder = e.Item.FindControl("lblRiskAmountCurrentTotalPointUnder")

                lbl1HBetAwaySpread = e.Item.FindControl("lbl1HBetAwaySpread")
                lbl1HBetHomeSpread = e.Item.FindControl("lbl1HBetHomeSpread")
                lblRiskAmount1HAwaySpread = e.Item.FindControl("lblRiskAmount1HAwaySpread")
                lblRiskAmount1HHomeSpread = e.Item.FindControl("lblRiskAmount1HHomeSpread")
                lbl1HBetAwayML = e.Item.FindControl("lbl1HBetAwayML")
                lblRiskAmount1HAwayML = e.Item.FindControl("lblRiskAmount1HAwayML")
                lbl1HBetAwayTotalPoint = e.Item.FindControl("lbl1HBetAwayTotalPoint")
                lblRiskAmount1HTotalPointOver = e.Item.FindControl("lblRiskAmount1HTotalPointOver")
                lbl1HBetHomeML = e.Item.FindControl("lbl1HBetHomeML")
                lblRiskAmount1HHomeML = e.Item.FindControl("lblRiskAmount1HHomeML")
                lbl1HBetHomeTotalPoint = e.Item.FindControl("lbl1HBetHomeTotalPoint")
                lblRiskAmount1HTotalPointUnder = e.Item.FindControl("lblRiskAmount1HTotalPointUnder")

                lbl2HBetAwaySpread = e.Item.FindControl("lbl2HBetAwaySpread")
                lbl2HBetHomeSpread = e.Item.FindControl("lbl2HBetHomeSpread")
                lblRiskAmount2HAwaySpread = e.Item.FindControl("lblRiskAmount2HAwaySpread")
                lblRiskAmount2HHomeSpread = e.Item.FindControl("lblRiskAmount2HHomeSpread")
                lbl2HBetAwayML = e.Item.FindControl("lbl2HBetAwayML")
                lblRiskAmount2HAwayML = e.Item.FindControl("lblRiskAmount2HAwayML")
                lbl2HBetAwayTotalPoint = e.Item.FindControl("lbl2HBetAwayTotalPoint")
                lblRiskAmount2HTotalPointOver = e.Item.FindControl("lblRiskAmount2HTotalPointOver")
                lbl2HBetHomeML = e.Item.FindControl("lbl2HBetHomeML")
                lblRiskAmount2HHomeML = e.Item.FindControl("lblRiskAmount2HHomeML")
                lbl2HBetHomeTotalPoint = e.Item.FindControl("lbl2HBetHomeTotalPoint")
                lblRiskAmount2HTotalPointUnder = e.Item.FindControl("lblRiskAmount2HTotalPointUnder")
                'lblCurrentBetHomeSpread = e.Item.FindControl("lblCurrentBetHomeSpread")
                'lblCurrentBetHomeSpread = e.Item.FindControl("lblCurrentBetHomeSpread")
                'lblCurrentBetHomeSpread = e.Item.FindControl("lblCurrentBetHomeSpread")
                '''''''''spread''''''''''''''''''
                Dim kpAgentPosition As KeyValuePair(Of String, CAgentPosition) = e.Item.DataItem
                Dim oAgentPosition As CAgentPosition = kpAgentPosition.Value
                lblTransactionDate.Text = oAgentPosition.TransactionDate.ToString("MM/dd/yy hh:mm tt")
                lblAwayRotationNumber.Text = oAgentPosition.AwayRotationNumber
                lblAwayTeam.Text = oAgentPosition.AwayTeam
                lblHomeRotationNumber.Text = oAgentPosition.HomeRotationNumber
                lblHomeTeam.Text = oAgentPosition.HomeTeam


                If rdWin.Checked Then

                    lblCurrentBetAwaySpread.Text = oAgentPosition.BetCurrentSpreadAway
                    lblRiskAmountCurrentAwaySpread.Style.Add("cursor", "pointer")
                    lblRiskAmountCurrentAwaySpread.Text = oAgentPosition.WinAmountCurrentSpreadAway
                    If oAgentPosition.WinAmountCurrentSpreadAway > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentAwaySpread, oAgentPosition, "Spread", "AwaySpreadMoney", "Current")
                    End If
                    lblCurrentBetHomeSpread.Text = oAgentPosition.BetCurrentSpreadHome
                    lblRiskAmountCurrentHomeSpread.Text = oAgentPosition.WinAmountCurrentSpreadHome
                    If oAgentPosition.WinAmountCurrentSpreadHome > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentHomeSpread, oAgentPosition, "Spread", "HomeSpreadMoney", "Current")
                    End If
                    lblCurrentBetAwayML.Text = oAgentPosition.BetCurrentMLAway
                    lblRiskAmountCurrentAwayML.Text = oAgentPosition.WinAmountCurrentMLAway
                    If oAgentPosition.WinAmountCurrentMLAway > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentAwayML, oAgentPosition, "MoneyLine", "AwayMoneyLine", "Current")
                    End If
                    lblCurrentBetHomeML.Text = oAgentPosition.BetCurrentMLHome
                    lblRiskAmountCurrentHomeML.Text = oAgentPosition.WinAmountCurrentMLHome
                    If oAgentPosition.WinAmountCurrentMLHome > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentHomeML, oAgentPosition, "MoneyLine", "HomeMoneyLine", "Current")
                    End If
                    lblCurrentBetAwayTotalPoint.Text = oAgentPosition.BetCurrentTotalPointAway
                    lblRiskAmountCurrentTotalPointOver.Text = oAgentPosition.WinAmountCurrentTotalPointAway
                    If oAgentPosition.WinAmountCurrentTotalPointAway > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentTotalPointOver, oAgentPosition, "TotalPoints", "TotalPointsOverMoney", "Current")
                    End If
                    lblCurrentBetHomeTotalPoint.Text = oAgentPosition.BetCurrentTotalPointHome
                    lblRiskAmountCurrentTotalPointUnder.Text = oAgentPosition.WinAmountCurrentTotalPointHome
                    If oAgentPosition.WinAmountCurrentTotalPointHome > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentTotalPointUnder, oAgentPosition, "TotalPoints", "TotalPointsUnderMoney", "Current")
                    End If
                    lbl1HBetAwaySpread.Text = oAgentPosition.Bet1HSpreadAway
                    lblRiskAmount1HAwaySpread.Text = oAgentPosition.WinAmount1HSpreadAway
                    If oAgentPosition.WinAmount1HSpreadAway > 0 Then
                        setLinkOpenBet(lblRiskAmount1HAwaySpread, oAgentPosition, "Spread", "AwaySpreadMoney", "1H")
                    End If
                    lbl1HBetHomeSpread.Text = oAgentPosition.Bet1HSpreadHome
                    lblRiskAmount1HHomeSpread.Text = oAgentPosition.WinAmount1HSpreadHome
                    If oAgentPosition.WinAmount1HSpreadHome > 0 Then
                        setLinkOpenBet(lblRiskAmount1HHomeSpread, oAgentPosition, "Spread", "HomeSpreadMoney", "1H")
                    End If
                    lbl1HBetAwayML.Text = oAgentPosition.Bet1HMLAway
                    lblRiskAmount1HAwayML.Text = oAgentPosition.WinAmount1HMLAway
                    If oAgentPosition.WinAmount1HMLAway > 0 Then
                        setLinkOpenBet(lblRiskAmount1HAwayML, oAgentPosition, "MoneyLine", "AwayMoneyLine", "1H")
                    End If
                    lbl1HBetHomeML.Text = oAgentPosition.Bet1HMLHome
                    lblRiskAmount1HHomeML.Text = oAgentPosition.WinAmount1HMLHome
                    If oAgentPosition.WinAmount1HMLHome > 0 Then
                        setLinkOpenBet(lblRiskAmount1HHomeML, oAgentPosition, "MoneyLine", "HomeMoneyLine", "1H")
                    End If
                    lbl1HBetAwayTotalPoint.Text = oAgentPosition.Bet1HTotalPointAway
                    lblRiskAmount1HTotalPointOver.Text = oAgentPosition.WinAmount1HTotalPointAway
                    If oAgentPosition.WinAmount1HTotalPointAway > 0 Then
                        setLinkOpenBet(lblRiskAmount1HTotalPointOver, oAgentPosition, "TotalPoints", "TotalPointsOverMoney", "1H")
                    End If
                    lbl1HBetHomeTotalPoint.Text = oAgentPosition.Bet1HTotalPointHome
                    lblRiskAmount1HTotalPointUnder.Text = oAgentPosition.WinAmount1HTotalPointHome
                    If oAgentPosition.WinAmount1HTotalPointHome > 0 Then
                        setLinkOpenBet(lblRiskAmount1HTotalPointUnder, oAgentPosition, "TotalPoints", "TotalPointsUnderMoney", "1H")
                    End If
                    lbl2HBetAwaySpread.Text = oAgentPosition.Bet2HSpreadAway
                    lblRiskAmount2HAwaySpread.Text = oAgentPosition.WinAmount2HSpreadAway
                    If oAgentPosition.WinAmount2HSpreadAway > 0 Then
                        setLinkOpenBet(lblRiskAmount2HAwaySpread, oAgentPosition, "Spread", "AwaySpreadMoney", "2H")
                    End If
                    lbl2HBetHomeSpread.Text = oAgentPosition.Bet2HSpreadHome
                    lblRiskAmount2HHomeSpread.Text = oAgentPosition.WinAmount2HSpreadHome
                    If oAgentPosition.WinAmount2HSpreadHome > 0 Then
                        setLinkOpenBet(lblRiskAmount2HHomeSpread, oAgentPosition, "Spread", "HomeSpreadMoney", "2H")
                    End If
                    lbl2HBetAwayML.Text = oAgentPosition.Bet2HMLAway
                    lblRiskAmount2HAwayML.Text = oAgentPosition.WinAmount2HMLAway
                    If oAgentPosition.WinAmount2HMLAway > 0 Then
                        setLinkOpenBet(lblRiskAmount2HAwayML, oAgentPosition, "MoneyLine", "AwayMoneyLine", "2H")
                    End If
                    lbl2HBetHomeML.Text = oAgentPosition.Bet2HMLHome
                    lblRiskAmount2HHomeML.Text = oAgentPosition.WinAmount2HMLHome
                    If oAgentPosition.WinAmount2HMLHome > 0 Then
                        setLinkOpenBet(lblRiskAmount2HHomeML, oAgentPosition, "MoneyLine", "HomeMoneyLine", "2H")
                    End If
                    lbl2HBetAwayTotalPoint.Text = oAgentPosition.Bet2HTotalPointAway
                    lblRiskAmount2HTotalPointOver.Text = oAgentPosition.WinAmount2HTotalPointAway
                    If oAgentPosition.WinAmount2HTotalPointAway > 0 Then
                        setLinkOpenBet(lblRiskAmount2HTotalPointOver, oAgentPosition, "TotalPoints", "TotalPointsOverMoney", "2H")
                    End If
                    lbl2HBetHomeTotalPoint.Text = oAgentPosition.Bet2HTotalPointHome
                    lblRiskAmount2HTotalPointUnder.Text = oAgentPosition.WinAmount2HTotalPointHome
                    If oAgentPosition.WinAmount2HTotalPointHome > 0 Then
                        setLinkOpenBet(lblRiskAmount2HTotalPointUnder, oAgentPosition, "TotalPoints", "TotalPointsUnderMoney", "2H")
                    End If
                Else
                    lblCurrentBetAwaySpread.Text = oAgentPosition.BetCurrentSpreadAway
                    lblRiskAmountCurrentAwaySpread.Text = oAgentPosition.RiskAmountCurrentSpreadAway
                    If oAgentPosition.RiskAmountCurrentSpreadAway > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentAwaySpread, oAgentPosition, "Spread", "AwaySpreadMoney", "Current")
                    End If
                    lblCurrentBetHomeSpread.Text = oAgentPosition.BetCurrentSpreadHome
                    lblRiskAmountCurrentHomeSpread.Text = oAgentPosition.RiskAmountCurrentSpreadHome
                    If oAgentPosition.RiskAmountCurrentSpreadHome > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentHomeSpread, oAgentPosition, "Spread", "HomeSpreadMoney", "Current")
                    End If
                    lblCurrentBetAwayML.Text = oAgentPosition.BetCurrentMLAway
                    lblRiskAmountCurrentAwayML.Text = oAgentPosition.RiskAmountCurrentMLAway
                    If oAgentPosition.RiskAmountCurrentMLAway > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentAwayML, oAgentPosition, "MoneyLine", "AwayMoneyLine", "Current")
                    End If
                    lblCurrentBetHomeML.Text = oAgentPosition.BetCurrentMLHome
                    lblRiskAmountCurrentHomeML.Text = oAgentPosition.RiskAmountCurrentMLHome
                    If oAgentPosition.RiskAmountCurrentMLHome > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentHomeML, oAgentPosition, "MoneyLine", "HomeMoneyLine", "Current")
                    End If
                    lblCurrentBetAwayTotalPoint.Text = oAgentPosition.BetCurrentTotalPointAway
                    lblRiskAmountCurrentTotalPointOver.Text = oAgentPosition.RiskAmountCurrentTotalPointAway
                    If oAgentPosition.RiskAmountCurrentTotalPointAway > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentTotalPointOver, oAgentPosition, "TotalPoints", "TotalPointsOverMoney", "Current")
                    End If
                    lblCurrentBetHomeTotalPoint.Text = oAgentPosition.BetCurrentTotalPointHome
                    lblRiskAmountCurrentTotalPointUnder.Text = oAgentPosition.RiskAmountCurrentTotalPointHome
                    If oAgentPosition.RiskAmountCurrentTotalPointHome > 0 Then
                        setLinkOpenBet(lblRiskAmountCurrentTotalPointUnder, oAgentPosition, "TotalPoints", "TotalPointsUnderMoney", "Current")
                    End If
                    lbl1HBetAwaySpread.Text = oAgentPosition.Bet1HSpreadAway
                    lblRiskAmount1HAwaySpread.Text = oAgentPosition.RiskAmount1HSpreadAway
                    If oAgentPosition.RiskAmount1HSpreadAway > 0 Then
                        setLinkOpenBet(lblRiskAmount1HAwaySpread, oAgentPosition, "Spread", "AwaySpreadMoney", "1H")
                    End If
                    lbl1HBetHomeSpread.Text = oAgentPosition.Bet1HSpreadHome
                    lblRiskAmount1HHomeSpread.Text = oAgentPosition.RiskAmount1HSpreadHome
                    If oAgentPosition.RiskAmount1HSpreadHome > 0 Then
                        setLinkOpenBet(lblRiskAmount1HHomeSpread, oAgentPosition, "Spread", "HomeSpreadMoney", "1H")
                    End If
                    lbl1HBetAwayML.Text = oAgentPosition.Bet1HMLAway
                    lblRiskAmount1HAwayML.Text = oAgentPosition.RiskAmount1HMLAway
                    If oAgentPosition.RiskAmount1HMLAway > 0 Then
                        setLinkOpenBet(lblRiskAmount1HAwayML, oAgentPosition, "MoneyLine", "AwayMoneyLine", "1H")
                    End If
                    lbl1HBetHomeML.Text = oAgentPosition.Bet1HMLHome
                    lblRiskAmount1HHomeML.Text = oAgentPosition.RiskAmount1HMLHome
                    If oAgentPosition.RiskAmount1HMLHome > 0 Then
                        setLinkOpenBet(lblRiskAmount1HHomeML, oAgentPosition, "MoneyLine", "HomeMoneyLine", "1H")
                    End If
                    lbl1HBetAwayTotalPoint.Text = oAgentPosition.Bet1HTotalPointAway
                    lblRiskAmount1HTotalPointOver.Text = oAgentPosition.RiskAmount1HTotalPointAway
                    If oAgentPosition.RiskAmount1HTotalPointAway > 0 Then
                        setLinkOpenBet(lblRiskAmount1HTotalPointOver, oAgentPosition, "TotalPoints", "TotalPointsOverMoney", "1H")
                    End If
                    lbl1HBetHomeTotalPoint.Text = oAgentPosition.Bet1HTotalPointHome
                    lblRiskAmount1HTotalPointUnder.Text = oAgentPosition.RiskAmount1HTotalPointHome
                    If oAgentPosition.RiskAmount1HTotalPointHome > 0 Then
                        setLinkOpenBet(lblRiskAmount1HTotalPointUnder, oAgentPosition, "TotalPoints", "TotalPointsUnderMoney", "1H")
                    End If
                    lbl2HBetAwaySpread.Text = oAgentPosition.Bet2HSpreadAway
                    lblRiskAmount2HAwaySpread.Text = oAgentPosition.RiskAmount2HSpreadAway
                    If oAgentPosition.RiskAmount2HSpreadAway > 0 Then
                        setLinkOpenBet(lblRiskAmount2HAwaySpread, oAgentPosition, "Spread", "AwaySpreadMoney", "2H")
                    End If
                    lbl2HBetHomeSpread.Text = oAgentPosition.Bet2HSpreadHome
                    lblRiskAmount2HHomeSpread.Text = oAgentPosition.RiskAmount2HSpreadHome
                    If oAgentPosition.RiskAmount2HSpreadHome > 0 Then
                        setLinkOpenBet(lblRiskAmount2HHomeSpread, oAgentPosition, "Spread", "HomeSpreadMoney", "2H")
                    End If
                    lbl2HBetAwayML.Text = oAgentPosition.Bet2HMLAway
                    lblRiskAmount2HAwayML.Text = oAgentPosition.RiskAmount2HMLAway
                    If oAgentPosition.RiskAmount2HMLAway > 0 Then
                        setLinkOpenBet(lblRiskAmount2HAwayML, oAgentPosition, "MoneyLine", "AwayMoneyLine", "2H")
                    End If
                    lbl2HBetHomeML.Text = oAgentPosition.Bet2HMLHome
                    lblRiskAmount2HHomeML.Text = oAgentPosition.RiskAmount2HMLHome
                    If oAgentPosition.RiskAmount2HMLHome > 0 Then
                        setLinkOpenBet(lblRiskAmount2HHomeML, oAgentPosition, "MoneyLine", "HomeMoneyLine", "2H")
                    End If
                    lbl2HBetAwayTotalPoint.Text = oAgentPosition.Bet2HTotalPointAway
                    lblRiskAmount2HTotalPointOver.Text = oAgentPosition.RiskAmount2HTotalPointAway
                    If oAgentPosition.RiskAmount2HTotalPointAway > 0 Then
                        setLinkOpenBet(lblRiskAmount2HTotalPointOver, oAgentPosition, "TotalPoints", "TotalPointsOverMoney", "2H")
                    End If
                    lbl2HBetHomeTotalPoint.Text = oAgentPosition.Bet2HTotalPointHome
                    lblRiskAmount2HTotalPointUnder.Text = oAgentPosition.RiskAmount2HTotalPointHome
                    If oAgentPosition.RiskAmount2HTotalPointHome > 0 Then
                        setLinkOpenBet(lblRiskAmount2HTotalPointUnder, oAgentPosition, "TotalPoints", "TotalPointsUnderMoney", "2H")
                    End If
                End If

            End If
            If e.Item.ItemType = ListItemType.Header Then
                Dim lblRiskSpread, lblRiskML, lblRiskTotal, lblRiskSpread1H, lblRiskML1H, lblRiskTotal1H, lblRiskSpread2H, lblRiskML2H, lblRiskTotal2H As Label
                lblRiskSpread = e.Item.FindControl("lblRiskSpread")
                lblRiskML = e.Item.FindControl("lblRiskML")
                lblRiskTotal = e.Item.FindControl("lblRiskTotal")
                lblRiskSpread1H = e.Item.FindControl("lblRiskSpread1H")
                lblRiskML1H = e.Item.FindControl("lblRiskML1H")
                lblRiskTotal1H = e.Item.FindControl("lblRiskTotal1H")
                lblRiskSpread2H = e.Item.FindControl("lblRiskSpread2H")
                lblRiskML2H = e.Item.FindControl("lblRiskML2H")
                lblRiskTotal2H = e.Item.FindControl("lblRiskTotal2H")
                If rdWin.Checked Then
                    lblRiskSpread.Text = "Win"
                    lblRiskML.Text = "Win"
                    lblRiskTotal.Text = "Win"
                    lblRiskSpread1H.Text = "Win"
                    lblRiskML1H.Text = "Win"
                    lblRiskTotal1H.Text = "Win"
                    lblRiskSpread2H.Text = "Win"
                    lblRiskML2H.Text = "Win"
                    lblRiskTotal2H.Text = "Win"
                Else
                    lblRiskSpread.Text = "Risk"
                    lblRiskML.Text = "Risk"
                    lblRiskTotal.Text = "Risk"
                    lblRiskSpread1H.Text = "Risk"
                    lblRiskML1H.Text = "Risk"
                    lblRiskTotal1H.Text = "Risk"
                    lblRiskSpread2H.Text = "Risk"
                    lblRiskML2H.Text = "Risk"
                    lblRiskTotal2H.Text = "Risk"
                End If
            End If
            ''hidden 1h,2h

            If e.Item.ItemType = ListItemType.Header Then
                Dim td1HSpread, td1HML, td1HTotal, td1HSpreadBet, td1HSpreadRisk, td1HMLBet, td1HMLRisk, td1HTotalBet, td1HTotalRisk As HtmlControls.HtmlTableCell
                td1HSpread = e.Item.FindControl("td1HSpread")
                td1HML = e.Item.FindControl("td1HML")
                td1HTotal = e.Item.FindControl("td1HTotal")
                td1HSpreadBet = e.Item.FindControl("td1HSpreadBet")
                td1HMLBet = e.Item.FindControl("td1HMLBet")
                td1HMLRisk = e.Item.FindControl("td1HMLRisk")
                td1HTotalBet = e.Item.FindControl("td1HTotalBet")
                td1HTotalRisk = e.Item.FindControl("td1HTotalRisk")
                td1HSpreadRisk = e.Item.FindControl("td1HSpreadRisk")
                If (Not bShow1H) Then
                    td1HSpread.Visible = False
                    td1HML.Visible = False
                    td1HTotal.Visible = False
                    td1HSpreadBet.Visible = False
                    td1HSpreadRisk.Visible = False
                    td1HMLBet.Visible = False
                    td1HMLRisk.Visible = False
                    td1HTotalBet.Visible = False
                    td1HTotalRisk.Visible = False
                End If
                Dim td2HSpread, td2HML, td2HTotal, td2HSpreadBet, td2HSpreadRisk, td2HMLBet, td2HMLRisk, td2HTotalBet, td2HTotalRisk As HtmlControls.HtmlTableCell
                td2HSpread = e.Item.FindControl("td2HSpread")
                td2HML = e.Item.FindControl("td2HML")
                td2HTotal = e.Item.FindControl("td2HTotal")
                td2HSpreadBet = e.Item.FindControl("td2HSpreadBet")
                td2HMLBet = e.Item.FindControl("td2HMLBet")
                td2HMLRisk = e.Item.FindControl("td2HMLRisk")
                td2HTotalBet = e.Item.FindControl("td2HTotalBet")
                td2HTotalRisk = e.Item.FindControl("td2HTotalRisk")
                td2HSpreadRisk = e.Item.FindControl("td2HSpreadRisk")
                If (Not bShow2H) Then
                    td2HSpread.Visible = False
                    td2HML.Visible = False
                    td2HTotal.Visible = False
                    td2HSpreadBet.Visible = False
                    td2HSpreadRisk.Visible = False
                    td2HMLBet.Visible = False
                    td2HMLRisk.Visible = False
                    td2HTotalBet.Visible = False
                    td2HTotalRisk.Visible = False
                End If

            End If

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then


                Dim td1HSpreadBetAway, td1HSpreadRiskAway, td1HMLBetAway, td1HMLRiskAway, td1HTotalBetAway, td1HTotalRiskAway As HtmlControls.HtmlTableCell
                Dim td1HSpreadBetHome, td1HSpreadRiskHome, td1HMLBetHome, td1HMLRiskHome, td1HTotalBetHome, td1HTotalRiskHome As HtmlControls.HtmlTableCell


                td1HSpreadBetAway = e.Item.FindControl("td1HSpreadBetAway")
                td1HSpreadRiskAway = e.Item.FindControl("td1HSpreadRiskAway")
                td1HMLBetAway = e.Item.FindControl("td1HMLBetAway")
                td1HMLRiskAway = e.Item.FindControl("td1HMLRiskAway")
                td1HTotalBetAway = e.Item.FindControl("td1HTotalBetAway")
                td1HTotalRiskAway = e.Item.FindControl("td1HTotalRiskAway")
                td1HSpreadBetHome = e.Item.FindControl("td1HSpreadBetHome")
                td1HSpreadRiskHome = e.Item.FindControl("td1HSpreadRiskHome")
                td1HMLBetHome = e.Item.FindControl("td1HMLBetHome")
                td1HMLRiskHome = e.Item.FindControl("td1HMLRiskHome")
                td1HTotalBetHome = e.Item.FindControl("td1HTotalBetHome")
                td1HTotalRiskHome = e.Item.FindControl("td1HTotalRiskHome")


                If (Not bShow1H) Then


                    td1HSpreadBetAway.Visible = False
                    td1HSpreadRiskAway.Visible = False
                    td1HMLBetAway.Visible = False
                    td1HMLRiskAway.Visible = False
                    td1HTotalBetAway.Visible = False
                    td1HTotalRiskAway.Visible = False
                    td1HSpreadBetHome.Visible = False
                    td1HSpreadRiskHome.Visible = False
                    td1HMLBetHome.Visible = False
                    td1HMLRiskHome.Visible = False
                    td1HTotalBetHome.Visible = False
                    td1HTotalRiskHome.Visible = False

                End If

                Dim td2HSpreadBetAway, td2HSpreadRiskAway, td2HMLBetAway, td2HMLRiskAway, td2HTotalBetAway, td2HTotalRiskAway As HtmlControls.HtmlTableCell
                Dim td2HSpreadBetHome, td2HSpreadRiskHome, td2HMLBetHome, td2HMLRiskHome, td2HTotalBetHome, td2HTotalRiskHome As HtmlControls.HtmlTableCell


                td2HSpreadBetAway = e.Item.FindControl("td2HSpreadBetAway")
                td2HSpreadRiskAway = e.Item.FindControl("td2HSpreadRiskAway")
                td2HMLBetAway = e.Item.FindControl("td2HMLBetAway")
                td2HMLRiskAway = e.Item.FindControl("td2HMLRiskAway")
                td2HTotalBetAway = e.Item.FindControl("td2HTotalBetAway")
                td2HTotalRiskAway = e.Item.FindControl("td2HTotalRiskAway")
                td2HSpreadBetHome = e.Item.FindControl("td2HSpreadBetHome")
                td2HSpreadRiskHome = e.Item.FindControl("td2HSpreadRiskHome")
                td2HMLBetHome = e.Item.FindControl("td2HMLBetHome")
                td2HMLRiskHome = e.Item.FindControl("td2HMLRiskHome")
                td2HTotalBetHome = e.Item.FindControl("td2HTotalBetHome")
                td2HTotalRiskHome = e.Item.FindControl("td2HTotalRiskHome")


                If (Not bShow2H) Then
                    td2HSpreadBetAway.Visible = False
                    td2HSpreadRiskAway.Visible = False
                    td2HMLBetAway.Visible = False
                    td2HMLRiskAway.Visible = False
                    td2HTotalBetAway.Visible = False
                    td2HTotalRiskAway.Visible = False
                    td2HSpreadBetHome.Visible = False
                    td2HSpreadRiskHome.Visible = False
                    td2HMLBetHome.Visible = False
                    td2HMLRiskHome.Visible = False
                    td2HTotalBetHome.Visible = False
                    td2HTotalRiskHome.Visible = False

                End If
            End If

        End Sub

        Protected Sub rptPortType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPortType.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                bShow1H = False
                bShow2H = False
                BindAgentPosition(CType(e.Item.FindControl("rptOpenBet"), Repeater), SafeString(e.Item.DataItem), CType(e.Item.FindControl("lblGameType"), Label))
            End If
        End Sub

        Protected Sub btnViewPosition_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewPosition.Click
            BindSportType()
            getTotal()
        End Sub

        Private Sub getTotal()
            If rdRisk.Checked Then
                lblAmountRiskWin.Text = "Amount (risk)"
                lblAmount.Text = SafeString(SafeRound(Risk))
            Else
                lblAmountRiskWin.Text = "Amount (win)"
                lblAmount.Text = SafeString(SafeRound(Win))
            End If
            lblBet.Text = Bet
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            BindSportType()
            getTotal()
        End Sub
    End Class
End Namespace