using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.NetCore.Signal
{
    /// <summary>
    /// 单线程任务队列
    /// </summary>
    /// <typeparam name="TData">等待处理的数据类型</typeparam>
    public abstract class AsyncWorkQueue<TData>
    {
        /// <summary>
        /// 数据通知
        /// </summary>
        AutoResetEvent _signal = new AutoResetEvent(false);

        /// <summary>
        /// 数据队列
        /// </summary>
        ConcurrentQueue<TData> _queue = new ConcurrentQueue<TData>();

        /// <summary>
        /// 初始化队列
        /// </summary>
        public AsyncWorkQueue()
        {
            new Thread(async () => await Worker())
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            }.Start(); //启动处理线程
        }

        /// <summary>
        /// 处理线程
        /// </summary>
        private async Task Worker()
        {
            await Task.Yield();
            while (_signal.WaitOne())
            {
                while (_queue.TryDequeue(out TData data))
                {
                    try
                    {
                        await ProcessOneAsync(data);
                    }
                    catch
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }

        /// <summary>
        /// 插入待处理数据
        /// </summary>
        /// <param name="data"></param>
        public void Enqueue(TData data)
        {
            _queue.Enqueue(data);
            _signal.Set();
        }

        /// <summary>
        /// 处理一条数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task ProcessOneAsync(TData data);
    }
}
