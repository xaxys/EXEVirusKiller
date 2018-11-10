using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace 文件夹病毒专杀工具
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.Name = "主线程";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Mutex mutex = new Mutex(true, Application.ProductName, out bool ret);
            if (ret)
            {
                Util.Icon = new Icon();
                Application.Run();
            }
            else
            {
                MessageBox.Show("该程序已经启动", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
