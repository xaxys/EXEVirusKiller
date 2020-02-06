using System;
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
                Logger.Info(string.Format("正在启动 {0} v{1}, 版本: {2}, 64位: {3}.",
                    Application.ProductName,
                    Application.ProductVersion,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    Util.Is64BitProcess
                    ));
                Util.PromoteBackupPrivilege();
                Util.PromoteRestorePrivilege();
                App.GetHookManager().EnableHook();
                Icon icon = App.GetIcon();
                USBDevice.CheckDevice();
                Application.Run(icon);
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("已有一个程序正在运行", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
