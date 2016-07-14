Namespace SBSWebsite
    Partial Class SBS_Players_Odds
        Inherits System.Web.UI.Page

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'Page.Title = "Odds"
            'Dim menuOdds As HtmlAnchor = CType(Me.Master.FindControl("menuOdds"), HtmlAnchor)
            'menuOdds.Style.Add("color", "yellow")
            'Dim lblColor As Label = CType(Me.Master.FindControl("lblcolor"), Label)
            'If lblColor.Text.ToUpper = "GREEN" Then
            '    menuOdds.Style.Add("background-image", "/SBS/images/bg_active_green.jpg")
            'ElseIf lblColor.Text.ToUpper = "BLUE" Then
            '    menuOdds.Style.Add("background-image", "/SBS/images/menu_active_blue.png")
            'ElseIf lblColor.Text.ToUpper = "RED" Then
            '    menuOdds.Style.Add("background-image", "/SBS/images/menu_active_red.png")
            'ElseIf lblColor.Text.ToUpper = "SKY" Then
            '    menuOdds.Style.Add("background-image", "/SBS/images/menu_active_sky.png")
            'ElseIf lblColor.Text.ToUpper = "BLACK" Then
            '    menuOdds.Style.Add("background-image", "/SBS/images/menu_active_black.png")
            'End If
            'menuOdds.Style.Add("background-position", "top center")
            'menuOdds.Style.Add("background-repeat", "repeat-x")
            'menuOdds.Style.Add("height", "19px")
            'menuOdds.Style.Add("margin-top", "-3px")
            'menuOdds.Style.Add("margin-left", "-3px")
            'menuOdds.Style.Add("line-height", "17px")
            'menuOdds.Style.Add("border-left", "1px solid #FFF")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                ucContentFileDB.LoadFileDBContent("ODDS")
            End If
        End Sub
    End Class
End Namespace
