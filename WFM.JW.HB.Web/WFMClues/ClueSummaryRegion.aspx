<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ClueSummaryRegion.aspx.cs" Inherits="WFM.JW.HB.Web.WFMClues.ClueSummaryRegion" %>

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
                    <div class="span-7 ">
                        <asp:Label ID="Label5" runat="server" Text="乡镇街道:" CssClass="span-2"></asp:Label>
                        <asp:TextBox ID="TextBox2" runat="server" CssClass="title span-5"></asp:TextBox>
                    </div>

                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="span-24">

            <div class="span-22" style="text-align: right;">
                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="ImageButton" OnClientClick="return RefreshContext()">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/refresh.png" ImageAlign="AbsMiddle" />
                    <span>清&nbsp;&nbsp;空</span>
                </asp:LinkButton>
                <asp:LinkButton ID="LinkButton3" runat="server" CssClass="ImageButton" OnClick="LinkButtonQuery_Click">
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                    <span>查&nbsp;&nbsp;询</span>
                </asp:LinkButton>
            </div>
        </div>

    </div>
    <div class="span-24 DataArea">

        <div class="span-24 pager" style="text-align: right;">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <div id="ScrollList" style="width: 990px; overflow: auto; margin-left: -5px;">
        <cc1:CLGridViewEx ID="CLGridClueSummary" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" DataKeyNames="RegionGuid">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <HeaderStyle CssClass="tr-header" HorizontalAlign="Center" />
            <Columns>
                <asp:BoundField DataField="RegionGuid" HeaderText="RegionGuid" SortExpression="RegionGuid" Visible="false" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength' Text=' <%# this.CLGridClueSummary.PageIndex * this.CLGridClueSummary.PageSize + CLGridClueSummary.Rows.Count + 1%>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center" Width="25px"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="区县">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-2' Text='<%# Bind("RegionName") %>'
                            ToolTip='<%# Eval("RegionName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="乡镇街道">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-3' Text='<%# Bind("townName") %>'
                            ToolTip='<%# Eval("townName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="线索总数">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("TotalClues") %>'
                            ToolTip='<%# Eval("TotalClues") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="问题线索">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("Problems") %>'
                            ToolTip='<%# Eval("Problems") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="录入错误数">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("InputErrors") %>'
                            ToolTip='<%# Eval("InputErrors") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="已核查数">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("Confirmed") %>'
                            ToolTip='<%# Eval("Confirmed") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="核实数">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("Confirmed") %>'
                            ToolTip='<%# Eval("Confirmed") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>


            </Columns>
        </cc1:CLGridViewEx>
         
    </div>

    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
