namespace 文件夹病毒专杀工具
{
    static class App
    {
        public volatile static Icon Icon = null;
        public volatile static MainForm MainForm = null;
        public volatile static LogForm LogForm = null;

        public static Icon GetIcon()
        {
            if (Icon == null || Icon.IsDisposed)
            {
                Icon = new Icon();
            }
            return Icon;
        }

        public static MainForm GetMainForm()
        {
            if (MainForm  == null || MainForm.IsDisposed)
            {
                MainForm = new MainForm();
                Logger.Info(Util.MainThread, "主窗体打开");
            }
            return MainForm;
        }

        public static LogForm GetLogForm()
        {
            if (LogForm  == null || LogForm.IsDisposed)
            {
                LogForm = new LogForm();
                Logger.Info(Util.MainThread, "日志信息窗体打开");
            }
            return LogForm;
        }
    }
}
