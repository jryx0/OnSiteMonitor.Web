<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pages.ascx.cs" Inherits="WFM.JW.HB.Web.Pager" %>
<span style="margin-left: 10px;">总条数：</span><asp:Label ID="totalnum" runat="server"
    Text="0"></asp:Label><span>条</span>
<asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../Styles/Img/pagehome.GIF"
    OnClick="ImageButton2_Click" ImageAlign="AbsMiddle" />
<asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="../Styles/Img/pageup.GIF"
    OnClick="ImageButton3_Click" ImageAlign="AbsMiddle" />
<asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="../Styles/Img/pagedown.GIF"
    OnClick="ImageButton4_Click" ImageAlign="AbsMiddle" />
<asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="../Styles/Img/pagelast.GIF"
    OnClick="ImageButton5_Click" ImageAlign="AbsMiddle" />
<span>第</span>
<asp:Label ID="pg" runat="server" Text="1"></asp:Label>
<span>/</span>
<asp:Label ID="total" runat="server" Text="1"></asp:Label>
<span>页</span> <span>转到</span>
<input type="text" name="txtPage" size="3" style="width: 30px;" id="txtPage" runat="server" />
&nbsp;<asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../Styles/Img/go.GIF"
    OnClick="ImageButton1_Click" ImageAlign="AbsMiddle"/>
<%--<asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtPage"
    ErrorMessage="请输入正确页数！" MaximumValue="999999999" MinimumValue="0" Type="Integer"></asp:RangeValidator>--%>
