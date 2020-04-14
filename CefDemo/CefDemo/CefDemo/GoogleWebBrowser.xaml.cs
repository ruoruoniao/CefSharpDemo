using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using MSHTML;

namespace CefDemo
{
    /// <summary>
    /// GoogleWebBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class GoogleWebBrowser : UserControl
    {
        #region 成员字段
        //当前页面是否加载完成
        private bool _isDocumentLoaded = false;

        //chrome是否加载完成
        private bool _isChromeLoaded = false;

        private bool _isDocumentGeted = false;
        #endregion

        #region 成员属性

        #region 当前的页面需要调用的C#对象实例
        public object ObjectForScripting
        {
            set
            {
                CSharpEventsBinding(value, false);
            }
        }
        #endregion

        #region 当前页面的Dom树
        /// <summary>
        /// 当前页面的Dom树
        /// </summary>
        public HTMLDocument Document
        {
            get { return GetDocument(); }
        }
        #endregion

        #region 当前页面链接
        /// <summary>
        /// 当前页面链接
        /// </summary>
        public string Source
        {
            get
            {
                if(!IsDocumentLoaded || !IsChromeLoaded)
                {
                    return String.Empty;
                }
                return WebBrowser.Address;
            }
        }
        #endregion

        #region 当前页面是否加载完成
        /// <summary>
        /// 当前页面是否加载完成
        /// </summary>
        public bool IsDocumentLoaded
        {
            get { return _isDocumentLoaded; }
        }
        #endregion

        #region 当前浏览器控件是否加载完成
        /// <summary>
        /// 当前浏览器控件是否加载完成
        /// </summary>
        public bool IsChromeLoaded
        {
            get { return _isChromeLoaded; }
        }
        #endregion

        #region 浏览器加载网页完成事件
        /// <summary>
        /// 谷歌浏览器网页加载完成事件
        /// </summary>
        public event EventHandler<CefSharp.FrameLoadEndEventArgs> LoadCompleted
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
        #endregion

        #region 设定内部浏览器控件大小
        /// <summary>
        /// 设定内部浏览器控件大小
        /// </summary>
        public void SetSize(double width, double height)
        {
            WebBrowser.Width = width;
            WebBrowser.Height = height;
        }

        /// <summary>
        /// 对浏览器打开的网页进行重定向
        /// </summary>
        /// <param name="uri"></param>
        public void Navigate(Uri uri)
        {
            WebBrowser.Address = uri.AbsolutePath;
        }

        /// <summary>
        /// 对浏览器打开的网页进行重定向
        /// </summary>
        /// <param name="uri"></param>
        public void Navigate(string uri)
        {
            WebBrowser.Address = uri;
        }
        #endregion

        #endregion

        #region 构造函数
        public GoogleWebBrowser()
        {
            if (!IsDesignMode(this))
            {
                InitializeCefSettings();
            }
            InitializeComponent();
        }

        /// <summary>
        /// 初始化Cef设置
        /// </summary>
        private void InitializeCefSettings()
        {
            if (Cef.IsInitialized != true)
            {
                CefSettings settings = new CefSettings();
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                CefSharpSettings.WcfEnabled = true;
                settings.CefCommandLineArgs.Add("no-proxy-server", "1");
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                Cef.Initialize(settings);
            }
        }
        #endregion

        #region 前后端交互

        #region 调用页面js方法
        public string InvokeScript(string funcName, bool isEval = false)
        {
            try
            {
                if (!isEval)
                {
                    funcName = JointMethodStr(funcName, null);
                }
                ThreadQueue tq = new ThreadQueue(
                    new Task<Task<JavascriptResponse>>(funcEval => {
                        return WebBrowser.GetBrowser().MainFrame.EvaluateScriptAsync(funcEval.ToString());
                    }, funcName), this);
                tq.Start();
                tq.Wait();
                if (null != tq.Result)
                {
                    if (null != tq.Result.Result)
                    {
                        if (null != tq.Result.Result.Result)
                        {
                            return tq.Result.Result.Result.ToString();
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 调用页面js方法
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="obj">参数列表</param>
        /// <returns>返回值</returns>
        public string InvokeScript(string funcName, object[] obj)
        {
            try
            {
                string func = JointMethodStr(funcName, obj);
                ThreadQueue tq = new ThreadQueue(
                    new Task<Task<JavascriptResponse>>(funcEval => {
                        return WebBrowser.EvaluateScriptAsync(funcEval.ToString());
                    }, func), this);
                tq.Start();
                tq.Wait();
                if(null != tq.Result)
                {
                    if(null != tq.Result.Result)
                    {
                        if(null!= tq.Result.Result.Result)
                        {
                            return tq.Result.Result.Result.ToString();
                        }
                    } 
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 绑定页面中要用到的C#方法类
        /// <summary>
        /// 绑定页面中要用到的C#方法类
        /// </summary>
        /// <param name="BindingClass">要用到的C#方法类</param>
        /// <param name="isAsync">是否异步调用</param>
        public void CSharpEventsBinding(object BindingClass, bool Async)
        {
            try
            {
                WebBrowser.JavascriptObjectRepository.Register("external", BindingClass, isAsync: Async, options: new CefSharp.BindingOptions() { CamelCaseJavascriptNames = false });
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 获取当前页面DOM树
        /// <summary>
        /// 获取当前页面Dom树
        /// </summary>
        private HTMLDocument GetDocument()
        {
            if(!IsChromeLoaded || !IsDocumentLoaded)
            {
                return new HTMLDocument();
            }
            Task<string> returnStr = null;
            IHTMLDocument2 document = (IHTMLDocument2)(new HTMLDocumentClass());
            this.Dispatcher.Invoke(() =>
            {
                returnStr = WebBrowser.GetBrowser().MainFrame.GetSourceAsync();
            });
            var waiter = returnStr.GetAwaiter();
            waiter.OnCompleted(() => {
                this._isDocumentGeted = true;
                document.write(returnStr.Result.ToString());
            });
            return (HTMLDocument)document;
        }
        #endregion

        public async Task<string> GetHtmlStr()
        {
            string result = await WebBrowser.GetSourceAsync();
            return result;
        }
        public async Task<string> GetSourceAsync()
        {
            var result = await WebBrowser.GetSourceAsync();
            return result;
        }

        #region 转换字符串为dom树
        /// <summary>
        /// 转换字符串为dom树
        /// </summary>
        /// <param name="html">html字符串</param>
        /// <returns>HTMLDocument</returns>
        public HTMLDocument GetDocument(string html)
        {
            IHTMLDocument2 document = (IHTMLDocument2)(new HTMLDocumentClass());
            document.write(html);
            return (HTMLDocument)document;
        }
        #endregion

        #endregion

        #region 加载完成方法

        #region 浏览器控件加载完成
        /// <summary>
        /// 浏览器控件加载完成
        /// </summary>
        private void LoadChromeBrowserComplete(object sender, RoutedEventArgs e)
        {
            this._isChromeLoaded = true;
        }
        #endregion

        #region 页面加载完成
        /// <summary>
        /// 页面加载完成
        /// </summary>
        private void LoadWebDocumentComplete(object sender, FrameLoadEndEventArgs e)
        {
            this._isDocumentLoaded = true;
        }
        #endregion

        #endregion

        #region 垃圾清理
        /// <summary>
        /// 结束浏览器页面
        /// </summary>
        public void Shutdown()
        {
            Cef.Shutdown();
        }
        #endregion

        #region 其他工具

        #region 控件代码是否运行于设计模式
        /// <summary>
        /// 控件代码是否运行于设计模式
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        private bool IsDesignMode(UserControl ctl)
        {
            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(ctl);
        }
        #endregion

        #region 拼接函数
        /// <summary>
        /// 拼接函数语句
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="obj">参数</param>
        /// <returns>函数语句</returns>
        private string JointMethodStr(string funcName, object[] obj)
        {
            if("" == funcName || null == funcName)
            {
                throw new MyException("函数名称为空");
            }
            StringBuilder func = new StringBuilder(funcName);
            func.Append("(");
            for (int i = 0; i < obj.Length; i++)
            {
                func.Append("\"");
                func.Append(obj[i]);
                func.Append("\",");
            }
            if (obj.Length > 0)
            {
                func.Remove(func.Length - 1, 1);
            }

            func.Append(")");
            return func.ToString();
        }
        #endregion
        #endregion
    }
    public class MyException : Exception
    {
        private string message = String.Empty;
        public override string Message
        {
            get { return message; }
        }
        public MyException(string msg)
        {
            message = msg;
        }
    }
}
