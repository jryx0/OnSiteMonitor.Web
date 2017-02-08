<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SAccountQuery.aspx.cs" Inherits="WFM.JW.HB.Web.CMSSettlement.SAccountQuery"
    EnableViewState="true" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .Wdate
        {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
    <script type="text/javascript">
        function syncdate() {
            var newdate = $('input[id$=TextBoxStartDate]').attr('value');
            $('input[id$=TextBoxEndDate]').attr('value', newdate);

            return false;
        }

        function RefreshContext() {
            $('input[id$=TextBoxProject]').attr('value', '');
            $('input[id$=TextBoxContractName]').attr('value', '');
            $('input[id$=TextBoxStartDate]').attr('value', '');
            $('input[id$=TextBoxEndDate]').attr('value', '');
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="span-23 Query groupingborder">
        <div class="span-23 ">
            <div class="span-7">
                <asp:Label ID="Label3" runat="server" Text="项目名称:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxProject" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-7">
                <asp:Label ID="Label8" runat="server" Text="合同名称:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="TextBoxContractName" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-4 ">
                <asp:CheckBox ID="CheckBoxAllContract" runat="server" CssClass="span-1" />
                <asp:Label ID="Label18" runat="server" Text="显示全部合同" CssClass="span-3"></asp:Label>
            </div>
        </div>
        <div class="span-23 " style="margin-top: 10px;">
            <div class="span-7 ">
                <asp:Label ID="Label2" runat="server" Text="签约日期:" CssClass="span-2"></asp:Label>
                <input id="TextBoxStartDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true,dateFmt:'yyyy-MM'})"
                    class="title span-5 Wdate" runat="server" readonly="readonly" onkeydown="return (event.keyCode!=8)" />
            </div>
            <div class="span-7 prepend-1">
                <%--<asp:Label ID="Label3" runat="server" Text="至    " CssClass="span-2"></asp:Label>--%>
                <p class="span-1" style="margin: 4px 4px 0 0px; font-size: 1.2em; line-height: 1.5em;">
                    至</p>
                <input id="TextBoxEndDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true,dateFmt:'yyyy-MM'})"
                    class="title span-5 Wdate" runat="server" readonly="readonly" onkeydown="return (event.keyCode!=8)" />
            </div>
            <div class="span-6 push-2 " style="margin-bottom: -5px">
                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="ImageButton" OnClientClick="return RefreshContext()">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/refresh.png" ImageAlign="AbsMiddle" />
                    <span>清&nbsp;&nbsp;空</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="ImageButton" OnClick="LinkButton1_Click">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                    <span>查&nbsp;&nbsp;询</span></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="span-24 DataArea">
        <div class="span-8" style="margin-bottom: 4px; margin-left: 2px;">
            <asp:LinkButton ID="ButtonExport" runat="server" CssClass="ImageButton" 
                onclick="ButtonExport_Click" >
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Styles/Img/export.ico" ImageAlign="AbsMiddle" />
                <span>导&nbsp;出</span></asp:LinkButton> <asp:LinkButton ID="LinkButtonContractModify" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonContractModify_Click">
                <asp:Image ID="ImageModify" runat="server" ImageUrl="~/Styles/Img/modify.ico" ImageAlign="AbsMiddle" />
                <span>合同管理</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButtonSettlementModify" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonSettlementModify_Click">
                <asp:Image ID="ImageDelete" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>结算管理</span></asp:LinkButton>
           
        </div>
        <div class="span-3 ">
            <asp:LinkButton ID="LinkButtonDetails" runat="server" CssClass="ImageButton hiddencontrol ">
                <asp:Image ID="ImageDetails" runat="server" ImageUrl="~/Styles/Img/detail.ico" ImageAlign="AbsMiddle" />
                <span>明&nbsp;细</span></asp:LinkButton>
        </div>
        <div class="span-12 pager">
            <uc1:Pages ID="Pager" runat="server" />
        </div>
    </div>
    <div id="ScrollList" style="width: 960px; overflow: auto;">
        <cc1:CLGridViewEx ID="CLGridViewSAccount" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" DataKeyNames="SettlementInfoID,Contract"
            OnDataBound="CLGridViewSAccount_DataBound" OnRowCommand="CLGridViewSAccount_RowCommand">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <Columns>
                <asp:BoundField DataField="SettlementInfoID" HeaderText="SettlementInfoID" Visible="false" />
                <asp:BoundField DataField="Contract.ContractID" HeaderText="ContractID" Visible="false" />
                <asp:TemplateField HeaderText="序号">
                    <ItemTemplate>
                        <%# this.CLGridViewSAccount.PageIndex * this.CLGridViewSAccount.PageSize + CLGridViewSAccount.Rows.Count + 1%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="签约日期">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Width="65px" Text='<%# String.Format("{0:yyyy-MM}", Eval("Contract.ContractDate")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="项目名称">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Contract.Project.ProjectName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--                <asp:TemplateField HeaderText="合同编号">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass="mlength-0" Text='<%# Bind("Contract.ContractCode") %>'
                            ToolTip='<%# Eval("Contract.ContractCode") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="合同名称">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" CssClass="mlength-3" Text='<%# Bind("Contract.ContractName") %>'
                            ToolTip='<%# Eval("Contract.ContractName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="合同金额" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# ((double)Eval("Contract.ContractValue") == 0) ? "" : String.Format("{0:C2}",Eval("Contract.ContractValue")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="结算日期">
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Width="65px" Text='<%# ((DateTime)Eval("SettlementDate") == DateTime.MinValue) ? "" : String.Format("{0:yyyy-MM}", Eval("SettlementDate")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SettlementCode" HeaderText="结算编号" />
                <asp:TemplateField HeaderText="报审金额" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%# ((double)Eval("AmountDeclared") == 0) ? "" : String.Format("{0:c2}",Eval("AmountDeclared")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="审核金额" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" Text='<%# ((double)Eval("AmountAudited") == 0)  ? "" :  String.Format("{0:c2}",Eval("AmountAudited")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="审减金额" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="Label7" runat="server" Text='<%#((double)Eval("AuditDeduction") == 0)  ? ""  :  String.Format("{0:c2}",Eval("AuditDeduction"))  %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="增加变更" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%# ((double)Eval("AmountChanged") == 0) ? "" :  String.Format("{0:c2}",Eval("AmountChanged")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Auditor" HeaderText="审核人" />
                <asp:BoundField DataField="Statementor" HeaderText="对账人" />
                <asp:TemplateField HeaderText="履约情况">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Contract.ContractStatus.ItemName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="结算状态">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("SettlementStatus.ItemName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="供方">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass="mlength-2" Text='<%# Bind("Contract.SupplierName") %>'
                            ToolTip='<%# Eval("Contract.SupplierName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Comment" HeaderText="备注" />
                <asp:BoundField DataField="Filing" HeaderText="归档" />
            </Columns>
            <HeaderStyle CssClass="tr-header" />
        </cc1:CLGridViewEx>
    </div>
    <div>
        <div class="span-24">
            <asp:Label ID="Label12" runat="server" Text="合同总数:" CssClass="span-3  Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractTotalNumber" runat="server" Text="123" CssClass="span-5 "
                Font-Bold="true">          
            </asp:Label>
            <asp:Label ID="Label21" runat="server" Text="本页合同总数:" CssClass="span-3 Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractNumber" runat="server" Text="123" CssClass="span-5  " Font-Bold="true">          
            </asp:Label>
        </div>
        <div class="span-24">
            <asp:Label ID="Label9" runat="server" Text="结算总数:" CssClass="span-3  Numbers ">          
            </asp:Label>
            <asp:Label ID="TotalSettlementNumber" runat="server" Text="123" CssClass="span-5 "
                Font-Bold="true">          
            </asp:Label>
            <asp:Label ID="Label11" runat="server" Text="本页结算总数:" CssClass="span-3 Numbers ">          
            </asp:Label>
            <asp:Label ID="SettlementNumber" runat="server" Text="123" CssClass="span-5  " Font-Bold="true">          
            </asp:Label>
        </div>
        <div class="span-24">
            <asp:Label ID="Label23" runat="server" Text="合同总金额:" CssClass="span-3  Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractTotalValue" runat="server" Text="123" CssClass="span-5 " Font-Bold="true">          
            </asp:Label>
            <asp:Label ID="Label25" runat="server" Text="本页合同总金额:" CssClass="span-3 Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractValue" runat="server" Text="123" CssClass="span-5  " Font-Bold="true">          
            </asp:Label>
        </div>
        <div class="span-24">
            <asp:Label ID="Label27" runat="server" Text="合同报审总金额:" CssClass="span-3  Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractTotalSettmentDeclare" runat="server" Text="123" CssClass="span-5 "
                Font-Bold="true">          
            </asp:Label>
            <asp:Label ID="Label111" runat="server" Text="本页合同报审金额:" CssClass="span-3 Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractSettmentDeclare" runat="server" Text="123" CssClass="span-5  "
                Font-Bold="true">          
            </asp:Label>
        </div>
        <div class="span-24">
            <asp:Label ID="Label31" runat="server" Text="合同审核总金额:" CssClass="span-3  Numbers ">          
            </asp:Label>
            <asp:Label ID="ContractTotalSettmentValue" runat="server" Text="123" CssClass="span-5 "
                Font-Bold="true">          
            </asp:Label>
            <asp:Label ID="Label33" runat="server" Text="本页合同审核金额:" CssClass="span-3 Numbers ">          
            </asp:Label>
            <asp:Label ID="SettmentAuditValue" runat="server" Text="123" CssClass="span-5  "
                Font-Bold="true">          
            </asp:Label>
        </div>
        <div class="span-24">
            <asp:Label ID="Label35" runat="server" Text="审减总金额:" CssClass="span-3  Numbers ">          
            </asp:Label>
            <asp:Label ID="TotalDeduceValue" runat="server" Text="123" CssClass="span-5 " Font-Bold="true">          
            </asp:Label>
            <asp:Label ID="Label37" runat="server" Text="本页审减金额:" CssClass="span-3 Numbers ">          
            </asp:Label>
            <asp:Label ID="DeduceValue" runat="server" Text="123" CssClass="span-5  " Font-Bold="true">          
            </asp:Label>
        </div>
    </div>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
</asp:Content>
