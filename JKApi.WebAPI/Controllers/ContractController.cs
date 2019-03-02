using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.Service.Service.FOM;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos;
using JKApi.WebAPI.Filters;
using JKViewModels.Contract;
using Microsoft.Web.Http;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/contract")]
    [Authorized]
    public class ContractController : BaseApiController
    {
        private readonly IContractService _contractService;

        // ======================================================================================
        #region ContractController > Constructor
        // ======================================================================================

        /// <summary>
        /// Contract an OperationControll with dependency injections.
        /// </summary>
        /// <param name="contractService"></param>
        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        #endregion
        // ======================================================================================
        #region ContractController > Private Helpers
        // ======================================================================================

        private ContractResponseDto _buildContractDtoFromModel(ContractModel model)
        {
            if (model == null) return null;
            var contractDto = ConvertModelToDto<ContractResponseDto>(model);
            contractDto.Address = ConvertModelToDto<AddressResponseDto>(model.Address);
            return contractDto;
        }

        #endregion
        // ======================================================================================
        #region ContractController > API Calls
        // ======================================================================================

        /// <summary>
        /// Get the list of contracts by franchisee id.
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [Route(template: "listbyfranchisee")]
        [HttpPost]
        [ResponseType(typeof(IList<ContractResponseDto>))]
        public IHttpActionResult ListByFranchisee(ContractByFranchiseeRequestDto requestDto)
        {
            // Check the request dto
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            // Build the response
            try
            {
                var responseDtos = new List<ContractResponseDto>();
                var contractModels = _contractService.GetContractsByFranchisee(requestDto.FranchiseeId);
                foreach (var contractModel in contractModels)
                {
                    var contractDto = _buildContractDtoFromModel(contractModel);
                    if (contractDto != null)
                    {
                        responseDtos.Add(contractDto);
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
