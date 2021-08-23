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
using NoticeWorkerService.Data;

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
        private readonly IApiClient _apiClient;

        /// <summary>
        /// 
        /// </summary>
        public StockMonitorService(IMemoryCache memoryCache,
            IWeChatService weChatService,
            IOptions<AppSettings> options,
            IApiClient apiClient,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _weChatService = weChatService;
            _appSettings = options.Value;
            _apiClient = apiClient;
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
                var message = await GetStockTradeInfo(item);

                Logger.WriteLog(LogLevel.Info, "读取消息", new { name = item, message });

                if (!string.IsNullOrEmpty(message))
                {
                    var request = new WechatRequest()
                    {
                        ToTag = _appSettings.ToTag,
                        ToUser = _appSettings.ToUser,
                        ToParty = _appSettings.ToParty,
                        MsgType = NoticeType.text.ToString(),
                        Text = new NoticeText { Content = $"{item}\n{message}" }
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
        async Task<string> GetStockTradeInfo(string name)
        {
            var result = "";
            try
            {
                var data = await _apiClient.GetStockTradeListByNameAsync(name);
                if (data != null && data.Count > 0)
                {
                    var cacheKey = $"stocktrade:time:{name.Md5()}";
                    var time = _memoryCache.Get<DateTime>(cacheKey);
                    if (time < DateTime.Today)
                    {
                        time = DateTime.Today;
                    }
                    var list = data.Where(x => x.TradeTime > time).ToList();
                    if (list != null && list.Count > 0)
                    {
                        list.ForEach(item =>
                        {
                            result += $"{item.TradeTime}\n{item.Busimsg}：{item.Secuname}({item.StockCode})\n委托价：{item.EntrustPrice}元({item.Entrustamt}股)，撤单{item.Cancelamt}股\n成交价：{item.DealPrice}元({item.DealAmount}股)\n原仓位：{item.Stkpospre}\n目标仓位：{item.Stkposdst}\n成交仓位：{item.DealPosition};\n\n";
                        });
                    }
                    time = data.Max(x => x.TradeTime);
                    _memoryCache.Set(cacheKey, time, TimeSpan.FromDays(1));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogLevel.Error, "读取好股数据异常", name, ex);
            }
            return result;
        }
    }
}
