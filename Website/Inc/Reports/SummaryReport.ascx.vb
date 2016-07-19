Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports System.Xml

Namespace SBCWebsite
    Partial Class SummaryReport
        Inherits SBCBL.UI.CSBCUserControl
        Private nNumPlayer As Integer = 0
        Private nAllPlayer As Integer = 0
        Public Event OnChange()

#Region "Properties"
        Property SuperID() As String
            Get
                Return SafeString(ViewState("_SUPER_ID"))
            End Get
            Set(ByVal value As String)
                ViewState("_SUPER_ID") = value
            End Set
        End Property

        Property AgentID() As String
            Get
                Return SafeString(ViewState("_AGENT_ID"))
            End Get
            Set(ByVal value As String)
                ViewState("_AGENT_ID") = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return SafeString(ViewState("_TITLE"))
            End Get
            Set(ByVal value As String)
                ViewState("_TITLE") = value
            End Set
        End Property

        Public Property Pending() As Double
            Get
                Return SafeDouble(ViewState("PENDING"))
            End Get
            Set(ByVal value As Double)
                ViewState("PENDING") = value
            End Set
        End Property

        Property FromDate() As Date
            Get
                Return SafeDate(ViewState("FromDate"))
            End Get
            Set(ByVal value As Date)
                ViewState("FromDate") = value
            End Set
        End Property

        Property ToDate() As Date
            Get
                Return SafeDate(ViewState("ToDate"))
            End Get
            Set(ByVal value As Date)
                ViewState("ToDate") = value
            End Set
        End Property

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

        '        ' BindData()
        '    End Set
        'End Property
#End Region

#Region "Bind Data"


        Public Sub bindWeek()
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

            If FromDate <> Date.MinValue Then
                ddlWeeks.SelectedValue = FromDate.ToString("MM/dd/yyyy")
            End If
        End Sub

        Public Sub ReloadData()
            'trWeek.Visible = ShowWeekList
            bindWeek()

            'trAgent.Visible = False
            'If ShowAgentList Then
            '    Dim oAgentManager As New CAgentManager()
            '    If SuperID <> "" OrElse oAgentManager.NumOfSubAgents(AgentID) > 0 Then
            '    End If
            'End If
            bindSubAgent()

        End Sub

        Private Sub bindSubAgent()
            Dim odtAgent As DataTable
            'If Not SafeString(UserSession.UserID).Equals(UserSession.AgentSelectID, StringComparison.CurrentCultureIgnoreCase) Then
            'odtAgent = (New CAgentManager).GetAllAgentsAndSubAgentByAgent(UserSession.AgentSelectID, Nothing)
            'Else
            '    odtAgent = (New CAgentManager).GetAllAgentsByAgent(UserSession.AgentSelectID, Nothing)
            'End If
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                If AgentID = "All" Then
                    odtAgent = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
                Else
                    odtAgent = (New CAgentManager).GetAllAgentSubAgentsByAgent(AgentID, Nothing)
                End If

            Else
                odtAgent = (New CAgentManager).GetAllAgentSubAgentsByAgent(UserSession.UserID, Nothing)
            End If

            Dim odr As DataRow = odtAgent.NewRow
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin AndAlso AgentID = "All" Then

                odr("AgentName") = "All"
            Else
                odr("AgentName") = "All"
            End If
            odtAgent.Rows.Add(odr)
            rptMain.DataSource = odtAgent
            rptMain.DataBind()
        End Sub


        Public Sub BindData(ByVal dgSubPlayers As DataGrid, ByVal psAgentID As String)
            'Dim sAgentID As String = ""
            'If (UserSession.UserType = SBCBL.EUserType.SuperAdmin OrElse UserSession.UserType = SBCBL.EUserType.Agent) AndAlso ddlAgents.Value <> "" Then
            '    sAgentID = ddlAgents.Value
            'Else
            '    sAgentID = AgentID
            'End If

            Dim sStartDate As String
            If FromDate = Date.MinValue Then
                sStartDate = SafeDate(ddlWeeks.Value)
            Else
                sStartDate = FromDate
            End If
            'If ddlWeeks.Value <> "" Then
            '    sStartDate = ddlWeeks.Value
            'End If
            nNumPlayer = 0
            Pending = 0
            Dim oAgentMamanager As New CAgentManager()
            Dim oTable As DataTable
            If String.IsNullOrEmpty(psAgentID.Trim) Then
                If Session("Row") IsNot Nothing Then
                    Dim odt1 As DataTable = CType(Session("Row"), DataTable)
                    '  ClientAlert(odt1.Rows(0)("Pending"), True)
                    dgSubPlayers.DataSource = odt1
                    dgSubPlayers.DataBind()
                    Session("Row") = Nothing
                    Return
                End If
            End If
            'ClientAlert(psAgentID)
            If sStartDate <> "" Then
                oTable = oAgentMamanager.GetPlayersDashboard(psAgentID, SafeDate(sStartDate))
            Else
                oTable = oAgentMamanager.GetPlayersDashboard(psAgentID)
            End If
            Dim oDay As String() = {"Playerid", "Player", "Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun", "Gross", "Net", "PL", "Pending"}
            If Session("Row") Is Nothing Then
                Dim odt = New DataTable()
                For Each sDay In oDay
                    odt.Columns.Add(sDay)
                Next
                If oTable Is Nothing OrElse oTable.Rows.Count = 0 Then
                    Return

                End If
                Dim odr As DataRow = odt.NewRow
                For Each sDay In oDay
                    If sDay = "Player" OrElse sDay = "Playerid" Then
                        odr(sDay) = "All" ' SafeDouble(oTable.Compute(sDay, True))
                    Else
                        If oTable.Rows.Count > 0 Then
                            'ClientAlert(sDay & ":" & oTable.Rows(0)(sDay), True)
                            odr(sDay) = SafeDouble(oTable.Compute("SUM(" & sDay & ")", "Playerid<>''"))
                            odr(sDay) = FormatNumber(odr(sDay), SBCBL.std.GetRoundMidPoint)
                        End If

                    End If
                Next
                odt.Rows.Add(odr)
                Session("Row") = odt
            Else
                Dim odt As DataTable = CType(Session("Row"), DataTable)
                Dim odr As DataRow = odt.Rows(0)
                For Each sDay In oDay
                    If Not sDay.Equals("Player") AndAlso Not sDay.Equals("Playerid") Then
                        If oTable.Rows.Count > 0 Then
                            odr(sDay) = SafeDouble(odr(sDay)) + SafeDouble(oTable.Compute("SUM(" & sDay & ")", "Playerid<>''"))
                            odr(sDay) = FormatNumber(odr(sDay), SBCBL.std.GetRoundMidPoint)
                        End If
                    End If
                Next
                Session("Row") = odt
            End If
            dgSubPlayers.DataSource = oTable

            'If oTable.Rows.Count > 1 Then
            dgSubPlayers.Visible = True
            'trChart.Visible = True

            dgSubPlayers.DataBind()
            'lblXML1.Text = BuildChartXML(oTable)
            'Else
            '    dgSubPlayers.Visible = False
            '    trChart.Visible = False
            '    Title = ""
            ' End If
        End Sub
#End Region

        Protected Sub dgSubPlayers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
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

                Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)
                If SafeString(oData("Player")).Contains("All") Then
                    CType(e.Item.FindControl("lblPlayer"), Label).Text = "Total  ( " & nAllPlayer & " Players )"
                    CType(e.Item.FindControl("lblPlayer"), Label).CssClass = "Bold"
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = False
                    CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Visible = False
                End If
                If SafeString(oData("Player")).Contains("Total") Then
                    For Each oColumn As TableCell In e.Item.Cells
                        oColumn.CssClass = "Bold"
                    Next
                    'CType(e.Item.FindControl("lbtEdit"), LinkButton).Visible = False
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = False
                    CType(e.Item.FindControl("lblPending"), Label).Text = FormatNumber(Pending, SBCBL.std.GetRoundMidPoint)
                    If SBCBL.std.SafeDouble(oData("Pending")) = 0 AndAlso (SBCBL.std.SafeDouble(oData("Mon")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Tues")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Wed")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Thurs")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Fri")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Sat")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Sun")) = 0) Then
                        e.Item.Parent.Visible = False
                        e.Item.Parent.Parent.Parent.FindControl("lblAgentWeekly").Visible = False
                        Return
                    End If
                    'CType(e.Item.FindControl("lblThisWeek"), Label).ForeColor = Drawing.Color.White
                    'oData("Player") = "Totals " & (e.Item.ItemIndex) & "Player"
                    nAllPlayer += nNumPlayer
                    CType(e.Item.FindControl("lblPlayer"), Label).Text = "Sub Total  ( " & nNumPlayer & " Players )"
                    'For Each oColumn As TableCell In e.Item.Cells
                    '    oColumn.ForeColor = Drawing.Color.White
                    '    oColumn.BackColor = Drawing.ColorTranslator.FromHtml("#A50502")

                    'Next
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = False
                    CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Visible = False
                Else
                    If SBCBL.std.SafeDouble(oData("Pending")) = 0 AndAlso (SBCBL.std.SafeDouble(oData("Mon")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Tues")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Wed")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Thurs")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Fri")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Sat")) = 0 AndAlso SBCBL.std.SafeDouble(oData("Sun")) = 0) Then
                        e.Item.Visible = False
                        Return
                    Else
                        nNumPlayer += 1
                    End If
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
                    'CType(e.Item.FindControl("lblPending"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblTotalBalance"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblCasino"), Label).ForeColor = Drawing.Color.Black
                    'CType(e.Item.FindControl("lblMon"), Label).ForeColor = Drawing.Color.Black

                End If

             
                If SafeString(oData("Player")) <> "Totals" Andalso SafeString(oData("Player")) <> "All" Then

                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).NavigateUrl = "javascript:function void(){};"
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Attributes("onclick") = "window.open('CreditBack.aspx?player=" & SafeString(oData("Player")) & _
                                                         "&playerid=" & SafeString(oData("PlayerID")) & "','creditback','menubar=0,resizable=0,width=300,height=200'); return false;"
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = SafeString(oData("Player")).Contains("Total") AndAlso UserSession.UserType = SBCBL.EUserType.SuperAdmin
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = SafeString(oData("Player")).Contains("All'") AndAlso UserSession.UserType = SBCBL.EUserType.SuperAdmin

                    CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).NavigateUrl = "javascript:function void(){};"
                    CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Attributes("onclick") = "openModalDialog('/" & SBCBL.std.GetSiteType & "/" & IIf(UserSession.UserType = SBCBL.EUserType.SuperAdmin, "SuperAdmins", "Agents/Management") & "/HistoricalAmount.aspx?playerID=" & SafeString(oData("PlayerID")) & "',250, 600, false,false,false); return false;"
                    CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Visible = Not SafeString(oData("Player")).Contains("Total")
                    '  CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Visible = Not SafeString(oData("Player")).Contains("All")

                    Dim sLink As String
                    Dim oDate As Date

                    ''in super page (in ucSubPlayerBalanceReport) trweek is invisible so date is wrong when send url
                    'If trWeek.Visible AndAlso ddlWeeks.Value <> "" Then
                    'If ddlWeeks.Value <> "" Then
                    '    oDate = SafeDate(ddlWeeks.Value)
                    'Else
                    If FromDate <> Date.MinValue Then
                        oDate = FromDate
                    Else
                        oDate = SafeDate(ddlWeeks.Value)
                    End If

                    'End If

                    '' Set font color for positive and negative amount
                    'Monday
                    Dim lblMon As Label = CType(e.Item.FindControl("lblMon"), Label)
                    If SafeDouble(oData("Mon")) < 0 Then
                        lblMon.ForeColor = Drawing.Color.Red
                    Else
                        lblMon.CssClass = ""
                    End If

                    'Tuesday
                    Dim lblTues As Label = CType(e.Item.FindControl("lblTues"), Label)
                    If SafeDouble(oData("Tues")) < 0 Then
                        lblTues.ForeColor = Drawing.Color.Red
                    Else
                        lblTues.CssClass = ""
                    End If

                    'Wednesday
                    Dim lblWed As Label = CType(e.Item.FindControl("lblWed"), Label)
                    If SafeDouble(oData("Wed")) < 0 Then
                        lblWed.ForeColor = Drawing.Color.Red
                    Else
                        lblWed.CssClass = ""
                    End If

                    'Thursday
                    Dim lblThurs As Label = CType(e.Item.FindControl("lblThurs"), Label)
                    If SafeDouble(oData("Thurs")) < 0 Then
                        lblThurs.ForeColor = Drawing.Color.Red
                    Else
                        lblThurs.CssClass = ""
                    End If

                    'Friday
                    Dim lblFri As Label = CType(e.Item.FindControl("lblFri"), Label)
                    If SafeDouble(oData("Fri")) < 0 Then
                        lblFri.ForeColor = Drawing.Color.Red
                    Else
                        lblFri.CssClass = ""
                    End If

                    'Saturday
                    Dim lblSat As Label = CType(e.Item.FindControl("lblSat"), Label)
                    If SafeDouble(oData("Sat")) < 0 Then
                        lblSat.ForeColor = Drawing.Color.Red
                    Else
                        lblSat.CssClass = ""
                    End If

                    'Sunday
                    Dim lblSun As Label = CType(e.Item.FindControl("lblSun"), Label)
                    If SafeDouble(oData("Sun")) < 0 Then
                        lblSun.ForeColor = Drawing.Color.Red
                    Else
                        lblSun.CssClass = ""
                    End If

                    'Gross
                    Dim lblGross As Label = CType(e.Item.FindControl("lblGross"), Label)
                    If SafeDouble(oData("Gross")) < 0 Then
                        lblGross.ForeColor = Drawing.Color.Red
                    Else
                        lblGross.CssClass = ""
                    End If

                    'Net
                    Dim lblNet As Label = CType(e.Item.FindControl("lblNet"), Label)
                    If SafeDouble(oData("Net")) < 0 Then
                        lblNet.ForeColor = Drawing.Color.Red
                    Else
                        lblNet.CssClass = ""
                    End If

                    'PL
                    Dim lblPL As Label = CType(e.Item.FindControl("lblPL"), Label)
                    If SafeDouble(oData("PL")) < 0 Then
                        lblPL.ForeColor = Drawing.Color.Red
                    Else
                        lblPL.CssClass = ""
                    End If

                    'Pending
                    Dim lblPending As Label = CType(e.Item.FindControl("lblPending"), Label)
                    If SafeDouble(oData("Pending")) < 0 Then

                        lblPending.ForeColor = Drawing.Color.Red

                    Else

                        lblPending.ForeColor = Drawing.Color.Black
                    End If
                    lblPending.Text = FormatNumber(SafeDouble(oData("Pending")), SBCBL.std.GetRoundMidPoint)
                    Pending += SafeDouble(oData("Pending"))
                    '' Set href URL
                    If HistoryPage <> "" AndAlso (Not SafeString(oData("Player")).Contains("Total")) Then
                        sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}", HistoryPage, SafeString(Session("AgentID")), _
                                               SafeString(oData("PlayerID")))

                        'ClientAlert(SafeString(oData("Player")), True)
                        Dim lblPlayer As Label = CType(e.Item.FindControl("lblPlayer"), Label)
                        lblPlayer.CssClass = "hyperlink"
                        lblPlayer.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}';", _
                                                               Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))

                        lblMon.CssClass = "hyperlink " & lblMon.CssClass
                        lblMon.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate, "MM/dd/yyyy")))

                        lblTues.CssClass = "hyperlink " & lblTues.CssClass
                        lblTues.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(1), "MM/dd/yyyy")))
                        If SafeDouble(oData("Tues")) < 0 Then
                            lblTues.ForeColor = Drawing.Color.Red
                        End If


                        lblWed.CssClass = "hyperlink " & lblWed.CssClass

                        lblWed.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                                Format(oDate.AddDays(2), "MM/dd/yyyy")))
                        If SafeDouble(oData("Wed")) < 0 Then
                            lblWed.ForeColor = Drawing.Color.Red
                        End If

                        lblThurs.CssClass = "hyperlink " & lblThurs.CssClass

                        lblThurs.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(3), "MM/dd/yyyy")))

                        If SafeDouble(oData("Thurs")) < 0 Then
                            lblThurs.ForeColor = Drawing.Color.Red
                        End If

                        lblFri.CssClass = "hyperlink " & lblFri.CssClass

                        lblFri.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(4), "MM/dd/yyyy")))
                        If SafeDouble(oData("Fri")) < 0 Then
                            lblFri.ForeColor = Drawing.Color.Red
                        End If

                        lblSat.CssClass = "hyperlink " & lblSat.CssClass

                        lblSat.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(5), "MM/dd/yyyy")))
                        If SafeDouble(oData("Sat")) < 0 Then
                            lblSat.ForeColor = Drawing.Color.Red
                        End If

                        lblSun.CssClass = "hyperlink " & lblSun.CssClass

                        lblSun.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(6), "MM/dd/yyyy")))
                        If SafeDouble(oData("Sun")) < 0 Then
                            lblSun.ForeColor = Drawing.Color.Red
                        End If

                        lblGross.CssClass = "hyperlink " & lblGross.CssClass

                        lblGross.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}';", _
                                                               Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))
                        If SafeDouble(oData("Gross")) < 0 Then
                            lblGross.ForeColor = Drawing.Color.Red
                        End If
                    Else

                    End If

                    If PendingPage <> "" Then
                        sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}", PendingPage, SafeString(Session("AgentID")), _
                                              SafeString(oData("PlayerID")))

                        If SafeDouble(oData("Pending")) <> 0 Then
                            lblPending.CssClass = "hyperlink " & lblPending.CssClass

                            lblPending.Attributes.Add("onClick", _
                                                     String.Format(sLink & "&SDate={0}&EDate={1}';", _
                                                                   Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))
                        End If

                    End If
                End If
            End If
        End Sub

        Private Function BuildChartXML(ByVal poTable As DataTable) As String
            Dim oChart As XElement = <chart lineThickness="1" showValues="0" formatNumberScale="1" anchorRadius="2"
                                         divLineAlpha="20" divLineIsDashed="1" showAlternateHGridColor="1" shadowAlpha="40"
                                         labelStep="2" numvdivlines="5" chartRightMargin="35" bgAngle="270" bgAlpha="10,10"/>

            '' Bind Categories
            Dim oCategories As XElement = <categories/>
            oChart.Add(oCategories)

            For Each oClm As DataColumn In poTable.Columns
                If oClm.ColumnName = "Player" OrElse oClm.ColumnName = "Net" _
                OrElse oClm.ColumnName = "PlayerID" OrElse oClm.ColumnName = "Pending" _
                OrElse oClm.ColumnName = "Gross" OrElse oClm.ColumnName = "PL" Then
                    Continue For
                End If

                oCategories.Add(<category label=<%= oClm.ColumnName %>/>)
            Next

            '' Bind data
            For Each oRow As DataRow In poTable.Rows
                If oRow("Player") = "Totals" Then
                    Continue For
                End If

                Dim oDataset As XElement = <dataset seriesName=<%= SafeString(oRow("Player")) %>/>
                oChart.Add(oDataset)

                For Each oClm As DataColumn In poTable.Columns
                    If oClm.ColumnName = "Player" OrElse oClm.ColumnName = "Net" _
                    OrElse oClm.ColumnName = "PlayerID" OrElse oClm.ColumnName = "Pending" _
                    OrElse oClm.ColumnName = "Gross" Then
                        Continue For
                    End If

                    oDataset.Add(<set value=<%= SafeString(oRow(oClm.ColumnName)) %>/>)
                Next

            Next

            Return oChart.ToString().Replace("'", "&rsquo;").Replace("""", "'")
        End Function

        Public Function getDay(ByVal nday As Integer) As String
            Dim oDate As Date
            If FromDate = Date.MinValue Then
                oDate = SafeDate(ddlWeeks.Value)
            Else
                oDate = FromDate
            End If
            Return oDate.AddDays(nday).ToString("MM-dd")
        End Function

        Protected Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            'BindData()
            'RaiseEvent OnChange()
        End Sub
        Protected Sub ddlWeeks_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWeeks.SelectedIndexChanged
            FromDate = Nothing
            bindSubAgent()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If GetSiteType.Equals("SBS") Then
                trChart.Visible = False
            End If
        End Sub

        Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.EditItem Then
                Dim dgSubPlayers As DataGrid = e.Item.FindControl("dgSubPlayers")
                Dim lblAgentWeekly As Literal = e.Item.FindControl("lblAgentWeekly")
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin AndAlso AgentID = "All" Then

                    lblAgentWeekly.Text = "Agent: " & e.Item.DataItem("AgentName")
                Else
                    lblAgentWeekly.Text = "Agent: " & e.Item.DataItem("AgentName")
                End If

                Session("AgentID") = SafeString(e.Item.DataItem("AgentID"))
                BindData(dgSubPlayers, SafeString(e.Item.DataItem("AgentID")))
                Session("AgentID") = Nothing
            End If
        End Sub

       
    End Class
End Namespace