<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VolumnReport.ascx.vb" Inherits="SBS_Inc_VolumnReport" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>

<div class="row">
    <div class="col-lg-12">
        <div style="display: inline-block;" id="agentFilter" runat="server">
            <span>Agent</span>
            &nbsp; 
            <wlb:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control" hasOptionalItem="false"
                AutoPostBack="true" Width="230px" Style="display: inline-block;" />
        </div>
        &nbsp; &nbsp; 
        <span>Weeks</span>
        &nbsp; 
        <wlb:CDropDownList ID="ddlWeeks" runat="server" CssClass="form-control" hasOptionalItem="false"
            AutoPostBack="true" Width="230px" Style="display: inline-block;" />
    </div>
</div>
<div class="mbxl"></div>



<div class="row">
    <div class="col-lg-12">
        <div class="panel panel-grey">
            <div class="panel-heading">Report By Agent</div>
            <div class="panel-body">

                <asp:Repeater ID="rptMain" runat="server">
                    <ItemTemplate>
                        <h2><asp:Literal ID="lblAgentWeekly" runat="server" Text=""></asp:Literal></h2>

                        <asp:DataGrid ID="dgSubPlayers" runat="server" AutoGenerateColumns="false"
                            OnItemDataBound="dgSubPlayers_ItemDataBound" CssClass="table table-hover table-bordered">
                            <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateColumn HeaderText="Player" ItemStyle-Width="230">
                                    <ItemTemplate>
                                        <%# Container.DataItem("PlayerName")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='FootBall'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("FootBall")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='BasketBall'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("BasketBall")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='BaseBall'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("BaseBall")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='Hockey'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("Hockey")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='Soccer'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("Soccer")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='Other'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("Other")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='Par,Teas,Rev,Pro'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("FiveBetTotal")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                
                                <asp:TemplateColumn HeaderText='Total'>
                                    <ItemTemplate>
                                        <%# Container.DataItem("Total")%>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                

                            </Columns>
                        </asp:DataGrid>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>
    </div>
</div>
<div class="mbxl"></div>
