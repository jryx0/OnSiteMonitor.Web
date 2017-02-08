<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ClueMgr.aspx.cs" Inherits="WFM.JW.HB.Web.WFMClues.ClueMgr" %>

<%@ Register Assembly="WFM.JW.HB.Web" Namespace="WFM.JW.HB.Web" TagPrefix="cc1" %>
<%@ Register Src="../WebUtility/Pages.ascx" TagName="Pages" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DeletConfirm() {
            return confirm("确认要导出吗?")
        }

        function CheckContry() {
            if ($('select[id$=DropDownListCity]')[0].selectedIndex <= 0) {
                alert("请选择单位！");
                return false;
            } else
                if ($('select[id$=DropDownListContry]')[0].length > 1)
                    if ($('select[id$=DropDownListContry]')[0].selectedIndex <= 0) {
                        alert("请选择辖区单位！");
                        return false;
                    }

            if ($('input[id$=tbID]')[0].value.length != 0)
                if ($('input[id$=tbID]')[0].value.length < 10) {
                    alert("输入的身份证号长度应超过10位号码!");
                    return false;
                }
        }

        function StatusCityControl() {
            if ($('select[id$=DropDownListCity]')[0].selectedIndex > 14) {
                $('select[id$=DropDownListContry]')[0].selectedIndex = 0;
                $('select[id$=DropDownListContry]').attr({ "disabled": "disabled" });
                $('select[id$=DropDownListContry]').attr({ "style": "background-color: darkgray;" });
                $('span[id$=Label3]').innerText = "单位:";
            }
            else {
                $('select[id$=DropDownListContry]').removeAttr("disabled");
                $('select[id$=DropDownListContry]').removeAttr("style");
                $('span[id$=Label3]').innerText = "上级单位:";
            }
            return true;
        }

        function StatusControl() {
            if ($('select[id$=DropDownList1]')[0].selectedIndex == 1) {
                $('select[id$=DropDownList2]')[0].selectedIndex = 0;
                $('select[id$=DropDownList2]').attr({ "disabled": "disabled" });
                $('select[id$=DropDownList2]').attr({ "style": "background-color: darkgray;" });
            }
            else {
                $('select[id$=DropDownList2]').removeAttr("disabled");
                $('select[id$=DropDownList2]').removeAttr("style");
            }

            return false;
        }

        function RefreshContext() {
            $('select[id$=DropDownListCity]')[0].selectedIndex = 0;
            $('select[id$=DropDownListContry]')[0].selectedIndex = 0;
            $('select[id$=DropDownList1]')[0].selectedIndex = 0;
            $('select[id$=DropDownList2]')[0].selectedIndex = 0;
            $('select[id$=DropDownListItem]')[0].selectedIndex = 0;
            $('input[id$=tbID]').attr('value', '');



            return false;
        }

        $(document).ready(function () {
            var message = $('input[id$=ShowDialog]').val();
            if (message.length > 1) {
                alert(message);
                $('input[id$=ShowDialog]').val("");
            }

            StatusControl();


        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="server">
    </asp:ScriptManager>
    <div class="span-24 Query  groupingborder">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="span-24 push-1">
                    <div class="span-7 ">
                        <asp:Label ID="Label3" runat="server" Text="单位:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListCity" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CityList_IndexChanged" onchange="StatusCityControl()">
                        </asp:DropDownList>
                    </div>
                    <div class="span-7 ">
                        <asp:Label ID="Label1" runat="server" Text="辖区单位:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListContry" runat="server">
                        </asp:DropDownList>
                    </div>
                    <div class="span-7 ">
                        <asp:Label ID="Label12" runat="server" Text="项目名称:" CssClass="span-2"></asp:Label>
                        <asp:DropDownList ID="DropDownListItem" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="span-24 push-1">

            <div class="span-7 ">
                <asp:Label ID="Label4" runat="server" Text="身份证号:" CssClass="span-2"></asp:Label>
                <asp:TextBox ID="tbID" runat="server" CssClass="title span-5" Font-Names="Lucida Console"></asp:TextBox>
            </div>

            <div class="span-7 ">
                <asp:Label ID="Label5" runat="server" Text="线索状态:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownList1" runat="server" onchange="return StatusControl()">
                    <asp:ListItem Value="-1">全部</asp:ListItem>
                    <asp:ListItem Value="0">未核查</asp:ListItem>
                    <asp:ListItem Value="1">已核查</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="span-7 ">
                <asp:Label ID="Label6" runat="server" Text="查实情况:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownList2" runat="server">
                    <asp:ListItem Value="-1">全部</asp:ListItem>
                    <asp:ListItem Value="0">查否</asp:ListItem>
                    <asp:ListItem Value="1">查实</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="span-24 push-1">
            <div class="span-7 ">
                <asp:Label ID="Label7" runat="server" Text="线索类型:" CssClass="span-2"></asp:Label>
                <asp:DropDownList ID="DropDownList3" runat="server">
                    <asp:ListItem Value="-1">全部</asp:ListItem>
                    <asp:ListItem Value="0" Selected="True">问题线索</asp:ListItem>
                    <asp:ListItem Value="1">录入错误</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="span-13 push-1" style="text-align: right;">
                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="ImageButton" OnClientClick="return RefreshContext()">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Styles/Img/refresh.png" ImageAlign="AbsMiddle" />
                    <span>清&nbsp;&nbsp;空</span>
                </asp:LinkButton>
                <asp:LinkButton ID="LinkButton3" runat="server" CssClass="ImageButton" OnClick="LinkButtonQuery_Click" OnClientClick="return CheckContry()">
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Styles/Img/find.ico" ImageAlign="AbsMiddle" />
                    <span>查&nbsp;&nbsp;询</span>
                </asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="span-24 DataArea">
        <div class="span-6" style="margin-bottom: 4px; margin-left: 2px;">

            <%-- <asp:LinkButton ID="LinkButtonNew" runat="server" CssClass="ImageButton" OnClick="LinkButtonNew_Click">
                <asp:Image ID="ImageNew" runat="server" ImageUrl="~/Styles/Img/new.png" ImageAlign="AbsMiddle" />
                <span>新&nbsp;增</span>
            </asp:LinkButton>--%>
            <asp:LinkButton ID="LinkButtonModify" runat="server" CssClass="ImageButton hidden"
                OnClick="LinkButtonModify_Click">
                <asp:Image ID="ImageModify" runat="server" ImageUrl="~/Styles/Img/modify.ico" ImageAlign="AbsMiddle" />
                <span>详&nbsp;细</span>
            </asp:LinkButton>
            <asp:LinkButton ID="LinkButtonDelete" runat="server" CssClass="ImageButton hidden"
                OnClientClick="return DeletConfirm()" OnClick="LinkButtonImport_Click">
                <asp:Image ID="ImageDelete" runat="server" ImageUrl="~/Styles/Img/export.ico" ImageAlign="AbsMiddle" />
                <span>导&nbsp;出</span>
            </asp:LinkButton>
        </div>
        <div class="span-17 pager" style="text-align: right;">
            <uc1:Pages ID="Pages1" runat="server" />
        </div>
    </div>
    <div id="ScrollList" style="width: 990px; overflow: auto; margin-left: -5px;">
        <cc1:CLGridViewEx ID="CLGridViewClueMgrInfo" runat="server" AutoGenerateColumns="False"
            CssClass="span-24 DataGridViewStyle" AllowDBClick="True" DataKeyNames="CluesGuid"
            OnRowCommand="CLGridViewRegionUserInfo_RowCommand">
            <AlternatingRowStyle BackColor="#EEEEEE" />
            <SelectedRowStyle CssClass="tr-selected" />
            <HeaderStyle CssClass="tr-header" HorizontalAlign="Center" />
            <Columns>
                <asp:BoundField DataField="CluesGuid" HeaderText="CluesGuid" SortExpression="CluesGuid" Visible="false" />
                <asp:TemplateField HeaderText="序号" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# this.CLGridViewClueMgrInfo.PageIndex * this.CLGridViewClueMgrInfo.PageSize + CLGridViewClueMgrInfo.Rows.Count + 1%>
                    </ItemTemplate>
                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="单位">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("RegionName") %>'
                            ToolTip='<%# Eval("RegionName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="乡镇街道">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-2' Text='<%# Bind("PersonRegion") %>'
                            ToolTip='<%# Eval("PersonRegion") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="身份证号">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-4' Text='<%# Bind("ID") %>'
                            ToolTip='<%# Eval("ID") %>' Font-Names="Lucida Console" Font-Size="10pt"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="姓名">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("Name") %>'
                            ToolTip='<%# Eval("Name") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="线索状态">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%#
                            Eval("InputError").ToString() == "1" ? "录入错误": (Eval("Confirmed").ToString() == "1" ? "已核查" : "未核查") %>'
                            ToolTip='<%# Eval("Confirmed") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查实情况">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Eval("Confirmed").ToString() == "1" ?  (Eval("ClueTrue").ToString() == "1" ? "查实" : "查否") : "" %>'
                            ToolTip='<%# Eval("Confirmed") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="项目名称">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-1' Text='<%# Bind("Table1") %>'
                            ToolTip='<%# Eval("Table1") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="线索名称">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-5' Text='<%# Bind("ClueType") %>'
                            ToolTip='<%# Eval("ClueType") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="家庭地址">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-4' Text='<%# Bind("Addr") %>'
                            ToolTip='<%# Eval("Addr") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="领取情况">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-5' Text='<%# Bind("DateRange") %>'
                            ToolTip='<%# Eval("DateRange") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="备注">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" CssClass='mlength-6' Text='<%# Bind("Comment") %>'
                            ToolTip='<%# Eval("Comment") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>


            </Columns>
        </cc1:CLGridViewEx>
    </div>
    <asp:HiddenField runat="server" ID="selectedIndex" Value="0" />
    <asp:HiddenField runat="server" ID="ShowDialog" Value="0" />

</asp:Content>
