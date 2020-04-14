using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CefDemo
{
    /// <summary>
    /// 先入先出线程队列
    /// </summary>
    class ThreadQueue
    {
        #region 成员字段
        //线程锁
        private static object _lock = new object();
        //在等待中的线程队列
        private static ConcurrentQueue<ThreadQueue> _ThreadQuery = new ConcurrentQueue<ThreadQueue>();
        //要处理的线程
        private Task<Task<CefSharp.JavascriptResponse>> _action = null;
        //返回值
        private Task<CefSharp.JavascriptResponse> _result = null;
        public Task<CefSharp.JavascriptResponse> Result
        {
            get { return _result; }
        }
        public GoogleWebBrowser _webBrowser = null;
        #endregion

        #region 构造函数
        public ThreadQueue(Task<Task<CefSharp.JavascriptResponse>> handle, GoogleWebBrowser webB)
        {
            this._action = handle;
            this._webBrowser = webB;
        }
        #endregion

        #region 将线程加入线程队列等待自动执行
        /// <summary>
        /// 将线程加入线程队列等待处理
        /// </summary>
        /// <param name="isBackground">是否为后台线程</param>
        public void Start(bool isBackground = true)
        {
            _ThreadQuery.Enqueue(this);
            Thread th = new Thread(() => {
                bool isEnter = Monitor.TryEnter(_lock);
                if (isEnter)
                {
                    while (!_ThreadQuery.IsEmpty)
                    {
                        while (!(_webBrowser.IsChromeLoaded && _webBrowser.IsDocumentLoaded))
                        {
                            Thread.Sleep(15);
                        }
                        ThreadQueue task;
                        _ThreadQuery.TryDequeue(out task);
                        task._action.Start();
                        task._action.Wait();
                        this._result = task._action.Result;
                    }
                    Monitor.Exit(_lock);
                }
            });
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = isBackground;
            th.Start();
        }
        #endregion

        public void Wait()
        {
            SpinWait sw = new SpinWait();
            while (null == this._result)
            {
                sw.SpinOnce();
            }
        }
    }
}
