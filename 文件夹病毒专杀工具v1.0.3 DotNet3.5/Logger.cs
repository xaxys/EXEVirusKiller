using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    class Logger
    {
        public static SortedList<string, StringBuilder> list = new SortedList<string, StringBuilder>();
        public static void Warn(string key, Exception e)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new StringBuilder());
                list[key].Capacity = 10;
                if (Util.LogForm != null) Util.LogForm.InitList();
            }
            StringBuilder sb = new StringBuilder(GetLogText(e.Message));
            sb.Append(GetExceptionInfo(e));
            list[key].Append(sb);
            if (Util.LogForm != null && Util.LogForm.Selected == key)
            {
                Util.LogForm.AddText(sb);
            }
        }
        public static void Info(string key, string s)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new StringBuilder());
                if (Util.LogForm != null) Util.LogForm.InitList();
            }
            StringBuilder sb = new StringBuilder(GetLogText(s));
            list[key].Append(sb);
            if (Util.LogForm != null && Util.LogForm.Selected == key)
            {
                Util.LogForm.AddText(sb);
            }
        }

        public static string GetLogText(string s)
        {
            return String.Format("[{0:T}] [{1}/Info]: {2}",
                DateTime.Now, Thread.CurrentThread.Name, s) + Environment.NewLine;
        }

        public static StringBuilder GetExceptionInfo(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    > 异常对象：" + e.Source);
            sb.AppendLine("    > 调用堆栈：" + e.StackTrace.Trim());
            sb.AppendLine("    > 触发方法：" + e.TargetSite);
            return sb;
        }
    }
}
