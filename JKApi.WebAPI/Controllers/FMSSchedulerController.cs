using System;
using System.Web.Http;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Outlook;
using JKApi.WebAPI.Common;
using Microsoft.Web.Http;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Core;
using JKApi.WebAPI.Filters;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/Schedule")]
    [Authorized]
    public class FMSSchedulerController : BaseApiController
    {
        private readonly ICRM_Service _crmService;
        private readonly IOutlookService _outlookService;
        private readonly ICustomerService _customerService;
        private readonly MailService _mailService;
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        public jkDatabaseEntities jkEntityModel = jkDatabaseEntities.Instance;
        // ======================================================================================
        #region CRMController > Constructor
        // ======================================================================================

        /// <summary>
        /// Construct with dependencies.
        /// </summary>
        public FMSSchedulerController(ICRM_Service crmService, IOutlookService outlookService, ICustomerService customerService, IUserService userService, ICommonService commonService)
        {
            _crmService = crmService;
            _outlookService = outlookService;
            _customerService = customerService;
            _mailService = new MailService();
            _userService = userService;
            _commonService = commonService;
        }

        [Route(template: "CustomerSuspendedToActive")]
        [HttpGet]
        public IHttpActionResult CustomerSuspendedToActive()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }
            try
            {
                return ResponseSuccessResult(_commonService.Customer_SuspendedTOActive());
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }
        #endregion
    }
}
