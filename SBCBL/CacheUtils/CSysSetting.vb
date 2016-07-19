Imports SBCBL.std

Namespace CacheUtils

    <Serializable()> Public Class CSysSetting

#Region "Fields"

        Private _SysSettingID As String
        Private _Category As String
        Private _SubCategory As String
        Private _Key As String
        Private _Value As String
        Private _Orther As String
        Private _ItemIndex As Integer
#End Region

#Region "Constructors"

        Public Sub New()
            Me._SysSettingID = newGUID()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            _SysSettingID = SafeString(podrData("SysSettingID"))
            _Category = SafeString(podrData("Category"))
            _SubCategory = SafeString(podrData("SubCategory"))
            _Key = SafeString(podrData("Key"))
            _Value = SafeString(podrData("Value"))
            _ItemIndex = SafeInteger(podrData("ItemIndex"))
            _Orther = SafeString(podrData("Orther"))
        End Sub

#End Region

#Region "Properties"

        Public Property SysSettingID() As String
            Get
                Return _SysSettingID
            End Get
            Set(ByVal value As String)
                _SysSettingID = value
            End Set
        End Property

        Public Property Category() As String
            Get
                Return _Category
            End Get
            Set(ByVal value As String)
                _Category = value
            End Set
        End Property

        Public Property Key() As String
            Get
                Return _Key
            End Get
            Set(ByVal value As String)
                _Key = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return _Value
            End Get
            Set(ByVal value As String)
                _Value = value
            End Set
        End Property

        Public Property SubCategory() As String
            Get
                Return _SubCategory
            End Get
            Set(ByVal value As String)
                _SubCategory = value
            End Set
        End Property

        Public Property Orther() As String
            Get
                Return _Orther
            End Get
            Set(ByVal value As String)
                _Orther = value
            End Set
        End Property

        Public Property ItemIndex() As Integer
            Get
                Return _ItemIndex
            End Get
            Set(ByVal value As Integer)
                _ItemIndex = value
            End Set
        End Property
#End Region

    End Class

    <Serializable()> Public Class CSysSettingList
        Inherits List(Of CSysSetting)

        ''' <summary>
        ''' Use only: after get the list of settings by Category or (Category and SubCategory)
        ''' </summary>
        Public Function GetSysSetting(ByVal psKey As String) As CSysSetting
            Return Me.Find(Function(oSetting) UCase(oSetting.Key) = UCase(psKey))
        End Function

        Public Function GetSysSetting(ByVal psKey As String, ByVal psOrther As String) As CSysSetting
            Return Me.Find(Function(oSetting) UCase(oSetting.Key) = UCase(psKey) AndAlso UCase(oSetting.Orther) = UCase(psOrther))
        End Function

        ''' <summary>
        ''' Use only: after get the list of settings by Category or (Category and SubCategory)
        ''' </summary>
        Public Function GetValue(ByVal psKey As String) As String
            Dim oSetting As CSysSetting = GetSysSetting(psKey)

            If oSetting IsNot Nothing Then
                Return oSetting.Value
            End If

            Return ""
        End Function

        ''' <summary>
        ''' Use only: after get the list of settings by Category or (Category and SubCategory)
        ''' </summary>

        Public Function GetValue(ByVal psKey As String, ByVal psOrther As String) As String
            Dim oSetting As CSysSetting = GetSysSetting(psKey, psOrther)
            If oSetting IsNot Nothing Then
                Return oSetting.Value
            End If

            Return ""
        End Function

        ''' <summary>
        ''' Use only: after get the list of settings by Category or (Category and SubCategory)
        ''' </summary>
        Public Function GetBooleanValue(ByVal psKey As String) As Boolean
            Return SafeBoolean(GetValue(psKey))
        End Function

        ''' <summary>
        ''' Use only: after get the list of settings by Category or (Category and SubCategory)
        ''' </summary>
        Public Function GetDoubleValue(ByVal psKey As String) As Double
            Return SafeDouble(GetValue(psKey))
        End Function

        ''' <summary>
        ''' Use only: after get the list of settings by Category or (Category and SubCategory)
        ''' </summary>
        Public Function GetIntegerValue(ByVal psKey As String) As Integer
            Return SafeInteger(GetValue(psKey))
        End Function

        Public Function GetIntegerValue(ByVal psKey As String, ByVal psOrther As String) As Integer
            Return SafeInteger(GetValue(psKey, psOrther))
        End Function

    End Class

End Namespace