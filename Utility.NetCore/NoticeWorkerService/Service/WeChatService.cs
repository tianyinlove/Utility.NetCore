using NoticeWorkerService.Core;
using NoticeWorkerService.Model;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utility.Extensions;

namespace NoticeWorkerService.Service
{
    /// <summary>
    /// 
    /// </summary>
    class WeChatService : IWeChatService
    {
        private IHttpClientFactory _httpClientFactory;
        private IMemoryCache _memoryCache;
        private string getTokenUrl = "https://qyapi.weixin.qq.com/cgi-bin/gettoken";
        private string sendUrl = "https://qyapi.weixin.qq.com/cgi-bin/message/send";
        private int _agentId = 1000002;
        private string _corpId = "ww1c5ca8f9af6164f4";
        private string _corpSecret = "0KpZS4ri3HuQOeiu0niga_peKXBp1--aTrviaTT8Z54";
        /// <summary>
        /// 
        /// </summary>
        public WeChatService(IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        async Task<string> GetToken()
        {
            string cacheKey = "Emapp:AlertNotice:Token";
            var result = _memoryCache.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(result))
            {
                var apiUrl = $"{getTokenUrl}?corpid={_corpId}&corpsecret={_corpSecret}";
                var response = await _httpClientFactory.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, apiUrl));
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(data))
                {
                    var responseData = data.FromJson<WechatResponse>();
                    if (responseData.ErrCode != 0)
                    {
                        throw new Exception(responseData.ErrMsg);
                    }
                    result = responseData.Access_Token;
                    _memoryCache.Set(cacheKey, result, TimeSpan.FromSeconds(responseData.Expires_In - 10));
                }
            }
            return result;
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <param name="request">通知对象</param>
        /// <returns></returns>
        public async Task<bool> Notice(WechatRequest request)
        {
            if (string.IsNullOrEmpty(request.ToUser) && string.IsNullOrEmpty(request.ToTag) && string.IsNullOrEmpty(request.ToParty))
            {
                throw new Exception("通知用户不能为空");
            }
            if (request.News == null && request.Image == null && request.File == null && request.Text == null && request.TextCard == null && request.Video == null && request.Voice == null)
            {
                throw new Exception("通知内容不能为空");
            }
            var token = await GetToken();
            var apiUrl = $"{sendUrl}?access_token={token}";
            request.AgentId = _agentId;
            var jsonData = request.ToJson(NullValueHandling.Ignore);
            var requestData = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };
            var response = await _httpClientFactory.CreateClient().SendAsync(requestData);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(data))
            {
                var responseData = data.FromJson<WechatResponse>();
                if (responseData.ErrCode != 0)
                {
                    throw new Exception(responseData.ErrMsg);
                }
            }
            return true;
        }
    }
}
