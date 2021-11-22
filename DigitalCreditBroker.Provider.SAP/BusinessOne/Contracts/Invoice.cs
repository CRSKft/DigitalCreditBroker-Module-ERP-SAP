using DigitalCreditBroker.Swagger;
using Newtonsoft.Json;
using System;

namespace DigitalCreditBroker.Provider.SAP.BusinessOne.Contracts
{
    public class Invoice
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string DocType { get; set; }
        public string DocEntry { get; set; }
        public string DocNum { get; set; }

        public double DocTotal { get; set; }
        public string DocCurrency { get; set; }
        public string DocumentStatus { get; set; }

        public string Confirmed { get; set; }

        public DateTime? ClosingDate { get; set; }
    }
}
