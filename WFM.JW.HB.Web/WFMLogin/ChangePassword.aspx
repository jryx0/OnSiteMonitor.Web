<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="WFM.JW.HB.Web.WFMLogin.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ChangePassword ID="ChangePassword2" runat="server">
        <ChangePasswordTemplate>
            <div class="span-23 Query groupingborder">
                <div class="span-23 prepend-1 Magin-1">
                    <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"
                        CssClass="span-3">密码:</asp:Label>
                    <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password" CssClass="title span-5"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                        ErrorMessage="必须填写“密码”。" ToolTip="必须填写“密码”。" ValidationGroup="ChangePassword1">*</asp:RequiredFieldValidator>
                </div>
                <div class="span-23 prepend-1 Magin-1">
                    <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"
                        CssClass="span-3">新密码:</asp:Label>
                    <asp:TextBox ID="NewPassword" runat="server" TextMode="Password" CssClass="title span-5"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                        ErrorMessage="必须填写“新密码”。" ToolTip="必须填写“新密码”。" ValidationGroup="ChangePassword1">*</asp:RequiredFieldValidator>
                </div>
                <div class="span-23 prepend-1 Magin-1">
                    <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"
                        CssClass="span-3">确认新密码:</asp:Label>
                    <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" CssClass="title span-5"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                        ErrorMessage="必须填写“确认新密码”。" ToolTip="必须填写“确认新密码”。" ValidationGroup="ChangePassword1">*</asp:RequiredFieldValidator>
                </div>
                <div class="span-23 Magin-1 push-2">
                    <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
                        <asp:LinkButton ID="ChangePasswordPushButton" runat="server" CssClass="ImageButton"
                            CommandName="ChangePassword">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                            <span>更改密码</span></asp:LinkButton>
                        <asp:LinkButton ID="CancelPushButton" runat="server" CssClass="ImageButton" CommandName="Cancel"
                            CausesValidation="False">
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/cancel.ico" ImageAlign="AbsMiddle" />
                            <span>取消</span></asp:LinkButton>
                    </div>
                </div>
            </div>
        </ChangePasswordTemplate>
        <SuccessTemplate>
            <div class="span-23 Query groupingborder">
                <div class="span-23 prepend-1 Magin-1">
                    <asp:Label ID="label" runat="server" CssClass="span-9">修改密码成功！下次登录须输入新密码！</asp:Label>
                </div>
            </div>
        </SuccessTemplate>
    </asp:ChangePassword>
</asp:Content>
