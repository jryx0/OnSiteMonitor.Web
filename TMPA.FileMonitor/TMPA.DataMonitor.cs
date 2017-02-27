using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMPA.FileMonitor
{
    public partial class Form1 : Form
    {

        bool CanExit = false;

        public Form1()
        {
            InitializeComponent();
        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Importor imp = new Importor(backgroundWorker1);
            imp.DataImportor( ); 
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //AppendLog("---->" + e.ProgressPercentage.ToString() + " %\r\n");
            if(e.ProgressPercentage == -1)
            {
                AppendLog(e.UserState.ToString() + "\r\n");
            }
            else 
                this.progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AppendLog("---------DoWork Complete-----------\r\n");


            btnStop.Enabled = true;
            this.progressBar1.Value = 100;


            if (CanExit)
            {
                btnStart.Enabled = true;
                btnExit.Enabled = true;
                btnStop.Enabled = false;
                AppendLog("---------终止定时器---------\r\n");
            }
            else
            {
                this.FileFinder.Start();

                AppendLog("---------定时器启动-----------\r\n");
                this.FileFinder.Interval = Properties.Settings.Default.TimerInterval * 60 * 100;
                AppendLog("-间隔时间:" + this.FileFinder.Interval/1000 + "秒\r\n");
            }
        }

        private void FileFinder_Tick(object sender, EventArgs e)
        {
            this.FileFinder.Stop();          
             
            if (CanExit)
            {
                AppendLog("---------终止定时器---------\r\n");
                btnStart.Enabled = true;
                btnExit.Enabled = true;
                btnStop.Enabled = false;                
            }
            else
            {
                AppendLog("---------DoWork启动-----------\r\n");
                this.progressBar1.Value = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }


        #region Log
        delegate void delOneStr(string log);
        void AppendLog(string _log)
        {
            if (tbLogWin.InvokeRequired)
            {
                delOneStr dd = new delOneStr(AppendLog);
                tbLogWin.Invoke(dd, new object[] { _log });
            }
            else
            {
                _log = System.DateTime.Now.ToString("[yyyy-HH-dd HH:mm:ss fff]") + _log;

                StringBuilder builder;
                if (tbLogWin.Lines.Length > 1999)
                {
                    builder = new StringBuilder(tbLogWin.Text);
                    builder.Remove(0, tbLogWin.Text.IndexOf('\r', 3000) + 2);

                    builder.Append(_log);
                    tbLogWin.Clear();
                    tbLogWin.AppendText(builder.ToString());
                }
                else
                {
                    tbLogWin.AppendText(_log);
                }
            }
        }
        #endregion

        private void btnStop_Click(object sender, EventArgs e)
        {
            AppendLog("---------定时器停止-----------\r\n");

            if (!backgroundWorker1.IsBusy)
            {
                btnExit.Enabled = true;
                btnStop.Enabled = false;
                this.FileFinder.Stop();
                btnStart.Enabled = true;                
            }
            else
            {
                CanExit = true;
                btnStop.Enabled = false;
                AppendLog("---------正在退出, 等待线程结束-----------\r\n");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            AppendLog("---------定时器启动-----------\r\n");
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnExit.Enabled = false;

            CanExit = false;
            this.FileFinder.Interval = Properties.Settings.Default.TimerInterval * 10;
            AppendLog("---------间隔时间:" + this.FileFinder.Interval/1000 + "秒\r\n");
            this.FileFinder.Start();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                MessageBox.Show("请先停止监控！");
            else
                Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                MessageBox.Show("请先停止监控！");
                e.Cancel = true;
            }            
        }
    }
}
