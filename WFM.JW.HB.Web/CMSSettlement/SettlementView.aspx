<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SettlementView.aspx.cs" Inherits="WFM.JW.HB.Web.CMSSettlement.SettlementView" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        function RefreshContext() {
            $('input[id$=TextBoxSettlementCode]').attr('value', '');
            $('input[id$=TextBoxSettlementStartDate]').attr('value', '');
            $('input[id$=TextBoxSettlementEndDate]').attr('value', '');
            $('select[id$=DropDownListSettlementStatus]')[0].selectedIndex = 0;
            $('select[id$=DropDownListAuditor]')[0].selectedIndex = 0;
            $('input[id$=TextBoxStatementor]').attr('value', '');
            $('input[id$=TextBoxContractName]').attr('value', '');
            $('input[id$=TextBoxContractCode]').attr('value', '');

            return false;
        }

        function DeleteCheck() {
            return confirm("确认删除结算数据么？");
        }

        $(document).ready(function () {
            var message = $('input[id$=ShowDialog]').val();
            if (message.length > 1) {
                alert(message);
                $('input[id$=ShowDialog]').val("");
            }

        });
    
    
    </script>
    <style type="text/css">
        .Wdate
        {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="ContractID" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    <div class="span-23 Query groupingborder">
        <div class="span-24 ">
            <div class="span-7 prepend-1">
                <asp:Label ID="Label1" runat="server" Text="结算编号:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxSettlementCode" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label2" runat="server" Text="结算日期:" CssClass="span-2"></asp:Label>
                <input id="TextBoxSettlementStartDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true})"
                    class="title span-5 Wdate" runat="server" readonly="readonly" onkeydown="return (event.keyCode!=8)" />
            </div>
            <div class="span-7 prepend-1">
                <%--<asp:Label ID="Label3" runat="server" Text="至    " CssClass="span-2"></asp:Label>--%>
                <p class="span-2" style="margin: 4px 4px 0 0px; font-size: 1.2em; line-height: 1.5em;">
                    至</p>
                <input id="TextBoxSettlementEndDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true})"
                    class="title span-5 Wdate" runat="server" readonly="readonly" onkeydown="return (event.keyCode!=8)" />
            </div>
        </div>
        <div class="span-24" style="margin-top: 10px;">
            <div class="span-7 prepend-1">
                <asp:Label ID="Label6" runat="server" Text="结算状态:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListSettlementStatus" runat="server">
                    <%--<asp:ListItem Value="all" Selected="True">全部</asp:ListItem>
                    <asp:ListItem Value="true">已结算</asp:ListItem>
                    <asp:ListItem Value="false">未结算</asp:ListItem>--%>
                </asp:DropDownList>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label5" runat="server" Text="审核人:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownListAuditor" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-7 prepend-1">
                <asp:Label ID="Label4" runat="server" Text="对账人:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxStatementor" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
        </div>
        <div class="span-24 ">
            <div class="span-7 prepend-1" style="margin-top: 10px; margin-bottom: 0px">
                <asp:Label ID="Label3" runat="server" Text="合同名称:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxContractName" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7" style="margin-top: 10px; margin-bottom: 0px">
                <asp:Label ID="Label7" runat="server" Text="合同编号:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxContractCode" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-6 push-2 " style="margin-top: 10px; margin-bottom: 0px">
                
                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="ImageButton" OnClientClick="return RefreshContext()">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/refresh.png" ImageAlign="AbsMiddle" />
                    <span>清&nbsp;空</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButtonQuery" runat="server" CssClass="ImageButton" OnClick="LinkButtonQuery_Click">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                    <span>查&nbsp;&nbsp;询</span></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="span-24 DataArea">
        <div class="span-6" style="margin-bottom: 4px; margin-left: 2px;">
            <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton" OnCommand="LinkButtonNew_Command">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/new.png" ImageAlign="AbsMiddle" />
                <span>新&nbsp;增</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonModify" runat="server" CssClass="ImageButton hidden"
                OnCommand="LinkButtonModify_Command">
                <asp:Image ID="ImageModify" runat="server" ImageUrl="~/Styles/Img/modify.ico" ImageAlign="AbsMiddle" />
                <span>修&nbsp;改</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonDelete" runat="server" CssClass="ImageButton hidden"
                OnClientClick='return DeleteCheck()' OnClick="LinkButtonDelete_Click">
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
    <div id="ScrollList" style="width: 960px; overflow: auto;">
        <cc1:CLGridViewEx ID="CLGridViewSettlement" runat="server" CssClass="span-24 DataGridViewStyle"
            AutoGenerateColumns="False" AllowDBClick="True" DataKeyNames="SettlementInfoID"
            OnRowCommand="CLGridViewSettlement_RowCommand">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <Columns>
                <asp:BoundField DataField="SettlementInfoID" HeaderText="SettlementInfoID" Visible="False" />
                <asp:BoundField DataField="version" HeaderText="version" Visible="False" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.CLGridViewSettlement.PageIndex * this.CLGridViewSettlement.PageSize + CLGridViewSettlement.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="结算状态">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("SettlementStatus.ItemName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SettlementDate" HeaderText="结算日期" DataFormatString="{0:yyyy-MM}"
                    ItemStyle-CssClass="mlength-0" />
                <asp:TemplateField HeaderText="合同编号">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Contract.ContractCode") %>'
                            CssClass="mlength-1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="合同名称">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Contract.ContractName") %>'
                            ToolTip='<%# Eval("Contract.ContractName") %>' CssClass="mlength-2"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SettlementCode" HeaderText="结算编号" />
                <asp:BoundField DataField="AmountDeclared" HeaderText="报审金额" DataFormatString="{0:C2}"
                    ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="AmountAudited" HeaderText="审核金额" DataFormatString="{0:C2}"
                    ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="AuditDeduction" HeaderText="审减金额" DataFormatString="{0:C2}"
                    ItemStyle-HorizontalAlign="Right" />
                <asp:TemplateField HeaderText="审核人">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Auditor") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="对账人">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Statementor") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Filing" HeaderText="归档" />
                <asp:BoundField DataField="Comment" HeaderText="备注" />
            </Columns>
            <HeaderStyle CssClass="tr-header" />
        </cc1:CLGridViewEx>
    </div>
  
    <div class="span-24">
        <asp:Label ID="Label13" runat="server" Text="结算总数:" CssClass="span-2 Numbers ">          
        </asp:Label>
        <asp:Label ID="TotalNumber" runat="server" Text="123" CssClass="span-2" Font-Bold="true">          
        </asp:Label>
        <asp:Label ID="Label15" runat="server" Text="报审总金额:" CssClass="span-3 pull-1 Numbers">          
        </asp:Label>
        <asp:Label ID="TotalDeclareValue" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
        <asp:Label ID="Label10" runat="server" Text="审核总金额:" CssClass="span-3 pull-1 Numbers">          
        </asp:Label>
        <asp:Label ID="TotalSettmentValue" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
        <asp:Label ID="Label8" runat="server" Text="审减总金额:" CssClass="span-3 pull-1 Numbers">          
        </asp:Label>
        <asp:Label ID="TotalDeduce" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
    </div>
    <div class="span-24">
        <asp:Label ID="Label20" runat="server" Text="本页数量:" CssClass="span-2 Numbers">          
        </asp:Label>
        <asp:Label ID="SettleNumber" runat="server" Text="232" CssClass="span-2 " Font-Bold="true"></asp:Label>
        <asp:Label ID="Label19" runat="server" Text="报审金额:" CssClass="span-2  Numbers">          
        </asp:Label>
        <asp:Label ID="DeclareValue" runat="server" Text="134567.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
        <asp:Label ID="Label11" runat="server" Text="审核金额:" CssClass="span-2 Numbers">          
        </asp:Label>
        <asp:Label ID="SettmentValue" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
        <asp:Label ID="Label12" runat="server" Text="审减金额:" CssClass="span-2 Numbers">          
        </asp:Label>
        <asp:Label ID="SettleDeduce" runat="server" Text="123454.00" CssClass="span-4 Numbers"
            Font-Bold="true"></asp:Label>
    </div>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="t" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
