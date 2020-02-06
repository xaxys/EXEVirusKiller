using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    class HookManager64 : HookManager
    {
        public bool Statue { get; private set; } = false;
        callBackHandler func = new callBackHandler(ShowMessage);

        [DllImport("HookToolDll64.dll")]
        public static extern unsafe int Install();
        [DllImport("HookToolDll64.dll")]
        public static extern unsafe int UnInstall();
        [DllImport("HookToolDll64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void SetCallBack(callBackHandler fun);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int callBackHandler(int a, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder s);

        public bool DisableHook()
        {
            try
            {
                Logger.Info(Util.MainThread, "正在拉起Hook卸载代码");
                int result = UnInstall();
                if (result == 0)
                {
                    Logger.Info(Util.MainThread, "Hook卸载失败");
                    return false;
                }
                Statue = false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(Util.MainThread, ex);
                return false;
            }
        }

        public bool EnableHook()
        {
            try
            {
                Logger.Info(Util.MainThread, "正在安装CallBack函数");
                SetCallBack(func);
                Logger.Info(Util.MainThread, "正在拉起Hook安装代码");
                int result = Install();
                if (result == 0)
                {
                    Logger.Info(Util.MainThread, "Hook安装失败");
                    return false;
                }
                Statue = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(Util.MainThread, ex);
                return false;
            }
        }
        public static int ShowMessage(int a, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder s)
        {
            switch (a)
            {
                case 0:
                    Logger.Info("HookDll64", "拦截CreateFile请求:" + s.ToString());
                    break;
                case 1:
                    Logger.Info("HookDll64", s.ToString());
                    break;
                case 2:
                    Logger.Warn("HookDll64", "LhInstallHook failed..");
                    break;
                case 3:
                    Logger.Warn("HookDll64", "LhSetInclusiveACL failed..");
                    break;
                case 4:
                    Logger.Info("HookDll64", "InstallHook success...");
                    break;
                case 5:
                    Logger.Info("HookDll64", "UninstallHook success...");
                    break;
                case 6:
                    Logger.Info("HookDll64", "DLL_PROCESS_ATTACH");
                    break;
                case 7:
                    Logger.Info("HookDll64", "DLL_PROCESS_DETACH");
                    break;
            }
            return 0;
        }
    }
}
