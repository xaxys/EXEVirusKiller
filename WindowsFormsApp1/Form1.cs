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
            notifyIcon1.Icon = Icon;
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
                label2.Invoke(d, new object[] { f });
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

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == USBDevice.WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case USBDevice.WM_DEVICECHANGE:
                            break;
                        case USBDevice.DBT_DEVICEARRIVAL://U盘插入
                            if (自动扫描U盘ToolStripMenuItem.Checked)
                                USBDevice.CheckDevice();
                            break;
                        case USBDevice.DBT_CONFIGCHANGECANCELED:
                            break;
                        case USBDevice.DBT_CONFIGCHANGED:
                            break;
                        case USBDevice.DBT_CUSTOMEVENT:
                            break;
                        case USBDevice.DBT_DEVICEQUERYREMOVE:
                            break;
                        case USBDevice.DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case USBDevice.DBT_DEVICEREMOVECOMPLETE: //U盘卸载
                            USBDevice.RemoveDevice();
                            break;
                        case USBDevice.DBT_DEVICEREMOVEPENDING:
                            break;
                        case USBDevice.DBT_DEVICETYPESPECIFIC:
                            break;
                        case USBDevice.DBT_DEVNODES_CHANGED:
                            break;
                        case USBDevice.DBT_QUERYCHANGECONFIG:
                            break;
                        case USBDevice.DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            {
                e.Cancel = true;
                ShowInTaskbar = false;
                Hide();
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show();
            }

            if (e.Button == MouseButtons.Left)
            {
                ShowInTaskbar = true;
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void AddMenuItem(ToolStripMenuItem item)
        {
            if (contextMenuStrip1.Items.Count <= 3)
                aToolStripMenuItem.Visible = true;
            contextMenuStrip1.Items.Insert(0, item);
        }

        public void RemoveMenuItem(ToolStripMenuItem item)
        {
            contextMenuStrip1.Items.Remove(item);
            if (contextMenuStrip1.Items.Count <= 3)
                aToolStripMenuItem.Visible = false;
        }

        private void 自动扫描U盘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (自动扫描U盘ToolStripMenuItem.Checked)
                自动扫描U盘ToolStripMenuItem.Checked = false;
            else 自动扫描U盘ToolStripMenuItem.Checked = true;
        }

        private void checkBox3_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;//提示信息的可见时间
            toolTip1.InitialDelay = 500;//事件触发多久后出现提示
            toolTip1.ReshowDelay = 500;//指针从一个控件移向另一个控件时，经过多久才会显示下一个提示框
            toolTip1.ShowAlways = true;//是否显示提示框
            toolTip1.SetToolTip(checkBox3, "增大变种病毒能力，但小概率会误伤正常文件");
        }
    }
}
