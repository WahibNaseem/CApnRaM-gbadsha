using System.Collections;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using JKApi.Core;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos;

namespace JKApi.WebAPI.Controllers
{
    /// <summary>
    /// Base Api Controller
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public abstract class BaseApiController : ApiController
    {
        #region BaseApiController > Properties

        /// <summary>
        /// NLogger
        /// </summary>
        protected NLogger NLogger = NLogger.Instance;

        /// <summary>
        /// CacheProvider
        /// </summary>
        protected CacheProvider CacheProvider = CacheProvider.Instance;

        /// <summary>
        /// ContainerResponseModel
        /// </summary>
        protected IContainerResponseDto ContainerResponseDto;

        #endregion

        #region BaseApiController > Constructor

        /// <summary>
        /// Construct a new BaseApiController.
        /// </summary>
        protected BaseApiController()
        {
            ContainerResponseDto = new ContainerResponseDto();
        }

        #endregion

        #region BaseApiController > Common Methods

        /// <summary>
        /// Generate a success response.
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        protected IHttpActionResult ResponseSuccessResult(object data)
        {
            ContainerResponseDto.IsSuccess = true;
            ContainerResponseDto.Data = data;
            return Ok(ContainerResponseDto);
        } 
        protected IHttpActionResult ResponseSuccessResult(object data, ApiException.ErrorCode errorCode, string Message)
        {
            ContainerResponseDto.IsSuccess = true;
            ContainerResponseDto.Data = data;
            ContainerResponseDto.MessageCode = errorCode;
            ContainerResponseDto.Message = Message;
            return Ok(ContainerResponseDto);
        }

        /// <summary>
        /// Generate an error response.
        /// </summary>
        /// <param name="errorCode">Error Code</param>
        /// <returns></returns>
        protected IHttpActionResult ResponseErrorResult(ApiException.ErrorCode errorCode)
        {
            ContainerResponseDto.IsSuccess = false;
            ContainerResponseDto.MessageCode = errorCode;
            ContainerResponseDto.Message = ApiException.GetDescription(errorCode);
            return Ok(ContainerResponseDto);
        }

        /// <summary>
        /// Generate an error response.
        /// </summary>
        /// <param name="errorCode">Error Code</param>
        /// <param name="message">Error Message</param>
        /// <returns></returns>
        protected IHttpActionResult ResponseErrorResult(ApiException.ErrorCode errorCode, string message)
        {
            ContainerResponseDto.IsSuccess = false;
            ContainerResponseDto.MessageCode = errorCode;
            ContainerResponseDto.Message = message;
            return Ok(ContainerResponseDto);
        }

        /// <summary>
        /// Response for invalid model state such as requested dto is not what the API defines. 
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        protected IHttpActionResult ResponseForInvalidModelState()
        {
            var errors = ApiException.GetErrors(ModelState);
            NLogger.Error($"Request Model is invalid: {string.Join(", ", errors.ToArray())}");
            return ResponseErrorResult(ApiException.ErrorCode.InvalidRequest, string.Join(", ", errors.ToArray()));
        }

        /// <summary>
        /// Convert a model to web service dto object.
        /// </summary>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T ConvertModelToDto<T>(object model) where T : new()
        {
            if (model == null) return default(T);

            var propertyName = string.Empty;
            var dto = new T();
            var dtoProperties = dto.GetType().GetProperties();
            foreach (var dtoProperty in dtoProperties)
            {
                var modelProperty = model.GetType().GetProperty(dtoProperty.Name);
                if (modelProperty != null)
                {
                    // We do not process nesting
                    if (modelProperty.PropertyType.FullName != null &&
                        (modelProperty.PropertyType.IsClass && !modelProperty.PropertyType.FullName.StartsWith("System.")))
                    {
                        continue;
                    }

                    var value = modelProperty.GetValue(model, null);
                    if (!(value is IList) || !value.GetType().IsGenericType)
                    {
                        dtoProperty.SetValue(dto, value, null);
                    }
                }
            }
            return dto;
        }

        #endregion
    }
}
