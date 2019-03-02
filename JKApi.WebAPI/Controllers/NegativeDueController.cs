using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.Service;
using JKApi.Service.AccountPayable;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Filters;
using Microsoft.Web.Http;
using System.Linq;
using JK.Repository.Uow;
using JKApi.WebAPI.Dtos.NegativeDue;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using JKApi.Data.DAL;
using JKViewModels.AccountsPayable;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/negativedue")]
    [Authorized]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NegativeDueController : BaseApiController
    {
        private readonly AccountPayableService _accountsPayableService;
        private readonly IUserService _userService;

        // ======================================================================================
        #region NegativeDueController > Constructor
        // ======================================================================================

        //        public NegativeDueController(IAccountPayableService accountPayableService)
        //        {
        //            _accountsPayableService = accountsPayableService;
        //        }
        /* Somehow is not letting me inject the IAccountPayableService, need to check why and update. - German Sosa 10/31/2018*/
        public NegativeDueController(AccountPayableService accountsPayableService, IUserService userService)
        {
            _accountsPayableService = accountsPayableService;
            _userService = userService;

        }
        #endregion
        // ======================================================================================
        #region NegativeDueController > API Calls
        // ======================================================================================




        [Route("GetList")]
        [HttpPost]
        public IHttpActionResult GetList(GetListRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {

                /*Get Period Requested */
                var period = _accountsPayableService.GetPeriod(requestDto.SelectedPeriodId);

                /* Validate If Period has already processed Negative Dues */
                var response = _accountsPayableService.GetNagativeDue(requestDto.FranchiseeStatus, requestDto.RegionIds);
                var results = from f in response
                                      orderby f.RegionName ascending, f.FranchiseeNo ascending
                                      select new
                                      {
                                          FranchiseeNo = f.FranchiseeNo,
                                          FranchiseeName = f.Name,
                                          Balance = (decimal)(f.Balance != null ? f.Balance : 0.00m),
                                          Name = f.Name,
                                          FranchiseeId = f.FranchiseeId,
                                          RegionName = f.RegionName,
                                          NegativeDueId = f.NegativeDueId
                                      };

                //var getAllValuesList = new List<getAllValues>()
                //{
                //    new getAllValues()
                //    {
                //        SelectedPeriod = period,
                //        Data  = negativeDueData
                //    }
                //};

                return ResponseSuccessResult(results);
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

        [Route("SetNextAction")]
        [HttpPost]
        public IHttpActionResult SetNextAction(List<SetNextActionRequestDto> selectedNegativeDueRows)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                bool returnedValue = false;
                foreach (var negativeDue in selectedNegativeDueRows)
                {
                    /* Verify if Period Still Open */
                    var resultdata = _accountsPayableService.GetPeriodClosed(negativeDue.SelectedPeriodId);

                    if (resultdata.NegativeDueFinalized == false)
                    {
                        var finalizeNegativeDuePeriod = true;
                        var selectedPeriodId = (int)resultdata.PeriodId;
                        var selectedRegionId = negativeDue.RegionId;
                        var userId = -99; /* This should be changed to get the current used logged in in the api */
                        foreach (var row in negativeDue.SelectedRows)
                        {
                                returnedValue = _accountsPayableService.UpdateNegativeDue(userId, selectedPeriodId, row.NegativeDueId, row.Rollover, row.BalanceAfterRollover);
                                row.IsNegativeDueProcessed = returnedValue;
                                row.ProcessStatusDescription = "Succesfully processed Negative Due.";
                                if (!returnedValue)
                                {
                                    finalizeNegativeDuePeriod = false;
                                    row.ProcessStatusDescription = "Error processing negative due";
                                }
                        }

                        if (finalizeNegativeDuePeriod)
                        {
                            bool finalizeNegativeDueResult = _accountsPayableService.UpdatePeriodClosed(negativeDue.SelectedPeriodId);
                            negativeDue.IsSelectedPeriodIdFinalized = finalizeNegativeDueResult;
                            negativeDue.SelectedPeriodProcessStatus = "Negative Due for this period has Succesfully Processed and Negative Due for the Period Finalized.";
                        }
                    } else
                    {
                        negativeDue.IsSelectedPeriodIdFinalized = false;
                        negativeDue.SelectedPeriodProcessStatus = "Negative Due for this period has been processed already.";
                    }
                }

                return ResponseSuccessResult(selectedNegativeDueRows);
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

        private class getAllValues
        {
            public Period SelectedPeriod { get; internal set; }
            public NegativeDueViewModel Data { get; internal set; }
        }


        #endregion
    }
}