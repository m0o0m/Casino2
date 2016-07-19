Imports System.Web
Imports WebsiteLibrary
Imports WebsiteLibrary.CSBCStd
Imports System.Collections

<Serializable()> _
Public MustInherit Class CShoppingCart

    Public Items As New CShoppingCartItemsList()

    Public MustOverride Sub updateItemQuantity(ByVal itemCode As String, ByVal quantity As Integer)

    Public Function findItem(ByVal itemID As String) As CShoppingCartItem
        Dim item As CShoppingCartItem
        For Each item In Items
            If item.itemID = itemID Then
                Return item
            End If
        Next
        Return Nothing
    End Function

End Class

<Serializable()> _
Public Class CShoppingCartItemsList
    Inherits ArrayList

    Public Sub addShoppingCartItem(ByVal item As CShoppingCartItem)
        MyBase.add(item)
    End Sub

    Default Public Shadows Property Item(ByVal index As Integer) As CShoppingCartItem
        Get
            Return CType(MyBase.Item(index), CShoppingCartItem)
        End Get
        Set(ByVal Value As CShoppingCartItem)
            MyBase.Item(index) = Value
        End Set
    End Property
End Class

<Serializable()> _
Public Class CShoppingCartItem
    Private _itemID As String
    Public Property itemID() As String
        Get
            Return _itemID
        End Get
        Set(ByVal Value As String)
            _itemID = Value
        End Set
    End Property

    Public itemCode As String
    Public itemName As String
    Public itemDescription As String
    Public price As Double
    Public quantity As Integer
    Public thumbURL As String
    Public imageURL As String
End Class
