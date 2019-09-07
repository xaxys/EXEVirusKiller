using System;
using System.IO;
using System.Windows.Forms;

namespace 文件夹病毒专杀工具
{
    public partial class Icon : Form
    {
        delegate void Callback();
        private int StaticItemNum;

        public Icon()
        {
            InitializeComponent();
            Show();
            ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            StaticItemNum = contextMenuStrip1.Items.Count;
            BeginInvoke(new Callback(()=>USBDevice.CheckDevice()));
            Logger.Date = DateTime.Now.ToString(Logger.DateFormat);
            if (!Directory.Exists(Logger.LogPath)) Directory.CreateDirectory(Logger.LogPath);
        }
        public bool 自动扫描U盘
        {
            get { return 自动扫描U盘ToolStripMenuItem.Checked; }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show();
            }
            else if (e.Button == MouseButtons.Left)
            {
                MainForm mainForm = App.GetMainForm();
                mainForm.Show();
                mainForm.WindowState = FormWindowState.Normal;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void AddMenuItem(ToolStripMenuItem item)
        {
            contextMenuStrip1.Items.Insert(0, item);
            if (contextMenuStrip1.Items.Count > StaticItemNum)
                aToolStripMenuItem.Visible = true;
        }

        public void RemoveMenuItem(ToolStripMenuItem item)
        {
            contextMenuStrip1.Items.Remove(item);
            if (contextMenuStrip1.Items.Count <= StaticItemNum)
                aToolStripMenuItem.Visible = false;
        }

        private void 自动扫描U盘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (自动扫描U盘ToolStripMenuItem.Checked)
                自动扫描U盘ToolStripMenuItem.Checked = false;
            else 自动扫描U盘ToolStripMenuItem.Checked = true;
        }

        private void 查看日志信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogForm logForm = App.GetLogForm();
            logForm.Show();
            logForm.WindowState = FormWindowState.Normal;
        }

        private void 关于toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = App.GetAboutBox();
            aboutBox.Show();
            aboutBox.WindowState = FormWindowState.Normal;
        }

        public void ShowTips(int virusnum, int fixednum, string s)
        {
            if (virusnum == 0)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, s + "扫描完成",
                    "没有发现病毒",
                    ToolTipIcon.None);
            }
            else
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, s + "扫描完成",
                    "清除了" + virusnum + "个病毒, 修复了" + fixednum + "个文件夹",
                    ToolTipIcon.None);
            }
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
                            Logger.Info(Util.MainThread, "检测到U盘已插入");
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
                            Logger.Info(Util.MainThread, "检测到U盘已卸载");
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
            catch (Exception e)
            {
                Logger.Warn(Util.MainThread, e);
            }
            base.WndProc(ref m);
        }

        private void 工具toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolsForm toolsForm = App.GetToolsForm();
            toolsForm.Show();
            toolsForm.WindowState = FormWindowState.Normal;
        }
    }
}
