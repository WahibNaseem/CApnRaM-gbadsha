using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.Service.Service.Inspection;
using JKApi.Service.Service.Job;
using JKApi.Service.ServiceContract.Inspection;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos;
using JKApi.WebAPI.Filters;
using Microsoft.Web.Http;
using JKViewModels;
using JKViewModels.Inspection;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/inspection")]
    [Authorized]
    public class InspectionController : BaseApiController
    {
        private readonly IInspectionService _inspectionService;

        // ======================================================================================
        #region InspectionController > Constructor
        // ======================================================================================

        public InspectionController(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        #endregion
        // ======================================================================================
        #region InspectionController > API Calls

        [Route("listbyregion")]
        [HttpPost]
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult ListByRegion(InspectionFormListByRegionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    RegionId = requestDto.RegionId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormListByRegion(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult ListByFranchisee(InspectionFormListByFranchiseeRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    FranchiseeId = requestDto.FranchiseeId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormListByFranchisee(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult ListByCustomer(InspectionFormListByCustomerDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    CustomerId = requestDto.CustomerId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormListByCustomer(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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

        [Route("listbyjob")]
        [HttpPost]
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult ListByJob(InspectionFormListByJobDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    JobId = requestDto.JobId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormListByJob(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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

        [Route("{inspectionid}")]
        [HttpGet]
        [ResponseType(typeof(InspectionFormModel))]
        public IHttpActionResult ById(int inspectionid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspection = _inspectionService.GetInspectionForm(inspectionid);
                return ResponseSuccessResult(inspection);
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

        [Route("currentinspection")]
        [HttpPost]
        [ResponseType(typeof(InspectionFormModel))]
        public IHttpActionResult CurrentInspection(InspectionFormListByCustomerDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspection = _inspectionService.GetCurrentInspectionFormByCustomer(requestDto.CustomerId);
                return ResponseSuccessResult(inspection);
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

        [Route("addorupdate")]
        [HttpPut]
        [ResponseType(typeof(InspectionFormModel))]
        public IHttpActionResult AddOrUpdate(InspectionFormUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspectionForm = new InspectionFormModel
                {
                    InspectionFormId = requestDto.InspectionFormId,
                    JobId = requestDto.JobId,
                    CustomerId = requestDto.CustomerId,
                    ServiceTypeListId = requestDto.ServiceTypeListId,
                    AccountTypeListId = requestDto.AccountTypeListId,
                    InspectionStatusId = requestDto.InspectionStatusId,
                    CallDate = requestDto.CallDate,
                    RecordedDate = requestDto.RecordedDate,
                    UploadedDate = requestDto.UploadedDate,
                    IsCompleted = requestDto.IsCompleted,
                    InspectedBy = requestDto.InspectedBy,
                    InspectorId = requestDto.InspectorId,
                    FormName = requestDto.FormName,
                    Description = requestDto.Description,
                    PassPoints = requestDto.PassPoints,
                    FailPoints = requestDto.FailPoints,
                    NeedImprovementPoints = requestDto.NeedImprovementPoints,
                    ScorePercent = requestDto.ScorePercent,
                    SignatureUrl = requestDto.SignatureUrl,
                    IsEnable = true,
                    IsDelete = false,
                    ModifiedDate = DateTime.Now
                };

                inspectionForm = _inspectionService.AddOrUpdateInspectionForm(inspectionForm);
                return ResponseSuccessResult(inspectionForm);
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

        [Route("complete")]
        [HttpPut]
        [ResponseType(typeof(InspectionFormModel))]
        public IHttpActionResult Complete(InspectionFormUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspectionForm = new InspectionFormModel
                {
                    InspectionFormId = requestDto.InspectionFormId,
                    JobId = requestDto.JobId,
                    CustomerId = requestDto.CustomerId,
                    ServiceTypeListId = requestDto.ServiceTypeListId,
                    AccountTypeListId = requestDto.AccountTypeListId,
                    InspectionStatusId = requestDto.InspectionStatusId,
                    CallDate = requestDto.CallDate,
                    RecordedDate = requestDto.RecordedDate,
                    UploadedDate = requestDto.UploadedDate,
                    IsCompleted = true,
                    InspectedBy = requestDto.InspectedBy,
                    FormName = requestDto.FormName,
                    Description = requestDto.Description,
                    PassPoints = requestDto.PassPoints,
                    FailPoints = requestDto.FailPoints,
                    NeedImprovementPoints = requestDto.NeedImprovementPoints,
                    ScorePercent = requestDto.ScorePercent,
                    SignatureUrl = requestDto.SignatureUrl,
                    IsEnable = true,
                    IsDelete = false,
                    ModifiedDate = DateTime.Now
                };

                inspectionForm = _inspectionService.CompleteInspectionForm(inspectionForm);
                return ResponseSuccessResult(inspectionForm);
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

        [Route("area/addorupdate")]
        [HttpPut]
        [ResponseType(typeof(InspectionFormSectionModel))]
        public IHttpActionResult AddOrUpdateArea(InspectionFormSectionResponseDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var area = _inspectionService.AddOrUpdateInspectionFormSection(new InspectionFormSectionModel
                {
                    InspectionFormSectionId = requestDto.InspectionFormSectionId,
                    InspectionFormId = requestDto.InspectionFormSectionId,
                    SectionOrder = requestDto.SectionOrder,
                    SectionName = requestDto.SectionName,
                    SectionStatus = requestDto.SectionStatus,
                    PassPoints = requestDto.PassPoints,
                    FailPoints = requestDto.FailPoints,
                    NeedImprovementPoints = requestDto.NeedImprovementPoints,
                    SectionAutoFail = requestDto.SectionAutoFail,
                    SectionAutoFailReason = requestDto.SectionAutoFailReason,
                    IsEnable = true,
                    IsDelete = false,
                    ModifiedDate = DateTime.Now
                });
                return ResponseSuccessResult(area);
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

        [Route("area/addarea")]
        [HttpPut]
        [ResponseType(typeof(InspectionFormSectionResponseDto))]
        public IHttpActionResult AddAreaFromAreaTemplate(TemplateAreaRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var section = _inspectionService.AddInpsectionFormSectionFromTemplateArea(requestDto.InspectionFormId, new TemplateAreaModel
                {
                    TemplateAreaId = requestDto.TemplateAreaId,
                    AreaName = requestDto.AreaName,
                });
                return ResponseSuccessResult(section);
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

        [Route("area/{areaid}")]
        [HttpGet]
        [ResponseType(typeof(List<InspectionFormItemModel>))]
        public IHttpActionResult ItemsByArea(int areaid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspectionFormItems = _inspectionService.GetInspectionFormItemListBySection(areaid);
                return ResponseSuccessResult(inspectionFormItems);
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

        [Route("area/item/{itemid}")]
        [HttpGet]
        [ResponseType(typeof(InspectionFormItemModel))]
        public IHttpActionResult AreaItem(int itemid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspectionFormItem = _inspectionService.GetInspectionFormItem(itemid);
                return ResponseSuccessResult(inspectionFormItem);
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

        [Route("area/item/addorupdate")]
        [HttpPut]
        [ResponseType(typeof(InspectionFormItemModel))]
        public IHttpActionResult AddOrUpdateAreaItem(InspectionFormItemResponseDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var item = _inspectionService.AddOrUpdateInpsectionFormItem(new InspectionFormItemModel
                {
                    InspectionFormItemId = requestDto.InspectionFormItemId,
                    InspectionFormSectionId = requestDto.InspectionFormSectionId,
                    FormItemOrder = requestDto.FormItemOrder,
                    FormItemType = requestDto.FormItemType,
                    FormItemValue = requestDto.FormItemValue,
                    IsDirty = requestDto.IsDirty,
                    IsRequired = requestDto.IsRequired,
                    IsEnable = true,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                });
                return ResponseSuccessResult(item);
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

        [Route("area/updateitemlist")]
        [HttpPut]
        [ResponseType(typeof(List<InspectionFormItemModel>))]
        public IHttpActionResult UpdateAreaItem(InspectionFormSectionUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormItemListModel
                {
                    InspectionFormId = requestDto.InspectionFormId,
                    InspectionFormSectionId = requestDto.InspectionFormSectionId,
                    SectionAutoFail = requestDto.SectionAutoFail,
                    SectionAutoFailReason = requestDto.SectionAutoFailReason,
                    IsEnable = true
                };
                foreach (var formItem in requestDto.FormItems)
                {
                    listModel.InspectionFormItemList.Add(new InspectionFormItemModel
                    {
                        InspectionFormItemId = formItem.InspectionFormItemId,
                        IsDirty = true,
                        IsRequired = true,
                        FormItemValue = formItem.FormItemValue,
                        ModifiedDate = DateTime.Now
                    });
                }
                listModel = _inspectionService.UpdateInspectionFormItemList(listModel);
                return ResponseSuccessResult(listModel.InspectionFormItemList);
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

        [Route("history/listbyregion")]
        [HttpPost]
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult HistoryListByRegion(InspectionFormListByRegionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    RegionId = requestDto.RegionId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormHistoryListByRegion(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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

        [Route("history/listbyfranchisee")]
        [HttpPost]
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult HistoryListByFranchisee(InspectionFormListByFranchiseeRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    FranchiseeId = requestDto.FranchiseeId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormHistoryListByFranchisee(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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

        [Route("history/listbycustomer")]
        [HttpPost]
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult HistoryListByCustomer(InspectionFormListByCustomerDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    CustomerId = requestDto.CustomerId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormHistoryListByCustomer(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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

        [Route("history/listbyjob")]
        [HttpPost]
        [ResponseType(typeof(List<InspectionFormModel>))]
        public IHttpActionResult HistoryListByJob(InspectionFormListByJobDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewInspectionFormListModel
                {
                    JobId = requestDto.JobId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _inspectionService.GetInspectionFormHistoryListByJob(listModel);
                return ResponseSuccessResult(listModel.InspectionFormList);
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

        [Route("history/{inspectionid}")]
        [HttpGet]
        [ResponseType(typeof(InspectionFormModel))]
        public IHttpActionResult HistoryById(int inspectionid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspection = _inspectionService.GetInspectionFormHistory(inspectionid);
                return ResponseSuccessResult(inspection);
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

        [Route("history/area/{areaid}")]
        [HttpGet]
        [ResponseType(typeof(List<InspectionFormItemModel>))]
        public IHttpActionResult HistoryItemsByArea(int areaid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspectionFormItems = _inspectionService.GetInspectionFormItemHistoryListBySection(areaid);
                return ResponseSuccessResult(inspectionFormItems);
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

        [Route("history/area/item/{itemid}")]
        [HttpGet]
        [ResponseType(typeof(InspectionFormItemModel))]
        public IHttpActionResult HistoryAreaItem(int itemid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var inspectionFormItem = _inspectionService.GetInspectionFormItemHistory(itemid);
                return ResponseSuccessResult(inspectionFormItem);
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