<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TaskEdit.aspx.cs" Inherits="WFM.JW.HB.Web.WFMTask.TaskEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Scripts/jquery-ui-1.8.21.custom.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-ui-1.8.21.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        .Wdate {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
    <script type="text/javascript">

        function CheckDroplist() {

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="TaskGuid" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    <div class="span-23 Query groupingborder">
        <div class="span-23 prepend-1">
            <div class="span-15  Magin-0">
                <asp:Label ID="Label4" runat="server" Text="上级单位:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownListCity" runat="server" CssClass="span-6">
                </asp:DropDownList>
            </div>

        </div>
        <div class="span-23 prepend-1">
            <div class="span-15  Magin-0">
                <asp:Label ID="Label2" runat="server" Text="单位:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownListContry" runat="server" CssClass="span-6">
                </asp:DropDownList>
            </div>

        </div>
        <div class="span-23 prepend-1">
            <div class="span-15  Magin-0">
                <asp:Label ID="Label5" runat="server" Text="上传时间:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxCreateTime" runat="server" CssClass="title span-6"></asp:TextBox>
            </div>

        </div>
        <div class="span-23 prepend-1">
            <div class="span-15  Magin-0">
                <asp:Label ID="Label8" runat="server" Text="数据名称:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxTaskName" runat="server" CssClass="title span-6"></asp:TextBox>
            </div>

        </div>
        <div class="span-23 prepend-1">
            <div class="span-15 Magin-0">
                <asp:Label ID="Label7" runat="server" Text="数据状态:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxStatus" runat="server" CssClass="title span-6 suppliername"></asp:TextBox>
                <%--<asp:DropDownList ID="DropDownSupplier" runat="server">
                </asp:DropDownList>--%>
            </div>

        </div>

        <div class="span-23 prepend-1">
            <div class="span-15 Magin-0">
                 <asp:Label ID="Label1" runat="server" Text="是否上报数据:" CssClass="span-3"></asp:Label>
                
                <asp:DropDownList ID="DropDownListData" runat="server">
                </asp:DropDownList>
            </div>

        </div>

        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelInfo" runat="server" Text=""></asp:Label>
        </div>
        <div class="span-10 Magin-1 push-7">
            <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
                <asp:LinkButton ID="LinkButtonApply" runat="server" CssClass="ImageButton" OnClientClick="return CheckDroplist()"
                    OnCommand="LinkButtonApply_Command"> <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                    <span>应&nbsp;&nbsp;用</span> </asp:LinkButton>
                <%--<asp:LinkButton ID="LinkButtonSave" runat="server" CssClass="ImageButton" OnClientClick="return CheckDroplist()"
                    OnCommand="LinkButtonSave_Command"> <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/save.ico" ImageAlign="AbsMiddle" />
                    <span>保&nbsp;&nbsp;存</span> </asp:LinkButton>--%>
                <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="ImageButton " OnCommand="LinkButtonBack_Command"> <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/undo.ico" ImageAlign="AbsMiddle" />
                    <span>返&nbsp;&nbsp;回</span> </asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>
