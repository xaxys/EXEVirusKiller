using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        delegate string SelectedCallback();
        public string Selected{
            get {
                if (InvokeRequired)
                {
                    SelectedCallback d = new SelectedCallback(() => { return (string)listBox1.SelectedItem; });
                    return (string)Invoke(d);
                }
                else return (string)listBox1.SelectedItem;
            }
        }

        public Form2()
        {
            InitializeComponent();
            InitList();
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            textBox1.Height = listBox1.Height;
        }

        delegate void InitListCallback();
        public void InitList()
        {
            if (InvokeRequired)
            {
                InitListCallback d = new InitListCallback(InitList);
                Invoke(d);
            }
            else
            {
                listBox1.Items.Clear();
                foreach (var item in Logger.list)
                {
                    listBox1.Items.Add(item.Key);
                }
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
            RefreshForm2();
        }

        private void RefreshTextBox()
        {
            if (listBox1.SelectedIndex != -1)
            {
                textBox1.Clear();
                textBox1.Text += Logger.list[(string)listBox1.SelectedItem];
            }
        }

        public void RefreshForm2()
        {
            object obj = listBox1.SelectedItem;
            InitList();
            listBox1.SelectedItem = obj;
            RefreshTextBox();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.Info(Util.MainThread, "日志信息窗体关闭");
            Form1.form2 = null;
        }

        delegate void AddTextCallback(string s);
        public void AddText(string s)
        {
            if (InvokeRequired)
            {
                AddTextCallback d = new AddTextCallback(AddText);
                Invoke(d, new object[] { s });
            }
            else textBox1.Text += s + Environment.NewLine;
        }
    }
}
