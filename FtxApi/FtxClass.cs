using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi
{
    #region Balance
    public class FtxBalanceResult
    {
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("error")]
        public string error { get; set; }
        [JsonProperty("result")]
        public FtxBalance[] result { get; set; }
    }

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
    public class FtxAccountResult
    {
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("error")]
        public string error { get; set; }
        [JsonProperty("result")]
        public FtxAccount result { get; set; }
    }

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
}
