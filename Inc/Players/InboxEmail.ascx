<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InboxEmail.ascx.vb" Inherits="SBCPlayer.InboxEmail" %>
 <%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fck" %>
 <asp:DataGrid ID="dgEmailInbox" runat="server" AutoGenerateColumns="false" Width="100%" ItemStyle-ForeColor="Black"
    CssClass="table table-hover table-bordered">
    <HeaderStyle CssClass="tableheading" ForeColor="Black" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Left" />
      <Columns >
    <asp:TemplateColumn ItemStyle-Width="20" >
        <ItemTemplate>
         <asp:Panel ID="pnAlertMail" runat="server" Visible='<%#iif(DataBinder.Eval(Container.DataItem, "IsMailOpen").equals("N") AndAlso ((SBCBL.std.safeString(DataBinder.Eval(Container.DataItem, "IsAnswer")).equals("Y") OrElse (SBCBL.std.safeString(DataBinder.Eval(Container.DataItem, "IsSuperNotice")).equals("Y"))) OrElse ReplyEmail = EReply.Reply),true,false) %>'>
                        <img alt="Email" src="/images/MailAlert.png" id="imgMail" />
                    </asp:Panel>
        </ItemTemplate>
        </asp:TemplateColumn>
        </Columns>
     <Columns >
    <asp:TemplateColumn HeaderText="Email Subject" ItemStyle-Width="30%" HeaderStyle-CssClass ="titleColor" >
        <ItemTemplate>
                  <asp:LinkButton ID="lbnSubject"  Text='<%#DataBinder.Eval(Container.DataItem, "Subject") %>' CommandName="SHOW" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>' Font-Underline="false" runat="server"></asp:LinkButton>
        </ItemTemplate>
        </asp:TemplateColumn> 
    </Columns>
    <Columns>
    <asp:TemplateColumn HeaderText="Messages"  HeaderStyle-CssClass ="titleColor" >
        <ItemTemplate>
           <asp:LinkButton ID="lbnMessages" Font-Underline="false"  runat="server" CommandName="SHOW" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>'> <asp:Literal ID="lblMessages" Text='<%# DataBinder.Eval(Container.DataItem, "Messages").replace("<p>","").replace("</p>","")%>' Mode="Encode" runat="server" /></asp:LinkButton>
        </ItemTemplate>
        </asp:TemplateColumn> 
    </Columns>
    <Columns>
    <asp:TemplateColumn HeaderText="Sent Date" ItemStyle-Width="10%" ItemStyle-Wrap="false" HeaderStyle-CssClass ="titleColor" >
        <ItemTemplate>
            <asp:LinkButton ID="lbnSentDates" Font-Underline="false"  runat="server" CommandName="SHOW" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>'> <asp:Literal ID="lblSentDates" Text='<%# DataBinder.Eval(Container.DataItem, "SentDate")%>' Mode="Encode" runat="server" /></asp:LinkButton>
        </ItemTemplate>
        </asp:TemplateColumn> 
    </Columns>
    <Columns>
        <asp:TemplateColumn HeaderText="Reply" ItemStyle-Width="5%" HeaderStyle-CssClass ="titleColor">
            <ItemTemplate>
                <asp:LinkButton ID="lbtEmailReply" runat="server" Text="Reply" CommandName="REPLY" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailID")%>' Visible='<%# iif(SBCBL.std.safeString(DataBinder.Eval(Container.DataItem, "IsAnswer")).equals("Y"),true,false) %>'
                    Font-Underline="false" ToolTip="View Email Reply"></asp:LinkButton>
                   </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<asp:Label ID="lblNoMessage" runat="server" style="margin:0 auto" Text=""></asp:Label>
<fieldset style="width:806px;background:#e3e4e7" visible="false" runat="server" id="fdsEmailContent" >
<legend style="margin-left:20px">
   <b style="Color:#036c08;vertical-align:top">Email Message  </b>
 </legend>
 <table>
    <tr>
        <td>
         <b style="color:#036c08">From :</b>   <asp:label ID="lblFrom" ForeColor="blue"  runat="server"></asp:label>
        </td>
    </tr>
    <tr>
        <td>
          <b style="color:#036c08">To :</b>  <asp:label ID="lblTo"  ForeColor="blue"  runat="server"></asp:label>
        </td>
    </tr>
    <tr>
        <td>
            <h2 style="margin-bottom:-1px;margin-top:-1px"><asp:Label ID="lblEmailSubject" ForeColor="Black" runat="server" Text=""></asp:Label></h2>
        </td>
    </tr>
    <tr id="trEmailMessages" runat="server">
        <td>
           <iframe width="800" height="225"  id="ifMessage" src ="../../Utils/EmailMessage.aspx?MailId=+ <asp:Literal ID='lblMessages'   runat='server'/>" >
           </iframe>
        </td>
    </tr>
    
    
    <tr id="trReplyEmail" runat="server">
        <td>
        
           <iframe width="800" height="225"  id="ifEmailMessages" src ="/Utils/EmailMessage.aspx?MailId=+ <asp:Literal ID='lblEmailMessages'   runat='server'/>" >
           </iframe>
           <div id="dvContentReply">
               <b style="Color:#036c08;margin:0;font-size:larger">Reply Message  </b>
               <iframe width="800" height="225"  id="ifMessageReply" src ="/Utils/EmailMessage.aspx?MailId=+ <asp:Literal ID='lblMessagesReply'   runat='server'/>" >
               </iframe>
           </div>
        </td>
    </tr>
    
    
    
 </table>

</fieldset>

