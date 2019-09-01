using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    class Killer
    {
        static readonly FileAttributes NormalDir = FileAttributes.Normal | FileAttributes.Directory;
        static readonly FileAttributes SystemHiddenDir = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory;
        static readonly string[] SystemFolder =
        {
            "System Volume Information",
            "$RECYCLE.BIN",
            ".Trash",
            "LOST.DIR"
        };
        static readonly string[] ExtensionList =
        {
            ".exe",
            ".scr"
        };
        static readonly Regex[] ExtraVirus = 
        {
            new Regex("^Data Administrator.exe"),
            new Regex("^New Folder.*.scr")
        };

        void Invoke(Delegate d, params object[] obj)
        {
            if (d != null) App.GetIcon().Invoke(d, obj);
        }

        public delegate void SetProcessBarCallback(int v);
        public delegate void AddListCallback(object obj);
        public delegate void SetLabelCallback(string s);
        public delegate void FinishCheckCallback();
        public delegate void SetNumCallback(int v);
        public SetProcessBarCallback SetProcessBarMethod = null;
        public AddListCallback AddListMethod = null;
        public SetLabelCallback SetLabelMethod = null;
        public FinishCheckCallback FinishCheckMethod = null;
        public SetNumCallback SetVirusNumMethod = null;
        public SetNumCallback SetFixedNumMethod = null;

        public string RootDir;
        public string ThreadName;
        public bool IsRun = false;
        public bool Abort = false;
        bool SubFolder = true;
        bool FixFolder = false;
        bool FuzzyCheck = false;
        int VirusNum = 0;
        int FixedNum = 0;

        double progress = 0;
        double Progress
        {
            get { return progress; }
            set
            {
                if (SetProcessBarMethod != null && (int)value != (int)progress)
                    Invoke(SetProcessBarMethod, (int)value);
                progress = value;
            }
        }

        public Killer(string threadName)
        {
            ThreadName = threadName;
        }

        public void SetOption(bool a, bool b, bool c)
        {
            SubFolder = a;
            FixFolder = b;
            FuzzyCheck = c;
        }

        public void Run()
        {
            Logger.Info(Util.MainThread, "启动" + ThreadName + "线程");
            Thread thread = new Thread(new ThreadStart(StartSearch))
            {
                Name = ThreadName + "线程",
                IsBackground = true
            };
            thread.Start();
        }

        void StartSearch()
        {
            Progress = 0;
            IsRun = true;
            Abort = false;
            Logger.Info(ThreadName, "开始查杀" + RootDir);
            Invoke(AddListMethod, "开始查杀" + RootDir);

            SearchDir(RootDir, 100);

            Progress = 0;
            IsRun = false;
            if (Abort)
            {
                Logger.Info(ThreadName, ThreadName + "线程被终止");
                Invoke(AddListMethod, ThreadName + "线程被终止");
            }
            Logger.Info(ThreadName, RootDir + " 查杀完成！清除了" + VirusNum + "个病毒, 修复了" + FixedNum + "个文件夹");
            Invoke(AddListMethod, RootDir + " 查杀完成！清除了" + VirusNum + "个病毒, 修复了" + FixedNum + "个文件夹");
            Invoke(SetVirusNumMethod, VirusNum);
            Invoke(SetFixedNumMethod, FixedNum);
            Invoke(FinishCheckMethod);
        }

        void AddDeleteInfo(string s)
        {
            if (AddListMethod != null)
            {
                Invoke(AddListMethod, "删除: " + s);
            }
            Logger.Info(ThreadName, "删除: " + s);
            Logger.Info("删除列表", s);
            VirusNum++;
        }

        void AddFixedInfo(string s)
        {
            if (AddListMethod != null)
            {
                Invoke(AddListMethod, "修复: " + s);
            }
            Logger.Info(ThreadName, "修复: " + s);
            FixedNum++;
        }

        void AddExceptionInfo(Exception e)
        {
            Logger.Warn(ThreadName, e);
            Invoke(AddListMethod, e);
        }

        private void SearchDir(string path, double k)
        {
            try {
                Invoke(SetLabelMethod, path);
                if (!path.EndsWith(Util.Separator.ToString())) path += Util.Separator;

                HashSet<string> list = new HashSet<string>();
                DirectoryInfo theFolder = new DirectoryInfo(path);
                DirectoryInfo[] theFolders = null;
                try { theFolders = theFolder.GetDirectories(); }
                catch (Exception e) { AddExceptionInfo(e); return; }

                double ProgressNow = Progress;
                double step = 1.00 / theFolders.Length * k;
                int i = 1;

                //遍历文件夹及子文件夹
                foreach (DirectoryInfo NextDir in theFolders)
                {
                    if (Abort) return;
                    if (FixFolder && NextDir.Attributes.IsHidden())
                    {
                        try {
                            NextDir.Attributes = NormalDir;
                            AddFixedInfo(NextDir.FullName);
                        }
                        catch (Exception e) { AddExceptionInfo(e); }
                    }
                    if (FuzzyCheck || NextDir.Attributes.IsSystem())
                        list.Add(NextDir.Name);
                }

                //查杀病毒文件
                FileInfo[] theFiles = null;
                foreach (string Extension in ExtensionList)
                {
                    try { theFiles = theFolder.GetFiles("*" + Extension, SearchOption.TopDirectoryOnly); }
                    catch (Exception e) { AddExceptionInfo(e); return; }
                    foreach (FileInfo NextFile in theFiles)
                    {
                        if (Abort) return;
                        string s = NextFile.Name.Substring(0, NextFile.Name.Length - Extension.Length).TrimEnd();
                        if (list.Contains(s))
                        {
                            AddDeleteInfo(NextFile.FullName);
                            NextFile.Delete();
                            if (!FixFolder)
                            {
                                try
                                {
                                    File.SetAttributes(path + s, NormalDir);
                                    AddFixedInfo(path + s);
                                }
                                catch (Exception e) { AddExceptionInfo(e); }
                            }
                        }
                        else
                        {
                            //清除多余病毒
                            foreach (Regex regex in ExtraVirus)
                            {
                                if (regex.IsMatch(s))
                                {
                                    AddDeleteInfo(NextFile.FullName);
                                    NextFile.Delete();
                                }
                            }
                        }
                    }
                }

                //隐藏系统文件夹
                foreach (string theName in SystemFolder)
                {
                    if (Directory.Exists(path + theName))
                    {
                        try { File.SetAttributes(path + theName, SystemHiddenDir); }
                        catch (Exception e) { AddExceptionInfo(e); }
                    }
                }

                //扫描子文件夹
                if (SubFolder)
                {
                    foreach (DirectoryInfo NextDir in theFolders)
                    {
                        if (Abort) return;
                        SearchDir(path + NextDir.Name + Util.Separator, step);
                        if (step > 1e-2) Progress = ProgressNow + step * i++;
                    }
                }
            }
            catch(Exception e) { AddExceptionInfo(e); }
        }
    }
}
