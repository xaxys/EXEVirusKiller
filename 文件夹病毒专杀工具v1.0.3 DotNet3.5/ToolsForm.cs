using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 文件夹病毒专杀工具
{
    public partial class ToolsForm : Form
    {
        public ToolsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("设置开机自启动，需要修改注册表！程序移动位置后请重新添加自启！", "提示");
            Logger.Info(Util.MainThread, "添加开机自启动");
            string path = Application.ExecutablePath;
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.SetValue("FolderEXEVirusKiller", "\"" + path + "\"");
            rk2.Close();
            rk.Close();
            MessageBox.Show("添加开机自启动完成", "提示");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
            Logger.Info(Util.MainThread, "取消开机自启动");
            string path = Application.ExecutablePath;
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.DeleteValue("FolderEXEVirusKiller", false);
            rk2.Close();
            rk.Close();
            MessageBox.Show("取消开机自启动完成", "提示");
        }

        private void ToolsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.Info(Util.MainThread, "工具窗口关闭");
        }

        private void ToolsForm_Load(object sender, EventArgs e)
        {
            Logger.Info(Util.MainThread, "工具窗口打开");
        }
    }
}
