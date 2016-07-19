Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports System.Xml

Namespace SBCWebsite
    Partial Class AgentBalanceReport
        Inherits SBCBL.UI.CSBCUserControl
        Dim intActivePlayer As Integer = 0
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

        Public Property ReportPage() As String
            Get
                Return SafeString(ViewState("_REPORT_PAGE"))
            End Get
            Set(ByVal value As String)
                ViewState("_REPORT_PAGE") = value
            End Set
        End Property

#End Region

#Region "Bind Data"
        Private Sub bindAgents()
            Dim oAgentManager As New CAgentManager()
            If SuperID <> "" Then
                ddlAgents.DataSource = oAgentManager.GetAgentsBySuperID(SuperID, Nothing)
            Else
                ddlAgents.DataSource = oAgentManager.GetAgentsByAgentID(AgentID, Nothing)
            End If

            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub

        Private Sub bindWeek()
            'Dim nTimeZome As Integer = 0
            'If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
            '    nTimeZome = UserSession.SuperAdminInfo.TimeZone
            'ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
            '    nTimeZome = UserSession.AgentUserInfo.TimeZone
            'End If
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
            trWeek.Visible = ShowWeekList
            If ShowWeekList Then
                bindWeek()
            End If

            trAgent.Visible = ShowAgentList
            If ShowAgentList Then
                bindAgents()
            End If

            BindData()
        End Sub

        Public Sub BindData()
            Dim sAgentID As String = ""
            If trAgent.Visible AndAlso ddlAgents.Value <> "" Then
                sAgentID = ddlAgents.Value
            End If

            Dim sStartDate As String = ""
            If trWeek.Visible AndAlso ddlWeeks.Value <> "" Then
                sStartDate = ddlWeeks.Value
            End If

            Dim oAgentMamanager As New CAgentManager()
            Dim oTable As DataTable

            If sAgentID <> "" Then
                oTable = oAgentMamanager.GetAgentsDashboard(sAgentID, SafeDate(sStartDate))
            Else
                oTable = oAgentMamanager.GetAgentsDashboard(SuperID, SafeDate(sStartDate), True)
            End If

            dgSubAgents.DataSource = oTable

            If oTable.Rows.Count > 1 Then
                dgSubAgents.Visible = True
                trChart.Visible = True

                dgSubAgents.DataBind()

                lblXML1.Text = BuildChartXML(oTable)
            Else
                dgSubAgents.Visible = False
                trChart.Visible = False
                Title = ""
            End If

        End Sub
#End Region

        Protected Sub dgSubAgents_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgSubAgents.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)
                If SafeString(oData("Agent")) = "Totals" Then
                    For Each oColumn As TableCell In e.Item.Cells
                        oColumn.CssClass = "Bold"
                    Next
                    CType(e.Item.FindControl("lblAgent"), Label).Text = "Totals (Active Players : <b style='color:red'>" & intActivePlayer & "</b>)"
                End If

                Dim nTimeZome As Integer = 0
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    nTimeZome = UserSession.SuperAdminInfo.TimeZone
                ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                    nTimeZome = UserSession.AgentUserInfo.TimeZone
                End If

                If SafeString(oData("Agent")) <> "Totals" Then
                    Dim sLink As String
                    Dim oDate As Date
                    If trWeek.Visible AndAlso ddlWeeks.Value <> "" Then
                        oDate = SafeDate(ddlWeeks.Value)
                    Else
                        oDate = GetMondayOfCurrentWeek(nTimeZome)
                    End If

                    '' Active Players
                    If SafeInteger(oData("ActivePlayers")) <> 0 Then
                        intActivePlayer += SafeInteger(oData("ActivePlayers"))
                        Dim lblActivePlayers As Literal = CType(e.Item.FindControl("lblActivePlayers"), Literal)
                        lblActivePlayers.Text = String.Format(" (Active Players: <label style='color: red;'>{0}</label>)", oData("ActivePlayers"))
                    End If

                    If ReportPage <> "" Then
                        sLink = String.Format("window.location='{0}?AgentID={1}", ReportPage, _
                                              SafeString(oData("AgentID")))

                        Dim lblAgent As Label = CType(e.Item.FindControl("lblAgent"), Label)
                        lblAgent.CssClass = "hyperlink"
                        lblAgent.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}&tab=SUB_AGENTS ';", _
                                                               Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))
                    End If

                    '' Set font color for positive and negative amount
                    'Monday
                    Dim lblMon As Label = CType(e.Item.FindControl("lblMon"), Label)
                    If SafeDouble(oData("Mon")) < 0 Then
                        lblMon.CssClass = "red"
                    Else
                        lblMon.CssClass = ""
                    End If
                    If SafeDouble(oData("Mon")) < 0 Then
                        lblMon.ForeColor = Drawing.Color.Red
                    End If
                    'Tuesday
                    Dim lblTues As Label = CType(e.Item.FindControl("lblTues"), Label)
                    If SafeDouble(oData("Tues")) < 0 Then
                        lblTues.CssClass = "red"
                    Else
                        lblTues.CssClass = ""
                    End If
                    If SafeDouble(oData("Tues")) < 0 Then
                        lblTues.ForeColor = Drawing.Color.Red
                    End If
                    'Wednesday
                    Dim lblWed As Label = CType(e.Item.FindControl("lblWed"), Label)
                    If SafeDouble(oData("Wed")) < 0 Then
                        lblWed.CssClass = "red"
                    Else
                        lblWed.CssClass = ""
                    End If
                    If SafeDouble(oData("Wed")) < 0 Then
                        lblWed.ForeColor = Drawing.Color.Red
                    End If
                    'Thursday
                    Dim lblThurs As Label = CType(e.Item.FindControl("lblThurs"), Label)
                    If SafeDouble(oData("Thurs")) < 0 Then
                        lblThurs.CssClass = "red"
                    Else
                        lblThurs.CssClass = ""
                    End If
                    If SafeDouble(oData("Thurs")) < 0 Then
                        lblThurs.ForeColor = Drawing.Color.Red
                    End If
                    'Friday
                    Dim lblFri As Label = CType(e.Item.FindControl("lblFri"), Label)
                    If SafeDouble(oData("Fri")) < 0 Then
                        lblFri.CssClass = "red"
                    Else
                        lblFri.CssClass = ""
                    End If
                    If SafeDouble(oData("Fri")) < 0 Then
                        lblFri.ForeColor = Drawing.Color.Red
                    End If
                    'Saturday
                    Dim lblSat As Label = CType(e.Item.FindControl("lblSat"), Label)
                    If SafeDouble(oData("Sat")) < 0 Then
                        lblSat.CssClass = "red"
                    Else
                        lblSat.CssClass = ""
                    End If
                    If SafeDouble(oData("Sat")) < 0 Then
                        lblSat.ForeColor = Drawing.Color.Red
                    End If
                    'Sunday
                    Dim lblSun As Label = CType(e.Item.FindControl("lblSun"), Label)
                    If SafeDouble(oData("Sun")) < 0 Then
                        lblSun.CssClass = "red"
                    Else
                        lblSun.CssClass = ""
                    End If
                    If SafeDouble(oData("Sun")) < 0 Then
                        lblSun.ForeColor = Drawing.Color.Red
                    End If
                    'Gross
                    Dim lblGross As Label = CType(e.Item.FindControl("lblGross"), Label)
                    If SafeDouble(oData("Gross")) < 0 Then
                        lblGross.CssClass = "red"
                    Else
                        lblGross.CssClass = ""
                    End If
                    If SafeDouble(oData("Gross")) < 0 Then
                        lblGross.ForeColor = Drawing.Color.Red
                    End If
                    'Net
                    Dim lblNet As Label = CType(e.Item.FindControl("lblNet"), Label)
                    If SafeDouble(oData("Net")) < 0 Then
                        lblNet.CssClass = "red"
                    Else
                        lblNet.CssClass = ""
                    End If
                    If SafeDouble(oData("Net")) < 0 Then
                        lblNet.ForeColor = Drawing.Color.Red
                    End If
                    'PL
                    Dim lblPL As Label = CType(e.Item.FindControl("lblPL"), Label)
                    If SafeDouble(oData("PL")) < 0 Then
                        lblPL.CssClass = "red"
                    Else
                        lblPL.CssClass = ""
                    End If
                    If SafeDouble(oData("PL")) < 0 Then
                        lblPL.ForeColor = Drawing.Color.Red
                    End If
                    'Pending
                    Dim lblPending As Label = CType(e.Item.FindControl("lblPending"), Label)
                    If SafeDouble(oData("Pending")) < 0 Then
                        lblPending.ForeColor = Drawing.Color.Red
                    Else
                        lblPending.ForeColor = Drawing.Color.Empty
                    End If

                    '' Set href URL
                    If HistoryPage <> "" Then
                        sLink = String.Format("window.location='{0}?AgentID={1}", HistoryPage, _
                                              SafeString(oData("AgentID")))

                        lblMon.CssClass = "hyperlink " & lblMon.CssClass
                        lblMon.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate, "MM/dd/yyyy")))

                        lblTues.CssClass = "hyperlink " & lblTues.CssClass
                        lblTues.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(1), "MM/dd/yyyy")))

                        lblWed.CssClass = "hyperlink " & lblWed.CssClass
                        lblWed.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                                Format(oDate.AddDays(2), "MM/dd/yyyy")))

                        lblThurs.CssClass = "hyperlink " & lblThurs.CssClass
                        lblThurs.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(3), "MM/dd/yyyy")))

                        lblFri.CssClass = "hyperlink " & lblFri.CssClass
                        lblFri.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(4), "MM/dd/yyyy")))

                        lblSat.CssClass = "hyperlink " & lblSat.CssClass
                        lblSat.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(5), "MM/dd/yyyy")))

                        lblSun.CssClass = "hyperlink " & lblSun.CssClass
                        lblSun.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={0}';", _
                                                               Format(oDate.AddDays(6), "MM/dd/yyyy")))

                        lblGross.CssClass = "hyperlink " & lblGross.CssClass
                        lblGross.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}';", _
                                                               Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))

                    End If

                    If PendingPage <> "" Then
                        sLink = String.Format("window.location='{0}?AgentID={1}", PendingPage, _
                                              SafeString(oData("AgentID")))

                        If SafeDouble(oData("Pending")) <> 0 Then
                            If SafeDouble(oData("Pending")) < 0 Then
                                lblPending.CssClass = "hyperlink red"
                            Else
                                lblPending.CssClass = "hyperlink"
                            End If

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
                If oClm.ColumnName = "Agent" OrElse oClm.ColumnName = "Net" _
                OrElse oClm.ColumnName = "AgentID" OrElse oClm.ColumnName = "Pending" _
                OrElse oClm.ColumnName = "Gross" OrElse oClm.ColumnName = "PL" Then
                    Continue For
                End If

                oCategories.Add(<category label=<%= oClm.ColumnName %>/>)
            Next

            '' Bind data
            For Each oRow As DataRow In poTable.Rows
                If oRow("Agent") = "Totals" Then
                    Continue For
                End If

                Dim oDataset As XElement = <dataset seriesName=<%= SafeString(oRow("Agent")) %>/>
                oChart.Add(oDataset)

                For Each oClm As DataColumn In poTable.Columns
                    If oClm.ColumnName = "Agent" OrElse oClm.ColumnName = "Net" _
                    OrElse oClm.ColumnName = "AgentID" OrElse oClm.ColumnName = "Pending" _
                    OrElse oClm.ColumnName = "Gross" Then
                        Continue For
                    End If

                    oDataset.Add(<set value=<%= SafeString(oRow(oClm.ColumnName)) %>/>)
                Next

            Next

            Return oChart.ToString().Replace("'", "&rsquo;").Replace("""", "'")
        End Function

        Protected Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            BindData()
        End Sub

        'Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        '    If GetSiteType().Equals("SBS") Then
        '        'trChart.Visible = False
        '    End If
        'End Sub
    End Class
End Namespace