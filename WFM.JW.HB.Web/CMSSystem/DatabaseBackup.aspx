<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DatabaseBackup.aspx.cs" Inherits="WFM.JW.HB.Web.CMSSystem.DatabaseBackup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="span-24 DataArea">
        <div class="span-6" style="margin-bottom: 4px; margin-left: 2px;">
            <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/export.ico" ImageAlign="AbsMiddle" />
                <span>导出数据库</span></asp:LinkButton>
        </div>
    </div>
</asp:Content>
