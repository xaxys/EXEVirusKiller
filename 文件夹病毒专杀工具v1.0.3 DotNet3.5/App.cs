namespace 文件夹病毒专杀工具
{
    static class App
    {
        public volatile static Icon Icon = null;
        public volatile static MainForm MainForm = null;
        public volatile static LogForm LogForm = null;
        public volatile static AboutBox AboutBox = null;
        public volatile static ToolsForm ToolsForm = null;

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
            }
            return MainForm;
        }

        public static LogForm GetLogForm()
        {
            if (LogForm  == null || LogForm.IsDisposed)
            {
                LogForm = new LogForm();
            }
            return LogForm;
        }

        public static AboutBox GetAboutBox()
        {
            if (AboutBox == null || AboutBox.IsDisposed)
            {
                AboutBox = new AboutBox();
            }
            return AboutBox;
        }

        public static ToolsForm GetToolsForm()
        {
            if (ToolsForm == null || ToolsForm.IsDisposed)
            {
                ToolsForm = new ToolsForm();
            }
            return ToolsForm;
        }
    }
}
