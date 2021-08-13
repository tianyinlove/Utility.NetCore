using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 微信返回信息
    /// </summary>
    public class WechatResponse
    {
        /// <summary>
        /// 错误码(0成功)
        /// </summary>
        public int ErrCode { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Access_Token { get; set; }

        /// <summary>
        /// 过期时间(秒)
        /// </summary>
        public int Expires_In { get; set; }
    }
}
