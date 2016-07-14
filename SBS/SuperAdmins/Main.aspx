<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false" CodeFile="Main.aspx.vb" Inherits="SBS_SuperAdmins_Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="row mbl">
        <div class="col-lg-12">
            <div class="list-group">
                <asp:Repeater ID="rptSubMenu" runat="server">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton CssClass="list-group-item" ID="LinkButton2" runat="server" ToolTip='<%#Container.DataItem("MenuToolTip")%>' PostBackUrl='<%#Container.DataItem("MenuUrl")%>'><%#Container.DataItem("MenuText")%></asp:LinkButton>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>



