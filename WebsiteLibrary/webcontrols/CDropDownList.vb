Imports System.ComponentModel.Design
Imports System.Xml
Imports System.Data.OleDb
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.ComponentModel
Imports WebsiteLibrary.CSBCStd
Imports System.Collections

Public Class CDropDownList
    Inherits System.Web.UI.WebControls.DropDownList

    Public Property Editable() As Boolean
        Get
            If IsNothing(ViewState("Editable")) Then
                ViewState("Editable") = False
            End If
            Return CBool(ViewState("Editable"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("Editable") = Value
        End Set
    End Property

    Public Property EditableText() As String
        Get
            If IsNothing(ViewState("EditableText")) Then
                Return ""
            End If

            Return CStr(ViewState("EditableText"))
        End Get
        Set(ByVal Value As String)
            ViewState("EditableText") = Value
        End Set
    End Property

    Public Property EditableIndex() As Integer
        Get
            If IsNothing(ViewState("EditableIndex")) Then
                Return 0
            End If

            Return safeInteger(ViewState("EditableIndex"))
        End Get
        Set(ByVal Value As Integer)
            ViewState("EditableIndex") = Value
        End Set
    End Property

    Public Property CaseSensitive() As Boolean
        Get
            If IsNothing(ViewState("CaseSensitive")) Then
                Return False
            End If
            Return CBool(ViewState("CaseSensitive"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("CaseSensitive") = Value
        End Set
    End Property

    <DefaultValue(GetType(Boolean), "True"), Description("Sorts the list items DSLd on the text displayed")> _
    Public Property SortItems() As Boolean
        Get
            If IsNothing(ViewState("SortItems")) Then
                Return True
            End If
            Return CBool(ViewState("SortItems"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("SortItems") = Value
        End Set
    End Property


    'default value is True because it takes less work to create a custom optional field this way
    ' - all you need to do is set the optional text/value property
    'If default value is False, you'd have to set it to true AND change the optional text/value property
    'Reversely, to make an drop down as non optional, just requires 1 straight forward property set
    <DefaultValue(GetType(Boolean), "True"), Description("Set to false to remove ANY optional item")> _
    Public Property hasOptionalItem() As Boolean
        Get
            If IsNothing(ViewState("hasOptionalItem")) Then
                Return True
            End If
            Return CBool(ViewState("hasOptionalItem"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("hasOptionalItem") = Value
        End Set
    End Property

    Public Property OptionalText() As String
        Get
            If IsNothing(ViewState("OptionalText")) Then
                Return ""
            End If

            Return CStr(ViewState("OptionalText"))
        End Get
        Set(ByVal Value As String)
            ViewState("OptionalText") = Value
        End Set
    End Property

    Public Property OptionalValue() As String
        Get
            If IsNothing(ViewState("OptionalValue")) Then
                Return ""
            End If
            Return CStr(ViewState("OptionalValue"))
        End Get
        Set(ByVal Value As String)
            ViewState("OptionalValue") = Value
        End Set
    End Property

    Public Property Value() As String
        Get
            If Me.SelectedIndex = -1 Then
                Return ""
            Else
                Return Me.Items(SelectedIndex).Value
            End If
        End Get
        Set(ByVal Value As String)
            Me.SelectedIndex = getItemIndexByVal(Me, Value, CaseSensitive)
        End Set
    End Property

    Private Sub CEnumDropDownList_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
        '        traceA("EnumDrop init")
        If Editable Then
            Me.Attributes("onKeyDown") = "if(window.event.keyCode == 8 || window.event.keyCode==127){window.event.keyCode = '';return true;}window.event.keyCode = '';return true;"
            Me.Attributes("onKeyUp") = "return false"
            Me.Attributes("onKeyPress") = "fnKeyPressHandler(this, " & EditableIndex & ")"
            Me.Attributes("onChange") = "fnChangeHandler(this, " & EditableIndex & ")"

            If Me.Items(EditableIndex).Attributes("id") <> "EditMe" Then
                Dim litem As New ListItem()
                litem.Text = EditableText
                litem.Attributes("id") = "EditMe"
                litem.Attributes("name") = "EditMe"
                Me.Items.Insert(EditableIndex, litem)
            End If
        End If

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        If Editable Then
            writer.Write("<script type=""text/javascript"" src=""/inc/CommonControls/CEditableDropDown.js""></script>")
            Dim strWriter As New System.IO.StringWriter()
            Dim htmlWriter As New HtmlTextWriter(strWriter)
            MyBase.Render(htmlWriter)

            Dim finalcontent As String = strWriter.ToString
            finalcontent = finalcontent.Replace("<option value=""" & EditableText & """>" & EditableText & "</option>", "<option id=""EditMe"" name=""EditMe"" value=""" & EditableText & """>" & EditableText & "</option>")

            writer.Write(finalcontent)
        Else
            MyBase.Render(writer)
        End If
    End Sub

    Public Overrides Sub DataBind()
        'only clear out the items if they're bound through EnumVariable or DataSource
        If (Not Me.DataSource Is Nothing) Then
            Me.Items.Clear()
        End If
        MyBase.DataBind()

        If hasOptionalItem Then
            Me.Items.Insert(0, New ListItem(Me.OptionalText, Me.OptionalValue))
        End If
    End Sub

    Public Shared Function getItemIndexByVal(ByVal list As ListControl, ByVal sVal As String, ByVal bCaseSensitive As Boolean) As Integer
        Dim li As ListItem
        Dim i As Integer
        If IsNothing(sVal) Then
            sVal = ""
        End If

        If bCaseSensitive Then
            For i = 0 To list.Items.Count - 1
                li = list.Items(i)
                If li.Value = sVal Then
                    Return i
                End If
            Next
        Else
            For i = 0 To list.Items.Count - 1
                li = list.Items(i)
                If SafeString(li.Value).ToUpper = SafeString(sVal).ToUpper Then
                    Return i
                End If
            Next
        End If

        Return -1
    End Function


    'retrieve the website folder -- this is done by obitaining the websitelibrary path , 
    'backtrack 3 folders, then forward track 1 folder to WEBSITE -- THIS WILL NEED TO CHANGE ON PROJECT BY PROJECT BASIS
    Friend Shared Function getWebsiteRoot(ByVal context As ITypeDescriptorContext) As ArrayList
        Dim sWebsiteRoot As String = ""
        Dim WebsiteRoots As New ArrayList()
        Try
            Dim service As ITypeResolutionService = CType(context.GetService(GetType(ITypeResolutionService)), ITypeResolutionService)
            Dim asm As String = service.GetPathOfAssembly(context.Instance.GetType.Assembly.GetName())
            Dim path As String = System.IO.Path.GetDirectoryName(asm)
            Dim i As Integer, arrDirPaths() As String
            arrDirPaths = path.Split("\"c)
            For i = 0 To arrDirPaths.Length - 4
                sWebsiteRoot += arrDirPaths(i) & "\"
            Next

            Dim currentDir As String
            For Each currentDir In System.IO.Directory.GetDirectories(sWebsiteRoot)
                WebsiteRoots.Add(currentDir & "\")
            Next

            If System.IO.Directory.Exists(sWebsiteRoot) = False Then
                Throw New Exception("Source code is pointing to non-existent website folder:" & sWebsiteRoot)
            End If
        Catch ex As Exception
            CSBCStd.pbTrace("ERR", "Unable to get websiteRoot:" & ex.ToString)
            Return Nothing
        End Try

        Return WebsiteRoots
    End Function

End Class


Public Class CEnumDropDownListConverter
    Inherits StringConverter

    Public Overloads Overrides Function GetStandardValuesSupported(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

    Public Overloads Overrides Function GetStandardValuesExclusive(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        'return false here means the proeprty will have a drop down and a value that can be manaully entered
        Return False
    End Function

    Public Overloads Overrides Function GetStandardValues(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.ComponentModel.TypeConverter.StandardValuesCollection
        Dim sWebsiteRoot As ArrayList = CDropDownList.getWebsiteRoot(context)
        If sWebsiteRoot.Count = 0 Or sWebsiteRoot Is Nothing Then
            Return Nothing
        End If

        Dim scurrWebsiteRoot As String
        Dim oEnum As XmlNode
        Dim oEnumEl As XmlElement
        Dim idList As New ArrayList()

        For Each scurrWebsiteRoot In sWebsiteRoot
            Try
                Dim sXMLFile As String = scurrWebsiteRoot & "Web.config"
                'std.pbTrace("VER", "xml file:" & sXMLFile)
                Dim doc As New System.Xml.XmlDocument()
                doc.Load(sXMLFile)

                Dim oEnums As XmlNodeList = doc.SelectSingleNode("//enumVariables").ChildNodes

                For Each oEnum In oEnums
                    Try
                        oEnumEl = CType(oEnum, XmlElement)
                        If Not idList.Contains(oEnumEl.Name) Then
                            idList.Add(oEnumEl.Name)
                        End If
                    Catch e As System.InvalidCastException
                        'just means we found a Comment node
                    End Try
                Next
            Catch ex As Exception
                'std.pbTrace("ERR", "Unable to load xml file. Ex:" & ex.ToString)
            End Try
        Next

        idList.Sort()
        Dim oValues As New StandardValuesCollection(idList.ToArray)
        Return oValues
    End Function
End Class
