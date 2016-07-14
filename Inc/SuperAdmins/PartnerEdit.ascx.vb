Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class PartnerEdit
        Inherits SBCBL.UI.CSBCUserControl

        Public Event ButtonClick(ByVal sButtonType As String)

#Region "Properties"
        Public Property PartnerID() As String
            Get
                Return SafeString(SafeString(ViewState("_EDIT_PARTNER_ID")).Split("|"c)(0))
            End Get
            Set(ByVal value As String)
                ViewState("_EDIT_PARTNER_ID") = value
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
            If (New CSuperUserManager).IsExistedLogin(txtLogin.Text, PartnerID, SBCBL.std.GetSiteType) Then
                ClientAlert("Login Has Already Existed. Please Try A Different Login", True)
                txtLogin.Focus()
                Return False
            End If
            If PartnerID = "" Then
                If SafeString(psdPassword.Password) = "" Then
                    ClientAlert("Password Is Required", True)
                    psdPassword.Focus()
                    Return False
                End If

                If Not ValidPassword(psdPassword.Password) Then
                    ClientAlert("Password Is Imvalid", True)
                    psdPassword.Focus()
                    Return False
                End If
            End If
            Return True
        End Function

        Public Sub ClearPartnerInfo()
            PartnerID = ""
            txtLogin.Text = ""
            txtName.Text = ""
            psdPassword.Password = ""
            ddlTimeZone.SelectedIndex = -1
            chkIsLocked.Checked = False
            txtLockReason.Text = ""
            
        End Sub

        Public Function LoadPartnerInfo(ByVal psPartnerID As String) As Boolean
            PartnerID = psPartnerID

            Dim oPartnerManager As New CSuperUserManager()
            Dim oData As DataTable = oPartnerManager.GetSuperByID(PartnerID)

            If oData Is Nothing OrElse oData.Rows.Count = 0 Then
                ClearPartnerInfo()
                Return False
            End If

            With oData
                txtLogin.Text = SafeString(.Rows(0)("Login"))
                txtName.Text = SafeString(.Rows(0)("Name"))
                ddlTimeZone.Value = SafeString(.Rows(0)("TimeZone"))
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
            Dim oPartnerManager As New CSuperUserManager()
            If PartnerID <> "" Then 'Update
                bSuccess = oPartnerManager.UpdateSuper(PartnerID, SafeString(txtName.Text), SafeString(txtLogin.Text), _
                                                       SafeString(psdPassword.Password), SafeInteger(ddlTimeZone.Value), "", "", _
                                                        chkIsLocked.Checked, False, SafeString(txtLockReason.Text), UserSession.UserID)

                ClientAlert("Successfully Updated", True)
            Else 'Insert
                bSuccess = oPartnerManager.InsertSuper(newGUID(), SafeString(txtName.Text), SafeString(txtLogin.Text), _
                    SafeString(psdPassword.Password), "", "", SafeInteger(ddlTimeZone.Value), _
                    chkIsLocked.Checked, False, SafeString(txtLockReason.Text), UserSession.UserID, SiteType.ToString(), True)

                ClientAlert("Successfully Saved", True)
            End If

            RaiseEvent ButtonClick(SafeString(IIf(bSuccess, "SAVE SUCCESSFUL", "SAVE UNSUCCESSFUL")))
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            ClearPartnerInfo()
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
            If PartnerID <> "" Then
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
            If PartnerID <> "" Then
                btnSave.Text = "Update"
                btnSave.ToolTip = "Update partner."
            Else
                btnSave.Text = "Add"
                btnSave.ToolTip = "Create new partner."
            End If

            ibtGenerateLogin.Visible = PartnerID <> ""
        End Sub

    End Class

End Namespace