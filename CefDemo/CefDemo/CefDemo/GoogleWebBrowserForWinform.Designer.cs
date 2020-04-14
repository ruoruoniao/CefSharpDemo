namespace CefDemo
{
    partial class GoogleWebBrowserForWinform
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
            this.WebBrowserPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // WebBrowserPanel
            // 
            this.WebBrowserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebBrowserPanel.Location = new System.Drawing.Point(0, 0);
            this.WebBrowserPanel.Name = "WebBrowserPanel";
            this.WebBrowserPanel.Size = new System.Drawing.Size(150, 150);
            this.WebBrowserPanel.TabIndex = 0;
            // 
            // GoogleWebBrowserForWinform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.WebBrowserPanel);
            this.Name = "GoogleWebBrowserForWinform";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel WebBrowserPanel;
    }
}
