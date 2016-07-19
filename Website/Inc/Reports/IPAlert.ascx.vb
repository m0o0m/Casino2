Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports SBCBL.UI

Namespace SBCWebsite
    Partial Class IPAlert
        Inherits SBCBL.UI.CSBCUserControl
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType)
        Private ReadOnly lastNumDay As Integer = -14

#Region "Property"

        Public Property AgentLabel() As String
            Get
                If ViewState("AGENTLABEL") IsNot Nothing Then
                    Return CType(ViewState("AGENTLABEL"), String)
                Else
                    Return "Agent"
                End If
            End Get
            Set(ByVal value As String)
                ViewState("AGENTLABEL") = value
            End Set
        End Property

        Public Property ParentID() As String
            Get
                If ViewState("PARENTID") IsNot Nothing Then
                    Return CType(ViewState("PARENTID"), String)
                Else
                    If SafeString(ddlAgent.SelectedValue) = "" Then
                        Return UserSession.UserID
                    Else
                        Return SafeString(ddlAgent.SelectedValue)
                    End If

                End If
            End Get
            Set(ByVal value As String)
                ViewState("PARENTID") = value
            End Set
        End Property

        Public ReadOnly Property IsSuperAdmin() As Boolean
            Get
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    Return ddlAgent.SelectedValue = ""
                End If

                Return False
            End Get
        End Property

        Private Property IPALert() As DataTable
            Get
                If ViewState("IPALERT") IsNot Nothing Then
                    Return CType(ViewState("IPALERT"), DataTable)
                Else
                    Return New DataTable
                End If
            End Get
            Set(ByVal value As DataTable)
                ViewState("IPALERT") = value
            End Set
        End Property

        Private Property IPTraces() As DataTable
            Get
                If ViewState("IPTRACES") IsNot Nothing Then
                    Return CType(ViewState("IPTRACES"), DataTable)
                Else
                    Return New DataTable
                End If
            End Get
            Set(ByVal value As DataTable)
                ViewState("IPTRACES") = value
            End Set
        End Property

#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindAgent()
                Dim userSession As New CSBCSession()
                userSession.HasIPAlert = False
                btnSearch_Click(sender, e)
            End If
        End Sub

#End Region

#Region "Controls Events"

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
            getSearchResult()

            dgIPAlert.DataSource = Me.IPALert
            dgIPAlert.DataBind()
            dgIPAlert.Visible = Me.IPALert.Rows.Count > 1
        End Sub

        Protected Sub dgIPAlert_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgIPAlert.ItemCommand
            
            Select Case e.CommandName
                Case "ViewIPListByAgent"                    
                    ViewIPList(SafeString(e.CommandArgument))                                

                Case "ViewIPListByPlayer"                    
                    ViewIPList(SafeString(e.CommandArgument))

                Case "ViewUserLoginList"                    
                    ViewUserLoginList(SafeString(e.CommandArgument))

            End Select
        End Sub

        Protected Sub dgIPAlert_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgIPAlert.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
                Dim sLoginName As String = SafeString(DataBinder.Eval(e.Item.DataItem, "LoginName"))
                Dim sPlayer As String = SafeString(DataBinder.Eval(e.Item.DataItem, "Player"))
                Dim sIP As String = SafeString(DataBinder.Eval(e.Item.DataItem, "IP"))

                '' Agent
                Dim olbnAgentName As LinkButton = CType(e.Item.FindControl("lbnAgentName"), LinkButton)
                If olbnAgentName IsNot Nothing Then
                    If Not IsHaveManyIP(sLoginName) Or sPlayer <> "" Then
                        olbnAgentName.Enabled = False
                    Else
                        olbnAgentName.Style("color") = "Red"
                    End If
                End If

                '' Player
                Dim olbnPlayer As LinkButton = CType(e.Item.FindControl("lbnPlayer"), LinkButton)
                If olbnPlayer IsNot Nothing Then
                    If Not IsHaveManyIP(sLoginName) Or sPlayer = "" Then
                        olbnPlayer.Enabled = False
                    Else
                        olbnPlayer.Style("color") = "Red"
                    End If
                End If

                '' IP
                Dim olbnIP As LinkButton = CType(e.Item.FindControl("lbnIP"), LinkButton)
                If olbnIP IsNot Nothing Then
                    If Not IsHaveManyUserLogin(sIP) Then
                        olbnIP.Enabled = False
                    Else
                        olbnIP.Style("color") = "Red"
                    End If
                End If


            End If
        End Sub

#End Region

#Region "Private Functions"

        Private Sub bindAgent()
            Dim dtParents As DataTable = Nothing
            dtParents = (New CAgentManager).GetAllAgentsBySuperAdminID(Me.ParentID, Nothing, False, "", Me.IsSuperAdmin)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgent.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                loadSubAgent(sAgentID, dtParents)
            Next

            ddlAgent.Items.Insert(0, "")

            ddlAgent.Visible = ddlAgent.Items.Count > 1
            lblMessage.Visible = Not ddlAgent.Visible
            If Me.IsSuperAdmin Then
                lblMessage.Text = "We don't have any agent."
            Else
                lblMessage.Text = "We don't have any sub agent."
            End If
        End Sub

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlAgent.Items.Add(New ListItem(sText, sAgentID))

                loadSubAgent(sAgentID, podtAgents)
            Next
        End Sub

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""

            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next

            Return sLoop
        End Function

        Private Sub getSearchResult()
            Dim sDateFrom As String = Date.Now.AddDays(lastNumDay).ToUniversalTime.ToString("MM/dd/yyyy") 'SafeString(ddlDateFrom.SelectedValue) & " 00:00:00"

            Dim oIPTracesManager As New CIPTracesManager()
            Dim oData As DataTable = Nothing

            If Me.IsSuperAdmin Then
                If ddlAgent.SelectedValue <> "" Then
                    oData = oIPTracesManager.GetIPAlertSearchResult(Me.ParentID, False, sDateFrom)
                Else
                    oData = oIPTracesManager.GetIPAlertSearchResult(Me.ParentID, True, sDateFrom)
                End If
            Else
                oData = oIPTracesManager.GetIPAlertSearchResult(Me.ParentID, False, sDateFrom)
            End If

            Me.IPALert = oData

            getIPTracesByDateRange(sDateFrom)
        End Sub

        Private Sub getIPTracesByDateRange(ByVal psDateFrom As String)
            Dim oIPTracesManager As New CIPTracesManager()
            Dim oData As DataTable = oIPTracesManager.GetIPTracesByDateRange(Me.ParentID, Me.IsSuperAdmin, psDateFrom)

            Me.IPTraces = oData
        End Sub

        Private Function IsHaveManyIP(ByVal psLoginName As String) As Boolean
            If Me.IPTraces.Rows.Count <= 0 Then Return False

            Dim nCountIP As Integer = SafeInteger(Me.IPTraces.Compute("Count(IP)", "LoginName=" & SQLString(psLoginName)))

            Return nCountIP > 1
        End Function

        Private Function IsHaveManyUserLogin(ByVal psIP As String) As Boolean
            If Me.IPTraces.Rows.Count <= 0 Then Return False

            Dim nCountUserLogin As Integer = SafeInteger(Me.IPTraces.Compute("Count(LoginName)", "IP=" & SQLString(psIP)))

            Return nCountUserLogin > 1
        End Function

        Private Sub ViewIPList(ByVal psAgent As String)

            Dim obj = (From oIPTraces In Me.IPTraces _
                       Where SafeString(oIPTraces("LoginName")) = psAgent _
                       Order By oIPTraces("TraceDate") Descending _
                       Select New With {.IP = SafeString(oIPTraces("IP")), _
                                        .LastLogin = SafeString(oIPTraces("TraceDate")), _
                                        .NumLogin = SafeString(oIPTraces("LoginTime"))} _
                      ).ToList()

            dgIPListLoginByAgent.DataSource = obj
            dgIPListLoginByAgent.DataBind()

            ScriptManager.RegisterStartupScript(Page, Me.GetType, "CreateDropdownWindow", _
                                                                  "CreateDropdownWindow('View IP list by LoginName: " & psAgent & "', '400px', true, 'divIPListLoginByAgent');", True)
        End Sub

        Private Sub ViewUserLoginList(ByVal psIP As String)            

            Dim obj = (From oIPTraces In Me.IPTraces _
                       Where SafeString(oIPTraces("IP")) = psIP _
                       Order By oIPTraces("LoginName"), oIPTraces("TraceDate") Descending _
                       Select New With {.UserLogin = SafeString(oIPTraces("LoginName")), _
                                        .LastLogin = SafeString(oIPTraces("TraceDate")), _
                                        .NumLogin = SafeString(oIPTraces("LoginTime"))}).ToList()

            dgUserLoginList.DataSource = obj
            dgUserLoginList.DataBind()

            ScriptManager.RegisterStartupScript(Page, Me.GetType, "CreateDropdownWindow", _
                                                                  "CreateDropdownWindow('View UserLogin list by IP: " & psIP & "', '400px', true, 'divUserLoginList');", True)
        End Sub

#End Region

    End Class
End Namespace