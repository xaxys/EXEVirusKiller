using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1
{
    public static class Util
    {
        public static bool isRun;
        public static bool SubFolder;
        public static bool FixFolder;
        public static bool HideSystemFolder;
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

        public static string zuo = "3";
        public static string xia = "6";
        public static Color huang = Color.FromArgb(255, 255, 128);
        public static Color hong = Color.FromArgb(255, 128, 128);
        public static Color lv = Color.FromArgb(128, 255, 128);
        public static int ShowHeight = 520;
        public static int HideHeight = 280;
        public static void SetState(bool a, bool b, bool c)
        {
            SubFolder = a;
            FixFolder = b;
            HideSystemFolder = c;
            VirusNum = 0;
            progress = 0;
        }

        public static FileAttributes NormalDir = FileAttributes.Normal | FileAttributes.Directory;
        public static FileAttributes SystemHiddenDir = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory;

        public static bool IsHidden(this FileAttributes attr)
        {
            return (attr & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        private static void AddDeleteInfo(string s)
        {
            if (VirusNum == 0)
                Form1.THIS.AddList("已删除：");
            Form1.THIS.AddList(">>> " + s);
            VirusNum++;
        }

        public static void SearchDir(string path, double k)
        {
            Form1.THIS.SetLabel3(path);
            if (!path.EndsWith("\\")) path += '\\';

            HashSet<string> list = new HashSet<string>();
            DirectoryInfo theFolder = new DirectoryInfo(path);
            DirectoryInfo[] theFolders = null;
            try { theFolders = theFolder.GetDirectories(); }
            catch (Exception e) { Form1.THIS.AddList(e); return; }

            void CheckDir(string Extension)
            {
                FileInfo[] theFiles = null;
                try { theFiles = theFolder.GetFiles(Extension, SearchOption.TopDirectoryOnly); }
                catch (Exception e) { Form1.THIS.AddList(e); return; }
                foreach (FileInfo NextFile in theFiles)
                {
                    string s = NextFile.Name.Substring(0, NextFile.Name.Length - 4).TrimEnd();
                    if (list.Contains(s))
                    {
                        NextFile.Delete();
                        AddDeleteInfo(NextFile.FullName);
                        if (!FixFolder)
                        {
                            try { File.SetAttributes(path + s, NormalDir); }
                            catch (Exception e) { Form1.THIS.AddList(e); }
                        }
                    }
                }
            }

            double ProgressNow = Progress;
            double step = 1.00 / theFolders.Length * k;
            int i = 1;

            foreach(DirectoryInfo NextDir in theFolders)
            {
                if (!isRun) break;
                if (FixFolder && NextDir.Attributes.IsHidden())
                {
                    try { NextDir.Attributes = NormalDir; }
                    catch (Exception e) { Form1.THIS.AddList(e); }
                }
                list.Add(NextDir.Name);
                if (SubFolder) SearchDir(path + NextDir.Name + '\\', step);
                if (step > 1e-3) Progress = ProgressNow + step * i++;
            }

            CheckDir(".exe");
            CheckDir(".scr");

            if (HideSystemFolder && isRun)
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
