Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCWebsite
    Partial Class IPReports
        Inherits SBCBL.UI.CSBCUserControl
        Private nRowIndex As Integer = 1
        Private ReadOnly lastNumDay As Integer = -14
        Private ReadOnly resultsForCustomer As String = "Results for Customer: "

#Region "Search"
        Protected Sub IPSearch(ByVal psLoginName As String)
            Dim oIPTracesManager As New CIPTracesManager()
            If psLoginName <> "" AndAlso psLoginName IsNot Nothing Then
                dgIPReport.DataSource = oIPTracesManager.GetIPTraces(psLoginName, SBCBL.std.GetSiteType, "", lastNumDay)
                dgIPReport.DataBind()
            End If


        End Sub

        Protected Sub FindMatch(ByVal psLoginName As String, ByVal psIpAddress As String)
            Dim oIPTracesManager As New CIPTracesManager()
            If psIpAddress <> "" AndAlso psIpAddress IsNot Nothing Then
                dgIPReport.DataSource = oIPTracesManager.GetIPTraces(psLoginName, SBCBL.std.GetSiteType, psIpAddress, lastNumDay)
                dgIPReport.DataBind()

            End If


        End Sub
#End Region

        Protected Sub btnIpSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnIpSearch.Click
            If txtCustomerID.Text <> "" Then
                ltlCustomer.Text = resultsForCustomer & txtCustomerID.Text
                IPSearch(txtCustomerID.Text)
                dgIPReport.Columns(1).Visible = False
            Else
                ClientAlert("Please Input Customer ID", False)
                txtCustomerID.Focus()
            End If

        End Sub

        Protected Sub btnFindMatch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFindMatch.Click
            If txtIpAddress.Text <> "" Then
                FindMatch(txtCustomerID.Text, txtIpAddress.Text)
                dgIPReport.Columns(1).Visible = SafeString(txtCustomerID.Text) = ""

            Else
                ClientAlert("Please Input IP Address", False)
                txtIpAddress.Focus()
                Return
            End If
            If txtCustomerID.Text <> "" Then
                ltlCustomer.Text = resultsForCustomer & txtCustomerID.Text
            Else
                ltlCustomer.Text = ""
            End If
        End Sub

        Protected Sub dgIPReport_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgIPReport.ItemDataBound

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim lblIndex As Label = CType(e.Item.FindControl("lblIndex"), Label)
                lblIndex.Text = nRowIndex
                nRowIndex += 1
                Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)
                Dim lblLastTimeUsed As Label = CType(e.Item.FindControl("lblLastTimeUsed"), Label)
                lblLastTimeUsed.Text = UserSession.ConvertToEST(SafeString(oData("LastTimeUsed"))).ToString()

            End If

        End Sub
    End Class
End Namespace