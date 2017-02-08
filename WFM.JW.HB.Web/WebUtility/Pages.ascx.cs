using System;
using System.Web.UI;

namespace WFM.JW.HB.Web
{
    public delegate void GoPageEventHandler(int page);
    public partial class Pager : System.Web.UI.UserControl
    {
        PageSupport ps;
        bool ifnq;

        public event GoPageEventHandler GoPage;
        protected void Page_Load(object sender, EventArgs e)
        {
            ifnq = false;
            ps = (PageSupport)ViewState["PageSupport"];
            
            if (ps == null)
            {
                this.ImageButton1.Enabled = false;
                this.ImageButton2.Enabled = false;
                this.ImageButton3.Enabled = false;
                this.ImageButton4.Enabled = false;
                this.ImageButton5.Enabled = false;
                this.txtPage.Disabled = true;
                this.totalnum.Text = "0";
            }
            else
            {
                this.ImageButton1.Enabled = true;
                this.ImageButton2.Enabled = true;
                this.ImageButton3.Enabled = true;
                this.ImageButton4.Enabled = true;
                this.ImageButton5.Enabled = true;
                this.txtPage.Disabled = false;
                this.totalnum.Text =  ps.TotalCount.ToString();
                this.total.Text = ps.TotalPagesCount.ToString();
                this.pg.Text = ps.CurrentPageNo.ToString();
            }
        }
        public PageSupport GetPageSupport()
        {
            return (PageSupport)ViewState["PageSupport"];
        }
        public void SetPageSupport(PageSupport pages)
        {
            ViewState["PageSupport"] = pages;
            ps = pages;
        }



        protected void txtPage_ServerChange(object sender, EventArgs e)
        {

        }

        public void SetPage(PageSupport page)
        {
            ps = page;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            int pageNO = int.Parse(string.IsNullOrEmpty(this.txtPage.Value) ? "0" : this.txtPage.Value);
            this.GoPage(pageNO);//, ifnq);
        }

        public void PageReload()
        {
            if (ps != null)
            {
                this.pg.Text = ps.CurrentPageNo.ToString();
                this.total.Text = ps.TotalPagesCount.ToString();
                this.txtPage.Value = ps.CurrentPageNo.ToString();
                this.ImageButton1.Enabled = true;
                this.ImageButton2.Enabled = true;
                this.ImageButton3.Enabled = true;
                this.ImageButton4.Enabled = true;
                this.ImageButton5.Enabled = true;
                this.txtPage.Disabled = false;
                
                this.totalnum.Text = ps.TotalCount.ToString();
                this.total.Text = ps.TotalPagesCount.ToString();
                this.pg.Text = ps.CurrentPageNo.ToString();
            }
            else
            {
                if (ps == null)
                {
                    this.ImageButton1.Enabled = false;
                    this.ImageButton2.Enabled = false;
                    this.ImageButton3.Enabled = false;
                    this.ImageButton4.Enabled = false;
                    this.ImageButton5.Enabled = false;
                    this.txtPage.Disabled = true;
                    this.totalnum.Text = "0";
                }
            }

        }
        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            if (ps != null)
            {
                this.GoPage(1);//, ifnq);
            }
        }
        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            if (ps != null)
            {
                if (!(ps.CurrentPageNo <= 0 || ps.CurrentPageNo.Equals("")))
                {
                    if (ps.CurrentPageNo == 1)
                        this.GoPage(1);
                    else
                    this.GoPage(ps.CurrentPageNo - 1);//, ifnq);
                }
            }
        }
        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            if (ps != null)
            {
                if (!(ps.CurrentPageNo <= 0 || ps.CurrentPageNo.Equals("")))
                {
                    this.GoPage(ps.CurrentPageNo + 1);//, ifnq);
                }
            }
        }
        protected void ImageButton5_Click(object sender, ImageClickEventArgs e)
        {
            if (ps != null)
            {
                if (!(ps.TotalPagesCount <= 0 || ps.TotalPagesCount.Equals("")))
                {
                    this.GoPage(ps.TotalPagesCount);//, ifnq);
                }
            }
        }
    }
}