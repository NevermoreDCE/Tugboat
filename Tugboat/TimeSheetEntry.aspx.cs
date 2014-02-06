using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tugboat
{
    public partial class TimeSheetEntry : System.Web.UI.Page
    {
        TugboatDBEntities t;
        ClientWeek cw;
        protected void Page_Init(object sender, EventArgs e)
        {
            rptEmployeeSearchResults.ItemDataBound += new RepeaterItemEventHandler(rptEmployeeSearchResults_ItemDataBound);
            rptTimeSheet.ItemDataBound += new RepeaterItemEventHandler(rptTimeSheet_ItemDataBound);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            t = new TugboatDBEntities();
            if (Request.QueryString.AllKeys.Contains("ClientWeek"))
            {
                Guid g = Guid.Parse(Request.QueryString["ClientWeek"]);
                cw = t.ClientWeeks.Single(f => f.ClientWeekGuid == g);
            }
            else
                cw = new ClientWeek();

            if (!Page.IsPostBack)
            {
                initClients();
                initWeek();
                initSearch();
                if (Request.QueryString.AllKeys.Contains("ClientWeek"))
                    setupClientWeek(cw,true);
                
            }
        }

        #region Init
        private void initClients()
        {
            tdClientSearch.Visible = true;
            tdClientSelected.Visible = false;
            litClientName.Text = string.Empty;
            hidClientID.Value = string.Empty;
            ddlClients.Items.Clear();
            ListItem li = new ListItem("Select Client", "-1");
            ddlClients.Items.Add(li);
            foreach(Client c in t.Clients.OrderBy(f=>f.ClientName))
            {
                li = new ListItem(c.ClientName, c.ClientId.ToString());
                ddlClients.Items.Add(li);
            }
        }

        private void initWeek()
        {
            var tps = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            tbWeekEnding.Text = tps.ToString("MM/dd/yyyy");
            initWeekLabels(tps);
        }

        private void initWeekLabels(DateTime weekEnding)
        {
            litMondayLabel.Text = string.Format("Monday ({0})", weekEnding.AddDays(-6).ToString("MM/dd"));
            litTuesdayLabel.Text = string.Format("Tuesday ({0})", weekEnding.AddDays(-5).ToString("MM/dd"));
            litWednesdayLabel.Text = string.Format("Wednesday ({0})", weekEnding.AddDays(-4).ToString("MM/dd"));
            litThursdayLabel.Text = string.Format("Thursday ({0})", weekEnding.AddDays(-3).ToString("MM/dd"));
            litFridayLabel.Text = string.Format("Friday ({0})", weekEnding.AddDays(-2).ToString("MM/dd"));
            litSaturdayLabel.Text = string.Format("Saturday ({0})", weekEnding.AddDays(-1).ToString("MM/dd"));
            litSundayLabel.Text = string.Format("Sunday ({0})", weekEnding.ToString("MM/dd"));
        }

        private void initSearch()
        {
            tbEmployeeSearch.Text = string.Empty;
            tdEmployeeSearch.Visible = true;
            tdEmployeeSelected.Visible = false;
            rptEmployeeSearchResults.Visible = false;
            hidEmployeeID.Value = string.Empty;
            litEmployeeName.Text = string.Empty;
        }

        private void initTimes()
        {
            tbHoursMonday.Text = string.Empty;
            tbHoursTuesday.Text = string.Empty;
            tbHoursWednesday.Text = string.Empty;
            tbHoursThursday.Text = string.Empty;
            tbHoursFriday.Text = string.Empty;
            tbHoursSaturday.Text = string.Empty;
            tbHoursSunday.Text = string.Empty;
        }

        private void setupClientWeek(ClientWeek cw, bool checkStudent)
        {
            foreach (ListItem li in ddlClients.Items)
                if (li.Value == cw.ClientId.ToString())
                    li.Selected = true;
            litClientName.Text = cw.Client.ClientName;
            hidClientID.Value = cw.ClientId.ToString();
            tdClientSearch.Visible = false;
            tdClientSelected.Visible = true;
            tbWeekEnding.Text = cw.Week.WeekEnding.ToString("MM/dd/yyyy");
            setupSummary(cw);
            if (Request.QueryString.AllKeys.Contains("Student"))
                setupStudent(Request.QueryString["Student"], cw);
        }

        private void setupStudent(string studentGuid,ClientWeek clientWeek)
        {
            Guid g = Guid.Parse(studentGuid);
            Student s = t.Students.Single(f => f.StudentGuid == g);
            litEmployeeName.Text = string.Format("{0}, {1}", s.LastName, s.FirstName);
            hidEmployeeID.Value = s.StudentId.ToString();
            tdEmployeeSearch.Visible = false;
            tdEmployeeSelected.Visible = true;
            List<Time> studentWeek = t.Times.Where(f=>f.StudentId==s.StudentId&&f.ClientWeekId==clientWeek.ClientWeekId).ToList<Time>();
            if(studentWeek.Count>0)
            {
                tbHoursMonday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==1)).HoursWorked.ToString();
                tbHoursTuesday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==2)).HoursWorked.ToString();
                tbHoursWednesday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==3)).HoursWorked.ToString();
                tbHoursThursday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==4)).HoursWorked.ToString();
                tbHoursFriday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==5)).HoursWorked.ToString();
                tbHoursSaturday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==6)).HoursWorked.ToString();
                tbHoursSunday.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==7)).HoursWorked.ToString();
                tbPayRate.Text=(studentWeek.SingleOrDefault(f=>f.DayOfWeek==1)).PayRate.ToString();
            }

        }

        private void setupSummary(ClientWeek cw)
        {
            litHeaderMonday.Text = cw.Week.WeekEnding.AddDays(-6).ToString("MM/dd");
            litHeaderTuesday.Text = cw.Week.WeekEnding.AddDays(-5).ToString("MM/dd");
            litHeaderWednesday.Text = cw.Week.WeekEnding.AddDays(-4).ToString("MM/dd");
            litHeaderThursday.Text = cw.Week.WeekEnding.AddDays(-3).ToString("MM/dd");
            litHeaderFriday.Text = cw.Week.WeekEnding.AddDays(-2).ToString("MM/dd");
            litHeaderSaturday.Text = cw.Week.WeekEnding.AddDays(-1).ToString("MM/dd");
            litHeaderSunday.Text = cw.Week.WeekEnding.ToString("MM/dd");
            
            List<Result> dataset = new List<Result>();
            var stuff = from stu in t.Students
                        join mon in t.Times on new { key1 = stu.StudentId, key2 = 1, key3 = cw.ClientWeekId } equals new { key1 = mon.StudentId, key2 = mon.DayOfWeek.Value, key3 = mon.ClientWeekId }
                        join tue in t.Times on new { key1 = stu.StudentId, key2 = 2, key3 = cw.ClientWeekId } equals new { key1 = tue.StudentId, key2 = tue.DayOfWeek.Value, key3 = tue.ClientWeekId }
                        join wed in t.Times on new { key1 = stu.StudentId, key2 = 3, key3 = cw.ClientWeekId } equals new { key1 = wed.StudentId, key2 = wed.DayOfWeek.Value, key3 = wed.ClientWeekId }
                        join thur in t.Times on new { key1 = stu.StudentId, key2 = 4, key3 = cw.ClientWeekId } equals new { key1 = thur.StudentId, key2 = thur.DayOfWeek.Value, key3 = thur.ClientWeekId }
                        join fri in t.Times on new { key1 = stu.StudentId, key2 = 5, key3 = cw.ClientWeekId } equals new { key1 = fri.StudentId, key2 = fri.DayOfWeek.Value, key3 = fri.ClientWeekId }
                        join sat in t.Times on new { key1 = stu.StudentId, key2 = 6, key3 = cw.ClientWeekId } equals new { key1 = sat.StudentId, key2 = sat.DayOfWeek.Value, key3 = sat.ClientWeekId }
                        join sun in t.Times on new { key1 = stu.StudentId, key2 = 7, key3 = cw.ClientWeekId } equals new { key1 = sun.StudentId, key2 = sun.DayOfWeek.Value, key3 = sun.ClientWeekId }
                        select new {stu.StudentId, stu.StudentGuid, stu.LastName, stu.FirstName, stu.LastFour, mon.PayRate, monday = mon.HoursWorked, tuesday = tue.HoursWorked, wednesday = wed.HoursWorked, thursday = thur.HoursWorked, friday = fri.HoursWorked, saturday = sat.HoursWorked, sunday = sun.HoursWorked};
            foreach (var s in stuff)
                dataset.Add(new Result(s.StudentId, cw.ClientWeekId, s.StudentGuid, string.Format("{0}, {1}",s.LastName,s.FirstName), s.LastFour, s.PayRate, s.monday, s.tuesday, s.wednesday, s.thursday, s.friday, s.saturday, s.sunday));
            rptTimeSheet.DataSource = dataset.OrderBy(f=>f.EmployeeName);
            rptTimeSheet.DataBind();
                          
            
        }

        #endregion


        #region Events
        void rptEmployeeSearchResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Button b = (Button)e.Item.FindControl("btnSelectEmployee");
            Literal l = (Literal)e.Item.FindControl("litEmployeeName");
            Student s = (Student)e.Item.DataItem;
            l.Text = string.Format("{0}, {1}", s.LastName, s.FirstName);
            b.CommandName = s.StudentId.ToString();
        }

        void rptTimeSheet_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Literal litPayRate = (Literal)e.Item.FindControl("litPayRate");
            Literal litTSEmployeeName = (Literal)e.Item.FindControl("litTSEmployeeName");
            Literal litSSN = (Literal)e.Item.FindControl("litSSN");
            Literal litMon = (Literal)e.Item.FindControl("litMon");
            Literal litTue = (Literal)e.Item.FindControl("litTue");
            Literal litWed = (Literal)e.Item.FindControl("litWed");
            Literal litThur = (Literal)e.Item.FindControl("litThur");
            Literal litFri = (Literal)e.Item.FindControl("litFri");
            Literal litSat = (Literal)e.Item.FindControl("litSat");
            Literal litSun = (Literal)e.Item.FindControl("litSun");
            Literal litSumReg = (Literal)e.Item.FindControl("litSumReg");
            Literal litSumOT = (Literal)e.Item.FindControl("litSumOT");
            ImageButton ibDelRow = (ImageButton)e.Item.FindControl("ibDelRow");
            Result dataItem = (Result)e.Item.DataItem;
            litPayRate.Text = dataItem.PayRate.ToString();
            litTSEmployeeName.Text = string.Format("<a href=\"TimeSheetEntry.aspx?ClientWeek={0}&Student={1}\">{2}</a>", cw.ClientWeekGuid, dataItem.EmployeeGuid, dataItem.EmployeeName);
            litSSN.Text = dataItem.SSN;
            litMon.Text = dataItem.Monday.ToString();
            litTue.Text = dataItem.Tuesday.ToString();
            litWed.Text = dataItem.Wednesday.ToString();
            litThur.Text = dataItem.Thursday.ToString();
            litFri.Text = dataItem.Friday.ToString();
            litSat.Text = dataItem.Saturday.ToString();
            litSun.Text = dataItem.Sunday.ToString();
            litSumReg.Text = dataItem.TotalHours.ToString();
            litSumOT.Text = dataItem.OTHours.ToString();
            ibDelRow.CommandName = string.Format("{0}:{1}",dataItem.EmployeeId.ToString(),dataItem.ClientWeek.ToString());
        }

        protected void tbWeekEnding_TextChanged(object sender, EventArgs e)
        {
            initWeekLabels(DateTime.Parse(tbWeekEnding.Text));
        }

        protected void btnEmployeeSearch_Click(object sender, EventArgs e)
        {
            var result = t.Students.Where(f => f.LastName.Contains(tbEmployeeSearch.Text) || f.FirstName.Contains(tbEmployeeSearch.Text) || f.LastFour.Contains(tbEmployeeSearch.Text));
            rptEmployeeSearchResults.DataSource = result;
            rptEmployeeSearchResults.DataBind();
            rptEmployeeSearchResults.Visible = true;
        }

        protected void btnSelectEmployee_Command(object sender, CommandEventArgs e)
        {
            var EmployeeId = int.Parse(e.CommandName);
            tdEmployeeSearch.Visible = false;
            tbEmployeeSearch.Text = string.Empty;
            rptEmployeeSearchResults.Visible = false;
            tdEmployeeSelected.Visible = true;
            Student s = t.Students.SingleOrDefault(f => f.StudentId == EmployeeId);
            litEmployeeName.Text = string.Format("{0}, {1}", s.LastName, s.FirstName);
            hidEmployeeID.Value = s.StudentId.ToString();
            
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            DateTime weekEnding = DateTime.Parse(tbWeekEnding.Text);
            int studentId = int.Parse(hidEmployeeID.Value);
            int clientId = int.Parse(hidClientID.Value);
            Client cl;
            Week wk;

            if (cw.ClientId != clientId)
            {
                cl = t.Clients.Single(f => f.ClientId == clientId);
                wk = setupWeek(weekEnding);
                setupClientWeek(cl, wk);
                removeOldTimes(studentId, cw);
            }
            else if (Request.QueryString.AllKeys.Contains("ClientWeek"))
            {
                Guid cwGuid = Guid.Parse(Request.QueryString["ClientWeek"]);
                cw = t.ClientWeeks.Single(f => f.ClientWeekGuid == cwGuid);
                cl = cw.Client;
                wk = cw.Week;
            }
            else
            {
                wk = setupWeek(weekEnding);

                int selectedClientId = int.Parse(hidClientID.Value);
                cl = t.Clients.Single(f => f.ClientId == selectedClientId);

                setupClientWeek(cl, wk);
            }
            decimal payRate = 0.0m;
            decimal.TryParse(tbPayRate.Text, out payRate);

            Time mon = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 1);
            if (mon == default(Time))
            {
                mon = new Time();
                mon.ClientWeekId = cw.ClientWeekId;
                mon.CreatedBy = "davide";
                mon.ModifiedBy = "davide";
                mon.DateModified = DateTime.Now;
                mon.DateCreated = DateTime.Now;
                mon.StudentId = studentId;
                mon.TimeDate = weekEnding.AddDays(-6);
                mon.DayOfWeek = 1;
                mon.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(mon);
            }
            decimal hoursMonday = 0.0m;
            decimal.TryParse(tbHoursMonday.Text, out hoursMonday);
            mon.HoursWorked = hoursMonday;
            mon.PayRate = payRate;
            

            Time tue = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 2);
            if (tue == default(Time))
            {
                tue = new Time();
                tue.ClientWeekId = cw.ClientWeekId;
                tue.CreatedBy = "davide";
                tue.ModifiedBy = "davide";
                tue.DateModified = DateTime.Now;
                tue.DateCreated = DateTime.Now;
                tue.StudentId = studentId;
                tue.TimeDate = weekEnding.AddDays(-5);
                tue.DayOfWeek = 2;
                tue.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(tue);
            }
            decimal hoursTuesday = 0.0m;
            decimal.TryParse(tbHoursTuesday.Text, out hoursTuesday);
            tue.HoursWorked = hoursTuesday;
            tue.PayRate = payRate;

            Time wed = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 3);
            if (wed == default(Time))
            {
                wed = new Time();
                wed.ClientWeekId = cw.ClientWeekId;
                wed.CreatedBy = "davide";
                wed.ModifiedBy = "davide";
                wed.DateModified = DateTime.Now;
                wed.DateCreated = DateTime.Now;
                wed.StudentId = studentId;
                wed.TimeDate = weekEnding.AddDays(-4);
                wed.DayOfWeek = 3;
                wed.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(wed);
            }
            decimal hoursWednesday = 0.0m;
            decimal.TryParse(tbHoursWednesday.Text, out hoursWednesday);
            wed.HoursWorked = hoursWednesday;
            wed.PayRate = payRate;

            Time thur = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 4);
            if (thur == default(Time))
            {
                thur = new Time();
                thur.ClientWeekId = cw.ClientWeekId;
                thur.CreatedBy = "davide";
                thur.ModifiedBy = "davide";
                thur.DateModified = DateTime.Now;
                thur.DateCreated = DateTime.Now;
                thur.StudentId = studentId;
                thur.TimeDate = weekEnding.AddDays(-3);
                thur.DayOfWeek = 4;
                thur.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(thur);
            }
            decimal hoursThursday = 0.0m;
            decimal.TryParse(tbHoursThursday.Text, out hoursThursday);
            thur.HoursWorked = hoursThursday;
            thur.PayRate = payRate;

            Time fri = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 5);
            if (fri == default(Time))
            {
                fri = new Time();
                fri.ClientWeekId = cw.ClientWeekId;
                fri.CreatedBy = "davide";
                fri.ModifiedBy = "davide";
                fri.DateModified = DateTime.Now;
                fri.DateCreated = DateTime.Now;
                fri.StudentId = studentId;
                fri.TimeDate = weekEnding.AddDays(-2);
                fri.DayOfWeek = 5;
                fri.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(fri);
            }
            decimal hoursFriday = 0.0m;
            decimal.TryParse(tbHoursFriday.Text, out hoursFriday);
            fri.HoursWorked = hoursFriday;
            fri.PayRate = payRate;

            Time sat = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 6);
            if (sat == default(Time))
            {
                sat = new Time();
                sat.ClientWeekId = cw.ClientWeekId;
                sat.CreatedBy = "davide";
                sat.ModifiedBy = "davide";
                sat.DateModified = DateTime.Now;
                sat.DateCreated = DateTime.Now;
                sat.StudentId = studentId;
                sat.TimeDate = weekEnding.AddDays(-1);
                sat.DayOfWeek = 6;
                sat.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(sat);
            }
            decimal hoursSaturday = 0.0m;
            decimal.TryParse(tbHoursSaturday.Text, out hoursSaturday);
            sat.HoursWorked = hoursSaturday;
            sat.PayRate = payRate;

            Time sun = t.Times.SingleOrDefault(f => f.ClientWeekId == cw.ClientWeekId && f.StudentId == studentId && f.DayOfWeek == 7);
            if (sun == default(Time))
            {
                sun = new Time();
                sun.ClientWeekId = cw.ClientWeekId;
                sun.CreatedBy = "davide";
                sun.ModifiedBy = "davide";
                sun.DateModified = DateTime.Now;
                sun.DateCreated = DateTime.Now;
                sun.StudentId = studentId;
                sun.TimeDate = weekEnding;
                sun.DayOfWeek = 7;
                sun.TimeGuid = Guid.NewGuid();
                t.Times.AddObject(sun);
            }
            decimal hoursSunday = 0.0m;
            decimal.TryParse(tbHoursSunday.Text, out hoursSunday);
            sun.HoursWorked = hoursSunday;
            sun.PayRate = payRate;

            t.SaveChanges();

            initSearch();
            initTimes();

            Response.Redirect(string.Format("TimeSheetEntry.aspx?ClientWeek={0}", cw.ClientWeekGuid),false);

        }

        private void removeOldTimes(int employeeId, ClientWeek clientWeek)
        {
            List<Time> forDeletion = t.Times.Where(f => f.StudentId == employeeId && f.ClientWeekId == clientWeek.ClientWeekId).ToList<Time>();
            foreach (var del in forDeletion)
                t.DeleteObject(del);
        }

        private Week setupWeek(DateTime weekEnding)
        {
            Week wk;
            wk = t.Weeks.SingleOrDefault(f => f.WeekEnding == weekEnding);
            if (wk == default(Week))
            {
                wk = new Week();
                wk.WeekEnding = DateTime.Parse(tbWeekEnding.Text);
                wk.CreatedBy = "davide";
                wk.ModifiedBy = "davide";
                wk.DateModified = DateTime.Now;
                wk.DateCreated = DateTime.Now;
                wk.WeekGuid = Guid.NewGuid();
                t.Weeks.AddObject(wk);
            }
            return wk;
        }

        private void setupClientWeek(Client cl, Week wk)
        {
            cw = t.ClientWeeks.SingleOrDefault(f => f.ClientId == cl.ClientId && f.WeekId == wk.WeekId);
            if (cw == default(ClientWeek))
            {
                cw = new ClientWeek();
                cw.ClientId = cl.ClientId;
                cw.WeekId = wk.WeekId;
                cw.CreatedBy = "davide";
                cw.ModifiedBy = "davide";
                cw.DateModified = DateTime.Now;
                cw.DateCreated = DateTime.Now;
                cw.ClientWeekGuid = Guid.NewGuid();
                t.ClientWeeks.AddObject(cw);
            }
            setupClientWeek(cw, false);
        }

        protected void ibChangeEmployee_Click(object sender, EventArgs e)
        {
            initSearch();
        }

        protected void ibDelRow_Command(object sender, CommandEventArgs e)
        {
            string[] split = e.CommandName.Split(':');
            int employeeId = int.Parse(split[0]);
            int clientWeekId = int.Parse(split[1]);
            List<Time> forDeletion = t.Times.Where(f => f.ClientWeekId == clientWeekId && f.StudentId == employeeId).ToList<Time>();
            foreach (Time del in forDeletion)
                t.Times.DeleteObject(del);
            t.SaveChanges();
        }

        protected void ddlClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            int clientId = int.Parse(ddlClients.SelectedItem.Value);
            if (clientId > 0)
            {
                hidClientID.Value = clientId.ToString();
                Client c = t.Clients.Single(f => f.ClientId == clientId);
                litClientName.Text = c.ClientName;
                tdClientSearch.Visible = false;
                tdClientSelected.Visible = true;
            }
        }

        protected void ibChangeClient_Click(object sender, EventArgs e)
        {
            initClients();
        }
        #endregion

    }

    public class Result
    {
        public decimal PayRate { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public int ClientWeek { get; set; }
        public Guid EmployeeGuid { get; set; }
        public string SSN { get; set; }
        public decimal Monday { get; set; }
        public decimal Tuesday { get; set; }
        public decimal Wednesday { get; set; }
        public decimal Thursday { get; set; }
        public decimal Friday { get; set; }
        public decimal Saturday { get; set; }
        public decimal Sunday { get; set; }
        public decimal TotalHours
        {
            get
            {
                decimal sum = 0.0m;
                sum = Monday + Tuesday + Wednesday + Thursday + Friday + Saturday + Sunday;
                if (sum > 40.0m)
                    return 40.0m;
                else
                    return sum;
            }
        }
        public decimal OTHours
        {
            get
            {
                decimal sum = 0.0m;
                sum = Monday + Tuesday + Wednesday + Thursday + Friday + Saturday + Sunday;
                if (sum > 40.0m)
                    return sum - 40.0m;
                else
                    return 0.0m;
            }
        }

        public Result(int employeeId, int clientWeek, Guid employeeGuid, string employeeName, string ssn, decimal payRate, decimal monday, decimal tuesday, decimal wednesday, decimal thursday, decimal friday, decimal saturday, decimal sunday)
        {
            EmployeeId = employeeId;
            EmployeeGuid = employeeGuid;
            EmployeeName = employeeName;
            SSN = ssn;
            PayRate = payRate;
            Monday = monday;
            Tuesday = tuesday;
            Wednesday = wednesday;
            Thursday = thursday;
            Friday = friday;
            Saturday = saturday;
            Sunday = sunday;
            ClientWeek = clientWeek;
        }
    }
}