<%@ Control Language="VB" AutoEventWireup="false" CodeFile="_breadCrumbLayout4.ascx.vb" Inherits="SBS_Players_Layout_Layout4_breadCrumbLayout4" %>
<div id="title-breadcrumb-option-demo" class="page-title-breadcrumb">
    <div class="page-header pull-left">
        <div class="page-title">
            <a href="/SBS/Players/Default.aspx?bettype=BetTheBoard">Figures</a> 
            <%--| <a href="/SBS/Players/Casino.aspx?bettype=casino">CASINO</a>--%>
        </div>
    </div>
    <ol class="breadcrumb page-breadcrumb pull-right">
        <li>
            <a style="font-size: 17px; font-weight: bold;">
                Available Balance : <asp:Literal ID="lblAvailableBalance" runat="server" />
            </a>
        </li>
    </ol>
    <div class="clearfix">
    </div>
</div>
