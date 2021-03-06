﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EmployeesView.aspx.cs" Inherits="WFM.JW.HB.Web.CMSEmployees.EmployeesView" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DeletCheck() {
            return confirm("请确认是否删除数据项?")
        }
        $(document).ready(function () {
            var message = $('input[id$=ShowDialog]').val();
            if (message.length > 1) {
                alert(message);
                $('input[id$=ShowDialog]').val("");
            }

        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="span-23 Query groupingborder">
        <div class="span-24 ">
            <div class="span-7 prepend-1">
                <asp:Label ID="Label3" runat="server" Text="所属部门:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListDepartment" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label4" runat="server" Text="姓名:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxUserName" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7 prepend-1">
                <asp:Label ID="LabelEnable" runat="server" Text="状态:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListEnable" runat="server">
                    <asp:ListItem Value="all" Selected="True">全部</asp:ListItem>
                    <asp:ListItem Value="true">启用</asp:ListItem>
                    <asp:ListItem Value="false"> 禁用</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-24 ">
            <div class="span-6 push-17" style="margin-top: 5px; margin-bottom: -5px">
                
                <asp:LinkButton ID="LinkButtonQuery" runat="server" CssClass="ImageButton" OnClick="LinkButtonQuery_Click">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                    <span>查&nbsp;&nbsp;询</span></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="span-24 DataArea">
        <div class="span-6" style="margin-bottom: 4px; margin-left: 2px;">
            <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton" OnClick="LinkButtonNew_Click">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/new.png" ImageAlign="AbsMiddle" />
                <span>新&nbsp;增</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonModify" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonModify_Click">
                <asp:Image ID="ImageModify" runat="server" ImageUrl="~/Styles/Img/modify.ico" ImageAlign="AbsMiddle" />
                <span>修&nbsp;改</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonDelete" runat="server" CssClass="ImageButton hidden"
                OnClientClick='return DeletCheck()' OnClick="LinkButtonDelete_Click">
                <asp:Image ID="ImageDelete" runat="server" ImageUrl="~/Styles/Img/cancel.ico" ImageAlign="AbsMiddle" />
                <span>删&nbsp;除</span></asp:LinkButton>
        </div>
        <div class="span-2 ">
            <asp:LinkButton ID="LinkButtonDetails" runat="server" CssClass="ImageButton hiddencontrol">
                <asp:Image ID="ImageDetails" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>明&nbsp;细</span></asp:LinkButton>
        </div>
        <div class="span-15 pager">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <cc1:CLGridViewEx ID="CLGridViewEmployees" runat="server" CssClass="span-24 DataGridViewStyle"
        AutoGenerateColumns="False" OnRowCommand="GridViewItem_RowCommand" AllowDBClick="True"
        DataKeyNames="EmployeeID">
        <AlternatingRowStyle BackColor="#EEEEEE" />
        <SelectedRowStyle CssClass="tr-selected" />
        <Columns>
            <asp:BoundField DataField="EmployeeID" HeaderText="EmployeeID" Visible="false" />
            <asp:BoundField DataField="version" HeaderText="version" Visible="false" />
            <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <%# this.CLGridViewEmployees.PageIndex * this.CLGridViewEmployees.PageSize + CLGridViewEmployees.Rows.Count + 1%>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="所属部门">
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Department.DepartmentName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="EmployeeName" HeaderText="姓名" />
            <asp:TemplateField HeaderText="登录名">
                <ItemTemplate>
                    <asp:Label ID="EnableLabel" runat="server" Text='<%# (bool)Eval("IsUser")? Eval("UserName") : " " %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="状态">
                <ItemTemplate>
                    <asp:Label ID="EnableLabel" runat="server" Text='<%# (bool)Eval("Enable")?  "启用" : "禁用" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Modifier" HeaderText="修改人" />
            <asp:BoundField DataField="ModifyDate" HeaderText="修改日期" DataFormatString="{0:d}" />
        </Columns>
        <HeaderStyle CssClass="tr-header" />
    </cc1:CLGridViewEx>
    
    <asp:HiddenField runat="server" ID="selectedIndex" Value="t" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value = "0" />
</asp:Content>
