using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace WindowsFormsApp1
{
    public static class Util
    {
        public static bool isRun;
        private static bool SubFolder;
        private static bool FixFolder;
        private static bool FuzzyCheck;
        public static int VirusNum;
        private static double progress;
        public static double Progress
        {
            get { return progress; }
            set
            {
                if ((int)value != (int)progress)
                    Form1.THIS.SetProgressBar((int)value);
                progress = value;
            }
        }

        public static List<string> SystemFolder = new List<string>()
        {
            "System Volume Information",
            "$RECYCLE.BIN",
            ".Trash",
            "LOST.DIR"
        };

        public static string[] ExtensionList =
        {
            "*.exe",
            "*.scr"
        };

        public static string zuo = "3";
        public static string xia = "6";
        public static Color huang = Color.FromArgb(255, 255, 128);
        public static Color hong = Color.FromArgb(255, 128, 128);
        public static Color lv = Color.FromArgb(128, 255, 128);
        public static int ChangeHeight = 240;
        public static string MainThread = "主线程";
        public static string CheckThread = "主查杀线程";
        public static char Separator = Path.DirectorySeparatorChar;

        public static void SetState(bool a, bool b, bool c)
        {
            SubFolder = a;
            FixFolder = b;
            FuzzyCheck = c;
            VirusNum = 0;
            Progress = 0;
        }

        public static FileAttributes NormalDir = FileAttributes.Normal | FileAttributes.Directory;
        public static FileAttributes SystemHiddenDir = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory;

        public static bool IsHidden(this FileAttributes attr)
        {
            return (attr & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        public static bool IsSystem(this FileAttributes attr)
        {
            return (attr & FileAttributes.System) == FileAttributes.System;
        }

        public static void Add(this StringBuilder Str, string s)
        {
            Str.Append(s + Environment.NewLine);
        }
        public static bool Include(this List<USBDevice> list, DriveInfo drive)
        {
            foreach (USBDevice device in list)
            {
                if (device.Name == drive.Name) return true;
            }
            return false;
        }

        public static USBDevice GetUSBDevice(this List<USBDevice> list, string Caption)
        {
            foreach (USBDevice device in list)
            {
                if (device.Caption == Caption) return device;
            }
            return null;
        }

        private static void AddDeleteInfo(string s)
        {
            if (VirusNum == 0)
                Form1.THIS.AddList("已删除：");
            Form1.THIS.AddList("> " + s);
            Logger.Info(CheckThread, "删除" + s);
            Logger.Info("删除列表", s);
            VirusNum++;
        }

        public static void SearchDir(string path, double k)
        {
            Form1.THIS.SetLabel3(path);
            if (!path.EndsWith(Separator.ToString())) path += Separator;

            HashSet<string> list = new HashSet<string>();
            DirectoryInfo theFolder = new DirectoryInfo(path);

            DirectoryInfo[] theFolders = null;
            try { theFolders = theFolder.GetDirectories(); }
            catch (Exception e) { Form1.THIS.AddList(e); return; }

            double ProgressNow = Progress;
            double step = 1.00 / theFolders.Length * k;
            int i = 1;

            //遍历文件夹及子文件夹
            foreach(DirectoryInfo NextDir in theFolders)
            {
                if (!isRun) break;
                if (FixFolder && NextDir.Attributes.IsHidden())
                {
                    try { NextDir.Attributes = NormalDir; }
                    catch (Exception e) { Form1.THIS.AddList(e); }
                }
                if (FuzzyCheck || NextDir.Attributes.IsSystem())
                    list.Add(NextDir.Name);
                if (SubFolder) SearchDir(path + NextDir.Name + '\\', step);
                if (step > 1e-3) Progress = ProgressNow + step * i++;
            }

            //查杀病毒文件
            FileInfo[] theFiles = null;
            foreach (string Extension in ExtensionList) {
                try { theFiles = theFolder.GetFiles(Extension, SearchOption.TopDirectoryOnly); }
                catch (Exception e) { Form1.THIS.AddList(e); return; }
                foreach (FileInfo NextFile in theFiles)
                {
                    string s = NextFile.Name.Substring(0, NextFile.Name.Length - 4).TrimEnd();
                    if (list.Contains(s))
                    {
                        AddDeleteInfo(NextFile.FullName);
                        NextFile.Delete();
                        if (!FixFolder)
                        {
                            try { File.SetAttributes(path + s, NormalDir); }
                            catch (Exception e) { Form1.THIS.AddList(e); }
                        }
                    }
                }
            }

            if (isRun)
            {
                foreach (string theName in SystemFolder)
                {
                    if (Directory.Exists(path + theName))
                        try { File.SetAttributes(path + theName, SystemHiddenDir); }
                        catch (Exception e) { Form1.THIS.AddList(e); }
                }
            }
        }
    }
}
