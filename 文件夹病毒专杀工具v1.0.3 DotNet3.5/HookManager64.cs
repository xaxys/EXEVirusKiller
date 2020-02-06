using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    class HookManager64 : HookManager
    {
        const string DllName = "InjectorDll64.dll";
        const string LogName = "InjectorDll64";
        public bool Statue { get; private set; } = false;
        static callBackHandler func = new callBackHandler(ShowMessage);

        /*
        [DllImport("HookToolDll64.dll")]
        public static extern unsafe int Install();
        [DllImport("HookToolDll64.dll")]
        public static extern unsafe int UnInstall();
        [DllImport("HookToolDll64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void SetCallBack(callBackHandler fun);
        */

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int callBackHandler(int a, IntPtr ptr);

        [DllImport(DllName)]
        public static extern unsafe int Inject(int x);

        [DllImport(DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void SetCallBack(callBackHandler fun);

        
        public bool DisableHook()
        {
            return true;
            /*
            try
            {
                Logger.Info("正在拉起Hook卸载代码");
                int result = UnInstall();
                if (result == 0)
                {
                    Logger.Info("Hook卸载失败");
                    return false;
                }
                Statue = false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                return false;
            }
            */
        }


        public bool EnableHook()
        {
            Thread thread = new Thread(new ThreadStart(EnumAndInject))
            {
                Name = "注入线程",
                IsBackground = true
            };
            thread.Start();
            return true;
        }

        void EnumAndInject()
        {
            Logger.Info("正在安装CallBack函数");
            SetCallBack(func);

            Process[] ProcList = Process.GetProcesses();
            for (int i = 0; i < ProcList.Length; i++)
            {
                Process Proc = ProcList[i];
                try
                {
                    Logger.Info(LogName, string.Format("正在注入 [{0}/{1}] : {2}", i + 1, ProcList.Length, Proc.MainModule.FileName));
                    DoInject(Proc.Id);
                }
                catch (Exception e)
                {
                    Logger.Warn(e);
                }
            }
        }

        bool DoInject(int x)
        {
            try
            {
                Logger.Info(LogName, "正在拉起Hook安装代码");
                int result = Inject(x);
                if (result == 0)
                {
                    Logger.Info(LogName, "Hook安装失败");
                    return false;
                }
                Statue = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                return false;
            }
        }

        public static int ShowMessage(int a, IntPtr ptr)
        {
            string s = Marshal.PtrToStringUni(ptr);
            switch (a)
            {
                case 0:
                    Logger.Info(LogName, "拦截CreateFile请求:" + s);
                    break;
                case 1:
                    Logger.Info(LogName, s);
                    break;
                case 2:
                    Logger.Warn(LogName, s);
                    break;
                case 3:
                    Logger.Info(LogName, "Debug >" + s);
                    break;
            }
            return 0;
        }
    }
}
