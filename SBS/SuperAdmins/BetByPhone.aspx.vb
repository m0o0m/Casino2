Imports System.Data
Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class BetByPhone
        Inherits SBCBL.UI.CSBCPage
        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Call Center Activity"
            SideMenuTabName = "BET_BY_PHONE"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Call Center Activity")

            If Not IsPostBack Then
                BindAgents()
                bindWeek()
            End If
        End Sub

        Public Sub BindAgents()
            Dim oAgentManager As New SBCBL.Managers.CCallCenterAgentManager()
            Dim oDT As DataTable = oAgentManager.GetAgents(SBCBL.std.GetSiteType)
            ddlAgents.DataTextField = "Name"
            ddlAgents.DataValueField = "CallCenterAgentID"
            ddlAgents.DataSource = oDT
            ddlAgents.DataBind()
        End Sub

        Protected Sub ddpAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            dtgLogs.CurrentPageIndex = 0
            SearchNotes()
        End Sub

        Private Function SearchNotes() As DataTable
            Dim oDate As Date = Nothing
            If ddlWeeks.SelectedValue <> "" Then
                oDate = SafeDate(ddlWeeks.SelectedValue)
            Else
                oDate = GetMondayOfCurrentWeek(UserSession.SuperAdminInfo.TimeZone)
            End If
            'oDate = SafeDate("06/28/2010")
            oDate = UserSession.ConvertToGMT(oDate)

            Dim oNoteManager As New SBCBL.Managers.CActivityNotesManager()
            Dim oDT As DataTable = oNoteManager.GetAgentNotes(ddlAgents.SelectedValue, oDate, oDate.AddDays(6), SBCBL.std.GetSiteType, ddlPlayers.SelectedValue)
            dtgLogs.DataSource = oDT
            dtgLogs.DataBind()
            Return oDT
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Dim oDT As DataTable = SearchNotes()

                '' bind to ddlPlayer the list of users that made call
                ddlPlayers.DataSource = oDT.DefaultView.ToTable(True, "PlayerName")
                ddlPlayers.DataValueField = "PlayerName"
                ddlPlayers.DataTextField = "PlayerName"
                ddlPlayers.DataBind()
            End If
        End Sub

        Protected Sub dtgLogs_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dtgLogs.PageIndexChanged
            dtgLogs.CurrentPageIndex = e.NewPageIndex
            SearchNotes()
        End Sub

        Private Sub bindWeek()
            Dim oDate As Date = GetMondayOfCurrentWeek(UserSession.SuperAdminInfo.TimeZone)
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

        Protected Sub ddlWeeks_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWeeks.SelectedIndexChanged
            dtgLogs.CurrentPageIndex = 0
            SearchNotes()
        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            ddlAgents.Enabled = ddlPlayers.SelectedValue = ""
            dtgLogs.CurrentPageIndex = 0
            SearchNotes()
        End Sub
    End Class
End Namespace