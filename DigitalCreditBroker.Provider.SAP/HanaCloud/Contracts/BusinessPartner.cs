using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalCreditBroker.Provider.SAP.HanaCloud.Contracts
{
    public partial class BusinessPartner
    {
        [JsonProperty("BusinessPartner")]
        public string Id { get; set; }

        [JsonProperty("Customer")]
        public string Customer { get; set; }

        [JsonProperty("BusinessPartnerFullName")]
        public string BusinessPartnerFullName { get; set; }

        [JsonProperty("BusinessPartnerName")]
        public string BusinessPartnerName { get; set; }

        [JsonProperty("to_BusinessPartnerAddress")]
        public ToAddress ToBusinessPartnerAddress { get; set; }

        [JsonProperty("to_BusinessPartnerTax")]
        public ToBusinessPartnerTax ToBusinessPartnerTax { get; set; }
    }

    public partial class ToAddress
    {
        [JsonProperty("results")]
        public List<ToBusinessPartnerAddressResult> Results { get; set; }
    }

    public partial class ToBusinessPartnerAddressResult
    {
        [JsonProperty("AddressID")]
        public string AddressId { get; set; }


        [JsonProperty("Country")]
        public string Country { get; set; }

    }

    public partial class ToBusinessPartnerTax
    {
        [JsonProperty("results")]
        public List<ToBusinessPartnerTaxResult> Results { get; set; }
    }

    public partial class ToBusinessPartnerTaxResult
    {
        [JsonProperty("BPTaxNumber")]
        public string BpTaxNumber { get; set; }
    }
}
