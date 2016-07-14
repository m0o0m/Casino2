<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ConfigureLogo.ascx.vb" Inherits="SBSAgents.ConfigureLogo" %>
<div  style="padding:5px;padding-top:20px;">
    <fieldset style="padding:10px">
        <legend style="margin-left:20px"> Your Image</legend>

            <table style="float:left">
                <tr>
                    <td colspan="2">
                     
                    </td> 
                </tr>
                <tr>
                    <td colspan="2">
                       <%= ShowLogo() %>
                    </td>
                </tr>
                <tr valign="middle">
                                <td>
                                   Your Logo
                                </td>
                                <td>
                                    <asp:FileUpload ID="FULogo" runat="server" CssClass="textInput"  />
                                </td>
                                <td>
                                    <asp:Button id="btnSaveLogo" Text="Upload Logo" runat="server" CssClass="button"/>
                                </td>
                            </tr>
                            
                            <tr>
                                <td colspan="3">
                                   <%=ShowLogoLoginImage()%>
                                </td>
                            </tr>
                           <tr>
                                <td>
                                    Logo Login Image</td>
                                <td>
                                    <asp:FileUpload ID="FuLogoLoginImage" runat="server"     CssClass="textInput" />
                                </td>
                                <td>
                                    <asp:Button id="btnSaveLogoLogin" Text="Upload Logo Login" runat="server" CssClass="button"/>
                                    <asp:Button id="btnRemoveLogoLogin" Text="Remove Logo Login" runat="server" CssClass="button"/>
                                </td>
                        </tr>     
                            
                            <tr>
                    <td colspan="3">
                       <%=ShowBackground()%>
                    </td>
                </tr>
               <tr>
                        <td>
                            Background Image</td>
                        <td>
                            <asp:FileUpload ID="FuBackgroundImage" runat="server"     CssClass="textInput" />
                        </td>
                        <td>
                            <asp:Button id="btnSaveBackground" Text="Upload Background" runat="server" CssClass="button"/>
                             <asp:Button id="btnRemoveBackground" Text="Remove Background" runat="server" CssClass="button"/>
                        </td>
                    </tr>        
                   <tr>
                   <td colspan="2">
                       <%=ShowLoginBackground()%>
                    </td>
                    </tr>
                    <tr>
                        <td>
                            Background Login Image</td>
                        <td>
                            <asp:FileUpload ID="FuBackgroundLoginImage" runat="server"     CssClass="textInput" />
                        </td>
                        <td>
                            <asp:Button id="btnSaveLoginBackground" Text="Upload Login Background" runat="server" CssClass="button"/>
                             <asp:Button id="btnRemoveLoginBackground" Text="Remove Login Background" runat="server" CssClass="button"/>
                        </td>
                    </tr>
                    <tr>
                    <td colspan="2">
                       <%=ShowLeftBackgroundLoginImage()%>
                    </td>
                </tr>
                    <tr>
                        <td>
                           Left Background Login Image</td>
                        <td>
                            <asp:FileUpload ID="FuLeftBackgroundLoginImage" runat="server"     CssClass="textInput" />
                        </td>
                        <td>
                            <asp:Button id="btnSaveLoginLeftBackground" Text="Upload Login Left Background" runat="server" CssClass="button"/>
                             <asp:Button id="btnRemoveLoginLeftBackground" Text="Remove Login Left Background" runat="server" CssClass="button"/>
                        </td>
                    </tr>  
                     <tr>
                    <td colspan="2">
                       <%=ShowRightBackgroundLoginImage()%>
                    </td>
                </tr>
                    <tr>
                        <td>
                           Right Background Login Image</td>
                        <td>
                            <asp:FileUpload ID="FuRightBackgroundLoginImage" runat="server"     CssClass="textInput" />
                        </td>
                        <td>
                            <asp:Button id="btnSaveLoginRightBackground" Text="Upload Login Right  Background" runat="server" CssClass="button"/>
                             <asp:Button id="btnRemoveLoginRightBackground" Text="Remove Login Right Background" runat="server" CssClass="button"/>
                        </td>
                    </tr> 
                     <tr>
                    <td colspan="2">
                       <%=ShowBottomBackgroundLoginImage()%>
                    </td>
                </tr>
                    <tr>
                        <td>
                           Bottom Background Login Image</td>
                        <td>
                            <asp:FileUpload ID="FuBottomBackgroundLoginImage" runat="server"     CssClass="textInput" />
                        </td>
                        <td>
                            <asp:Button id="btnSaveLoginBottomBackground" Text="Upload Login Bottom Background" runat="server" CssClass="button"/>
                             <asp:Button id="btnRemoveLoginBottomBackground" Text="Remove Login Bottom Background" runat="server" CssClass="button"/>
                        </td>
                    </tr>           
            </table>

    </fieldset>
</div> 