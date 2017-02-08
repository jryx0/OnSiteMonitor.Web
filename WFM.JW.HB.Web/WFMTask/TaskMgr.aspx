<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TaskMgr.aspx.cs" Inherits="WFM.JW.HB.Web.WFMTask.TaskMgr" %>

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
            <div class="span-7">
                <asp:Label ID="Label6" runat="server" Text="单位:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListCity" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CityList_IndexChanged">
                </asp:DropDownList>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label7" runat="server" Text="辖区单位:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListContry" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-8">
                <asp:Label ID="Label111" runat="server" Text="上报情况:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListContractStatus" runat="server" CssClass="span-5">
                    <asp:ListItem Value="-1">全部</asp:ListItem>
                    <asp:ListItem Value="1">正式上报数据</asp:ListItem>
                    <asp:ListItem Value="0">非上报数据</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-23 push-1" style="margin-bottom: -15px; margin-top: -10px;">
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
            <asp:LinkButton ID="LinkButtonModify" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonModify_Click">
                <asp:Image ID="ImageModify" runat="server" ImageUrl="~/Styles/Img/modify.ico" ImageAlign="AbsMiddle" />
                <span>修&nbsp;改</span>
            </asp:LinkButton>
        </div>
        <div class="span-3 ">
        </div>
        <div class="span-12 pager">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <div id="ScrollList" style="width: 960px; overflow: auto;  ">
        <cc1:CLGridViewEx ID="CLGridViewTaskInfo" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" OnRowCommand="CLGridViewTaskInfo_RowCommand"
            DataKeyNames="TaskGuid">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <Columns>
                <asp:BoundField DataField="TaskGuid" HeaderText="TaskGuid" Visible="False" />
                <asp:BoundField DataField="Version" HeaderText="Version" Visible="False" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.CLGridViewTaskInfo.PageIndex * this.CLGridViewTaskInfo.PageSize + CLGridViewTaskInfo.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="上级单位">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("ParentName") %>'
                            ToolTip='<%# Eval("ParentName") %>'></asp:Label>
                    </ItemTemplate>
                     
                </asp:TemplateField>
                <asp:TemplateField HeaderText="单位">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-2' Text='<%# Bind("RegionName") %>'
                            ToolTip='<%# Eval("RegionName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="数据名称">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-5' Text='<%# Bind("ClientTaskName") %>'
                            ToolTip='<%# Eval("ClientTaskName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="是否上报">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Eval("TStatus").ToString() == "1" ? "上报数据" : "" %>'
                            ToolTip='<%# Eval("TStatus") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="上传时间">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-3' Text='<%# Bind("CreateTime") %>'
                            ToolTip='<%# Eval("CreateTime") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Username" HeaderText="上传用户" />
                <asp:TemplateField HeaderText="数据状态">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-2' Text='<%# SetStatus(Eval("Status").ToString())  %>'
                            ToolTip='<%# SetStatus(Eval("Status").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作时间">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-3' Text='<%# Bind("OpTime") %>'
                            ToolTip='<%# Eval("OpTime") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="备注">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-2' Text='<%# Bind("ErrorMessage") %>'
                            ToolTip='<%# Eval("ErrorMessage") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="tr-header"  HorizontalAlign="Center"/>
        </cc1:CLGridViewEx>
    </div>
   
  <%--  <div class="span-24">
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
    </div>--%>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
