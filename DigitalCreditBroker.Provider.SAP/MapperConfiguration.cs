using AutoMapper;
using DigitalCreditBroker.Company.Erp;
using DigitalCreditBroker.Provider.Erp.Dto;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace DigitalCreditBroker.Provider.SAP
{
    public class MapperConfiguration
    {
        public static Action<IMapperConfigurationExpression> Config
        {
            get
            {
                return cfg =>
                {
                    cfg.CreateMap<BusinessOne.Contracts.BusinessPartner, BusinessPartnerDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CardCode))
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CardName))
                        .ForMember(dest => dest.VAT, opt => opt.MapFrom(src => src.VATRegistrationNumber));

                    cfg.CreateMap<BusinessOne.Contracts.Invoice, InvoiceDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DocEntry))
                        .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.DocDate))
                        .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DocDueDate))
                        .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.DocTotal))
                        .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.DocCurrency))
                        .ForMember(dest => dest.ClosingDate, opt => opt.MapFrom(src => src.ClosingDate))
                        .ForMember(dest => dest.Currency, opt => opt.MapFrom<CurrencyResolver>());

                    cfg.CreateMap<HanaCloud.Contracts.BusinessPartner, BusinessPartnerDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BusinessPartnerFullName))
                        .ForMember(dest => dest.VAT, opt => opt.MapFrom((src, dst, opt) =>
                        {
                            return src.ToBusinessPartnerTax?.Results?.FirstOrDefault()?.BpTaxNumber ?? "";
                        }))
                        .ForMember(dest => dest.Country, opt => opt.MapFrom((src, dst, opt) =>
                        {
                            return src.ToBusinessPartnerAddress?.Results?.FirstOrDefault()?.Country ?? "";                            
                        }));


                    cfg.CreateMap<HanaCloud.Contracts.Invoice, InvoiceDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BillingDocument))
                        .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.PostingDate))
                        .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.NetDueDate))
                        .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AmountInTransactionCurrency))
                        .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TransactionCurrency))
                        .ForMember(dest => dest.ClosingDate, opt => opt.MapFrom(src => src.ClearingDate));
                };
            }
        }
    }
}
