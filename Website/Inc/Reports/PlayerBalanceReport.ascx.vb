Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports System.Xml

Namespace SBCWebsite
    Partial Class PlayerBalanceReport
        Inherits SBCBL.UI.CSBCUserControl
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
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

        Property AllPlayer() As Boolean
            Get
                Return SafeBoolean(ViewState("AllPlayer"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("AllPlayer") = value
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

        Public Property LabelSelectedUser() As String
            Get
                Return lblUsers.Text
            End Get
            Set(ByVal value As String)
                lblUsers.Text = value
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

        Public Property SelectedAgent() As String
            Get
                Return ddlAgents.Value
            End Get
            Set(ByVal value As String)
                ddlAgents.Value = value

                BindData()
            End Set
        End Property

        Public Property SelectedWeek() As String
            Get
                Return ddlWeeks.Value
            End Get
            Set(ByVal value As String)
                ddlWeeks.Value = value

                BindData()
            End Set
        End Property
#End Region

#Region "Bind Data"
        Public Sub bindAgents()
            Dim dtAgent As DataTable = loadAgents()
            If AllPlayer Then
                dtAgent.Rows(0)("AgentName") = "All"
            End If
            ddlAgents.DataSource = dtAgent
            ddlAgents.DataTextField = "AgentName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()

        End Sub
#Region "Super bindagent"
        Private Sub bindSuperAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                loadSubAgent(sAgentID, dtParents)
            Next

            ddlAgents.Items.Insert(0, "")
        End Sub

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlAgents.Items.Add(New ListItem(sText, sAgentID))

                loadSubAgent(sAgentID, podtAgents)
            Next
        End Sub
#End Region
        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))

            ''parent
            Dim sParentName As String = String.Format("{0} ({1})", UserSession.AgentUserInfo.Login, UserSession.AgentUserInfo.Name)
            odtAgents.Rows.Add(New Object() {UserSession.UserID, sParentName})

            Dim oAgentManager As New CAgentManager
            Dim dtParents As DataTable = oAgentManager.GetAllAgentsByAgent(UserSession.UserID, Nothing)

            Dim nMinAgentLevel As Integer = SafeInteger(dtParents.Compute("MIN(AgentLevel)", ""))
            Dim odrParents As DataRow() = dtParents.Select("AgentLevel=" & SafeString(nMinAgentLevel), "AgentName")

            For Each drParent As DataRow In odrParents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drParent("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drParent("AgentLevel")) - 1) & SafeString(drParent("AgentName"))

                loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents)
            Next

            Return odtAgents
        End Function

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtParents As DataTable, ByRef odtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtParents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drChild("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))

                loadSubAgent(SafeString(drChild("AgentID")), podtParents, odtAgents)
            Next
        End Sub

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""

            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next

            Return sLoop & " "
        End Function

        'Private Sub bindAgents()
        '    Dim oAgentManager As New CAgentManager()
        '    If SuperID <> "" Then
        '        ddlAgents.DataSource = oAgentManager.GetAgentsBySuperID(SuperID, Nothing)
        '    Else
        '        ddlAgents.DataSource = oAgentManager.GetAgentsByAgentID(AgentID, Nothing)
        '    End If

        '    ddlAgents.DataTextField = "FullName"
        '    ddlAgents.DataValueField = "AgentID"
        '    ddlAgents.DataBind()

        '    If ddlAgents.Items.Count > 0 Then
        '        ddlAgents.SelectedIndex = 0
        '    End If
        'End Sub

        Private Sub bindWeek()
            ' Dim nTimeZome As Integer = 0
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
            bindWeek()
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                bindSuperAgents()
                trAgent.Visible = True
                BindData()
                Return
            End If
            trAgent.Visible = False
            If ShowAgentList Then
                Dim oAgentManager As New CAgentManager()

                If SuperID <> "" OrElse oAgentManager.NumOfSubAgents(AgentID) > 0 Then
                    bindAgents()
                    trAgent.Visible = True
                End If
            End If

            BindData()
        End Sub

        Public Sub BindData()
            Dim sAgentID As String = ""
            If (UserSession.UserType = SBCBL.EUserType.SuperAdmin OrElse UserSession.UserType = SBCBL.EUserType.Agent) AndAlso ddlAgents.Value <> "" Then
                sAgentID = ddlAgents.Value
            Else
                sAgentID = AgentID
            End If

            Dim sStartDate As String = ""
            If ddlWeeks.Value <> "" Then
                sStartDate = ddlWeeks.Value
            End If

            Dim oAgentMamanager As New CAgentManager()
            Dim oTable As DataTable
            If AllPlayer Then
                oTable = oAgentMamanager.GetAllPlayersDashboard(sAgentID, SafeDate(sStartDate))

            ElseIf sStartDate <> "" Then
                oTable = oAgentMamanager.GetPlayersDashboard(sAgentID, SafeDate(sStartDate))
            Else
                oTable = oAgentMamanager.GetPlayersDashboard(sAgentID)
            End If

            dgSubPlayers.DataSource = oTable
            If oTable.Rows.Count > 1 Then
                dgSubPlayers.Visible = True
                trChart.Visible = True

                dgSubPlayers.DataBind()

                If AllPlayer Then

                    '     Dim oCountAgents As List(Of CItemCount) = (From oRow In oTable.AsEnumerable _
                    'Group oRow By sAgent = SafeString(oRow("Agent")) Into Count() _
                    'Where Count >= 1 _
                    'Select New CItemCount(sAgent, Count)).ToList()

                    '     If oCountAgents.Count = 0 Then
                    '         Return
                    '     End If
                    '     Dim nIndex As Integer = 0
                    '     While nIndex < dgSubPlayers.Items.Count - 1
                    '         Dim oItem As DataGridItem = dgSubPlayers.Items(nIndex)
                    '         Dim sAgent As String = SafeString(CType(oItem.FindControl("lblAgent"), Label).Text)
                    '         Dim oCountByAgent As CItemCount = oCountAgents.Find(Function(x) x.ItemID = sAgent)
                    '         If oCountByAgent IsNot Nothing Then
                    '             Dim nRowSpan As Integer = oCountByAgent.ItemCount
                    '             oItem.Cells(0).RowSpan = nRowSpan 'Internet/Phone
                    '             For nRowSpanStart As Integer = 1 To nRowSpan - 1
                    '                 Dim oItemRowSpan As DataGridItem = dgSubPlayers.Items(nRowSpanStart + nIndex)
                    '                 oItemRowSpan.Cells(0).Visible = False
                    '             Next
                    '             nIndex += nRowSpan
                    '         Else
                    '             nIndex += 1
                    '         End If

                    '     End While

                    Dim i As Integer = 1
                    Dim k As Integer = 0
                    Dim n As Integer = 0
                    Dim strAgent As String = ""
                    For Each odr As DataRow In oTable.Rows
                        If (String.IsNullOrEmpty(strAgent)) Then
                            strAgent = SafeString(odr("Agent"))
                            i = 2
                        ElseIf strAgent.Equals(SafeString(odr("Agent"))) Then
                            i += 1
                            dgSubPlayers.Items(k).Cells(0).Visible = False

                        Else
                            dgSubPlayers.Items(k).Cells(0).Visible = False
                            dgSubPlayers.Items(n).Cells(0).Visible = True

                            ' ClientAlert(n & ":" & k)
                            'dgSubPlayers.Items(n).BorderColor = Drawing.Color.Red
                            dgSubPlayers.Items(n).Cells(0).RowSpan = i
                            'ClientAlert(n & ":" & i)
                            strAgent = SafeString(odr("Agent"))
                            i = 2
                            n = k + 2
                        End If
                        k += 1
                    Next
                    ' dgSubPlayers.Items(k - 1).Cells(0).Visible = False
                    'ClientAlert(n & ":" & i)

                    ' dgSubPlayers.Items(i + 1).Cells(0).RowSpan = i

                End If





                'If AllPlayer Then
                '    Dim nNumPlayer As Integer
                '    Dim oPlayerManager As New CPlayerManager()
                '    Dim strAgentID As String
                '    Dim oTotalItem As DataGridItem = dgSubPlayers.Items(0)
                '    nNumPlayer = SafeDouble(oPlayerManager.GetNumPlayersByAgentID(sAgentID).Rows(0)("NumPlayer"))
                '    oTotalItem.Cells(0).RowSpan = nNumPlayer + 1
                '    For i As Integer = 1 To nNumPlayer
                '        dgSubPlayers.Items(i).Cells(0).Visible = False
                '    Next

                '    'oTotalItem = dgSubPlayers.Items(10 + 1)
                '    ' oTotalItem.Cells(0).RowSpan = 10
                '    Dim odt As DataTable = oAgentMamanager.GetAllAgentsByAgent(sAgentID, False)
                '    For Each odr As DataRow In odt.Rows
                '        'LogDebug(_log, "vofor" & odt.Rows.Count)
                '        Dim ncount As Integer
                '        strAgentID = SafeString(odr("AgentID"))
                '        ncount = SafeDouble(oPlayerManager.GetNumPlayersByAgentID(strAgentID).Rows(0)("NumPlayer"))
                '        If ncount = 0 Then
                '            dgSubPlayers.Items(nNumPlayer + 1).Visible = False
                '            nNumPlayer += 1
                '            Continue For
                '        End If
                '        oTotalItem = dgSubPlayers.Items(nNumPlayer + 1)
                '        oTotalItem.Cells(0).RowSpan = ncount + 1
                '        '  ClientAlert(ncount & ":" & SafeString(odr("Login")))
                '        For i As Integer = nNumPlayer + 2 To nNumPlayer + ncount + 1
                '            dgSubPlayers.Items(i).Cells(0).Visible = False
                '        Next
                '        nNumPlayer += ncount + 1
                '        'LogDebug(_log, "voforaa" & nNumPlayer)
                '    Next
                'Else
                '    dgSubPlayers.Columns(0).Visible = False
                '    'For i As Integer = 0 To oTable.Rows.Count - 1
                '    '    dgSubPlayers.Items(i).Cells(0).Visible = False
                '    'Next
                'End If

                '    ' lblXL1.Text = BuildChartXML(oTable)
            Else
                dgSubPlayers.Visible = False
                trChart.Visible = False
                Title = ""
            End If
        End Sub
#End Region

        Protected Sub ucPlayerEdit_ButtonClick(ByVal sButtonType As String) Handles ucPlayerEdit.ButtonClick
            BindData()
            ucPlayerEdit.ResetPlayerInfor()
            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)
                Case "SAVE SUCCESSFUL"
            End Select
            tblPlayer.Visible = True
            ucPlayerEdit.Visible = False
            PlayerEdit.Visible = False
        End Sub

        Protected Sub dgSubPlayers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgSubPlayers.ItemCommand
            Dim oPlayerIDAndLogins As New List(Of String)
            Dim oPlayerManager As New CPlayerManager


            If Not ucPlayerEdit.LoadPlayerInfo(SafeString(e.CommandArgument)) Then
                ClientAlert("Can't Load Player's Information", True)
                ucPlayerEdit.ResetPlayerInfor()
                Return
            End If

            tblPlayer.Visible = False
            PlayerEdit.Visible = True
            ucPlayerEdit.Visible = True

            'dgPlayers.SelectedIndex = e.Item.ItemIndex
            ' ''Edit template User
            'If UserSession.AgentUserInfo.IsEnableBettingProfile Then
            '    PlayerID = e.CommandArgument.Split("|")(0)
            '    Dim hfPlayerTemplateID = CType(e.Item.FindControl("hfPlayerTemplateID"), HiddenField)
            '    dgPlayers.SelectedIndex = e.Item.ItemIndex
            '    ''load template player
            '    Dim arrTemplateID As String() = hfPlayerTemplateID.Value.Split("|")
            '    If String.IsNullOrEmpty(arrTemplateID(0)) Then
            '        PlayerTemplateEditID = arrTemplateID(1)
            '    Else
            '        PlayerTemplateEditID = arrTemplateID(0)
            '    End If
            'End If
        End Sub

        Protected Sub dgSubPlayers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgSubPlayers.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim nTimeZome As Integer = 0
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    nTimeZome = UserSession.SuperAdminInfo.TimeZone
                ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                    nTimeZome = UserSession.AgentUserInfo.TimeZone
                End If

                Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)

                If SafeString(oData("Player")) = "Totals" OrElse Not UserSession.AgentUserInfo.HasSystemManagement Then
                    For Each oColumn As TableCell In e.Item.Cells
                        oColumn.CssClass = "Bold"
                    Next
                    CType(e.Item.FindControl("lbtEdit"), LinkButton).Visible = False
                    CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = False
                End If

                CType(e.Item.FindControl("hlCreditBack"), HyperLink).NavigateUrl = "javascript:function void(){};"
                CType(e.Item.FindControl("hlCreditBack"), HyperLink).Attributes("onclick") = "window.open('CreditBack.aspx?player=" & SafeString(oData("Player")) & _
                                                                                    "&playerid=" & SafeString(oData("PlayerID")) & "','creditback','menubar=0,resizable=0,width=300,height=200'); return false;"
                CType(e.Item.FindControl("hlCreditBack"), HyperLink).Visible = SafeString(oData("Player")) <> "Totals" AndAlso UserSession.UserType = SBCBL.EUserType.SuperAdmin

                CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).NavigateUrl = "javascript:function void(){};"
                CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Attributes("onclick") = "openModalDialog('/" & SBCBL.std.GetSiteType & "/" & IIf(UserSession.UserType = SBCBL.EUserType.SuperAdmin, "SuperAdmins", "Agents/Management") & "/HistoricalAmount.aspx?playerID=" & SafeString(oData("PlayerID")) & "',250, 600, false,false,false); return false;"
                CType(e.Item.FindControl("hlHistoricalAmount"), HyperLink).Visible = SafeString(oData("Player")) <> "Totals"

                If AllPlayer AndAlso SafeDouble(oData("Mon")) = 0 AndAlso SafeDouble(oData("Tues")) = 0 AndAlso SafeDouble(oData("Wed")) = 0 AndAlso SafeDouble(oData("Thurs")) = 0 AndAlso SafeDouble(oData("Fri")) = 0 AndAlso SafeDouble(oData("Sat")) = 0 AndAlso SafeDouble(oData("Sun")) = 0 AndAlso SafeDouble(oData("Gross")) = 0 AndAlso SafeDouble(oData("Net")) = 0 AndAlso SafeDouble(oData("PL")) = 0 AndAlso SafeDouble(oData("Pending")) = 0 Then
                    '  If SafeString(oData("Player")) <> "Totals" Then
                    ''For i As Integer = 0 To e.Item.Cells.Count - 1
                    ''    e.Item.Cells(i).Visible = False
                    ''Next
                    'Return
                    e.Item.Style.Add("display", "none")
                    '  e.Item.Visible = False
                    'End If
                End If
                If SafeString(oData("Player")) <> "Totals" Then

                    Dim sLink As String
                    Dim oDate As Date
                    ''in super page (in ucSubPlayerBalanceReport) trweek is invisible so date is wrong when send url
                    'If trWeek.Visible AndAlso ddlWeeks.Value <> "" Then
                    If ddlWeeks.Value <> "" Then
                        oDate = SafeDate(ddlWeeks.Value)
                    Else
                        oDate = GetMondayOfCurrentWeek(nTimeZome)
                    End If

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
                    'If SafeDouble(oData("Pending")) < 0 Then

                    '    lblPending.ForeColor = Drawing.Color.Red

                    'Else

                    '    lblPending.ForeColor = Drawing.Color.Empty
                    'End If

                    '' Set href URL
                    If HistoryPage <> "" Then
                        sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}", HistoryPage, IIf(String.IsNullOrEmpty(SafeString(ddlAgents.SelectedValue)), SafeString(AgentID), SafeString(ddlAgents.SelectedValue)), _
                                              SafeString(oData("PlayerID")))


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
                        'If SafeDouble(oData("Sun")) < 0 Then
                        '    lblSun.ForeColor = Drawing.Color.Red
                        'End If

                        lblGross.CssClass = "hyperlink " & lblGross.CssClass

                        lblGross.Attributes.Add("onClick", String.Format(sLink & "&SDate={0}&EDate={1}';", _
                                                               Format(oDate, "MM/dd/yyyy"), Format(oDate.AddDays(6), "MM/dd/yyyy")))
                        'If SafeDouble(oData("Gross")) < 0 Then
                        '    lblGross.ForeColor = Drawing.Color.Red
                        'End If

                    End If

                    If PendingPage <> "" Then
                        sLink = String.Format("window.location='{0}?AgentID={1}&PlayerID={2}", PendingPage, SafeString(AgentID), _
                                              SafeString(oData("PlayerID")))

                        If SafeDouble(oData("Pending")) <> 0 Then
                            ' lblPending.CssClass = "hyperlink " & lblPending.CssClass

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

        Protected Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            BindData()
            RaiseEvent OnChange()
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If GetSiteType.Equals("SBS") Then
                trChart.Visible = False
            End If
        End Sub
    End Class
End Namespace