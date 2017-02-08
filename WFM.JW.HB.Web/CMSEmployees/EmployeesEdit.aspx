<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EmployeesEdit.aspx.cs" Inherits="WFM.JW.HB.Web.CMSEmployees.EmployeesEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DataCheck() {
            var ddlVal = $('#<%=DropDownListDepartment.ClientID%>').val();

            if (ddlVal == '0') {
                alert("请选择所属部门!");
                return false;
            }

            return true;
        }
        function DropDownListCheck() {
            var selected = $('select[id$=DropDownListUser]').children('option:selected').val();

            if (selected == "1") {
                $('span[id$=LabelUserName]').removeClass('hidden');
                $('input[id$=TextBoxUserName]').removeClass('hidden');
            }
            else if (selected == "0") {
                $('span[id$=LabelUserName]').addClass('hidden');
                $('input[id$=TextBoxUserName]').addClass('hidden');
            }
        }

        $(document).ready(function () {
            DropDownListCheck();
            $('select[id$=DropDownListUser]').change(function () {
                DropDownListCheck();                
            })
        });
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="EmployeeID" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
   
    <div class="span-23 Query groupingborder">
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label3" runat="server" Text="所属部门:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="DropDownListDepartment" runat="server">
            </asp:DropDownList>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label4" runat="server" Text="姓名:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxName" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelSystemUser" runat="server" Text="系统用户:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="DropDownListUser" runat="server">
                <asp:ListItem Value="0">否</asp:ListItem>
                <asp:ListItem Value="1">是</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelUserName" runat="server" Text="用户名:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxUserName" runat="server" CssClass="title span-5"></asp:TextBox>
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
                <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="ImageButton " OnClick="LinkButtonBack_Click">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/undo.ico" ImageAlign="AbsMiddle" />
                    <span>返&nbsp;&nbsp;回</span></asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>
