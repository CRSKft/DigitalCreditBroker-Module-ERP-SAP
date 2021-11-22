using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalCreditBroker.Provider.SAP.BusinessOne
{
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        [JsonProperty("value")]
        public List<T> Value { get; set; }

        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }
    }
}
