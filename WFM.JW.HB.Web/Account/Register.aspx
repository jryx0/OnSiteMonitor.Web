<%@ Page Title="注册" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="WFM.JW.HB.Web.Account.Register" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:CreateUserWizard ID="RegisterUser" runat="server" EnableViewState="false" OnCreatedUser="RegisterUser_CreatedUser" OnContinueButtonClick="RegisterUser_Continue">
        <LayoutTemplate>
            <asp:PlaceHolder ID="wizardStepPlaceholder" runat="server"></asp:PlaceHolder>
            <asp:PlaceHolder ID="navigationPlaceholder" runat="server"></asp:PlaceHolder>
        </LayoutTemplate>
        <WizardSteps>
            <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server" >
                <ContentTemplate>
                    <h2>
                        创建新帐户
                    </h2>
                    <p style=" font-size:1.2em">
                        使用以下表单创建新帐户。
                    </p>
                    <p style=" font-size:1.2em">
                        密码的长度至少必须为 <%= Membership.MinRequiredPasswordLength %> 个字符。
                    </p>
                    <span class="failureNotification">
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    </span>
                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification" 
                         ValidationGroup="RegisterUserValidationGroup"/>
                    <div class="accountInfo Query"  style=" font-size:1.2em">
                        <fieldset class="register">
                            <legend>帐户信息</legend>
                            <p class="span-23 prepend-1 Magin-1">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" CssClass="span-3">用户名:</asp:Label>
                                <asp:TextBox ID="UserName" runat="server" CssClass="textEntry title span-5"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                                     CssClass="failureNotification" ErrorMessage="必须填写“用户名”。" ToolTip="必须填写“用户名”。" 
                                     ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            </p>
                            <p class="span-23 prepend-1 Magin-1">
                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" CssClass="span-3">电子邮件:</asp:Label>
                                <asp:TextBox ID="Email" runat="server" CssClass="textEntry title span-5"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" 
                                     CssClass="failureNotification" ErrorMessage="必须填写“电子邮件”。" ToolTip="必须填写“电子邮件”。" 
                                     ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            </p>
                            <p class="span-23 prepend-1 Magin-1">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="span-3">密码:</asp:Label>
                                <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry title span-5" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                                     CssClass="failureNotification" ErrorMessage="必须填写“密码”。" ToolTip="必须填写“密码”。" 
                                     ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                            </p>
                            <p class="span-23 prepend-1 Magin-1">
                                <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword" CssClass="span-3">确认密码:</asp:Label>
                                <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry title span-5" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" CssClass="failureNotification" Display="Dynamic" 
                                     ErrorMessage="必须填写“确认密码”。" ID="ConfirmPasswordRequired" runat="server" 
                                     ToolTip="必须填写“确认密码”。" ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                                     CssClass="failureNotification" Display="Dynamic" ErrorMessage="“密码”和“确认密码”必须匹配。"
                                     ValidationGroup="RegisterUserValidationGroup">*</asp:CompareValidator>
                            </p>
                        </fieldset>
                        <p class="span-23 prepend-1 Magin-1 submitButton">
                            <asp:Button ID="CreateUserButton" runat="server" CommandName="MoveNext" Text="创建用户" 
                                 ValidationGroup="RegisterUserValidationGroup"/>
                        </p>
                    </div>
                </ContentTemplate>
                <CustomNavigationTemplate>
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</asp:Content>

