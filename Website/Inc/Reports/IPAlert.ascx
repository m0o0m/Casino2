<%@ Control Language="VB" AutoEventWireup="false" CodeFile="IPAlert.ascx.vb" Inherits="SBCWebsite.IPAlert" %>
<%@ Register Namespace="WebsiteLibrary" Assembly="WebsiteLibrary" TagPrefix="wlb" %>
<div id="divSearch" class="col-lg-12">
    <span><%=Me.AgentLabel%></span>
    <wlb:CDropDownList runat="server" ID="ddlAgent" CssClass="form-control" Style="display: inline-block;" Width="230px" />
    <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="btn btn-primary" />
    <asp:Label runat="server" ID="lblMessage" Style="color: Red" />
    <span>(Searches are for last 14 days)</span>
</div>
<div class="mbxl"></div>

<asp:DataGrid ID="dgIPAlert" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
    <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Left" Wrap="false" />
    <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" />
    <Columns>
        <asp:TemplateColumn HeaderText="Agent">
            <ItemTemplate>
                <asp:LinkButton runat="server" ID="lbnAgentName" Text='<%# Container.DataItem("AgentName")%>'
                    CommandName="ViewIPListByAgent" CommandArgument='<%# Container.DataItem("LoginName")%>'
                    Style="text-decoration: none" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
    <Columns>
        <asp:TemplateColumn HeaderText="Player">
            <ItemTemplate>
                <asp:LinkButton runat="server" ID="lbnPlayer" Text='<%# Container.DataItem("Player")%>'
                    CommandName="ViewIPListByPlayer" CommandArgument='<%# Container.DataItem("LoginName")%>'
                    Style="text-decoration: none" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
    <Columns>
        <asp:TemplateColumn HeaderText="IP">
            <ItemTemplate>
                <asp:LinkButton runat="server" ID="lbnIP" Text='<%# Container.DataItem("IP")%>' CommandName="ViewUserLoginList"
                    CommandArgument='<%# Container.DataItem("IP")%>' Style="text-decoration: none;" />
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
    <Columns>
        <asp:BoundColumn DataField="TraceDate" HeaderText="Last Login"></asp:BoundColumn>
    </Columns>
</asp:DataGrid>

<div style="display: none" id="divIPListLoginByAgent">
    <asp:DataGrid ID="dgIPListLoginByAgent" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
        <HeaderStyle HorizontalAlign="Center" CssClass="active" />
        <ItemStyle HorizontalAlign="Left" Wrap="false" CssClass="warning" />
        <AlternatingItemStyle HorizontalAlign="Left" Wrap="false" CssClass="success" />
        <Columns>
            <asp:TemplateColumn HeaderText="IP">
                <ItemTemplate>
                    <%#Eval("IP")%>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Login Date">
                <ItemTemplate>
                    <%#Eval("LastLogin")%>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Numbers Of Login" ItemStyle-HorizontalAlign="Right">
                <ItemTemplate>
                    <%#Eval("NumLogin")%>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</div>
<div style="display: none" id="divUserLoginList">
    <asp:DataGrid ID="dgUserLoginList" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
        <HeaderStyle HorizontalAlign="Center" CssClass="active" />
        <ItemStyle HorizontalAlign="Left" Wrap="false" CssClass="warning" />
        <AlternatingItemStyle HorizontalAlign="Left" Wrap="false"  CssClass="success" />
        <Columns>
            <asp:TemplateColumn HeaderText="User">
                <ItemTemplate>
                    <%#Eval("UserLogin")%>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Login Date">
                <ItemTemplate>
                    <%#Eval("LastLogin")%>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Numbers Of Login" ItemStyle-HorizontalAlign="Right">
                <ItemTemplate>
                    <%#Eval("NumLogin")%>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</div>
