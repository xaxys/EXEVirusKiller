using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    class Killer
    {
        static readonly FileAttributes NormalDir = FileAttributes.Normal | FileAttributes.Directory;
        static readonly FileAttributes SystemHiddenDir = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory;
        static readonly List<string> SystemFolder = new List<string>()
        {
            "System Volume Information",
            "$RECYCLE.BIN",
            ".Trash",
            "LOST.DIR"
        };
        static readonly string[] ExtensionList =
        {
            "*.exe",
            "*.scr"
        };

        void Invoke(Delegate d, params object[] obj)
        {
            if (d != null) Util.Icon.Invoke(d, obj);
        }

        public delegate void SetProcessBarCallback(int v);
        public delegate void AddListCallback(object obj);
        public delegate void SetLabelCallback(string s);
        public delegate void FinishCheckCallback();
        public SetProcessBarCallback SetProcessBarMethod = null;
        public AddListCallback AddListMethod = null;
        public SetLabelCallback SetLabelMethod = null;
        public FinishCheckCallback FinishCheckMethod = null;

        public string RootDir;
        public string ThreadName;
        public bool IsRun = false;
        bool SubFolder = true;
        bool FixFolder = false;
        bool FuzzyCheck = false;
        int VirusNum = 0;
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

        public int Run()
        {
            Logger.Info(Util.MainThread, "启动" + ThreadName + "线程");
            Thread thread = new Thread(new ThreadStart(StartSearch))
            {
                Name = ThreadName + "线程",
                IsBackground = true
            };
            thread.Start();
            return VirusNum;
        }

        void StartSearch()
        {
            Progress = 0;
            IsRun = true;
            Logger.Info(ThreadName, "开始查杀" + RootDir);
            SearchDir(RootDir, 100);
            Progress = 0;
            IsRun = false;
            Logger.Info(ThreadName, RootDir + "查杀完成！搞定了" + VirusNum + "个病毒");
            Invoke(AddListMethod, RootDir + "查杀完成！搞定了" + VirusNum + "个病毒");
            Invoke(FinishCheckMethod);
        }

        void AddDeleteInfo(string s)
        {
            if (AddListMethod != null)
            {
                if (VirusNum == 0)
                    Invoke(AddListMethod, "已删除：");
                Invoke(AddListMethod, "> " + s);
            }
            Logger.Info(ThreadName, "删除" + s);
            Logger.Info("删除列表", s);
            VirusNum++;
        }

        void SearchDir(string path, double k)
        {   
            Invoke(SetLabelMethod, path);
            if (!path.EndsWith(Util.Separator.ToString())) path += Util.Separator;

            HashSet<string> list = new HashSet<string>();
            DirectoryInfo theFolder = new DirectoryInfo(path);
            DirectoryInfo[] theFolders = null;
            try { theFolders = theFolder.GetDirectories(); }
            catch (Exception e) { Invoke(AddListMethod, e); return; }

            double ProgressNow = Progress;
            double step = 1.00 / theFolders.Length * k;
            int i = 1;

            //遍历文件夹及子文件夹
            foreach (DirectoryInfo NextDir in theFolders)
            {
                if (!IsRun) break;
                if (FixFolder && NextDir.Attributes.IsHidden())
                {
                    try { NextDir.Attributes = NormalDir; }
                    catch (Exception e) { Invoke(AddListMethod, e); }
                }
                if (FuzzyCheck || NextDir.Attributes.IsSystem())
                    list.Add(NextDir.Name);
                if (SubFolder) SearchDir(path + NextDir.Name + Util.Separator, step);
                if (step > 1e-3) Progress = ProgressNow + step * i++;
            }

            //查杀病毒文件
            FileInfo[] theFiles = null;
            foreach (string Extension in ExtensionList)
            {
                try { theFiles = theFolder.GetFiles(Extension, SearchOption.TopDirectoryOnly); }
                catch (Exception e) { Invoke(AddListMethod, e); return; }
                foreach (FileInfo NextFile in theFiles)
                {
                    string s = NextFile.Name.Substring(0, NextFile.Name.Length - Extension.Length).TrimEnd();
                    if (list.Contains(s))
                    {
                        AddDeleteInfo(NextFile.FullName);
                        NextFile.Delete();
                        if (!FixFolder)
                        {
                            try { File.SetAttributes(path + s, NormalDir); }
                            catch (Exception e) { Invoke(AddListMethod, e); }
                        }
                    }
                }
            }

            foreach (string theName in SystemFolder)
            {
                if (Directory.Exists(path + theName))
                {
                    try { File.SetAttributes(path + theName, SystemHiddenDir); }
                    catch (Exception e) { Invoke(AddListMethod, e); }
                }
            }
        }
    }
}
