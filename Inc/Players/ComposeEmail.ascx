<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ComposeEmail.ascx.vb"
    Inherits="SBCPlayer.ComposeEmail" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fck" %>

<div class="panel panel-grey">
    <div class="panel-heading">Compose Mail</div>
    <div class="panel-body">
        <div class="row">
            <div id="trSelectUser" runat="server" class="form-group">
                <div class="col-md-2">
                    <label class="control-label">Select Type User</label>
                </div>
                <div class="col-md-10">
                    <asp:RadioButtonList ID="rblUser" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="form-control"
                        AutoPostBack="true">
                        <asp:ListItem Selected="True" Text="All" Value="All" Style="margin-left: 15px"></asp:ListItem>
                        <asp:ListItem Text="Agent" Value="Agent" Style="margin-left: 15px"></asp:ListItem>
                        <asp:ListItem Text="Player" Value="Player" Style="margin-left: 15px"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div class="col-md-2">
                    <asp:Label runat="Server" ID="lblAgent" class="control-label" Text="Agent" Visible="false" />
                </div>
                <div class="col-md-10">
                    <wlb:CDropDownList ID="ddlAgent" Visible="false" runat="server" CssClass="form-control"
                        hasOptionalItem="true" OptionalText="All" OptionalValue="ALL"
                        AutoPostBack="true" />
                </div>
                <div class="col-md-2">
                    <asp:Label runat="Server" ID="lblPlayer" class="control-label" Text="Player" Visible="false" />
                </div>
                <div class="col-md-10">
                    <wlb:CDropDownList ID="ddlPlayer" Visible="false" runat="server" CssClass="form-control"
                        AutoPostBack="false" />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <label class="control-label col-sm-2">Email Subject</label>
                <div class="col-md-10">
                    <wlb:CDropDownList ID="ddlEmailSubject" runat="server" CssClass="form-control"
                        hasOptionalItem="false" AutoPostBack="false" />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <label class="control-label col-sm-2">From</label>
                <div class="col-md-10">
                    <asp:TextBox ID="txtReplyToAddress" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RegularExpressionValidator CssClass="error" ID="RegularExpressionValidatorEmail"
                        ValidationGroup="senEmail" ControlToValidate="txtReplyToAddress" Display="Dynamic"
                        runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Email Is Wrong"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <label class="control-label col-sm-2">Content</label>
                <div class="col-md-10">
                    <asp:TextBox ID="fckMessage" TextMode="MultiLine" Rows="7" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-10 col-lg-offset-2">
                    <asp:Label ID="lblMessageEmpty" ForeColor="red" runat="server" Style="float: left" Text=""></asp:Label>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-10 col-lg-offset-2">
                    <asp:Button ID="btnSenMail" runat="server" Text="Send Email" ValidationGroup="senEmail" CssClass="btn btn-grey" />
                </div>
            </div>
        </div>
    </div>
</div>
