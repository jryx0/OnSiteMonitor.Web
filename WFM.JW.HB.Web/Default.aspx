<%@ Page Title="主页" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WFM.JW.HB.Web._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="span-23  groupingborder">
        <div class="span-23 prepend-1 Magin-1" style="font-size:16px;">
            <asp:Label ID="ProjectNumber" runat="server" CssClass="span-9" />
            <asp:Label ID="ContractNumber" runat="server" CssClass="span-9" />
            <asp:Label ID="ContractValue" runat="server" CssClass="span-9" />
            <asp:Label ID="ContractSettlement" runat="server" CssClass="span-9" />

        </div>

        <div class="span-23 prepend-1 Magin-1" style="font-size:16px;">
            <asp:Label ID="Label1" runat="server" CssClass="span-9" Text ="111" />
            <asp:Label ID="Label2" runat="server" CssClass="span-9"  Text ="<%Request.Path %>"/>
            <asp:Label ID="Label3" runat="server" CssClass="span-9" />
            <asp:Label ID="Label4" runat="server" CssClass="span-9" />

        </div>
    </div>
</asp:Content>
