using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.Service.Service.Inspection;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos;
using JKApi.WebAPI.Filters;
using JKViewModels;
using Microsoft.Web.Http;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/template")]
    [Authorized]
    public class TemplateController : BaseApiController
    {
        private readonly ITemplateService _templateService;

        // ======================================================================================
        #region TemplateController > Constructor
        // ======================================================================================

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        #endregion
        // ======================================================================================
        #region TemplateController > Private

        private TemplateAreaResponseDto _buildTemplateAreaDtoFromModel(TemplateAreaModel model)
        {
            if (model == null) return null;
            var templateAreaDto = ConvertModelToDto<TemplateAreaResponseDto>(model);
            return templateAreaDto;
        }

        private TemplateAreaItemResponseDto _buildTemplateAreaItemFromModel(TemplateAreaItemModel model)
        {
            if (model == null) return null;
            var templateAreaItemDto = ConvertModelToDto<TemplateAreaItemResponseDto>(model);
            return templateAreaItemDto;
        }

        #endregion
        // ======================================================================================
        #region TemplateController > API Calls

        [Route(template: "arealist")]
        [HttpGet]
        [ResponseType(typeof(List<TemplateAreaResponseDto>))]
        public IHttpActionResult TemplateAreaList()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var responseDtos = new List<TemplateAreaResponseDto>();
                var models = _templateService.GetTemplateAreaList();
                foreach (var model in models)
                {
                    var responseDto = _buildTemplateAreaDtoFromModel(model);
                    if (responseDto != null)
                    {
                        responseDtos.Add(responseDto);
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

        [Route(template: "itemlist")]
        [HttpGet]
        [ResponseType(typeof(List<TemplateAreaItemResponseDto>))]
        public IHttpActionResult TemplateAreaItemList()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var responseDtos = new List<TemplateAreaItemResponseDto>();
                var models = _templateService.GetTemplateAreaItemList();
                foreach (var model in models)
                {
                    var responseDto = _buildTemplateAreaItemFromModel(model);
                    if (responseDto != null)
                    {
                        responseDtos.Add(responseDto);
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

        #endregion
        // ======================================================================================
    }
}
