<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InboxMail.ascx.vb" Inherits="SBCSuperAdmins.InboxMail" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fck" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<div class="row">
    <div class="col-lg-12">
        <label>Email Subject</label>
        <wlb:CDropDownList ID="ddlEmailSubject" runat="server" CssClass="form-control" Style="display: inline-block;" Width="600" hasOptionalItem="true" OptionalText="All" OptionalValue="" AutoPostBack="true" />
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <asp:DataGrid ID="dgEmailInbox" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
            <HeaderStyle CssClass="tableheading" ForeColor="Black" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Left" />
            <Columns>
                <asp:TemplateColumn HeaderText="" ItemStyle-Width="30">
                    <ItemTemplate>
                        <asp:Panel ID="pnAlertMail" runat="server" Visible='<%#iif(DataBinder.Eval(Container.DataItem, "IsSuperMailOpen").equals("N"),true,false) %>'>
                            <img alt="Email" src="/images/MailAlert.png" id="imgMail" />
                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Email Subject" ItemStyle-Width="30%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbnSubject" Text='<%#DataBinder.Eval(Container.DataItem, "Subject") %>' CommandName="SHOW" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>' ForeColor="Black" Font-Underline="false" runat="server"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Messages" ItemStyle-Width="60%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbnMessages" ForeColor="Black" Font-Underline="false" runat="server" CommandName="SHOW" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>'>
                            <asp:Literal ID="lblMessages" Text='<%# DataBinder.Eval(Container.DataItem, "Messages").replace("<p>","").replace("</p>","")%>' Mode="Encode" runat="server" />
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Sender">
                    <ItemTemplate>
                        <asp:Label ID="lblSender" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="UserType">
                    <ItemTemplate>
                        <asp:Label ID="lblUserType" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Sent Date" ItemStyle-Width="10%" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbnSentDates" ForeColor="Black" Font-Underline="false" runat="server" CommandName="SHOW" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>'>
                            <asp:Literal ID="lblSentDates" Text='<%# DataBinder.Eval(Container.DataItem, "SentDate")%>' Mode="Encode" runat="server" />
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <Columns>
                <asp:TemplateColumn HeaderText="Reply" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtEmailReply" runat="server" Text="Reply" CommandName="REPLY" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>' Visible='<%# iif(SBCBL.std.safeString(DataBinder.Eval(Container.DataItem, "IsAnswer")).equals(""),true,false) %>'
                            Font-Underline="false" ToolTip="View Email Reply"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>

<div class="row">
    <div class="col-lg-12">
        <asp:Label ID="lblNoMessage" runat="server" Style="margin: 0 auto" Text=""></asp:Label>
        <asp:HiddenField ID="hfEmailID" runat="server" />
        <asp:HiddenField ID="hfToID" runat="server" />
        <fieldset visible="false" runat="server" id="fdsEmailContent">
            <legend>
                <b>Email Message  </b>
            </legend>
            <table class="table table-hover">
                <tr>
                    <td>
                        <b style="color: #036c08">From :</b>
                        <asp:Label ID="lblFrom" ForeColor="blue" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b style="color: #036c08">To :</b>
                        <asp:Label ID="lblTo" ForeColor="blue" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <h2 style="margin-bottom: -1px; margin-top: -1px">
                            <asp:Label ID="lblEmailSubject" ForeColor="Black" runat="server" Text=""></asp:Label></h2>
                    </td>
                </tr>
                <tr>
                    <td>

                        <iframe width="800" height="225" id="ifMessage" src="/Utils/EmailMessage.aspx?MailId=+ <asp:Literal ID='lblMessages'   runat='server'/>"></iframe>
                        <div id="dvContentReply" runat="server">
                            <b style="Color: #036c08; margin: 0; font-size: larger">Reply Message  </b>
                            <iframe width="800" height="225" id="ifMessageReply" src="/Utils/EmailMessage.aspx?MailId=+ <asp:Literal ID='lblMessagesReply'   runat='server'/>"></iframe>
                        </div>
                    </td>
                </tr>

                <tr id="trReply" runat="server">
                    <td>
                        <b style="Color: #036c08; margin: 0; font-size: larger">Reply Message  </b>
                        <asp:TextBox ID="fckMessage" Width="800" Height="300" TextMode="MultiLine" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnReply" runat="server" Text="Reply" CssClass="btn btn-primary" />
                        <asp:Button ID="btnCancel" runat="server" Text="Back" CssClass="btn btn-primary" />
                    </td>
                </tr>
            </table>

        </fieldset>

    </div>
    <div class="clearfix"></div>
</div>
<div class="mbxl"></div>



