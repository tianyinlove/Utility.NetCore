﻿
#if NETFRAMEWORK
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
namespace Utility.Http
{
    using EventQueue = ConcurrentQueue<Tuple<SendOrPostCallback, object>>;
    using EventTask = Tuple<SendOrPostCallback, object>;

    /// <summary>
    /// A Helper class to run Asynchronous functions from synchronous ones
    /// https://github.com/tejacques/AsyncBridge/blob/master/src/AsyncBridge/AsyncHelper.cs
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// 同步调用异步方法并避免死锁，https://github.com/tejacques/AsyncBridge
        /// </summary>
        /// <param name="task"></param>
        public static void RunSync(this Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var currentContext = new ExclusiveSynchronizationContext(oldContext);
            SynchronizationContext.SetSynchronizationContext(currentContext);

            try
            {
                currentContext.Post(async _ =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        currentContext.InnerException = e;
                    }
                    finally
                    {
                        currentContext.EndMessageLoop();
                    }
                }, null);

                currentContext.BeginMessageLoop();
            }
            catch (AggregateException e)
            {
                throw e?.InnerException ?? e;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        /// <summary>
        /// 同步调用异步方法并避免死锁，https://github.com/tejacques/AsyncBridge
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T RunSync<T>(this Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var currentContext = new ExclusiveSynchronizationContext(oldContext);
            SynchronizationContext.SetSynchronizationContext(currentContext);
            T result = default(T);
            try
            {
                currentContext.Post(async _ =>
                {
                    try
                    {
                        result = await task();
                    }
                    catch (Exception e)
                    {
                        currentContext.InnerException = e;
                    }
                    finally
                    {
                        currentContext.EndMessageLoop();
                    }
                }, null);

                currentContext.BeginMessageLoop();
            }
            catch (AggregateException e)
            {
                throw e?.InnerException ?? e;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
            return result;
        }

        /// <summary>
        /// A class to bridge synchronous asynchronous methods
        /// </summary>
        public class AsyncBridge : IDisposable
        {
            private ExclusiveSynchronizationContext CurrentContext;
            private SynchronizationContext OldContext;
            private int TaskCount;

            /// <summary>
            /// Constructs the AsyncBridge by capturing the current
            /// SynchronizationContext and replacing it with a new
            /// ExclusiveSynchronizationContext.
            /// </summary>
            internal AsyncBridge()
            {
                OldContext = SynchronizationContext.Current;
                CurrentContext =
                    new ExclusiveSynchronizationContext(OldContext);
                SynchronizationContext
                    .SetSynchronizationContext(CurrentContext);
            }

            /// <summary>
            /// Execute's an async task with a void return type
            /// from a synchronous context
            /// </summary>
            /// <param name="task">Task to execute</param>
            /// <param name="callback">Optional callback</param>
            public void Run(Task task, Action<Task> callback = null)
            {
                CurrentContext.Post(async _ =>
                {
                    try
                    {
                        Increment();
                        await task;

                        if (null != callback)
                        {
                            callback(task);
                        }
                    }
                    catch (Exception e)
                    {
                        CurrentContext.InnerException = e;
                    }
                    finally
                    {
                        Decrement();
                    }
                }, null);
            }

            /// <summary>
            /// Execute's an async task with a T return type
            /// from a synchronous context
            /// </summary>
            /// <typeparam name="T">The type of the task</typeparam>
            /// <param name="task">Task to execute</param>
            /// <param name="callback">Optional callback</param>
            public void Run<T>(Task<T> task, Action<Task<T>> callback = null)
            {
                if (null != callback)
                {
                    Run((Task)task, (finishedTask) =>
                        callback((Task<T>)finishedTask));
                }
                else
                {
                    Run((Task)task);
                }
            }

            /// <summary>
            /// Execute's an async task with a T return type
            /// from a synchronous context
            /// </summary>
            /// <typeparam name="T">The type of the task</typeparam>
            /// <param name="task">Task to execute</param>
            /// <param name="callback">
            /// The callback function that uses the result of the task
            /// </param>
            public void Run<T>(Task<T> task, Action<T> callback)
            {
                Run(task, (t) => callback(t.Result));
            }

            private void Increment()
            {
                Interlocked.Increment(ref TaskCount);
            }

            private void Decrement()
            {
                Interlocked.Decrement(ref TaskCount);
                if (TaskCount == 0)
                {
                    CurrentContext.EndMessageLoop();
                }
            }

            /// <summary>
            /// Disposes the object
            /// </summary>
            public void Dispose()
            {
                try
                {
                    CurrentContext.BeginMessageLoop();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    SynchronizationContext
                        .SetSynchronizationContext(OldContext);
                }
            }
        }

        /// <summary>
        /// Creates a new AsyncBridge. This should always be used in
        /// conjunction with the using statement, to ensure it is disposed
        /// </summary>
        public static AsyncBridge Wait
        {
            get { return new AsyncBridge(); }
        }

        /// <summary>
        /// Runs a task with the "Fire and Forget" pattern using Task.Run,
        /// and unwraps and handles exceptions
        /// </summary>
        /// <param name="task">A function that returns the task to run</param>
        /// <param name="handle">Error handling action, null by default</param>
        public static void FireAndForget(
            Func<Task> task,
            Action<Exception> handle = null)
        {
            Task.Run(
            () =>
            {
                ((Func<Task>)(async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        handle?.Invoke(e);
                    }
                }))();
            });
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);

            private bool _done;
            private EventQueue _items;

            public Exception InnerException { get; set; }

            public ExclusiveSynchronizationContext(SynchronizationContext old)
            {
                //if (old is ExclusiveSynchronizationContext oldEx)
                //{
                //    _items = oldEx._items;
                //}
                //else
                {
                    _items = new EventQueue();
                }
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException(
                    "We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _items.Enqueue(Tuple.Create(d, state));
                _workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => _done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!_done)
                {
                    if (_items.TryDequeue(out EventTask task))
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // method threw an exeption
                        {
                            throw new AggregateException(
                                "AsyncBridge.Run method threw an exception.",
                                InnerException);
                        }
                    }
                    else
                    {
                        _workItemsWaiting.WaitOne(200);
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
#endif
