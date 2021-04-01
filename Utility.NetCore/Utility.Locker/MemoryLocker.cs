using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;

namespace Utility.NetLocker
{
    /// <summary>
    /// 
    /// </summary>
    public class MemoryLocker : IDisposable
    {
        /// <summary>
        /// 锁键值超时时间
        /// </summary>
        const int KEY_EXPIRE_MILLISECOND = 2000;

        /// <summary>
        /// 刷新锁的时间间隔
        /// </summary>

        const int RENEW_TIME = 200;

        /// <summary>
        /// 一个随机数值，确认这个锁是本线程创建的
        /// </summary>
        private readonly Guid _uniqueValue;

        /// <summary>
        /// 锁键值
        /// </summary>
        private readonly string _lockKey;

        /// <summary>
        /// redis客户端实例
        /// </summary>
        private readonly IMemoryCache _memory;

        /// <summary>
        /// 
        /// </summary>
        private bool _disposing;

        /// <summary>
        /// 最大锁定时间
        /// </summary>
        private readonly int _maxLockTimeMilliSecond;

        /// <summary>
        /// 锁延期定时器
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// 占用锁的开始时间
        /// </summary>
        private readonly DateTime _lockTime;

        /// <summary>
        /// 是否成功申请到锁
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 创建一个内存锁，请使用using确保调用Dispose,
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="lockKey">竞争的Key</param>
        /// <param name="timeoutMilliSecond">超时时间，0表示无限等待,超时后可以通过Success字段判断是否成功</param>
        /// <param name="maxLockTimeMilliSecond">最大锁定时间，超过该时间后，即使事务未结束也将释放锁，默认是10000</param>
        public MemoryLocker(IMemoryCache memory, string lockKey, int timeoutMilliSecond, int maxLockTimeMilliSecond = 10000)
        {
            _lockKey = lockKey;
            _memory = memory;
            _uniqueValue = Guid.NewGuid();
            _maxLockTimeMilliSecond = maxLockTimeMilliSecond;
            Random rnd = new Random();
            var startTime = DateTime.Now;
            while (timeoutMilliSecond == 0 || DateTime.Now < startTime.AddMilliseconds(timeoutMilliSecond))
            {
                var key = _memory.Get<Guid>(_lockKey);
                if (key == null || key == Guid.Empty)
                {
                    _memory.Set(_lockKey, _uniqueValue, TimeSpan.FromMilliseconds(KEY_EXPIRE_MILLISECOND));
                    Success = true;
                    _lockTime = DateTime.Now;
                    _timer = new Timer(RenewExpire, null, RENEW_TIME, Timeout.Infinite);
                    break;
                }
                else
                {
                    Thread.Sleep(rnd.Next(30, 100));
                }
            }

        }

        /// <summary>
        /// 如果这个线程还活着，就去内存做个延期操作
        /// </summary>
        /// <param name="state"></param>
        private void RenewExpire(object state)
        {
            try
            {
                if (_memory.Get<Guid>(_lockKey) == _uniqueValue)
                {
                    _memory.Set(_lockKey, _uniqueValue, TimeSpan.FromMilliseconds(KEY_EXPIRE_MILLISECOND));

                    if (!_disposing && (DateTime.Now - _lockTime).TotalMilliseconds + KEY_EXPIRE_MILLISECOND < _maxLockTimeMilliSecond)
                    {
                        _timer.Change(RENEW_TIME, Timeout.Infinite);
                    }
                }
            }
            catch //disposing
            {

            }
        }

        /// <summary>
        /// 释放锁，如果是自己创建的就删除键值
        /// </summary>
        public void Dispose()
        {
            _disposing = true;
            if (_timer != null)
            {
                _timer.Dispose();
            }
            if (Success && _memory.Get<Guid>(_lockKey) == _uniqueValue)
            {
                _memory.Remove(_lockKey);
            }
        }
    }
}
