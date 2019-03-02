using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.Service.ServiceContract.Customer;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos;
using JKApi.WebAPI.Filters;
using JKViewModels.Customer;
using Microsoft.Web.Http;
using ServiceCallLogAreaList = JKApi.WebAPI.Dtos.ServiceCallLogAreaList;
using ServiceCallLogTypeList = JKApi.WebAPI.Dtos.ServiceCallLogTypeList;
using StatusResultList = JKApi.WebAPI.Dtos.StatusResultList;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/customer")]
    [Authorized]
    public class CustomerController : BaseApiController
    {
        private readonly ICustomerService _customerService;

        // ======================================================================================
        #region ContractController > Constructor
        // ======================================================================================

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        #endregion
        // ======================================================================================
        #region CustomerController > API Calls
        // ======================================================================================

        [Route("listbyregion")]
        [HttpPost]
        [ResponseType(typeof(IList<CustomerModel>))]
        public IHttpActionResult ListByRegion(CustomerByRegionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerModels = _customerService.GetCustomerListByRegion(requestDto.RegionId, requestDto.PageSize, requestDto.Page);
                return ResponseSuccessResult(customerModels);
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
        [ResponseType(typeof(IList<CustomerModel>))]
        public IHttpActionResult ListByFranchisee(CustomerByFranchiseeRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerModels = _customerService.GetCustomerListByFranchisee(requestDto.FranchiseeId, requestDto.PageSize, requestDto.Page);
                return ResponseSuccessResult(customerModels);
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

        [Route("nearbylistbyfranchisee")]
        [HttpPost]
        [ResponseType(typeof(IList<CustomerModel>))]
        public IHttpActionResult NearByListByFranchisee(NearbyByFranchiseeRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerModels = _customerService.GetNearByCustomerListByFranchisee(requestDto.FranchiseeId, requestDto.Latitude, requestDto.Longitude, requestDto.Distance);
                return ResponseSuccessResult(customerModels);
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

        [Route("nearbylistbyregion")]
        [HttpPost]
        [ResponseType(typeof(IList<CustomerModel>))]
        public IHttpActionResult NearByListByRegion(NearbyByRegionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerModels = _customerService.GetNearByCustomerListByRegion(requestDto.RegionId, requestDto.Latitude, requestDto.Longitude, requestDto.Distance);
                return ResponseSuccessResult(customerModels);
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

        [Route("nearbyleadlistbyregion")]
        [HttpPost]
        [ResponseType(typeof(IList<CustomerLeadModel>))]
        public IHttpActionResult NearByLeadListByRegion(NearbyByRegionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerModels = _customerService.GetNearByLeadListByRegion(
                    requestDto.RegionId, requestDto.Latitude, requestDto.Longitude, requestDto.Distance);
                return ResponseSuccessResult(customerModels);
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

        [Route("lead/{leaddetailid}")]
        [HttpGet]
        [ResponseType(typeof(IList<CustomerLeadModel>))]
        public IHttpActionResult NearByLeadListByRegion(int leaddetailid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerLeadModel = _customerService.GetLeadDetail(leaddetailid);
                return ResponseSuccessResult(customerLeadModel);
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

        [Route("pendinglistbyregion")]
        [HttpPost]
        [ResponseType(typeof(IList<CustomerModel>))]
        public IHttpActionResult PendingList(CustomerByRegionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customerModels = _customerService.GetCustomerPendingListByRegion(requestDto.RegionId, requestDto.PageSize, requestDto.Page);
                return ResponseSuccessResult(customerModels);
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

        [Route("{customerid}")]
        [HttpGet]
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult ById(int customerid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var customer = _customerService.GetCustomer(customerid);
                return ResponseSuccessResult(customer);
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

        [Route("accountwalkthru/itemlistbycustomer")]
        [HttpPost]
        [ResponseType(typeof(IList<AccountWalkThruItemModel>))]
        public IHttpActionResult AccountWalkThruItemListByCustomer(AccountWalkThruItemListByCustomerRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var itemModels = _customerService.GetAccountWalkThruItemListByCustomer(requestDto.CustomerId);
                return ResponseSuccessResult(itemModels);
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

        [Route("accountwalkthru/formbycustomer")]
        [HttpPost]
        [ResponseType(typeof(AccountWalkThruFormModel))]
        public IHttpActionResult AccountWalkThruFormByCustomer(AccountWalkThruItemListByCustomerRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var items = _customerService.GetAccountWalkThruItemListByCustomer(requestDto.CustomerId);
                var formModel = new AccountWalkThruFormModel(items);
                return ResponseSuccessResult(formModel);
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

        [Route("accountwalkthru/{accountwalkthruitemid}")]
        [HttpGet]
        [ResponseType(typeof(AccountWalkThruFormModel))]
        public IHttpActionResult AccountWalkThruItemById(int accountwalkthruitemid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var walkThruItem = _customerService.GetAccountWalkThruItemById(accountwalkthruitemid);
                return ResponseSuccessResult(walkThruItem);
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

        [Route("accountwalkthru/addorupdateform")]
        [HttpPut]
        [ResponseType(typeof(AccountWalkThruFormModel))]
        public IHttpActionResult AddOrUpdateForm(AccountWalkThruCheckListUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var items = new List<AccountWalkThruItemModel>();
                foreach (var checkItem in requestDto.CheckList)
                {
                    var item = new AccountWalkThruItemModel
                    {
                        AccountWalkThruItemId = checkItem.AccountWalkThruItemId,
                        AccountWalkThruType = checkItem.AccountWalkThruType,
                        CustomerId = checkItem.CustomerId,
                        FranchiseeId = checkItem.FranchiseeId,
                        FieldValue = checkItem.FieldValue,
                        FieldText = checkItem.FieldText,
                        FileUrl = checkItem.FileUrl,
                        CreatedBy = checkItem.CreatedBy,
                        CreatedDate = checkItem.CreatedDate,
                        ModifiedBy = checkItem.ModifiedBy,
                        ModifiedDate = requestDto.WalkThruDate ?? DateTime.Now
                    };
                    item = _customerService.AddOrUpdateAccountWalkThruItem(item);
                    if (item != null) items.Add(item);
                }

                var formModel = new AccountWalkThruFormModel(items);
                return ResponseSuccessResult(formModel);
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

        [Route("accountwalkthru/addorupdateitem")]
        [HttpPut]
        [ResponseType(typeof(AccountWalkThruItemModel))]
        public IHttpActionResult AddOrUpdateItem(AccountWalkThruUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var walkThruItem = new AccountWalkThruItemModel
                {
                    AccountWalkThruItemId = requestDto.AccountWalkThruItemId,
                    AccountWalkThruType = requestDto.AccountWalkThruType,
                    CustomerId = requestDto.CustomerId,
                    FranchiseeId = requestDto.FranchiseeId,
                    FieldValue = requestDto.FieldValue,
                    FieldText = requestDto.FieldText,
                    FileUrl = requestDto.FileUrl,
                    CreatedBy = requestDto.CreatedBy,
                    CreatedDate = requestDto.CreatedDate,
                    ModifiedBy = requestDto.ModifiedBy,
                    ModifiedDate = requestDto.ModifiedDate
                };

                walkThruItem = _customerService.AddOrUpdateAccountWalkThruItem(walkThruItem);
                return ResponseSuccessResult(walkThruItem);
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

        [Route("getservicecalllogdata")]
        [HttpPost]
        [ResponseType(typeof(ServiceCallLogData))]
        public IHttpActionResult GetServiceCallLogData()
        {
            try
            {
                var responseDtos = new ServiceCallLogData();

                #region ::  Service Call Log Area List ::

                var ServiceCallLogAreaList = _customerService.GetServiceCallLogAreaList();
                if (ServiceCallLogAreaList != null)
                {
                    List<ServiceCallLogAreaList> ServiceCallLogAreaListModel = new List<ServiceCallLogAreaList>();
                    foreach (var item in ServiceCallLogAreaList)
                    {
                        ServiceCallLogAreaList ServiceCallLogAreaItem = new ServiceCallLogAreaList();
                        ServiceCallLogAreaItem.ServiceCallLogAreaListId = item.ServiceCallLogAreaListId;
                        ServiceCallLogAreaItem.Name = item.Name;
                        ServiceCallLogAreaListModel.Add(ServiceCallLogAreaItem);
                    }
                    responseDtos.ServiceCallLogAreaList = ServiceCallLogAreaListModel;
                }

                #endregion

                #region :: Service Call Log Type List ::

                var ServiceCallLogTypeList = _customerService.GetServiceCallLogTypeList();
                if (ServiceCallLogTypeList != null)
                {
                    List<ServiceCallLogTypeList> ServiceCallLogTypeListModel = new List<ServiceCallLogTypeList>();
                    foreach (var item in ServiceCallLogTypeList)
                    {
                        ServiceCallLogTypeList ServiceCallLogTypeListtemItem = new ServiceCallLogTypeList();
                        ServiceCallLogTypeListtemItem.ServiceCallLogTypeListId = item.ServiceCallLogTypeListId;
                        ServiceCallLogTypeListtemItem.Name = item.Name;
                        ServiceCallLogTypeListModel.Add(ServiceCallLogTypeListtemItem);
                    }
                    responseDtos.ServiceCallLogTypeList = ServiceCallLogTypeListModel;
                }

                #endregion

                #region :: Status Result List ::

                var StatusResultList = _customerService.GetStatusResultList();
                if (StatusResultList != null)
                {
                    List<StatusResultList> StatusResultListModel = new List<StatusResultList>();
                    foreach (var item in StatusResultList)
                    {
                        StatusResultList StatusResultListItem = new StatusResultList();
                        StatusResultListItem.StatusResultListId = item.StatusResultListId;
                        StatusResultListItem.Name = item.Name;
                        StatusResultListModel.Add(StatusResultListItem);
                    }
                    responseDtos.StatusResultList = StatusResultListModel;
                }

                #endregion

                var userLista = _customerService.GetUserOfDefaultRegion(2, true);
                if (userLista.Count > 0)
                {

                    List<UserResultList> UserResultListModel = new List<UserResultList>();
                    foreach (var item in userLista)
                    {
                        UserResultList UserResultListItem = new UserResultList();
                        UserResultListItem.UserId = item.UserId;
                        UserResultListItem.UserName = item.UserName;
                        UserResultListItem.FirstName = item.FirstName;
                        UserResultListItem.LastName = item.LastName;
                        UserResultListModel.Add(UserResultListItem);
                    }
                    responseDtos.UserResultList = UserResultListModel;                    
                }                
                return ResponseSuccessResult(responseDtos);
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

        [Route("getfrancisesbycustomerId")]
        [HttpPost]
        [ResponseType(typeof(IList<FranchiseModel>))]
        public IHttpActionResult GetFrancisesByCustomerId(int CustomerId)
        {
            try
            {
                var responseDtos = new List<FranchiseModel>();
                var FrancisesData = _customerService.getFrancisesByCustomerId(CustomerId);
                if (FrancisesData != null)
                {
                    foreach (var item in FrancisesData)
                    {
                        FranchiseModel FranchiseModelItem = new Dtos.FranchiseModel();
                        FranchiseModelItem.FranchiseId = item.Value;
                        FranchiseModelItem.FranchiseName = item.Key;
                        responseDtos.Add(FranchiseModelItem);
                    }
                }
                return ResponseSuccessResult(responseDtos);
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

        [Route("getcollectionlogdata")]
        [HttpPost]
        [ResponseType(typeof(IList<CollectionsCallLogData>))]
        public IHttpActionResult GetCollectionLogData(int customerId)
        {
            try
            {
                var responseDtos = new List<CollectionsCallLogData>();

                #region :: Collections Call Log :: 

                var collectionsData = _customerService.GetCollectionLog(customerId);
                if (collectionsData != null)
                {
                    foreach (var item in collectionsData)
                    {
                        var collectionsCallLogDataItem = new CollectionsCallLogData
                        {
                            CollectionsCallLogId = item.CollectionsCallLogId,
                            CallDate = item.CallDate,
                            CallTime = item.CallTime,
                            Status = item.StatusResultListId != null ? _customerService.GetStatus(item.StatusResultListId) : null,
                            SpokeWith = item.SpokeWith ?? string.Empty,
                            Action = item.Action ?? string.Empty,
                            CallBack = item.CallBack,
                            Comments = item.Comments
                        };
                        responseDtos.Add(collectionsCallLogDataItem);
                    }
                }

                #endregion

                return ResponseSuccessResult(responseDtos);
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

        [Route("getservicecalldata")]
        [HttpPost]
        [ResponseType(typeof(IList<ServiceCallLogList>))]
        public IHttpActionResult GetServiceCallData(int customerId)
        {
            try
            {
                var responseDtos = new List<ServiceCallLogList>();

                #region :: Service Call Log List :: 

                var serviceLogData = _customerService.GetServiceLog(customerId);
                if (serviceLogData != null)
                {
                    foreach (var item in serviceLogData)
                    {
                        var serviceCallLogListItem = new ServiceCallLogList
                        {
                            ServiceCallLogId = item.ServiceCallLogId,
                            CallDate = item.CallDate != null ? item.CallDate.ToString() : null,
                            CallTime = item.CallTime?.ToString() != null ? item.CallTime?.ToString() : null,
                            Status = item.StatusResultListId != null ? _customerService.GetStatus(item.StatusResultListId) : null,
                            SpokeWith = item.SpokeWith ?? string.Empty,
                            Action = item.Action ?? string.Empty,
                            CallBack = item.CallBack,
                            Comments = item.Comments
                        };
                        responseDtos.Add(serviceCallLogListItem);
                    }
                }

                #endregion

                return ResponseSuccessResult(responseDtos);
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

        [Route("saveservicecalllog")]
        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult SaveserviceCallLog(ServiceCallLogModel serviceCallLogModel)
        {
            try
            {
                var id = _customerService.SaveServiceCallLogDetailsByFORM(serviceCallLogModel);
                return ResponseSuccessResult(id);
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

        [Route("savecollectionscalllog")]
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveCollectionsCallLog(CollectionsCallLogModel collectionsCallLogModel)
        {
            try
            {
                var status = _customerService.SaveCollectionCallLog(collectionsCallLogModel);
                return ResponseSuccessResult(status);
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