using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 视频
    /// </summary>
    public class NoticeVideo : NoticeMedia
    {
        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
