Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.Web.UI.WebControls
Imports System.Web.UI.Design.WebControls
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Globalization
Imports System.Reflection
Imports System.Collections

<ValidationProperty("SelectedItem"), Designer(GetType(CListDesigner)), DataBindingHandler(GetType(CListDataBindingHandler)), DefaultEvent("SelectedIndexChanged"), DefaultProperty("DataSource"), ParseChildren(True, "Items")> _
Public Class CListBox
    Inherits WebControl
    Implements IPostBackDataHandler

    ' Events
    <Category("Action"), Description("CListBox_OnSelectedIndexChanged")> _
    Public Event SelectedIndexChanged As EventHandler

    ' Fields
    Private _cachedSelectedIndex As Integer
    Private _cachedSelectedValue As String
    Private _dataSource As Object
    Private _items As CListItemCollection
    Private Shared ReadOnly _EventSelectedIndexChanged As Object
    Private Shared ReadOnly _SPLIT_CHARS As Char()
    Private attrState As StateBag
    Private attrColl As System.Web.UI.AttributeCollection

    ' Methods
    Shared Sub New()
        CListBox._EventSelectedIndexChanged = New Object
        Dim chArray1 As Char() = New Char() {","c}
        CListBox._SPLIT_CHARS = chArray1
    End Sub 'New

    Public Sub New()
        MyBase.New(HtmlTextWriterTag.Select)
        Me._cachedSelectedIndex = -1
        If Me.EnableHScroll Then
            If Me.Width.IsEmpty Then Me.Width = Unit.Pixel(100)
            If Me.Height.IsEmpty Then Me.Height = Unit.Pixel(100)
        End If
    End Sub 'New

    Protected Overrides Sub AddAttributesToRender(ByVal writer As HtmlTextWriter)
        If (Not Me.Page Is Nothing) Then
            Me.Page.VerifyRenderingInServerForm(Me)
        End If
        writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
        If Me.ListType = ListType.ListBox Then
            If Me.EnableHScroll Then
                writer.AddAttribute(HtmlTextWriterAttribute.Size, "4")
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Size, Me.Rows.ToString(NumberFormatInfo.InvariantInfo))
            End If
            If (Me.SelectionMode = ListSelectionMode.Multiple) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple")
            End If
        End If
        If (Me.AutoPostBack AndAlso (Not Me.Page Is Nothing)) Then
            Dim text1 As String = Me.Page.ClientScript.GetPostBackEventReference(Me, "")

            Dim _hasAttributes As Boolean
            'Because the following method call is private:
            'If MyBase.HasAttributes Then
            'I used this workaround:
            Dim methodInfo As methodInfo = MyBase.GetType.GetMethod("get_HasAttributes", BindingFlags.Instance Or BindingFlags.NonPublic)
            If Not (methodInfo Is Nothing) Then
                _hasAttributes = CType(methodInfo.Invoke(Me, Nothing), Boolean)
            End If

            If _hasAttributes Then
                'If MyBase.HasAttributes Then
                Dim text2 As String = MyBase.Attributes("onchange")
                If (Not text2 Is Nothing) Then
                    text1 = (text2 & text1)
                    MyBase.Attributes.Remove("onchange")
                End If
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, text1)
            writer.AddAttribute("language", "javascript")
        End If
        MyBase.AddAttributesToRender(writer)
    End Sub 'AddAttributesToRender

    Public Overridable Sub ClearSelection()
        For num1 As Integer = 0 To Me.Items.Count - 1
            Me.Items(num1).Selected = False
        Next num1
    End Sub 'ClearSelection

    Protected Overrides Sub LoadViewState(ByVal savedState As Object)
        If (Not savedState Is Nothing) Then
            Dim triplet1 As Triplet = CType(savedState, Triplet)
            MyBase.LoadViewState(triplet1.First)
            Me.Items.LoadViewState(triplet1.Second)
            Dim obj1 As Object = triplet1.Third
            If (Not obj1 Is Nothing) Then
                Me.SelectInternal(CType(obj1, ArrayList))
            End If
        End If
    End Sub 'LoadViewState

    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        Dim enumerable1 As IEnumerable = DataSourceHelper.GetResolvedDataSource(Me.DataSource, Me.DataMember)
        If (Not enumerable1 Is Nothing) Then
            Dim flag1 As Boolean = False
            Dim flag2 As Boolean = False
            Dim flag3 As Boolean = False
            Dim text1 As String = Me.DataTextField
            Dim text2 As String = Me.DataValueField
            Dim text3 As String = Me.DataTextFormatString
            Dim text4 As String = Me.DataOptGroupField
            Dim text5 As String = Me.DataOptGroupFormatString
            Me.Items.Clear()
            Dim collection1 As ICollection
            If TypeOf enumerable1 Is ICollection Then
                collection1 = CType(enumerable1, ICollection)
            Else
                collection1 = Nothing
            End If
            If (Not collection1 Is Nothing) Then
                Me.Items.Capacity = collection1.Count
            End If
            If (((text1.Length <> 0) OrElse (text2.Length <> 0)) OrElse (text4.Length <> 0)) Then
                flag1 = True
            End If
            If (text3.Length <> 0) Then
                flag2 = True
            End If
            If (text5.Length <> 0) Then
                flag3 = True
            End If
            For Each obj1 As Object In enumerable1
                Dim item1 As New CListItem
                If flag1 Then
                    If (text1.Length > 0) Then
                        item1.Text = DataBinder.GetPropertyValue(obj1, text1, text3)
                    End If
                    If (text2.Length > 0) Then
                        item1.Value = DataBinder.GetPropertyValue(obj1, text2, Nothing)
                    End If
                    If (text4.Length > 0) Then
                        item1.OptGroup = DataBinder.GetPropertyValue(obj1, text4, text5)
                    End If
                Else
                    If flag2 Then
                        item1.Text = String.Format(text3, obj1)
                    Else
                        item1.Text = obj1.ToString
                    End If
                    item1.Value = obj1.ToString
                    If flag3 Then
                        item1.OptGroup = String.Format(text5, obj1)
                    Else
                        item1.OptGroup = obj1.ToString
                    End If
                End If
                Me.Items.Add(item1)
            Next obj1
        End If
        If (Not Me._cachedSelectedValue Is Nothing) Then
            Dim num1 As Integer = -1
            num1 = Me.Items.FindByValueInternal(Me._cachedSelectedValue)
            If (-1 = num1) Then
                Throw New ArgumentOutOfRangeException("value")
            End If
            If ((Me._cachedSelectedIndex <> -1) AndAlso (Me._cachedSelectedIndex <> num1)) Then
                Throw New ArgumentException("The SelectedIndex and SelectedValue attributes are mutually exclusive")
            End If
            Me.SelectedIndex = num1
            Me._cachedSelectedValue = Nothing
            Me._cachedSelectedIndex = -1
        Else
            If (Me._cachedSelectedIndex <> -1) Then
                Me.SelectedIndex = Me._cachedSelectedIndex
                Me._cachedSelectedIndex = -1
            End If
        End If
    End Sub 'OnDataBinding

    Protected Overridable Sub RegisterScript()
        If Me.EnableHScroll Then

            'this script is client script and should appear only once
            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("CList_js") Then
                Dim reader As New System.IO.StreamReader(Me.GetType().Assembly.GetManifestResourceStream(Me.GetType(), "CListBox.js"))
                Dim script As String = "<script language='javascript' type='text/javascript' >" _
                                    + ControlChars.CrLf _
                                    + "<!--" _
                                    + ControlChars.CrLf _
                                    + reader.ReadToEnd() _
                                    + ControlChars.CrLf _
                                    + "//-->" _
                                    + ControlChars.CrLf _
                                    + "</script>"
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "CList_js", script)

                reader = Nothing
                script = Nothing
            End If

            'this script is startup script and should appear for each instance of the control
            If Not Me.Page.ClientScript.IsStartupScriptRegistered(Me.ClientID) Then
                Dim script As String = "<script language='javascript' type='text/javascript' >" _
                                    + ControlChars.CrLf _
                                    + "<!--" _
                                    + ControlChars.CrLf _
                                    + "objSelect = document.getElementById('" + Me.ClientID + "');" _
                                    + ControlChars.CrLf _
                                    + "objSelect.fireEvent(""onresize"");" _
                                    + ControlChars.CrLf _
                                    + "//-->" _
                                    + ControlChars.CrLf _
                                    + "</script>"
                Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), Me.ClientID, script)
            End If
        End If

    End Sub 'RegisterScript

    Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
        MyBase.OnPreRender(e)
        If (((Not Me.Page Is Nothing) AndAlso Me.Enabled) AndAlso Me.AutoPostBack) Then
            'Because the following method call is private:
            'Me.Page.RegisterPostBackScript()
            'I used this workaround:
            Dim methodInfo As methodInfo = Me.Page.GetType.GetMethod("RegisterPostBackScript", BindingFlags.Instance Or BindingFlags.NonPublic)
            If Not (methodInfo Is Nothing) Then
                methodInfo.Invoke(Me.Page, New Object() {})
            End If
        End If
        If Me.ListType = ListType.ListBox Then
            If (((Not Me.Page Is Nothing) AndAlso (Me.SelectionMode = ListSelectionMode.Multiple)) AndAlso Me.Enabled) Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End If

        If Me.EnableHScroll Then
            RegisterScript()
        End If
    End Sub 'OnPreRender

    Protected Overrides Function CreateControlCollection() As ControlCollection
        Select Case Me.ListType
            Case ListType.ListBox
                Return MyBase.CreateControlCollection()
            Case ListType.DropDownList
                Return New EmptyControlCollection(Me)
        End Select

        Return Nothing
    End Function 'CreateControlCollection

    Protected Overridable Sub OnSelectedIndexChanged(ByVal e As EventArgs)
        Dim handler1 As EventHandler = CType(MyBase.Events(CListBox._EventSelectedIndexChanged), EventHandler)
        If (Not handler1 Is Nothing) Then
            handler1.Invoke(Me, e)
        Else
            RaiseEvent SelectedIndexChanged(Me, e)
        End If

    End Sub 'OnSelectedIndexChanged

    Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        If Me.EnableHScroll Then

            Dim tag1 As HtmlTextWriterTag = Me.TagKey
            writer.AddAttribute("ID", Me.ClientID & "_div")
            writer.AddAttribute("name", Me.ClientID & "_div")
            writer.AddStyleAttribute("display", "inline")
            writer.AddStyleAttribute("overflow", "scroll")
            writer.AddStyleAttribute("border-width", "thin")
            writer.AddStyleAttribute("border-style", "inset")
            writer.AddStyleAttribute("width", Me.Width.ToString)
            writer.AddStyleAttribute("height", Me.Height.ToString)
            If Not MyBase.Style("TOP") Is Nothing Then
                writer.AddStyleAttribute("TOP", MyBase.Style("TOP"))
            End If
            If Not MyBase.Style("LEFT") Is Nothing Then
                writer.AddStyleAttribute("LEFT", MyBase.Style("LEFT"))
            End If
            If Not MyBase.Style("POSITION") Is Nothing Then
                writer.AddStyleAttribute("POSITION", MyBase.Style("POSITION"))
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Div)
            MyBase.Style.Remove("TOP")
            MyBase.Style.Remove("LEFT")
            MyBase.Style.Remove("POSITION")
            MyBase.Attributes.Add("onchange", "javascript:CList_ShowOption(this);")
            MyBase.Attributes.Add("onresize", "javascript:CList_ResizeSelect(this);CList_ShowOption(this);")
            MyBase.AddAttributesToRender(writer)
            If (tag1 <> HtmlTextWriterTag.Unknown) Then
                writer.RenderBeginTag(tag1)
            Else
                writer.RenderBeginTag(Me.TagName)
            End If
        Else
            MyBase.RenderBeginTag(writer)
        End If
    End Sub 'RenderBeginTag

    Private sPrevOptGroup As String = ""
    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        Dim flag1 As Boolean = False
        Dim flag2 As Boolean = (Me.SelectionMode = ListSelectionMode.Single)
        Dim collection1 As CListItemCollection = Me.Items
        Dim num1 As Integer = collection1.Count
        If (num1 > 0) Then
            For num2 As Integer = 0 To num1 - 1
                Dim item1 As CListItem = collection1(num2)
                'render optgroups if they're enabled
                If Me.EnableOptGroups Then

                    Dim sOptGroup As String = item1.OptGroup
                    'if the optgroup has changed, unless it's the first
                    'optgroup, end the previous optgroup
                    If Not sOptGroup = sPrevOptGroup And Not num2 = 0 Then
                        writer.WriteEndTag("optgroup")
                        writer.WriteLine()
                    End If
                    'if it's the first optgroup, or if the optgroup
                    'has changed, start a new optgroup
                    If Not sOptGroup = sPrevOptGroup Or num2 = 0 Then
                        writer.WriteBeginTag("optgroup")
                        writer.WriteAttribute("label", sOptGroup)
                        writer.Write(">"c)
                        writer.WriteLine()
                        sPrevOptGroup = sOptGroup
                    End If
                End If
                writer.WriteBeginTag("option")
                If item1.Selected Then
                    If flag2 Or Me.ListType = ListType.DropDownList Then
                        If flag1 Then
                            Throw New HttpException("You cannot select multiple items in single selection mode")
                        End If
                        flag1 = True
                    End If
                    writer.WriteAttribute("selected", "selected")
                End If
                writer.WriteAttribute("value", item1.Value, True)
                'This line is missing in Microsoft's code, which is why
                'item attributes don't render.  Note that even adding
                'this back won't help if you don't save the state of any
                'attributes you give your items.
                item1.Attributes.Render(writer)
                writer.Write(">"c)
                HttpUtility.HtmlEncode(item1.Text, writer)
                writer.WriteEndTag("option")
                writer.WriteLine()
            Next num2
            'end the last optgroup if they're enabled
            If Me.EnableOptGroups Then
                writer.WriteEndTag("optgroup")
                writer.WriteLine()
            End If
        End If
    End Sub 'RenderContents

    Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.RenderEndTag(writer)
        If Me.EnableHScroll Then
            writer.RenderEndTag()
        End If
    End Sub 'RenderEndTag

    Protected Overrides Function SaveViewState() As Object
        Dim obj1 As Object = MyBase.SaveViewState
        Dim obj2 As Object = Me.Items.SaveViewState
        Dim obj3 As Object = Nothing
        If Me.SaveSelectedIndicesViewState Then
            obj3 = Me.SelectedIndicesInternal
        End If
        If (((obj3 Is Nothing) AndAlso (obj2 Is Nothing)) AndAlso (obj1 Is Nothing)) Then
            Return Nothing
        End If
        Return New Triplet(obj1, obj2, obj3)
    End Function 'SaveViewState

    Friend Sub SelectInternal(ByVal selectedIndices As ArrayList)
        Me.ClearSelection()
        For num1 As Integer = 0 To selectedIndices.Count - 1
            Dim num2 As Integer = CType(selectedIndices(num1), Integer)
            If ((num2 >= 0) AndAlso (num2 < Me.Items.Count)) Then
                Me.Items(num2).Selected = True
            End If
        Next num1
    End Sub 'SelectInternal

    Protected Overrides Sub TrackViewState()
        MyBase.TrackViewState()
        Me.Items.TrackViewState()
    End Sub 'TrackViewState

    Private Function IPostBackDataHandler_LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
        Dim textArray1 As String() = postCollection.GetValues(postDataKey)
        Dim flag1 As Boolean = False
        If (Not textArray1 Is Nothing) Then
            If (Me.SelectionMode = ListSelectionMode.Single) Then
                Dim num1 As Integer = Me.Items.FindByValueInternal(textArray1(0))
                If (Me.SelectedIndex <> num1) Then
                    Me.SelectedIndex = num1
                    flag1 = True
                End If
                Return flag1
            End If
            If Me.ListType = ListType.ListBox Then
                Dim num2 As Integer = textArray1.Length
                Dim list1 As ArrayList = Me.SelectedIndicesInternal
                Dim list2 As New ArrayList(num2)
                For num3 As Integer = 0 To num2 - 1
                    list2.Add(Me.Items.FindByValueInternal(textArray1(num3)))
                Next num3
                Dim num4 As Integer = 0
                If (Not list1 Is Nothing) Then
                    num4 = list1.Count
                End If
                If (num4 = num2) Then
                    For num5 As Integer = 0 To num2 - 1
                        If (CType(list2(num5), Integer) <> CType(list1(num5), Integer)) Then
                            flag1 = True
                            Exit For
                        End If
                    Next num5
                Else
                    flag1 = True
                End If
                If flag1 Then
                    SelectInternal(list2)
                End If
                Return flag1
            End If
        End If
        Select Case Me.ListType
            Case ListType.ListBox
                If (Me.SelectedIndex <> -1) Then
                    Me.SelectedIndex = -1
                    flag1 = True
                End If
                Return flag1
            Case ListType.DropDownList
                Return False
        End Select
    End Function 'IPostBackDataHandler_LoadPostData

    Private Sub IPostBackDataHandler_RaisePostDataChangedEvent() Implements IPostBackDataHandler.RaisePostDataChangedEvent
        Me.OnSelectedIndexChanged(EventArgs.Empty)
    End Sub 'IPostBackDataHandler_RaisePostDataChangedEvent

    ' Properties
    <Category("Behavior"), DefaultValue(False), Description("CList_AutoPostBack")> _
    Public Overridable Property AutoPostBack() As Boolean
        Get
            Dim obj1 As Object = Me.ViewState("AutoPostBack")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            Me.ViewState("AutoPostBack") = value
        End Set
    End Property 'AutoPostBack

    <Browsable(False)> _
    Public Overrides Property BorderColor() As Color
        Get
            Return MyBase.BorderColor
        End Get
        Set(ByVal value As Color)
            MyBase.BorderColor = value
        End Set
    End Property 'BorderColor

    <Browsable(False)> _
    Public Overrides Property BorderStyle() As BorderStyle
        Get
            Return MyBase.BorderStyle
        End Get
        Set(ByVal value As BorderStyle)
            MyBase.BorderStyle = value
        End Set
    End Property 'BorderStyle

    <Browsable(False)> _
    Public Overrides Property BorderWidth() As Unit
        Get
            Return MyBase.BorderWidth
        End Get
        Set(ByVal value As Unit)
            MyBase.BorderWidth = value
        End Set
    End Property 'BorderWidth

    <Description("CList_DataMember"), Category("Data"), DefaultValue("")> _
    Public Overridable Property DataMember() As String
        Get
            Dim obj1 As Object = Me.ViewState("DataMember")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, String)
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.ViewState("DataMember") = value
        End Set
    End Property 'DataMember

    <Bindable(True), Description("CList_DataSource"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Data"), DefaultValue(CType(Nothing, String))> _
    Public Overridable Property DataSource() As Object
        Get
            Return Me._dataSource
        End Get
        Set(ByVal value As Object)
            If (((Not value Is Nothing) AndAlso Not TypeOf value Is IListSource) AndAlso Not TypeOf value Is IEnumerable) Then
                Throw New ArgumentException(Me.ID + " is an invalid datasource type.")
            End If
            Me._dataSource = value
        End Set
    End Property 'DataSource

    <Description("CList_DataTextField"), Category("Data"), DefaultValue("")> _
    Public Overridable Property DataTextField() As String
        Get
            Dim obj1 As Object = Me.ViewState("DataTextField")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, String)
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.ViewState("DataTextField") = value
        End Set
    End Property 'DataTextField

    <Description("CList_DataTextFormatString"), DefaultValue(""), Category("Data")> _
    Public Overridable Property DataTextFormatString() As String
        Get
            Dim obj1 As Object = Me.ViewState("DataTextFormatString")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, String)
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.ViewState("DataTextFormatString") = value
        End Set
    End Property 'DataTextFormatString

    <Description("CList_DataOptGroupField"), Category("Data"), DefaultValue("")> _
    Public Overridable Property DataOptGroupField() As String
        Get
            Dim obj1 As Object = Me.ViewState("DataOptGroupField")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, String)
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.ViewState("DataOptGroupField") = value
        End Set
    End Property 'DataOptGroupField

    <Description("CList_DataOptGroupFormatString"), DefaultValue(""), Category("Data")> _
    Public Overridable Property DataOptGroupFormatString() As String
        Get
            Dim obj1 As Object = Me.ViewState("DataOptGroupFormatString")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, String)
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.ViewState("DataOptGroupFormatString") = value
        End Set
    End Property 'DataOptGroupFormatString

    <Description("CList_DataValueField"), DefaultValue(""), Category("Data")> _
    Public Overridable Property DataValueField() As String
        Get
            Dim obj1 As Object = Me.ViewState("DataValueField")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, String)
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.ViewState("DataValueField") = value
        End Set
    End Property 'DataValueField

    <Description("CList_Rows"), DefaultValue(4), Bindable(True), Category("Appearance")> _
    Public Overridable Property Rows() As Integer
        Get
            Dim obj1 As Object = Me.ViewState("Rows")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, Integer)
            End If
            Return 4
        End Get
        Set(ByVal value As Integer)
            If ((value < 1) OrElse (value > 2000)) Then
                Throw New ArgumentOutOfRangeException("value")
            End If
            Me.ViewState("Rows") = value
        End Set
    End Property 'Rows

    <PersistenceMode(PersistenceMode.InnerDefaultProperty), Category("Default"), DefaultValue(CType(Nothing, String)), MergableProperty(False), Description("CList_Items")> _
    Public Overridable ReadOnly Property Items() As CListItemCollection
        Get
            If (Me._items Is Nothing) Then
                Me._items = New CListItemCollection
                If MyBase.IsTrackingViewState Then
                    Me._items.TrackViewState()
                End If
            End If
            Return Me._items
        End Get
    End Property 'Items

    Private ReadOnly Property SaveSelectedIndicesViewState() As Boolean
        Get
            If (((Not MyBase.Events(CListBox._EventSelectedIndexChanged) Is Nothing) OrElse Not Me.Enabled) OrElse Not Me.Visible) Then
                Return True
            End If
            Dim type1 As Type = MyBase.GetType
            'If (((Not type1 Is GetType(DropDownList)) AndAlso (Not type1 Is GetType(ListBox))) AndAlso ((Not type1 Is GetType(CheckBoxList)) AndAlso (Not type1 Is GetType(RadioButtonList)))) Then
            If (Not type1 Is GetType(CListBox)) Then
                Return True
            End If
            Return False
        End Get
    End Property 'SaveSelectedIndicesViewState

    <DefaultValue(0), Bindable(True), Browsable(False), Category("Behavior"), Description("CList_SelectedIndex"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overridable Property SelectedIndex() As Integer
        Get
            For num1 As Integer = 0 To Me.Items.Count - 1
                If Me.Items(num1).Selected Then
                    If Me.ListType = ListType.DropDownList Then
                        If ((num1 < 0) AndAlso (Me.Items.Count > 0)) Then
                            Me.Items(0).Selected = True
                            num1 = 0
                        End If
                    End If
                    Return num1
                End If
            Next num1
            Return -1
        End Get
        Set(ByVal value As Integer)
            If (Me.Items.Count = 0) Then
                Me._cachedSelectedIndex = value
            Else
                If ((value < -1) OrElse (value >= Me.Items.Count)) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                Me.ClearSelection()
                If (value >= 0) Then
                    Me.Items(value).Selected = True
                End If
            End If
        End Set
    End Property 'SelectedIndex

    Friend Overridable ReadOnly Property SelectedIndicesInternal() As ArrayList
        Get
            Dim list1 As ArrayList = Nothing
            For num1 As Integer = 0 To Me.Items.Count - 1
                If Me.Items(num1).Selected Then
                    If (list1 Is Nothing) Then
                        list1 = New ArrayList(3)
                    End If
                    list1.Add(num1)
                End If
            Next num1
            Return list1
        End Get
    End Property 'SelectedIndicesInternal

    <Browsable(False), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(CType(Nothing, String)), Description("CList_SelectedItem")> _
    Public Overridable ReadOnly Property SelectedItem() As CListItem
        Get
            Dim num1 As Integer = Me.SelectedIndex
            If (num1 >= 0) Then
                Return Me.Items(num1)
            End If
            Return Nothing
        End Get
    End Property 'SelectedItem

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(True), Browsable(False), Category("Behavior"), DefaultValue(""), Description("CList_SelectedValue")> _
    Public Overridable Property SelectedValue() As String
        Get
            Dim num1 As Integer = Me.SelectedIndex
            If (num1 >= 0) Then
                Return Me.Items(num1).Value
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            If (Me.Items.Count = 0) Then
                Me._cachedSelectedValue = value
            Else
                If (value Is Nothing) Then
                    Me.ClearSelection()
                Else
                    Dim item1 As CListItem = Me.Items.FindByValue(value)
                    If (item1 Is Nothing) Then
                        Throw New ArgumentOutOfRangeException(value)
                    End If
                    Me.ClearSelection()
                    item1.Selected = True
                End If
            End If
        End Set
    End Property 'SelectedValue

    <DefaultValue(0), Category("Behavior"), Description("CList_SelectionMode")> _
    Public Overridable Property SelectionMode() As ListSelectionMode
        Get
            Dim obj1 As Object = Me.ViewState("SelectionMode")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, ListSelectionMode)
            End If
            Return ListSelectionMode.Single
        End Get
        Set(ByVal value As ListSelectionMode)
            If ((value < ListSelectionMode.Single) OrElse (value > ListSelectionMode.Multiple)) Then
                Throw New ArgumentOutOfRangeException("value")
            End If
            Me.ViewState("SelectionMode") = value
        End Set
    End Property 'SelectionMode

    <DefaultValue(0), Category("Behavior"), Description("ListType")> _
    Public Overridable Property ListType() As ListType
        Get
            Dim obj1 As Object = Me.ViewState("CListType")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, ListType)
            End If
            Return ListType.ListBox
        End Get
        Set(ByVal value As ListType)
            If ((value < ListType.ListBox) OrElse (value > ListType.DropDownList)) Then
                Throw New ArgumentOutOfRangeException("value")
            End If
            If value = ListType.DropDownList Then
                Me.EnableHScroll = False
            End If
            Me.ViewState("CListType") = value
        End Set
    End Property 'CListType

    <Category("Appearance"), DefaultValue(False), Description("CList_EnableOptGroups")> _
    Public Overridable Property EnableOptGroups() As Boolean
        Get
            Dim obj1 As Object = Me.ViewState("EnableOptGroups")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            Me.ViewState("EnableOptGroups") = value
        End Set
    End Property 'EnableOptGroups

    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public Overrides Property Width() As Unit
        Get
            If Me.EnableHScroll AndAlso MyBase.Width.IsEmpty Then
                Return Unit.Pixel(100)
            End If
            Return MyBase.Width
        End Get
        Set(ByVal Value As Unit)
            MyBase.Width = Value
        End Set
    End Property 'Width

    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public Overrides Property Height() As Unit
        Get
            If Me.EnableHScroll AndAlso MyBase.Height.IsEmpty Then
                Return Unit.Pixel(100)
            End If
            Return MyBase.Height
        End Get
        Set(ByVal Value As Unit)
            MyBase.Height = Value
        End Set
    End Property 'Height

    <Category("Appearance"), DefaultValue(False), Description("CList_EnableHScroll")> _
    Public Overridable Property EnableHScroll() As Boolean
        Get
            Dim obj1 As Object = Me.ViewState("EnableHScroll")
            If (Not obj1 Is Nothing) Then
                Return CType(obj1, Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            If value Then
                Me.ListType = ListType.ListBox
            End If
            Me.ViewState("EnableHScroll") = value
        End Set
    End Property 'EnableHScroll

    <Browsable(False), Bindable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property ToolTip() As String
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
        End Set
    End Property 'ToolTip

End Class 'CListBox

<TypeConverter(GetType(ExpandableObjectConverter)), ControlBuilder(GetType(CListItemControlBuilder))> _
Public Class CListItem
    Implements IStateManager, IParserAccessor, IAttributeAccessor
    ' Fields
    Private Const _SELECTED As Integer = 0
    Private Const _MARKED As Integer = 1
    Private Const _TEXTISDIRTY As Integer = 2
    Private Const _VALUEISDIRTY As Integer = 3
    Private Const _OPTGROUPISDIRTY As Integer = 4
    Private Const _ATTRIBUTESISDIRTY As Integer = 5
    Private _attributes As System.Web.UI.AttributeCollection
    Private _attributes2 As Pair()
    Private _misc As BitArray
    Private _optGroup As String
    Private _text As String
    Private _value As String

    ' Methods
    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub 'New

    Public Sub New(ByVal text As String)
        Me.New(text, Nothing)
    End Sub 'New

    Public Sub New(ByVal text As String, ByVal value As String)
        Me._text = text
        Me._value = value
        Me._misc = New BitArray(6)
    End Sub 'New

    Public Overloads Function Equals(ByVal o As Object) As Boolean
        Dim item1 As CListItem
        If TypeOf o Is CListItem Then
            item1 = CType(o, CListItem)
        Else
            item1 = Nothing
        End If
        If ((Not item1 Is Nothing) AndAlso Me.Value.Equals(item1.Value)) Then
            Return Me.Text.Equals(item1.Text)
        End If
        Return False
    End Function 'Equals

    Public Shared Function FromString(ByVal s As String) As CListItem
        Return New CListItem(s)
    End Function 'FromString

    Public Overrides Function GetHashCode() As Integer
        Return MyBase.GetHashCode
    End Function 'GetHashCode

    Friend Sub LoadViewState(ByVal state As Object)
        Dim arrState As Object() = CType(state, Object())

        If (Not arrState(0) Is Nothing) Then
            If TypeOf arrState(0) Is Pair Then
                Dim pair1 As Pair = CType(arrState(0), Pair)
                If (Not pair1.First Is Nothing) Then
                    Me.Text = CType(pair1.First, String)
                End If
                Me.Value = CType(pair1.Second, String)
            Else
                Me.Text = CType(arrState(0), String)
            End If
        End If

        'custom state management for OptGroup
        If Not arrState(1) Is Nothing Then
            Me.OptGroup = CType(arrState(1), String)
        End If

        'custom state management for Attributes
        If Not arrState(2) Is Nothing Then
            If TypeOf arrState(2) Is Pair() Then
                Dim colAttributes As Pair() = CType(arrState(2), Pair())
                For i As Integer = 0 To colAttributes.Length - 1
                    Me.Attributes.Add(colAttributes(i).First.ToString, colAttributes(i).Second.ToString)
                Next i
            End If
        End If
    End Sub 'LoadViewState

    Friend Function SaveViewState() As Object
        Dim arrState(2) As Object

        If (Me._misc.Get(2) AndAlso Me._misc.Get(3)) Then
            arrState(0) = New Pair(Me.Text, Me.Value)
        ElseIf Me._misc.Get(2) Then
            arrState(0) = Me.Text
        ElseIf Me._misc.Get(3) Then
            arrState(0) = New Pair(Nothing, Me.Value)
        Else
            arrState(0) = Nothing
        End If

        'custom state management for OptGroup
        arrState(1) = Me.OptGroup

        ''custom state management for Attributes
        If Me.Attributes.Count > 0 Then
            ReDim _attributes2(Me.Attributes.Count - 1)
            Dim i As Integer = 0

            Dim keys As IEnumerator = Me.Attributes.Keys.GetEnumerator
            Dim key As String
            While keys.MoveNext()
                key = CType(keys.Current, String)
                _attributes2(i) = New Pair(key, Me.Attributes.Item(key))
                i += 1
            End While

            arrState(2) = _attributes2
        End If

        Return arrState
    End Function 'SaveViewState

    Private Function IAttributeAccessor_GetAttribute(ByVal name As String) As String Implements IAttributeAccessor.GetAttribute
        Return Me.Attributes(name)
    End Function 'IAttributeAccessor_GetAttribute

    Private Sub IAttributeAccessor_SetAttribute(ByVal name As String, ByVal value As String) Implements IAttributeAccessor.SetAttribute
        Me.Attributes(name) = value
        Me._misc.Set(5, True)
    End Sub 'IAttributeAccessor_SetAttribute

    Private Sub IParserAccessor_AddParsedSubObject(ByVal obj As Object) Implements IParserAccessor.AddParsedSubObject
        If TypeOf obj Is LiteralControl Then
            Me.Text = CType(obj, LiteralControl).Text
        Else
            If TypeOf obj Is DataBoundLiteralControl Then
                Throw New HttpException("You cannot bind an CListItem to a datasource")
            End If
            Throw New HttpException("CListItem cannot have children of type " + obj.GetType.Name.ToString)
        End If
    End Sub 'IParserAccessor_AddParsedSubObject

    <Browsable(True), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public ReadOnly Property IsTrackingViewState() As Boolean Implements IStateManager.IsTrackingViewState
        Get
            Return Me._misc.Get(1)
        End Get
    End Property 'IsTrackingViewState

    Private Sub IStateManager_LoadViewState(ByVal state As Object) Implements IStateManager.LoadViewState
        Me.LoadViewState(state)
    End Sub 'IStateManager_LoadViewState

    Private Function IStateManager_SaveViewState() As Object Implements IStateManager.SaveViewState
        Return Me.SaveViewState
    End Function 'IStateManager_SaveViewState

    Private Sub IStateManager_TrackViewState() Implements IStateManager.TrackViewState
        Me.TrackViewState()
    End Sub 'IStateManager_TrackViewState

    Public Overrides Function ToString() As String
        Return Me.Text
    End Function 'ToString

    Friend Sub TrackViewState()
        Me._misc.Set(1, True)
    End Sub 'TrackViewState

    ' Properties
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public ReadOnly Property Attributes() As System.Web.UI.AttributeCollection
        Get
            If (Me._attributes Is Nothing) Then
                Me._attributes = New System.Web.UI.AttributeCollection(New StateBag(True))
            End If
            Return Me._attributes
        End Get
    End Property 'Attributes

    Friend Property Dirty() As Boolean
        Get
            If Not Me._misc.Get(2) Then
                If Not Me._misc.Get(3) Then
                    If Not Me._misc.Get(4) Then
                        Return Me._misc.Get(5)
                    End If
                    Return True
                End If
                Return True
            End If
            Return True
        End Get
        Set(ByVal value As Boolean)
            Me._misc.Set(2, value)
            Me._misc.Set(3, value)
            Me._misc.Set(4, value)
            Me._misc.Set(5, value)
        End Set
    End Property 'Dirty

    <DefaultValue(False)> _
    Public Property Selected() As Boolean
        Get
            Return Me._misc.Get(0)
        End Get
        Set(ByVal value As Boolean)
            Me._misc.Set(0, value)
        End Set
    End Property 'Selected

    <DefaultValue(""), PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)> _
    Public Property [Text]() As String
        Get
            If (Not Me._text Is Nothing) Then
                Return Me._text
            End If
            If (Not Me._value Is Nothing) Then
                Return Me._value
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me._text = value
            If Me.IsTrackingViewState Then
                Me._misc.Set(2, True)
            End If
        End Set
    End Property '[Text]

    <DefaultValue("")> _
    Public Property Value() As String
        Get
            If (Not Me._value Is Nothing) Then
                Return Me._value
            End If
            If (Not Me._text Is Nothing) Then
                Return Me._text
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me._value = value
            If Me.IsTrackingViewState Then
                Me._misc.Set(3, True)
            End If
        End Set
    End Property 'Value

    <DefaultValue("")> _
    Public Property OptGroup() As String
        Get
            Return Me._optGroup
        End Get
        Set(ByVal value As String)
            Me._optGroup = value
            If Me.IsTrackingViewState Then
                Me._misc.Set(4, True)
            End If
        End Set
    End Property 'OptGroup

End Class 'CListItem

<Editor(GetType(CListItemsCollectionEditor), GetType(UITypeEditor)), DefaultMember("Item")> _
Public Class CListItemCollection
    Implements IList, ICollection, IEnumerable, IStateManager

    ' Fields
    Private _xListItems As ArrayList
    Private _marked As Boolean
    Private _saveAll As Boolean

    ' Methods
    Public Sub New()
        Me._xListItems = New ArrayList
        Me._marked = False
        Me._saveAll = False
    End Sub 'New

    Public Sub Add(ByVal item As String)
        Me.Add(New CListItem(item))
    End Sub 'Add

    Public Sub Add(ByVal item As CListItem)
        Me._xListItems.Add(item)
        If Me._marked Then
            item.Dirty = True
        End If
    End Sub 'Add

    Public Sub AddRange(ByVal _items As CListItem())
        If (_items Is Nothing) Then
            Throw New ArgumentNullException("_items")
        End If
        Dim itemArray1 As CListItem() = _items
        For num1 As Integer = 0 To itemArray1.Length - 1
            Dim item1 As CListItem = itemArray1(num1)
            Me.Add(item1)
        Next num1
    End Sub 'AddRange

    Public Sub Clear() Implements IList.Clear
        Me._xListItems.Clear()
        If Me._marked Then
            Me._saveAll = True
        End If
    End Sub 'Clear

    Public Function Contains(ByVal item As CListItem) As Boolean
        Return Me._xListItems.Contains(item)
    End Function 'Contains

    Public Sub CopyTo(ByVal array As Array, ByVal index As Integer) Implements IList.CopyTo
        Me._xListItems.CopyTo(array, index)
    End Sub 'CopyTo

    Public Function FindByText(ByVal _text As String) As CListItem
        Dim num1 As Integer = Me.FindByTextInternal(_text)
        If (num1 <> -1) Then
            Return CType(Me._xListItems(num1), CListItem)
        End If
        Return Nothing
    End Function 'FindByText

    Friend Function FindByTextInternal(ByVal _text As String) As Integer
        Dim num1 As Integer = 0
        For Each item1 As CListItem In Me._xListItems
            If item1.Text.Equals(_text) Then
                Return num1
            End If
            num1 += 1
        Next item1
        Return -1
    End Function 'FindByTextInternal

    Public Function FindByValue(ByVal value As String) As CListItem
        Dim num1 As Integer = Me.FindByValueInternal(value)
        If (num1 <> -1) Then
            Return CType(Me._xListItems(num1), CListItem)
        End If
        Return Nothing
    End Function 'FindByValue

    Friend Function FindByValueInternal(ByVal value As String) As Integer
        Dim num1 As Integer = 0
        For Each item1 As CListItem In Me._xListItems
            If item1.Value.Equals(value) Then
                Return num1
            End If
            num1 += 1
        Next item1
        Return -1
    End Function 'FindByValueInternal

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me._xListItems.GetEnumerator
    End Function 'GetEnumerator

    Public Function IndexOf(ByVal item As CListItem) As Integer
        Return Me._xListItems.IndexOf(item)
    End Function 'IndexOf

    Public Sub Insert(ByVal index As Integer, ByVal item As String)
        Me.Insert(index, New CListItem(item))
    End Sub 'Insert

    Public Sub Insert(ByVal index As Integer, ByVal item As CListItem)
        Me._xListItems.Insert(index, item)
        If Me._marked Then
            Me._saveAll = True
        End If
    End Sub 'Insert

    Friend Sub LoadViewState(ByVal state As Object)
        If (Not state Is Nothing) Then
            If TypeOf state Is Pair Then
                Dim pair1 As Pair = CType(state, Pair)
                Dim list1 As ArrayList = CType(pair1.First, ArrayList)
                Dim list2 As ArrayList = CType(pair1.Second, ArrayList)
                For num1 As Integer = 0 To list1.Count - 1
                    Dim num2 As Integer = CType(list1(num1), Integer)
                    If (num2 < Me.Count) Then
                        Me(num2).LoadViewState(list2(num1))
                    Else
                        Dim item1 As New CListItem
                        item1.LoadViewState(list2(num1))
                        Me.Add(item1)
                    End If
                Next num1
            Else
                Dim triplet1 As Triplet = CType(state, Triplet)
                Me._xListItems = New ArrayList(CType(triplet1.First, Integer))
                Me._saveAll = True
                Dim textArray1 As String() = CType(triplet1.Second, String())
                Dim textArray2 As String() = CType(triplet1.Third, String())
                For num3 As Integer = 0 To textArray1.Length - 1
                    Me.Add(New CListItem(textArray1(num3), textArray2(num3)))
                Next num3
            End If
        End If
    End Sub 'LoadViewState

    Public Sub Remove(ByVal item As String)
        Dim num1 As Integer = Me.IndexOf(New CListItem(item))
        If (num1 >= 0) Then
            Me.RemoveAt(num1)
        End If
    End Sub 'Remove

    Public Sub Remove(ByVal item As CListItem)
        Dim num1 As Integer = Me.IndexOf(item)
        If (num1 >= 0) Then
            Me.RemoveAt(num1)
        End If
    End Sub 'Remove

    Public Sub RemoveAt(ByVal index As Integer) Implements IList.RemoveAt
        Me._xListItems.RemoveAt(index)
        If Me._marked Then
            Me._saveAll = True
        End If
    End Sub 'RemoveAt

    Friend Function SaveViewState() As Object
        If Me._saveAll Then
            Dim num1 As Integer = Me.Count
            Dim objArray1 As Object() = New String(num1 - 1) {}
            Dim objArray2 As Object() = New String(num1 - 1) {}
            For num2 As Integer = 0 To num1 - 1
                objArray1(num2) = Me(num2).Text
                objArray2(num2) = Me(num2).Value
            Next num2
            Return New Triplet(num1, objArray1, objArray2)
        End If
        Dim list1 As New ArrayList(4)
        Dim list2 As New ArrayList(4)
        For num3 As Integer = 0 To Me.Count - 1
            Dim obj1 As Object = Me(num3).SaveViewState
            If (Not obj1 Is Nothing) Then
                list1.Add(num3)
                list2.Add(obj1)
            End If
        Next num3
        If (list1.Count > 0) Then
            Return New Pair(list1, list2)
        End If
        Return Nothing
    End Function 'SaveViewState

    Private Function IList_Add(ByVal item As Object) As Integer Implements IList.Add
        Dim item1 As CListItem = CType(item, CListItem)
        Dim num1 As Integer = Me._xListItems.Add(item1)
        If Me._marked Then
            item1.Dirty = True
        End If
        Return num1
    End Function 'IList_Add

    Private Function IList_Contains(ByVal item As Object) As Boolean Implements IList.Contains
        Return Me.Contains(CType(item, CListItem))
    End Function 'IList_Contains

    Public ReadOnly Property IsFixedSize() As Boolean Implements IList.IsFixedSize
        Get
            Return False
        End Get
    End Property 'IsFixedSize

    Private Function IList_IndexOf(ByVal item As Object) As Integer Implements IList.IndexOf
        Return Me.IndexOf(CType(item, CListItem))
    End Function 'IList_IndexOf

    Private Sub IList_Insert(ByVal index As Integer, ByVal item As Object) Implements IList.Insert
        Me.Insert(index, CType(item, CListItem))
    End Sub 'IList_Insert

    Private Sub IList_Remove(ByVal item As Object) Implements IList.Remove
        Me.Remove(CType(item, CListItem))
    End Sub 'IList_Remove

    Public ReadOnly Property IsTrackingViewState() As Boolean Implements IStateManager.IsTrackingViewState
        Get
            Return Me._marked
        End Get
    End Property 'IsTrackingViewState

    Private Sub IStateManager_LoadViewState(ByVal state As Object) Implements IStateManager.LoadViewState
        Me.LoadViewState(state)
    End Sub 'IStateManager_LoadViewState

    Private Function IStateManager_SaveViewState() As Object Implements IStateManager.SaveViewState
        Return Me.SaveViewState
    End Function 'IStateManager_SaveViewState

    Private Sub IStateManager_TrackViewState() Implements IStateManager.TrackViewState
        Me.TrackViewState()
    End Sub 'IStateManager_TrackViewState

    Friend Sub TrackViewState()
        Me._marked = True
        For num1 As Integer = 0 To Me.Count - 1
            Me(num1).TrackViewState()
        Next num1
    End Sub 'TrackViewState

    ' Properties
    Public Property Capacity() As Integer
        Get
            Return Me._xListItems.Capacity
        End Get
        Set(ByVal value As Integer)
            Me._xListItems.Capacity = value
        End Set
    End Property 'Capacity

    Public ReadOnly Property Count() As Integer Implements ICollection.Count
        Get
            Return Me._xListItems.Count
        End Get
    End Property 'Count

    Public ReadOnly Property IsReadOnly() As Boolean Implements IList.IsReadOnly
        Get
            Return Me._xListItems.IsReadOnly
        End Get
    End Property 'IsReadOnly

    Public ReadOnly Property IsSynchronized() As Boolean Implements ICollection.IsSynchronized
        Get
            Return Me._xListItems.IsSynchronized
        End Get
    End Property 'IsSynchronized

    Private Property IList_Item(ByVal index As Integer) As Object Implements IList.Item
        Get
            Return Me._xListItems(index)
        End Get
        Set(ByVal Value As Object)
            Me._xListItems(index) = CType(Value, CListItem)
        End Set
    End Property 'IList_Item

    Default Public ReadOnly Property Item(ByVal index As Integer) As CListItem
        Get
            Return CType(Me.IList_Item(index), CListItem)
        End Get
    End Property 'Item

    Public ReadOnly Property SyncRoot() As Object Implements ICollection.SyncRoot
        Get
            Return Me
        End Get
    End Property 'SyncRoot

End Class 'CListItemCollection

Public Class CListItemControlBuilder
    Inherits ControlBuilder

    ' Methods
    Public Sub New()
    End Sub '

    Public Overrides Function AllowWhitespaceLiterals() As Boolean
        Return False
    End Function '

    Public Overrides Function HtmlDecodeLiterals() As Boolean
        Return True
    End Function '

End Class 'CListItemControlBuilder

Public Class CListDataBindingHandler
    Inherits DataBindingHandler

    ' Methods
    Public Sub New()
    End Sub 'New

    Public Overrides Sub DataBindControl(ByVal designerHost As IDesignerHost, ByVal _control As Control)
        Dim binding1 As DataBinding = CType(_control, IDataBindingsAccessor).DataBindings("DataSource")
        If (Not binding1 Is Nothing) Then
            Dim control1 As CListBox = CType(_control, CListBox)
            control1.Items.Clear()
            control1.Items.Add("bound")
        End If
    End Sub 'DataBindControl

End Class 'CListDataBindingHandler

Public Class CListDesigner
    Inherits ControlDesigner
    Implements IDataSourceProvider

    ' Fields
    Private _xList As CListBox

    ' Methods
    Public Sub New()
    End Sub 'New

    Public Overrides Function GetDesignTimeHtml() As String
        Dim collection1 As CListItemCollection = Me._xList.Items
        If (collection1.Count > 0) Then
            'Return MyBase.GetDesignTimeHtml
            Return GetDesignTimeResize()
        End If
        If Me.IsDataBound Then
            collection1.Add("bound")
        Else
            collection1.Add("unbound")
        End If
        'Dim text1 As String = MyBase.GetDesignTimeHtml
        Dim text1 As String = GetDesignTimeResize()
        collection1.Clear()
        Return text1
    End Function 'GetDesignTimeHtml

    Public Overridable Function GetDesignTimeResize() As String
        Dim _xList As CListBox = CType(Component, CListBox)
        If _xList.EnableHScroll Then
            Dim str As String = MyBase.GetDesignTimeHtml
            str = Replace(str, "<select", "<select style=width:" & _xList.Width.ToString)
            Dim _itemCount As Integer = _xList.Items.Count
            Dim _selectTagEnd As Integer = InStr(str, ">")
            _selectTagEnd = InStr(_selectTagEnd + 1, str, ">")
            Dim _sizeAttribute As Integer = InStr(str, " size=")
            If _sizeAttribute > 0 And _sizeAttribute < _selectTagEnd Then
                str = Replace(str, "size=""4""", "size=""" & IIf(_itemCount < 2, 2, _itemCount).ToString & """")
            Else
                str = Replace(str, "<select", "<select size=""" & IIf(_itemCount < 2, 2, _itemCount).ToString & """")
            End If
            Return str
        Else
            Return MyBase.GetDesignTimeHtml
        End If
    End Function 'GetDesignTimeResize

    Public Function GetResolvedSelectedDataSource() As IEnumerable Implements IDataSourceProvider.GetResolvedSelectedDataSource
        Dim enumerable1 As IEnumerable = Nothing
        Dim binding1 As DataBinding = MyBase.DataBindings("DataSource")
        If (Not binding1 Is Nothing) Then
            enumerable1 = DesignTimeData.GetSelectedDataSource(Me._xList, binding1.Expression, Me.DataMember)
        End If
        Return enumerable1
    End Function 'GetResolvedSelectedDataSource

    Public Function GetSelectedDataSource() As Object Implements IDataSourceProvider.GetSelectedDataSource
        Dim obj1 As Object = Nothing
        Dim binding1 As DataBinding = MyBase.DataBindings("DataSource")
        If (Not binding1 Is Nothing) Then
            obj1 = DesignTimeData.GetSelectedDataSource(Me._xList, binding1.Expression)
        End If
        Return obj1
    End Function 'GetSelectedDataSource

    Public Overrides Sub Initialize(ByVal component As IComponent)
        MyBase.Initialize(component)
        Me._xList = CType(component, CListBox)
    End Sub 'Initialize

    Private Function IsDataBound() As Boolean
        Dim binding1 As DataBinding = MyBase.DataBindings("DataSource")
        Return (Not binding1 Is Nothing)
    End Function 'IsDataBound

    Public Overrides Sub OnComponentChanged(ByVal source As Object, ByVal ce As ComponentChangedEventArgs)
        If ((Not ce.Member Is Nothing) AndAlso (ce.Member.Name.Equals("DataSource") OrElse ce.Member.Name.Equals("DataMember"))) Then
            Me.OnDataSourceChanged()
        End If
        MyBase.OnComponentChanged(source, ce)
    End Sub 'OnComponentChanged

    Public Overridable Sub OnDataSourceChanged()
    End Sub 'OnDataSourceChanged

    Protected Overrides Sub PreFilterProperties(ByVal properties As IDictionary)
        MyBase.PreFilterProperties(properties)
        Dim descriptor1 As PropertyDescriptor = CType(properties("DataSource"), PropertyDescriptor)
        Dim collection1 As System.ComponentModel.AttributeCollection = descriptor1.Attributes
        Dim attributeArray1 As Attribute() = New Attribute((collection1.Count + 1) - 1) {}
        collection1.CopyTo(attributeArray1, 0)
        attributeArray1(collection1.Count) = New TypeConverterAttribute(GetType(DataSourceConverter))
        descriptor1 = TypeDescriptor.CreateProperty(MyBase.GetType, "DataSource", GetType(String), attributeArray1)
        properties("DataSource") = descriptor1
        descriptor1 = CType(properties("DataMember"), PropertyDescriptor)
        Dim attributeArray3 As Attribute() = New Attribute() {New TypeConverterAttribute(GetType(DataMemberConverter))}
        descriptor1 = TypeDescriptor.CreateProperty(MyBase.GetType, descriptor1, attributeArray3)
        properties("DataMember") = descriptor1
        attributeArray3 = New Attribute() {New TypeConverterAttribute(GetType(DataFieldConverter))}
        Dim attributeArray2 As Attribute() = attributeArray3
        descriptor1 = CType(properties("DataTextField"), PropertyDescriptor)
        descriptor1 = TypeDescriptor.CreateProperty(MyBase.GetType, descriptor1, attributeArray2)
        properties("DataTextField") = descriptor1
        descriptor1 = CType(properties("DataValueField"), PropertyDescriptor)
        descriptor1 = TypeDescriptor.CreateProperty(MyBase.GetType, descriptor1, attributeArray2)
        properties("DataValueField") = descriptor1
        descriptor1 = CType(properties("DataOptGroupField"), PropertyDescriptor)
        descriptor1 = TypeDescriptor.CreateProperty(MyBase.GetType, descriptor1, attributeArray2)
        properties("DataOptGroupField") = descriptor1
    End Sub 'PreFilterProperties

    ' Properties
    Public Property DataMember() As String
        Get
            Return Me._xList.DataMember
        End Get
        Set(ByVal value As String)
            Me._xList.DataMember = value
            Me.OnDataSourceChanged()
        End Set
    End Property 'DataMember

    Public Property DataSource() As String
        Get
            Dim binding1 As DataBinding = MyBase.DataBindings("DataSource")
            If (Not binding1 Is Nothing) Then
                Return binding1.Expression
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            If ((value Is Nothing) OrElse (value.Length = 0)) Then
                MyBase.DataBindings.Remove("DataSource")
            Else
                Dim binding1 As DataBinding = MyBase.DataBindings("DataSource")
                If (binding1 Is Nothing) Then
                    binding1 = New DataBinding("DataSource", GetType(IEnumerable), value)
                Else
                    binding1.Expression = value
                End If
                MyBase.DataBindings.Add(binding1)
            End If
            Me.OnDataSourceChanged()

        End Set
    End Property 'DataSource

    Public Property DataTextField() As String
        Get
            Return Me._xList.DataTextField
        End Get
        Set(ByVal value As String)
            Me._xList.DataTextField = value
        End Set
    End Property 'DataTextField

    Public Property DataValueField() As String
        Get
            Return Me._xList.DataValueField
        End Get
        Set(ByVal value As String)
            Me._xList.DataValueField = value
        End Set
    End Property 'DataValueField

    Public Property DataOptGroupField() As String
        Get
            Return Me._xList.DataOptGroupField
        End Get
        Set(ByVal value As String)
            Me._xList.DataOptGroupField = value
        End Set
    End Property 'DataValueField

End Class 'CListDesigner

Public Class CListItemsCollectionEditor
    Inherits CollectionEditor

    ' Methods
    Public Sub New(ByVal type As Type)
        MyBase.New(type)
    End Sub 'New

    Protected Overrides Function CanSelectMultipleInstances() As Boolean
        Return False
    End Function 'CanSelectMultipleInstances

End Class 'CListItemsCollectionEditor

Friend Class DataSourceHelper

    ' Methods
    Private Sub New()
    End Sub 'New

    Friend Shared Function GetResolvedDataSource(ByVal dataSource As Object, ByVal dataMember As String) As IEnumerable
        If (Not dataSource Is Nothing) Then
            Dim source1 As IListSource
            If TypeOf dataSource Is IListSource Then
                source1 = CType(dataSource, IListSource)
            Else
                source1 = Nothing
            End If
            If (Not source1 Is Nothing) Then
                Dim list1 As IList = source1.GetList
                If Not source1.ContainsListCollection Then
                    Return list1
                End If
                If ((Not list1 Is Nothing) AndAlso TypeOf list1 Is ITypedList) Then
                    Dim list2 As ITypedList = CType(list1, ITypedList)
                    Dim collection1 As PropertyDescriptorCollection = list2.GetItemProperties(New PropertyDescriptor(0 - 1) {})
                    If ((collection1 Is Nothing) OrElse (collection1.Count = 0)) Then
                        Throw New HttpException("The list source has no data members")
                    End If
                    Dim descriptor1 As PropertyDescriptor = Nothing
                    If ((dataMember Is Nothing) OrElse (dataMember.Length = 0)) Then
                        descriptor1 = collection1.Item(0)
                    Else
                        descriptor1 = collection1.Find(dataMember, True)
                    End If
                    If (Not descriptor1 Is Nothing) Then
                        Dim obj1 As Object = list1.Item(0)
                        Dim obj2 As Object = descriptor1.GetValue(obj1)
                        If ((Not obj2 Is Nothing) AndAlso TypeOf obj2 Is IEnumerable) Then
                            Return CType(obj2, IEnumerable)
                        End If
                    End If
                    Throw New HttpException("The list source is missing a data member")
                End If
            End If
            If TypeOf dataSource Is IEnumerable Then
                Return CType(dataSource, IEnumerable)
            End If
        End If
        Return Nothing
    End Function 'GetResolvedDataSource

End Class 'DataSourceHelper

Public Enum ListType
    ' Fields
    ListBox = 0
    DropDownList = 1
End Enum 'CListType
