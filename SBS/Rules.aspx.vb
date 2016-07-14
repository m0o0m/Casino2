Namespace SBSWebsite
    Partial Class Rules
        Inherits System.Web.UI.Page

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Page.Title = "Rules"
            'Dim menuRules As HtmlAnchor = CType(Me.Master.FindControl("menuRules"), HtmlAnchor)
            'menuRules.Style.Add("color", "yellow")
            'Dim lblColor As Label = CType(Me.Master.FindControl("lblcolor"), Label)
            'If lblColor.Text.ToUpper = "GREEN" Then
            '    menuRules.Style.Add("background-image", "/SBS/images/bg_active_green.jpg")
            'ElseIf lblColor.Text.ToUpper = "BLUE" Then
            '    menuRules.Style.Add("background-image", "/SBS/images/menu_active_blue.png")
            'ElseIf lblColor.Text.ToUpper = "RED" Then
            '    menuRules.Style.Add("background-image", "/SBS/images/menu_active_red.png")
            'ElseIf lblColor.Text.ToUpper = "SKY" Then
            '    menuRules.Style.Add("background-image", "/SBS/images/menu_active_sky.png")
            'ElseIf lblColor.Text.ToUpper = "BLACK" Then
            '    menuRules.Style.Add("background-image", "/SBS/images/menu_active_black.png")
            'End If
            'menuRules.Style.Add("background-position", "top center")
            'menuRules.Style.Add("background-repeat", "repeat-x")
            'menuRules.Style.Add("height", "19px")
            'menuRules.Style.Add("margin-top", "-3px")
            'menuRules.Style.Add("margin-left", "-3px")
            'menuRules.Style.Add("line-height", "17px")
            'menuRules.Style.Add("border-left", "1px solid #FFF")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                ucContentFileDB.LoadFileDBContent("RULES")
            End If
        End Sub

    End Class
End Namespace