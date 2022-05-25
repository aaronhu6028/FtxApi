using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi
{
    #region quote
    public class FtxTicker
    {
        public string Instrument;
        public DateTime Time;
        public decimal LastPrice;
    }
    #endregion

    #region Result
    public class FtxResult<T>
    {
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("error")]
        public string error { get; set; }
        [JsonProperty("result")]
        public T result { get; set; }
    }

    #endregion

    #region Balance
    public class FtxBalance
    {
        [JsonProperty("coin")]
        public string coin { get; set; }

        [JsonProperty("free")]
        public decimal free { get; set; }

        [JsonProperty("total")]
        public decimal total { get; set; }
    }
    #endregion

    #region Account
    public class FtxAccount
    {
        [JsonProperty("leverage")]
        public decimal leverage { get; set; }

        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("positions")]
        public FtxPosition[] positions { get; set; }
    }

    public class FtxPosition
    {
        [JsonProperty("future")]
        public string future { get; set; }

        [JsonProperty("side")]
        public string side { get; set; }

        [JsonProperty("size")]
        public decimal size { get; set; }

    }
    #endregion

    #region Order
    public class FtxOrder
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime createdAt { get; set; }

        [JsonProperty("future")]
        public string future { get; set; }

        [JsonProperty("side")]
        public string side { get; set; }

        [JsonProperty("size")]
        public decimal size { get; set; }
    }
    #endregion

    #region TradeRecord
    public class FtxTradeRecord
    {
        [JsonProperty("future")]
        public string future { get; set; }

        [JsonProperty("time")]
        public DateTime time { get; set; }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("orderId")]
        public long orderId { get; set; }

        [JsonProperty("tradeId")]
        public long tradeId { get; set; }

        [JsonProperty("side")]
        public string side { get; set; }

        [JsonProperty("price")]
        public decimal price { get; set; }

        [JsonProperty("size")]
        public decimal size { get; set; }

        [JsonProperty("fee")]
        public decimal fee { get; set; }
        [JsonProperty("feeRate")]
        public decimal feeRate { get; set; }
        [JsonProperty("feeCurrency")]
        public string feeCurrency { get; set; }
        [JsonProperty("liquidity")]
        public string liquidity { get; set; }
    }
    #endregion
}
