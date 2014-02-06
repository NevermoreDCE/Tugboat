using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tugboat
{
    public partial class TimeSheetList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Events

        protected void lbAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("TimeSheetEntry.aspx");
        }

        #endregion
    }
}