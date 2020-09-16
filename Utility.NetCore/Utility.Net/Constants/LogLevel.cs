using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Constants
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 跟踪,有关通常仅用于调试的信息。 这些消息可能包含敏感应用程序数据，因此不得在生产环境中启用它们。 默认情况下禁用。
        /// </summary>
        Trace = 0,

        /// <summary>
        /// 调试,有关在开发和调试中可能有用的信息。  由于日志数量过多，因此仅当执行故障排除时，才在生产中启用 Debug 级别日志。
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 信息记录,用于跟踪应用的常规流。 这些日志通常有长期价值。
        /// </summary>
        Info = 2,

        /// <summary>
        /// 警示，需要关注的程序异常。表示应用流中的异常或意外事件。 可能包括不会中断应用运行但仍需调查的错误或其他条件。 Warning 日志级别常用于已处理的异常。
        /// </summary>
        Warning = 3,

        /// <summary>
        /// 错误，应该要优化并修复的异常。表示无法处理的错误和异常。 这些消息指示的是当前活动或操作（例如当前 HTTP 请求）中的失败，而不是整个应用中的失败。
        /// </summary>
        Error = 4,

        /// <summary>
        /// 致命错误，表示服务已经出现了某种程度的不可用，需要立即处理。需要立即关注的失败。 例如数据丢失、磁盘空间不足。
        /// </summary>
        Critical = 5,

        /// <summary>
        /// 不处理日志
        /// </summary>
        None = 6
    }
}
