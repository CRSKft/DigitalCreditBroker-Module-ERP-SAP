using DigitalCreditBroker.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalCreditBroker.Provider.SAP.BusinessOne.Contracts
{
    public class BusinessPartner
    {
        public string CardName { get; set; }
        public string CardCode { get; set; }
        public string VATRegistrationNumber { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
    }
}
