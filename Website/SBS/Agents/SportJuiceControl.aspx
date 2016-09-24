<%@ Page Title="" Language="VB" MasterPageFile="~/SBS/Agents/Agents.master" AutoEventWireup="false" CodeFile="SportJuiceControl.aspx.vb" Inherits="SBS_Agents_SportJuiceControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="Server">
    <div class="col-lg-12">
        <table class="table table-hover table-striped" id="sportJuices" agentid="<%=UserSession.UserID%>">
            <thead>
                <tr>
                    <th>Sport Type</th>
                    <th style="width: 110px;">Adjust Value</th>
                    <th>Bet Types</th>
                    <th>GM</th>
                    <th>1H</th>
                    <th>2H</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <%--<% For Each item As SportJuice In JuiceRules%>
                        <tr>
                            <td>
                                <select class="sport">
                                </select>
                            </td>
                            <td>
                                <input type="text" value="<%=item.Value%>" />
                            </td>
                            <td>
                                <select class="bettype">
                                </select>
                            </td>
                            <td >
                                <input type="checkbox" class="gm" />
                            </td>
                            <td>
                                <input type="checkbox" class="1h" />
                            </td>
                            <td>
                                <input type="checkbox" class="2h" />
                            </td>
                            <td>
                                <a href="#" class="btn btn-red btn-sm mlm mrm" onclick="sportjuicecontrol.deleteRule(this);"><i class="fa fa-trash-o"></i>&nbsp;Delete</a>
                            </td>
                        </tr>
                        <% Next%>--%>
            </tbody>
        </table>
        <div class="col-lg-12 text-right">
            <button type="button" class="btn btn-green btn-sm" onclick="sportjuicecontrol.addRule();">
                Add Rule</button>
        </div>
        <div class="col-lg-12">
            <div class="form-actions text-center pal">
                <button type="button" class="btn btn-danger" onclick="sportjuicecontrol.reloadPage();">
                    Cancel</button>
                <button type="button" class="btn btn-primary" onclick="sportjuicecontrol.save();">
                    Save</button>
            </div>
        </div>
    </div>

    <script type="text/javascript" src="/content/js/jquery-1.11.0.min.js"></script>
    <script type="text/javascript" src="/content/js/pages/sbsa.sportjuicecontrol.js?ver=1.1"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            sportjuicecontrol.init({
                tbJuiceId: '#sportJuices',
                saveURL: 'SportJuiceControl.aspx/SaveJuices',
                betTypes: JSON.parse('<%= JSS.Serialize(BetTypes)%>'),
                sports: JSON.parse('<%= JSS.Serialize(Sports)%>'),
                juices: JSON.parse('<%= JSS.Serialize(JuiceRules)%>'),
            });
        });
    </script>
</asp:Content>

