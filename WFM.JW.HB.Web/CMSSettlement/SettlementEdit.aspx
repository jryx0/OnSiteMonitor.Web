<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SettlementEdit.aspx.cs" Inherits="WFM.JW.HB.Web.CMSSettlement.SettlementEdit" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Styles/Dialog.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .Wdate
        {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
    <script type="text/javascript">

        function showDialog() {
           // $('div#bg').show();
            $('div#dialog').show();
         //   return true;
        }

        function cancelDialog() {
            $('div#dialog').hide();
            $('div#bg').hide();
            return false;
        }

        function okDialog() {
            $('div#dialog').hide();
            $('div#bg').hide();
            return true;
        }

        function DataCheck() {
            var contractid = $('input[id$=ContractID]').val();
            if (contractid == "0") {
                alert("请选择合同");
                return false;
            }
            return true; //check contractid 
        }

        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="ContractID" Value="0" runat="server" />
    <asp:HiddenField ID="SettlementID" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="dialog" class="DialogForm Query ">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="LinkButtonContractName" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                </asp:UpdateProgress>
                <div class="Query span-17 groupingborder" style="margin-left: -5px;">
                    <div class="span-7">
                        <asp:Label ID="Label12" runat="server" Text="合同名称:" CssClass="span-2"></asp:Label>
                        <asp:TextBox ID="TextBoxContractNamesearch" runat="server" CssClass="title span-4"></asp:TextBox>
                    </div>
                    <div class="span-7">
                        <asp:Label ID="Label13" runat="server" Text="结算编号:" CssClass="span-2"></asp:Label>
                        <asp:TextBox ID="TextBoxContractCodesearch" runat="server" CssClass="title span-4"></asp:TextBox>
                    </div>
                    <asp:LinkButton ID="LinkButton3" runat="server" CssClass="LittleImageButton" OnClick="LinkButtonContract_Click"
                        OnClientClick="return showDialog()">
                        <asp:Image ID="Image8" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                        <span>查&nbsp;询</span>
                    </asp:LinkButton>
                </div>
                <div class="span-16 pager" style="text-align: right;">
                    <uc1:Pages ID="Pager" runat="server" />
                </div>
                <div id="ScrollList" style="width: 650px; height: 290px; overflow: auto; padding-left: 20px;
                    margin-left: -15px;">
                    <cc1:CLGridViewEx ID="ContractGrid" runat="server" AutoGenerateColumns="False" CssClass="span-24 DataGridViewStyle"
                        DataKeyNames="ContractID" AllowDBClick="true"  PageSize="8" 
                        onrowcommand="ContractGrid_RowCommand" >
                        <AlternatingRowStyle BackColor="#EEEEEE" />
                        <SelectedRowStyle CssClass="tr-selected" />
                        <HeaderStyle CssClass="tr-header" />
                        <Columns>
                            <asp:TemplateField HeaderText="序号">
                                <ItemTemplate>
                                    <%# this.ContractGrid.PageIndex * this.ContractGrid.PageSize + ContractGrid.Rows.Count + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ContractID" HeaderText="ContractID" Visible="false" />
                            <asp:BoundField DataField="ContractName" HeaderText="合同名称" />
                            <asp:BoundField DataField="ContractCode" HeaderText="合同代码" />
                        </Columns>
                    </cc1:CLGridViewEx>
                    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="margin-top: 10px; text-align: right; padding-right: 40px;">
            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="ImageButton" OnClientClick="return okDialog()">
                <asp:Image ID="Image6" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                <span>确&nbsp;&nbsp;定</span></asp:LinkButton>
            <asp:LinkButton ID="LinkButton2" runat="server" CssClass="ImageButton" OnClientClick="return cancelDialog()">
                <asp:Image ID="Image7" runat="server" ImageUrl="~/Styles/Img/save.ico" ImageAlign="AbsMiddle" />
                <span>取&nbsp;&nbsp;消</span></asp:LinkButton></div>
    </div>
    <div class="span-23 Query groupingborder">
        <div class="span-10 prepend-1 Magin-1 ">
            <asp:Label ID="Label3" runat="server" Text="合同名称:" CssClass="span-3 "></asp:Label>
            <input id="TextBoxContractName" type="text" class="title span-5 readonly" runat="server" onkeydown="return (event.keyCode!=8)"
                readonly="readonly" />
            <asp:LinkButton ID="LinkButtonContractName" runat="server" CssClass="LittleImageButton"
                OnClick="LinkButtonContract_Click" OnClientClick="return showDialog()" Visible="false">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
            </asp:LinkButton>
        </div>
        <div class="span-10 prepend-1 Magin-1 ">
            <asp:Label ID="Label4" runat="server" Text="合同编号:" CssClass="span-3"></asp:Label>
            <input id="TextBoxContractCode" type="text" class="title span-5 readonly" runat="server" onkeydown="return (event.keyCode!=8)"
                readonly="readonly" />
            <asp:LinkButton ID="LinkButtonContractCode" runat="server" CssClass="LittleImageButton"
                OnClick="LinkButtonContract_Click" OnClientClick="return showDialog()" Visible="true">
                <asp:Image ID="Image5" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
            </asp:LinkButton>
        </div>
        <div class="span-10 prepend-1 Magin-1 ">
            <asp:Label ID="Label1" runat="server" Text="结算编号:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxSettlementCode" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-10 prepend-1 Magin-1 ">
            <asp:Label ID="Label11" runat="server" Text="结算日期:" CssClass="span-3"></asp:Label>
            <input id="TextBoxSettlementDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:false})"
                class="title span-5 Wdate" runat="server"  onkeydown="return (event.keyCode!=8)" />
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label8" runat="server" Text="结算状态:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="DropDownListStatus" runat="server">
            </asp:DropDownList>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label7" runat="server" Text="归档:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxFiling" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="LabelUserName" runat="server" Text="报审金额:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxDeclared" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label5" runat="server" Text="增加变更:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxChange" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="EnableLabel" runat="server" Text="审核金额:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxAudited" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label2" runat="server" Text="审减金额:" CssClass="span-3"></asp:Label>
            <input id="TextBoxDeduction" type="text" class="title span-5" runat="server" onkeydown="return (event.keyCode!=8)"
                readonly="readonly" />
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label9" runat="server" Text="审核人:" CssClass="span-3"></asp:Label>
            <asp:DropDownList ID="DropDownListAuditor" runat="server">
            </asp:DropDownList>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label6" runat="server" Text="备注:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxComment" runat="server" CssClass="title span-5"></asp:TextBox>
        </div>
        <div class="span-10 prepend-1 Magin-1">
            <asp:Label ID="Label10" runat="server" Text="对账人:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="TextBoxStatementor" runat="server" CssClass="title span-5"></asp:TextBox>
            <%-- <asp:DropDownList ID="DropDownListStatementor" runat="server">
            </asp:DropDownList>--%>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelInfo" runat="server" Text=""></asp:Label>
        </div>
        <div class="span-23 Magin-1 push-8">
            <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
                <asp:LinkButton ID="LinkButtonApply" runat="server" CssClass="ImageButton" OnClientClick="return DataCheck()"
                    OnCommand="LinkButtonApply_Command">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                    <span>应&nbsp;&nbsp;用</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButtonSave" runat="server" CssClass="ImageButton" OnClientClick="return DataCheck()"
                    OnCommand="LinkButtonSave_Command">
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/save.ico" ImageAlign="AbsMiddle" />
                    <span>保&nbsp;&nbsp;存</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="ImageButton " OnCommand="LinkButtonBack_Command">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/undo.ico" ImageAlign="AbsMiddle" />
                    <span>返&nbsp;&nbsp;回</span></asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>
