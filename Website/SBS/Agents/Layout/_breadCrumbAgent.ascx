<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_breadCrumbAgent.ascx.vb" Inherits="SBS_Agents_Layout_breadCrumbAgent" %>
<div id="title-breadcrumb-option-demo" class="page-title-breadcrumb">
    <div class="page-header pull-left">
        <div class="page-title">
            Current Balance:
            <asp:Label ForeColor="#44c789" runat="server" ID="lblAgentBalance" />.
            Rental Balance: 
            <asp:Label ForeColor="#bf4346" runat="server" ID="ltrRentalBalance" />
        </div>
    </div>
    <ol class="breadcrumb page-breadcrumb pull-right">
        <li>
            <a href="javascript:window.print();"></a>
        </li>
    </ol>
    <div class="clearfix">
    </div>
</div>
