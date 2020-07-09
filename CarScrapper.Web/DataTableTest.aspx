<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTableTest.aspx.cs" Inherits="CarScrapper.Web.DataTableTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.0/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js"></script>
    <link href="https://cdn.datatables.net/1.10.20/css/jquery.dataTables.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        th, #tbGrid_length, #tbGrid_filter, #tbGrid_info
        { 
            font-size: 12px; font-family:Arial; 
        }
        
        td { 
            font-size: 11px; font-family:Arial; 
        }
        
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
        $(document).ready(function () {

            $("#btnSearch").click(function () {
                StartSearch();
            });

        });

        function StartSearch() {
            var tMake = $("#<%=tbMake.ClientID%>").val();
            var tModel = $("#<%=tbModel.ClientID%>").val();
            var tData = { make: tMake, model: tModel, dealerType: "DealerCom", isLoaner: "false" }; //TODO: wire type and isLoaner parameters

            $.ajax({
                url: 'https://api-carscraper.azurewebsites.net/api/startsearch',
                type: 'post',
                data: JSON.stringify(tData),
                dataType: 'json',
                contentType: "application/json"
            }).done(function (data) {
                PollResultEndpoint(data.searchKey, data.retryAfter, data.keyRetrievalEndpoint);
            }).fail(function () {
                alert("error");
            });
        };

        function PollResultEndpoint(searchKey, retryAfter, endpoint) {
            $.getJSON(endpoint + "?searchKey=" + searchKey,
                {
                    format: "json"
                })
                .done(function (json) {
                    var i = false;
                    //continue polling, results arent ready yet
                    if (json.status == "SearchInProgress") {
                        setTimeout(PollResultEndpoint, 10000, searchKey, retryAfter, endpoint); //10 sec, implement retryAfter logic 
                    }
                    else {
                        if (json.results) {
                            $("#tbGrid").DataTable(
                                {
                                    destroy: true,
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
                        }
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    alert("fail")
                });
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divMain" style="width: 500px">
            <table id="tbHeaderTable" border="0" cellpadding="2">
                <tr>
                    <td>Make:</td>
                    <td><asp:TextBox runat="server" ID="tbMake" AutoPostBack="false"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Model:</td>
                    <td>
                        <asp:TextBox runat="server" ID="tbModel" AutoPostBack="false"></asp:TextBox>
                        <%--<asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" />--%>
                        <input type="button" id="btnSearch" name="btnSearch" value="Search" />
                    </td>
                </tr>
            </table>
            <br /><br />
            <table id="tbGrid" class="display compact" style="width:100%">
                <thead>
                    <tr>
                        <th>Make</th>
                        <th class="col-200">Model</th>
                        <th>ExteriorColor</th>
                        <th>InteriorColor</th>
                        <th>MSRP</th>
                        <th>Engine</th>
                        <th>DriveType</th>
                        <th>StockNumber</th>
                        <th>VIN</th>
                        <th>URL</th>
                        <th>BodyStyle</th>
                        <th>ModelCode</th>
                        <th>Transmission</th>
                        <th>DealerName</th>
                        <th>IPacket</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
