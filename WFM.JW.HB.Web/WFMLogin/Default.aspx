<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WFM.JW.HB.Web.WFMLogin.Default" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        .DialogForm
        {
            position: absolute;
            left: 30%;
            top: 25%;
            width: 350px;
            height: 260px;
            padding: 15px 15px 15px 15px;
            background-color: #fff;
            border: 2px solid #ccc;
            background-color: #fff;
            z-index: 100;
            display: none;
        }
        .DialogBG
        {
            display: none;
            position: absolute;
            top: 0;
            left: 0;
            height: 100%;
            width: 100%;
            z-index: 50;
            background-color: #dddddd;
            filter: alpha(opacity=60); /**/ /*IE*/
            opacity: 60; /**/ /*Firefox*/
        }
    </style>
    <script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showDialog() {
            $('div#dialog').show();
            $('div#dialog1').show();
            return false;
        }

        function hidenDialog() {
            $('div#dialog').hide();
            $('div#dialog1').hide();
            return false;
        }   
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server" class="page">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="dialog1" class="DialogForm">
        <p>
            <asp:LinkButton ID="LinkButton2" runat="server" OnCommand="LinkButton1_Command" >LinkButton</asp:LinkButton>
            test</p>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <%--<asp:PostBackTrigger ControlID="LinkButtonContractName" />--%>
                <asp:AsyncPostBackTrigger ControlID="LinkButton2" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <cc1:CLGridViewEx ID="ContractGrid" runat="server" CssClass="span-24 DataGridViewStyle"
                    AutoGenerateColumns="false" AllowDBClick="True">
                    <AlternatingRowStyle BackColor="#EEEEEE" />
                    <SelectedRowStyle CssClass="tr-selected" />
                    <Columns>
                        <asp:BoundField DataField="ContractID" HeaderText="合同id" />
                        <asp:BoundField DataField="ContractName" HeaderText="合同名称" />
                        <asp:BoundField DataField="ContractCode" HeaderText="合同代码" />
                    </Columns>
                </cc1:CLGridViewEx>
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </div>
     <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="return showDialog()"
            >LinkButton</asp:LinkButton>
    <%--<asp:LinkButton ID="LinkButton1" runat="server" 
        OnClientClick="return showDialog()" oncommand="LinkButton1_Command">LinkButton</asp:LinkButton>--%>
    </form>
</body>
</html>
