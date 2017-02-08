<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ContractEdit.aspx.cs" Inherits="WFM.JW.HB.Web.CMSProject.ContractEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Scripts/jquery-ui-1.8.21.custom.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-ui-1.8.21.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        .Wdate
        {
            border: #999 1px solid;
            height: 20px;
            background: #fff url(../Styles/Img/datePicker.gif) no-repeat right;
        }
    </style>
    <script type="text/javascript">

        $(function () {
            $(".suppliername").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "WebSupplierServices.asmx/GetSupplierName",
                        data: "{'name':'" + request.term + "'}",
                        dataType: "json",
                        async: true,
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (result) {
                            alert("error！");
                        }
                    });
                },
                minLength: 1

            });
        });

        function CheckDroplist() {
            var ddlVal = $('#<%=DropDownListDepartment.ClientID%>').val();

            if (ddlVal == 'all') {
                alert("请选择部门名称!");
                return false;
            }

            ddlVal = $('#<%=DropDownListContractStatus.ClientID%>').val();

            if (ddlVal == 'all') {
                alert("请选择合同状态名称!");
                return false;
            }

            ddlVal = $('#<%=DropDownProject.ClientID%>').val();

            if (ddlVal == 'all') {
                alert("请选择项目名称!");
                return false;
            }

            ddlVal = $('#<%=DropDownListContractType.ClientID%>').val();

            if (ddlVal == 'all') {
                alert("请选择合同类型名称!");
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="ContractID" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    <div class="span-23 Query groupingborder">
        <div class="span-23 prepend-1">
            <div class="span-10  Magin-0">
                <asp:Label ID="Label4" runat="server" Text="所属部门:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownListDepartment" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-10  Magin-0">
                <asp:Label ID="Label3" runat="server" Text="所属项目:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownProject" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-23 prepend-1">
            <div class="span-10  Magin-0">
                <asp:Label ID="Label2" runat="server" Text="合同类别:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownListContractType" runat="server">
                </asp:DropDownList>
            </div>
            <div class="span-10  Magin-0">
                <asp:Label ID="Label9" runat="server" Text="履约情况:" CssClass="span-3"></asp:Label>
                <asp:DropDownList ID="DropDownListContractStatus" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-23 prepend-1">
            <div class="span-10  Magin-0">
                <asp:Label ID="Label5" runat="server" Text="合同编号:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxContractCode" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-10  Magin-0">
                <asp:Label ID="Label6" runat="server" Text="合同名称:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxContractName" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
        </div>
        <div class="span-23 prepend-1">
            <div class="span-10  Magin-0">
                <asp:Label ID="Label8" runat="server" Text="合同金额:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxValue" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
            <div class="span-10  Magin-0">
                <asp:Label ID="Label10" runat="server" Text="签约时间:" CssClass="span-3"></asp:Label>
                <input id="ContractDate" type="text" onclick="WdatePicker({isShowClear:false,readOnly:true})"
                    class="title span-5 Wdate" runat="server" readonly="readonly" />
            </div>
        </div>
        <div class="span-23 prepend-1">
            <div class="span-10 Magin-0">
                <asp:Label ID="Label7" runat="server" Text="供方:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxSupplier" runat="server" CssClass="title span-5 suppliername"></asp:TextBox>
                <%--<asp:DropDownList ID="DropDownSupplier" runat="server">
                </asp:DropDownList>--%>
            </div>
            <div class="span-10 Magin-0">
                <asp:Label ID="Label1" runat="server" Text="备注:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxComment" runat="server" CssClass="title span-5"></asp:TextBox>
            </div>
        </div>

         <div class="span-23 prepend-1">
            <div class="span-10 Magin-0">
                <asp:Label ID="Label11" runat="server" Text="归档:" CssClass="span-3"></asp:Label>
                <asp:TextBox ID="TextBoxFiling" runat="server" CssClass="title span-5"></asp:TextBox>
                
            </div>
           
        </div>

        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelInfo" runat="server" Text=""></asp:Label>
        </div>
        <div class="span-10 Magin-1 push-7">
            <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
                <asp:LinkButton ID="LinkButtonApply" runat="server" CssClass="ImageButton" OnClientClick="return CheckDroplist()"
                    OnCommand="LinkButtonApply_Command">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                    <span>应&nbsp;&nbsp;用</span></asp:LinkButton>
                <asp:LinkButton ID="LinkButtonSave" runat="server" CssClass="ImageButton" OnClientClick="return CheckDroplist()"
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
