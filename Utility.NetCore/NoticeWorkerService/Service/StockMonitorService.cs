using NoticeWorkerService.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utility.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Utility.NetLog;
using Utility.Constants;
using NoticeWorkerService.Core;
using Microsoft.Extensions.Options;

namespace NoticeWorkerService.Service
{
    /// <summary>
    /// 
    /// </summary>
    class StockMonitorService : IStockMonitorService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IWeChatService _weChatService;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// 
        /// </summary>
        public StockMonitorService(IMemoryCache memoryCache,
            IWeChatService weChatService,
            IOptions<AppSettings> options,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _weChatService = weChatService;
            _appSettings = options.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SendMessage()
        {
            Logger.WriteLog(LogLevel.Info, "开始执行服务", _appSettings);

            var w = DateTime.Now.DayOfWeek;
            var h = DateTime.Now.Hour;
            if (w == DayOfWeek.Saturday || w == DayOfWeek.Sunday || h < _appSettings.StartHour || h > _appSettings.EndHour)
            {
                return;
            }

            if (_appSettings.NameList == null || _appSettings.NameList.Count <= 0)
            {
                return;
            }
            foreach (var item in _appSettings.NameList)
            {
                var message = await GetStockInfo1(item);

                Logger.WriteLog(LogLevel.Info, "读取消息", new { name = item, message });

                if (!string.IsNullOrEmpty(message))
                {
                    var request = new WechatRequest()
                    {
                        ToTag = "1",
                        MsgType = NoticeType.text.ToString(),
                        Text = new NoticeText { Content = $"{item}:{message}" }
                    };

                    try
                    {
                        await _weChatService.Notice(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(LogLevel.Error, "发送微信消息通知异常", request, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        async Task<string> GetStockInfo2(string name)
        {
            var result = "";
            var apiUrl = $"http://api.m.emoney.cn/servicecenter/EmappSmartInvestment/StrategyPoolHit/GetStockTradeListById?apikey=zhimakaimen";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("emapp-apikey", "zhimakaimen");
                request.Content = new StringContent((new
                {
                    name = name
                }).ToJson(), Encoding.UTF8, "application/json");
                var response = await _httpClientFactory.CreateClient().SendAsync(request);
                var jsonStr = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    var data = jsonStr.FromJson<ResponseData<List<StockTradeInfo>>>();
                    if (data != null && data.Detail != null && data.Detail.Count > 0)
                    {
                        var cacheKey = $"stocktrade:time:{name.Md5()}";
                        var time = _memoryCache.Get<DateTime>(cacheKey);
                        if (time < DateTime.Today)
                        {
                            time = DateTime.Today;
                        }
                        var list = data.Detail.Where(x => Convert.ToDateTime(x.TradeTime) > time).ToList();
                        if (list != null && list.Count > 0)
                        {
                            list.ForEach(item =>
                            {
                                result += $"{item.TradeTime} {item.TradeTypeName}({item.Busimsg}) {item.Secuname}({item.StockCode})，成交价：{item.DealPrice}元，成交{item.DealAmount}股;";
                            });
                        }
                        time = data.Detail.Max(x => Convert.ToDateTime(x.TradeTime));
                        _memoryCache.Set(cacheKey, time, TimeSpan.FromDays(1));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Utility.Constants.LogLevel.Error, "读取好股数据异常", apiUrl, ex);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        async Task<string> GetStockInfo1(string name)
        {
            var result = "";
            var apiUrl = $"https://yktapi.emoney.cn/JinNang/BackData/GetTradePage";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("emapp-apikey", "zhimakaimen");
                request.Content = new StringContent((new
                {
                    beginTme = DateTime.Today.ToString("yyyyMMdd"),
                    endTime = DateTime.Today.ToString("yyyyMMdd"),
                    jinNangName = name,
                    pageIndex = 0,
                    pageSize = 50
                }).ToJson(), Encoding.UTF8, "application/json");
                var response = await _httpClientFactory.CreateClient().SendAsync(request);
                var jsonStr = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    var data = jsonStr.FromJson<ResponseData<StockResponse>>();
                    if (data != null && data.Detail != null && data.Detail.List != null && data.Detail.List.Count > 0)
                    {
                        var cacheKey = $"stocktrade:time:{name.Md5()}";
                        var time = _memoryCache.Get<DateTime>(cacheKey);
                        if (time < DateTime.Today)
                        {
                            time = DateTime.Today;
                        }
                        var list = data.Detail.List.Where(x => x.TradeTime > time).ToList();
                        if (list != null && list.Count > 0)
                        {
                            list.ForEach(item =>
                            {
                                result += $"{item.TradeTime.ToString("yyyy-MM-dd HH:mm:ss")} {item.TradeTypeName}({item.Busimsg}) {item.Secuname}({item.StockCode})，成交价：{item.DealPrice}元，成交{item.DealAmount}股;";
                            });
                        }
                        time = data.Detail.List.Max(x => x.TradeTime);
                        _memoryCache.Set(cacheKey, time, TimeSpan.FromDays(1));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogLevel.Error, "读取好股数据异常", apiUrl, ex);
            }
            return result;
        }
    }
}
