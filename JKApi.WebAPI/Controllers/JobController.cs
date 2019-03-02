using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos;
using Microsoft.Web.Http;
using JKApi.Service.Service.Job;
using JKApi.WebAPI.Filters;
using JKViewModels.Job;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/job")]
    [Authorized]
    public class JobController : BaseApiController
    {
        private readonly IJobService _jobService;

        // ======================================================================================
        #region JobController > Constructor
        // ======================================================================================

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        #endregion
        // ======================================================================================
        #region JobController > API Calls

        [Route("listbyregion")]
        [HttpPost]
        [ResponseType(typeof(List<JobModel>))]
        public IHttpActionResult ListByRegion(JobListRequestByRegionDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var jobs = _jobService.GetJobListByRegionId(requestDto.RegionId);
                return ResponseSuccessResult(jobs);
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

        [Route("listbyfranchisee")]
        [HttpPost]
        [ResponseType(typeof(List<JobModel>))]
        public IHttpActionResult ListByFranchisee(JobListRequestByFranchiseeDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var jobs = _jobService.GetJobListByFranchiseeId(requestDto.FranchiseeId);
                return ResponseSuccessResult(jobs);
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

        [Route("listbycustomer")]
        [HttpPost]
        [ResponseType(typeof(List<JobModel>))]
        public IHttpActionResult ListByCustomer(JobListRequestByCustomerDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var jobs = _jobService.GetJobListByCustomerId(requestDto.CustomerId);
                return ResponseSuccessResult(jobs);
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

        [Route("listbyids")]
        [ResponseType(typeof(List<JobModel>))]
        public IHttpActionResult ByIds(JobListRequestByIdsDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var jobs = requestDto.JobIds.Select(jobId => _jobService.GetJob(jobId)).Where(job => job != null).ToList();
                return ResponseSuccessResult(jobs);
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

        [Route("{jobid}")]
        [HttpGet]
        [ResponseType(typeof(JobModel))]
        public IHttpActionResult ById(int jobid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var job = _jobService.GetJob(jobid);
                return ResponseSuccessResult(job);
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

        [Route("update")]
        [HttpPut]
        [ResponseType(typeof(JobModel))]
        public IHttpActionResult Update(JobAddOrUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var job = _jobService.UpdateJob(new JobModel
                {
                    JobId = requestDto.JobId,
                    DistributionId = requestDto.DistributionId,
                    FranchiseeId = requestDto.FranchiseeId,
                    ContractId = requestDto.ContractId,
                    ContractDetailId = requestDto.ContractDetailId,
                    SoldById = requestDto.SoldById,
                    ContractTypeListId = requestDto.ContractTypeListId,
                    AccountTypeListId = requestDto.AccountTypeListId,
                    ServiceTypeListId = requestDto.ServiceTypeListId,
                    PurchaseOrderNumber = requestDto.PurchaseOrderNumber,
                    CustomerNumber = requestDto.CustomerNumber,
                    CustomerName = requestDto.CustomerName,
                    PrimaryContact = requestDto.PrimaryContact,
                    PrimaryContactPhone = requestDto.PrimaryContactPhone,
                    PrimaryContactPhoneExt = requestDto.PrimaryContactPhoneExt,
                    StartDate = requestDto.StartDate,
                    ExpirationDate = requestDto.ExpirationDate,
                    ContractDescription = requestDto.ContractDescription,
                    ContractTermMonth = requestDto.ContractTermMonth,
                    Amount = requestDto.Amount,
                    SquareFootage = requestDto.SquareFootage,
                    CleanTimes = requestDto.CleanTimes,
                    Mon = requestDto.Mon,
                    Tue = requestDto.Tue,
                    Wed = requestDto.Wed,
                    Thu = requestDto.Thu,
                    Fri = requestDto.Fri,
                    Sat = requestDto.Sat,
                    Sun = requestDto.Sun,
                    StartTime = requestDto.StartTime,
                    EndTime = requestDto.EndTime,
                    AssignedTeamId = requestDto.AssignedTeamId,
                    AssignedEmployeeId = requestDto.AssignedEmployeeId,
                    AssignedTemplate = requestDto.AssignedTemplate
                });
                return ResponseSuccessResult(job);
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
        // ======================================================================================
    }
}
