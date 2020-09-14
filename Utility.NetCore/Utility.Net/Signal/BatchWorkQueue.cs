using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Utility.Signal
{
    /// <summary>
    /// 单线程批处理任务队列
    /// </summary>
    /// <typeparam name="TItem">等待处理的数据类型</typeparam>
    public abstract class BatchWorkQueue<TItem>
    {
        /// <summary>
        /// 数据通知
        /// </summary>
        AutoResetEvent _signal = new AutoResetEvent(false);

        /// <summary>
        /// 数据队列
        /// </summary>
        ConcurrentQueue<TItem> _queue = new ConcurrentQueue<TItem>();

        /// <summary>
        /// 每次处理的数据条数
        /// </summary>
        private readonly int _pageSize = 10;
        /// <summary>
        /// 初始化队列
        /// </summary>
        public BatchWorkQueue(int pageSize)
        {
            if (pageSize > 0)
            {
                _pageSize = pageSize;
            }
            new Thread(Worker)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            }.Start();//启动处理线程
        }

        /// <summary>
        /// 处理线程
        /// </summary>
        private void Worker()
        {
            while (_signal.WaitOne())
            {
                try
                {
                    while (true)
                    {
                        List<TItem> data = new List<TItem>();
                        while (data.Count < _pageSize && _queue.TryDequeue(out TItem item))
                        {
                            data.Add(item);
                        }
                        if (data.Count > 0)
                        {
                            try
                            {
                                Process(data);
                            }
                            catch
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 插入待处理数据
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(TItem item)
        {
            _queue.Enqueue(item);
            _signal.Set();
        }

        /// <summary>
        /// 处理一批数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract void Process(List<TItem> data);
    }
}
