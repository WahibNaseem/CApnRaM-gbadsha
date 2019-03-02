using Core.Framework.Logger;
using Elmah;
using JK.Repository.Helpers;
using JK.Repository.RepositoryFactory;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Service.AccountReceivable;
using JKApi.Service.AccountPayable;
using JKApi.Service.Service.CRM;
using JKApi.Service.Service.Customer;
using JKApi.Service.Service.CustomerInvoice;
using JKApi.Service.Service.Region;
using JKApi.Service.Service.Management;
using JKApi.Service.Outlook;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.AccountPayable;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.CustomerInvoice;
using JKApi.Service.ServiceContract.Franchisee;
using JKApi.Service.ServiceContract.JKControl;
using JKApi.Service.ServiceContract.Management;
using JKApi.Service.ServiceContract.Outlook;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Web.Mvc;
using System.Web.Routing;
using JKApi.Core.EncryptDecrypt;
using JKApi.Service;
using JKApi.Service.Service.Administration.Company;
using JKApi.Service.ServiceContract.Company;
using JKApi.Service.Service.Company;
using JKApi.Service.Service.Inspection;
using JKApi.Service.Service.Job;
using JKApi.Service.ServiceContract.Inspection;

namespace JK.FMS.MVC
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = new UnityContainer();

            IControllerFactory unityFactory = new UnityControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(unityFactory);

            BuildUnityContainer(container);

        }

        private static void BuildUnityContainer(IUnityContainer container)
        {


            container.RegisterType<ILoggerUtility, LoggerUtility>();
            container.RegisterType<IInterceptionBehavior, ExceptionLoggerBehavior>();

            container.RegisterType<RepositoryFactories, RepositoryFactories>();
            container.RegisterType<IRepositoryProvider, RepositoryProvider>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IJKEfUow, JKEfUow>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ICustomerService, CustomerService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ICustomerInvoiceService, CustomerInvoiceService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ILeadService, LeadService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IFranchiseeService, FranchiseeService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IAccountReceivableService, AccountReceivableService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IAccountPayableService, AccountPayableService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
          
            container.RegisterType<ICRM_Service, CRM_Service>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IOutlookService, OutlookService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IInspectionService, InspectionService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IUserService, UserService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ICompanyService, CompanyService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IManagementService,ManagementService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ITemplateService, TemplateService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<IJobService, JobService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            //These are common for all

            container.RegisterType<IEncryptDecrypt, EncryptDecrypt>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ICacheProvider, CacheProvider>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());
            container.RegisterType<ICommonService, CommonService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<ExceptionLoggerBehavior>());

            


            
        }
    }

    /// <summary>
    /// Custom Controller Factory to allow Unity to manage controller creation and dep injection.
    /// </summary>
    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer _container;

        public UnityControllerFactory(IUnityContainer container)
        {
            _container = container;

            _container.AddNewExtension<Interception>();
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {  
            try
            {
                return (IController)_container.Resolve(GetControllerType(requestContext, controllerName));
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
            }
            return base.CreateController(requestContext, controllerName);
        }

        public override void ReleaseController(IController controller)
        {
            _container.Teardown(controller);

            base.ReleaseController(controller);
        }
    }
}