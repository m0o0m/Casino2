<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlayerEdit.ascx.vb" Inherits="SBCAgents.Inc_PlayerEdit" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/Inc/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>
<%@ Register Src="~/SBS/Inc/PlayerLimit.ascx" TagName="PlayerLimit" TagPrefix="uc1" %>
<style type="text/css">
    .strengthMeter { width: 150px; }
</style>
<div class="form-group">
    <div class="col-md-12">
        <asp:Button ID="btnCancelInfo" runat="server" CssClass="btn btn-default pull-right" Text="Cancel" CausesValidation="false" />
        <asp:Button ID="btnSaveInfo" runat="server" CssClass="btn btn-success pull-right mr4 ml4" Text="Save" />
        <asp:Button ID="btnResetBalance" runat="server" OnClientClick="return confirm('Do You Want Reset Manualy Account Balance ?')" CssClass="btn btn-primary pull-right" Visible="false" Text="Reset Manualy Account Balance" />
    </div>
</div>
<div class="form-group">
    <div class="col-lg-6">
        <div class="panel panel-grey">
            <div class="panel-heading">Player's Infomation</div>
            <div class="panel-body">
                <div class="form-group">
                    <label class="col-md-3 control-label">Name</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="txtName" CssClass="form-control" MaxLength="50" runat="server"
                            AutoPostBack="true" />
                    </div>
                    <div class="col-md-1">
                        <asp:ImageButton ID="ibtGenerateLogin" runat="server" ToolTip="Generate login" ImageUrl="~/images/cancel.gif"
                            ImageAlign="AbsMiddle" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Login</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="txtLogin" CssClass="form-control" MaxLength="50" runat="server"
                            onBlur="AutoFillFoneLogin()" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-3">
                        <label class="control-label">Password</label>
                        <label class="control-label mt20">Confirm Password</label>
                    </div>
                    <div class="col-md-6">
                        <uc1:passwordEditor runat="server" ID="psdPassword" Required="false" HorizontalAlign="false"
                            SetOnblurFunction="AutoFillPhonePassWord()" TextVisible="false" SetCheckCapsLockClientFunction="capsLock(event, 'divCapsLock')" />
                        <asp:HiddenField ID="hfdPassword" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Num of Failed</label>
                    <div class="col-md-6">
                        <asp:TextBox CssClass="form-control" ID="txtNumFailedAttempts" MaxLength="4" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Agent</label>
                    <div class="col-md-6">
                        <div id="divCapsLock" style="color: red; display: none;">Caps Lock is ON.</div>
                        <cc1:CDropDownList ID="ddlAgents" runat="server" CssClass="form-control" hasOptionalItem="false"
                            AutoPostBack="true" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">TimeZone</label>
                    <div class="col-md-6">
                        <cc1:CDropDownList ID="ddlTimeZone" runat="server" CssClass="form-control" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-3"></div>
                    <div class="col-md-6">
                        <asp:CheckBox ID="chkRequireChangePass" runat="server" Text="Require change password" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-3"></div>
                    <div class="col-md-9">
                        <asp:CheckBox ID="chkIsBettingLocked" runat="server" Text="Betting Locked" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-3"></div>
                    <div class="col-md-3">
                        <asp:CheckBox ID="chkCasino" runat="server" Text="Casino" />
                    </div>
                    <div class="col-md-3">
                        <asp:CheckBox ID="chkIsLocked" runat="server" Text="Locked" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Lock reason</label>
                    <div class="col-md-9">
                        <asp:TextBox ID="txtLockreason" CssClass="form-control" runat="server" MaxLength="200"
                            TextMode="MultiLine" Columns="20" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Alert Message</label>
                    <div class="col-md-9">
                        <asp:TextBox ID="txtAlertMessage" CssClass="form-control" runat="server" MaxLength="200"
                            TextMode="MultiLine" />
                    </div>
                </div>
                <div class="form-actions text-right pal">
                    <asp:Button ID="btnSavePlayerInfo" runat="server" Text="Save Player Info" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="panel panel-grey">
            <div class="panel-heading">Limits</div>
            <div class="panel-body">
                <div class="form-group">
                    <label class="col-md-3 control-label">Template</label>
                    <div class="col-md-3">
                        <cc1:CDropDownList ID="ddlTemplates" runat="server" CssClass="form-control"
                            AutoPostBack="true" />
                    </div>
                    <div class="col-md-6"></div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Account Balance</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtAccountBalance" runat="server" CssClass="form-control"
                            onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:formatNumber(this,2);" Enabled="false" />
                    </div>
                    <div class="col-md-6"></div>
                </div>

                <div class="form-group">
                    <label class="col-md-3 control-label">Max Casino Credit</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtCasinoMaxAmount" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                            onblur="javascript:formatNumber(this,2);" />
                    </div>
                    <div class="col-md-6"></div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Min Bet Phone</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtCreditMinBetPhone" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                            onblur="javascript:formatNumber(this,2);" />
                    </div>
                    <div class="col-md-6"></div>
                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Min Bet Internet</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtCreditMinBetInternet" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                            onblur="javascript:formatNumber(this,2);" />
                    </div>

                </div>

                <div class="mbxl"></div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Credit Limit</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtCreditMaxAmount" runat="server" CssClass="form-control"
                            onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:formatNumber(this,2);" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnUpdateOrginalAmount" CssClass="btn btn-primary" runat="server" Text="Update" />
                    </div>

                </div>
                <div class="form-group">
                    <label class="col-md-3 control-label">Temporary Credit</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtTemporary" Visible="true" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                            onblur="javascript:formatNumber(this,2);" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnTemporary" CssClass="btn btn-primary" runat="server" Text="Update Temporary" />
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-md-3 control-label">Max Per Game</label>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtCreditmaxSingle" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                            onblur="javascript:formatNumber(this,2);" />
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnUpdateMaxPerGame" CssClass="btn btn-primary" runat="server" Text="Update Max Per Game" />
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-md-3 control-label">Manually Reset Balance</label>
                    <div class="col-md-3">
                        <asp:CheckBox ID="chkManuaReset" AutoPostBack="true" runat="server" Text=""></asp:CheckBox>
                    </div>
                    <div class="col-md-6">
                        <asp:Button ID="btnResetMunualy" Visible="false" CssClass="btn btn-primary" runat="server" Text="Reset Manually" />
                    </div>
                </div>
                
                <div class="form-actions text-right pal">
                    <asp:Button ID="btnUpdateLimits" Visible="false" runat="server" Text="Update Limit" CssClass="btn btn-primary" />
                    <asp:Button ID="btnSavePlayerLimitInfo" runat="server" Text="Save Limit" CssClass="btn btn-primary" />
                </div>

                <%--Hidden field value--%>
                <div class="form-group hide">
                    <div class="col-md-4 hide">
                        <asp:HiddenField ID="hfBalanceAmount" runat="server" />
                        <asp:HiddenField ID="hfMaxCredit" runat="server" />

                        <label class="col-md-3 control-label hide">Max 1H</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txt1stH" Visible="false" runat="server" CssClass="form-control"
                                onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max 2H</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox Visible="false" ID="txt2stH" runat="server" CssClass="form-control"
                                onkeypress="javascript:return inputNumber(this,event, false);" onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max 1Q</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txt1stQ" Visible="false" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max 2Q</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txt2ndQ" Visible="false" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max 3Q</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txt3rdQ" runat="server" Visible="false" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max 4Q</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txt4thQ" runat="server" Visible="false" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max Reverse</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txtCreditMaxReverseActionParlay" Visible="false" runat="server" CssClass="form-control"
                                onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>

                        <label class="col-md-3 control-label ">Max Parlay</label>
                        <div class="col-md-3 ">
                            <asp:TextBox ID="txtCreditMaxParlay" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>
                        <label class="col-md-3 control-label hide">Max Teaser</label>
                        <div class="col-md-3 hide">
                            <asp:TextBox ID="txtCreditMaxTeaserParlay" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumber(this,event, false);"
                                onblur="javascript:formatNumber(this,2);" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<uc1:PlayerLimit runat="server" ID="ucPlayerLimit" />

<div class="form-group">
    <div class="col-md-12">
        <asp:HiddenField ID="hfPlayerID" runat="server" />
        <asp:HiddenField ID="hfPlayerTemplateID" runat="server" />
        <asp:HiddenField ID="hfOldLogin" runat="server" />
        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default pull-right mr4" Text="Cancel"
            CausesValidation="false" />
        <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success pull-right" Text="Save" />
    </div>
</div>

<script language="javascript">
    window.onload = function () {
        var Height = document.getElementById("tdRight").clientHeight - 14
        document.getElementById("fsRight").style.height = Height + "px";
    }


</script>

