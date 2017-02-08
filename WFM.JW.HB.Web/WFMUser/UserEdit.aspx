<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserEdit.aspx.cs" Inherits="WFM.JW.HB.Web.WFMUser.UserEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function checkUserName() {
            var sRet = "";
            var password = $('input[id$=PasswordTextBox]').val();
            var ConfirmPassword = $('input[id$=ConfirmPassword]').val();
            if (password.length < 6)
                sRet = '密码长度大于6！';
            else
                if (password.valueOf() != ConfirmPassword.valueOf()) {
                    sRet = '密码不一致！';
                }


            var reg = /^[a-zA-Z]\w{4,9}$/;
            var username = $('input[id$=UserNameTextBox]').val();
            if (typeof (username) == 'undefined') {
                sRet = '用户名不能为空！';
            }
            else
                if (!reg.test(username.valueOf()))
                    sRet = '用户名以字母开头，长度在5-10位之间，只能包含字母、数字!';
            //                if (username.length < 6 || username.length > 10) {
            //                    sRet = '用户名长度在6-10位!';
            //                }

            var city = $('select[id$=DropDownListCity]');
            if (city[0].selectedIndex == 0) {
                sRet = '选择上级单位';
            }

            var Info = $('span[id$=LabelInfo]');
            if (typeof (sRet) != "undefined")
                if (sRet.length != 0) {
                    alert(sRet);

                    if (typeof (Info) != "undefined") {
                        Info.css('color', 'red');
                        Info[0].innerText = '错误：' + sRet;
                    }
                    return false;
                }

            if (typeof (Info) != "undefined") {
                Info.css('color', 'black');
                Info[0].innerText = '';
            }
            return true;
        }

        $(document).ready(function () {
            var message = $('input[id$=ShowDialog]').val();
            if (message.length > 1) {
                alert(message);
                $('input[id$=ShowDialog]').val("");
            }

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="RegionUserGuid" Value="0" runat="server" />
    <asp:HiddenField ID="Version" Value="0" runat="server" />
    <asp:ScriptManager runat="server">
    </asp:ScriptManager>
    <div class="span-23 Query groupingborder">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="span-23 prepend-1 Magin-1">
                    <asp:Label ID="Label1" runat="server" Text="上级单位:" CssClass="span-3"></asp:Label>
                    <asp:DropDownList ID="DropDownListCity" CssClass="title span-5" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="CityList_IndexChanged">
                    </asp:DropDownList>
                    <asp:Label ID="DropDownListCityComment" runat="server" CssClass="span-7"></asp:Label>
                </div>
                <div class="span-23 prepend-1 Magin-1">
                    <asp:Label ID="Label2" runat="server" Text="单位:" CssClass="span-3"></asp:Label>
                    <asp:DropDownList ID="DropDownListContry" CssClass="title span-5" runat="server"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="UserName" runat="server" Text="用户名:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="UserNameTextBox" runat="server" CssClass="title span-5"></asp:TextBox>
            <asp:Label ID="UserNameComment" runat="server" CssClass="span-7"></asp:Label>
        </div>
        <%-- <div id="oldpass" class="span-23 prepend-1 Magin-1" runat="server">
            <asp:Label ID="Label14" runat="server" Text="旧密码:" CssClass="span-3"></asp:Label>
           <asp:TextBox ID="oldpd" runat="server" CssClass="title span-5" TextMode="Password"></asp:TextBox>
        </div>--%>
        <div class="span-23 prepend-1 Magin-1" runat="server">
            <asp:Label ID="PasswordLabel" runat="server" Text="密码:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="PasswordTextBox" runat="server" CssClass="title span-5" TextMode="Password"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="Label5" runat="server" Text="确认密码:" CssClass="span-3"></asp:Label>
            <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="title span-5" TextMode="Password"></asp:TextBox>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <asp:Label ID="LabelInfo" runat="server" Text=""></asp:Label>
        </div>
        <div class="span-23 prepend-1 Magin-1">
            <div class="span-23 Magin-1 push-2">
                <div class="span-10 " style="margin-top: 5px; margin-bottom: -5px">
                    <asp:LinkButton ID="LinkButtonApply" runat="server" CssClass="ImageButton" OnClientClick="return checkUserName()"
                        OnCommand="LinkButtonApply_Command">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Styles/Img/apply.ico" ImageAlign="AbsMiddle" />
                        <span>保&nbsp;&nbsp;存</span>
                    </asp:LinkButton>
                    <%-- <asp:LinkButton ID="LinkButtonSave" runat="server" CssClass="ImageButton" OnClientClick="return checkUserName()"
                        OnCommand="LinkButtonSave_Command">
                        <asp:Image ID="Image4" runat="server" ImageUrl="~/Styles/Img/save.ico" ImageAlign="AbsMiddle" />
                        <span>保&nbsp;&nbsp;存</span></asp:LinkButton>--%>
                    <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="ImageButton " OnClick="LinkButtonBack_Click">
                        <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/undo.ico" ImageAlign="AbsMiddle" />
                        <span>返&nbsp;&nbsp;回</span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />
</asp:Content>
