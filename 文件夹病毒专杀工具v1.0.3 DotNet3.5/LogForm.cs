using System;
using System.Text;
using System.Windows.Forms;

namespace 文件夹病毒专杀工具
{
    public partial class LogForm : Form
    {

        public LogForm()
        {
            InitializeComponent();
            InitList();
            Logger.AddTextMethod += AddText;
            Logger.InitListMethod += InitList;
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.AddTextMethod -= AddText;
            Logger.InitListMethod -= InitList;
            Logger.Info(Util.MainThread, "日志信息窗体关闭");
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            textBox1.Height = listBox1.Height;
        }

        public void InitList()
        {
            listBox1.Items.Clear();
            foreach (var item in Logger.list)
            {
                listBox1.Items.Add(item.Key);
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.X, e.Y);
            listBox1.SelectedIndex = index;
            RefreshTextBox();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            RefreshContent();
        }

        delegate void Callback();
        private void RefreshTextBox()
        {
            BeginInvoke(new Callback(() =>
            {
                if (listBox1.SelectedIndex != -1)
                {
                    textBox1.Clear();
                    textBox1.Text += Logger.list[(string)listBox1.SelectedItem];
                }
            }));
        }

        public void RefreshContent()
        {
            object obj = listBox1.SelectedItem;
            InitList();
            listBox1.SelectedItem = obj;
            RefreshTextBox();
        }

        public void AddText(string key, StringBuilder sb)
        {
            if ((string)listBox1.SelectedItem == key)
            {
                textBox1.Text += sb;
                textBox1.SelectionStart = textBox1.Text.Length; //设定光标位置
                textBox1.ScrollToCaret(); //滚动到光标处
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Logger.LogPath);
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            Logger.Info(Util.MainThread, "日志信息窗体打开");
        }
    }
}
