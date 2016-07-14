Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data

Namespace SBCAgents
    Partial Class Inc_OddsRulesSetup
        Inherits SBCBL.UI.CSBCUserControl

        Private Const YES As String = "Y"
        Private Const LOWERTHAN As String = "LowerThan"
        Private Const GREATERTHAN As String = "GreaterThan"
        Private Const INCREASE As String = "Increase"
        Private Const ISODDRULELOCKED As String = "IsOddRuleLocked"

        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Dim poAgentID As String = ""
        Dim oSysManager As New CSysSettingManager()

#Region "property"
        Private Property OddRuleID() As String
            Get
                Return SafeString(ViewState("OddRuleID"))
            End Get
            Set(ByVal value As String)
                ViewState("OddRuleID") = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return SafeString(ViewState("Title"))
            End Get
            Set(ByVal value As String)
                ViewState("Title") = value
            End Set
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Title = "Odd Rule"
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    poAgentID = SafeString(UserSession.UserID)
                    rowAgent.Visible = False
                ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    BindSuperAgents()
                    poAgentID = SafeString(ddlAgents.SelectedItem.Value)
                    rowAgent.Visible = True
                    If SafeString(ddlAgents.SelectedItem.Value) <> "" Then
                        dgOddsRules.Visible = True
                    Else
                        dgOddsRules.Visible = False
                    End If
                End If
                bindOddsRules(poAgentID)
            End If
        End Sub

        Private Sub bindOddsRules(ByVal poAgentID As String)
            Dim oOddRuleManager As New COddRuleManager()
            dgOddsRules.DataSource = oOddRuleManager.GetOddRulesByAgentID(poAgentID)
            dgOddsRules.DataBind()

        End Sub

        Private Sub BindSuperAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))
            Next
            ddlAgents.Items.Insert(0, "")
            ddlAgents.SelectedValue = SafeString(Request("AgentID"))
        End Sub


        Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
            Try
                If checkValid() Then
                    Dim oOddRuleManager As New COddRuleManager()
                    If UserSession.UserType = SBCBL.EUserType.Agent Then
                        poAgentID = SafeString(UserSession.UserID)
                    ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                        poAgentID = SafeString(ddlAgents.SelectedItem.Value)
                    End If
                    oOddRuleManager.InsertOddRule(poAgentID, SafeInteger(txtLowerThan.Text), SafeInteger(txtGreaterThan.Text), SafeDouble(txtIncrease.Text), UserSession.UserID, IIf(SafeBoolean(chkOddRuleLocked.Checked), "Y", "N"))
                    bindOddsRules(poAgentID)
                    OddRuleID = Nothing
                    btnUpdate.Enabled = False
                    clearOddRuleInfor()
                    dgOddsRules.SelectedIndex = -1
                End If
            Catch ex As Exception
                LogError(log, "Cannot Add new value settings.", ex)
            End Try
        End Sub

        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click

            If checkValid() Then
                Dim oOddRuleManager As New COddRuleManager()
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    poAgentID = SafeString(UserSession.UserID)
                ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    poAgentID = SafeString(ddlAgents.SelectedItem.Value)
                End If
                oOddRuleManager.UpdateOddRuleByID(OddRuleID, SafeInteger(txtLowerThan.Text), SafeInteger(txtGreaterThan.Text), SafeInteger(txtIncrease.Text), IIf(SafeBoolean(chkOddRuleLocked.Checked), "Y", "N"))
                bindOddsRules(poAgentID)
                OddRuleID = Nothing
                btnUpdate.Enabled = False
                btnAdd.Enabled = True
                clearOddRuleInfor()
                dgOddsRules.SelectedIndex = -1
            End If

        End Sub

        Protected Sub dgOddsRules_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgOddsRules.ItemCommand
            Dim oOddRuleManager As New COddRuleManager()
            OddRuleID = SafeString(e.CommandArgument)

            Select Case UCase(e.CommandName)
                Case "DELETEODDSRULES"
                    If UserSession.UserType = SBCBL.EUserType.Agent Then
                        poAgentID = SafeString(UserSession.UserID)
                    ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                        poAgentID = SafeString(ddlAgents.SelectedItem.Value)
                    End If
                    oOddRuleManager.DeleteOddRule(OddRuleID, UserSession.UserID)
                    OddRuleID = Nothing
                    UserSession.Cache.ClearGameOddRule(poAgentID)
                    bindOddsRules(poAgentID)

                    dgOddsRules.SelectedIndex = -1
                Case "EDITODDSRULES"
                    If UserSession.UserType = SBCBL.EUserType.Agent Then
                        poAgentID = SafeString(UserSession.UserID)
                    ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                        poAgentID = SafeString(ddlAgents.SelectedItem.Value)
                    End If
                    Dim oOddRule As New COddsRule(oOddRuleManager.GetOddRulesByOddRuleID(OddRuleID).Rows(0), True)
                    bindOddRuleInfor(oOddRule.GreaterThan, oOddRule.Increase, oOddRule.LowerThan, oOddRule.IsOddRuleLocked)
                    btnUpdate.Enabled = True
                    btnAdd.Enabled = False
                    dgOddsRules.SelectedIndex = e.Item.ItemIndex
            End Select


        End Sub

        Private Function checkValid() As Boolean

            If UserSession.UserType = SBCBL.EUserType.SuperAdmin AndAlso String.IsNullOrEmpty(ddlAgents.Text) Then
                ClientAlert("Please Select Agent")
                ddlAgents.Focus()
                Return False
            End If

            If String.IsNullOrEmpty(txtGreaterThan.Text) Or String.IsNullOrEmpty(txtLowerThan.Text) Then
                ClientAlert("Please Input 'From' And 'To', Remember 'From' < 'To' ")
                txtGreaterThan.Focus()
                Return False
            End If
            If Not String.IsNullOrEmpty(txtGreaterThan.Text) AndAlso Not String.IsNullOrEmpty(txtLowerThan.Text) Then
                If SafeDouble(txtGreaterThan.Text) >= SafeDouble(txtLowerThan.Text) Then
                    ClientAlert("The Range Is Invalid")
                    txtLowerThan.Focus()
                    Return False
                End If
                If Not chkOddRuleLocked.Checked And SafeSingle(txtIncrease.Text) = 0 Then
                    ClientAlert("Please Check Lock Or Input Increase")
                    txtIncrease.Focus()
                    Return False
                End If
            End If

            Return True

        End Function

        Private Sub clearOddRuleInfor()
            txtGreaterThan.Text = ""
            txtIncrease.Text = ""
            txtLowerThan.Text = ""
            chkOddRuleLocked.Checked = False
            UserSession.Cache.ClearGameOddRule(poAgentID)
        End Sub

        Private Sub bindOddRuleInfor(ByVal psGreaterThan As Double, ByVal psIncrease As Single, ByVal psLowerThan As Double, ByVal pbOddRuleLocked As Boolean)
            txtGreaterThan.Text = SafeString(psGreaterThan)
            txtIncrease.Text = SafeString(psIncrease)
            txtLowerThan.Text = SafeString(psLowerThan)
            chkOddRuleLocked.Checked = SafeBoolean(pbOddRuleLocked)
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            If SafeString(ddlAgents.SelectedItem.Value) <> "" Then
                Dim oOddRuleManager As New COddRuleManager()
                dgOddsRules.DataSource = oOddRuleManager.GetOddRulesByAgentID (SafeString(ddlAgents.SelectedItem.Value))
                dgOddsRules.DataBind()
                dgOddsRules.Visible = True
            Else
                dgOddsRules.Visible = False
            End If
        End Sub
    End Class
End Namespace