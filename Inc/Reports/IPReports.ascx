<%@ Control Language="VB" AutoEventWireup="false" CodeFile="IPReports.ascx.vb" Inherits="SBCWebsite.IPReports" %>

<div class="col-md-6 col-md-offset-3">
    <div class="panel panel-grey">
        <div class="panel-heading">IP Report</div>
        <div class="panel-body">
            <div class="form-group">
                <label class="col-md-3 control-label">Customer ID</label>
                <div class="col-md-4">
                    <asp:TextBox ID="txtCustomerID" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnIpSearch" Text="Ip Search" runat="server" CssClass="btn btn-success" />
                </div>
            </div>
            <div class="form-group">
                <label class="col-md-3 control-label">IPAddress</label>
                <div class="col-md-4">
                    <asp:TextBox ID="txtIpAddress" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnFindMatch" Text="Find Match" runat="server" CssClass="btn btn-warning" />
                </div>
            </div>
            <div class="form-group">
                <label class="col-md-3 control-label"></label>
                <div class="col-md-9">
                    (Searches are for last 14 days)
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-12">
                    <asp:Literal ID="ltlCustomer" runat="server"></asp:Literal>
                </div>
            </div>
            <asp:DataGrid ID="dgIPReport" runat="server"  AutoGenerateColumns="false"
                CssClass="table table-hover table-bordered">
                <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" Wrap="false" />
                <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
                <Columns>
                    <asp:TemplateColumn HeaderText="">
                        <ItemTemplate>
                            <asp:Label ID="lblIndex" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Users">
                        <ItemTemplate>
                            <asp:Label ID="lblLoginName" runat="server" Text='<%# Container.DataItem("LoginName")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Ip Addresses">
                        <ItemTemplate>
                            <asp:Label ID="lblIpAddress" runat="server" Text='<%# Container.DataItem("IpAddress")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Number Of Times Used" ItemStyle-HorizontalAlign="center">
                        <ItemTemplate>
                            <asp:Label ID="lblTimeUsed" runat="server" Text='<%# Container.DataItem("TimeUsed")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText=" Last Time Used" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:Label ID="lblLastTimeUsed" runat="server" Text='<%# Container.DataItem("LastTimeUsed")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
</div>
<div class="clearfix">
    
</div>

