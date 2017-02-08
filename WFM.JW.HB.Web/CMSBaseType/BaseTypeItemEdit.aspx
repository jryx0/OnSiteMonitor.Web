<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BaseTypeItemEdit.aspx.cs" Inherits="WFM.JW.HB.Web.CMSBaseType.BaseTypeItemEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DataCheck() {
            var ddlVal = $('#<%=DropDownListBaseType.ClientID%>').val();

            if (ddlVal == '0') {
                alert("请选择数据类型名称!");
                return false;
            }

            return true;
        }
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="BaseTypeItemID" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    
    <div class="span-23 Query groupingborder">
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label3" runat="server" Text="类型名称:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="DropDownListBaseType" runat="server">
            </asp:DropDownList>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label4" runat="server" Text="数据项:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ItemName" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label1" runat="server" Text="数据项值:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ItemValue" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label2" runat="server" Text="数据项值类型:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ItemType" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label5" runat="server" Text="备注:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="CommentTextBox" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
    <div class="span-23 prepend-1 Magin-1">
        <asp:Label ID="LabelOrder" runat="server" Text="顺序:" CssClass="span-3" Visible="false"></asp:Label>
        <asp:TextBox ID="ItemOrder" runat="server" CssClass="title span-5" Visible="false"></asp:TextBox>
    </div>
    <div class="span-23 prepend-1 Magin-1">
        <asp:Label ID="EnableLabel" runat="server" Text="状态:" CssClass="span-3" Visible="false"></asp:Label>
        <asp:DropDownList ID="EnableDropDownList" runat="server" Visible="false">
            <asp:ListItem Value="0">禁用</asp:ListItem>
            <asp:ListItem Value="1">启用</asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="span-23 prepend-1 Magin-1">
        <asp:Label ID="LabelInfo" runat="server" Text=""></asp:Label>
    </div>
    <div class="span-23 Magin-1 push-2">
        <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
            <asp:LinkButton ID="LinkButtonApply" runat="server" CssClass="ImageButton" OnClientClick="return DataCheck()"
                OnCommand="LinkButtonApply_Command">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                <span>应&nbsp;&nbsp;用</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonSave" runat="server" CssClass="ImageButton" OnClientClick="return DataCheck()"
                OnCommand="LinkButtonSave_Command">
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/save.ico" ImageAlign="AbsMiddle" />
                <span>保&nbsp;&nbsp;存</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="ImageButton " 
                onclick="LinkButtonBack_Click" >
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/undo.ico" ImageAlign="AbsMiddle" />
                <span>返&nbsp;&nbsp;回</span></asp:LinkButton>
        </div>
    </div>
    </div>
</asp:Content>
