using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 图文
    /// </summary>
    public class NoticeNews
    {
        /// <summary>
        /// 图片消息(1-8条)
        /// </summary>
        [JsonProperty("articles")]
        public List<NoticeArticles> Articles { get; set; }
    }
}
