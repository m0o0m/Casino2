Namespace UI
    Public Class CSBCUserControl
        Inherits System.Web.UI.UserControl

        Public Shadows Property Page() As CSBCPage
            Get
                Return CType(MyBase.Page, CSBCPage)
            End Get
            Set(ByVal value As CSBCPage)
                MyBase.Page = value
            End Set
        End Property

        Public ReadOnly Property UserSession() As CSBCSession
            Get
                Return Page.UserSession
            End Get
        End Property
    End Class

End Namespace