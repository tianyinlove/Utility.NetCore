using System;
using System.Collections.Generic;
using System.Text;
using Utility.Constants;

namespace Utility.Configuration
{
    /// <summary>
    /// 日志设置
    /// </summary>
    internal class LogConfig
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 文本日志
        /// </summary>
        public TextOptions Text { get; set; } = new TextOptions();

        /// <summary>
        /// 阿里云日志上传
        /// </summary>
        public AliyunOptions Aliyun { get; set; } = new AliyunOptions();
    }

    /// <summary>
    /// 文本日志配置
    /// </summary>
    class TextOptions
    {
        /// <summary>
        /// 文件日志最小级别
        /// </summary>
        public LogLevel MinLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// 文件保存目录
        /// </summary>
        public string SavePath { get; set; } = "/App_Data/Log/";

        /// <summary>
        /// 每个类型错误目录下最大保留的文件数
        /// </summary>
        public int MaxFileCount { get; set; } = 30;

        /// <summary>
        /// 每天的日志文件数
        /// </summary>
        public int MaxLoopCount { get; set; } = 10;

        /// <summary>
        /// 每个文件大小上限
        /// </summary>
        public int MaxFileSize { get; set; } = 2 << 20;
    }

    /// <summary>
    /// 阿里云日志服务
    /// </summary>
    class AliyunOptions
    {
        /// <summary>
        /// 上传日志级别
        /// </summary>
        public LogLevel MinLevel { get; set; } = LogLevel.None;

        /// <summary>
        /// 服务入口
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// 项目名
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LogStoreName { get; set; }

        /// <summary>
        /// 访问密钥信息
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// 访问密钥信息
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// 设置是否使用代理
        /// </summary>
        public bool UseProxy { get; set; } = false;
    }
}
