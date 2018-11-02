using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class USBDevice
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

        public static List<USBDevice> DriveList = new List<USBDevice>();

        public static bool Include(DriveInfo drive)
        {
            foreach (USBDevice device in DriveList)
            {
                if (device.Name == drive.Name) return true;
            }
            return false;
        }

        public static void CheckDevice()
        {
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo drive in s)
            {
                if (drive.DriveType == DriveType.Removable && !Include(drive))
                {
                    DriveList.Add(new USBDevice(drive));
                }
            }
        }

        public static void RemoveDevice()
        {
            DriveInfo[] d = DriveInfo.GetDrives();
            List<string> s = new List<string>();
            foreach (var dr in d){
                s.Add(dr.Name);
            }
            for (int i = DriveList.Count - 1; i >= 0; i--)
            {
                USBDevice device = DriveList[i];
                if (!s.Contains(device.Name))
                {
                    Form1.THIS.RemoveMenuItem(device.Item);
                    DriveList.Remove(device);
                }
            }
        }

        public string Name;
        public DirectoryInfo RootDir;
        public ToolStripMenuItem Item;
        private string Caption;
        private bool isRun;
        private int VirusNum;

        USBDevice(DriveInfo drive)
        {
            Name = drive.Name;
            RootDir = drive.RootDirectory;
            Caption = drive.VolumeLabel + "(" + drive.Name + ")";
            Item = new ToolStripMenuItem(Caption);
            Form1.THIS.AddMenuItem(Item);
            VirusNum = 0;

            Thread thread = new Thread(new ThreadStart(StartSearch));
            thread.Name = Name + "查杀线程";
            thread.IsBackground = true;
            thread.Start();
        }

        delegate void SetItemTextCallback(string s);
        public void SetItemText(string s)
        {
            Item.Text = s;
        }

        private void StartSearch()
        {
            Logger.Info(Thread.CurrentThread, Caption, "开始自动查杀" + Name);
            Item.Text = Caption + " [正在扫描]";
            if (!Util.isRun) Form1.THIS.AddList("开始自动查杀" + Name);
            SearchDir(RootDir.FullName);
            Logger.Info(Thread.CurrentThread, Caption, Name + "查杀完成！搞定了" + VirusNum + "个病毒");
            if (!Util.isRun) Form1.THIS.AddList(Name + "查杀完成！搞定了" + VirusNum + "个病毒");
            isRun = false;
            SetItemTextCallback d = new SetItemTextCallback(SetItemText);
            Form1.THIS.Invoke(d, new object[] { Caption + " [已扫描]" });
        }
        public void SearchDir(string path)
        {
            if (!path.EndsWith(Util.Separator.ToString())) path += Util.Separator;
            HashSet<string> list = new HashSet<string>();

            DirectoryInfo theFolder = new DirectoryInfo(path);
            DirectoryInfo[] theFolders = null;
            try { theFolders = theFolder.GetDirectories(); }
            catch (Exception e) {
                Logger.Warn(Thread.CurrentThread, Caption, e);
                return;
            }

            //遍历文件夹及子文件夹
            foreach (DirectoryInfo NextDir in theFolders)
            {
                if (NextDir.Attributes.IsSystem())
                    list.Add(NextDir.Name);
                SearchDir(NextDir.FullName);
            }

            //查杀病毒文件
            FileInfo[] theFiles = null;
            foreach (string Extension in Util.ExtensionList)
            {
                try { theFiles = theFolder.GetFiles(Extension, SearchOption.TopDirectoryOnly); }
                catch (Exception e)
                {
                    Logger.Warn(Thread.CurrentThread, Caption, e);
                    return;
                }
                foreach (FileInfo NextFile in theFiles)
                {
                    string s = NextFile.Name.Substring(0, NextFile.Name.Length - 4).TrimEnd();
                    if (list.Contains(s))
                    {
                        AddDeleteInfo(NextFile.FullName);
                        NextFile.Delete();
                        try { File.SetAttributes(path + s, Util.NormalDir); }
                        catch (Exception e)
                        {
                            Logger.Warn(Thread.CurrentThread, Caption, e);
                        }
                    }
                }
            }

            foreach (string theName in Util.SystemFolder)
            {
                if (Directory.Exists(path + theName))
                    try { File.SetAttributes(path + theName, Util.SystemHiddenDir); }
                    catch (Exception e)
                    {
                        Logger.Warn(Thread.CurrentThread, Caption, e);
                    }
            }
        }
        private void AddDeleteInfo(string s)
        {
            if (VirusNum == 0)
                Form1.THIS.AddList(Name + "已删除：");
            Form1.THIS.AddList("> " + s);
            Logger.Info(Thread.CurrentThread, Caption, "删除" + s);
            Logger.Info(Thread.CurrentThread, "删除列表", s);
            VirusNum++;
        }
    }
}
