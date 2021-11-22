using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DigitalCreditBroker.Provider.SAP.HanaCloud
{
    public partial class ODataResponse<T>
    {
        [JsonProperty("d")]
        public D<T> D { get; set; }
    }

    public partial class D<T>
    {
        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }


}


