<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master" AutoEventWireup="false"
    CodeFile="PlayerTemplates.aspx.vb" Inherits="SBSSuperAdmin.PlayerTemplates" %>

<%@ Register Src="~/Inc/SuperAdmins/editPlayerTemplate.ascx" TagName="editPlayerTemplate" TagPrefix="uc1" %>
<%@ Register Src="~/SBS/Inc/PlayerLimit.ascx" TagName="editPlayerTemplateLimits" TagPrefix="uc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:UpdatePanel runat="server" ID="upPlayers">
        <ContentTemplate>

            <div class="row">
                <div class="col-lg-4">
                    <h3>List Templates</h3>
                    <asp:DataGrid ID="grdTemplates" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
                        <HeaderStyle CssClass="tableheading" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Left" />
                        <AlternatingItemStyle HorizontalAlign="Left" />
                        <SelectedItemStyle BackColor="YellowGreen" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Name">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtEdit" CssClass="itemplayer" runat="server" CausesValidation="false" CommandName="EDIT_TEMPLATE"
                                        CommandArgument='<%# Container.DataItem("PlayerTemplateID") %>' Text='<%# Container.DataItem("TemplateName") %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Balance" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="85px">
                                <ItemTemplate>
                                    <%#FormatCurrency(SBCBL.std.SafeDouble(Container.DataItem("AccountBalance"))).Replace("$", "")%>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="25px">
                                <ItemTemplate>
                                    <asp:LinkButton CssClass="itemplayer" ID="lbtCopy" runat="server" CausesValidation="false" CommandArgument='<%# Container.DataItem("PlayerTemplateID") %>'
                                        CommandName="COPY_TEMPLATE" Text="Copy" ToolTip="Copy this template to create new" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <asp:Label ID="lblAlert" runat="server" ForeColor="Red" Text="There are 0 templates" Visible="false" />
                </div>
                <div class="col-lg-8">
                    <h3>Template Detail</h3>
                    <nav role="navigation" class="navbar navbar-default">
                        <div class="container-fluid">
                            <div class="navbar-header">
                                <span class="navbar-brand">Tabs:</span>
                            </div>
                            <div id="bs-example-navbar-collapse-1" class="collapse navbar-collapse">
                                <ul class="nav navbar-nav">
                                    <li class="active" id="tTempl" runat="server">
                                        <asp:LinkButton runat="server" ID="lbtTabTemplate" CommandArgument="Template" ToolTip="Template tab"
                                            Text="Template" CausesValidation="false" OnClick="lbtTab_Click" />
                                    </li>
                                    <li id="tTemplLimits" runat="server">
                                        <asp:LinkButton runat="server" ID="lbtTabLimits" CommandArgument="Limits" ToolTip="Template Limits tab"
                                            Text="Template Limits" CausesValidation="false" OnClick="lbtTab_Click" />
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </nav>
                    <div class="col-lg-12">
                        <uc1:editPlayerTemplate ID="ucEditTemplate" runat="server" />
                        <uc1:editPlayerTemplateLimits ID="ucEditLimits" runat="server" />
                    </div>
                    <div class="col-lg-12">
                        <asp:HiddenField ID="hfTemplateID" runat="server" />
                        <asp:HiddenField ID="hfCopyFromTemplateID" runat="server" />
                        <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" CausesValidation="false"
                            Text="Save" ToolTip="Save this template" />
                        <asp:Button ID="btnCopy" runat="server" CssClass="btn btn-green" CausesValidation="false"
                            Text="Copy" ToolTip="Copy this template to create new" />
                        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-red" CausesValidation="false"
                            Text="Cancel" ToolTip="Cancel" />
                    </div>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="mbxl"></div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <%--Processing--%>
    <asp:UpdateProgress ID="up1" runat="server" DisplayAfter="300">
        <ProgressTemplate>
            <div id="divUpdateProgressModal" class="modalBackground">
            </div>
            <div class="ProcessingPanel" id="divProcessPnl" style="left: 400px; top: 200px; position: fixed">
                <img alt="processing" src="/images/processing.gif" style="vertical-align: text-bottom;" />
                &nbsp;&nbsp; <b>Processing. Please wait...</b>
            </div>

            <script type="text/javascript">

                // This help to fill the gray cover to full of the screen
                // 1.get instance of the PageRequestManager
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                // 2.register 'what i'm gonna do is SHOW the gray screen' when init the Request ;)
                prm.add_initializeRequest(InitializeRequest);
                // 3.and 'the gray screen' should disapear when the Request end
                prm.add_endRequest(EndRequest);

                function InitializeRequest(sender, args) {
                    //                    if (Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                    //                        alert('One postback at a time please');
                    //                        args.set_cancel(true);
                    //
                    grayOut(true, "");
                }

                function EndRequest(sender, args) {
                    debugger;
                    var e = args.get_error();

                    if (e != null) {
                        args.set_errorHandled(true);
                        alert(e.message.replace("Sys.WebForms.PageRequestManagerServerErrorException: ", ""));
                    }

                    grayOut(false, "");
                    setIsGrayOut(true);
                }
                var _IsShowGrayOut = true;
                function setIsGrayOut(val) {
                    _IsShowGrayOut = val;
                }
                function GrayOutAfterRequest() {
                    setTimeout("grayOut(true,'')", 0);
                }
                function grayOut(vis, options) {
                    var optionsoptions = options || {};
                    var zindex = options.zindex || 9;
                    var opacity = options.opacity || 50;
                    var opaque = (opacity / 100);
                    var bgcolor = options.bgcolor || '#5b5a58';
                    var dark = document.getElementById('darkenScreenObject');
                    if (!dark) {
                        // The dark layer doesn't exist, it's never been created.  So we'll    
                        // create it here and apply some basic styles.     
                        var tbody = document.getElementsByTagName("body")[0];
                        var tnode = document.createElement('div');
                        tnode.style.position = 'absolute';
                        tnode.style.top = '0px';
                        tnode.style.left = '0px';
                        tnode.style.overflow = 'hidden';
                        tnode.style.display = 'none';
                        tnode.id = 'darkenScreenObject';
                        tbody.appendChild(tnode);
                        dark = document.getElementById('darkenScreenObject');
                    }

                    if (vis) {
                        dark.style.opacity = opaque;
                        dark.style.MozOpacity = opaque;
                        dark.style.filter = 'alpha(opacity=' + opacity + ')';
                        dark.style.zIndex = zindex;
                        dark.style.backgroundColor = bgcolor;
                        dark.style.width = document.body.scrollWidth + 'px';
                        dark.style.minWidth = '100%';
                        dark.style.height = document.body.scrollHeight + 'px';
                        dark.style.minHeight = '100%';
                        dark.style.display = 'block';
                        $('#' + '<%=up1.ClientID %>').css('display', '');
                        $('#divProcessPnl').css('display', '');
                    }
                    else {
                        dark.style.display = 'none';
                        $('#' + '<%=up1.ClientID %>').css('display', 'none');
                        $('#divProcessPnl').css('display', 'none');
                    }

                    if (!_IsShowGrayOut && vis) {
                        dark.style.display = 'none';
                        $('#' + '<%=up1.ClientID %>').css('display', 'none');
                        $('#divProcessPnl').css('display', 'none');
                    }
                }
            </script>

        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>
