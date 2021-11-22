using Abp.AspNetCore.Configuration;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using DigitalCreditBroker.Configuration;
using DigitalCreditBroker.Provider.Erp;
using DigitalCreditBroker.Provider.SAP.BusinessOne;
using DigitalCreditBroker.Provider.SAP.HanaCloud;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace DigitalCreditBroker.Provider.SAP
{
    public class DigitalCreditBrokerSAPModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public DigitalCreditBrokerSAPModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(MapperConfiguration.Config);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(Component.For<IErpService>().ImplementedBy<SAPB1Service>().Named("SapB1Service"));
            IocManager.IocContainer.Register(Component.For<IErpService>().ImplementedBy<SAPHANAService>().Named("SapHANAService"));

            //Configuration.Modules.AbpAspNetCore()
            //   .CreateControllersForAppServices(typeof(DigitalCreditBrokerSAPModule).Assembly, moduleName: "provider", useConventionalHttpVerbs: true);
        }
    }
}
