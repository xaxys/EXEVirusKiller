using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace 文件夹病毒专杀工具
{
    public static class Util
    {
        public static Icon Icon = null;
        public static MainForm MainForm = null;
        public static LogForm LogForm = null;

        public static string zuo = "3";
        public static string xia = "6";
        public static Color huang = Color.FromArgb(255, 255, 128);
        public static Color hong = Color.FromArgb(255, 128, 128);
        public static Color lv = Color.FromArgb(128, 255, 128);
        public static int ChangeHeight = 240;
        public static string MainThread = "主线程";
        public static string CheckThread = "主查杀";
        public static char Separator = Path.DirectorySeparatorChar;

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
    }
}
