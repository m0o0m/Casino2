<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="WhiteLabelSettings.aspx.vb" Inherits="SBSSuperAdmin.WhiteLabelSettings"
    Title="Untitled Page" %>

<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <div style="width: 100%; padding-bottom: 10px;">
        <asp:UpdatePanel ID="pnl" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="bnSave" />
            </Triggers>
            <ContentTemplate>
                <table class="table table-hover">
                    <tr class="tableheading">
                        <td colspan="4">Setting
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 230px;">Site URL
                        </td>
                        <td>
                            <asp:TextBox ID="txtSiteURL" runat="server" CssClass="textInput" Width="95%" />
                        </td>
                        <td style="width: 250px;">Copyright
                        </td>
                        <td>
                            <asp:TextBox ID="txtCopyright" runat="server" CssClass="textInput" Width="95%" />
                        </td>
                    </tr>
                    <tr>
                        <td>Mobile Phone
                        </td>
                        <td>
                            <asp:TextBox ID="txtSuperAgentPhone" runat="server" CssClass="textInput" Width="95%" />
                        </td>
                        <td>Color Scheme
                        </td>
                        <td>
                            <asp:DropDownList CssClass="textInput" ID="ddlColor" runat="server">

                                <asp:ListItem Text="Blue" Value="Blue"></asp:ListItem>
                                <asp:ListItem Text="Black" Value="Black"></asp:ListItem>
                                <asp:ListItem Text="Brown" Value="Brown"></asp:ListItem>
                                <asp:ListItem Text="Layout1" Value="Layout1"></asp:ListItem>
                                <asp:ListItem Text="Layout4" Value="Layout4"></asp:ListItem>
                                <asp:ListItem Text="Layout5" Value="Layout5"></asp:ListItem>
                                <%--<asp:ListItem Text="Red" Value="Red"></asp:ListItem>
                                <asp:ListItem Text="Green" Value="Green"></asp:ListItem>
                                <asp:ListItem Text="Sky" Value="Sky"></asp:ListItem>--%>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Login Template  
                        </td>
                        <td>
                            <asp:DropDownList CssClass="textInput" ID="ddlLoginTemplate" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>Logo File Name
                        </td>
                        <td>
                            <asp:FileUpload ID="FULogo" runat="server" CssClass="textInput" Width="95%" />
                            <asp:Image ID="imgLogo" runat="server" Width="100" Height="70" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>Login Logo</td>
                        <td>
                            <asp:FileUpload ID="FuLoginLogo" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgLoginLogo" Width="100" Height="70" runat="server" Visible="false" />
                            <asp:Button ID="btnDeleteLoginLogo" runat="server" Text="Delete" Visible="false" />
                        </td>
                        <td>Favicon
                        </td>
                        <td>
                            <asp:FileUpload ID="FUFavicon" runat="server" CssClass="textInput"
                                Width="95%" />
                            <asp:Image ID="imgFav" runat="server" Width="100" Height="70" Visible="false" />
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td>WelCome Image
                        </td>
                        <td>
                            <asp:FileUpload ID="FUWelComeImage" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgWelComeImage" Width="100" Height="70" runat="server" Visible="false" />
                        </td>
                        <td>Background Image</td>
                        <td>
                            <asp:FileUpload ID="FuBackgroundImage" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgBackground" Width="100" Height="70" runat="server" Visible="false" />
                            <asp:Button ID="btnDeleteBackground" runat="server" Text="Delete" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>Login Background Image</td>
                        <td>
                            <asp:FileUpload ID="FuBackgroundLoginImage" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgLoginBackground" Width="100" Height="70" runat="server" Visible="false" />
                            <asp:Button ID="btnDeleteLoginBackground" runat="server" Text="Delete" Visible="false" />
                        </td>
                        <td>Left Login Background Image</td>
                        <td>
                            <asp:FileUpload ID="FuLeftBackgroundLoginImage" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgLeftLoginBackground" Width="100" Height="70" runat="server" Visible="false" />
                            <asp:Button ID="btnDeleteLeftLoginBackground" runat="server" Text="Delete" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>Right Login Background Image</td>
                        <td>
                            <asp:FileUpload ID="FuRightBackgroundLoginImage" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgRightLoginBackground" Width="100" Height="70" runat="server" Visible="false" />
                            <asp:Button ID="btnDeleteRightLoginBackground" runat="server" Text="Delete" Visible="false" />
                        </td>
                        <td>Bottom Login Background Image</td>
                        <td>
                            <asp:FileUpload ID="FuBottomBackgroundLoginImage" Width="95%" runat="server"
                                CssClass="textInput" />
                            <asp:Image ID="imgBottomLoginBackground" Width="100" Height="70" runat="server" Visible="false" />
                            <asp:Button ID="btnDeleteBottomLoginBackground" runat="server" Text="Delete" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>Backup URL
                        </td>
                        <td>
                            <asp:TextBox ID="txtBackupURL" runat="server" Width="95%" ></asp:TextBox>
                        </td>
                        <td colspan="2"></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align: center;">
                            <asp:Button ID="bnSave" runat="server" CssClass="btn btn-primary" Text="Save" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-red" Text="Cancel" />
                        </td>
                    </tr>
                </table>
                <div class="mbxl"></div>
                <asp:DataGrid runat="server" ID="dtgSettings" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
                    <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                    <SelectedItemStyle BackColor="YellowGreen" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="Site URL">
                            <ItemTemplate>
                                <%#Container.DataItem("SiteURL")%>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Copyright">
                            <ItemTemplate>
                                <%#Container.DataItem("CopyrightName")%>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Logo">
                            <ItemTemplate>
                                <asp:Image ID="imgLogo" runat="server" Width="50" Height="50" ImageUrl='<%#Container.DataItem("LogoFileName")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("LogoFileName")) <> "" %>' />
                                <asp:HiddenField ID="HFLogo" runat="server" Value='<%#Container.DataItem("LogoFileName")%>' />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Logo Login">
                            <ItemTemplate>
                                <asp:Image ID="imgLogoLogin" runat="server" Width="50" Height="50" ImageUrl='<%#Container.DataItem("LogoLoginImage")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("LogoLoginImage")) <> "" %>' />
                                <asp:HiddenField ID="HFLogoLogin" runat="server" Value='<%#Container.DataItem("LogoLoginImage")%>' />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Favicon">
                            <ItemTemplate>
                                <asp:Image ID="imgFavicon" runat="server" Width="50" Height="50" ImageUrl='<%#Container.DataItem("Favicon")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("Favicon")) <> "" %>' />
                                <asp:HiddenField ID="HFFavicon" runat="server" Value='<%#Container.DataItem("Favicon")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Background">
                            <ItemTemplate>
                                <asp:Image ID="imgBackground" Width="50" Height="50" runat="server" ImageUrl='<%#Container.DataItem("BackgroundImage")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("BackgroundImage")) <> "" %>' />
                                <asp:HiddenField ID="HFBackgroundImage" runat="server" Value='<%#Container.DataItem("BackgroundImage")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn HeaderText="Login">
                            <ItemTemplate>
                                <asp:Image ID="imgBackgroundLogin" Width="50" Height="50" runat="server" ImageUrl='<%#Container.DataItem("BackgroundLoginImage")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("BackgroundLoginImage")) <> "" %>' />
                                <asp:HiddenField ID="HFBackgroundLoginImage" runat="server" Value='<%#Container.DataItem("BackgroundLoginImage")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>


                        <asp:TemplateColumn HeaderText="Left Login">
                            <ItemTemplate>
                                <asp:Image ID="imgLeftBackgroundLoginImage" runat="server" Width="50" Height="50" ImageUrl='<%#Container.DataItem("LeftBackgroundLoginImage")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("LeftBackgroundLoginImage")) <> "" %>' />
                                <asp:HiddenField ID="HFLeftBackgroundLoginImage" runat="server" Value='<%#Container.DataItem("LeftBackgroundLoginImage")%>' />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn HeaderText="Right Login">
                            <ItemTemplate>
                                <asp:Image ID="imgRightBackgroundLoginImage" runat="server" Width="50" Height="50" ImageUrl='<%#Container.DataItem("RightBackgroundLoginImage")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("RightBackgroundLoginImage")) <> "" %>' />
                                <asp:HiddenField ID="HFRightBackgroundLoginImage" runat="server" Value='<%#Container.DataItem("RightBackgroundLoginImage")%>' />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Bottom Login">
                            <ItemTemplate>
                                <asp:Image ID="imgBottomBackgroundLoginImage" runat="server" Width="50" Height="50" ImageUrl='<%#Container.DataItem("BottomBackgroundLoginImage")%>'
                                    Visible='<%# SBCBL.std.SafeString(Container.DataItem("BottomBackgroundLoginImage")) <> "" %>' />
                                <asp:HiddenField ID="HFBottomBackgroundLoginImage" runat="server" Value='<%#Container.DataItem("BottomBackgroundLoginImage")%>' />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateColumn>


                        <asp:TemplateColumn ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbnEdit" CommandArgument='<%#Container.DataItem("WhiteLabelSettingID")%>'
                                    CommandName="EDIT" runat="server">Edit</asp:LinkButton>
                                &nbsp;
                                <asp:LinkButton ID="lbnDelete" CommandArgument='<%#Container.DataItem("WhiteLabelSettingID")%>'
                                    CommandName="DELETE" runat="server">Delete</asp:LinkButton>
                                <cc1:CButtonConfirmer ID="CButtonConfirmer1" runat="server" AttachTo="lbnDelete"
                                    ConfirmExpression="confirm('Are you sure to delete this white label?')">
                                </cc1:CButtonConfirmer>
                            </ItemTemplate>



                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
