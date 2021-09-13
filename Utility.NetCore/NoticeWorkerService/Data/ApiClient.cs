using NoticeWorkerService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utility.Constants;
using Utility.Model;
using Utility.NetLog;
using Utility.Extensions;
using Microsoft.Extensions.Options;
using NoticeWorkerService.Core;

namespace NoticeWorkerService.Data
{
    /// <summary>
    /// 
    /// </summary>
    class ApiClient : IApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// 
        /// </summary>
        public ApiClient(IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _appSettings = options.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<StockTradeInfo>> GetStockTradeListByNameAsync(string name)
        {
            var result = new List<StockTradeInfo>();
            try
            {
                var response = await ProtoBufInvokeAsync<StockQryEntrustResponse>(_appSettings.TradeUrl, 9400, new
                {
                    token = "",
                    prodid = 0,
                    market = 0,
                    secucode = "",
                    startdate = Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd")),
                    enddate = Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd")),
                    entrustNo = 0,
                    filterDeal = 0,
                    filterBS = 0,
                    pos = 0,
                    req = 20,
                    stockcode = 0,
                    zoneid = 2801,
                    prodtitle = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(name)),
                    authorname = ""
                });

                if (response.Result.Code != 0)
                {
                    Logger.WriteLog(LogLevel.Warning, "读取交易信息业务接口异常", new { name, response });
                    throw new Exception("读取交易信息业务接口异常");
                }
                if (response.Detail?.entrust != null)
                {
                    result = response.Detail?.entrust.Select(a =>
                    {
                        var result = new StockTradeInfo
                        {
                            StockCode = a.Secucode,
                            Secuname = a.Secuname,
                            TradeTime = string.IsNullOrEmpty(a.Dealtime) ? DateTime.Now : Convert.ToDateTime(a.Dealtime),
                            Busimsg = a.Busimsg,
                            DealAmount = a.Dealamt,
                            Entrustamt = a.Entrustamt,
                            Cancelamt = a.Cancelamt,
                            EntrustPrice = (decimal)(a.Entrustprice / 10000.0),
                            DealPrice = (decimal)(a.Dealprice / 10000.0),
                            DealPosition = (a.Ownratio / (double)10000).ToString("p2").Replace(",", ""),
                            Stkpospre = (a.Stkpospre / (double)10000).ToString("p2").Replace(",", ""),
                            Stkposdst = (a.Stkposdst / (double)10000).ToString("p2").Replace(",", "")
                        };
                        return result;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogLevel.Error, "读取交易信息异常", name, ex);
                throw new Exception("读取交易信息接口异常");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="protocolId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ApiData<T>> ProtoBufInvokeAsync<T>(string apiUrl, int? protocolId = null, object parameters = null)
        {
            var packageData = await ProtoBufInvokeAsync(apiUrl, protocolId, parameters);
            var apiData = new ApiData<T>();
            var result = apiData.Result as ApiStatus;
            result.Code = packageData.Result.Code;
            result.Msg = packageData.Result.Msg;
            if (packageData.Detail?.Value != null)
            {
                using var ms = new MemoryStream(packageData.Detail.Value);
                apiData.Detail = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            return apiData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocolId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<XResponse> ProtoBufInvokeAsync(string apiUrl, int? protocolId = null, object parameters = null)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ArgumentNullException(nameof(apiUrl));
            }
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            if (parameters != null)
            {
                request.Method = HttpMethod.Post;
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, parameters);
                request.Content = new ByteArrayContent(ms.ToArray());
            }
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/x-protobuf-v3");
            if (protocolId != null)
            {
                request.Headers.Add("X-Protocol-Id", protocolId.ToString());
                request.Headers.Add("X-Request-Id", Guid.NewGuid().ToString("N"));
            }
            using var response = await _httpClientFactory.CreateClient().SendAsync(request);
            response.EnsureSuccessStatusCode();
            using var contentStream = await response.Content.ReadAsStreamAsync();
            return ProtoBuf.Serializer.Deserialize<XResponse>(contentStream);
        }
    }
}
