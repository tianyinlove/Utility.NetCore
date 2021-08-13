using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 文本
    /// </summary>
    public class NoticeText
    {
        /// <summary>
        /// 通知内容文本(不超过2048个字节)
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
