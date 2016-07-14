Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports System.Xml

Namespace SBCWebsite
    Partial Class WeeklyAllAgent
        Inherits SBCBL.UI.CSBCUserControl

#Region "Properties"
        Property SuperID() As String
            Get
                Return SafeString(ViewState("_SUPER_ID"))
            End Get
            Set(ByVal value As String)
                ViewState("_SUPER_ID") = value
            End Set
        End Property

        Property ThisWeek() As Double
            Get
                Return SafeDouble(ViewState("ThisWeek"))
            End Get
            Set(ByVal value As Double)
                ViewState("ThisWeek") = value
            End Set
        End Property

        Property Mon() As Double
            Get
                Return SafeDouble(ViewState("Mon"))
            End Get
            Set(ByVal value As Double)
                ViewState("Mon") = value
            End Set
        End Property

        Property Tue() As Double
            Get
                Return SafeDouble(ViewState("Tue"))
            End Get
            Set(ByVal value As Double)
                ViewState("Tue") = value
            End Set
        End Property

        Property Wed() As Double
            Get
                Return SafeDouble(ViewState("Wed"))
            End Get
            Set(ByVal value As Double)
                ViewState("Wed") = value
            End Set
        End Property

        Property Thu() As Double
            Get
                Return SafeDouble(ViewState("Thu"))
            End Get
            Set(ByVal value As Double)
                ViewState("Thu") = value
            End Set
        End Property

        Property Fri() As Double
            Get
                Return SafeDouble(ViewState("Fri"))
            End Get
            Set(ByVal value As Double)
                ViewState("Fri") = value
            End Set
        End Property

        Property Sat() As Double
            Get
                Return SafeDouble(ViewState("Sat"))
            End Get
            Set(ByVal value As Double)
                ViewState("Sat") = value
            End Set
        End Property
        Property Sun() As Double
            Get
                Return SafeDouble(ViewState("Sun"))
            End Get
            Set(ByVal value As Double)
                ViewState("Sun") = value
            End Set
        End Property

        Property NumPlayer() As Integer
            Get
                Return SafeInteger(ViewState("NumPlayer"))
            End Get
            Set(ByVal value As Integer)
                ViewState("NumPlayer") = value
            End Set
        End Property

        ReadOnly Property AgentID() As String
            Get
                Return SafeString(UserSession.AgentSelectID)
            End Get
            'Set(ByVal value As String)
            '    ViewState("_AGENT_ID") = value
            'End Set
        End Property

        Public Property Title() As String
            Get
                Return SafeString(ViewState("_TITLE"))
            End Get
            Set(ByVal value As String)
                ViewState("_TITLE") = value
            End Set
        End Property

        'Public Property LabelSelectedUser() As String
        '    Get
        '        Return lblUsers.Text
        '    End Get
        '    Set(ByVal value As String)
        '        lblUsers.Text = value
        '    End Set
        'End Property

        Public Property ShowAgentList() As Boolean
            Get
                Return SafeBoolean(ViewState("_SHOW_AGENT_LIST"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_SHOW_AGENT_LIST") = value
            End Set
        End Property

        Public Property ShowWeekList() As Boolean
            Get
                Return SafeBoolean(ViewState("_SHOW_WEEK_LIST"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_SHOW_WEEK_LIST") = value
            End Set
        End Property

        Public Property HistoryPage() As String
            Get
                Return SafeString(ViewState("_HISTORY_PAGE"))
            End Get
            Set(ByVal value As String)
                ViewState("_HISTORY_PAGE") = value
            End Set
        End Property

        Public Property PendingPage() As String
            Get
                Return SafeString(ViewState("_PENDING_PAGE"))
            End Get
            Set(ByVal value As String)
                ViewState("_PENDING_PAGE") = value
            End Set
        End Property

        Public ReadOnly Property SelectedAgent() As String
            Get
                Return UserSession.AgentSelectID
            End Get
        End Property

        'Public Property SelectedWeek() As String
        '    Get
        '        Return ddlWeeks.Value
        '    End Get
        '    Set(ByVal value As String)
        '        ddlWeeks.Value = value

        '        BindData()
        '    End Set
        'End Property
#End Region

#Region "Bind Data"


        Private Sub bindWeek()
            Dim nTimeZome As Integer = 0
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                nTimeZome = UserSession.SuperAdminInfo.TimeZone
            ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                nTimeZome = UserSession.AgentUserInfo.TimeZone
            End If
            Dim oDate As Date = GetEasternMondayOfCurrentWeek()
            Dim olstWeek As New Dictionary(Of String, String)

            For nIndex As Integer = 1 To 8
                Dim oTemp As Date = oDate.AddDays((nIndex - 1) * -7)
                olstWeek.Add(oTemp.ToString("MM/dd/yyyy") & " - " & oTemp.AddDays(6).ToString("MM/dd/yyyy"), oTemp.ToString("MM/dd/yyyy"))
            Next

            ddlWeeks.DataSource = olstWeek
            ddlWeeks.DataTextField = "Key"
            ddlWeeks.DataValueField = "value"
            ddlWeeks.DataBind()
        End Sub

        Public Sub ReloadData()
            'trWeek.Visible = ShowWeekList
            bindWeek()

            'If ShowAgentList Then
            '    Dim oAgentManager As New CAgentManager()
            '    If SuperID <> "" OrElse oAgentManager.NumOfSubAgents(AgentID) > 0 Then
            '    End If
            'End If

            BindData()
        End Sub

        Public Sub BindData()
            'Dim sAgentID As String = ""
            'If (UserSession.UserType = SBCBL.EUserType.SuperAdmin OrElse UserSession.UserType = SBCBL.EUserType.Agent) AndAlso ddlAgents.Value <> "" Then
            '    sAgentID = ddlAgents.Value
            'Else
            '    sAgentID = AgentID
            'End If
            ThisWeek = 0
            NumPlayer = 0
            Mon = 0
            Tue = 0
            Wed = 0
            Thu = 0
            Fri = 0
            Sat = 0
            Sun = 0
            Dim oAgentMamanager As New CAgentManager()
            'Dim oTable As DataTable
            'oTable = oAgentMamanager.GetPlayersDashboard(AgentID, SafeDate(GetEasternMondayOfCurrentWeek()))
            Dim oTable As New DataTable
            Dim listWeekly As New List(Of DataRow)
            Dim nPending As Double = 0
            Dim nPL As Double = 0
            Dim nNet As Double = 0
            Dim odtAgent = (New CAgentManager).GetAllAgentSubAgentsByAgent(UserSession.AgentSelectID, Nothing)
            For Each odr As DataRow In odtAgent.Rows
                oTable = oAgentMamanager.GetPlayersDashboard(SafeString(odr("AgentID")), SafeDate(ddlWeeks.Value))
                Dim odrTotalbalance = oTable.Rows(oTable.Rows.Count - 1)
                Dim oAgent = UserSession.Cache.GetAgentInfo(SafeString(odr("AgentID")))
                odrTotalbalance("Player") = oAgent.Login & "|" & oAgent.UserID
                listWeekly.Add(odrTotalbalance)
                nPending += SafeDouble(odrTotalbalance("Pending"))
                nPL += SafeDouble(odrTotalbalance("PL"))
                nNet += SafeDouble(odrTotalbalance("Net"))
            Next
            Dim odrFooter = oTable.NewRow
            ' odrFooter("Player") = "Total " & listWeekly.Count & " Agent" & "|" & UserSession.AgentSelectID
            odrFooter("Player") = "Total " & "|" & UserSession.AgentSelectID
            odrFooter("Pending") = nPending
            odrFooter("PL") = nPL
            odrFooter("Net") = nNet
            listWeekly.Add(odrFooter)
            dgSubPlayers.DataSource = listWeekly
            If listWeekly.Count > 1 Then
                ' dgSubPlayers.Visible = True
                trChart.Visible = True

                dgSubPlayers.DataBind()
                'lblXML1.Text = BuildChartXML(oTable)
            Else
                dgSubPlayers.Visible = False
                trChart.Visible = False
                Title = ""
            End If
        End Sub
#End Region

        Protected Sub dgSubPlayers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgSubPlayers.ItemDataBound
            If e.Item.ItemType = ListItemType.Header Then
                e.Item.Cells(1).Text = "Mon <br/>" & getDay(0)
                e.Item.Cells(2).Text = "Tue <br/>" & getDay(1)
                e.Item.Cells(3).Text = "Wed <br/>" & getDay(2)
                e.Item.Cells(4).Text = "Thu <br/>" & getDay(3)
                e.Item.Cells(5).Text = "Fri <br/>" & getDay(4)
                e.Item.Cells(6).Text = "Sat <br/>" & getDay(5)
                e.Item.Cells(7).Text = "Sun <br/>" & getDay(6)
            End If
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim nTimeZome As Integer = 0
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    nTimeZome = UserSession.SuperAdminInfo.TimeZone
                ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                    nTimeZome = UserSession.AgentUserInfo.TimeZone
                End If

                Dim oData As DataRow = CType(e.Item.DataItem, DataRow)
                ThisWeek += (SBCBL.std.SafeDouble(oData("Mon")) + SBCBL.std.SafeDouble(oData("Tues")) + SBCBL.std.SafeDouble(oData("Wed")) + SBCBL.std.SafeDouble(oData("Thurs")) + SBCBL.std.SafeDouble(oData("Fri")) + SBCBL.std.SafeDouble(oData("Sat")) + SBCBL.std.SafeDouble(oData("Sun")))
                If SafeString(oData("Player")).Contains("Total") Then
                    'oData("Player") = "Totals " & (e.Item.ItemIndex) & "Player"
                    CType(e.Item.FindControl("lblTotalBalance"), Label).Text = FormatNumber(ThisWeek, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lblThisWeek"), Label).Text = FormatNumber(ThisWeek, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lblThisWeek"), Label).ForeColor = Drawing.Color.White
                    For Each oColumn As TableCell In e.Item.Cells
                        oColumn.ForeColor = Drawing.Color.White
                        oColumn.BackColor = Drawing.ColorTranslator.FromHtml("#A50502")
                    Next
                    Dim lblCountHead = CType(e.Item.FindControl("lblCountHead"), Label)
                    lblCountHead.Text = NumPlayer
                    lblCountHead.ForeColor = Drawing.Color.White
                    CType(e.Item.FindControl("lbtMon"), LinkButton).Text = FormatNumber(Mon, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lbtTues"), LinkButton).Text = FormatNumber(Tue, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lbtWed"), LinkButton).Text = FormatNumber(Wed, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lbtThurs"), LinkButton).Text = FormatNumber(Thu, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lbtFri"), LinkButton).Text = FormatNumber(Fri, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lbtSat"), LinkButton).Text = FormatNumber(Sat, SBCBL.std.GetRoundMidPoint)
                    CType(e.Item.FindControl("lbtSun"), LinkButton).Text = FormatNumber(Sun, SBCBL.std.GetRoundMidPoint)
                Else
                    'CType(e.Item.FindControl("lblMon"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblTues"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblWed"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblThurs"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblFri"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblSat"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblSun"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblGross"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblNet"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblPL"), Label).ForeColor = Drawing.Color.Black
                    CType(e.Item.FindControl("lblPending"), Label).ForeColor = Drawing.Color.Black
                    CType(e.Item.FindControl("lblTotalBalance"), Label).ForeColor = Drawing.Color.Black
                    CType(e.Item.FindControl("lblCasino"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblMon"), Label).ForeColor = Drawing.Color.Black

                End If

                '  CType(e.Item.FindControl("hlCreditBack"), HyperLink).NavigateUrl = "javascript:function void(){};"
                ' CType(e.Item.FindControl("hlCreditBack"), HyperLink).Attributes("onclick") = "window.open('CreditBack.aspx?player=" & SafeString(oData("Player")) & _
                '                                      "&playerid=" & SafeString(oData("PlayerID")) & "','creditback','menubar=0,resizable=0,width=300,height=200'); return false;"
                'CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = SafeString(oData("Player")) <> "Totals" AndAlso UserSession.UserType = SBCBL.EUserType.SuperAdmin

                'CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).NavigateUrl = "javascript:function void(){};"
                'CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Attributes("onclick") = "openModalDialog('/" & SBCBL.std.GetSiteType & "/" & IIf(UserSession.UserType = SBCBL.EUserType.SuperAdmin, "SuperAdmins", "Agents/Management") & "/HistoricalAmount.aspx?playerID=" & SafeString(oData("PlayerID")) & "',250, 600, false,false,false); return false;"
                'CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Visible = SafeString(oData("Player")) <> "Totals"


                If Not SafeString(oData("Player")).Contains("Total") Then
                    Dim sLink As String
                    Dim oDate As Date
                    ''in super page (in ucSubPlayerBalanceReport) trweek is invisible so date is wrong when send url
                    'If trWeek.Visible AndAlso ddlWeeks.Value <> "" Then

                    oDate = GetMondayOfCurrentWeek(nTimeZome)
                    '' Set font color for positive and negative amount
                    'Monday
                    Dim lbtMon As LinkButton = CType(e.Item.FindControl("lbtMon"), LinkButton)
                    If SafeDouble(oData("Mon")) < 0 Then
                        lbtMon.ForeColor = Drawing.Color.Red
                    Else
                        lbtMon.CssClass = ""
                    End If
                    Mon += SafeDouble(oData("Mon"))
                    'Tuesday
                    Dim lbtTues As LinkButton = CType(e.Item.FindControl("lbtTues"), LinkButton)
                    If SafeDouble(oData("Tues")) < 0 Then
                        lbtTues.ForeColor = Drawing.Color.Red
                    Else
                        lbtTues.CssClass = ""
                    End If
                    Tue += SafeDouble(oData("Tues"))
                    'Wednesday
                    Dim lbtWed As LinkButton = CType(e.Item.FindControl("lbtWed"), LinkButton)
                    If SafeDouble(oData("Wed")) < 0 Then
                        lbtWed.ForeColor = Drawing.Color.Red
                    Else
                        lbtWed.CssClass = ""
                    End If
                    Wed += SafeDouble(oData("Wed"))
                    'Thursday
                    Dim lbtThurs As LinkButton = CType(e.Item.FindControl("lbtThurs"), LinkButton)
                    If SafeDouble(oData("Thurs")) < 0 Then
                        lbtThurs.ForeColor = Drawing.Color.Red
                    Else
                        lbtThurs.CssClass = ""
                    End If
                    Thu += SafeDouble(oData("Thurs"))
                    'Friday
                    Dim lbtFri As LinkButton = CType(e.Item.FindControl("lbtFri"), LinkButton)
                    If SafeDouble(oData("Fri")) < 0 Then
                        lbtFri.ForeColor = Drawing.Color.Red
                    Else
                        lbtFri.CssClass = ""
                    End If
                    Fri += SafeDouble(oData("Fri"))
                    'Saturday
                    Dim lbtSat As LinkButton = CType(e.Item.FindControl("lbtSat"), LinkButton)
                    If SafeDouble(oData("Sat")) < 0 Then
                        lbtSat.ForeColor = Drawing.Color.Red
                    Else
                        lbtSat.CssClass = ""
                    End If
                    Sat += SafeDouble(oData("Sat"))
                    'Sunday
                    Dim lbtSun As LinkButton = CType(e.Item.FindControl("lbtSun"), LinkButton)
                    If SafeDouble(oData("Sun")) < 0 Then
                        lbtSun.ForeColor = Drawing.Color.Red
                    Else
                        lbtSun.CssClass = ""
                    End If
                    Sun += SafeDouble(oData("Sun"))
                    'count head
                    Dim lblCountHead = CType(e.Item.FindControl("lblCountHead"), Label)
                    'Dim nNumPlayer = (New CPlayerManager()).getNumPlayerByAgentID(SafeString(oData("Player")).Split("|")(1))
                    ' NumPlayer += nNumPlayer
                    Dim tblActivePlayers = (New CAgentManager()).GetActivePlayersForAgent(SafeString(SafeString(oData("Player")).Split("|")(1)), ddlWeeks.Value, _
                                                               SafeDate(ddlWeeks.Value).AddDays(6))
                    lblCountHead.Text = tblActivePlayers.Rows.Count
                    NumPlayer += tblActivePlayers.Rows.Count
                    'Gross
                    'Dim lblGross As Label = CType(e.Item.FindControl("lblGross"), Label)
                    'If SafeDouble(oData("Gross")) < 0 Then
                    '    lblGross.ForeColor = Drawing.Color.Red
                    'Else
                    '    lblGross.CssClass = ""
                    'End If

                    'Net
                    'Dim lblNet As Label = CType(e.Item.FindControl("lblNet"), Label)
                    'If SafeDouble(oData("Net")) < 0 Then
                    '    lblNet.ForeColor = Drawing.Color.Red
                    'Else
                    '    lblNet.CssClass = ""
                    'End If

                    'PL
                    'Dim lblPL As Label = CType(e.Item.FindControl("lblPL"), Label)
                    'If SafeDouble(oData("PL")) < 0 Then
                    '    lblPL.ForeColor = Drawing.Color.Red
                    'Else
                    '    lblPL.CssClass = ""
                    'End If

                    'Pending
                    ' Dim lblPending As Label = CType(e.Item.FindControl("lblPending"), Label)
                    'If SafeDouble(oData("Pending")) < 0 Then

                    '    lblPending.ForeColor = Drawing.Color.Red

                    'Else

                    '    lblPending.ForeColor = Drawing.Color.Black
                    'End If

                    '' Set href URL
                    'If HistoryPage <> "" Then
                    '    sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}", HistoryPage, IIf(String.IsNullOrEmpty(AgentID), SafeString(AgentID), SafeString(AgentID)), _
                    '                          SafeString(oData("PlayerID")))


                    '    'Dim lblPlayer As Label = CType(e.Item.FindControl("lblPlayer"), Label)
                    '    'lblPlayer.CssClass = "hyperlink"
                    '    'lblPlayer.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}';", _
                    '    '                                       Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))

                    '    'lbtMon.CssClass = "hyperlink " & lbtMon.CssClass
                    '    'lbtMon.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                       Format(oDate, "MM/dd/yyyy")))

                    '    'lbtTues.CssClass = "hyperlink " & lbtTues.CssClass
                    '    'lbtTues.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                       Format(oDate.AddDays(1), "MM/dd/yyyy")))
                    '    'If SafeDouble(oData("Tues")) < 0 Then
                    '    '    lbtTues.ForeColor = Drawing.Color.Red
                    '    'End If


                    '    'lbtWed.CssClass = "hyperlink " & lbtWed.CssClass

                    '    'lblWed.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                        Format(oDate.AddDays(2), "MM/dd/yyyy")))
                    '    'If SafeDouble(oData("Wed")) < 0 Then
                    '    '    lblWed.ForeColor = Drawing.Color.Red
                    '    'End If

                    '    'lblThurs.CssClass = "hyperlink " & lblThurs.CssClass

                    '    'lblThurs.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                       Format(oDate.AddDays(3), "MM/dd/yyyy")))

                    '    'If SafeDouble(oData("Thurs")) < 0 Then
                    '    '    lblThurs.ForeColor = Drawing.Color.Red
                    '    'End If

                    '    'lblFri.CssClass = "hyperlink " & lblFri.CssClass

                    '    'lblFri.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                       Format(oDate.AddDays(4), "MM/dd/yyyy")))
                    '    'If SafeDouble(oData("Fri")) < 0 Then
                    '    '    lblFri.ForeColor = Drawing.Color.Red
                    '    'End If

                    '    'lblSat.CssClass = "hyperlink " & lblSat.CssClass

                    '    'lblSat.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                       Format(oDate.AddDays(5), "MM/dd/yyyy")))
                    '    'If SafeDouble(oData("Sat")) < 0 Then
                    '    '    lblSat.ForeColor = Drawing.Color.Red
                    '    'End If

                    '    'lblSun.CssClass = "hyperlink " & lblSun.CssClass

                    '    'lblSun.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                    '    '                                       Format(oDate.AddDays(6), "MM/dd/yyyy")))
                    '    'If SafeDouble(oData("Sun")) < 0 Then
                    '    '    lblSun.ForeColor = Drawing.Color.Red
                    '    'End If

                    '    'lblGross.CssClass = "hyperlink " & lblGross.CssClass

                    '    'lblGross.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}';", _
                    '    '                                       Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))
                    '    'If SafeDouble(oData("Gross")) < 0 Then
                    '    '    lblGross.ForeColor = Drawing.Color.Red
                    '    'End If

                    'End If

                    'If PendingPage <> "" Then
                    '    sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}", PendingPage, SafeString(AgentID), _
                    '                          SafeString(oData("PlayerID")))

                    '    If SafeDouble(oData("Pending")) <> 0 Then
                    '        lblPending.CssClass = "hyperlink " & lblPending.CssClass

                    '        lblPending.Attributes.Add("onClick", _
                    '                                 String.Format(sLink & "&SDate={0}&EDate={1}';", _
                    '                                               Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))
                    '    End If

                    'End If
                End If
            End If
        End Sub

        'Private Function BuildChartXML(ByVal poTable As DataTable) As String
        '    Dim oChart As XElement = <chart lineThickness="1" showValues="0" formatNumberScale="1" anchorRadius="2"
        '                                 divLineAlpha="20" divLineIsDashed="1" showAlternateHGridColor="1" shadowAlpha="40"
        '                                 labelStep="2" numvdivlines="5" chartRightMargin="35" bgAngle="270" bgAlpha="10,10"/>

        '    '' Bind Categories
        '    Dim oCategories As XElement = <categories/>
        '    oChart.Add(oCategories)

        '    For Each oClm As DataColumn In poTable.Columns
        '        If oClm.ColumnName = "Player" OrElse oClm.ColumnName = "Net" _
        '        OrElse oClm.ColumnName = "PlayerID" OrElse oClm.ColumnName = "Pending" _
        '        OrElse oClm.ColumnName = "Gross" OrElse oClm.ColumnName = "PL" Then
        '            Continue For
        '        End If

        '        oCategories.Add(<category label=<%= oClm.ColumnName %>/>)
        '    Next

        '    '' Bind data
        '    For Each oRow As DataRow In poTable.Rows
        '        If oRow("Player") = "Totals" Then
        '            Continue For
        '        End If

        '        Dim oDataset As XElement = <dataset seriesName=<%= SafeString(oRow("Player")) %>/>
        '        oChart.Add(oDataset)

        '        For Each oClm As DataColumn In poTable.Columns
        '            If oClm.ColumnName = "Player" OrElse oClm.ColumnName = "Net" _
        '            OrElse oClm.ColumnName = "PlayerID" OrElse oClm.ColumnName = "Pending" _
        '            OrElse oClm.ColumnName = "Gross" Then
        '                Continue For
        '            End If

        '            oDataset.Add(<set value=<%= SafeString(oRow(oClm.ColumnName)) %>/>)
        '        Next

        '    Next

        '    Return oChart.ToString().Replace("'", "&rsquo;").Replace("""", "'")
        'End Function

        Public Function getDay(ByVal nday As Integer) As String
            Dim oDate As Date = SafeDate(ddlWeeks.Value)

            Return oDate.AddDays(nday).ToString("MM-dd")
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                lblAgent.Text = UserSession.Cache.GetAgentInfo(UserSession.AgentSelectID).Login
            End If
        End Sub

        Protected Sub btnSeach_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSeach.Click
            BindData()
        End Sub

        Protected Sub changeAgent_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            '  Session("AGENT_SELECT_ID") = CType(sender, LinkButton).CommandArgument
            Response.Redirect("/SBS/Agents/Management/PlayersReports.aspx")
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If GetSiteType.Equals("SBS") Then
                trChart.Visible = False
            End If
        End Sub

        Protected Sub btnPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrint.Click
            Response.Redirect("PrintWeeklyBalance.aspx?Date=" & ddlWeeks.SelectedValue)
        End Sub
    End Class
End Namespace