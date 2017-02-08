<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserMgr.aspx.cs" Inherits="WFM.JW.HB.Web.WFMUser.UserMgr" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DeletConfirm() {
            return confirm("确认要删除项目吗?")
        }

        function RefreshContext() {
            $('select[id$=DropDownListCity]')[0].selectedIndex = 0;
            $('select[id$=DropDownListContry]')[0].selectedIndex = 0;

            return false;
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
    <asp:ScriptManager runat="server">
    </asp:ScriptManager>
    <div class="span-24 Query  groupingborder">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="span-24 push-1">
                    <div class="span-7 ">
                        <asp:Label ID="Label3" runat="server" Text="单位:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListCity" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CityList_IndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="span-7 ">
                        <asp:Label ID="Label1" runat="server" Text="辖区单位:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListContry" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="span-23 push-1" style="margin-bottom: -20px; margin-top: 20px;">
            <div class="span-6 push-16 ">
                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="ImageButton" OnClientClick="return RefreshContext()">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/refresh.png" ImageAlign="AbsMiddle" />
                    <span>清&nbsp;&nbsp;空</span>
                </asp:LinkButton>
                <asp:LinkButton ID="LinkButtonQuery" runat="server" CssClass="ImageButton" OnClick="LinkButtonQuery_Click">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                    <span>查&nbsp;&nbsp;询</span>
                </asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="span-24 DataArea">
        <div class="span-6" style="margin-bottom: 4px; margin-left: 2px;">
            <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton" OnClick="LinkButtonNew_Click">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/new.png" ImageAlign="AbsMiddle" />
                <span>新&nbsp;增</span>
            </asp:LinkButton>
            <asp:LinkButton ID="LinkButtonModify" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonModify_Click">
                <asp:Image ID="ImageModify" runat="server" ImageUrl="~/Styles/Img/modify.ico" ImageAlign="AbsMiddle" />
                <span>修&nbsp;改</span>
            </asp:LinkButton>
            <asp:LinkButton ID="LinkButtonDelete" runat="server" CssClass="ImageButton hidden"
                OnClientClick="return DeletConfirm()" OnClick="LinkButtonDelete_Click">
                <asp:Image ID="ImageDelete" runat="server" ImageUrl="~/Styles/Img/cancel.ico" ImageAlign="AbsMiddle" />
                <span>删&nbsp;除</span>
            </asp:LinkButton>
        </div>
        <%--  <div class="span-3 ">
                    <asp:LinkButton ID="LinkButtonDetails" runat="server" CssClass="ImageButton hidden">
                        <asp:Image ID="ImageDetails" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                        <span>合同明细</span></asp:LinkButton>
                </div>--%>
        <div class="span-17 pager" style="text-align: right;">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <div id="ScrollList" style="width: 960px; overflow: auto;">
        <cc1:CLGridViewEx ID="CLGridViewUserMgrInfo" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" DataKeyNames="RegionUserID"
            OnRowCommand="CLGridViewRegionUserInfo_RowCommand">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <HeaderStyle CssClass="tr-header" HorizontalAlign="Center" />
            <Columns>
                <asp:BoundField DataField="RegionUserID" HeaderText="RegionUserID" SortExpression="RegionUserID" Visible="false" />
                <asp:BoundField DataField="Version" HeaderText="version" Visible="false" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.CLGridViewUserMgrInfo.PageIndex * this.CLGridViewUserMgrInfo.PageSize + CLGridViewUserMgrInfo.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="UserName" HeaderText="用户名" />
                <asp:BoundField DataField="ParentName" HeaderText="上级单位" />
                <asp:BoundField DataField="RegionName" HeaderText="单位" />
                <asp:BoundField DataField="LastActivityDate" HeaderText="上次登录时间" />
                <asp:BoundField DataField="Createby" HeaderText="创建人" />
            </Columns>
        </cc1:CLGridViewEx>
    </div>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
