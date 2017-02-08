<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ClueReportRegion.aspx.cs" Inherits="WFM.JW.HB.Web.WFMClues.ClueReportRegion" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .Wdate {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
    <script type="text/javascript">
        function DeletConfirm() {
            return confirm("确认要删除项目吗?")
        }

        function RefreshContext() {
            //$('input[id$=TextBoxContractName]').attr('value', '');
            //$('input[id$=TextBoxContractCode]').attr('value', '');
            //$('select[id$=DropDownListContractType]')[0].selectedIndex = 0;
            //$('select[id$=DropDownProject]')[0].selectedIndex = 0;
            //$('select[id$=DropDownListDepartment]')[0].selectedIndex = 0;
            //$('input[id$=TextBoxSupplier]').attr('value', '');
            //$('select[id$=DropDownListContractStatus]')[0].selectedIndex = 0;

            return false;
        }

        $(document).ready(function () {
            var message = $('input[id$=ShowDialog]').val();
            if (message.length > 1) {
                alert(message);
                $('input[id$=ShowDialog]').val("");
            }

        });

        function syncdate() {


            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="server">
    </asp:ScriptManager>
    <div class="span-24 Query groupingborder">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="span-23 push-1">
                    <div class="span-7 ">
                        <asp:Label ID="Label3" runat="server" Text="单位:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListCity" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CityList_IndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="span-7 ">
                        <asp:Label ID="Label5" runat="server" Text="辖区单位:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListContry" runat="server">
                        </asp:DropDownList>
                    </div>
                    <div class="span-7 ">
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
            <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton" OnClick="LinkButtonDetails_Click">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>详&nbsp;细</span>
            </asp:LinkButton>
          
        </div>

        <div class="span-17 pager"  style="text-align: right;">
            <uc1:Pages ID="Pages1" runat="server"/>
        </div>
    </div>
    <div id="ScrollList" style="width: 990px; overflow: auto; margin-left:-5px;" >
        <cc1:CLGridViewEx ID="CLGridClueReport" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" OnRowCommand="CLGridClueReport_RowCommand"
            DataKeyNames="RegionGuid">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <Columns>
                <asp:BoundField DataField="RegionGuid" HeaderText="RegionGuid" Visible="False" />
                <asp:BoundField DataField="version" HeaderText="version" Visible="False" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.CLGridClueReport.PageIndex * this.CLGridClueReport.PageSize + CLGridClueReport.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="市州">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-5' Text='<%# Bind("RegionName") %>'
                            ToolTip='<%# Eval("RegionName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="线索总数">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-3' Text='<%# Bind("TotalClues") %>'
                             ToolTip='<%# Eval("TotalClues") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="问题线索">
                    <ItemTemplate>
                        <asp:Label ID="LabelDepartment" runat="server" CssClass='mlength-3' Text='<%# Bind("Problems") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="录入错误数">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-3' Text='<%# Bind("InputErrors") %>'
                            ToolTip='<%# Eval("InputErrors") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="已核查数">
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" CssClass="mlength-3" Text='<%# Bind("Confirmed") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查实问题数">
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" CssClass="mlength-2" Text='<%# Bind("IsTrue") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="备注">
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" CssClass="mlength-1" Text=''></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

            </Columns>
            <HeaderStyle CssClass="tr-header" />
        </cc1:CLGridViewEx>
    </div>

    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
