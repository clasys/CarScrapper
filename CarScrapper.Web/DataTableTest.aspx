<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTableTest.aspx.cs" Inherits="CarScrapper.Web.DataTableTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.0/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js"></script>
    <link href="https://cdn.datatables.net/1.10.20/css/jquery.dataTables.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        .col-200 {
            width: 200px !important;
        }

        .col-100 {
            width: 100px !important;
        }

        table#gvResults{
            table-layout:fixed !important;
        }
    </style>

    <script type="text/javascript">
        $(function () {
            $.getJSON("https://api-carscraper.azurewebsites.net/api/getresults?searchKey=2429a559-1249-41f8-b360-fde517ddf1fe",
                {
                    format: "json"
                })
                .done(function (json) {
                    //alert("Success")

                    $("[id*=gvResults]").DataTable(
                        {
                            bLengthChange: true,
                            lengthMenu: [[10, 50, -1], [10, 50, "All"]],
                            theme: 'energyblue',
                            bFilter: true,
                            bSort: true,
                            bPaginate: true,
                            data: json.results,
                            fixedColums: true,
                            columns: [
                                { 'data': 'make' },
                                { 'data': 'model' },
                                { 'data': 'exteriorColor' },
                                { 'data': 'interiorColor' },
                                { 'data': 'msrp' },
                                { 'data': 'engine' },
                                { 'data': 'driveType' },
                                { 'data': 'stockNumber' },
                                { 'data': 'vin' },
                                { 'data': 'url' },
                                { 'data': 'bodyStyle' },
                                { 'data': 'modelCode' },
                                { 'data': 'transmission' },
                                { 'data': 'dealerName' },
                                { 'data': 'iPacket' }
                            ]
                        })
                })
                .fail(function (jqxhr, textStatus, error) {
                    alert("fail")
                });


        });

        //function OnSuccess(response) {
        //    $("[id*=gvResults]").DataTable(
        //    {
        //        bLengthChange: true,
        //        lengthMenu: [[5, 10, -1], [5, 10, "All"]],
        //        bFilter: true,
        //        bSort: true,
        //        bPaginate: true,
        //        data: response.d,
        //        columns: [{ 'data': 'CustomerID' },
        //                  { 'data': 'ContactName' },
        //                  { 'data': 'City' },
        //                  { 'data': 'Country'}]
        //    });
        //};
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 500px">
            <asp:GridView ID="gvResults" runat="server" CssClass="stripe row-border order-column compact" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="Make" HeaderText="Make" />
                    <asp:BoundField DataField="Model" HeaderText="Model" HeaderStyle-CssClass="col-200" />
                    <asp:BoundField DataField="ExteriorColor" HeaderText="ExteriorColor" />
                    <asp:BoundField DataField="InteriorColor" HeaderText="Interior Color" />
                    <asp:BoundField DataField="MSRP" HeaderText="MSRP" />
                    <asp:BoundField DataField="Engine" HeaderText="Engine" />
                    <asp:BoundField DataField="DriveType" HeaderText="Drive Type" />
                    <asp:BoundField DataField="StockNumber" HeaderText="Stock #" />
                    <asp:BoundField DataField="VIN" HeaderText="VIN" />
                    <asp:BoundField DataField="URL" HeaderText="URL" />
                    <asp:BoundField DataField="BodyStyle" HeaderText="Body Style" HeaderStyle-CssClass="col-100" />
                    <asp:BoundField DataField="ModelCode" HeaderText="Model Code" />
                    <asp:BoundField DataField="Transmission" HeaderText="Transmission" />
                    <asp:BoundField DataField="DealerName" HeaderText="Dealer Name" HeaderStyle-CssClass="col-100" />
                    <asp:BoundField DataField="IPacket" HeaderText="IPacket" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
