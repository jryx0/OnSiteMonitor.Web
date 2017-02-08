<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ContractView.aspx.cs" Inherits="WFM.JW.HB.Web.CMSProject.ContractView" %>

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
            $('input[id$=TextBoxContractName]').attr('value', '');
            $('input[id$=TextBoxContractCode]').attr('value', '');
            $('select[id$=DropDownListContractType]')[0].selectedIndex = 0;
            $('select[id$=DropDownProject]')[0].selectedIndex = 0;
            $('select[id$=DropDownListDepartment]')[0].selectedIndex = 0;
            $('input[id$=TextBoxSupplier]').attr('value', '');
            $('select[id$=DropDownListContractStatus]')[0].selectedIndex = 0;

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
    <div class="span-24 Query groupingborder">
        <div class="span-23 push-1">
            <div class="span-7 ">
                <asp:Label ID="Label3" runat="server" Text="合同名称:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxContractName" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label1" runat="server" Text="合同编号:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxContractCode" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label5" runat="server" Text="合同类别:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListContractType" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-23 push-1" style="margin-top: -10px;">
            <div class="span-7">
                <asp:Label ID="Label6" runat="server" Text="所属项目:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownProject" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label7" runat="server" Text="所属部门:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListDepartment" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-8">
                <asp:Label ID="Label111" runat="server" Text="履约情况:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListContractStatus" runat="server" CssClass="span-5">
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-23 push-1" style="margin-bottom: -15px; margin-top: -10px;">
            <div class="span-8">
                <asp:Label ID="Label2" runat="server" Text="签约日期:" CssClass="span-2"></asp:Label>
                <input id="TextBoxStartDate" type="text" onclick="WdatePicker({ isShowClear: false, readOnly: true, dateFmt: 'yyyy-MM' })"
                    class="title span-5 Wdate" onchange="return syncdate()" runat="server" readonly="readonly"
                    onkeydown="return (event.keyCode!=8)" />
            </div>
            <div class="span-6">
                <%--<asp:Label ID="Label3" runat="server" Text="至    " CssClass="span-2"></asp:Label>--%>
                <p class="span-1" style="margin: 4px 4px 0 0px; font-size: 1.2em; line-height: 1.5em;">
                    至
                </p>
                <input id="TextBoxEndDate" type="text" onclick="WdatePicker({ isShowClear: false, readOnly: true, dateFmt: 'yyyy-MM' })"
                    class="title span-5 Wdate" runat="server" readonly="readonly" onkeydown="return (event.keyCode!=8)" />
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label9" runat="server" Text="供方:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxSupplier" runat="server" CssClass="title span-5"></asp:TextBox>
                <%--<asp:DropDownList ID="DropDownSupplier" runat="server">
                </asp:DropDownList>--%>
            </div>
        </div>
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
        <div class="span-8" style="margin-bottom: 4px; margin-left: 2px;">
            <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton" OnClick="LinkButtonNew_Click">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/new.png" ImageAlign="AbsMiddle" />
                <span>新&nbsp;增</span>
            </asp:LinkButton>
            <asp:LinkButton ID="LinkButton2" runat="server" CssClass="ImageButton" OnClick="LinkButton2_Click">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/export.ico" ImageAlign="AbsMiddle" />
                <span>导&nbsp;出</span>
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
        <div class="span-3 ">
            <asp:LinkButton ID="LinkButtonDetails" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonDetails_Click">
                <asp:Image ID="ImageDetails" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>结算明细</span>
            </asp:LinkButton>
        </div>
        <div class="span-12 pager">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <div id="ScrollList" style="width: 960px; overflow: auto;">
        <cc1:CLGridViewEx ID="CLGridViewContractInfo" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" OnRowCommand="CLGridViewProjectInfo_RowCommand"
            DataKeyNames="ContractID">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <Columns>
                <asp:BoundField DataField="ContractID" HeaderText="ContractID" Visible="False" />
                <asp:BoundField DataField="version" HeaderText="version" Visible="False" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.CLGridViewContractInfo.PageIndex * this.CLGridViewContractInfo.PageSize + CLGridViewContractInfo.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="ContractCode" HeaderText="合同编码" ItemStyle-CssClass="mlength-1" />
                <%--<asp:BoundField DataField="ContractName" HeaderText="合同名称" ItemStyle-CssClass="mlength-3"
                  />--%>
                <asp:TemplateField HeaderText="合同名称">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-4' Text='<%# Bind("ContractName") %>'
                            ToolTip='<%# Eval("ContractName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="部门">
                    <ItemTemplate>
                        <asp:Label ID="LabelDepartment" runat="server" Text='<%# Bind("Department.DepartmentName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="类型">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength' Text='<%# Bind("contractType.ItemName") %>'
                            ToolTip='<%# Eval("contractType.ItemName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="项目">
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("Project.ProjectName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="供方">
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("SupplierName") %>' CssClass='mlength-2'
                            ToolTip='<%# Eval("SupplierName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="状态">
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("ContractStatus.ItemName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ContractValue" HeaderText="合同金额" DataFormatString="{0:C2}"
                    ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="ContractSettle" HeaderText="结算金额" DataFormatString="{0:C2}"
                    ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="ContractDate" DataFormatString="{0:yyyy-MM}" HeaderText="签订时间"
                    ItemStyle-CssClass="mlength-1" />
                <asp:BoundField DataField="Creator" HeaderText="Creator" Visible="False" />
                <asp:BoundField DataField="CreateDate" DataFormatString="{0:d}" HeaderText="修改时间"
                    Visible="false" />
                <asp:BoundField DataField="Modifier" HeaderText="修改人" Visible="false" />
                <%--<asp:BoundField DataField="Comment" HeaderText="备注" />--%>
                <asp:TemplateField HeaderText="备注">
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("Comment") %>' CssClass='mlength-2'
                            ToolTip='<%# Eval("Comment") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Filing" HeaderText="归档" />
            </Columns>
            <HeaderStyle CssClass="tr-header" />
        </cc1:CLGridViewEx>
    </div>
    <%--<div class="span-24">
        <asp:Label ID="Label00" runat="server" Text="合同总数:" CssClass="span-2  ">          
        </asp:Label>
        <asp:Label ID="ContractTotalNumber" runat="server" Text="123" CssClass="span-4" Font-Bold="true">          
        </asp:Label>
        <asp:Label ID="Label15" runat="server" Text="合同总金额:" CssClass="span-2">          
        </asp:Label>
        <asp:Label ID="Label16" runat="server" Text="123454.00" CssClass="span-4" Font-Bold="true"></asp:Label>
        <asp:Label ID="Label11" runat="server" Text="本页数量:" CssClass="span-2  ">          
        </asp:Label>
        <asp:Label ID="ContractNumber" runat="server" Text="232323" CssClass="span-4 " Font-Bold="true"></asp:Label>
        <asp:Label ID="Label19" runat="server" Text="本页金额:" CssClass="span-2  ">          
        </asp:Label>
        <asp:Label ID="Label20" runat="server" Text="￥12,123,456,134,567.00" CssClass="span-4"
            Font-Bold="true"></asp:Label>
    </div>
    <div class="span-24">
    </div>--%>
    <div class="span-24">
        <asp:Label ID="Label13" runat="server" Text="合同总数:" CssClass="span-2  ">          
        </asp:Label>
        <asp:Label ID="ContractTotalNumber" runat="server" Text="123" CssClass="span-2" Font-Bold="true">          
        </asp:Label>
        <asp:Label ID="Label15" runat="server" Text="合同总金额:" CssClass="span-3">          
        </asp:Label>
        <asp:Label ID="ContractTotalValue" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
        <asp:Label ID="Label10" runat="server" Text="结算总金额:" CssClass="span-3">          
        </asp:Label>
        <asp:Label ID="ContractTotalSettmentValue" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
    </div>
    <div class="span-24">
        <asp:Label ID="Label20" runat="server" Text="本页数量:" CssClass="span-2">          
        </asp:Label>
        <asp:Label ID="ContractNumber" runat="server" Text="232" CssClass="span-2 " Font-Bold="true"></asp:Label>
        <asp:Label ID="Label19" runat="server" Text="本页金额:" CssClass="span-3  ">          
        </asp:Label>
        <asp:Label ID="ContractValue" runat="server" Text="134567.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
        <asp:Label ID="Label11" runat="server" Text="本页结算:" CssClass="span-3">          
        </asp:Label>
        <asp:Label ID="ContractSettmentValue" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
    </div>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
