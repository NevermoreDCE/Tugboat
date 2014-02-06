<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TimeSheetList.aspx.cs" Inherits="Tugboat.TimeSheetList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Timesheets</h1>
<asp:LinkButton ID="lbAddNew" runat="server" Text="Add New Timesheet" OnClick="lbAddNew_Click" /><br />
<asp:GridView ID="gvTimesheets" runat="server" AllowPaging="true" PageSize="25">
    <Columns>
        <asp:HyperLinkField HeaderText="Company" DataTextField="CompanyName" DataNavigateUrlFields="CompanyWeekGuid" DataNavigateUrlFormatString="TimeSheetEntry.aspx?CompanyWeek={0}" SortExpression="CompanyName" />
        <asp:BoundField HeaderText="Week Ending" DataField="WeekEnding" SortExpression="WeekEnding" />
        <asp:BoundField HeaderText="Number of Employees" DataField="NumberOfEmployees" />
        <asp:BoundField HeaderText="Total Regular Hours" DataField="TotalRegularHours" />
        <asp:BoundField HeaderText="Total O.T. Hours" DataField="TotalOTHours" />
        <asp:CommandField HeaderText="Delete" ButtonType="Image" InsertImageUrl
    </Columns>
</asp:GridView>

</asp:Content>
