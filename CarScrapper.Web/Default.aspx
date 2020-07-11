﻿<%@ Page Title="Car Scraper" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CarScrapper.Web._Default" Async="true"%>

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
        height: 16px;
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

    <table border="0" cellpadding="8" cellspacing="0">
        <tr>
            <td style="text-align:left;width:50;text-wrap:none;">
                <table border="0" cellpadding="2">
                    <tr>
                        <td colspan="2"><span style="font-size:13px;color:red;">Type "Loaner" works only for DealerCom Volvo sites</span></td>
                    </tr>
                    <tr>
                        <td style="vertical-align:bottom;">Type:</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlInventoryType">
                                <asp:ListItem Value="1">New</asp:ListItem>
                                <asp:ListItem Value="2">Loaner</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Make:</td>
                        <td><asp:TextBox runat="server" ID="tbMake" AutoPostBack="false"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Model:</td>
                        <td>
                            <asp:TextBox runat="server" ID="tbModel" AutoPostBack="false"></asp:TextBox>
                            <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="Search" />
                        </td>
                    </tr>
                </table>
            </td>
            <td style="text-align:left;">
                <asp:CheckBox runat="server" ID="cbDealerOn" Checked="true" />include DealerOn sites<br />
                <asp:CheckBox runat="server" ID="cbDealerInspire" Checked="false" />include DealerInspire sites<br />
                <asp:CheckBox runat="server" ID="cbDealerCom" Checked="false" />include DealerCom sites
            </td>
            <td style="text-align:left;">
                <asp:TextBox runat="server" ID="tbMakesList" TextMode="MultiLine" Rows="5" EnableViewState="true" Width="300px" Text="Makes available:" />
                &nbsp;
                <asp:TextBox runat="server" ID="tbDealerList" TextMode="MultiLine" Rows="5" EnableViewState="true" Width="300px" Text="Dealers available:" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label runat="server" EnableViewState="true" ID="lblCount" Text="0" />
                <asp:Label runat="server" ID="lblMessage"/>
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
                    OnRowCreated="grid1_RowCreated"
                    OnPageIndexChanging="grid1_PageIndexChanging"></asp:GridView>
            </td>
        </tr>
    </table>

    

    
    
    
    



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
