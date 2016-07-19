Imports System.Data.OleDb
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.ComponentModel
Imports WebsiteLibrary.CSBCStd
Imports System.Collections.Specialized
Imports System.Data

Public Class CDataGridPageSort
    Inherits System.Web.UI.WebControls.DataGrid
    Implements IPostBackDataHandler, IPostBackEventHandler

    Public Event getDataView(ByRef oResult As DataView)
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Sort Direction/Key"
    'pulls the sortdirection and sortkey out of the hidden input tags
    Public Function LoadPostData(ByVal postDataKey As String, ByVal values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
        'SortKey and SortDir has change to ViewState so don't need any code here
        'do nothing and let sort direction be default
        Return False
    End Function

    'useless func required by postbackdatahandler interface. should never reach here
    Public Sub RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
    End Sub
#End Region

#Region "Simple properties"
    Private _enableViewStateForColumns As String()
    <TypeConverter(GetType(CStringListConverter)), Description("Comma delimited list of columns INDEXES that we want to have ViewState enabled for.")> Public Property enableViewStateForColumns() As String()
        Get
            Return _enableViewStateForColumns
        End Get
        Set(ByVal Value As String())
            _enableViewStateForColumns = Value
        End Set
    End Property

    <Category("Misc"), Description("Deprecated. Please don't use anymore")> Public Property SQL() As String
        Get
            If viewstate(Me.UniqueID & "_SQL") Is Nothing Then
                viewstate(Me.UniqueID & "_SQL") = ""
            End If
            Return SafeString(viewstate(Me.UniqueID & "_SQL"))
        End Get
        Set(ByVal Value As String)
            If Value = "" Then
                '                Throw New Exception("SQL cannot be set empty!")
            End If
            viewstate(Me.UniqueID & "_SQL") = Value
        End Set
    End Property

    Public Property ImageASC() As String
        Get
            If CStr(ViewState("ImageASC")) = "" AndAlso ConfigurationManager.AppSettings("IMAGE_DIR") <> "" Then
                ViewState("ImageASC") = ConfigurationManager.AppSettings("IMAGE_DIR") & "tri_asc.gif"
            End If
            Return CStr(viewstate("ImageASC"))
        End Get
        Set(ByVal Value As String)
            viewstate("ImageASC") = Value
        End Set
    End Property

    Public Property ImageDesc() As String
        Get
            If CStr(ViewState("ImageDESC")) = "" AndAlso ConfigurationManager.AppSettings("IMAGE_DIR") <> "" Then
                'first time around, appsettings will return empty string
                ViewState("ImageDESC") = ConfigurationManager.AppSettings("IMAGE_DIR") & "tri_desc.gif"
            End If
            Return CStr(viewstate("ImageDESC"))
        End Get
        Set(ByVal Value As String)
            viewstate("ImageDESC") = Value
        End Set
    End Property

    <DefaultValue(GetType(ListSortDirection), "Ascending")> Public Property SortDir() As System.ComponentModel.ListSortDirection
        Get
            If SafeString(ViewState("SortDir")) = "" Then
                ViewState("SortDir") = "Ascending"
            End If
            Return CType([Enum].Parse(GetType(ListSortDirection), SafeString(ViewState("SortDir"))), ListSortDirection)
        End Get
        Set(ByVal Value As System.ComponentModel.ListSortDirection)
            ViewState("SortDir") = Value.ToString
        End Set
    End Property

    Public Property SortKey() As String
        Get
            Return SafeString(ViewState("SortKey"))
        End Get
        Set(ByVal Value As String)
            ViewState("SortKey") = Value
        End Set
    End Property

    Public Property HeaderCssClass() As String
        Get
            Return SafeString(Me.ViewState("HEADER_CSSCLASS"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("HEADER_CSSCLASS") = value
        End Set
    End Property

    <System.ComponentModel.BrowsableAttribute(False), System.ComponentModel.DesignerSerializationVisibilityAttribute(0)> Public Property _Page() As CGlobalPage
        Get
            Return CType(MyBase.Page, CGlobalPage)
        End Get
        Set(ByVal Value As CGlobalPage)
            MyBase.Page = Value
        End Set
    End Property

    Public Property ShowFirstLast() As Boolean
        Get
            If ViewState("ShowFirstLast") Is Nothing Then
                ViewState("ShowFirstLast") = True
            End If

            Return CBool(ViewState("ShowFirstLast"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("ShowFirstLast") = Value
        End Set
    End Property

#End Region

    Private _binding As Boolean = False

    Private Sub bindGrid(ByVal dv As DataView)
        If _binding Then
            Exit Sub
        End If

        _binding = True

        If SortKey <> "" Then
            'pbTrace("VER", "Sorting DSL on :" & SortKey & " " & CStr(IIf(Me.SortDir = ListSortDirection.Descending, "DESC", "ASC")))            
            dv.Sort = Me.SortKey & " " & CStr(IIf(Me.SortDir = ListSortDirection.Descending, "DESC", "ASC"))
        End If
        '' Store Page Index
        Dim nPageIndex As Integer = CurrentPageIndex
        CurrentPageIndex = 0
        MyBase.DataSource = dv
        MyBase.DataBind()

        '' Set CurrentPageIndex in safe case
        If nPageIndex > 0 AndAlso nPageIndex < PageCount Then
            CurrentPageIndex = nPageIndex
            MyBase.DataBind()
        End If
        _binding = False
    End Sub

    Private Sub Page_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles MyBase.PageIndexChanged
        Dim dv As DataView = getTheDataView()
        If IsNothing(dv) Then
            pbTrace("ERR", "Data grid cannot be bound because sql property was not set or getDataView event did not return any dataview.  At least one or the other must be implemented!")
            Throw New ApplicationException("Data grid cannot be bound because sql property was not set or getDataView event did not return any dataview.  At least one or the other must be implemented!")
        End If

        CurrentPageIndex = e.NewPageIndex
        bindGrid(dv)
    End Sub

    'returns a dataview, either from event or SQL
    Private Function getTheDataView() As DataView
        Dim dv As DataView = Nothing
        RaiseEvent getDataView(dv)

        If IsNothing(dv) = False Then
            'traceA("using dataview from event handler")
            Return dv
        Else
            Return Nothing
        End If
    End Function

    Private Sub CDataGridPageSortEX_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.DataBinding
        If _binding = False Then
            Dim dv As DataView = getTheDataView()
            If IsNothing(dv) Then
                'under the IDE, we will prob have nothing
                Exit Sub
            End If

            bindGrid(dv)
        End If
    End Sub



    ''For Custom Pager with PrePageList|PrePage|Numbers|NextPage|NextPageList
    Protected Overrides Sub OnItemCommand(ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs)
        If e.CommandName = "__PreviousPager" OrElse e.CommandName = "__NextPager" _
                OrElse e.CommandName = "__FirstPager" OrElse e.CommandName = "__LastPager" Then

            Dim dv As DataView = getTheDataView()
            If IsNothing(dv) Then
                pbTrace("ERR", "Data grid cannot be bound because sql property was not set or getDataView event did not return any dataview.  At least one or the other must be implemented!")
                Throw New ApplicationException("Data grid cannot be bound because sql property was not set or getDataView event did not return any dataview.  At least one or the other must be implemented!")
            End If

            Select Case e.CommandName
                Case "__PreviousPager"
                    Me.CurrentPageIndex = Me.CurrentPageIndex - 1
                Case "__NextPager"
                    Me.CurrentPageIndex = Me.CurrentPageIndex + 1
                Case "__FirstPager"
                    Me.CurrentPageIndex = 0
                Case "__LastPager"
                    Me.CurrentPageIndex = Me.PageCount - 1
            End Select

            bindGrid(dv)

            Return
        End If

        MyBase.OnItemCommand(e)
    End Sub

    Protected Overrides Sub OnItemCreated(ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
        MyBase.OnItemCreated(e)

        'handle header creation
        Dim oItemType As ListItemType = e.Item.ItemType

        If oItemType <> ListItemType.Header Then
            If oItemType = ListItemType.Pager AndAlso Me.PagerStyle.Mode = PagerMode.NumericPages Then
                PagerItemCreated(e)
            End If

            handleRowViewstate(e)
            Return
        End If

        'replace all header cell contentscontents of all cells with our custom link
        Dim oCell As TableCell = Nothing

        'For Each cell In e.Item.Cells
        For nIndex As Integer = 0 To Me.Columns.Count - 1
            Dim col As DataGridColumn = Me.Columns(nIndex)

            If col.SortExpression <> "" Then
                Dim oHlk As New HyperLink()

                oHlk.Text = col.HeaderText
                oHlk.NavigateUrl = Page.ClientScript.GetPostBackClientHyperlink(Me, col.SortExpression)
                oHlk.CssClass = Me.HeaderCssClass
                oCell = e.Item.Cells(nIndex)
                oCell.Controls.Clear()
                oCell.Controls.Add(oHlk)

                oCell.Controls.Add(New LiteralControl("&nbsp;"))
            End If
        Next
    End Sub

    ''Create Custom Pager with PrePageList|PrePage|Numbers|NextPage|NextPageList
    Protected Sub PagerItemCreated(ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
        If Me.PagerStyle.PageButtonCount = 0 Then
            Me.PagerStyle.PageButtonCount = 10
        End If

        Dim nCurPageIndex As Int32 = Me.CurrentPageIndex

        Dim nPreIndexOf As Int32 = -1, nNextIndexOf As Int32 = -1

        ''Pager: Cells(0).Controls (Lable, LiteralControl, LinkButton)
        For Each oCtrl As Control In e.Item.Cells(0).Controls
            If TypeOf oCtrl Is Label Then
                Dim oLable As Label = CType(oCtrl, Label)
                oLable.Text = "[" & SafeString(IIf(SafeInteger(oLable.Text) < 10, "0", "")) & oLable.Text & "]"
            ElseIf TypeOf oCtrl Is LinkButton Then
                Dim oLnk As LinkButton = CType(oCtrl, LinkButton)

                If oLnk.Text = "..." Then
                    If SafeInteger(oLnk.CommandArgument) < (nCurPageIndex + 1) Then
                        oLnk.ToolTip = "Previous Page List"
                        nPreIndexOf = e.Item.Cells(0).Controls.IndexOf(oCtrl)
                    Else
                        oLnk.ToolTip = "Next Page List"
                        nNextIndexOf = e.Item.Cells(0).Controls.IndexOf(oCtrl)
                    End If
                Else
                    If SafeInteger(oLnk.CommandArgument) < 10 Then
                        oLnk.Text = "0" & oLnk.CommandArgument
                    End If
                    oLnk.ToolTip = "Page " & SafeString(oLnk.CommandArgument)
                End If
            End If
        Next

        If nPreIndexOf >= 0 Then
            nPreIndexOf += 1
            e.Item.Cells(0).Controls.AddAt(nPreIndexOf, getLinkButtonControl(" Previous ", "Page " & SafeString(nCurPageIndex), "__PreviousPager"))

            If Me.ShowFirstLast Then
                e.Item.Cells(0).Controls.AddAt(0, getLinkButtonControl("First ", "First Page 1", "__FirstPager"))
            End If

        ElseIf nCurPageIndex > 0 Then
            e.Item.Cells(0).Controls.AddAt(0, getLinkButtonControl("Previous ", "Page " & SafeString(nCurPageIndex), "__PreviousPager"))
        End If

        If nNextIndexOf >= 0 Then
            If nPreIndexOf >= 0 Then
                nNextIndexOf += 1
                If Me.ShowFirstLast Then nNextIndexOf += 1
            End If
            e.Item.Cells(0).Controls.AddAt(nNextIndexOf, getLinkButtonControl(" Next ", "Page " & SafeString(nCurPageIndex + 2), "__NextPager"))

            If Me.ShowFirstLast Then
                e.Item.Cells(0).Controls.Add(getLinkButtonControl(" Last", "Last Page " & SafeString(Me.PageCount), "__LastPager"))
            End If

        ElseIf nCurPageIndex < Me.PageCount - 1 Then
            e.Item.Cells(0).Controls.Add(getLinkButtonControl(" Next", "Page " & SafeString(nCurPageIndex + 2), "__NextPager"))
        End If

        nPreIndexOf = -1 : nNextIndexOf = -1
    End Sub

    Private Function getLinkButtonControl(ByVal psText As String, ByVal psTooltip As String, ByVal psCmdName As String) As LinkButton
        Dim oLnk As New LinkButton

        oLnk.CausesValidation = False
        oLnk.CommandName = psCmdName
        oLnk.Text = psText
        oLnk.ToolTip = psTooltip

        Return oLnk
    End Function

    Private Sub handleRowViewstate(ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
        'only do all the viewstate disabling/enabling fun if they specified enable only columns
        'also remember to ignore special rows with different column count (ie footer)

        If Not IsNothing(_enableViewStateForColumns) AndAlso _enableViewStateForColumns.Length > 0 AndAlso e.Item.Cells.Count = Me.Columns.Count Then
            Dim colindex As Integer
            'disable viewstate for everyone
            For colindex = 0 To e.Item.Cells.Count - 1
                e.Item.Cells(colindex).EnableViewState = False
            Next

            'enable it only for specified rows
            If Not IsNothing(_enableViewStateForColumns) Then
                For colindex = 0 To _enableViewStateForColumns.Length - 1

                    Dim ind As Integer = SafeInteger(_enableViewStateForColumns(colindex)) Mod Me.Columns.Count
                    'allow wraparound so that -1 refers to last column, -2 refers to 2nd to last col, etc
                    If ind < 0 Then
                        ind = Me.Columns.Count + ind
                    End If

                    e.Item.Cells(ind).EnableViewState = True
                    log.Debug("Enabled viewstate for column " & Me.Columns(ind).HeaderText)
                Next
            End If
        End If
    End Sub

    Private Sub invertSort(ByVal sortExpression As String)
        '    traceA(SortKey & ":" & sortExpression & " " & SortDir)
        If SortKey = "" Then
            'first coming here
            SortDir = ListSortDirection.Ascending
        ElseIf SortKey <> sortExpression Then
            'we switched from a prev clicked column to a new column
            SortDir = ListSortDirection.Ascending
        ElseIf SortKey = sortExpression Then
            'invert sortdirection             
            If SortDir = ListSortDirection.Ascending Then
                SortDir = ListSortDirection.Descending
            ElseIf SortDir = ListSortDirection.Descending Then
                SortDir = ListSortDirection.Ascending
            Else
                'default
                SortDir = ListSortDirection.Ascending
            End If
        End If

        'save attributes for next round trip        
        SortKey = sortExpression

        'traceA("Sorting :" & sortExpression & CStr(viewstate("sortDir")))
    End Sub

    'IMPORTANT - RegisterRequiresPostBack MUST be called in order for the postback methods above to be called!!
    'otherwise the postbackdata methods will never be called since a datagrid doesn't have data to post back to itself
    Private Sub CDataGridPageSort_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
        Page.RegisterRequiresPostBack(Me)
    End Sub

    'This event is raised when 
    Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        Dim newsortkey As String = eventArgument
        If SortKey <> "" Then
            invertSort(newsortkey)
        End If

        'set the arrow pix on the sorting        traceA("SDLFKJSDFLKSDJFKLS " & Me.)
        Dim col As DataGridColumn
        For Each col In Me.Columns
            ' clear icons (ascending or descending) after header text
            If col.HeaderText.IndexOf("&nbsp;") > 0 Then
                col.HeaderText = col.HeaderText.Substring(0, col.HeaderText.IndexOf("&nbsp;"))
            End If

            If col.SortExpression = newsortkey Then
                Dim img As New HyperLink()
                With img
                    .ID = "direction_arrow_image"
                    If SortDir = ListSortDirection.Ascending Then
                        .ImageUrl = ImageASC
                        .ToolTip = "Click to sort column descending by " & col.HeaderText
                    Else
                        .ImageUrl = ImageDesc
                        .ToolTip = "Click to sort column ascending by " & col.HeaderText
                    End If

                    .BorderStyle = WebControls.BorderStyle.None

                End With
                img.NavigateUrl = Page.ClientScript.GetPostBackClientHyperlink(Me, newsortkey)

                Dim strWriter As New System.IO.StringWriter()
                Dim htmlWriter As New HtmlTextWriter(strWriter)
                img.RenderControl(htmlWriter)

                'super cheesy ghetto stylez !!!!
                'TODO: THE HREF underline extends over the &nbsp;... how to get rid of it?

                ' Add icon (ascending or descending) after header text
                col.HeaderText = col.HeaderText & "&nbsp;" & strWriter.ToString

            End If
        Next
        SortKey = newsortkey

        bindGrid(getTheDataView())
    End Sub

    Public Overrides Sub RenderControl(ByVal writer As System.Web.UI.HtmlTextWriter)
        If Me.Items.Count = 0 Then
            Me.Visible = False
        Else
            MyBase.Render(writer)
        End If
    End Sub

    Private Sub CDataGridPageSort_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender


        If Me.EnableViewState = False Then
            pbTrace("ERR", "Please do not disable entire viewstate for datagrid!  Doing so side effects and prevent sorting from working.  If you need to, disable the viewstates for individual columns!")
        End If
    End Sub

End Class

Public Class CStringListConverter
    Inherits ExpandableObjectConverter

    Public Overloads Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object) As Object
        Return Split(SafeString(value), ",")
    End Function

    Public Overloads Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object

        Dim arr As String() = CType(value, String())
        Return Join(arr, ",")
    End Function

    Public Overloads Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean

        If (sourceType.Name = "String") Then
            Return True
        End If
        Return MyBase.CanConvertFrom(context, sourceType)
    End Function

End Class
