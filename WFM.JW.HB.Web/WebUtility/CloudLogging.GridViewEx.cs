using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WFM.JW.HB.Web
{
    [DefaultProperty("AutoFillEmptyRow")]
    [ToolboxData("<{0}:CLGridViewEx runat=server></{0}:CLGridViewEx>")]
    public class CLGridViewEx : GridView
    {

        [DefaultValue(true), Category("数据"), Localizable(true), Description("自动填充空行")]
        public bool AutoFillEmptyRow
        {
            set
            {
                this.ViewState["AutoFillEmptyRow"] = value;
            }
            get
            {
                return ((this.ViewState["AutoFillEmptyRow"] == null) || ((bool)this.ViewState["AutoFillEmptyRow"]));
            }
        }

        [DefaultValue(10), Category("数据"), Localizable(true), Description("一页行数")]
        public override int PageSize
        {
            set
            {
                base.PageSize = value;
                //this.ViewState["TotalPageSize"] = value;
            }
            get
            {
                return base.PageSize;//(this.ViewState["TotalPageSize"] == null) ? 10 : (int)this.ViewState["TotalPageSize"];
            }
        }

        [DefaultValue(0), Category("数据"), Localizable(true), Description("空行数")]
        public int EmptyRowNumber
        {
            set
            {
                this.ViewState["EmptyRowNumber"] = value;
            }
            get
            {
                return (this.ViewState["EmptyRowNumber"] == null) ? 0 : ((int)this.ViewState["EmptyRowNumber"]);
            }
        }

        [DefaultValue(false), Category("数据"), Localizable(true), Description("允许双击")]
        public bool AllowDBClick
        {
            set
            {
                this.ViewState["AllowDBClick"] = value;
            }
            get
            {
                return (this.ViewState["AllowDBClick"] == null) ? false : ((bool)this.ViewState["AllowDBClick"]);
            }
        }


        [DefaultValue(false), Category("数据"), Localizable(true), Description("允许单击")]
        public bool AllowClick
        {
            set
            {
                this.ViewState["AllowClick"] = value;
            }
            get
            {
                return (this.ViewState["AllowClick"] == null) ? false : ((bool)this.ViewState["AllowClick"]);
            }
        }


        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow row in this.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    if(AllowDBClick)
                        row.Attributes["ondblclick"] =
                         Page.ClientScript.GetPostBackClientHyperlink(this,
                            "Select$" + row.DataItemIndex, true);
                    if (AllowClick)
                        row.Attributes["onclick"] = @"javascript:selectedRow(" + row.DataItemIndex.ToString() + ")";                   
                }
            }
            base.Render(writer);
        }

       

        protected override void OnPreRender(EventArgs e)
        {

            if (this.AutoFillEmptyRow)
            {
                if (base.PageSize <= 0)
                {
                    base.PageSize = 10;
                }
                if (this.Rows.Count > 0)
                {
                    for (int num = this.Rows.Count; num < this.PageSize; num++)
                    {
                        var row = new GridViewRow(-1, -1, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                        if ((num % 2) == 0)
                        {
                            row.ApplyStyle(base.RowStyle);
                        }
                        else
                        {
                            row.ApplyStyle(base.AlternatingRowStyle);
                        }
                        for (int num2 = 0; num2 < this.Columns.Count; num2++)
                        {
                            var cell = new TableCell
                            {
                                Text = "&nbsp;",
                                Visible = this.Columns[num2].Visible
                            };
                            row.Cells.Add(cell);

                            Style s = new Style();
                            s.CssClass = "emptyrow";
                            row.ApplyStyle(s);

                        }
                        this.Controls[0].Controls.Add(row);
                    }
                }
                else
                {
                    Table table;
                    GridViewRow row2;
                    if (this.Controls.Count == 0)
                    {
                        table = this.CreateChildTable();
                        row2 = new GridViewRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal);
                    }
                    else
                    {
                        table = this.Controls[0] as Table;
                        table.Rows.Clear();

                        row2 = new GridViewRow(-1, -1, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                    }
                     
                    //row2.ApplyStyle(base.HeaderStyle);
                    int num2 = 0;
                    while (num2 < this.Columns.Count)
                    {
                        var cell = new TableCell();
                        cell.ApplyStyle(this.Columns[num2].HeaderStyle);
                        if (cell.HorizontalAlign == HorizontalAlign.NotSet)
                        {
                            cell.HorizontalAlign = HorizontalAlign.Center;
                        }
                        if (cell.VerticalAlign == VerticalAlign.NotSet)
                        {
                            cell.VerticalAlign = VerticalAlign.Middle;
                        }
                        cell.Text = this.Columns[num2].HeaderText;
                        cell.ApplyStyle(this.Columns[num2].HeaderStyle);
                        cell.Visible = this.Columns[num2].Visible;
                        
                        row2.Cells.Add(cell);
                        num2++;
                    }
                    table.Rows.Add(row2);
                    for (int num = 0; num < this.PageSize; num++)
                    {
                        var row = new GridViewRow(-1, -1, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                        if ((num % 2) == 0)
                        {
                            row.ApplyStyle(base.RowStyle);
                        }
                        else
                        {
                            row.ApplyStyle(base.AlternatingRowStyle);
                        }
                        for (num2 = 0; num2 < this.Columns.Count; num2++)
                        {
                            var cell = new TableCell();
                            cell.ApplyStyle(this.Columns[num2].ItemStyle);
                            cell.Visible = this.Columns[num2].Visible;
                            cell.Text = "&nbsp;";
                            row.Cells.Add(cell);
                        }

                        Style s = new Style();
                        s.CssClass = "emptyrow";
                        row.ApplyStyle(s);
                        table.Rows.Add(row);
                    }
                    this.Controls.Add(table);
                }
            }

            base.OnPreRender(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

          
        }
    }
}
