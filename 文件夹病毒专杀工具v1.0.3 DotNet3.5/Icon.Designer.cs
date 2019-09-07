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
            this.自动扫描U盘ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看日志信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工具toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.关于toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aToolStripMenuItem,
            this.自动扫描U盘ToolStripMenuItem,
            this.查看日志信息ToolStripMenuItem,
            this.工具toolStripMenuItem1,
            this.关于toolStripMenuItem1,
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 120);
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(145, 6);
            this.aToolStripMenuItem.Visible = false;
            // 
            // 自动扫描U盘ToolStripMenuItem
            // 
            this.自动扫描U盘ToolStripMenuItem.Checked = true;
            this.自动扫描U盘ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.自动扫描U盘ToolStripMenuItem.Name = "自动扫描U盘ToolStripMenuItem";
            this.自动扫描U盘ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.自动扫描U盘ToolStripMenuItem.Text = "自动扫描U盘";
            this.自动扫描U盘ToolStripMenuItem.Click += new System.EventHandler(this.自动扫描U盘ToolStripMenuItem_Click);
            // 
            // 查看日志信息ToolStripMenuItem
            // 
            this.查看日志信息ToolStripMenuItem.Name = "查看日志信息ToolStripMenuItem";
            this.查看日志信息ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.查看日志信息ToolStripMenuItem.Text = "查看日志信息";
            this.查看日志信息ToolStripMenuItem.Click += new System.EventHandler(this.查看日志信息ToolStripMenuItem_Click);
            // 
            // 工具toolStripMenuItem1
            // 
            this.工具toolStripMenuItem1.Name = "工具toolStripMenuItem1";
            this.工具toolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.工具toolStripMenuItem1.Text = "工具";
            this.工具toolStripMenuItem1.Click += new System.EventHandler(this.工具toolStripMenuItem1_Click);
            // 
            // 关于toolStripMenuItem1
            // 
            this.关于toolStripMenuItem1.Name = "关于toolStripMenuItem1";
            this.关于toolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.关于toolStripMenuItem1.Text = "关于";
            this.关于toolStripMenuItem1.Click += new System.EventHandler(this.关于toolStripMenuItem1_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem 自动扫描U盘ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看日志信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem 关于toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 工具toolStripMenuItem1;
    }
}
