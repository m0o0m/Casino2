<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentEdit.ascx.vb" Inherits="SBSAgents.Inc_Agents_AgentEdit" %>
<%@ Register Assembly="WebsiteLibrary" Namespace="WebsiteLibrary" TagPrefix="wlb" %>
<%@ Register Src="~/Inc/passwordEditor.ascx" TagName="passwordEditor" TagPrefix="uc1" %>

<script type="text/javascript">
    function CheckPreset(objSource, objID) {
        if (objSource.checked) {
            var o = document.getElementById(objID);
            o.checked = false;
        }        
    }
</script>

<div id="tblEditAgent"  class="panel panel-grey">
    <div class="panel-heading">Agent Info</div>
    <div class="panel-body">
         <div class="form-group">
             <label class="col-md-2 control-label">Name</label>
             <div class="col-md-4">
                 <asp:TextBox ID="txtName" CssClass="form-control" MaxLength="50" runat="server" AutoPostBack="true" />
             </div>
             <div class="col-md-4">
                  <asp:ImageButton ID="ibtGenerateLogin" runat="server" ToolTip="Generate login" ImageUrl="~/images/cancel.gif"
                ImageAlign="AbsMiddle" />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Login</label>
             <div class="col-md-4">
                 <asp:TextBox ID="txtLogin" CssClass="form-control" MaxLength="50" runat="server" />
             </div>
         </div>
         <div class="form-group">
             <div class="col-md-2">
                 <label class="control-label">Password</label>
                 <label class="control-label mt20">Confirm Password</label>
             </div>
             <div class="col-md-4">
                 <uc1:passwordEditor runat="server" ID="psdPassword" Required="false" HorizontalAlign="false"
                    TextVisible="false" SetCheckCapsLockClientFunction="capsLock(event, 'divCapsLock')" />
                <asp:HiddenField ID="hfdPassword" runat="server" />
                <div id="divCapsLock" style="color: red; display: none;">
                    Caps Lock is ON.</div>
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Is Child Of</label>
             <div class="col-md-4">
                 <wlb:CDropDownList ID="ddlPAgents" runat="server" Enabled="false" CssClass="form-control" hasOptionalItem="false" AutoPostBack="true" />
                <asp:HiddenField ID="hfPAgentID" runat="server" />
                <asp:HiddenField ID="hfProfitPercentage" runat="server" />
                <asp:HiddenField ID="hfGrossPercentage" runat="server" />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">TimeZone</label>
             <div class="col-md-4">
                <wlb:CDropDownList ID="ddlTimeZone" runat="server" CssClass="form-control" /> 
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Assign Group Letters</label>
             <div class="col-md-4">
                 <asp:TextBox ID="txtSpecialKey" runat="server" CssClass="form-control" MaxLength="4"
                Style="width: 50px"></asp:TextBox>(max 4)
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Starting Number</label>
             <div class="col-md-4">
                 <asp:TextBox ID="txtCurrentPlayerNumber" runat="server" CssClass="form-control" onkeypress="javascript:return inputNumberOnly(event);"
                MaxLength="5" Style="text-align: right; width: 50px"></asp:TextBox>
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Preset Player Accounts</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chk10Acc" onclick="javascript:CheckPreset(this,chk20Acc)" Text="10 Accts"
                runat="server" />
                 &nbsp;&nbsp;&nbsp;
                 <asp:CheckBox ID="chk20Acc" onclick="javascript:CheckPreset(this, chk10Acc)" Text="20 Accts"
                runat="server" />
             </div>
         </div>
         <div  id="trPlayerTemplate" runat="server" class="form-group">
             <label class="col-md-2 control-label">Enable Player Template</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkEnablePlayerTemplate" runat="server" AutoPostBack="false"  />
             </div>
         </div>
         <div  id="trBettingProfile" runat="server" class="form-group">
             <label class="col-md-2 control-label">Enable Betting Profile</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkIsEnableBettingProfile" runat="server" AutoPostBack="false"  />
             </div>
         </div>
         <div  id="trSubAgentEnable" runat="server" class="form-group">
             <label class="col-md-2 control-label">Sub-Agent Enable</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkSubAgentEnable" runat="server" AutoPostBack="false"  />
             </div>
         </div>
         <div id="trMaxCreditSetting" runat="server" class="form-group">
             <label class="col-md-2 control-label">Max Credit Limit</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkMaxCreditSetting" runat="server" AutoPostBack="false"  />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Enable Player Block</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkEnablePlayerBlock" runat="server" AutoPostBack="false"  />
             </div>
         </div>
         <div id="trCasino" runat="server" class="form-group">
             <label class="col-md-2 control-label">Casino</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkCasino" runat="server" AutoPostBack="false"  />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Template</label>
             <div class="col-md-4">
                 <wlb:CDropDownList ID="ddlTemplates" runat="server" CssClass="form-control" />
             </div>
         </div>
         <div id="trRequireChangePass" runat="server" visible="false" class="form-group">
             <label class="col-md-2 control-label">Require Change Password?</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkRequireChangePass" Font-Bold="true" runat="server" />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Betting Locked?</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkIsBettingLocked" Font-Bold="true" runat="server" />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Locked?</label>
             <div class="col-md-4">
                 <asp:CheckBox ID="chkIsLocked" Font-Bold="true" runat="server" />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label">Lock Reason</label>
             <div class="col-md-4">
                  <asp:TextBox CssClass="form-control" ID="txtLockReason" TextMode="MultiLine" MaxLength="100"
                  runat="server" />
             </div>
         </div>
         <div class="form-group">
             <label class="col-md-2 control-label"></label>
             <div class="col-md-1">
                 <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" />
             </div>
             <div class="col-md-4">
                 <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default" Text="Cancel" />
             </div>
         </div>
    </div>
</div>

