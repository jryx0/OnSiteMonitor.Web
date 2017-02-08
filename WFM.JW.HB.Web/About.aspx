<%@ Page Title="关于我们" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="WFM.JW.HB.Web.About" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        关于
    </h2>
    <p>
        将内容放置在此处。
    </p>
    <cc1:CLDropDownListEx runat="server" 
        onselectedindexchanged="Unnamed1_SelectedIndexChanged" 
        ontextchanged="Unnamed1_TextChanged">
    </cc1:CLDropDownListEx>
</asp:Content>
