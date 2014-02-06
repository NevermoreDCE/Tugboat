<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TimeSheetEntry.aspx.cs" Inherits="Tugboat.TimeSheetEntry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
.formLabel {font-weight:bold;}
.formItem {}
.sectionHeader{font-weight:bold;font-size:1.2em;color:Black}

#tblTimeSheet tbody td {border: 1px solid black;}
.noBorder {border:0px !important}
#tblTimeSheet thead tr td{vertical-align:bottom; font-weight:bold; text-align:center; }

</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Add Timesheet Records</h1>
<asp:ScriptManager EnableViewState="true" runat="server" />
<asp:UpdatePanel ID="uplForm" runat="server" UpdateMode="Always" >
<ContentTemplate>
<table id="tblMain" runat="server">
    <tr><td class="formLabel">Client</td>
    <td id="tdClientSearch" runat="server" class="formItem"><asp:DropDownList ID="ddlClients" runat="server" OnSelectedIndexChanged="ddlClients_SelectedIndexChanged" AutoPostBack="true" /></td>
    <td id="tdClientSelected" class="formItem" runat="server" visible="false"><asp:Literal ID="litClientName" runat="server" /><asp:HiddenField ID="hidClientID" runat="server" /><asp:ImageButton ID="ibChangeClient" runat="server" ImageUrl="Images/delete2.png" AlternateText="Change Selected Client" OnClick="ibChangeClient_Click" /></td>
    </tr>

    <tr><td class="formLabel">Week Ending</td>
    <td class="formItem"><asp:TextBox ID="tbWeekEnding" runat="server" OnTextChanged="tbWeekEnding_TextChanged" AutoPostBack="true" /></td>
    </tr>

    <tr><td colspan="2" class="sectionHeader">Employee Info</td></tr>
    <tr><td class="formLabel">Employee</td>
    <td id="tdEmployeeSearch" runat="server" class="formItem">
        <asp:TextBox ID="tbEmployeeSearch" runat="server" /><asp:Button ID="btnEmployeeSearch" runat="server" Text="Search" OnClick="btnEmployeeSearch_Click" /><br/>
        <asp:Repeater ID="rptEmployeeSearchResults" runat="server">
            <ItemTemplate>
            <div><asp:Button ID="btnSelectEmployee" runat="server" Text="Select" OnCommand="btnSelectEmployee_Command" /><asp:Literal ID="litEmployeeName" runat="server" /></div>
            </ItemTemplate>
        </asp:Repeater>
    </td>
    <td id="tdEmployeeSelected" runat="server" visible="false" class="formItem"><asp:Literal ID="litEmployeeName" runat="server" /><asp:HiddenField ID="hidEmployeeID" runat="server" /><asp:ImageButton ID="ibChangeEmployee" runat="server" ImageUrl="Images/delete2.png" OnClick="ibChangeEmployee_Click" AlternateText="Change Selected Employee" /></td>
    </tr>

    <tr><td class="formLabel">Pay Rate</td>
    <td class="formItem"><asp:TextBox ID="tbPayRate" runat="server" /></td>
    </tr>

    <tr><td colspan="2" class="sectionHeader">Hours Worked</td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litMondayLabel" runat="server" Text="Monday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursMonday" runat="server" /></td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litTuesdayLabel" runat="server" Text="Tuesday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursTuesday" runat="server" /></td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litWednesdayLabel" runat="server" Text="Wednesday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursWednesday" runat="server" /></td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litThursdayLabel" runat="server" Text="Thursday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursThursday" runat="server" /></td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litFridayLabel" runat="server" Text="Friday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursFriday" runat="server" /></td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litSaturdayLabel" runat="server" Text="Saturday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursSaturday" runat="server" /></td></tr>

    <tr><td class="formLabel"><asp:Literal ID="litSundayLabel" runat="server" Text="Sunday" /></td>
    <td class="formItem"><asp:TextBox ID="tbHoursSunday" runat="server" /></td></tr>


</table>

<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"/>

<hr />
<table id="tblTimeSheet" class="summaryTable">
<thead>
<td style="width:65px;">Pay Rate</td>
<td style="width:220px;">Employee Name</td>
<td style="width:45px;">SSN#</td>
<td style="width:45px;"><asp:Literal ID="litHeaderMonday" runat="server" /><br />MON</td>
<td style="width:45px;"><asp:Literal ID="litHeaderTuesday" runat="server" /><br />TUE</td>
<td style="width:45px;"><asp:Literal ID="litHeaderWednesday" runat="server" /><br />WED</td>
<td style="width:45px;"><asp:Literal ID="litHeaderThursday" runat="server" /><br />THUR</td>
<td style="width:45px;"><asp:Literal ID="litHeaderFriday" runat="server" /><br />FRI</td>
<td style="width:45px;"><asp:Literal ID="litHeaderSaturday" runat="server" /><br />SAT</td>
<td style="width:45px;"><asp:Literal ID="litHeaderSunday" runat="server" /><br />SUN</td>
<td style="width:65px;">Total<br />Regular<br />Hours</td>
<td style="width:65px;">Total<br />OT<br />Hours</td>
<td style="width:35px;" class="noBorder"></td>
</thead>
<asp:Repeater ID="rptTimeSheet" runat="server">
<ItemTemplate>
<tr>
<td style="text-align:right;"><asp:Literal ID="litPayRate" runat="server" /></td>
<td><asp:Literal ID="litTSEmployeeName" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litSSN" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litMon" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litTue" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litWed" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litThur" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litFri" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litSat" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litSun" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litSumReg" runat="server" /></td>
<td style="text-align:right;"><asp:Literal ID="litSumOT" runat="server" /></td>
<td style="text-align:center;" class="noBorder"><asp:ImageButton ID="ibDelRow" runat="server" OnCommand="ibDelRow_Command" ImageUrl="/Images/row_delete.png" AlternateText="Delete This Timecard" /></td>
</tr>
</ItemTemplate>
</asp:Repeater>
</table>

</ContentTemplate>
</asp:UpdatePanel>



</asp:Content>
