using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 文本卡片
    /// </summary>
    public class NoticeTextCard
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
        /// <summary>
        /// 跳转地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// 按钮文字
        /// </summary>
        [JsonProperty("btntxt")]
        public string BtnTxt { get; set; }
    }
}
