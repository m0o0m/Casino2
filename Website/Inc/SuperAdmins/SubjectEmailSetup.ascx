<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SubjectEmailSetup.ascx.vb"
    Inherits="SBCSuperAdmin.SubjectEmailSetup" %>

<div class="row">
    <div class="col-lg-12">
        <strong>Email Subject</strong>
        <asp:TextBox ID="txtSubjectEmail" runat="server" CssClass="form-control" Style="display: inline-block;" MaxLength="300" Width="400"></asp:TextBox>
        <asp:Button ID="btnAddSubjectEmail" runat="server" Text="Add" CssClass="btn btn-primary" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<asp:DataGrid ID="dgSubjectEmail" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
    <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Left" Wrap="false" />
    <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
    <SelectedItemStyle BackColor="YellowGreen" />
    <Columns>
        <asp:TemplateColumn HeaderText="Email Subject" ItemStyle-Width="80%">
            <ItemTemplate>
                <asp:Label ID="lblSubject" Text='<%#DataBinder.Eval(Container.DataItem, "Subject") %>' runat="server"></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtSubject" Text='<%#Container.DataItem("Subject") %>' Width="100%" CssClass="textInput" MaxLength="300" runat="server"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateColumn>
    </Columns>
    <Columns>

        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:LinkButton ID="lbtEditSubjectEmail" runat="server" Text="Edit" CommandName="EDIT"
                    Font-Underline="false" ToolTip="Edit Email Subject"></asp:LinkButton>
                <asp:LinkButton ID="lbtCancelSubjectEmail" runat="server" Text="Cancel" CommandName="CANCEL"
                    Font-Underline="false" ToolTip="Cancel"></asp:LinkButton>
                <asp:LinkButton ID="lbtUpdateSubjectEmail" runat="server" CommandName="UPDATE" CommandArgument='<%# SBCBL.std.SafeString(DataBinder.Eval(Container.DataItem, "ID"))%>' Text="Update" Style="margin-left: 5px"
                    Font-Underline="false" ToolTip="Update Email Subject"></asp:LinkButton>
                <asp:LinkButton ID="lbtDeleteSubjectEmail" runat="server" OnClientClick=" return confirm('Are you sure delete this item?')" Style="margin-left: 5px"
                    CommandArgument='<%# SBCBL.std.SafeString(DataBinder.Eval(Container.DataItem, "ID")) %>'
                    CommandName="DELETE" ToolTip="Delete Email Subject" Text="Delete" Font-Underline="false" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>