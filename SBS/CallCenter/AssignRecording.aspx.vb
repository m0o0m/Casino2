Imports SBCBL.Managers
Imports SBCBL.std
Imports System.IO

Namespace SBCCallCenterAgents
    Partial Class AssignRecording
        Inherits SBCBL.UI.CSBCPage

#Region "Properties"
        Public ReadOnly Property TicketID() As String
            Get
                Return SafeString(Request("TicketID"))
            End Get
        End Property
        Public ReadOnly Property TransactionDate() As String
            Get
                Return SafeString(Request("TransactionDate"))
            End Get
        End Property
#End Region

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Assign Recording"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If TicketID <> "" Then
                    BindCCAgents()
                    Dim oTimeSearch As Date = UserSession.ConvertToEST(SafeDate(TransactionDate).ToString())
                    ucDateFrom.Value = oTimeSearch
                    ucDateTo.Value = oTimeSearch.AddMinutes(20)
                    ddlCCAgents.Value = UserSession.CCAgentUserInfo.PhoneExtension
                    bindAsterisk()
                End If
            End If
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
            bindAsterisk()
        End Sub

#Region "Databind"
        Private Sub BindCCAgents()
            Dim oCCAgentManager As New CCallCenterAgentManager
            ddlCCAgents.DataSource = oCCAgentManager.GetAgents(SBCBL.std.GetSiteType)
            ddlCCAgents.DataTextField = "FullName"
            ddlCCAgents.DataValueField = "PhoneExtension"
            ddlCCAgents.DataBind()
        End Sub

        Private Sub bindAsterisk()
            Dim oAsteriaManager As New CAsteriaManager
            '' Call Timestamp in Asteria save by Mid-Atlantic, so we must convert to Mid-Atlantic timezone
            grdCallLogs.DataSource = oAsteriaManager.GetCallDetails(ddlCCAgents.Value, _
                                                                    ucDateFrom.Value.AddHours(-3), _
                                                                    ucDateTo.Value.AddHours(-3))
            grdCallLogs.DataBind()

            grdCallLogs.Columns(1).Visible = ddlCCAgents.Value = ""
        End Sub
#End Region
        
        Protected Sub grdCallLogs_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdCallLogs.ItemCommand
            Select Case UCase(e.CommandName)
                Case "SELECT"
                    Dim hfAsteriskID, hfCallDate, hfCallerID, hfCallDuration, hfFileName As HiddenField
                    hfAsteriskID = CType(e.Item.FindControl("hfAsteriskID"), HiddenField)
                    hfCallDate = CType(e.Item.FindControl("hfCallDate"), HiddenField)
                    hfCallerID = CType(e.Item.FindControl("hfCallerID"), HiddenField)
                    hfCallDuration = CType(e.Item.FindControl("hfCallDuration"), HiddenField)
                    hfFileName = CType(e.Item.FindControl("hfFileName"), HiddenField)

                    If DownloadFileToLocal(SBCBL.CFileDBKeys.SBC_ASTERIA, _
                                           "http://agentsconnection.net/calls/" & hfFileName.Value, _
                                           hfFileName.Value) Then
                        Dim oTicketManager As New CTicketManager()
                        oTicketManager.AssignRecording(TicketID, hfAsteriskID.Value, SafeDate(hfCallDate), _
                                                      hfCallerID.Value, hfFileName.Value, _
                                                      SafeInteger(hfCallDuration.Value), UserSession.UserID)

                        closeMe(New WebsiteLibrary.CParentCallBackData("REFRESH_PAGE", False))

                    Else
                        ClientAlert("Fail To Assign Recording", True)
                    End If

            End Select
        End Sub

        Protected Sub grdCallLogs_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdCallLogs.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim oCallDetails As Data.DataRowView = CType(e.Item.DataItem, Data.DataRowView)
                Dim btnListen As Button = CType(e.Item.FindControl("btnListen"), Button)

                Dim sLink As String = "http://agentsconnection.net/calls/" & SafeString(oCallDetails("filename"))
                btnListen.OnClientClick = String.Format("window.open('{0}'); return false;", sLink)

                Dim lblCallDate As Label = CType(e.Item.FindControl("lblCallDate"), Label)
                If SafeString(oCallDetails("calldate")) <> "" Then
                    '' Convert Mid-Atlantic to EST
                    lblCallDate.Text = SafeString(oCallDetails("calldate"))
                End If
            End If
        End Sub
    End Class
End Namespace