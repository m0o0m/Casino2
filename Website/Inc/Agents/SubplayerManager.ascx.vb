Imports System.Data
Imports SBCBL.Security
Imports SBCBL.CEnums
Imports SBCBL.std
Imports SBCBL.Utils
Imports WebsiteLibrary.DBUtils
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL


Namespace SBSAgents
Partial Class SubplayerManager
        Inherits SBCBL.UI.CSBCUserControl
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindAgents()
                bindPlayers()
            End If
        End Sub

        Private Sub bindPlayers()
            Dim oMng As New CPlayerManager
            If Not String.IsNullOrEmpty(ddlAgents.Value) Then
                dgPlayers.DataSource = oMng.GetPlayersByAgentID(ddlAgents.Value)
                dgPlayers.DataBind()
            End If
        End Sub

        Private Sub bindAgents()
            ddlAgents.DataSource = loadAgents()
            ddlAgents.DataTextField = "AgentName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub
        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtParents As DataTable, ByRef odtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtParents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")
            For Each drChild As DataRow In odrSubAgents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)
                odrAgent("AgentID") = drChild("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                odrAgent("Login") = drChild("Login")
                odrAgent("IsLocked") = drChild("IsLocked")
                odrAgent("IsBettingLocked") = drChild("IsBettingLocked")
                odrAgent("LastLoginDate") = drChild("LastLoginDate")
                loadSubAgent(SafeString(drChild("AgentID")), podtParents, odtAgents)
            Next
        End Sub

        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))
            odtAgents.Columns.Add("Login", GetType(String))
            odtAgents.Columns.Add("IsLocked", GetType(String))
            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
            odtAgents.Columns.Add("LastLoginDate", GetType(DateTime))
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
                odrAgent("Login") = drParent("Login")
                odrAgent("IsLocked") = drParent("IsLocked")
                odrAgent("IsBettingLocked") = drParent("IsBettingLocked")
                odrAgent("LastLoginDate") = drParent("LastLoginDate")
                loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents)
            Next
            Return odtAgents
        End Function

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""
            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next
            Return sLoop & " "
        End Function

        Public Function formatDate(ByVal obj As Object) As String

            Dim dDate As Date?
            If Not String.IsNullOrEmpty(SafeString(obj)) Then
                dDate = SafeDate(obj)
            End If

            If dDate.HasValue Then
                Return dDate.Value.ToString("MM/dd/yyyy")
            End If
            Return String.Empty
        End Function
        Protected Sub dgPlayers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPlayers.ItemCommand

            Dim oPlayerIDAndLogins As New List(Of String)
            Dim oPlayerManager As New CPlayerManager
            'If Not ucPlayerEdit.LoadPlayerInfo(SafeString(e.CommandArgument)) Then
            '    ClientAlert("Can't Load Player's Information", True)
            '    ucPlayerEdit.ResetPlayerInfor()
            '    Return
            'End If
            ucPlayerSubAgentEdit.LoadPlayerInfo(SafeString(e.CommandArgument))
            ucPlayerSubAgentEdit.Visible = True
            pnPlayerManager.Visible = False
            bindPlayers()
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            bindPlayers()
        End Sub

        Protected Sub ucPlayerSubAgentEdit_ButtonClick(ByVal sButtonType As String) Handles ucPlayerSubAgentEdit.ButtonClick
            ucPlayerSubAgentEdit.Visible = False
            pnPlayerManager.Visible = True
        End Sub
    End Class
End Namespace
