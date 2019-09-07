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
using System.Runtime.InteropServices;

namespace 文件夹病毒专杀工具
{
    public partial class MainForm : Form
    {
        Killer MainKiller = null;
        FolderBrowserDialog fbd;
        public MainForm()
        {
            InitializeComponent();
            fbd = new FolderBrowserDialog();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            {
                if (MainKiller != null && MainKiller.IsRun)
                {
                    MessageBox.Show("请先停止查杀！");
                    e.Cancel = true;
                }
                else
                {
                    Logger.Info(Util.MainThread, "主窗体关闭");
                }
            }
            else
            {
                Logger.Info(Util.MainThread, e.CloseReason.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MainKiller != null && MainKiller.IsRun)
            {
                MainKiller.Abort = true;
                Logger.Info(Util.MainThread, "终止主查杀线程");
            }
            else {
                button2.Text = "停止扫描";
                button2.BackColor = Util.huang;
                listBox1.Items.Clear();
                ShowListBox();
                SwitchLabel(true);

                MainKiller = new Killer(Util.CheckThread)
                {
                    RootDir = textBox1.Text,
                    SetLabelMethod = (string s) => label3.Text = s,
                    SetProcessBarMethod = (int v) => progressBar1.Value = v,
                    AddListMethod = AddList,
                    FinishCheckMethod = FinishCheck
                };
                MainKiller.SetOption(checkBox1.Checked, checkBox2.Checked, checkBox3.Checked);
                MainKiller.Run();
            } 
        }

        private void checkBox3_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;//提示信息的可见时间
            toolTip1.InitialDelay = 500;//事件触发多久后出现提示
            toolTip1.ReshowDelay = 500;//指针从一个控件移向另一个控件时，经过多久才会显示下一个提示框
            toolTip1.ShowAlways = true;//是否显示提示框
            toolTip1.SetToolTip(checkBox3, "增大识别变种病毒能力，但小概率会误伤正常文件");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fbd.ShowDialog();
            textBox1.Text = fbd.SelectedPath;
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
            if (MainKiller == null || !MainKiller.IsRun)
            {
                if (Directory.Exists(textBox1.Text))
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

        private void FinishCheck()
        {
            button2.Text = "开始查杀";
            SwitchLabel(false);
            CheckPath();
        }

        public void AddList(object obj)
        {
            if (obj is string)
                listBox1.Items.Add((string)obj);
            else if (obj is Exception)
            {
                listBox1.Items.Add("[!] " + ((Exception)obj).Message);
                Logger.Warn(Util.CheckThread, (Exception)obj);
            }

        }

        public void SwitchLabel(bool f)
        {
            label2.Visible = f;
            label3.Visible = f;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.Info(Util.MainThread, "主窗体打开");
        }
    }
}