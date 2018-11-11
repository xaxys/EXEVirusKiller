using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    static class Logger
    {
        public static SortedList<string, StringBuilder> list = new SortedList<string, StringBuilder>();
        public delegate void AddTextCallback(string key, StringBuilder sb);
        public delegate void InitListCallback();
        public static AddTextCallback AddTextMethod = null;
        public static InitListCallback InitListMethod = null;

        static void BeginInvoke(Delegate d, params object[] obj)
        {
            if (d != null) Util.Icon.BeginInvoke(d, obj);
        }

        public static void Warn(string key, object obj)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new StringBuilder());
                BeginInvoke(InitListMethod);
            }
            StringBuilder sb = null;
            if (obj is Exception)
            {
                Exception e = (Exception)obj;
                sb = new StringBuilder(GetLogText(e.Message));
                sb.Append(GetExceptionInfo(e));
            }
            else if (obj is string)
            {
                string s = (string)obj;
                sb = new StringBuilder(GetLogText(s));
            }
            list[key].Append(sb);
            BeginInvoke(AddTextMethod, key, sb);
        }

        public static void Info(string key, string s)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new StringBuilder());
                BeginInvoke(InitListMethod);
            }
            StringBuilder sb = new StringBuilder(GetLogText(s));
            list[key].Append(sb);
            BeginInvoke(AddTextMethod, key, sb);
        }

        static string GetLogText(string s)
        {
            return String.Format("[{0:T}] [{1}/Info]: {2}",
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
    }
}
