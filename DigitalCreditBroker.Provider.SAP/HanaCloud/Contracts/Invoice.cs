using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalCreditBroker.Provider.SAP.HanaCloud.Contracts
{
    public class Invoice
    {

        [JsonProperty("PostingDate")]
        public DateTime PostingDate { get; set; }

        [JsonProperty("BillingDocument")]
        public string BillingDocument { get; set; }

        [JsonProperty("NetDueDate")]
        public DateTime NetDueDate { get; set; }

        [JsonProperty("AmountInTransactionCurrency")]
        public double AmountInTransactionCurrency { get; set; }

        [JsonProperty("TransactionCurrency")]
        public string TransactionCurrency { get; set; }

        [JsonProperty("IsCleared")]
        public bool IsCleared { get; set; }

        [JsonProperty("ClearingDate")]
        public DateTime? ClearingDate { get; set; }
    }
}
