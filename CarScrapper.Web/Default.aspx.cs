using CarScrapper.Core;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ViewState[SORT_KEY] = "ASC";
        }

        private void BindGrid()
        {
            var results = (IList<CarInfo>)ViewState[RESULTS_KEY];
            grid1.DataSource = results;
            grid1.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbMake.Text) &&
                !string.IsNullOrEmpty(tbModel.Text))
            {
                var prefs = new ProcessingPreferences(new DealerOnSelector(tbMake.Text, tbModel.Text));
                var processor = new Processor(prefs);
                var results = processor.Scrap();
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
    }
}