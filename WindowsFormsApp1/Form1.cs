using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static Form1 THIS = null;
        public Form1()
        {
            InitializeComponent();
            THIS = this;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Util.isRun)
            {
                Util.SetState(checkBox1.Checked, checkBox2.Checked, checkBox3.Checked);
                button2.Text = "停止扫描";
                button2.BackColor = Util.huang;
                ShowListBox();
                THIS.SwitchLabel(true);

                listBox1.Items.Clear();
                listBox1.Items.Add("开始查杀目录" + textBox1.Text);

                Thread thread = new Thread(new ThreadStart(StartSearch));
                thread.Name = "查杀";
                thread.IsBackground = true;
                thread.Start();
            }
            else Util.isRun = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            SwitchListBox();
        }

        private void SwitchListBox()
        {
            if (label4.Text == Util.zuo) ShowListBox();
            else HideListBox();
        }

        private void ShowListBox()
        {
            if (label4.Text == Util.zuo)
            {
                label4.Text = Util.xia;
                panel1.Show();
                Height += Util.ChangeHeight;
            }
        }

        private void HideListBox()
        {
            if (label4.Text == Util.xia)
            {
                label4.Text = Util.zuo;
                panel1.Hide();
                Height -= Util.ChangeHeight;
            }
        }

        private void CheckPath()
        {
            if (!Util.isRun)
            {
                if (Directory.Exists(textBox1.Text))//判断是否存在
                {
                    button2.Enabled = true;
                    button2.BackColor = Util.lv;
                }
                else
                {
                    button2.Enabled = false;
                    button2.BackColor = Util.hong;
                }
            }
        }

        delegate void FinishCheckCallback();
        private void FinishCheck()
        {
            if (button2.InvokeRequired)
            {
                while (!button2.IsHandleCreated)
                    if (button2.Disposing || button2.IsDisposed) return;
                FinishCheckCallback d = new FinishCheckCallback(FinishCheck);
                label3.Invoke(d);
            }
            else
            {
                button2.Text = "开始查杀";
                CheckPath();
            }
        }

        delegate void SetLabel3Callback(string s);
        public void SetLabel3(string s)
        {
            if (label3.InvokeRequired)
            {
                while (!label3.IsHandleCreated)
                    if (label3.Disposing || label3.IsDisposed) return;
                SetLabel3Callback d = new SetLabel3Callback(SetLabel3);
                label3.Invoke(d, new object[] { s });
            }
            else label3.Text = s;
        }

        delegate void AddListCallback(object obj);
        public void AddList(object obj)
        {
            if (listBox1.InvokeRequired)
            {
                while (!listBox1.IsHandleCreated)
                    if (listBox1.Disposing || listBox1.IsDisposed) return;
                AddListCallback d = new AddListCallback(AddList);
                listBox1.Invoke(d, obj);
            }
            else
            {
                if (obj is string)
                    listBox1.Items.Add((string)obj);
                else if (obj is Exception)
                    listBox1.Items.Add("[!] " + ((Exception)obj).Message);
            }
        }

        delegate void ProgressBarCallback(int k);
        public void SetProgressBar(int k)
        {
            if (progressBar1.InvokeRequired)
            {
                while (!progressBar1.IsHandleCreated)
                    if (progressBar1.Disposing || progressBar1.IsDisposed) return;
                ProgressBarCallback d = new ProgressBarCallback(SetProgressBar);
                progressBar1.Invoke(d, new object[] { k });
            }
            else progressBar1.Value = k;
        }

        delegate void SwitchLabelCallback(bool f);
        public void SwitchLabel(bool f)
        {
            if (label2.InvokeRequired && label3.InvokeRequired)
            {
                while (!label2.IsHandleCreated || !label3.IsHandleCreated)
                {
                    if (label2.Disposing || label2.IsDisposed) return;
                    if (label3.Disposing || label3.IsDisposed) return;
                }
                SwitchLabelCallback d = new SwitchLabelCallback(SwitchLabel);
                progressBar1.Invoke(d, new object[] { f });
            }
            else
            {
                label2.Visible = f;
                label3.Visible = f;
            }
        }

        private void StartSearch()
        {
            Util.isRun = true;
            Util.SearchDir(textBox1.Text, 100);
            AddList("查杀完成！搞定了" + Util.VirusNum + "个病毒");
            THIS.SwitchLabel(false);
            Util.isRun = false;
            FinishCheck();
            SetProgressBar(0);
        }
    }
}
