<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CarScrapper.Web.DataTableTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.0/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/gasparesganga-jquery-loading-overlay@2.1.7/dist/loadingoverlay.min.js"></script>
    <link href="https://cdn.datatables.net/1.10.20/css/jquery.dataTables.css" rel="stylesheet" type="text/css" />

    

    <style type="text/css">
        #tbGrid_length, #tbGrid_filter, #tbGrid_info, #tbGrid_paginate, p {
            font-size: 13px; font-family:Arial; 
        }
        
        th
        { 
            font-size: 12px; font-family:Arial; 
        }
        
        td { 
            font-size: 11px; font-family:Arial; 
        }
        
        

        table#gvResults{
            table-layout:fixed !important;
        }

        .container {
            width: 100%;
        }

        .left,
        .right {
          float: left;
          width: 50%;
  
          border: 0px solid red;
          box-sizing: border-box;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            GetAvailableDealerInfo();

            $("#btnSearch").click(function () {
                //$("#taStatus").val('');
                StartSearch();
                $(this).prop('disabled', true);
                $("#dvLeft").LoadingOverlay("show");
            });
        });

        function GetAvailableDealerInfo() {
            var dealersEndpoint = "https://api-carscraper.azurewebsites.net/api/GetAvailableDealers";
            var makesEndpoint = "https://api-carscraper.azurewebsites.net/api/GetAvailableMakes";

            $.getJSON(dealersEndpoint,{ format: "json" })
                .done(function (dealers) {
                    $("#taDealers").val("Available dealers (" + dealers.length + "):\n" + dealers.join("\n"));
                });

            $.getJSON(makesEndpoint, { format: "json" })
                .done(function (makes) {
                    $("#taMakes").val("Available makes (" + makes.length + "):\n" + makes.join("\n"));
                });
        };

        function AddToTextArea(lineOfText, clearFirst) {
            var tArea = $('#taStatus');

            if (clearFirst == true) {
                tArea.val(lineOfText);
            }
            else {
                tArea.val(tArea.val() + "\n" + lineOfText);
            }

            //autoscroll to bottom
            if (tArea.length)
                tArea.scrollTop(tArea[0].scrollHeight - tArea.height());
        };

        function StartSearch() {
            var attempt = 0;
            var tMake = $("#tbMake").val();
            var tModel = $("#tbModel").val();
            var tIsLoaner = $("#cbLoanerOnly").is(":checked");
            var tDealerType = "";
            var tRegion = "";

            $("input[type=radio][name='dealerType']").each(function () {
                if (this.checked == true) {
                    tDealerType = this.value;
                }
            });

            $("input[type=radio][name='region']").each(function () {
                if (this.checked == true) {
                    tRegion = this.value;
                }
            });

            var tData = { make: tMake, model: tModel, dealerType: tDealerType, isLoaner: tIsLoaner, region: tRegion };
            var tUrl = "https://api-carscraper.azurewebsites.net/api/startsearch";
            AddToTextArea("POST " + tUrl + ", parameters: " + JSON.stringify(tData), true);

            $.ajax({
                url: tUrl,
                type: 'post',
                data: JSON.stringify(tData),
                dataType: 'json',
                contentType: "application/json"
            }).done(function (data) {
                AddToTextArea("Response: searchKey=" + data.searchKey + "; retryAfter=" + data.retryAfter + ", result endpoint=" + data.keyRetrievalEndpoint);
                PollResultEndpoint(data.searchKey, data.retryAfter, data.keyRetrievalEndpoint, attempt+1);
            }).fail(function () {
                alert("error");
            });
        };

        function PollResultEndpoint(searchKey, retryAfter, endpoint, attempt) {
            AddToTextArea("Polling results endpoint with 10 sec interval. Try " + attempt);

            $.getJSON(endpoint + "?searchKey=" + searchKey,
                {
                    format: "json"
                })
                .done(function (json) {
                    var i = false;
                    //continue polling, results arent ready yet
                    if (json.status == "SearchInProgress") {
                        AddToTextArea("Response: SearchInProgress. Continue polling " + endpoint + "?searchKey=" + searchKey);
                        setTimeout(PollResultEndpoint, 10000, searchKey, retryAfter, endpoint, attempt+1); //10 sec, implement retryAfter logic 
                    }
                    else {
                        if (json.results) {
                            AddToTextArea("Response: Success. Stop polling. Results: " + json.count + ", duration: " + json.durationInSeconds + " sec" );
                            $("#tbGrid").DataTable(
                                {
                                    destroy: true,
                                    bLengthChange: true,
                                    lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                                    theme: 'energyblue',
                                    bFilter: true,
                                    bSort: true,
                                    bPaginate: true,
                                    data: json.results,
                                    fixedColums: true,
                                    initComplete: function () {
                                        $("#btnSearch").prop('disabled', false);
                                        $("#dvLeft").LoadingOverlay("hide", true);
                                    },
                                    //hide empty columns
                                    fnDrawCallback: function () {
                                        var api = this.api();
                                        setTimeout(function () {
                                            api.columns().flatten().each(function (colIdx) {
                                                var columnData = api.columns(colIdx).data().join('');
                                                if (columnData.length == (api.rows().count() - 1) ) {
                                                    api.column(colIdx).visible(false);
                                                }
                                            });
                                        }, 0)
                                    },
                                    columns: [
                                        { 'data': 'make', 'width' : '5%' },
                                        { 'data': 'model', 'width': '15%' },
                                        { 'data': 'exteriorColor', 'width': '5%' },
                                        { 'data': 'interiorColor', 'width': '5%' },
                                        { 'data': 'msrp', 'width': '5%' },
                                        { 'data': 'packages', 'width': '20%' },
                                        { 'data': 'engine', 'width': '5%' },
                                        { 'data': 'driveType', 'width': '5%' },
                                        { 'data': 'stockNumber', 'width': '5%' },
                                        { 'data': 'vin', 'width': '5%' },
                                        {
                                            "data": "url",
                                            "width": "5%",
                                            "render": function (data, type, row, meta) {
                                                if (type === 'display') {
                                                    data = '<a target="_new" href="' + data + '">listing</a>';
                                                }

                                                return data;
                                            }
                                        },
                                        { 'data': 'bodyStyle', 'width': '5%' },
                                        //{ 'data': 'modelCode', 'width': '5%' },
                                        { 'data': 'transmission', 'width': '5%' },
                                        { 'data': 'dealerName', 'width': '5%' },
                                        {
                                            "data": "iPacket",
                                            "width": "5%",
                                            "render": function (data, type, row, meta) {
                                                if (type === 'display' && data) {
                                                    data = '<a target="_new" href="' + data + '">IPacket</a>';
                                                }

                                                return data;
                                            }
                                        }
                                    ]
                                });

                            return;
                        }

                        alert("Something went wrong");
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    AddToTextArea("Error in getJSON callback for Url " + endpoint + "?searchKey=" + searchKey + ". Error: " + error);
                    alert("fail");
                });
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divMain" style="width: 100%;">
            
            <div id="dvLeft" class="left">
                <table id="tbHeaderTable" border="0" cellpadding="8" cellspacing="0">
                    <tr>
                        <td style="text-align:left;width:50;text-wrap:none;">
                            <table border="0" cellpadding="2">
                                <tr>
                                    <td>Make:</td>
                                    <td><input type="text" id="tbMake" /></td>
                                </tr>
                                <tr>
                                    <td>Model:</td>
                                    <td>
                                        <input type="text" id="tbModel" />
                                        <input type="button" id="btnSearch" name="btnSearch" value="Search" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <input type="radio" id="rDealerOn" name="dealerType" value="DealerOn"/>search DealerOn sites<br />
                            <input type="radio" id="rDealerInspire" name="dealerType" value="DealerInspire" />search DealerInspire sites<br />
                            <input type="radio" id="rDealerCom" name="dealerType" value="DealerCom"  />search DealerCom sites<br/>
                            <input type="radio" id="rAll" name="dealerType" value="All" checked="checked"  />search all (slow)<br/><br />
                            <input type="checkbox" id="cbLoanerOnly"/>loaners only<br />(works only with Volvo DealerCom sites)<br />
                        </td>
                        <td>
                            <input type="radio" id="regionsNe" name="region" value="Northeast" checked="checked" />Northeast<br/>
                            <input type="radio" id="regionsW" name="region" value="West" />West<br/>
                            <input type="radio" id="regionsSw" name="region" value="Southwest" />Southwest<br/>
                            <input type="radio" id="regionsMw" name="region" value="Midwest" />Midwest<br/>
                            <input type="radio" id="regionsSe" name="region" value="Southeast" />Southeast<br/>
                            <input type="radio" id="regionsAll" name="region" value="All" />All
                        </td>
                    </tr>
                </table>
            </div>

            <div id="dvRight" class="right">
                <textarea id="taStatus" rows="10" style="width:99%;font-size:9px;"></textarea>
            </div>

            <br /><br />

            <table id="tbGrid" class="display compact" style="width:100%">
                <thead>
                    <tr>
                        <th>Make</th>
                        <th class="col-200">Model</th>
                        <th>Exterior Color</th>
                        <th>Interior Color</th>
                        <th>MSRP</th>
                        <th>Packages</th>
                        <th>Engine</th>
                        <th>DriveType</th>
                        <th>Stock #</th>
                        <th>VIN</th>
                        <th>URL</th>
                        <th>Body Style</th>
                        <%--<th>Model Code</th>--%>
                        <th>Transmission</th>
                        <th>Dealer Name</th>
                        <th>IPacket</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
            <br /><br />
            <textarea id="taMakes" rows="20" style="width:200px;font-size:10px;"></textarea>
            <textarea id="taDealers" rows="20" style="width:200px;font-size:10px;"></textarea>
            <p><b>Search tips:</b><br />Search heavily depends on how you enter model. Model is always case sensitive. Some examples for hard-to-figure out models to get proper results:
                <br />
                For Mazda, enter model as "Mazda CX-5", "Mazda6"<br />
                For Mercedes-Benz, enter model as "GLC" instead of "GLC300"<br />
                For Lexus, enter model as "RX" instead of "RX350"<br />
                For Chevrolet, enter model as "Silverado 1500" instead of "Silverado"<br />
                For BMW, enter model as "3 Series" to get any 3-series results, "X3" and so on for SAVs<br />
            </p> 
        </div>
    </form>
</body>
</html>
