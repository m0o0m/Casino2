<%@ Control Language="VB" AutoEventWireup="true" CodeFile="dateTime.ascx.vb" Inherits="SBCWebsite.Inc_DateTime" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<div style="display: inline-block;">
    <asp:TextBox CssClass="form-control" ID="txtDate" runat="server" MaxLength="10" style="display: inline-block;" Width="105px"></asp:TextBox>
    <asp:ImageButton ID="ibtnCal" runat="server" ImageUrl="~/images/calendar.gif"
        ImageAlign="absBottom" OnClientClick="return false;" style="display: inline-block;" />
    <asp:DropDownList CssClass="form-control" ID="Hour" runat="server"  style="display: inline-block;" Width="60px">
        <asp:ListItem>01</asp:ListItem>
        <asp:ListItem>02</asp:ListItem>
        <asp:ListItem>03</asp:ListItem>
        <asp:ListItem>04</asp:ListItem>
        <asp:ListItem>05</asp:ListItem>
        <asp:ListItem>06</asp:ListItem>
        <asp:ListItem>07</asp:ListItem>
        <asp:ListItem>08</asp:ListItem>
        <asp:ListItem>09</asp:ListItem>
        <asp:ListItem>10</asp:ListItem>
        <asp:ListItem>11</asp:ListItem>
        <asp:ListItem Selected="true" Value="00">12</asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="lblColon" runat="server" Text=":"  style="display: inline-block;"></asp:Label>
    <asp:DropDownList CssClass="form-control" ID="Minute" runat="server"  style="display: inline-block;" Width="60px">
        <asp:ListItem Selected="true">00</asp:ListItem>
        <asp:ListItem>01</asp:ListItem>
        <asp:ListItem>02</asp:ListItem>
        <asp:ListItem>03</asp:ListItem>
        <asp:ListItem>04</asp:ListItem>
        <asp:ListItem>05</asp:ListItem>
        <asp:ListItem>06</asp:ListItem>
        <asp:ListItem>07</asp:ListItem>
        <asp:ListItem>08</asp:ListItem>
        <asp:ListItem>09</asp:ListItem>
        <asp:ListItem>10</asp:ListItem>
        <asp:ListItem>11</asp:ListItem>
        <asp:ListItem>12</asp:ListItem>
        <asp:ListItem>13</asp:ListItem>
        <asp:ListItem>14</asp:ListItem>
        <asp:ListItem>15</asp:ListItem>
        <asp:ListItem>16</asp:ListItem>
        <asp:ListItem>17</asp:ListItem>
        <asp:ListItem>18</asp:ListItem>
        <asp:ListItem>19</asp:ListItem>
        <asp:ListItem>20</asp:ListItem>
        <asp:ListItem>21</asp:ListItem>
        <asp:ListItem>22</asp:ListItem>
        <asp:ListItem>23</asp:ListItem>
        <asp:ListItem>24</asp:ListItem>
        <asp:ListItem>25</asp:ListItem>
        <asp:ListItem>26</asp:ListItem>
        <asp:ListItem>27</asp:ListItem>
        <asp:ListItem>28</asp:ListItem>
        <asp:ListItem>29</asp:ListItem>
        <asp:ListItem>30</asp:ListItem>
        <asp:ListItem>31</asp:ListItem>
        <asp:ListItem>32</asp:ListItem>
        <asp:ListItem>33</asp:ListItem>
        <asp:ListItem>34</asp:ListItem>
        <asp:ListItem>35</asp:ListItem>
        <asp:ListItem>36</asp:ListItem>
        <asp:ListItem>37</asp:ListItem>
        <asp:ListItem>38</asp:ListItem>
        <asp:ListItem>39</asp:ListItem>
        <asp:ListItem>40</asp:ListItem>
        <asp:ListItem>41</asp:ListItem>
        <asp:ListItem>42</asp:ListItem>
        <asp:ListItem>43</asp:ListItem>
        <asp:ListItem>44</asp:ListItem>
        <asp:ListItem>45</asp:ListItem>
        <asp:ListItem>46</asp:ListItem>
        <asp:ListItem>47</asp:ListItem>
        <asp:ListItem>48</asp:ListItem>
        <asp:ListItem>49</asp:ListItem>
        <asp:ListItem>50</asp:ListItem>
        <asp:ListItem>51</asp:ListItem>
        <asp:ListItem>52</asp:ListItem>
        <asp:ListItem>53</asp:ListItem>
        <asp:ListItem>54</asp:ListItem>
        <asp:ListItem>55</asp:ListItem>
        <asp:ListItem>56</asp:ListItem>
        <asp:ListItem>57</asp:ListItem>
        <asp:ListItem>58</asp:ListItem>
        <asp:ListItem>59</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList CssClass="form-control" ID="AMPM" runat="server" style="display: inline-block;" Width="65px">
        <asp:ListItem Selected="true">AM</asp:ListItem>
        <asp:ListItem>PM</asp:ListItem>
    </asp:DropDownList>
    <asp:CompareValidator ID="vdl" runat="server" Display="Dynamic" Type="Date" ControlToValidate="txtDate"
        Operator="DataTypeCheck" Text="*" style="display: inline-block;" />
    <cc1:MaskedEditExtender ID="MaskedEditDate" runat="server" TargetControlID="txtDate"
        Enabled="True" Mask="99/99/9999" MaskType="Date" ClearMaskOnLostFocus="true" />
    <asp:RegularExpressionValidator ID="rev_Date" runat="server" ControlToValidate="txtDate"
        Display="dynamic" ErrorMessage="" Visible="False" ValidationExpression="\d{2}/\d{2}/\d{4}" style="display: inline-block;">*
    </asp:RegularExpressionValidator>
    <cc1:CalendarExtender ID="ce" runat="server" TargetControlID="txtDate" PopupButtonID="txtDate" 
        PopupPosition="Right" />
</div>
