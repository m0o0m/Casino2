Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports SBCBL
Namespace SBSAgents
    Partial Class Inc_Agents_AgentMenu
        Inherits SBCBL.UI.CSBCUserControl


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim strMenuType As String = Request.QueryString("MenuType")
            If Not String.IsNullOrEmpty(strMenuType) Then
                Select Case strMenuType.ToLower()
                    Case "account"
                        pnAccount.Visible = True
                    Case "sys"
                        pnSysMagement.Visible = True
                    Case "game"
                        pnGameManament.Visible = True
                    Case "report"
                        pnReports.Visible = True
                    Case "user"
                        pnUserManament.Visible = True
                End Select
                Dim oagentManager As New CAgentManager()
                'Dim odr As DataRow = oagentManager.GetAgentByAgentID(UserSession.AgentUserInfo.UserID)
                If UserSession.AgentUserInfo.HasSystemManagement Then 'SafeBoolean(odr("HasSystemManagement")) Then
                    trAgent.Visible = True
                    trSubAgent.Visible = False
                Else
                    trAgent.Visible = False
                    trSubAgent.Visible = True
                End If
            End If


        End Sub
    End Class
End Namespace
