Imports System.Data
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports WebsiteLibrary

Namespace SBCSuperAdmins
    Partial Class SysSettingManager
        Inherits SBCBL.UI.CSBCUserControl

        ''NOTE: not allow create/edit/delete category 
        ''dont remove this code, need only set invisisble webcontrol instead of create/edit/delete category

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                Me.bindCategories()

                loadRoundingOptions()
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ''not allow create new Category
            ''dont remove this code
            btnAddCategory.Visible = False
        End Sub

#End Region

#Region "Bind data"

        Private Sub bindCategories()
            Dim oCatgories As XElement = XElement.Load(Server.MapPath("~/Inc/SuperAdmins/SysSettingCategories.xml"))

            'rptCategories.DataSource = (New CSysSettingManager).GetCategories()
            rptCategories.DataSource = oCatgories.<Category>
            rptCategories.DataBind()
        End Sub

        Private Sub bindSubCategories(ByVal poSubCategory As CDropDownList, ByVal psCategory As String)
            poSubCategory.Items.Clear()
            poSubCategory.Items.Add(New ListItem("--- None ---", ""))

            Dim odtCategories As DataTable = (New CSysSettingManager).GetSubCategories(psCategory)

            For Each odrCategory As DataRow In odtCategories.Rows
                Dim sSubCategory As String = SafeString(odrCategory("SubCategory"))

                If sSubCategory <> "" Then
                    poSubCategory.Items.Add(New ListItem(sSubCategory, sSubCategory))
                End If
            Next

            poSubCategory.Items.Add(New ListItem("---- Add New ---", "__ADD_SUB_CATEGORY"))
        End Sub

        Private Function bindKeyValues(ByVal poKeyValues As Repeater, ByVal psCategory As String) As Integer
            If psCategory <> "" Then
                poKeyValues.DataSource = (New CSysSettingManager).GetSysSettings(psCategory)
            Else
                poKeyValues.DataSource = Nothing
            End If
            poKeyValues.DataBind()

            Return poKeyValues.Items.Count
        End Function

        Private Function bindKeyValues(ByVal poKeyValues As Repeater, ByVal psCategory As String, ByVal psSubCategory As String) As Integer
            poKeyValues.DataSource = (New CSysSettingManager).GetSysSettings(psCategory, psSubCategory)
            poKeyValues.DataBind()

            Return poKeyValues.Items.Count
        End Function

        Private Sub loadRoundingOptions()
            Dim odtCategories As DataTable = (New CSysSettingManager).GetSysSettings("ROUNDING OPTION")

            If odtCategories.Rows.Count > 0 Then
                Dim oli As ListItem = rblRoundingOption.Items.FindByValue(SafeString(odtCategories.Rows(0)("Value")))

                If oli IsNot Nothing Then
                    rblRoundingOption.ClearSelection()
                    oli.Selected = True

                    hfSettingID.Value = SafeString(odtCategories.Rows(0)("SysSettingID"))
                End If
            End If
        End Sub

#End Region

#Region "Category"

        Protected Sub SaveCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim sCategory As String = SafeString(txtCategory.Text)

            If sCategory = "" Then
                ClientAlert("Category Is Required", True)
                txtCategory.Focus() : Return
            End If
            If (New CSysSettingManager).IsExistedCategory(sCategory) Then
                ClientAlert("This Category '" & sCategory & "' Has Already Existed", True)
                txtCategory.Focus() : Return
            End If

            Dim sOldCategory As String = btnSaveCategory.CommandArgument

            If sOldCategory <> "" Then
                If Not (New CSysSettingManager).UpdateCategoryName(sOldCategory, sCategory) Then
                    ClientAlert("Failed to Save Setting", True)
                    Return
                End If
            Else
                If Not (New CSysSettingManager).AddSysSetting(sCategory, "", "") Then
                    ClientAlert("Unsuccessfully Added", True)
                    Return
                End If
            End If

            CloseFormEditCategory_Click(Nothing, Nothing)

            UserSession.Cache.ClearSysSettings(sOldCategory)
            UserSession.Cache.ClearSysSettings(sCategory)

            bindCategories()
        End Sub

        Protected Sub CloseFormEditCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            rptCategories.Visible = True
            btnAddCategory.Visible = True
            tblEditCategory.Visible = False
        End Sub

        Protected Sub OpenFormEditCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            tblEditCategory.Visible = True

            rptCategories.Visible = False
            btnAddCategory.Visible = False

            txtCategory.Text = ""
            btnSaveCategory.CommandArgument = ""
        End Sub

        Protected Sub rptCategories_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptCategories.ItemCommand
            Select Case e.CommandName
                Case "EDIT_SUB_CATEGORY"
                    e.Item.FindControl("tblEditSubCategory").Visible = True
                    e.Item.FindControl("ibtEditSubCategory").Visible = False
                    e.Item.FindControl("ibtDeleteSubCategory").Visible = False

                    Dim ddlSubCategory As CDropDownList = CType(e.Item.FindControl("ddlSubCategory"), CDropDownList)
                    ddlSubCategory.Visible = False

                    e.Item.FindControl("rptKeyValues").Visible = False
                    e.Item.FindControl("lbnAddKeyValue").Visible = False

                    CType(e.Item.FindControl("txtSubCategory"), TextBox).Text = ddlSubCategory.Value
                    CType(e.Item.FindControl("btnSaveSubCategory"), Button).CommandArgument = ddlSubCategory.Value

                Case "DELETE_SUB_CATEGORY"
                    Dim ddlSubCategory As CDropDownList = CType(e.Item.FindControl("ddlSubCategory"), CDropDownList)

                    If ddlSubCategory.Value = "" OrElse UCase(ddlSubCategory.Value) = "__ADD_SUB_CATEGORY" Then
                        ClientAlert("Unseccessfully Deleted", True)
                        Return
                    End If

                    Dim sSubCategory As String = ddlSubCategory.Value
                    Dim sCategory As String = CType(e.Item.FindControl("lblCategory"), Label).Text

                    If Not (New CSysSettingManager).DeleteSettingBySubCategory(sCategory, sSubCategory) Then
                        ClientAlert("Unseccessfully Deleted", True)
                        Return
                    End If

                    UserSession.Cache.ClearSysSettings(sCategory, sSubCategory)

                    bindSubCategories(ddlSubCategory, sCategory)
                    bindKeyValues(CType(e.Item.FindControl("rptKeyValues"), Repeater), sCategory)

                    e.Item.FindControl("ibtEditSubCategory").Visible = False
                    e.Item.FindControl("ibtDeleteSubCategory").Visible = False

                Case "ADD_KEY_VALUE"
                    CType(e.Item.FindControl("ddlSubCategory"), CDropDownList).Enabled = False
                    e.Item.FindControl("ibtEditSubCategory").Visible = False
                    e.Item.FindControl("ibtDeleteSubCategory").Visible = False

                    e.Item.FindControl("rptKeyValues").Visible = False
                    e.Item.FindControl("lbnAddKeyValue").Visible = False

                    Dim tblEditKeyValue As HtmlTable = CType(e.Item.FindControl("tblEditKeyValue"), HtmlTable)
                    tblEditKeyValue.Attributes("title") = "Add new Key and Value"
                    tblEditKeyValue.Visible = True

                    CType(e.Item.FindControl("txtNewKey"), TextBox).Focus()
                    CType(e.Item.FindControl("txtNewKey"), TextBox).Text = ""
                    CType(e.Item.FindControl("txtNewValue"), TextBox).Text = ""

                Case "EDIT_CATEGORY"
                    tblEditCategory.Visible = True
                    rptCategories.Visible = False
                    btnAddCategory.Visible = False

                    Dim sCategory As String = SafeString(CType(e.Item.FindControl("lblCategory"), Label).Text)

                    txtCategory.Text = sCategory
                    btnSaveCategory.CommandArgument = sCategory

                Case "DELETE_CATEGORY"
                    Dim sCategory As String = CType(e.Item.FindControl("lblCategory"), Label).Text

                    If Not (New CSysSettingManager).DeleteSettingByCategory(sCategory) Then
                        ClientAlert("Unseccessfully Deleted", True)
                        Return
                    End If

                    UserSession.Cache.ClearSysSettings(sCategory)
                    bindCategories()
            End Select
        End Sub

        Protected Sub rptCategories_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptCategories.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim sCategory As String = CType(e.Item.DataItem, XElement).Value

                CType(e.Item.FindControl("lblCategory"), Label).Text = sCategory

                sCategory = GetCategory(sCategory)

                bindSubCategories(CType(e.Item.FindControl("ddlSubCategory"), CDropDownList), sCategory)
                bindKeyValues(CType(e.Item.FindControl("rptKeyValues"), Repeater), sCategory)

                ''not allow edit/delete Category
                ''dont remove this code
                CType(e.Item.FindControl("ibtEditCategory"), ImageButton).Visible = False
                CType(e.Item.FindControl("ibtDeleteCategory"), ImageButton).Visible = False
            End If
        End Sub

#End Region

#Region "Sub Category"

        Protected Sub ddlSubCategory_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim ddlSubCategory As CDropDownList = CType(sender, CDropDownList)
            Dim oriCategories As RepeaterItem = CType(ddlSubCategory.NamingContainer, RepeaterItem)
            Dim rptKeyValues As Repeater = CType(oriCategories.FindControl("rptKeyValues"), Repeater)

            Dim sSubCategory As String = ddlSubCategory.Value
            Dim bIsSubKeyValueBind As Boolean = True

            If UCase(sSubCategory) = "__ADD_SUB_CATEGORY" Then
                bIsSubKeyValueBind = False
                oriCategories.FindControl("tblEditSubCategory").Visible = True

                ddlSubCategory.Visible = False
                rptKeyValues.Visible = False

                oriCategories.FindControl("lbnAddKeyValue").Visible = False

                CType(oriCategories.FindControl("btnSaveSubCategory"), Button).CommandArgument = ""
                CType(oriCategories.FindControl("txtSubCategory"), TextBox).Text = ""

            End If

            'Set Button
            oriCategories.FindControl("ibtEditSubCategory").Visible = False
            oriCategories.FindControl("ibtDeleteSubCategory").Visible = False

            'load key, value of category OR sub category 
            If bIsSubKeyValueBind Then
                Dim sCategory As String = CType(oriCategories.FindControl("lblCategory"), Label).Text
                bindKeyValues(rptKeyValues, sCategory, sSubCategory)

                oriCategories.FindControl("ibtEditSubCategory").Visible = sSubCategory <> ""
                oriCategories.FindControl("ibtDeleteSubCategory").Visible = sSubCategory <> ""
            End If
        End Sub

        Protected Sub SaveSubCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim oriCategories As RepeaterItem = CType(CType(sender, Control).NamingContainer, RepeaterItem)
            Dim txtSubCategory As TextBox = CType(oriCategories.FindControl("txtSubCategory"), TextBox)

            If SafeString(txtSubCategory.Text) = "" Then
                ClientAlert("Sub Category Is Required", True)
                txtSubCategory.Focus() : Return
            End If

            Dim sCategory As String = CType(oriCategories.FindControl("lblCategory"), Label).Text
            Dim sSubCategory As String = SafeString(txtSubCategory.Text)
            Dim sOldSubCategory As String = SafeString(CType(sender, Button).CommandArgument)

            If (New CSysSettingManager).IsExistedSubCategory(sCategory, sSubCategory, sOldSubCategory) Then
                ClientAlert("This Sub Category '" & sSubCategory & "' Has Already Existed", True)
                txtSubCategory.Focus() : Return
            End If

            If sOldSubCategory <> "" Then
                If Not (New CSysSettingManager).UpdateSubCategoryName(sCategory, sOldSubCategory, sSubCategory) Then
                    ClientAlert("Unseccessfully Updated", True)
                    Return
                Else
                    UserSession.Cache.ClearSysSettings(sCategory, sOldSubCategory)
                End If
            Else
                If Not (New CSysSettingManager).AddSubCategory(sCategory, sSubCategory) Then
                    ClientAlert("Unsuccessfully Added New", True)
                    Return
                End If

                bindKeyValues(CType(oriCategories.FindControl("rptKeyValues"), Repeater), "")
            End If

            CloseFormEditSubCategory_Click(sender, Nothing)

            Dim ddlSubCategory As CDropDownList = CType(oriCategories.FindControl("ddlSubCategory"), CDropDownList)
            bindSubCategories(ddlSubCategory, sCategory)
            ddlSubCategory.Value = sSubCategory
        End Sub

        Protected Sub CloseFormEditSubCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim oriCategories As RepeaterItem = CType(CType(sender, Button).NamingContainer, RepeaterItem)

            Dim oddlSubCategory As CDropDownList = CType(oriCategories.FindControl("ddlSubCategory"), CDropDownList)
            oddlSubCategory.Visible = True
            oddlSubCategory.Enabled = True
            oddlSubCategory.Value = ""

            oriCategories.FindControl("rptKeyValues").Visible = True
            oriCategories.FindControl("lbnAddKeyValue").Visible = True
            oriCategories.FindControl("tblEditSubCategory").Visible = False
        End Sub

#End Region

#Region "Key & Value"

        Protected Sub OpenFormEditKeyValue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim oriKeyValues As RepeaterItem = CType(CType(sender, Control).NamingContainer, RepeaterItem)
            Dim oriCatgories As RepeaterItem = CType(oriKeyValues.Parent.NamingContainer, RepeaterItem)

            CType(oriCatgories.FindControl("ddlSubCategory"), CDropDownList).Enabled = False
            oriCatgories.FindControl("ibtEditSubCategory").Visible = False
            oriCatgories.FindControl("ibtDeleteSubCategory").Visible = False
            oriCatgories.FindControl("rptKeyValues").Visible = False
            oriCatgories.FindControl("lbnAddKeyValue").Visible = False

            Dim tblEditKeyValue As HtmlTable = CType(oriCatgories.FindControl("tblEditKeyValue"), HtmlTable)
            tblEditKeyValue.Attributes("title") = "Edit Category"
            tblEditKeyValue.Visible = True

            CType(oriCatgories.FindControl("txtNewKey"), TextBox).Text = CType(oriKeyValues.FindControl("lblKey"), Label).Text
            CType(oriCatgories.FindControl("txtNewValue"), TextBox).Text = CType(oriKeyValues.FindControl("lblValue"), Label).Text
            CType(oriCatgories.FindControl("hfSettingID"), HiddenField).Value = CType(oriKeyValues.FindControl("ibtEditKeyValue"), ImageButton).CommandArgument
        End Sub

        Protected Sub SaveKeyValue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim oriCategories As RepeaterItem = CType(CType(sender, Control).NamingContainer, RepeaterItem)

            Dim sCategory As String = GetCategory(CType(oriCategories.FindControl("lblCategory"), Label).Text)
            Dim sSubCategory As String = CType(oriCategories.FindControl("ddlSubCategory"), CDropDownList).Value

            If UCase(sSubCategory) = "__ADD_SUB_CATEGORY" Then
                sSubCategory = ""
            End If

            Dim hfSettingID As HiddenField = CType(oriCategories.FindControl("hfSettingID"), HiddenField)
            Dim txtKey As TextBox = CType(oriCategories.FindControl("txtNewKey"), TextBox)
            Dim txtValue As TextBox = CType(oriCategories.FindControl("txtNewValue"), TextBox)

            ''check 
            If SafeString(txtKey.Text) = "" Then
                ClientAlert("Key Is Required", True)
                txtKey.Focus() : Return
            End If

            If (New CSysSettingManager).IsExistedKey(sCategory, sSubCategory, SafeString(txtKey.Text), hfSettingID.Value) Then
                ClientAlert("This Key '" & SafeString(txtKey.Text) & "' Has Alreaddy Existed", True)
                txtKey.Focus() : Return
            End If
            If SafeString(txtValue.Text) = "" Then
                ClientAlert("Value Is Required", True)
                txtValue.Focus() : Return
            End If

            ''save
            If hfSettingID.Value = "" Then
                If Not (New CSysSettingManager).AddSysSetting(sCategory, SafeString(txtKey.Text), SafeString(txtValue.Text), sSubCategory) Then
                    ClientAlert("Unsuccessfully Added", True)
                    Return
                End If
            Else
                If Not (New CSysSettingManager).UpdateKeyValue(hfSettingID.Value, SafeString(txtKey.Text), SafeString(txtValue.Text)) Then
                    ClientAlert("Failed to Save Setting", True)
                    Return
                End If
            End If

            ''clear data
            hfSettingID.Value = ""
            txtKey.Text = ""
            txtValue.Text = ""

            closeFormEditKeyValue(CType(sender, Control))

            ''reload data and clear cache
            Dim rptKeyValues As Repeater = CType(oriCategories.FindControl("rptKeyValues"), Repeater)

            If sSubCategory = "" Then
                UserSession.Cache.ClearSysSettings(sCategory)
                bindKeyValues(rptKeyValues, sCategory)
            Else
                UserSession.Cache.ClearSysSettings(sCategory, sSubCategory)
                bindKeyValues(rptKeyValues, sCategory, sSubCategory)
            End If
        End Sub

        Protected Sub DeleteKeyValue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim ibtDeleteKeyValue As ImageButton = CType(sender, ImageButton)

            If Not (New CSysSettingManager).DeleteSetting(SafeString(ibtDeleteKeyValue.CommandArgument)) Then
                ClientAlert("Unsuccessfully Deleted", True)
                Return
            End If

            Dim rptKeyValues As Repeater = CType(ibtDeleteKeyValue.Parent.Parent, Repeater)
            Dim oriCategories As RepeaterItem = CType(rptKeyValues.NamingContainer, RepeaterItem)

            ''reload data and clear cache
            Dim sCategory As String = GetCategory(CType(oriCategories.FindControl("lblCategory"), Label).Text)
            Dim sSubCategory As String = CType(oriCategories.FindControl("ddlSubCategory"), CDropDownList).Value

            If UCase(sSubCategory) = "__ADD_SUB_CATEGORY" Then
                sSubCategory = ""
            End If

            If sSubCategory = "" Then
                UserSession.Cache.ClearSysSettings(sCategory)
                bindKeyValues(rptKeyValues, sCategory)
            Else
                UserSession.Cache.ClearSysSettings(sCategory, sSubCategory)
                bindKeyValues(rptKeyValues, sCategory, sSubCategory)
            End If
        End Sub

        Protected Sub CloseFormEditKeyValue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            closeFormEditKeyValue(CType(sender, Control))
        End Sub

        Private Sub closeFormEditKeyValue(ByVal oButton As Control)
            Dim oriCategories As RepeaterItem = CType(oButton.NamingContainer, RepeaterItem)
            Dim ddlSubCategory As CDropDownList = CType(oriCategories.FindControl("ddlSubCategory"), CDropDownList)

            ''get sub category
            Dim sSubCategory As String = ddlSubCategory.Value

            If sSubCategory = "__ADD_SUB_CATEGORY" Then
                sSubCategory = ""
            End If

            ''reset control
            ddlSubCategory.Enabled = True
            oriCategories.FindControl("ibtEditSubCategory").Visible = sSubCategory <> ""
            oriCategories.FindControl("ibtDeleteSubCategory").Visible = sSubCategory <> ""
            oriCategories.FindControl("tblEditKeyValue").Visible = False
            oriCategories.FindControl("rptKeyValues").Visible = True
            oriCategories.FindControl("lbnAddKeyValue").Visible = True
        End Sub

        Protected Sub rptKeyValues_ItemDataBound(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                e.Item.Visible = CType(e.Item.FindControl("lblKey"), Label).Text <> ""
            End If
        End Sub

#End Region

        Protected Sub rbtlRoundingOption_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblRoundingOption.SelectedIndexChanged
            If hfSettingID.Value = "" Then
                If Not (New CSysSettingManager).AddSysSetting("ROUNDING OPTION", "Rounding", rblRoundingOption.SelectedValue) Then
                    ClientAlert("Unsuccessfully Added ", True)
                    Return
                End If

                loadRoundingOptions()
            Else
                If Not (New CSysSettingManager).UpdateKeyValue(hfSettingID.Value, "Rounding", rblRoundingOption.SelectedValue) Then
                    ClientAlert("Failed to Save Setting", True)
                    Return
                End If
            End If

            UserSession.Cache.ClearSysSettings("ROUNDING OPTION")
        End Sub

        Private Sub SaveSettings(ByVal psCategory As String, ByVal psKey As String, ByVal psValue As String)
            Dim oSysManager As New CSysSettingManager()
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(psCategory)
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = psKey)

            Dim bExist As Boolean = oSysManager.IsExistedKey(psCategory, "", psKey)
            If oSetting Is Nothing Then
                oSysManager.AddSysSetting(psCategory, psKey, psValue)
            Else
                oSysManager.UpdateKeyValue(oSetting.SysSettingID, psKey, psValue)
            End If
            UserSession.Cache.ClearSysSettings(psCategory)
        End Sub

        Public Function GetCategory(ByVal psCategory As String) As String
            Select Case psCategory
                Case "Bookmaker"
                    Return SBCBL.std.GetSiteType() & " " & psCategory
                Case Else
                    Return psCategory
            End Select
        End Function
    End Class

End Namespace
