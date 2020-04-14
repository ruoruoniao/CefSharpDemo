using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Diagnostics;

namespace CefDemo
{
    public partial class GoogleWebBrowserForWinform : UserControl
    {
        public ChromiumWebBrowser WebBrowser = null;

        public event EventHandler<CefSharp.FrameLoadEndEventArgs> FrameLoadEnd
        {
            add
            {
                WebBrowser.FrameLoadEnd += value;
            }
            remove
            {
                WebBrowser.FrameLoadEnd -= value;
            }

        }

        public GoogleWebBrowserForWinform()
        {
            InitializeComponent();
            if (!IsDesignMode(this))
            {
                InitializeChromiumWebBrowser();
            }
        }

        /// <summary>
        /// 初始化Cef设置
        /// </summary>
        private void InitializeCefSettings()
        {
            //全局下Cef只能被初始化一次
            if(Cef.IsInitialized != true)
            {
                CefSettings settings = new CefSettings();
                //允许调用JS函数调用后端代码
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                CefSharpSettings.WcfEnabled = true;
                //禁用代理设置
                settings.CefCommandLineArgs.Add("no-proxy-server", "1");
                //禁用硬件加速
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                Cef.Initialize(settings);
            }
        }

        private void InitializeChromiumWebBrowser()
        {
            InitializeCefSettings();
            this.WebBrowser = new ChromiumWebBrowser("about:blank");
            this.WebBrowser.Dock = DockStyle.Fill;
            this.WebBrowser.Parent = this.WebBrowserPanel;
        }

        #region 控件代码是否运行于设计模式
        /// <summary>
        /// 控件代码是否运行于设计模式
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        private bool IsDesignMode(UserControl ctl)
        {
            bool returnFlag = false;

            #if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
            #endif

            return returnFlag;
        }
        #endregion
    }
}
