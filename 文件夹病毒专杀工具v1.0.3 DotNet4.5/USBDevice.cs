using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 文件夹病毒专杀工具
{
    public enum Status { Unchecked, Checking, Checked }
    public class USBDevice
    {
        public const int WM_DEVICECHANGE = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
        public const int DBT_CONFIGCHANGED = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
        public const int DBT_CUSTOMEVENT = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;  //请求删除一个设备或媒体片已被取消。
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;  //一个设备特定事件发生。
        public const int DBT_DEVNODES_CHANGED = 0x0007;  //一种设备已被添加到或从系统中删除。
        public const int DBT_QUERYCHANGECONFIG = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
        public const int DBT_USERDEFINED = 0xFFFF;  //此消息的含义是用户定义的
        public const uint GENERIC_READ = 0x80000000;
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_READ = 0x1;
        public const int FILE_SHARE_WRITE = 0x2;
        public const int IOCTL_STORAGE_EJECT_MEDIA = 0x2d4808;

        public static List<USBDevice> DeviceList = new List<USBDevice>();

        public static void CheckDevice()
        {
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo drive in s)
            {
                if (drive.DriveType == DriveType.Removable && !DeviceList.Include(drive))
                {
                    DeviceList.Add(new USBDevice(drive));
                }
            }
        }

        public static void RemoveDevice()
        {
            var DriveList = from drive in DriveInfo.GetDrives()
                            select drive.Name;
            for (int i = DeviceList.Count - 1; i >= 0; i--)
            {
                USBDevice device = DeviceList[i];
                if (!DriveList.Contains(device.Name))
                {
                    if (device.State == Status.Checking)
                    {
                        device.USBKiller.Abort = true;
                    }
                    Util.Icon.RemoveMenuItem(device.MenuItem);
                    DeviceList.Remove(device);
                }
            }
            GC.Collect();
        }

        delegate void SetItemTextCallback(string s);

        public string Name;
        public DirectoryInfo RootDir;
        public ToolStripMenuItem MenuItem;
        Killer USBKiller;
        public string Caption;
        private int VirusNum;
        private Status state;
        private Status State
        {
            get { return state; }
            set
            {
                state = value;
                string info = null;
                switch (value)
                {
                    case Status.Unchecked:
                        info = "单击进行扫描";
                        break;
                    case Status.Checking:
                        info = "正在扫描...单击打开文件浏览器";
                        break;
                    case Status.Checked:
                        info = "在文件浏览器中打开";
                        break;
                }
                if (Util.Icon.InvokeRequired)
                {
                    SetItemTextCallback d = (string s) => MenuItem.ToolTipText = s;
                    Util.Icon.Invoke(d, info);
                }
                else MenuItem.ToolTipText = info;
            }
        }

        USBDevice(DriveInfo drive)
        {
            Name = drive.Name;
            RootDir = drive.RootDirectory;
            Caption = drive.VolumeLabel == "" ? "可移动磁盘" : drive.VolumeLabel;
            Caption += "(" + drive.Name + ")";
            MenuItem = new ToolStripMenuItem(Caption);
            MenuItem.Click += Item_Click;
            Util.Icon.AddMenuItem(MenuItem);
            VirusNum = 0;
            State = Status.Unchecked;
            USBKiller = new Killer(Caption);
            USBKiller.RootDir = RootDir.FullName;
            USBKiller.SetProcessBarMethod = (int v) => MenuItem.Text = Caption + string.Format(" [{0}%]", v);
            USBKiller.FinishCheckMethod = FinishCheck;
            if (Util.Icon.自动扫描U盘) RunSearch();
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                string s = ((ToolStripMenuItem)sender).Text;
                USBDevice device = DeviceList.GetUSBDevice(s);
                if (device != null)
                {
                    if (device.State == Status.Unchecked)
                        device.RunSearch();
                    else
                        System.Diagnostics.Process.Start(device.RootDir.FullName);
                }
            }
        }

        public void RunSearch()
        {
            State = Status.Checking;
            VirusNum = USBKiller.Run();
        }

        private void FinishCheck()
        {
            MenuItem.Text = Caption + " [已扫描]";
            Util.Icon.ShowTips(VirusNum, Name);
            State = Status.Checked;
            USBKiller = null;
            GC.Collect();
        }
    }
}
