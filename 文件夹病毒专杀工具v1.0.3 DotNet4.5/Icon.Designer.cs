namespace 文件夹病毒专杀工具
{
    partial class Icon
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Icon));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.自动扫描U盘 = new System.Windows.Forms.ToolStripMenuItem();
            this.开启目录保护 = new System.Windows.Forms.ToolStripMenuItem();
            this.查看日志信息 = new System.Windows.Forms.ToolStripMenuItem();
            this.工具 = new System.Windows.Forms.ToolStripMenuItem();
            this.关于 = new System.Windows.Forms.ToolStripMenuItem();
            this.退出 = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aToolStripMenuItem,
            this.自动扫描U盘,
            this.开启目录保护,
            this.查看日志信息,
            this.工具,
            this.关于,
            this.退出});
            this.contextMenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 142);
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(145, 6);
            this.aToolStripMenuItem.Visible = false;
            // 
            // 自动扫描U盘
            // 
            this.自动扫描U盘.Checked = true;
            this.自动扫描U盘.CheckState = System.Windows.Forms.CheckState.Checked;
            this.自动扫描U盘.Name = "自动扫描U盘";
            this.自动扫描U盘.Size = new System.Drawing.Size(148, 22);
            this.自动扫描U盘.Text = "自动扫描U盘";
            this.自动扫描U盘.Click += new System.EventHandler(this.自动扫描U盘_Click);
            // 
            // 开启目录保护
            // 
            this.开启目录保护.Name = "开启目录保护";
            this.开启目录保护.Size = new System.Drawing.Size(148, 22);
            this.开启目录保护.Text = "开启目录保护";
            this.开启目录保护.Click += new System.EventHandler(this.开启目录保护_Click);
            // 
            // 查看日志信息
            // 
            this.查看日志信息.Name = "查看日志信息";
            this.查看日志信息.Size = new System.Drawing.Size(148, 22);
            this.查看日志信息.Text = "查看日志信息";
            this.查看日志信息.Click += new System.EventHandler(this.查看日志信息_Click);
            // 
            // 工具
            // 
            this.工具.Name = "工具";
            this.工具.Size = new System.Drawing.Size(148, 22);
            this.工具.Text = "工具";
            this.工具.Click += new System.EventHandler(this.工具_Click);
            // 
            // 关于
            // 
            this.关于.Name = "关于";
            this.关于.Size = new System.Drawing.Size(148, 22);
            this.关于.Text = "关于";
            this.关于.Click += new System.EventHandler(this.关于_Click);
            // 
            // 退出
            // 
            this.退出.Name = "退出";
            this.退出.Size = new System.Drawing.Size(148, 22);
            this.退出.Text = "退出";
            this.退出.Click += new System.EventHandler(this.退出_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "文件夹病毒专杀工具";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // Icon
            // 
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Icon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 自动扫描U盘;
        private System.Windows.Forms.ToolStripMenuItem 查看日志信息;
        private System.Windows.Forms.ToolStripMenuItem 退出;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem 关于;
        private System.Windows.Forms.ToolStripMenuItem 工具;
        private System.Windows.Forms.ToolStripMenuItem 开启目录保护;
    }
}
