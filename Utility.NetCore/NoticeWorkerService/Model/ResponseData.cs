using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseData<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Detail { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StockResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<StockTradeInfo> List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StockTradeInfo
    {
        /// <summary>
        /// 锦囊名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 证券名称
        /// </summary>
        public string Secuname { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// 建仓,加仓,减仓,平仓,撤单,送红股,派息,配股---->买入，卖出
        /// </summary>
        public string TradeTypeName { get; set; }

        /// <summary>
        /// 建仓,加仓,减仓,平仓,撤单,送红股,派息,配股
        /// </summary>

        public string Busimsg { get; set; }

        /// <summary>
        /// 成交数量 (股)
        /// </summary>
        public long DealAmount { get; set; }

        /// <summary>
        /// 成交价
        /// </summary>
        public decimal DealPrice { get; set; }

        /// <summary>
        /// 成交占比
        /// </summary>
        public string DealPosition { get; set; }

        /// <summary>
        /// 大咖姓名
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int JinNangId { get; set; }
    }
}
