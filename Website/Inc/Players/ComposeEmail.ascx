<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ComposeEmail.ascx.vb"
    Inherits="SBCPlayer.ComposeEmail" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fck" %>

<div class="panel panel-grey panel-style-1">
    <div class="panel-heading">Compose Mail</div>
    <div class="panel-body pdTB10">

        <div class="row">
            <div id="trSelectUser" runat="server" class="form-group clear">
                <div class="ly-w-1:6 left">
                    <label class="control-label">Select Type User</label>
                </div>
                <div class="ly-w-5:6 left">
                    <asp:RadioButtonList ID="rblUser" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="form-control"
                        AutoPostBack="true">
                        <asp:ListItem Selected="True" Text="All" Value="All" Style="margin-left: 15px"></asp:ListItem>
                        <asp:ListItem Text="Agent" Value="Agent" Style="margin-left: 15px"></asp:ListItem>
                        <asp:ListItem Text="Player" Value="Player" Style="margin-left: 15px"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div class="ly-w-1:6 left">
                    <asp:Label runat="Server" ID="lblAgent" class="control-label" Text="Agent" Visible="false" />
                </div>
                <div class="ly-w-5:6 left">
                    <wlb:CDropDownList ID="ddlAgent" Visible="false" runat="server" CssClass="form-control select-field-3 h28px full-w"
                        hasOptionalItem="true" OptionalText="All" OptionalValue="ALL"
                        AutoPostBack="true" />
                </div>
                <div class="ly-w-1:6 left">
                    <asp:Label runat="Server" ID="lblPlayer" class="control-label" Text="Player" Visible="false" />
                </div>
                <div class="ly-w-5:6 left">
                    <wlb:CDropDownList ID="ddlPlayer" Visible="false" runat="server" CssClass="form-control select-field-3 h28px full-w"
                        AutoPostBack="false" />
                </div>
            </div>
        </div>
        <div class="row pd10">
            <div class="form-group clear">
                <label class="control-label ly-w-1:6 left pdT5">Email Subject</label>
                <div class="ly-w-5:6 left">
                    <wlb:CDropDownList ID="ddlEmailSubject" runat="server" CssClass="form-control select-field-3 h28px full-w"
                        hasOptionalItem="false" AutoPostBack="false" />
                </div>
            </div>
        </div>
        <div class="row pd10">
            <div class="form-group clear">
                <label class="control-label ly-w-1:6 left pdT5">From</label>
                <div class="ly-w-5:6 left">
                    <asp:TextBox ID="txtReplyToAddress" runat="server" CssClass="form-control input-field-2 h28px full-w"></asp:TextBox>
                    <asp:RegularExpressionValidator CssClass="error" ID="RegularExpressionValidatorEmail"
                        ValidationGroup="senEmail" ControlToValidate="txtReplyToAddress" Display="Dynamic"
                        runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Email Is Wrong"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="row pd10">
            <div class="form-group clear">
                <label class="control-label ly-w-1:6 left pdT5">Content</label>
                <div class="ly-w-5:6 left">
                    <asp:TextBox ID="fckMessage" TextMode="MultiLine" Rows="7" CssClass="form-control input-field-2 h100px full-w pdT5-i" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row pd10">
            <div class="form-group clear">
                <label class="control-label ly-w-1:6 left"></label>
                <div class="ly-w-5:6 left">
                    <asp:Label ID="lblMessageEmpty" ForeColor="red" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>
        <div class="row pd10">
            <div class="form-group clear">
                <label class="control-label ly-w-1:6 left">&nbsp;&nbsp;</label>
                <div class="ly-w-5:6 left">
                    <asp:Button ID="btnSenMail" runat="server" Text="Send Email" ValidationGroup="senEmail" CssClass="btn btn-grey button-style-2 h26px w100px" />
                </div>
            </div>
        </div>
    </div>
</div>
