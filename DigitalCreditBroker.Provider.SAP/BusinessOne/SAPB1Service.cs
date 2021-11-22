using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using DigitalCreditBroker.Authorization.Users;
using DigitalCreditBroker.Company;
using DigitalCreditBroker.Company.Erp;
using DigitalCreditBroker.Provider.Erp;
using DigitalCreditBroker.Provider.Erp.Dto;
using DigitalCreditBroker.Provider.SAP.BusinessOne.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalCreditBroker.Provider.SAP.BusinessOne
{
    public class SAPB1Service : ErpBaseAppService
    {
        public SAPB1Service(IRepository<Application.Company, Guid> companyRepository, ICompanyPolicy companyPolicy, IObjectMapper objectMapper, IRepository<Application.FastCover, long> fastCoverRepository, UserManager userManager)
            : base(companyRepository, companyPolicy, objectMapper, fastCoverRepository, userManager) { }

        public override async Task<PaymentExperience> GetPaymentExperience(InvoiceInput input)
        {
            var sapClient = await GetODataClient(input.CompanyId);
            var filter = $"$filter = CardCode eq '{input.BusinessPartnerId}'";

            List<Invoice> invoices = await sapClient.GetAll<Invoice>($"Invoices?{filter}&$select=DocType,DocEntry,DocNum,DocDate,DocTotal,DocCurrency,DocDueDate,DocumentStatus,Confirmed,ClosingDate");

            var mappedInvoices = _mapper.Map<List<InvoiceDto>>(invoices);

            PaymentExperience result = CalculateBusinessPartnerPaymentExperience(mappedInvoices);

            return result;
        }

        public override async Task<bool> IsInvoiceSettled(InvoiceSettleRequest input)
        {
            var sapClient = await GetODataClient(input.CompanyId);
            var invoice = await sapClient.Get<Invoice>("Invoices", input.InvoiceNumber);
            return invoice.DocumentStatus == "bost_Close";
        }

        protected override async Task<List<InvoiceDto>> GetOpenInvoices(InvoiceInput input)
        {
            var sapClient = await GetODataClient(input.CompanyId);
            var filter = $"$filter = CardCode eq '{input.BusinessPartnerId}' and DocumentStatus eq 'bost_Open'";

            List<Invoice> invoices = await sapClient.GetAll<Invoice>($"Invoices?{filter}&$select=DocType,DocEntry,DocNum,DocDate,DocTotal,DocCurrency,DocDueDate,DocumentStatus,Confirmed");
            return _mapper.Map<List<InvoiceDto>>(invoices);
        }

        protected override async Task<List<BusinessPartnerDto>> GetAllBusinessPartners(BusinessPartnerInput input)
        {
            var sapClient = await GetODataClient(input.CompanyId);

            var filters = new List<string>();
            var filter = string.Empty;

            if (!string.IsNullOrEmpty(input.VAT))
                filters.Add($"VATRegistrationNumber eq '{input.VAT}'");

            if (!string.IsNullOrEmpty(input.Name))
                filters.Add($"contains(CardName, '{input.Name}')"); // a tolower() nem megy ebben a retekben, szóval csak case sensitve-en jó

            if (!string.IsNullOrEmpty(input.Id))
                filters.Add($"CardCode eq '{input.Id}'");

            if (!string.IsNullOrEmpty(input.Country))
                filters.Add($"Country eq '{input.Country}'");

            if (filters.Any())
            {
                filter = "$filter=";
                filter += string.Join(" and ", filters);
                filter += "&";
            }

            List<BusinessPartner> businessPartners = await sapClient.GetAll<BusinessPartner>($"BusinessPartners?{filter}$select=CardCode,CardName,VATRegistrationNumber,Country");

            return _mapper.Map<List<BusinessPartnerDto>>(businessPartners);
        }

        private async Task<ODataClient> GetODataClient(Guid companyId)
        {
            var company = await GetCompany(companyId);
            var erpSettings = JsonConvert.DeserializeObject<SapB1ConnectionString>(company.Settings.ErpSettings);
            return new ODataClient(erpSettings.BaseUrl, erpSettings.ApiKey);
        }
    }
}
