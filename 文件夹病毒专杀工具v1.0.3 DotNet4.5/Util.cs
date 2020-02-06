using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace 文件夹病毒专杀工具
{
    public static class Util
    {
        public static string zuo = "3";
        public static string xia = "6";
        public static Color huang = Color.FromArgb(255, 255, 128);
        public static Color hong = Color.FromArgb(255, 128, 128);
        public static Color lv = Color.FromArgb(128, 255, 128);
        public static int ChangeHeight = 240;
        public static string MainThread = "主线程";
        public static string CheckThread = "主查杀";
        public static char Separator = Path.DirectorySeparatorChar;
        public readonly static string ProgramPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\文件夹病毒专杀工具\";

        public static bool IsHidden(this FileAttributes attr)
        {
            return (attr & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        public static bool IsSystem(this FileAttributes attr)
        {
            return (attr & FileAttributes.System) == FileAttributes.System;
        }

        public static bool Include(this List<USBDevice> list, DriveInfo drive)
        {
            foreach (USBDevice device in list)
            {
                if (device.Name == drive.Name) return true;
            }
            return false;
        }

        public static USBDevice GetUSBDevice(this List<USBDevice> list, string ItemText)
        {
            foreach (USBDevice device in list)
            {
                if (device.MenuItem.Text == ItemText) return device;
            }
            return null;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccesss, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, uint ReturnLength);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LUID
        {
            public int LowPart;
            public uint HighPart;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privilege;
        }

        public const uint TOKEN_QUERY = 0x0008;
        public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const uint SE_PRIVILEGE_ENABLED = 0x00000002;

        public static void PromoteBackupPrivilege()
        {
            try
            {
                bool flag;
                LUID locallyUniqueIdentifier = new LUID();
                flag = LookupPrivilegeValue(null, "SeBackupPrivilege", ref locallyUniqueIdentifier);
                TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
                tokenPrivileges.PrivilegeCount = 1;

                LUID_AND_ATTRIBUTES luidAndAtt = new LUID_AND_ATTRIBUTES();
                // luidAndAtt.Attributes should be SE_PRIVILEGE_ENABLED to enable privilege
                luidAndAtt.Attributes = SE_PRIVILEGE_ENABLED;
                luidAndAtt.Luid = locallyUniqueIdentifier;
                tokenPrivileges.Privilege = luidAndAtt;

                IntPtr tokenHandle = IntPtr.Zero;
                flag = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
                flag = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024, IntPtr.Zero, 0);
                flag = CloseHandle(tokenHandle);
            }
            catch (Exception e)
            {
                Logger.Warn(Thread.CurrentThread.Name, e);
            }
        }

        public static void PromoteRestorePrivilege()
        {
            try
            {
                bool flag;
                LUID locallyUniqueIdentifier = new LUID();
                flag = LookupPrivilegeValue(null, "SeRestorePrivileg", ref locallyUniqueIdentifier);
                TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
                tokenPrivileges.PrivilegeCount = 1;

                LUID_AND_ATTRIBUTES luidAndAtt = new LUID_AND_ATTRIBUTES();
                // luidAndAtt.Attributes should be SE_PRIVILEGE_ENABLED to enable privilege
                luidAndAtt.Attributes = SE_PRIVILEGE_ENABLED;
                luidAndAtt.Luid = locallyUniqueIdentifier;
                tokenPrivileges.Privilege = luidAndAtt;

                IntPtr tokenHandle = IntPtr.Zero;
                flag = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
                flag = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024, IntPtr.Zero, 0);
                flag = CloseHandle(tokenHandle);
            }
            catch (Exception e)
            {
                Logger.Warn(Thread.CurrentThread.Name, e);
            }
        }
    }
}
