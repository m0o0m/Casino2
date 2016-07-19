Imports SBCBL
Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class SuperEdit
        Inherits SBCBL.UI.CSBCUserControl
        Private _sMaskFone As String = "___-___-____"
        Public Event ButtonClick(ByVal sButtonType As String)

#Region "Properties"
        Public Property SuperAdminID() As String
            Get
                Return SafeString(ViewState("SuperAdminID"))
            End Get
            Set(ByVal value As String)
                ViewState("SuperAdminID") = value
            End Set
        End Property

        Public Property SiteType() As CEnums.ESiteType
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As CEnums.ESiteType)
                ViewState("SiteType") = value
            End Set
        End Property

#End Region

#Region "Methods"
        Private Function checkCondition() As Boolean
            If SafeString(txtName.Text) = "" Then
                ClientAlert("Name is required", True)
                txtName.Focus()
                Return False
            End If

            If chkManager.Checked Then
                If ddlUrls.Value = "" Then
                    ClientAlert("Site Url is required", True)
                    ddlUrls.Focus()
                    Return False
                End If
            End If

            If SuperAdminID = "" Then
                If SafeString(psdAdminPassword.Password) = "" Then
                    ClientAlert("Password is required", True)
                    psdAdminPassword.Focus()
                    Return False
                End If

                If Not ValidPassword(psdAdminPassword.Password) Then
                    ClientAlert("Password is not valid", True)
                    psdAdminPassword.Focus()
                    Return False
                End If
            End If

            If (New CSuperUserManager).IsExistedLogin(SafeString(txtAdminLogin.Text), SuperAdminID, SBCBL.std.GetSiteType) Then
                ClientAlert("This login name is existed", True)
                Return False
            End If

            If SiteType = CEnums.ESiteType.SBS Then


                If Not txtCustomerService.Text.Equals(_sMaskFone) AndAlso (New CSuperUserManager).IsExistedCustomerService(SafeString(txtCustomerService.Text), SBCBL.std.GetSiteType, SuperAdminID) Then
                    ClientAlert("This Customer Service name is existed", True)
                    Return False
                End If
                If Not txtWagering.Text.Equals(_sMaskFone) AndAlso (New CSuperUserManager).IsExistedWagering(SafeString(txtWagering.Text), SBCBL.std.GetSiteType, SuperAdminID) Then
                    ClientAlert("This Wagering name is existed", True)
                    Return False
                End If
            End If


            Return True
        End Function

        Public Sub ClearSuperInfo()
            SuperAdminID = ""
            txtAdminLogin.Text = ""
            txtName.Text = ""
            txtCustomerService.Text = ""
            txtWagering.Text = ""
            psdAdminPassword.Password = ""
            ddlTimeZone.SelectedIndex = -1
            ddlUrls.Value = ""
            ddlUrls2.Value = ""
            'chkDefaultRules.Checked = False
            chkIsLocked.Checked = False
            txtLockReason.Text = ""

        End Sub

        Public Function LoadSuperInfo(ByVal psSuperAdminID As String) As Boolean
            SuperAdminID = psSuperAdminID

            Dim oSuperManager As New CSuperUserManager()
            Dim oData As DataTable = oSuperManager.GetSuperByID(psSuperAdminID)

            If oData Is Nothing OrElse oData.Rows.Count = 0 Then
                ClearSuperInfo()
                Return False
            End If

            With oData
                txtAdminLogin.Text = SafeString(.Rows(0)("Login"))
                txtName.Text = SafeString(.Rows(0)("Name"))
                ddlTimeZone.Value = SafeString(.Rows(0)("TimeZone"))
                ddlUrls.Value = SafeString(.Rows(0)("WhiteLabelSettingID"))
                chkIsLocked.Checked = SafeString(.Rows(0)("IsLocked")) = "Y"
                txtLockReason.Text = SafeString(.Rows(0)("LockReason"))
                chkManager.Checked = SafeString(.Rows(0)("IsManager")) = "Y"
                'chkDefaultRules.Checked = SafeBoolean(.Rows(0)("IsUseDefaultRules"))
                If SiteType = CEnums.ESiteType.SBS Then
                    txtWagering.Text = SafeString(.Rows(0)("Wagering"))
                    txtCustomerService.Text = SafeString(.Rows(0)("CustomerService"))
                End If
                ddlUrls.Text = SafeString(.Rows(0)("SiteURL"))
                ddlUrls2.Text = SafeString(.Rows(0)("SiteURLBackup"))
            End With
            Return True
        End Function
#End Region

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If Not checkCondition() Then
                Return
            End If

            Dim bSuccess As Boolean = False
            Dim oSuperManager As New CSuperUserManager()

            If SuperAdminID <> "" Then 'Update
                bSuccess = oSuperManager.UpdateSuper(SuperAdminID, SafeString(txtName.Text), _
                SafeString(txtAdminLogin.Text), SafeString(psdAdminPassword.Password), SafeInteger(ddlTimeZone.Value), ddlUrls.Value, ddlUrls2.Value, _
                chkIsLocked.Checked, chkManager.Checked, SafeString(txtLockReason.Text), UserSession.UserID, txtWagering.Text, txtCustomerService.Text)
                ClientAlert("Update user's info successfully", True)
            Else 'Insert
                bSuccess = oSuperManager.InsertSuper(newGUID, SafeString(txtName.Text), SafeString(txtAdminLogin.Text), _
                SafeString(psdAdminPassword.Password), ddlUrls.Value, ddlUrls2.Value, SafeInteger(ddlTimeZone.Value), _
                chkIsLocked.Checked, chkManager.Checked, SafeString(txtLockReason.Text), UserSession.UserID, SiteType.ToString, False, txtWagering.Text, txtCustomerService.Text)
                ClientAlert("Save user's info successfully", True)
            End If

            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            ClearSuperInfo()
            RaiseEvent ButtonClick("CANCEL")
        End Sub

        Protected Sub ibtGenerateLogin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtGenerateLogin.Click
            Dim sName As String = SafeString(txtName.Text)

            If sName = "" Then
                ClientAlert("Name is required to auto generate login", True)
                txtName.Focus()
                Return
            End If

            txtAdminLogin.Text = (New CPlayerManager).AutoGenerateLogin(sName, SBCBL.std.GetSiteType)
        End Sub

        'Protected Sub txtName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.TextChanged
        '    If SuperAdminID <> "" Then
        '        Return
        '    End If

        '    ibtGenerateLogin_Click(Nothing, Nothing)
        'End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If SiteType = CEnums.ESiteType.SBS Then
                tdCustomerService.Visible = True
                tdWagering.Visible = True
            End If
            If Not IsPostBack Then
                bindTimeZones()
                bindUrls()
            End If
        End Sub

        Private Sub bindUrls()
            Dim odt As DataTable = SBCBL.CacheUtils.CWhiteLabelSettings.LoadAll(GetSiteType)
            ddlUrls.DataSource = odt
            ddlUrls.DataTextField = "SiteURL"
            ddlUrls.DataValueField = "SiteURL"
            ddlUrls.DataBind()
            ddlUrls2.DataSource = odt
            ddlUrls2.DataTextField = "SiteURL"
            ddlUrls2.DataValueField = "SiteURL"
            ddlUrls2.DataBind()
        End Sub

        Private Sub bindTimeZones()
            ddlTimeZone.DataSource = UserSession.SysSettings("TimeZone")
            ddlTimeZone.DataTextField = "Key"
            ddlTimeZone.DataValueField = "Value"
            ddlTimeZone.DataBind()
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If SuperAdminID <> "" Then
                btnSave.Text = "Update"
                btnSave.ToolTip = "Update super admin"
            Else
                btnSave.Text = "Add"
                btnSave.ToolTip = "Create super admin"
            End If

            'ibtGenerateLogin.Visible = SuperAdminID <> ""
        End Sub

    End Class
End Namespace