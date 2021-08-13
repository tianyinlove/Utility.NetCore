using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class WechatRequest
    {
        /// <summary>
        /// 接收成员ID，多个用|分隔，最多支持1000个
        /// </summary>
        [JsonProperty("touser")]
        public string ToUser { get; set; }
        /// <summary>
        /// 接收部门ID，多个用|分隔，最多支持100个
        /// </summary>
        [JsonProperty("toparty")]
        public string ToParty { get; set; }
        /// <summary>
        /// 接收标签ID，多个用|分隔，最多支持100个
        /// </summary>
        [JsonProperty("totag")]
        public string ToTag { get; set; }
        /// <summary>
        /// 企业微信ID
        /// </summary>
        [JsonProperty("agentid")]
        public int AgentId { get; set; } = 1000002;
        /// <summary>
        /// 消息类型(默认文本:text)
        /// </summary>
        [JsonProperty("msgtype")]
        public string MsgType { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        [JsonProperty("text")]
        public NoticeText Text { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [JsonProperty("image")]
        public NoticeMedia Image { get; set; }
        /// <summary>
        /// 语音
        /// </summary>
        [JsonProperty("voice")]
        public NoticeMedia Voice { get; set; }
        /// <summary>
        /// 视频
        /// </summary>
        [JsonProperty("video")]
        public NoticeVideo Video { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        [JsonProperty("file")]
        public NoticeMedia File { get; set; }
        /// <summary>
        /// 文本卡片
        /// </summary>
        [JsonProperty("textcard")]
        public NoticeTextCard TextCard { get; set; }
        /// <summary>
        /// 图文
        /// </summary>
        [JsonProperty("news")]
        public NoticeNews News { get; set; }
    }
}
