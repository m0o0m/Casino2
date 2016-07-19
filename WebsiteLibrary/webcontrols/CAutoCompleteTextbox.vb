Imports WebsiteLibrary.CSBCStd
Imports Microsoft.VisualBasic
Imports System.Web.UI.WebControls
Imports System.Web
Imports System.Web.UI
Imports WebsiteLibrary.DBUtils
Imports System.Collections.Specialized
Imports System.Collections
Imports System.Data

Public Class CAutoCompleteTextbox
    Inherits TextBox

    Public Shared SBC_CONNECTION_STRING As String = System.Web.Configuration.WebConfigurationManager.ConnectionStrings("DSL_CONNECTIONSTRING").ConnectionString

    Public Property HasRegisterScript() As Boolean
        Get
            If IsNothing(ViewState("RegisterScript")) Then
                Return False
            End If
            Return CBool(ViewState("RegisterScript"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("RegisterScript") = Value
        End Set
    End Property

    Public Property ScriptPath() As String
        Get
            If IsNothing(ViewState("ScriptPath")) Then
                Return "/CC/Inc/scripts/"
            End If
            Return CStr(ViewState("ScriptPath"))
        End Get
        Set(ByVal Value As String)
            ViewState("ScriptPath") = Value
        End Set
    End Property

    Public Overrides Property ID() As String
        Get
            If IsNothing(ViewState("CAutoCompleteTextboxID")) Then
                Return ""
            End If
            Return CStr(ViewState("CAutoCompleteTextboxID"))
        End Get
        Set(ByVal Value As String)
            ViewState("CAutoCompleteTextboxID") = Value
        End Set
    End Property

    Public Property Category() As String
        Get
            Return SafeString(ViewState("CATEGORY" & ID))
        End Get
        Set(ByVal value As String)
            ViewState("CATEGORY" & ID) = value
        End Set
    End Property

    Public Property SubCategory() As String
        Get
            Return SafeString(ViewState("SubCategory" & ID))
        End Get
        Set(ByVal value As String)
            ViewState("SubCategory" & ID) = value
        End Set
    End Property

    Public Property OrganizationID() As String
        Get
            Return SafeString(ViewState("OrganizationID" & ID))
        End Get
        Set(ByVal value As String)
            ViewState("OrganizationID" & ID) = value
        End Set
    End Property

    Private ReadOnly Property _List() As ArrayList
        Get
            If ViewState("LIST" & ID) Is Nothing Then
                ViewState("LIST" & ID) = New ArrayList()
            End If

            Return CType(ViewState("LIST" & ID), ArrayList)
        End Get
    End Property

    Public Property AllowDuplicates() As Boolean
        Get
            Return CBool(ViewState("LIST" & ID))
        End Get
        Set(ByVal value As Boolean)
            ViewState("LIST" & ID) = value
        End Set
    End Property

    Public Property MaxRows() As String
        Get
            If SafeInteger(ViewState("MaxRows" & ID)) = 0 Then
                Return "4"
            End If
            Return SafeString(SafeInteger(ViewState("MaxRows" & ID)))
        End Get
        Set(ByVal value As String)
            ViewState("MaxRows" & ID) = value
        End Set
    End Property

    Public Property LimitToList() As Boolean
        Get
            If ViewState("LimitToList" & ID) Is Nothing Then
                Return False
            End If

            Return CBool(ViewState("LimitToList" & ID))
        End Get
        Set(ByVal value As Boolean)
            ViewState("LimitToList" & ID) = value
        End Set
    End Property

    Public Sub SetOptionList(ByVal oList As ArrayList)
        'put the single quotes around the strings for javascript generation later
        _List.Clear()
        For Each sValue As String In oList
            AddOption(sValue)
        Next
    End Sub

    Public Sub AddOption(ByVal psOption As String)
        If Not _List.Contains(SQLString(psOption)) Then
            ' dont use sqlstring(), it made JS error
            _List.Add("""" + psOption.Replace("""", "'") + """")
        End If
    End Sub

    Private Sub GetCategoryItems()
        Dim sKey As String = UCase("CATEGORYITEMS_" & Category & "_" & SubCategory & "_" & OrganizationID)

        If HttpContext.Current.Cache(sKey) Is Nothing Then
            Dim oWhere As New WebsiteLibrary.DBUtils.CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("(OrganizationID=" & SQLString(OrganizationID) & " OR OrganizationID='SYSTEM')")
            oWhere.AppendANDCondition("Category=" & SQLString(Category))
            oWhere.AppendOptionalANDCondition("SubCategory", SQLString(SubCategory), "=")

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim oDT As DataTable = oDB.getDataTable("SELECT Item, Value FROM CategoryItems " & oWhere.SQL & " ORDER BY OrganizationID desc,ItemIndex, Item, Value")
            oDB.closeConnection()

            Dim oList As New OrderedDictionary()
            For Each oDR As DataRow In oDT.Rows
                Dim hasDuplicate As Boolean = False
                For Each oItem As DictionaryEntry In oList
                    If SafeString(oItem.Key) = SafeString(oDR("Item")) Then
                        oItem.Value = SafeString(oDR("Value"))
                        hasDuplicate = True
                        Exit For
                    End If
                Next
                If Not hasDuplicate Then
                    If SafeString(oDR("Value")) <> "" Then
                        oList.Add(SafeString(oDR("Item")), SafeString(oDR("Value")))
                    Else
                        oList.Add(SafeString(oDR("Item")), SafeString(oDR("Item")))
                    End If
                End If
            Next

            HttpContext.Current.Cache.Add(sKey, oList, Nothing, Date.Now.AddMinutes(60), Nothing, Caching.CacheItemPriority.Default, Nothing)
        End If

        Dim oOrderedDictionary As OrderedDictionary = CType(HttpContext.Current.Cache(sKey), OrderedDictionary)

        For Each oCategoryItem As DictionaryEntry In oOrderedDictionary
            AddOption(oCategoryItem.Key.ToString())
        Next

    End Sub

    Private Sub CAutoCompleteTextbox_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Me.CssClass = "textInput"

        If Category <> "" Then
            GetCategoryItems()
        End If

        If Not IsNothing(_List) Then
            If Not HasRegisterScript Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "INCLUDE_AUTO_COMPLETE", "<script src=""" & ScriptPath & "ac.js""></script>")
            End If
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "INIT_AUTO_COMPLETE" & ID, "<script>customarray" & ID & "=new Array(" & Strings.Join(_List.ToArray(), ",") & ");</script>")

            Me.Attributes("onfocus") = "actb(this,event,customarray" & ID & "," & MaxRows & "," & SafeString(IIf(LimitToList = True, "true", "false")) & ");"
        End If
    End Sub

    Public Sub RegisterScriptData()
        Page.ClientScript.RegisterStartupScript(Me.GetType, "INIT_AUTO_COMPLETE" & ID, "customarray" & ID & "=new Array(" & Strings.Join(_List.ToArray(), ",") & ");", True)
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "INIT_AUTO_COMPLETE" & ID, "customarray" & ID & "=new Array(" & Strings.Join(_List.ToArray(), ",") & ");", True)
    End Sub
End Class
