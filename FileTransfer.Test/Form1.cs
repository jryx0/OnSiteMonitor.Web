using FileTranser.MTOM.ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileTransfer.Test
{
    public partial class Form1 : Form
    {
        WebServicesHelp wh = new WebServicesHelp();
        delegate string showmessage(string mes);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (wh.Login(tbUsername.Text, tbPassword.Text))
            {
                wh.progressChangeEvent = new ProgressChangedEventHandler(ft_ProgressChanged);
                wh.runWorkerComplateEvent = new RunWorkerCompletedEventHandler(ft_RunWorkerCompleted);

                label3.Text = wh.GetCurrentUserRegion() + "<->" + wh.GetCurrentUserPath();


                wh.IsUpdate("0.9");

            }
            else label2.Text = "登陆错误！";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (wh.Login(tbUsername.Text, tbPassword.Text))
            {
                wh.progressChangeEvent = new ProgressChangedEventHandler(ft_ProgressChanged);
                wh.runWorkerComplateEvent = new RunWorkerCompletedEventHandler(ft_RunWorkerCompleted);

                var token = wh.GetUploadToken(Path.GetFileName(textBox1.Text));
                wh.UploadFile(textBox1.Text, token);

                label7.Text = "OK";
            }
            else label2.Text = "登陆错误！";

        }

        void ft_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EndOperation(e);            
        }
        void ft_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged((sender as FileTransferBase).Guid, e.ProgressPercentage, e.UserState.ToString());          
        }
        delegate void EndOperationDelegate(RunWorkerCompletedEventArgs e);
        public void EndOperation(RunWorkerCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new EndOperationDelegate(this.EndOperation), new object[] { e });
                return;
            }            

            if (e.Error != null)
            {
                this.label2.Text = e.Error.Message;

                wh.FinishUpload(e.Result.ToString(), -1, "", e.Error.Message.Substring(0, 498));
            }
            else if (e.Cancelled)
            {
                this.label2.Text = "取消上传！";                 
            }
            else
            {
                this.label2.Text = "校验成功！上传完成。";
                wh.FinishUpload(e.Result.ToString(),  10, "", "校验成功");                
            }            
        }

        delegate void ProgressChangedDelegate(string Guid, int ProgressPercentage, string Message);
        public void ProgressChanged(string Guid, int ProgressPercentage, string Message)
        {
            if (this.InvokeRequired)
            {
                Invoke(new ProgressChangedDelegate(this.ProgressChanged), new object[] { Guid, ProgressPercentage, Message });
                return;
            }

            if (ProgressPercentage > 0 && ProgressPercentage < progressBar1.Maximum)
            {
                progressBar1.Value = ProgressPercentage;                
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            wh.Logout(tbUsername.Text, tbPassword.Text);

            base.OnClosed(e);
        }
    }
}
