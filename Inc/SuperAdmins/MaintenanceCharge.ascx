<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MaintenanceCharge.ascx.vb"
    Inherits="SBCSuperAdmin.MaintenanceCharge" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="panel panel-grey">
    <div class="panel-heading"></div>
    <div class="panel-body">

        <div class="col-lg-12">
            <div class="form-group">
                <label class="col-lg-2 control-label">Agent</label>
                <wlb:CDropDownList ID="ddlPAgents" runat="server" CssClass="form-control" hasOptionalItem="true" OptionalText="--- All Agent ---" AutoPostBack="true" Style="display: inline-block;" Width="210px" />
            </div>
        </div>

        <div class="col-lg-12">
            <div class="form-group">
                <label class="col-lg-2 control-label">Weekly Charge</label>
                <asp:TextBox ID="txtWeeklyCharge" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this, event, false);"
                    MaxLength="5" Style="display: inline-block;" Width="210px"></asp:TextBox>
                &nbsp;&nbsp;
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-sm btn-primary" />
            </div>
        </div>

        <div class="col-lg-12">
            <div class="form-group">
                <label class="col-lg-2 control-label">Week Of</label>
                <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="form-control" hasOptionalItem="false"
                    AutoPostBack="true" Style="display: inline-block;" Width="230px" />
                &nbsp;&nbsp;
                <asp:Button ID="btnProcess" runat="server" Text="Process" CssClass="btn btn-primary" Visible="false" />
            </div>
        </div>
        <div class="mbxl"></div>
        <div class="row">
            <div class="col-lg-12">
                <asp:DataGrid ID="dgWeeklyMaitenance" runat="server" AutoGenerateColumns="false"
                    CssClass="table table-hover table-bordered">
                    <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                    <AlternatingItemStyle HorizontalAlign="Left" />
                    <SelectedItemStyle BackColor="YellowGreen" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="Agent Login (Name)" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <%#Container.DataItem("FullName")%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Total Active Accounts" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnDetails" runat="server" CommandName="DETAILS" ToolTip="View Details"></asp:LinkButton>
                                <asp:Label ID="lblDetails" runat="server" Visible="false" Text='<%#Container.DataItem("Description")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Total Weekly Maintenance" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:Label ID="lblWeeklyMaintenance" runat="server"></asp:Label>
                                <asp:Label ID="lblTotalNotPaid" runat="server" Visible="False"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Charge Date" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#Container.DataItem("ChargeDate")%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Is Paid" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Literal ID="Literal1" runat="server" Text='<%# If(Container.DataItem("PaidDate") IsNot DBNull.Value, "Yes", "No")%>' Visible='<%# Container.DataItem("WeeklyChargeID") IsNot DBNull.Value%>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Paid Date" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Literal ID="Literal2" runat="server" Text='<%#UserSession.ConvertToLocalTime(CSBCStd.SafeString(Container.DataItem("PaidDate")))%>' Visible='<%# Container.DataItem("PaidDate") IsNot DBNull.Value%>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnSetPaid" runat="server" Text="Set Paid" CssClass="btn-xs btn-blue" CommandName="SETPAID" Visible='<%# (Container.DataItem("WeeklyChargeID") IsNot DBNull.Value AndAlso Container.DataItem("PaidDate") Is DBNull.Value)%>'
                                    CommandArgument='<%# Container.DataItem("WeeklyChargeID")%>' />
                                <wlb:CButtonConfirmer ID="CButtonConfirmer1" runat="server" AttachTo="btnSetPaid"
                                    ConfirmExpression="confirm('Are you sure to set Paid for this charge?')">
                                </wlb:CButtonConfirmer>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
            <div class="clearfix"></div>
        </div>

    </div>
</div>
