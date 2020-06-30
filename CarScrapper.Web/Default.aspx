<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CarScrapper.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .mydatagrid
        {
        width: 80%;
        border: solid 2px black;
        min-width: 80%;
        }
        .header
        {
        background-color: #000;
        font-family: Arial;
        color: White;
        height: 20px;
        text-align: center;
        font-size: 16px;
        }

        .rows
        {
        background-color: #fff;
        font-family: Arial;
        font-size: 11px;
        color: #000;
        min-height: 25px;
        text-align: left;
        }
        .rows:hover
        {
        background-color: #5badff;
        color: #fff;
        }

        .mydatagrid a /** FOR THE PAGING ICONS **/
        {
        background-color: Transparent;
        padding: 5px 5px 5px 5px;
        color: #fff;
        text-decoration: none;
        font-weight: bold;
        }

        .mydatagrid a.gridLink 
        {
            background-color: white;
            color: black;
            font-weight: normal;
            padding: 0px 0px 0px 0px;
        }

        .mydatagrid a:hover /** FOR THE PAGING ICONS HOVER STYLES**/
        {
        background-color: #000;
        color: #fff;
        }
        .mydatagrid span /** FOR THE PAGING ICONS CURRENT PAGE INDICATOR **/
        {
        background-color: #fff;
        color: #000;
        padding: 5px 5px 5px 5px;
        }
        .pager
        {
        background-color: #5badff;
        font-family: Arial;
        color: White;
        height: 30px;
        text-align: left;
        }

        .mydatagrid td
        {
        padding: 5px;
        }
        .mydatagrid th
        {
        padding: 5px;
        }
    </style>
    <br /><br />
    Make:
    <asp:TextBox runat="server" ID="tbMake" AutoPostBack="false"></asp:TextBox>
    Model:
    <asp:TextBox runat="server" ID="tbModel" AutoPostBack="false"></asp:TextBox>
    <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" />
    <asp:CheckBox runat="server" ID="cbDealerOn" Checked="true" />include DealerOn sites
    <asp:CheckBox runat="server" ID="cbDealerInspire" Checked="false" />include DealerInspire sites
    <br /><br />
    <asp:Label runat="server" EnableViewState="true" ID="lblCount" Text="0" />
    <asp:GridView 
        ID="grid1" 
        runat="server" 
        AutoGenerateColumns="true"
        CssClass="mydatagrid" 
        PagerStyle-CssClass="pager"
        HeaderStyle-CssClass="header" 
        RowStyle-CssClass="rows" 
        AllowPaging="False" 
        AllowSorting="true"
        OnSorting="grid1_Sorting"
        OnPreRender="grid1_PreRender"
        OnPageIndexChanging="grid1_PageIndexChanging"></asp:GridView>



    <%--<div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>--%>

</asp:Content>
