<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WFM.JW.HB.Web.WFMLogin.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        body
        {
            font: normal normal normal 100% serif;
            margin: 0;
            padding: 0;
        }
        
        .page
        {
            width: 950px;
            margin: 5px auto 0px auto;
        }
        
        .wrap
        {
            margin: 150px auto;
            width: 380px;
            overflow: hidden;
        }
        .loginForm
        {
            box-shadow: 0 0 2px rgba(0, 0, 0, 0.2), 0 1px 1px rgba(0, 0, 0, 0.2), 0 3px 0 #fff, 0 4px 0 rgba(0, 0, 0, 0.2), 0 6px 0 #fff, 0 7px 0 rgba(0, 0, 0, 0.2);
            position: absolute;
            z-index: 0;
            background-color: #FFF;
            height: 300px;
            width: 460px;
            margin: 20px 20px 5px 5px;
            filter: progid:DXImageTransform.Microsoft.gradient(GradientType=0, startColorStr='#F6E5ED', EndColorStr='#10F6E5ED');
            border: 1px solid #999;
        }
        .loginForm:before
        {
            content: '';
            position: absolute;
            z-index: -1;
            top: 2px;
            bottom: 2px;
            left: 2px;
            right: 2px;
        }
        .loginheader
        {
            text-align: center;
            
            line-height: 2.3em;
            margin: 8px 0 0px 0px;
            padding: 0 0 8px 0;
            letter-spacing: 4px;
            font: normal 20px/1.25 Microsoft YaHei, sans-serif;
            border-bottom: 1px solid #000;
            padding-left: 5px;             
        }
        
        fieldset
        {
            border: none;
            padding: 5px 10px 0;
        }
        
        
        
        fieldset input[type=text]
        {
            background: url(style/default/images/user.png) 4px 5px no-repeat;
        }
        fieldset input[type=password]
        {
            background: url(style/default/images/password.png) 4px 5px no-repeat;
        }
        fieldset input[type=text], fieldset input[type=password]
        {
            width: 100px;
            line-height: 2em;
            font-size: 14px;
            height: 24px;
            border: none;
            padding: 3px 4px 3px 1.2em;
            width: 250px;
        }
        fieldset input[type=submit]
        {
            text-align: center;
            padding: 2px 20px;
            line-height: 2em;
            border: 1px solid #FF1500;
            border-radius: 3px;
            /*background: -webkit-gradient(linear, left top, left 24, from(#FF6900), color-stop(0%, #FF9800), to(#FF6900));
            background: -moz-linear-gradient(top, #FF6900, #FF9800 0, #FF6900 24px);
            background: -o-linear-gradient(top, #FF6900, #FF9800 0, #FF6900 24px);*/
            /*filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#FF9800', endColorstr='#FF6900');*/
            /*-ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#FF9800', endColorstr='#FF6900')";*/
            height: 30px;
            cursor: pointer;
            letter-spacing: 4px;
            margin-left: 10px;
            color:  black;
            background-color:floralwhite
            
        }
        fieldset input[type=submit]:hover
        {
            background: -webkit-gradient(linear, left top, left 24, from(#FF9800), color-stop(0%, #FF6900), to(#FF9800));
            background: -moz-linear-gradient(top, #FF9800, #FF6900 0, #FF9800 24px);
            background: -o-linear-gradient(top, #FF6900, #FF6900 0, #FF9800 24px);
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#FF6900', endColorstr='#FF9800');
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#FF6900', endColorstr='#FF9800')";
        }
        
        .inputlabel
        {
           display: inline-block;
            width: 60px;
            text-align: right;
            font: normal 15px/1.25 Microsoft YaHei, sans-serif;
        }
        
        .inputWrap
        {
           display: inline-block;
            width: 230px;
            background: -webkit-gradient(linear, left top, left 24, from(#FFFFFF), color-stop(4%, #EEEEEE), to(#FFFFFF));
            background: -moz-linear-gradient(top, #FFFFFF, #EEEEEE 1px, #FFFFFF 24px);
            background: -o-linear-gradient(top, #FFFFFF, #EEEEEE 1px, #FFFFFF 24px);
            border-radius: 3px;
            border: 1px solid #CCC;
            margin: 10px 10px 0;
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#EEEEEE', endColorstr='#FFFFFF');
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#EEEEEE', endColorstr='#FFFFFF')";
        }
        fieldset input[type=checkbox]
        {
            margin-left: 10px;
            vertical-align: middle;
        }
        fieldset a
        {
            color: blue;
            font-size: 14px;
            margin: 6px 0 0 10px;
            text-decoration: none;
        }
        fieldset a:hover
        {
            text-decoration: underline;
        }
        fieldset span
        {
            font-size: 14px;
        }       
        
        .showborder
        {
            border: 1px solid #000;
        }
        
        table
        {
            width: 950px;
            height: 610px;
            background-position: center center;
          /* background-image: url('Image/CSCL-Login.png');*/
            background-repeat: no-repeat;
            border-collapse: collapse;
            border-spacing: 1px;
        }
        
        .login
        {
            position: relative;
            top: -80px;
            left: 490px;
            width: 360px;
            height: 230px;
            filter: progid:DXImageTransform.Microsoft.gradient(GradientType=0, startColorStr='#F6E5ED', EndColorStr='#10F6E5ED');
            background-image: -moz-linear-gradient(left, #4b6c9e, #ffffff);
        }
    </style>
    <title>湖北纪委精准扶贫政策落实情况监督检查管理系统</title>
</head>
<body onload="JavaScript:form1.userName.focus()">
    <form id="form1" runat="server" class="page">
    <table>
        <tr>
            <td>
                <div class="login loginForm">
                    <div class="loginheader">
                        精准扶贫政策落实情况监督检查问题核查管理系统</div>
                    <fieldset>
                        <div class="inputlabel">
                            用户名:</div>
                        <div class="inputWrap">
                            <asp:TextBox ID="userName" runat="server"></asp:TextBox>
                        </div>
                        <div class="inputlabel">
                            密 码:</div>
                        <div class="inputWrap">
                            <asp:TextBox ID="password" TextMode="Password" runat="server"></asp:TextBox>
                        </div>
                    </fieldset>
                    <fieldset style="margin-top: 5px; width: 320px; height: 16px; display: inline-block">
                        <asp:Label ID="ErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                        <%--<div class="inputlabel">
                            <asp:CheckBox ID="CheckBox1" Checked="false" runat="server"></asp:CheckBox>
                        </div>
                        <span>下次自动登录</span>--%>
                    </fieldset>
                    <fieldset style="text-align: right">
                        <%--                        <div style="width: 220px; display: inline-block">
                            <asp:Label ID="Label1" runat="server" Text="test"></asp:Label></div>--%>
                        <asp:Button ID="LoginButton" runat="server" Text="登录" 
                            onclick="LoginButton_Click"></asp:Button>
                    </fieldset>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
