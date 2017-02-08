<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BaseTypeQuery.aspx.cs" Inherits="WFM.JW.HB.Web.BaseTypeQuery" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DeletConfirm() {
            return confirm("确认要删除此数据类型吗?")
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
                <asp:Label ID="Label3" runat="server" Text="类型名称:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxBaseTypeName" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7">
                <asp:Label ID="Label4" runat="server" Text="状态:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListEnable" runat="server">
                    <asp:ListItem Value="all" Selected="True">全部</asp:ListItem>
                    <asp:ListItem Value="true">启用</asp:ListItem>
                    <asp:ListItem Value="false"> 禁用</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="span-7">
            </div>
        </div>
        <div class="span-24 ">
            <div class="span-4 push-18" style="margin-top: 5px; margin-bottom: -5px">
                <asp:LinkButton ID="LinkButton4" runat="server" CssClass="ImageButton" TabIndex="0"
                    OnClick="LinkButton4_Click">
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
        <div class="span-3  ">
            <asp:LinkButton ID="LinkButtonDetails" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonDetails_Click">
                <asp:Image ID="ImageDetails" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>数据项</span></asp:LinkButton>
        </div>
        <div class="span-14 pager ">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
        <cc1:CLGridViewEx ID="BaseTypeGridView" runat="server" CssClass="span-24 DataGridViewStyle"
            AutoGenerateColumns="False" DataKeyNames="BaseTypeID" OnRowCommand="GridView1_RowCommand"
            AllowDBClick="true">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <HeaderStyle CssClass="tr-header" />
            <Columns>
                <asp:BoundField DataField="BaseTypeID" HeaderText="BaseTypeID" Visible="false" />
                <asp:BoundField DataField="version" HeaderText="version" Visible="False" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.BaseTypeGridView.PageIndex * this.BaseTypeGridView.PageSize + BaseTypeGridView.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="BaseTypeName" HeaderText="类型名称" />
                <asp:BoundField DataField="TypeOrder" HeaderText="排列顺序" />
                <asp:BoundField DataField="Comment" HeaderText="备注" />
                <asp:TemplateField HeaderText="状态">
                    <EditItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Enable") %>' />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="EnableLabel" runat="server" Text='<%# (bool)Eval("Enable")?"启用" : "禁用" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Creator" HeaderText="创建人" />
                <asp:TemplateField HeaderText="创建时间" SortExpression="CreateDate">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CreateDate") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%#((DateTime)Eval("CreateDate")).ToString("yyyy/MM/dd") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Modifier" HeaderText="修改人" />
                <asp:TemplateField HeaderText="修改时间">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ModifyDate") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%#((DateTime)Eval("ModifyDate")).ToString("yyyy/MM/dd")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </cc1:CLGridViewEx>
    </div>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="t" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
