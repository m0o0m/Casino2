<%@ Page Language="VB" MasterPageFile="~/SBS/SuperAdmins/SuperAdmin.master"
    AutoEventWireup="false" CodeFile="PartnerChangePassword.aspx.vb" Inherits="SBCSuperAdmin.PartnerChangePassword" %>

<%@ Register Src="~/Inc/changePassword.ascx" TagName="changePassword" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <div style="width: 100%; margin: auto; padding-bottom: 10px;">
    <uc1:changePassword ID="ucChangePassword" runat="server" />
    </div>
</asp:Content>
