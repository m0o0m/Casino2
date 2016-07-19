Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data

Namespace SBCSuperAdmin
    Partial Class SubjectEmailSetup
        Inherits SBCBL.UI.CSBCUserControl

        Public Property Edit() As Boolean
            Get
                Return SafeBoolean(ViewState("Edit"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("Edit") = value
            End Set
        End Property

        Public Property ItemIndex() As Integer
            Get
                If ViewState("ItemIndex") Is Nothing Then
                    Return -1
                Else
                    Return SafeInteger(ViewState("ItemIndex"))
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("ItemIndex") = value
            End Set
        End Property
#Region "Bind Data"
        Public Sub BindEmailSubject()
            Dim oCSubjectEmailManager As CSubjectEmailManager = New CSubjectEmailManager()
            dgSubjectEmail.DataSource = oCSubjectEmailManager.GetALLSubjectEmail()
            dgSubjectEmail.DataBind()
        End Sub
#End Region
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindEmailSubject()
            End If
        End Sub

        Protected Sub dgSubjectEmail_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgSubjectEmail.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.EditItem Then
                If Edit AndAlso e.Item.ItemIndex = ItemIndex Then
                    CType(e.Item.FindControl("lbtEditSubjectEmail"), LinkButton).Visible = False
                    CType(e.Item.FindControl("lbtCancelSubjectEmail"), LinkButton).Visible = True
                    CType(e.Item.FindControl("lbtUpdateSubjectEmail"), LinkButton).Visible = True
                Else
                    CType(e.Item.FindControl("lbtEditSubjectEmail"), LinkButton).Visible = True
                    CType(e.Item.FindControl("lbtCancelSubjectEmail"), LinkButton).Visible = False
                    CType(e.Item.FindControl("lbtUpdateSubjectEmail"), LinkButton).Visible = False
                End If
            End If
        End Sub

        Protected Sub dgSubjectEmail_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgSubjectEmail.ItemCommand
            Dim oCSubjectEmailManager As CSubjectEmailManager = New CSubjectEmailManager()
            Select Case e.CommandName
                Case "EDIT"
                    Edit = True
                    dgSubjectEmail.EditItemIndex = e.Item.ItemIndex
                    ItemIndex = e.Item.ItemIndex
                    BindEmailSubject()
                Case "CANCEL"
                    ResetGrid()
                Case "UPDATE"
                    Dim sSubject As String = CType(e.Item.FindControl("txtSubject"), TextBox).Text
                    Dim sId As String = e.CommandArgument
                    If ValidSubjectEmail(sId, sSubject) Then
                        oCSubjectEmailManager.UpdateSubectEmail(sId, sSubject)
                        ResetGrid()
                    End If
                Case "DELETE"
                    oCSubjectEmailManager.DeleteSubectEmailByID(e.CommandArgument)
                    ResetGrid()
            End Select
        End Sub

        Protected Sub btnAddSubjectEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSubjectEmail.Click
            Dim oCSubjectEmailManager As CSubjectEmailManager = New CSubjectEmailManager()
            If ValidSubjectEmail("", txtSubjectEmail.Text) Then
                oCSubjectEmailManager.InsertSubjectEmail(newGUID(), txtSubjectEmail.Text)
                BindEmailSubject()
                txtSubjectEmail.Text = ""
                ResetGrid()
            End If
        End Sub

        Public Function ValidSubjectEmail(ByVal psID As String, ByVal psEmail As String) As Boolean
            Dim oCSubjectEmailManager As CSubjectEmailManager = New CSubjectEmailManager()
            If String.IsNullOrEmpty(psEmail) Then
                ClientAlert("Email Subject Can't Be Empty")
                Return False
            End If
            If oCSubjectEmailManager.CheckSubjectEmailExist(psID, psEmail) Then
                ClientAlert("Email Subject Has Already Existed")
                Return False
            End If
            If SafeString(psEmail).Length > 300 Then
                ClientAlert("Email Subject Length Must Be Less Than 300 Characters ")
                Return False
            End If
            Return True
        End Function

        Private Sub ResetGrid()
            Edit = False
            dgSubjectEmail.EditItemIndex = -1
            ItemIndex = -1
            BindEmailSubject()
        End Sub

    End Class
End Namespace
