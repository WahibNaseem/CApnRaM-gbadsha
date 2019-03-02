using Microsoft.Practices.Unity;
using System.Web.Http;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.Distribution;
using JKApi.Service;
using JKApi.Service.Service.Distribution;
using JKApi.Service.Service.Customer;
using JKApi.Service.Service.Inspection;
using JKApi.Service.ServiceContract.Franchisee;
using JKApi.Service.Service.Region;
using JKApi.Service.ServiceContract.Inspection;
using JKApi.Service.ServiceContract.Contract;
using JKApi.Service.Service.Contract;
using JKApi.Service.ServiceContract.Enums;
using JKApi.Service.Service.Enums;
using JKApi.Service.Service.CRM;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.Outlook;
using JKApi.Service.ServiceContract.Outlook;
using JK.Repository.Uow;
using JK.Repository.Helpers;
using JKApi.Core;
using Core.Framework.Logger;
using JKApi.Core.EncryptDecrypt;
using JKApi.Service.Service.File;
using JKApi.Service.Service.FOM;
using JKApi.Service.Service.Job;

namespace JKApi.WebAPI
{
    /// <summary>
    /// UnityConfig
    /// </summary>
    public static class UnityConfig
    {
        /// <summary>
        /// Register components for the project.
        /// </summary>
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        /// <summary>
        /// Register all the dependency injection components.
        /// </summary>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ILoggerUtility, LoggerUtility>();
           
            container.RegisterType<IEncryptDecrypt, EncryptDecrypt>();
            container.RegisterType<IRepositoryProvider, RepositoryProvider>();
            container.RegisterType<IJKEfUow, JKEfUow>();
            container.RegisterType<ICacheProvider, CacheProvider>();

            container.RegisterType<IAccountTypeListService, AccountTypeListService>();
            container.RegisterType<IServiceTypeListService, ServiceTypeListService>();
            container.RegisterType<IContactTypeListService, ContactTypeListService>();

            container.RegisterType<IDistributionService, DistributionService>();
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<IContractService, ContractService>();
            container.RegisterType<IContractDetailService, ContractDetailService>();
            container.RegisterType<IFranchiseeService, FranchiseeService>();
            container.RegisterType<IInspectionService, InspectionService>();
            container.RegisterType<ITemplateService, TemplateService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<ICRM_Service, CRM_Service>();
            container.RegisterType<IOutlookService, OutlookService>();
            container.RegisterType<IJobService, JobService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<ICommonService, CommonService>();
        }
    }
}