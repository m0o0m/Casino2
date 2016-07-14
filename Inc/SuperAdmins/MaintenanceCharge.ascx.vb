Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Imports System.Linq


Namespace SBCSuperAdmin
    Partial Class MaintenanceCharge
        Inherits SBCBL.UI.CSBCUserControl

        Private _nRowCount As Integer = 0
        Private _nTotalActiveAccount As Integer = 0
        Private _nTotalWeeklyAmount As Double = 0
        Private _nTotalNotPaidAmount As Double = 0

        Public ReadOnly Property WeeklyCharge() As Double
            Get
                Dim oAgentManager As New CAgentManager()
                Dim dtAgent As DataTable = oAgentManager.GetByID(ddlPAgents.SelectedValue)
                If dtAgent IsNot Nothing AndAlso dtAgent.Rows.Count > 0 Then
                    Return SafeDouble(dtAgent.Rows(0)("WeeklyCharge"))
                End If

                Return 0
            End Get
        End Property

        Public ReadOnly Property SuperAdminID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    If UserSession.SuperAdminInfo.IsPartner Then
                        ' Return main SuperAdminID
                        Return UserSession.SuperAdminInfo.PartnerOf
                    Else
                        ' Return SuperAdminID
                        Return UserSession.UserID
                    End If
                Else
                    ' only superadmin can use this control, return newguid to prevent SQL error
                    Return newGUID()
                End If
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindAgent()
                txtWeeklyCharge.Text = FormatNumber(WeeklyCharge, GetRoundMidPoint) 'FormatNumber(UserSession.SuperAdminInfo.ChargePerAccount, GetRoundMidPoint)
                bindWeek()
                BindGrid()

            End If
        End Sub

#Region "Bind Data"
        Private Sub bindWeek()
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

        Public Sub BindAgent()
            Dim dtParents As DataTable = (New CAgentManager).GetAllSuperAgents(SuperAdminID)
            ddlPAgents.DataValueField = "AgentID"
            ddlPAgents.DataTextField = "AgentName"
            dtParents.DefaultView.Sort = "AgentName"
            ddlPAgents.DataSource = dtParents
            ddlPAgents.DataBind()
        End Sub


        Private Sub BindGrid()
            Dim oWeeklyChargeManager As New CWeeklyChargeManager()
            Dim oTblWeekly As DataTable

            Dim agentId = ddlPAgents.SelectedValue
            If String.IsNullOrEmpty(agentId) Then
                oTblWeekly = oWeeklyChargeManager.GetWeeklyCharge(SuperAdminID, SafeDate(ddlWeeks.Value))
            Else
                oTblWeekly = oWeeklyChargeManager.GetWeeklyChargeByAgentID(SuperAdminID, agentId, SafeDate(ddlWeeks.Value))
            End If

            If oTblWeekly IsNot Nothing AndAlso oTblWeekly.Rows.Count > 0 Then
                Dim odrTotal As Data.DataRow = oTblWeekly.NewRow
                odrTotal("FullName") = "Total(s):"
                oTblWeekly.Rows.Add(odrTotal)

                _nRowCount = oTblWeekly.Rows.Count
            End If

            dgWeeklyMaitenance.DataSource = oTblWeekly
            dgWeeklyMaitenance.DataBind()

        End Sub
#End Region
        
#Region "Page's Events"
        Protected Sub dgWeeklyMaitenance_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgWeeklyMaitenance.ItemCommand
            Select Case UCase(e.CommandName)
                Case "DETAILS"
                    Dim lblDetails As Label = CType(e.Item.FindControl("lblDetails"), Label)
                    lblDetails.Visible = Not lblDetails.Visible
                Case "SETPAID"
                    Dim weeklyChargeId = e.CommandArgument
                    Dim oWeeklyChargeManager As New CWeeklyChargeManager()
                    Dim bSuccsess = oWeeklyChargeManager.UpdateWeeklyChargePaidDate(SuperAdminID, SafeString(ddlPAgents.SelectedValue), weeklyChargeId, DateTime.UtcNow)

                    If bSuccsess Then
                        BindGrid()
                    Else
                        ClientAlert("Failed to update Paid Date")
                    End If
            End Select
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            'If SafeDouble(txtWeeklyCharge.Text) = 0 Then
            '    ClientAlert("Weekly Charge Must Be Greater Than 0")
            '    Return
            'End If

            If String.IsNullOrEmpty(ddlPAgents.SelectedValue) Then
                ClientAlert("Please select an Agent")
                Return
            End If

            Dim oAgentManager As New CAgentManager()
            If oAgentManager.UpdateWeeklyCharge(ddlPAgents.SelectedValue, SafeDouble(txtWeeklyCharge.Text), UserSession.UserID) Then
                BindGrid()
                ClientAlert("Successfully Updated")
            Else
                ClientAlert("Unsuccessfully Updated")
            End If

        End Sub

        Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click

            If WeeklyCharge <= 0 Then
                ClientAlert("Weekly Charge Must Be Greater Than 0")
                Return
            End If
            Dim bSuccsess As Boolean = False
            Dim sDetails As String = ""
            Dim nWeeklyMaintenance As Double
            Dim nActivePlayers As Integer = 0
            Dim oTblActivePlayers As DataTable
            Dim oAgentManager As New CAgentManager()
            Dim oWeeklyChargeManager As New CWeeklyChargeManager()
            oTblActivePlayers = oAgentManager.GetActivePlayers(SafeString(ddlPAgents.SelectedValue), SafeDate(ddlWeeks.Value), _
                                                               SafeDate(ddlWeeks.Value).AddDays(6))
            If oTblActivePlayers IsNot Nothing Then
                nActivePlayers = oTblActivePlayers.Rows.Count
            End If
            sDetails = GetDetails(oTblActivePlayers)
            nWeeklyMaintenance = SafeRound(WeeklyCharge * nActivePlayers)

            Dim oTblWeekly As DataTable = oWeeklyChargeManager.GetWeeklyChargeByAgentID(ddlPAgents.SelectedValue, SafeDate(ddlWeeks.Value), _
                                                                                        SafeDate(ddlWeeks.Value).AddDays(6))

            If oTblWeekly Is Nothing OrElse oTblWeekly.Rows.Count = 0 Then
                bSuccsess = oWeeklyChargeManager.AddWeeklyCharge(SuperAdminID, SafeString(ddlPAgents.SelectedValue), _
                                                        nActivePlayers, nWeeklyMaintenance, sDetails, SafeDate(ddlWeeks.Value).AddDays(6), SuperAdminID)
            Else
                bSuccsess = oWeeklyChargeManager.UpdateWeeklyCharge(SuperAdminID, SafeString(ddlPAgents.SelectedValue), _
                                                     nActivePlayers, nWeeklyMaintenance, sDetails, SuperAdminID, SafeDate(ddlWeeks.Value), _
                                                     SafeDate(ddlWeeks.Value).AddDays(6))
            End If
            txtWeeklyCharge.Text = FormatNumber(WeeklyCharge, GetRoundMidPoint)
            If bSuccsess Then
                BindGrid()
                ClientAlert("Successfully Processed")
            Else
                ClientAlert("Unsuccessfully Processed")
            End If

        End Sub

        Protected Sub ddlWeeks_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWeeks.SelectedIndexChanged
            BindGrid()

        End Sub

        Protected Sub ddlPAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPAgents.SelectedIndexChanged
            txtWeeklyCharge.Text = FormatNumber(WeeklyCharge, GetRoundMidPoint)
            BindGrid()
        End Sub
#End Region

        Private Function GetDetails(ByVal poTblActivePlayers As DataTable) As String
            Dim sResult As String = ""
            If poTblActivePlayers IsNot Nothing Then
                sResult = "<table width='100%'><tr><td class='tableheading' style='text-align: center;height:12px'>Agent Login (Name)</td><td class='tableheading' style='text-align: center;'>Player Login (Name)</td></tr>"

                Dim sAgentName As String = ""
                For Each oDR As DataRow In poTblActivePlayers.Rows
                    If sAgentName <> SafeString(oDR("AgentFullName")) Then
                        sAgentName = SafeString(oDR("AgentFullName"))
                        sResult &= String.Format("<tr><td style='text-align: left;'>{0}</td><td>{1}</td></tr>", _
                                            sAgentName, SafeString(oDR("PlayerFullName")))
                    Else
                        sResult &= String.Format("<tr><td style='text-align: left;'>{0}</td><td>{1}</td></tr>", _
                                            "", SafeString(oDR("PlayerFullName")))
                    End If
                Next

                sResult &= "</table>"
            End If

            Return sResult
        End Function

        Private Function AddSpace(ByVal pnLevel As Integer) As String
            Dim sSpace As String = ""

            If pnLevel > 0 Then
                For nIndex As Integer = 0 To pnLevel
                    sSpace &= "---"
                Next
            End If

            Return sSpace
        End Function

        Protected Sub dgWeeklyMaitenance_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgWeeklyMaitenance.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oDr As Data.DataRowView = CType(e.Item.DataItem, Data.DataRowView)

                Dim nActivePlayers As Integer
                Dim nWeeklyCharge As Double
                nActivePlayers = SafeInteger(oDr("ActivePlayers"))
                nWeeklyCharge = SafeDouble(oDr("ChargeAmount"))

                Dim lbtnDetails As LinkButton
                Dim lblWeeklyMaintenance As Label
                lbtnDetails = CType(e.Item.FindControl("lbtnDetails"), LinkButton)
                lblWeeklyMaintenance = CType(e.Item.FindControl("lblWeeklyMaintenance"), Label)

                If e.Item.ItemIndex <> _nRowCount - 1 Then
                    lbtnDetails.Text = SafeString(nActivePlayers)
                    lblWeeklyMaintenance.Text = FormatNumber(nWeeklyCharge, GetRoundMidPoint)
                Else
                    lbtnDetails.Text = SafeString(_nTotalActiveAccount)
                    lblWeeklyMaintenance.Text = "Total: " & FormatNumber(_nTotalWeeklyAmount, GetRoundMidPoint)

                    Dim lblTotalNotPaid = CType(e.Item.FindControl("lblTotalNotPaid"), Label)
                    lblTotalNotPaid.Visible = True
                    lblTotalNotPaid.Text = "<br />Total Unpaid: " & FormatNumber(_nTotalNotPaidAmount, GetRoundMidPoint)

                    e.Item.Cells(0).Font.Bold = True
                    e.Item.Cells(1).Font.Bold = True
                    e.Item.Cells(2).Font.Bold = True
                    e.Item.Cells(3).Font.Bold = True
                End If

                _nTotalActiveAccount += nActivePlayers
                _nTotalWeeklyAmount += nWeeklyCharge
                If oDr("PaidDate") Is DBNull.Value Then
                    _nTotalNotPaidAmount += nWeeklyCharge
                End If

            End If
        End Sub
      
    End Class
End Namespace