using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    class Logger
    {
        public static string DateFormat = "yyyy-MM-dd";
        public static string Date = null;
        readonly static int MaxLogLength = 10000;
        public readonly static string LogPath = Util.ProgramPath + @"log\";
        public StreamWriter sw = null;
        public static SortedList<string, StringBuilder> list = new SortedList<string, StringBuilder>();
        public delegate void AddTextCallback(string key, StringBuilder sb);
        public delegate void InitListCallback();
        public static AddTextCallback AddTextMethod = null;
        public static InitListCallback InitListMethod = null;
        private SynchronizedQueue<string> queue = null;

        static void BeginInvoke(Delegate d, params object[] obj)
        {
            if (d != null) App.GetIcon().BeginInvoke(d, obj);
        }

        public static void Warn(string key, object obj)
        {
            App.GetLogger().warn(key, obj);
        }

        public void warn(string key, object obj)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new StringBuilder());
                BeginInvoke(InitListMethod);
            }
            StringBuilder sb = null;
            if (obj is Exception e)
            {
                sb = new StringBuilder(GetLogText(e.Message));
                sb.Append(GetExceptionInfo(e));
            }
            else if (obj is string s)
                sb = new StringBuilder(GetLogText(s));
            list[key].Append(sb);
            AddLog(sb);
            CheckLength(key);
            BeginInvoke(AddTextMethod, key, sb);
        }

        public static void Info(string key, string s)
        {
            App.GetLogger().info(key, s);
        }

        public void info(string key, string s)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new StringBuilder());
                BeginInvoke(InitListMethod);
            }
            StringBuilder sb = new StringBuilder(GetLogText(s));
            list[key].Append(sb);
            AddLog(sb);
            CheckLength(key);
            BeginInvoke(AddTextMethod, key, sb);
        }

        static string GetLogText(string s)
        {
            return string.Format("[{0:T}] [{1}/Info]: {2}",
                DateTime.Now, Thread.CurrentThread.Name, s) + Environment.NewLine;
        }

        static StringBuilder GetExceptionInfo(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    > 异常对象：" + e.Source);
            sb.AppendLine("    > 调用堆栈：" + e.StackTrace.Trim());
            sb.AppendLine("    > 触发方法：" + e.TargetSite);
            return sb;
        }

        void CheckLength(string key)
        {
            var sb = list[key];
            while (sb.Length >= MaxLogLength)
            {
                int idx = 1;
                while (sb[idx] != '[') idx++;
                sb.Remove(0, idx);
            }
        }

        void AddLog(StringBuilder sb)
        {
            if (DateTime.Now.ToString(DateFormat) != Date)
            {
                Date = DateTime.Now.ToString(DateFormat);
                sw.Close();
                sw = new StreamWriter(LogPath + Date + ".log", true, Encoding.UTF8);
            }
            queue.Enqueue(sb.ToString());
        }


        public Logger()
        {
            Date = DateTime.Now.ToString(DateFormat);
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            sw = new StreamWriter(LogPath + Date + ".log", true, Encoding.UTF8);
            queue = new SynchronizedQueue<string>(true);
            Thread thread = new Thread(new ThreadStart(Record))
            {
                Name = "日志线程",
                IsBackground = true
            };
            thread.Start();
        }

        private void Record()
        {
            while (true)
            {
                string s = queue.Dequeue();
                try
                {
                    sw.Write(s);
                    sw.Flush();
                }
                catch (Exception e)
                {
                    Warn(Util.MainThread, e);
                }
            }
        }
    }
}
