Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents
    Partial Class Inc_Agent_FixedSpreadMoney
        Inherits SBCBL.UI.CSBCUserControl
        Private ReadOnly _Category As String = "FixedSpreadMoney"
        Dim _Key As String = "SpreadMoney"
        Dim _HalfKey As String = "HalfSpreadMoney"
        Dim sMaxCircled As String = SBCBL.std.GetSiteType & " MaxCircled"
        Dim oSysManager As New CSysSettingManager()

#Region "Property"

        Public Property IsSuperAdmin() As Boolean
            Get
                If ViewState("ISSUPERADMIN") IsNot Nothing Then
                    Return CType(ViewState("ISSUPERADMIN"), Boolean)
                Else
                    Return False
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ISSUPERADMIN") = value
            End Set
        End Property

        Public ReadOnly Property UserID() As String
            Get
                If IsSuperAdmin Then
                    Return ddlAgents.SelectedValue & "FixexdSpreadMoney"
                Else
                    Return UserSession.UserID & "FixexdSpreadMoney"
                End If
            End Get

        End Property
#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                trAgents.Visible = Me.IsSuperAdmin
                hfIsSuperAdmin.Value = SafeString(Me.IsSuperAdmin)
            End If
        End Sub

#End Region

#Region "Controls Events"

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            LoadData()
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

            ' Dim nValue As Double = SafeDouble(txtSpreadMoney.Text)
            Dim nHValue As Double = SafeDouble(txtHalfSpreadMoney.Text)
            Dim nGameTotalValue As Double = SafeDouble(txtSpreadMoneyGT.Text)
            Dim nFlatSpreadMoney = SafeDouble(txtFlatSpreadMoney.Text)
            'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()

            Dim sAgentID As String = ""
            If Me.IsSuperAdmin Then
                sAgentID = SafeString(ddlAgents.SelectedValue)
            Else
                sAgentID = UserSession.UserID
            End If

            If sAgentID <> "" Then
                Dim bSuccess As Boolean = False

                ' If rdbtnSpreadMoney.Items(0).Selected Then
                '     If SafeDouble(txtSpreadMoney.Text) = 0 OrElse _
                ' txtSpreadMoney.Text = "" OrElse SafeDouble(txtHalfSpreadMoney.Text) = 0 _
                'OrElse txtSpreadMoneyGT.Text = "" OrElse txtHalfSpreadMoney.Text = "" Then
                '         ClientAlert("Please input all fields to save")
                '         txtSpreadMoney.Focus()
                '         Return
                '     End If
                If rdbtnSpreadMoney.Items(0).Selected() Then
                    If SafeDouble(txtFlatSpreadMoney.Text) = 0 OrElse txtFlatSpreadMoney.Text = "" Then
                        ClientAlert("Please Input Flat Spread Money")
                        txtFlatSpreadMoney.Focus()
                        Return
                    End If

                End If
                '  If oGameTypeOnOffManager.GetFixSpreadMoney(UserSession.UserID.ToString) > 0 Then
                'If oGameTypeOnOffManager.GetFixSpreadMoney(Me.UserID) > 0 Then
                '    bSuccess = oGameTypeOnOffManager.UpdateFixSpreadMoney(sAgentID, 0, nHValue, nGameTotalValue, _
                '                                                    nFlatSpreadMoney, rdbtnSpreadMoney.SelectedValue)
                'Else
                '    bSuccess = oGameTypeOnOffManager.InsertFixedSpreadMoney(sAgentID, newGUID, 0, _
                '                                        nHValue, nGameTotalValue, nFlatSpreadMoney, rdbtnSpreadMoney.SelectedValue)
                'End If
                If oSysManager.IsExistedCategory(Me.UserID) Then
                    bSuccess = oSysManager.UpdateValue(UserID, "", "HalfSpreadMoney", nHValue)
                    bSuccess = oSysManager.UpdateValue(UserID, "", "SpreadMoneyGT", nGameTotalValue)
                    bSuccess = oSysManager.UpdateValue(UserID, "", "FlatSpreadMoney", nFlatSpreadMoney)
                    bSuccess = oSysManager.UpdateValue(UserID, "", "IsFlatSpreadMoney", rdbtnSpreadMoney.SelectedValue)
                Else
                    bSuccess = oSysManager.AddSysSetting(UserID, "HalfSpreadMoney", nHValue)
                    bSuccess = oSysManager.AddSysSetting(UserID, "SpreadMoneyGT", nGameTotalValue)
                    bSuccess = oSysManager.AddSysSetting(UserID, "FlatSpreadMoney", nFlatSpreadMoney)
                    bSuccess = oSysManager.AddSysSetting(UserID, "IsFlatSpreadMoney", rdbtnSpreadMoney.SelectedValue)
                End If

                'UserSession.Cache.ClearFixSpreadMoney(sAgentID)
                UserSession.Cache.ClearSysSettings(UserID)
                If bSuccess Then
                    ClientAlert("Successfully saved ", True)
                Else
                    ClientAlert("Failed to Save Setting ", True)
                End If

            End If
        End Sub


#End Region

#Region "Functions"

        Public Sub BindCurrentUserData()
            LoadData()
        End Sub

        Public Sub bindAgents(ByVal psSuperAdminID As String)
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)

            ddlAgents.DataSource = oData
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub

        Private Sub LoadData()
            If UserID IsNot Nothing Then
                'Dim oFixSpreadMoney As CFixSpreadMoney = UserSession.Cache.GetFixSpreadMoney(psAgentID)

                If oSysManager.IsExistedCategory(UserID) Then
                    ' txtSpreadMoney.Text = SafeString(oFixSpreadMoney.NumFixedSpreadMoneyCurrent)
                    txtHalfSpreadMoney.Text = UserSession.Cache.GetSysSettings(UserID).GetDoubleValue("HalfSpreadMoney") 'SafeString(oFixSpreadMoney.NumFixedSpreadMoneyH)
                    txtSpreadMoneyGT.Text = UserSession.Cache.GetSysSettings(UserID).GetDoubleValue("SpreadMoneyGT")
                    txtFlatSpreadMoney.Text = UserSession.Cache.GetSysSettings(UserID).GetDoubleValue("FlatSpreadMoney") 'SafeSingle(oFixSpreadMoney.NumFlatSpreadMoney)
                    'If oFixSpreadMoney.UseFixedSpreadMoney Then
                    '    rdbtnSpreadMoney.Items(0).Selected = True
                    If UserSession.Cache.GetSysSettings(UserID).GetBooleanValue("IsFlatSpreadMoney") Then
                        rdbtnSpreadMoney.Items(0).Selected = True
                    Else
                        rdbtnSpreadMoney.Items(1).Selected = True
                    End If
                Else
                    resetControls()
                End If
            End If
        End Sub

        Private Sub resetControls()
            'txtSpreadMoney.Text = ""
            txtHalfSpreadMoney.Text = ""
            txtFlatSpreadMoney.Text = ""
        End Sub

#End Region

    End Class
End Namespace