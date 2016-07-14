Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class CallCenterAgentEdit
        Inherits SBCBL.UI.CSBCUserControl

        Public Event ButtonClick(ByVal sButtonType As String)

#Region "Properties"
        Public Property CallCenterAgentID() As String
            Get
                Return SafeString(SafeString(ViewState("_EDIT_CALL_CENTER_AGENTID")).Split("|"c)(0))
            End Get
            Set(ByVal value As String)
                ViewState("_EDIT_CALL_CENTER_AGENTID") = value
            End Set
        End Property

        Public Property SiteType() As String
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As String)
                ViewState("SiteType") = value
            End Set
        End Property
#End Region

#Region "Methods"
        Private Function checkCondition() As Boolean
            If SafeString(txtName.Text) = "" Then
                ClientAlert("Name Is Required", True)
                txtName.Focus()
                Return False
            End If
            If (New CCallCenterAgentManager).IsExistedLogin(txtLogin.Text, CallCenterAgentID, SBCBL.std.GetSiteType) Then
                ClientAlert("Login Is Existed. Please Try A Different Login", True)
                txtLogin.Focus()
                Return False
            End If
            If CallCenterAgentID = "" Then
                If SafeString(psdPassword.Password) = "" Then
                    ClientAlert("Password Is Required", True)
                    psdPassword.Focus()
                    Return False
                End If

                If Not ValidPassword(psdPassword.Password) Then
                    ClientAlert("Password Is Not Valid", True)
                    psdPassword.Focus()
                    Return False
                End If
            End If
            If String.IsNullOrEmpty(txtPhoneExt.Text) Then
                ClientAlert("Phone Extension  Is Required")
                txtPhoneExt.Focus()
                Return False
            End If
            If (New CCallCenterAgentManager).IsExistedPhoneExtension(txtPhoneExt.Text, CallCenterAgentID) Then
                ClientAlert("Phone Extension Is Existed. Please Try A Different Number.", True)
                txtPhoneExt.Focus()
                Return False
            End If
            Return True
        End Function

        Public Sub ClearAgentInfo()
            CallCenterAgentID = ""
            txtLogin.Text = ""
            txtName.Text = ""
            psdPassword.Password = ""
            ddlTimeZone.SelectedIndex = -1
            chkIsLocked.Checked = False
            txtLockReason.Text = ""
            txtPhoneExt.Text = ""
        End Sub

        Public Function LoadAgentInfo(ByVal psCallCenterAgentID As String) As Boolean
            CallCenterAgentID = psCallCenterAgentID

            Dim oAgentManager As New CCallCenterAgentManager()
            Dim oData As DataTable = oAgentManager.GetByID(CallCenterAgentID)

            If oData Is Nothing OrElse oData.Rows.Count = 0 Then
                ClearAgentInfo()
                Return False
            End If

            With oData
                txtLogin.Text = SafeString(.Rows(0)("Login"))
                txtName.Text = SafeString(.Rows(0)("Name"))
                ddlTimeZone.Value = SafeString(.Rows(0)("TimeZone"))
                txtPhoneExt.Text = SafeString(.Rows(0)("PhoneExtension"))
                chkIsLocked.Checked = SafeString(.Rows(0)("IsLocked")) = "Y"
                txtLockReason.Text = SafeString(.Rows(0)("LockReason"))
            End With

            Return True
        End Function
#End Region

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            If Not checkCondition() Then
                Return
            End If

            Dim bSuccess As Boolean = False
            Dim oAgentManager As New CCallCenterAgentManager()
            If CallCenterAgentID <> "" Then 'Update
                bSuccess = oAgentManager.UpdateAgent(CallCenterAgentID, SafeString(txtName.Text), _
                    SafeString(txtLogin.Text), SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), _
                    chkIsLocked.Checked, SafeString(txtLockReason.Text), txtPhoneExt.Text, UserSession.UserID)

                ClientAlert("Successfully Updated", True)
            Else 'Insert
                bSuccess = oAgentManager.InsertAgent(newGUID, SafeString(txtName.Text), SafeString(txtLogin.Text), _
                    SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), _
                    chkIsLocked.Checked, SafeString(txtLockReason.Text), txtPhoneExt.Text, UserSession.UserID, SiteType.ToString())

                ClientAlert("Successfully Saved", True)
            End If

            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            ClearAgentInfo()
            RaiseEvent ButtonClick("CANCEL")
        End Sub

        Protected Sub ibtGenerateLogin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtGenerateLogin.Click
            Dim sName As String = SafeString(txtName.Text)

            If sName = "" Then
                ClientAlert("Name Is Required", True)
                txtName.Focus()
                Return
            End If

            txtLogin.Text = (New CPlayerManager).AutoGenerateLogin(sName, SBCBL.std.GetSiteType)
        End Sub

        Protected Sub txtName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.TextChanged
            If CallCenterAgentID <> "" Then
                Return
            End If

            ibtGenerateLogin_Click(Nothing, Nothing)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindTimeZones()
            End If
        End Sub

        Private Sub bindTimeZones()
            ddlTimeZone.DataSource = UserSession.SysSettings("TimeZone")
            ddlTimeZone.DataTextField = "Key"
            ddlTimeZone.DataValueField = "Value"
            ddlTimeZone.DataBind()
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If CallCenterAgentID <> "" Then
                btnSave.Text = "Update"
                btnSave.ToolTip = "Update call center agent"
            Else
                btnSave.Text = "Add"
                btnSave.ToolTip = "Create new call center agent"
            End If

            ibtGenerateLogin.Visible = CallCenterAgentID <> ""
        End Sub

    End Class

End Namespace