using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 
    /// </summary>
    [ProtoBuf.ProtoContract(Name = @"PflQryEntrust_Response")]
    public class StockQryEntrustResponse
    {
        [ProtoBuf.ProtoMember(2)]
        public List<Entrust> entrust { get; set; } = new List<Entrust>();

        /// <summary>
        /// 总记录数：
        /// </summary>
        [ProtoBuf.ProtoMember(3, Name = @"totalnum")]
        [JsonProperty("totalnum")]
        public uint Totalnum { get; set; }

        [ProtoBuf.ProtoContract()]
        public class Entrust
        {
            /// <summary>
            /// 序号
            /// </summary>
            [ProtoBuf.ProtoMember(1, Name = @"idx")]
            [JsonProperty("idx")]
            public uint Idx { get; set; }

            /// <summary>
            /// 仓占比  *10000
            /// </summary>
            [ProtoBuf.ProtoMember(2, Name = @"ownratio")]
            [JsonProperty("ownratio")]
            public int Ownratio { get; set; }

            /// <summary>
            /// 委托时间，格式：2017-05-25 16:38:59.087
            /// </summary>
            [ProtoBuf.ProtoMember(3, Name = @"entrusttime")]
            [JsonProperty("entrusttime")]
            [System.ComponentModel.DefaultValue("")]
            public string Entrusttime { get; set; } = "";

            /// <summary>
            /// 市场代码：1:深圳；2:上海
            /// </summary>
            [ProtoBuf.ProtoMember(4, Name = @"market")]
            [JsonProperty("market")]
            public uint Market { get; set; }

            /// <summary>
            /// 证券代码：
            /// </summary>
            [ProtoBuf.ProtoMember(5, Name = @"secucode")]
            [JsonProperty("secucode")]
            [System.ComponentModel.DefaultValue("")]
            public string Secucode { get; set; } = "";

            /// <summary>
            /// 证券名称
            /// </summary>
            [ProtoBuf.ProtoMember(6, Name = @"secuname")]
            [JsonProperty("secuname")]
            [System.ComponentModel.DefaultValue("")]
            public string Secuname { get; set; } = "";

            /// <summary>
            /// 委托价格  	*10000
            /// </summary>
            [ProtoBuf.ProtoMember(7, Name = @"entrustprice")]
            [JsonProperty("entrustprice")]
            public uint Entrustprice { get; set; }

            /// <summary>
            /// 成交价格		*10000
            /// </summary>
            [ProtoBuf.ProtoMember(8, Name = @"dealprice")]
            [JsonProperty("dealprice")]
            public uint Dealprice { get; set; }

            /// <summary>
            /// 原来仓位  	*10000
            /// </summary>
            [ProtoBuf.ProtoMember(9, Name = @"stkpospre")]
            [JsonProperty("stkpospre")]
            public uint Stkpospre { get; set; }

            /// <summary>
            /// 目标仓位  	*10000
            /// </summary>
            [ProtoBuf.ProtoMember(10, Name = @"stkposdst")]
            [JsonProperty("stkposdst")]
            public uint Stkposdst { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [ProtoBuf.ProtoMember(11, Name = @"entrust_no")]
            [JsonProperty("entrustNo")]
            public ulong EntrustNo { get; set; }

            /// <summary>
            /// 状态码，0全部成交(成交价&gt;0)，1全部撤单，2已报[未成交](可撤)，3部分成交，4部成部撤，10未报(废弃，现在2表示可撤单)，11废单，12已报[未成交](不可撤)，99未知
            /// </summary>
            [ProtoBuf.ProtoMember(12, Name = @"statuscode")]
            [JsonProperty("statuscode")]
            public int Statuscode { get; set; }

            /// <summary>
            /// 状态信息(上述statuscode的中文字)
            /// </summary>
            [ProtoBuf.ProtoMember(13, Name = @"statusmsg")]
            [JsonProperty("statusmsg")]
            [System.ComponentModel.DefaultValue("")]
            public string Statusmsg { get; set; } = "";

            /// <summary>
            /// 限价或市价，：0限价,20市价
            /// </summary>
            [ProtoBuf.ProtoMember(14, Name = @"entflag")]
            [JsonProperty("entflag")]
            public int Entflag { get; set; }

            /// <summary>
            /// 限价或市价，如：限价
            /// </summary>
            [ProtoBuf.ProtoMember(15, Name = @"entmsg")]
            [JsonProperty("entmsg")]
            [System.ComponentModel.DefaultValue("")]
            public string Entmsg { get; set; } = "";

            /// <summary>
            /// G建仓,A加仓,D减仓,E平仓,W撤单,F送红股,P派息,T配股
            /// </summary>
            [ProtoBuf.ProtoMember(16, Name = @"busiflag")]
            [JsonProperty("busiflag")]
            [System.ComponentModel.DefaultValue("")]
            public string Busiflag { get; set; } = "";

            /// <summary>
            /// 建仓,加仓,减仓,平仓,撤单,送红股,派息,配股
            /// </summary>
            [ProtoBuf.ProtoMember(17, Name = @"busimsg")]
            [JsonProperty("busimsg")]
            [System.ComponentModel.DefaultValue("")]
            public string Busimsg { get; set; } = "";

            /// <summary>
            /// 操作说明
            /// </summary>
            [ProtoBuf.ProtoMember(18, Name = @"remark")]
            [JsonProperty("remark")]
            [System.ComponentModel.DefaultValue("")]
            public string Remark { get; set; } = "";

            /// <summary>
            /// 成交时间，格式：2017-05-25 16:38:59.087
            /// </summary>
            [ProtoBuf.ProtoMember(19, Name = @"dealtime")]
            [JsonProperty("dealtime")]
            [System.ComponentModel.DefaultValue("")]
            public string Dealtime { get; set; } = "";

            /// <summary>
            /// 盈亏 *1000 一千		2018.03.08 add
            /// </summary>
            [ProtoBuf.ProtoMember(20, Name = @"totalpl", DataFormat = ProtoBuf.DataFormat.ZigZag)]
            [JsonProperty("totalpl")]
            public long Totalpl { get; set; }

            /// <summary>
            /// 盈亏比例  *10000		2018.03.08 add
            /// </summary>
            [ProtoBuf.ProtoMember(21, Name = @"plscale", DataFormat = ProtoBuf.DataFormat.ZigZag)]
            [JsonProperty("plscale")]
            public int Plscale { get; set; }

            /// <summary>
            /// 带市场股票代码 （和market、secucode 二选一）
            /// </summary>
            [ProtoBuf.ProtoMember(22, Name = @"stockcode")]
            [JsonProperty("stockcode")]
            public uint Stockcode { get; set; }

            /// <summary>
            /// 成交数量 (股)
            /// </summary>
            [ProtoBuf.ProtoMember(23, Name = @"dealamt")]
            [JsonProperty("dealamt")]
            public uint Dealamt { get; set; }

            /// <summary>
            /// 资金发生数（包含税费）*1000 一千
            /// </summary>
            [ProtoBuf.ProtoMember(24, Name = @"trademoney", DataFormat = ProtoBuf.DataFormat.ZigZag)]
            [JsonProperty("trademoney")]
            public long Trademoney { get; set; }

            /// <summary>
            /// 锦囊id
            /// </summary>
            [ProtoBuf.ProtoMember(25, Name = @"prodid", DataFormat = ProtoBuf.DataFormat.FixedSize)]
            [JsonProperty("prodid")]
            public uint Prodid { get; set; }

            /// <summary>
            /// 产品名称，如果app请求(prodid=0)返回&quot;&quot;, 如果管理后台查询返回有效值
            /// </summary>
            [ProtoBuf.ProtoMember(26, Name = @"prodtitle")]
            [JsonProperty("prodtitle")]
            [System.ComponentModel.DefaultValue("")]
            public string Prodtitle { get; set; } = "";

            /// <summary>
            /// 大咖昵称，如果app请求(prodid=0)返回&quot;&quot;, 如果管理后台查询返回有效值
            /// </summary>
            [ProtoBuf.ProtoMember(27, Name = @"authorname")]
            [JsonProperty("authorname")]
            [System.ComponentModel.DefaultValue("")]
            public string Authorname { get; set; } = "";

            /// <summary>
            /// 委托数量 (股)
            /// </summary>
            [ProtoBuf.ProtoMember(28, Name = @"entrustamt")]
            [JsonProperty("entrustamt")]
            public uint Entrustamt { get; set; }

            /// <summary>
            /// 撤单数量 (股)
            /// </summary>
            [ProtoBuf.ProtoMember(29, Name = @"cancelamt")]
            [JsonProperty("cancelamt")]
            public uint Cancelamt { get; set; }
        }
    }
}
