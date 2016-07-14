Namespace CCWebsite
    Partial Class PasswordEditor
        Inherits UserControl
        'Inherits SBCBL.UI.CDSLUserControl

        Public WriteOnly Property HorizontalAlign() As Boolean
            Set(ByVal value As Boolean)
                If value Then
                    Me.spanConfirmPassword.Style.Remove("top")
                    Me.spanConfirmPassword.Style.Remove("left")
                    Me.spanConfirmPassword.Style.Add("position", "relative")
                    Me.spanConfirmPassword.Style.Add("top", "-40px")
                    spanConfirmPassword.Style.Add("left", "230px")
                End If
            End Set
        End Property

        Public WriteOnly Property Left() As Integer
            Set(ByVal value As Integer)
                divPasswordEditor.Style.Add("left", value & "px")
                divPasswordEditor.Style.Add("position", "relative")
            End Set
        End Property

        Public WriteOnly Property TextVisible() As Boolean
            Set(ByVal value As Boolean)
                ltrPasswordCaption.Visible = value
                ltrConfirmPasswordCaption.Visible = value
            End Set
        End Property

        Public WriteOnly Property MeterLeft() As Integer
            Set(ByVal value As Integer)
                div1.Style.Add("left", value & "px")
                div1.Style.Add("position", "relative")
            End Set
        End Property

        Public WriteOnly Property ConfirmPasswordLeft() As Integer
            Set(ByVal value As Integer)
                spanConfirmPassword.Style.Add("left", value & "px")
                spanConfirmPassword.Style.Add("position", "relative")
            End Set
        End Property

        Public WriteOnly Property SetCheckCapsLockClientFunction() As String
            Set(ByVal value As String)
                txtPassword.Attributes.Add("onkeypress", value)
                txtPasswordConfirm.Attributes.Add("onkeypress", value)
            End Set
        End Property

        Public WriteOnly Property TabIndex() As Short
            Set(ByVal value As Short)
                Me.txtPassword.TabIndex = value
                Me.txtPasswordConfirm.TabIndex = CShort(value + 1)
            End Set
        End Property

        Public Property Password() As String
            Get
                Return txtPassword.Text
            End Get
            Set(ByVal value As String)
                txtPassword.Attributes("value") = value
                Me.txtPasswordConfirm.Attributes("value") = value
                txtPassword.Text = value
                Me.txtPasswordConfirm.Text = value
            End Set
        End Property

        Public Property useWaterMark() As Boolean
            Get
                Return txtwePassword.Enabled
            End Get
            Set(ByVal value As Boolean)
                txtwePassword.Enabled = value
                lblAlert.Visible = txtwePassword.Enabled = False
            End Set
        End Property

        Public WriteOnly Property ValidationGroup() As String
            Set(ByVal value As String)
                custvPassword.ValidationGroup = value
                custvPasswordPattern.ValidationGroup = value
                cvPassword.ValidationGroup = value
                custvComparePassword.ValidationGroup = value
            End Set
        End Property


        Public Property PasswordCaption() As String
            Get
                Return ltrPasswordCaption.Text
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    ltrPasswordCaption.Text = value & " " & ltrPasswordCaption.Text
                    'ltrConfirmPasswordCaption.Text = "Confirm " & value & " Password"
                    custvPassword.ErrorMessage = value & " " & custvPassword.ErrorMessage
                    custvPasswordPattern.ErrorMessage = value & " " & custvPasswordPattern.ErrorMessage
                    'rfvConfirmPassword.ErrorMessage = value & " " & rfvConfirmPassword.ErrorMessage
                    cvPassword.ErrorMessage = value & " " & cvPassword.ErrorMessage
                    custvComparePassword.ErrorMessage = value & " " & custvComparePassword.ErrorMessage
                End If

            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return rfvPassword.Enabled
            End Get
            Set(ByVal value As Boolean)
                rfvPassword.Enabled = value
            End Set
        End Property

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            If Not Required Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), Guid.NewGuid().ToString(), String.Format("ValidatorEnable({0},{1});", rfvPassword.ClientID, "false"), True)
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            txtPassword.Attributes("OnKeyup") = "javascript:CheckPsdStrength(this.value, '" & div1.ClientID & "', '" & div2.ClientID & "', '" & Me.txtPassword.ClientID & "')"
            ' If txtPassword.Text <> "" Then
            '   Page.ClientScript.RegisterStartupScript(Page.GetType, "InitPasswordMether", "<script language='javascript' type='text/javascript'>CheckPsdStrength('" & txtPassword.Text & "', '" & div1.ClientID & "', '" & div2.ClientID & "', '" & Me.txtPassword.ClientID & "');</script>")
            ' End If
        End Sub

    End Class

End Namespace
