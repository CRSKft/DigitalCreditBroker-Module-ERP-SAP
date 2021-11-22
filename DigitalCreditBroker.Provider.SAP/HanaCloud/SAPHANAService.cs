using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using DigitalCreditBroker.Application;
using DigitalCreditBroker.Authorization.Users;
using DigitalCreditBroker.Company;
using DigitalCreditBroker.Company.Erp;
using DigitalCreditBroker.Provider.Erp;
using DigitalCreditBroker.Provider.Erp.Dto;
using DigitalCreditBroker.Provider.SAP.HanaCloud;
using DigitalCreditBroker.Provider.SAP.HanaCloud.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalCreditBroker.Provider.SAP.HanaCloud
{
    public class SAPHANAService : ErpBaseAppService
    {
        public SAPHANAService(IRepository<Application.Company, Guid> companyRepository, ICompanyPolicy companyPolicy, IObjectMapper objectMapper, IRepository<Application.FastCover, long> fastCoverRepository, UserManager userManager)
            : base(companyRepository, companyPolicy, objectMapper, fastCoverRepository, userManager) { }

        public override async Task<PaymentExperience> GetPaymentExperience(InvoiceInput input)
        {
            var sapClient = await GetODataClient(input.CompanyId);
            var filter = $"$filter=CompanyCode eq '{sapClient.CompanyCode}' and Customer eq '{input.BusinessPartnerId}' and AccountingDocumentType eq 'RV'";

            List<Invoice> invoices = await sapClient.GetAll<Invoice>($"API_OPLACCTGDOCITEMCUBE_SRV/A_OperationalAcctgDocItemCube?{filter}&$select=BillingDocument,PostingDate,NetDueDate,AmountInTransactionCurrency,TransactionCurrency,ClearingDate");

            var mappedInvoices = _mapper.Map<List<InvoiceDto>>(invoices);

            PaymentExperience result = CalculateBusinessPartnerPaymentExperience(mappedInvoices);

            return result;
        }

        public override async Task<bool> IsInvoiceSettled(InvoiceSettleRequest input)
        {
            var sapClient = await GetODataClient(input.CompanyId);
            var filter = $"$filter=BillingDocument eq '{input.InvoiceNumber}' and AccountingDocumentType eq 'RV'";
            var invoice = (await sapClient.GetAll<Invoice>($"API_OPLACCTGDOCITEMCUBE_SRV/A_OperationalAcctgDocItemCube?{filter}&$select=IsCleared")).Single();

            return invoice.IsCleared;
        }

        protected override async Task<List<InvoiceDto>> GetOpenInvoices(InvoiceInput input)
        {
            var sapClient = await GetODataClient(input.CompanyId);
            var filter = $"$filter=CompanyCode eq '{sapClient.CompanyCode}' and Customer eq '{input.BusinessPartnerId}' and IsCleared eq false and AccountingDocumentType eq 'RV'";

            List<Invoice> invoices = await sapClient.GetAll<Invoice>($"API_OPLACCTGDOCITEMCUBE_SRV/A_OperationalAcctgDocItemCube?{filter}&$select=BillingDocument,PostingDate,NetDueDate,AmountInTransactionCurrency,TransactionCurrency,ClearingDate");
            return _mapper.Map<List<InvoiceDto>>(invoices);
        }

        protected override async Task<List<BusinessPartnerDto>> GetAllBusinessPartners(BusinessPartnerInput input)
        {
            var sapClient = await GetODataClient(input.CompanyId);

            var filters = new List<string>();
            var filter = "$filter=Customer ne ''";

            //if (!string.IsNullOrEmpty(input.VAT))
            //    filters.Add($"VATRegistrationNumber eq '{input.VAT}'");

            if (!string.IsNullOrEmpty(input.Name))
                filters.Add($"contains(CustomerName, '{input.Name}')"); // a tolower() nem megy ebben a retekben, szóval csak case sensitve-en jó

            if (!string.IsNullOrEmpty(input.Id))
                filters.Add($"Customer eq '{input.Id}'");

            //if (!string.IsNullOrEmpty(input.Country))
            //    filters.Add($"Country eq '{input.Country}'");

            if (filters.Any())
            {
                filter += string.Join(" and ", filters);
            }
            filter += "&";

            List<BusinessPartner> businessPartners = await sapClient.GetAll<BusinessPartner>($"API_BUSINESS_PARTNER/A_BusinessPartner?{filter}$select=to_BusinessPartnerAddress,to_BusinessPartnerTax,BusinessPartner,Customer,BusinessPartnerFullName&$expand=to_BusinessPartnerAddress,to_BusinessPartnerTax");

            return _mapper.Map<List<BusinessPartnerDto>>(businessPartners);
        }


        private async Task<ODataClient> GetODataClient(Guid companyId)
        {
            var company = await GetCompany(companyId);
            var erpSettings = JsonConvert.DeserializeObject<SapHanaConnectionString>(company.Settings.ErpSettings);
            return new ODataClient(erpSettings.BaseUrl, erpSettings.ApiKey, erpSettings.CompanyCode);
        }

    }
}
