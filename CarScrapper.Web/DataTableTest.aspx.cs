using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CarScrapper.Web
{
    public partial class DataTableTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Make");
                dt.Columns.Add("Model");

                dt.Columns.Add("ExteriorColor");
                dt.Columns.Add("InteriorColor");
                dt.Columns.Add("MSRP");
                dt.Columns.Add("Engine");
                dt.Columns.Add("DriveType");
                dt.Columns.Add("StockNumber");
                dt.Columns.Add("VIN");
                dt.Columns.Add("URL");
                dt.Columns.Add("BodyStyle");
                dt.Columns.Add("ModelCode");
                dt.Columns.Add("Transmission");
                dt.Columns.Add("DealerName");
                dt.Columns.Add("IPacket");
                dt.Rows.Add();
                gvResults.DataSource = dt;
                gvResults.DataBind();

                //Required for jQuery DataTables to work.
                gvResults.UseAccessibleHeader = true;
                gvResults.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}