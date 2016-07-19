Imports Microsoft.VisualBasic

Namespace SBCWebsite

    Partial Class Inc_DateTime
        Inherits System.Web.UI.UserControl

        Public Property ShowCalendar() As Boolean
            Get
                Return ibtnCal.Visible
            End Get
            Set(ByVal value As Boolean)
                ibtnCal.Visible = value
                If value Then
                    ce.PopupButtonID = "ibtnCal"
                End If
            End Set
        End Property

        Public Property ShowDate() As Boolean
            Get
                Return txtDate.Visible
            End Get
            Set(ByVal value As Boolean)
                txtDate.Visible = value
                ibtnCal.Visible = txtDate.Visible

                If value Then
                    ce.PopupButtonID = "txtDate"
                End If

            End Set
        End Property

        Public Property ShowTime() As Boolean
            Get
                Return Hour.Visible
            End Get
            Set(ByVal value As Boolean)
                Hour.Visible = value
                Minute.Visible = Hour.Visible
                AMPM.Visible = Hour.Visible
                lblColon.Visible = Hour.Visible
            End Set
        End Property

        Public Property HourDispose() As String
            Get
                Return Hour.SelectedValue
            End Get
            Set(ByVal value As String)
                Hour.SelectedValue = value
            End Set
        End Property

        Public Property MinuteDispose() As String
            Get
                Return Minute.SelectedValue
            End Get
            Set(ByVal value As String)
                Minute.SelectedValue = value
            End Set
        End Property

        Public Property AMPMDispose() As String
            Get
                Return AMPM.SelectedValue
            End Get
            Set(ByVal value As String)
                AMPM.SelectedValue = value
            End Set
        End Property

        Public ReadOnly Property Style() As CssStyleCollection
            Get
                Return txtDate.Style
            End Get
        End Property

        Public Property Value() As Date
            Get
                If (IsDate(txtDate.Text)) Then
                    'check out of range datetime
                    If CDate(txtDate.Text).Year < 1753 Then
                        Return Date.MinValue
                    End If
                    'safe date
                    Return CDate(CDate(txtDate.Text).ToShortDateString() & " " & Hour.SelectedValue & ":" & Minute.SelectedValue & AMPM.SelectedValue)
                Else
                    Return Date.MinValue
                End If
            End Get
            Set(ByVal value As Date)
                If value > Date.MinValue Then
                    txtDate.Text = value.ToShortDateString()
                    Dim nMinute As Integer = value.Minute
                    Dim nHour As Integer = value.Hour

                    Minute.SelectedValue = IIf(nMinute < 10, "0" + nMinute.ToString(), nMinute.ToString()).ToString()
                    If (nHour < 12) Then
                        Hour.SelectedValue = IIf(nHour < 10, "0" + nHour.ToString(), nHour.ToString()).ToString()
                        AMPM.SelectedValue = "AM"
                    ElseIf (nHour = 12) Then
                        Hour.SelectedValue = "00"
                        AMPM.SelectedValue = "AM"
                    Else
                        nHour = nHour - 12
                        Hour.SelectedValue = IIf(nHour < 10, "0" + nHour.ToString(), nHour.ToString()).ToString()
                        AMPM.SelectedValue = "PM"
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property TextDateClientID() As String
            Get
                Return txtDate.ClientID
            End Get
        End Property

        'clear date
        Public Sub ClearDate()
            txtDate.Text = ""
            Hour.SelectedIndex = 0
            Minute.SelectedIndex = 0
            AMPM.SelectedIndex = 0
        End Sub
    End Class

End Namespace