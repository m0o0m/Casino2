Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data
Imports System.Xml

Namespace SBCSuperAdmin
    Partial Class TeaserRuleEdit
        Inherits SBCBL.UI.CSBCUserControl

        Public Event ButtonClick(ByVal sButtonType As String)

#Region "Properties"
        Public Property TeaserRuleID() As String
            Get
                Return SafeString(ViewState("_TEASER_RULE_ID"))
            End Get
            Set(ByVal value As String)
                ViewState("_TEASER_RULE_ID") = value
            End Set
        End Property
#End Region

#Region "Methods"
        Private Function checkCondition() As Boolean
            If ddlMinTeam.Value = "" Then
                ClientAlert("Min Team Is Required", True)
                ddlMinTeam.Focus()
                Return False
            End If

            If ddlMaxTeam.Value = "" Then
                ClientAlert("Max Team Is Required", True)
                ddlMaxTeam.Focus()
                Return False
            End If

            If SafeInteger(ddlMaxTeam.Value) < SafeInteger(ddlMinTeam.Value) Then
                ClientAlert("Max Team Must Be Greater Than Min Team", True)
                ddlMaxTeam.Focus()
                Return False
            End If

            If ddlBasketballPoint.Value = "" AndAlso ddlFootballPoint.Value = "" Then
                ClientAlert("Basketball Point Or Football Point Must Be Have Value", True)
                ddlBasketballPoint.Focus()
                Return False
            End If

            If (New CTeaserRuleManager()).IsExistName(SafeString(txtName.Text), TeaserRuleID, SBCBL.std.GetSiteType) Then
                ClientAlert("This Rule Name Has Already Existed", True)
                txtName.Focus()
                Return False
            End If

            Return True
        End Function

        Public Sub ClearTeaserRuleInfo()
            TeaserRuleID = ""
            txtName.Text = ""
            ddlMinTeam.Value = ""
            ddlMaxTeam.Value = ""
            ddlBasketballPoint.Value = ""
            ddlFootballPoint.Value = ""
            chkIsTiesLose.Checked = False
        End Sub

        Public Function LoadTeaserRuleInfo(ByVal psTeaserRuleID As String) As Boolean
            TeaserRuleID = psTeaserRuleID

            Dim oTeaserRuleManager As New CTeaserRuleManager()
            Dim oData As DataTable = oTeaserRuleManager.GetByID(TeaserRuleID)

            If oData Is Nothing OrElse oData.Rows.Count = 0 Then
                ClearTeaserRuleInfo()
                Return False
            End If

            With oData
                txtName.Text = SafeString(.Rows(0)("TeaserRuleName"))
                ddlMinTeam.Value = SafeString(.Rows(0)("MinTeam"))
                ddlMaxTeam.Value = SafeString(.Rows(0)("MaxTeam"))
                ddlBasketballPoint.Value = SafeString(SafeDouble(.Rows(0)("BasketballPoint")))
                ddlFootballPoint.Value = SafeString(SafeDouble(.Rows(0)("FootballPoint")))
                chkIsTiesLose.Checked = SafeString(.Rows(0)("IsTiesLose")) = "Y"
            End With

            Return True
        End Function
#End Region

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If Not checkCondition() Then
                Return
            End If

            Dim bSuccess As Boolean = False
            Dim oTeaserRuleManager As New CTeaserRuleManager()

            If TeaserRuleID <> "" Then 'Update
                bSuccess = oTeaserRuleManager.UpdateTeaserRule(TeaserRuleID, SafeString(txtName.Text), _
                SafeInteger(ddlMinTeam.Value), SafeInteger(ddlMaxTeam.Value), ddlBasketballPoint.Value, _
                ddlFootballPoint.Value, chkIsTiesLose.Checked, UserSession.UserID, SBCBL.std.GetSiteType)
            Else 'Insert
                bSuccess = oTeaserRuleManager.InsertTeaserRule(newGUID, SafeString(txtName.Text), _
                SafeInteger(ddlMinTeam.Value), SafeInteger(ddlMaxTeam.Value), ddlBasketballPoint.Value, _
                ddlFootballPoint.Value, chkIsTiesLose.Checked, UserSession.UserID, SBCBL.std.GetSiteType)
            End If

            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            ClearTeaserRuleInfo()
            RaiseEvent ButtonClick("CANCEL")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindData()
            End If
        End Sub

        Private Sub bindData()
            '' Bind MinTeam
            ddlMinTeam.DataSource = UserSession.SysSettings("Teaser Teams")
            ddlMinTeam.DataValueField = "Value"
            ddlMinTeam.DataTextField = "Value"
            ddlMinTeam.DataBind()

            '' Bind MaxTeam
            ddlMaxTeam.DataSource = UserSession.SysSettings("Teaser Teams")
            ddlMaxTeam.DataValueField = "Value"
            ddlMaxTeam.DataTextField = "Value"
            ddlMaxTeam.DataBind()

            '' Bind Basketball Points
            ddlBasketballPoint.DataSource = UserSession.SysSettings("Basketball Teaser Points")
            ddlBasketballPoint.DataValueField = "Value"
            ddlBasketballPoint.DataTextField = "Value"
            ddlBasketballPoint.DataBind()

            '' Bind Football Points
            ddlFootballPoint.DataSource = UserSession.SysSettings("Football Teaser Points")
            ddlFootballPoint.DataValueField = "Value"
            ddlFootballPoint.DataTextField = "Value"
            ddlFootballPoint.DataBind()

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If TeaserRuleID <> "" Then
                btnSave.Text = "Update"
                btnSave.ToolTip = "Update TeaserRule"
            Else
                btnSave.Text = "Add"
                btnSave.ToolTip = "Create new TeaserRule"
            End If
        End Sub

    End Class
End Namespace