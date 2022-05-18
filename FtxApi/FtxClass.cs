using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi
{
    public class JBalanceResult
    {
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("error")]
        public string error { get; set; }
        [JsonProperty("result")]
        public JBalance[] result { get; set; }
    }

    public class JBalance
    {
        [JsonProperty("coin")]
        public string coin { get; set; }

        [JsonProperty("free")]
        public decimal free { get; set; }

        [JsonProperty("total")]
        public decimal total { get; set; }

    }
}
