using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadTest
{
    class OrderedThread
    {
        #region 静态字段

        /// <summary>
        /// 当前的线程循序序号
        /// </summary>
        private static int _inner_orderedID = 0;

        /// <summary>
        /// 当前的线程的队列
        /// </summary>
        private static ConcurrentDictionary<int, OrderedThread> inner_readyThreadList = new ConcurrentDictionary<int, OrderedThread>();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static object _objLock = new object();

        #endregion

        #region 成员字段

        /// <summary>
        /// 处理的委托
        /// </summary>
        private Action<int> _inner_action = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="handling">处理的委托</param>
        public OrderedThread(Action<int> handling)
        {
            this._inner_action = handling;
        }

        #endregion

        #region 开始执行线程

        /// <summary>
        /// 开始执行线程
        /// </summary>
        public void Start(bool isBackgroundThread = true)
        {
            //添加到就绪集合中
            inner_readyThreadList.AddOrUpdate(_inner_orderedID++, this, (key, vlaue) =>
            {
                return this;
            });

            Thread th = new Thread(() =>
            {
                bool isEnter = Monitor.TryEnter(_objLock);
                if (isEnter)
                {
                    //Console.WriteLine("新开一个线程--------------------------");

                    //如果就绪队列中有要执行的线程
                    while (inner_readyThreadList.Count() > 0)
                    {
                        //找到起始的最小值
                        int minIndex = inner_readyThreadList.Keys.Min();

                        //绪队列中线程执行
                        inner_readyThreadList[minIndex]._inner_action.Invoke(minIndex);

                        //移除执行完成的
                        OrderedThread orderedThreadTmp = null;
                        inner_readyThreadList.TryRemove(minIndex, out orderedThreadTmp);
                    }

                    Monitor.Exit(_objLock);
                }
            });
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = isBackgroundThread;
            th.Start();
        }

        #endregion
    }
}
