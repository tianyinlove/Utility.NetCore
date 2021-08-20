using NoticeWorkerService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utility.Model;

namespace NoticeWorkerService.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<List<StockTradeInfo>> GetStockTradeListByNameAsync(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="protocolId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<ApiData<T>> ProtoBufInvokeAsync<T>(string apiUrl, int? protocolId = null, object parameters = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocolId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<XResponse> ProtoBufInvokeAsync(string apiUrl, int? protocolId = null, object parameters = null);
    }
}
