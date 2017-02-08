<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CMSSupplierView.aspx.cs" Inherits="WFM.JW.HB.Web.CMSSupplier.CMSSupplierView" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DeletConfirm() {
            return confirm("确认要删除供方吗?")
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
            <div class="span-8">
                <asp:Label ID="Label3" runat="server" Text="供方名称:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxSupplier" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-8">
                <asp:Label ID="Label2" runat="server" Text="供方类型:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownListSupplierType" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label4" runat="server" Text="状态:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListEnable" runat="server">
                    <asp:ListItem Value="all" Selected="True">全部</asp:ListItem>
                    <asp:ListItem Value="1">启用</asp:ListItem>
                    <asp:ListItem Value="0">禁用</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-24 ">
            <div class="span-3 push-20 " style="margin-top: 15px; margin-bottom: -5px">
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
                OnClientClick="return DeletConfirm()" OnClick="LinkButtonDelete_Click">
                <asp:Image ID="ImageDelete" runat="server" ImageUrl="~/Styles/Img/cancel.ico" ImageAlign="AbsMiddle" />
                <span>删&nbsp;除</span></asp:LinkButton>
        </div>
        <div class="span-3 ">
            <asp:LinkButton ID="LinkButtonDetails" runat="server" CssClass="ImageButton hiddencontrol">
                <asp:Image ID="ImageDetails" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>明&nbsp;细</span></asp:LinkButton>
        </div>
        <div class="span-14 pager">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <cc1:CLGridViewEx ID="CLGridViewSupplier" runat="server" AutoGenerateColumns="False"
        CssClass="span-24 DataGridViewStyle" DataKeyNames="SupplierID" 
        AllowDBClick="true" onrowcommand="CLGridViewSupplier_RowCommand">
        <AlternatingRowStyle BackColor="#EEEEEE" />
        <SelectedRowStyle CssClass="tr-selected" />
        <HeaderStyle CssClass="tr-header" />
        <Columns>
            <asp:BoundField DataField="SupplierID" HeaderText="SupplierID" Visible="false" />
            <asp:BoundField DataField="version" HeaderText="version" Visible="false" />
            <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <%# this.CLGridViewSupplier.PageIndex * this.CLGridViewSupplier.PageSize + CLGridViewSupplier.Rows.Count + 1%>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="SupplierName" HeaderText="供方名称" />
            <asp:TemplateField HeaderText="供方类型">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("SupplierType.ItemName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="状态">
                <ItemTemplate>
                    <asp:Label ID="EnableLabel" runat="server" Text='<%# (bool)Eval("Enable")?"启用" : "禁用" %>' />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>
             <asp:BoundField DataField="Comment" HeaderText="备注" />
            <asp:BoundField DataField="Creator" HeaderText="创建人" />
            <asp:BoundField DataField="CreateDate" HeaderText="创建时间" DataFormatString="{0:d}"
                ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center" />
            <asp:BoundField DataField="Modifier" HeaderText="修改人" />
            <asp:BoundField DataField="ModifyDate" HeaderText="修改时间" DataFormatString="{0:d}"
                ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center" />
        </Columns>
    </cc1:CLGridViewEx>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value = "0" />
</asp:Content>
