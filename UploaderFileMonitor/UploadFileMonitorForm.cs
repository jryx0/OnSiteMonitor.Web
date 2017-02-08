using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace UploaderFileMonitor
{
    public partial class UploadFileMonitorForm : Form
    {
        newasynchui _Task;

        bool CanExit = false;
        public UploadFileMonitorForm()
        {
            InitializeComponent();
            button2.Enabled = false;


            //创建任务管理对象
            _Task = new newasynchui();
            //挂接事件处理方法
            _Task.TaskStatusChanged += new TaskEventHandler(OnTaskStatusChanged);
            _Task.TaskProgressChanged += new TaskEventHandler(OnTaskProgressChanged1);
            _Task.TaskProgressChanged += new TaskEventHandler(OnTaskProgressChanged2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AppendLog("---------监控程序启动-----------\r\n");
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
            CanExit = false;
            this.FileFinder.Interval = Properties.Settings.Default.TimerInterval * 100; //起始间隔毫秒数          
            this.FileFinder.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            AppendLog("---------监控程序停止-----------\r\n");

            if (_Task.TaskState != TaskStatus.Running)
            {
                button3.Enabled = true;
                button2.Enabled = false;
                this.FileFinder.Stop();
                button1.Enabled = true;
            }
            else
            {
                CanExit = true;
                button2.Enabled = false;
                AppendLog("---------正在退出, 等待线程结束-----------\r\n");                
            }
        }

        private void FileFinder_Tick(object sender, EventArgs e)
        {
            this.FileFinder.Stop();
            ThreadStart ts = new ThreadStart(start);
            Thread thread = new Thread(ts);

            AppendLog("---------线程启动-----------\r\n");
            this.FileFinder.Interval = Properties.Settings.Default.TimerInterval * 60 * 1000;
            AppendLog("-间隔时间:" + Properties.Settings.Default.TimerInterval.ToString() + "分钟\r\n");
            if (CanExit)
            {
                button1.Enabled = false;
                button3.Enabled = false;
                button2.Enabled = true;
            }
            else
            {
                thread.Start();
            }
        }

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
                   
                    builder.Append(_log );
                    tbLogWin.Clear();
                    tbLogWin.AppendText(builder.ToString());
                }
                else
                {
                    tbLogWin.AppendText(_log);
                }
            }
        }
        #region 线程

        void start()
        {
            _Task.errorkey = 1;
            _Task.StartTask(new TaskDelegate(_Task.Work2), new object[] { });
        }

        //在UI线程,负责更新完成率
        private void OnTaskProgressChanged2(object sender, TaskEventArgs e)
        {
            //if (e.Result != null)
            //    textBox1.Text += e.Result.ToString() + "\r\n";           
        }   

        private void label_status_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(_Task.TaskState.ToString()+",CallThread:["+_Task.CallThread.GetHashCode().ToString()+"],WorkThread:["+_Task.WordThread.GetHashCode().ToString()+"]");
        }

        //在UI线程,负责更新进度条
        private void OnTaskProgressChanged1(object sender, TaskEventArgs e)
        {
            if (InvokeRequired)     //不在UI线程上,异步调用
            {
                TaskEventHandler TPChanged1 = new TaskEventHandler(OnTaskProgressChanged1);
                this.BeginInvoke(TPChanged1, new object[] { sender, e });
                Console.WriteLine("InvokeRequired=true");
            }
            else
            {               
                progressBar1.Value = e.Progress;

                if (e.Result != null)
                    lbInfo.Text = e.Result.ToString();
            }
        }

        //在UI线程,负责更新状态信息和按钮状态
        private void OnTaskStatusChanged(object sender, TaskEventArgs e)
        {
            string msg = string.Empty;
            switch (e.Status)
            {
                case TaskStatus.Running:
                    if (e.Result != null)
                        AppendLog(e.Result.ToString() + "\r\n");
                    break;
                case TaskStatus.Stopped:
                    progressBar1.Value = 100;
                    if (e.Result != null)
                        AppendLog(e.Result.ToString() + "\r\n");

                    if (CanExit)
                    {
                        button1.Enabled = true;
                        button3.Enabled = true;
                    }
                    else this.FileFinder.Start();
                    AppendLog("--------------线程结束-------------------\r\n\r\n");
                    break;
                case TaskStatus.CancelPending:
                    AppendLog("-----------------线程准备退出------------------\r\n");
                    break;
                case TaskStatus.AbortPending:
                case TaskStatus.ThrowErrorStoped:
                case TaskStatus.Aborted:
                    if (CanExit)
                    {
                        button1.Enabled = true;
                        button3.Enabled = true;
                    }
                    else this.FileFinder.Start();
                    AppendLog("**************线程异常******************\r\n");
                    break;
            }
           
        }
        #endregion
    }
}
