using AutoMapper;
using DigitalCreditBroker.Provider.Erp.Dto;
using DigitalCreditBroker.Provider.SAP.BusinessOne.Contracts;

namespace DigitalCreditBroker.Provider.SAP
{
	public class CurrencyResolver : IValueResolver<BusinessOne.Contracts.BusinessPartner, BusinessPartnerDto, string>, IValueResolver<Invoice, InvoiceDto, string>
	{
		public string Resolve(BusinessOne.Contracts.BusinessPartner source, BusinessPartnerDto destination, string member, ResolutionContext context)
		{
			return ResolveCurrency(source.Currency);
		}

		public string Resolve(BusinessOne.Contracts.Invoice source, InvoiceDto destination, string destMember, ResolutionContext context)
		{
			return ResolveCurrency(source.DocCurrency);
		}

		private string ResolveCurrency(string currency)
		{
			if (string.IsNullOrWhiteSpace(currency))
				return null;

			switch (currency)
			{
				case "$":
					return "USD";
				default:
					return currency;
			}
		}
	}
}
