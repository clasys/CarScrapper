using CarScrapper.Core;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CarScrapper.Web
{
    public partial class _Default : Page
    {
        private const string SORT_KEY= "__sort_dir";
        private const string RESULTS_KEY = "__results";
        private DateTime _startTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            _startTime = DateTime.Now;

            if (!IsPostBack)
            {
                ViewState[SORT_KEY] = "ASC";
                var dealers = Util.GetDealers();
                dealers.OrderBy(a => a.Name).Select(a => a.Name).Distinct().ForEach(a => { tbDealerList.Text += "\n" + a; });
                dealers.OrderBy(a => a.Make).Select(a => a.Make).Distinct().ForEach(a => { tbMakesList.Text += "\n" + a; });
            }
        }

        private void BindGrid()
        {
            var results = (IList<CarInfo>)ViewState[RESULTS_KEY];
            grid1.DataSource = results;
            grid1.DataBind();
            lblCount.Text = String.Format("{0} ({1} s)", results.Count().ToString(), Math.Round((DateTime.Now - _startTime).TotalSeconds, 2));

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbMake.Text) &&
                !string.IsNullOrEmpty(tbModel.Text))
            {
                var results = new List<CarInfo>();
                Processor processor = null;
                var make = tbMake.Text.Substring(0, 1).ToUpper() + tbMake.Text.Substring(1, tbMake.Text.Length - 1);
                var model = tbModel.Text.Substring(0, 1).ToUpper() + tbModel.Text.Substring(1, tbModel.Text.Length - 1).Replace(" ", "+");


                if (cbDealerOn.Checked)
                {
                    var prefs = new ProcessingPreferences(new DealerOnSelector(make, model, ddlInventoryType.SelectedValue == "1" ? InventoryType.New : InventoryType.Loaner));
                    processor = new Processor(prefs);
                    processor.Scrap().ForEach(a=> { results.Add(a); });
                }

                if (cbDealerInspire.Checked)
                {
                    var prefs = new ProcessingPreferences(new DealerInspireSelector(make, model, ddlInventoryType.SelectedValue == "1" ? InventoryType.New : InventoryType.Loaner));
                    processor = new Processor(prefs);
                    processor.Scrap().ForEach(a => { results.Add(a); });
                }

                if (cbDealerCom.Checked)
                {
                    var prefs = new ProcessingPreferences(new DealerComSelector(make, model, ddlInventoryType.SelectedValue == "1" ? InventoryType.New : InventoryType.Loaner));
                    processor = new Processor(prefs);
                    processor.Scrap().ForEach(a => { results.Add(a); });
                }

                ViewState[RESULTS_KEY] = results;
                BindGrid();
            }
        }

        protected void grid1_PageIndexChanging(object sender, EventArgs e)
        { }

        protected void grid1_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView grid = (GridView)sender;

            var data = (IList<CarInfo>)ViewState[RESULTS_KEY];
            var sortDir = (string)ViewState[SORT_KEY];

            if (data == null || sortDir == null)
                e.Cancel = true;

            if (sortDir == "ASC")
            {
                switch (e.SortExpression)
                {
                    case "Make":
                        grid.DataSource = data.OrderBy(a => a.Make);
                        break;
                    case "InteriorColor":
                        grid.DataSource = data.OrderBy(a => a.InteriorColor);
                        break;
                    case "BodyStyle":
                        grid.DataSource = data.OrderBy(a => a.BodyStyle);
                        break;
                    case "DealerName":
                        grid.DataSource = data.OrderBy(a => a.DealerName);
                        break;
                    case "DriveType":
                        grid.DataSource = data.OrderBy(a => a.DriveType);
                        break;
                    case "Engine":
                        grid.DataSource = data.OrderBy(a => a.Engine);
                        break;
                    case "ExteriorColor":
                        grid.DataSource = data.OrderBy(a => a.ExteriorColor);
                        break;
                    case "Model":
                        grid.DataSource = data.OrderBy(a => a.Model);
                        break;
                    case "ModelCode":
                        grid.DataSource = data.OrderBy(a => a.ModelCode);
                        break;
                    case "MSRP":
                        grid.DataSource = data.OrderBy(a => a.MSRP);
                        break;
                    case "StockNumber":
                        grid.DataSource = data.OrderBy(a => a.StockNumber);
                        break;
                    case "Transmission":
                        grid.DataSource = data.OrderBy(a => a.Transmission);
                        break;
                    case "VIN":
                        grid.DataSource = data.OrderBy(a => a.VIN);
                        break;
                    case "WebSite":
                        grid.DataSource = data.OrderBy(a => a.WebSite);
                        break;
                    case "URL":
                        grid.DataSource = data.OrderBy(a => a.URL);
                        break;
                    default:
                        grid.DataSource = data.OrderBy(a => a.DealerName);
                        break;
                }

                ViewState[SORT_KEY] = "DESC";
            }
            else
            {
                switch (e.SortExpression)
                {
                    case "Make":
                        grid.DataSource = data.OrderByDescending(a => a.Make);
                        break;
                    case "InteriorColor":
                        grid.DataSource = data.OrderByDescending(a => a.InteriorColor);
                        break;
                    case "BodyStyle":
                        grid.DataSource = data.OrderByDescending(a => a.BodyStyle);
                        break;
                    case "DealerName":
                        grid.DataSource = data.OrderByDescending(a => a.DealerName);
                        break;
                    case "DriveType":
                        grid.DataSource = data.OrderByDescending(a => a.DriveType);
                        break;
                    case "Engine":
                        grid.DataSource = data.OrderByDescending(a => a.Engine);
                        break;
                    case "ExteriorColor":
                        grid.DataSource = data.OrderByDescending(a => a.ExteriorColor);
                        break;
                    case "Model":
                        grid.DataSource = data.OrderByDescending(a => a.Model);
                        break;
                    case "ModelCode":
                        grid.DataSource = data.OrderByDescending(a => a.ModelCode);
                        break;
                    case "MSRP":
                        grid.DataSource = data.OrderByDescending(a => a.MSRP);
                        break;
                    case "StockNumber":
                        grid.DataSource = data.OrderByDescending(a => a.StockNumber);
                        break;
                    case "Transmission":
                        grid.DataSource = data.OrderByDescending(a => a.Transmission);
                        break;
                    case "VIN":
                        grid.DataSource = data.OrderByDescending(a => a.VIN);
                        break;
                    case "WebSite":
                        grid.DataSource = data.OrderByDescending(a => a.WebSite);
                        break;
                    case "URL":
                        grid.DataSource = data.OrderByDescending(a => a.URL);
                        break;
                    default:
                        grid.DataSource = data.OrderByDescending(a => a.DealerName);
                        break;
                }

                ViewState[SORT_KEY] = "ASC";
            }

            grid.DataBind();
        }

        protected void grid1_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            foreach (var row in grid.Rows)
            {
                if (row is GridViewRow)
                {
                    var cells = ((GridViewRow)row).Cells;

                    if (cells.Count > 0)
                    {
                        cells[0].Text = string.Format("<a class='gridLink' target='_blank' href='{0}'>{0}</a>", cells[0].Text);
                        cells[10].Text = string.Format("<a class='gridLink' target='_blank' href='{0}'>{0}</a>", cells[10].Text);
                    }
                }
            }
        }

        protected void grid1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //remove last IsLoaner column, until we bind columns explicitly
            e.Row.Cells[e.Row.Cells.Count - 1].Visible = false;
        }
    }
}