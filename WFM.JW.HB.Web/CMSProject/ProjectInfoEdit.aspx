<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProjectInfoEdit.aspx.cs" Inherits="WFM.JW.HB.Web.CMSProject.ProjectInfoEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .Wdate
        {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="ProjectID" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    
    <div class="span-23 Query groupingborder">
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label4" runat="server" Text="项目名称:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ProjectNameTextBox" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label7" runat="server" Text="项目简称:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ProjectShortNameTextBox" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label3" runat="server" Text="开始时间:" CssClass="span-3"></asp:Label>
            <input id="ProjectStartDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true})"
                class="title span-5 Wdate" runat="server" readonly="readonly"  />
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label2" runat="server" Text="结束时间:" CssClass="span-3"></asp:Label>
            <input id="ProjectEndDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true})"
                class="title span-5 Wdate" runat="server" readonly="readonly" />
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label5" runat="server" Text="项目类型:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="ProjectType" runat="server">
            </asp:DropDownList>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label6" runat="server" Text="项目状态:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="ProjectStatus" runat="server">
            </asp:DropDownList>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label1" runat="server" Text="备注:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ProjectCommentTextBox" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelInfo" runat="server" Text=""></asp:Label>
        </div>
        <div class="span-23 Magin-1 push-2">
            <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
                <asp:LinkButton ID="LinkButtonApply" runat="server" CssClass="ImageButton" 
                    oncommand="LinkButtonApply_Command">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                    <span>应&nbsp;&nbsp;用</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButtonSave" runat="server" CssClass="ImageButton" 
                    oncommand="LinkButtonSave_Command">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/save.ico" ImageAlign="AbsMiddle" />
                    <span>保&nbsp;&nbsp;存</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="ImageButton " 
                    oncommand="LinkButtonBack_Command">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/undo.ico" ImageAlign="AbsMiddle" />
                    <span>返&nbsp;&nbsp;回</span></asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>
