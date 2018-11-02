using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WindowsFormsApp1
{
    class Logger
    {
        public static SortedList<string, List<string>> list = new SortedList<string, List<string>>();
        public static void Warn(Thread th, string key, Exception e)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new List<string>());
                if (Form1.form2 != null)
                    Form1.form2.InitList();
            }
            string Str = String.Format("[{0:T}] [{1}/Warn]: {2}", DateTime.Now, th.Name, e.Message);
            list[key].Add(Str);
            list[key].Add("    > 异常对象：" + e.Source);
            list[key].Add("    > 调用堆栈：" + e.StackTrace.Trim());
            list[key].Add("    > 触发方法：" + e.TargetSite);
            if (Form1.form2 != null && Form1.form2.Selected == key)
            {
                Form1.form2.AddText(Str);
                Form1.form2.AddText("    > 异常对象：" + e.Source);
                Form1.form2.AddText("    > 调用堆栈：" + e.StackTrace.Trim());
                Form1.form2.AddText("    > 触发方法：" + e.TargetSite);
            }
        }
        public static void Info(Thread th, string key, string s)
        {
            if (!list.ContainsKey(key))
            {
                list.Add(key, new List<string>());
                if (Form1.form2 != null)
                    Form1.form2.InitList();
            }
            string Str = String.Format("[{0:T}] [{1}/Info]: {2}", DateTime.Now, th.Name, s);
            list[key].Add(Str);
            if (Form1.form2 != null && Form1.form2.Selected == key)
            {
                Form1.form2.AddText(Str);
            }
        }
    }
}
