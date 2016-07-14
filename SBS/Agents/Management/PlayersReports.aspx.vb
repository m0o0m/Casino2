Imports SBCBL.std

Namespace SBSAgents
    Partial Class PlayersReports
        Inherits SBCBL.UI.CSBCPage

#Region "Properties"
        Private ReadOnly Property AgentID() As String
            Get
                Dim sAgentID As String = SafeString(Request("AgentID"))
                If sAgentID = "" Then
                    sAgentID = UserSession.UserID
                End If

                Return sAgentID
            End Get
        End Property

#End Region

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Player Balance Reports"
            MenuTabName = "REPORT"
            SubMenuActive = "ALL_PLAYERS"
            DisplaySubTitlle(Me.Master, "Player Balance Reports")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            lbtAllPlayer.Attributes.Remove("href")
            lbtPlayers.Attributes.Remove("href")
            lbtSubAgents.Attributes.Remove("href")

            lbtAllPlayer.Attributes.Add("href", "#" + tabContent1.ClientID)
            lbtPlayers.Attributes.Add("href", "#" + tabContent2.ClientID)
            lbtSubAgents.Attributes.Add("href", "#" + tabContent3.ClientID)

            liAllPlayers.Attributes.Remove("class")
            liPlayerReport.Attributes.Remove("class")
            liSubAgents.Attributes.Remove("class")

            tabContent1.Attributes.Remove("class")
            tabContent2.Attributes.Remove("class")
            tabContent3.Attributes.Remove("class")

            tabContent1.Attributes.Add("class", "tab-pane fade in")
            tabContent2.Attributes.Add("class", "tab-pane fade in")
            tabContent3.Attributes.Add("class", "tab-pane fade in")

            If Request.QueryString("tab") IsNot Nothing Then
                If Request.QueryString("tab").ToString = "ALL_PLAYERS" Then
                    liAllPlayers.Attributes.Add("class", "active")
                    tabContent1.Attributes.Add("class", "tab-pane fade in active")
                ElseIf Request.QueryString("tab").ToString = "PLAYERS" Then
                    liPlayerReport.Attributes.Add("class", "active")
                    tabContent2.Attributes.Add("class", "tab-pane fade in active")
                ElseIf Request.QueryString("tab").ToString = "SUB_AGENTS" Then
                    liSubAgents.Attributes.Add("class", "active")
                    tabContent3.Attributes.Add("class", "tab-pane fade in active")
                End If
            Else
                liPlayerReport.Attributes.Add("class", "active")
                tabContent2.Attributes.Add("class", "tab-pane fade in active")
            End If

            If Not IsPostBack Then
                Dim oMng As New SBCBL.Managers.CAgentManager()
                If oMng.NumOfSubAgents(UserSession.UserID) > 0 Then
                    lbtSubAgents.Visible = True
                    lbtAllPlayer.Visible = True

                End If
                'If Not UserSession.AgentUserInfo.HasSystemManagement Then
                '    liPlayerReport.Visible = False
                'End If
                lbtAllPlayer.Visible = True
                setTabActive(SBCBL.std.SafeString(Request("tab"))) 'if request tab is empty, default Players tab
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'setTabActive(CType(sender, LinkButton).CommandArgument)
            Response.Redirect("PlayersReports.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)
            ucPlayerReports.AgentID = AgentID

            Select Case True
                Case UCase(psTabKey) = "SUB_AGENTS"
                    lbtSubAgents.CssClass = "selected"
                    ucPlayerReports.ShowAgentList = True
                    ucPlayerReports.Visible = True
                    ucSummaryReport.Visible = False
                Case UCase(psTabKey) = "ALL_PLAYERS"
                    lbtAllPlayer.CssClass = "selected"
                    ' ucPlayerReports.ShowAgentList = True
                    'ucPlayerReports.AllPlayer = True
                    ucPlayerReports.Visible = False
                    ucSummaryReport.Visible = True
                    ucSummaryReport.ReloadData()
                    ucSummaryReport.ShowWeekList = True
                    ucSummaryReport.ShowAgentList = True
                Case Else
                    lbtPlayers.CssClass = "selected"
                    ucPlayerReports.ShowAgentList = False
                    ucSummaryReport.Visible = False
                    ucPlayerReports.Visible = True
            End Select

            ucPlayerReports.ReloadData()
        End Sub
    End Class
End Namespace