using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Models.Account;
using JKApi.Service;
using Microsoft.Web.Http;
using JKApi.Core;
using JKApi.Core.EncryptDecrypt;
using JKApi.WebAPI.Dtos.Account;
using JKViewModels;
using JKViewModels.Common;
using Newtonsoft.Json;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/authentication")]
    [AllowAnonymous]
    public class AuthenticationController : BaseApiController
    {
        private readonly IUserService _userService;

        // ======================================================================================
        #region AuthenticationController > Constructor
        // ======================================================================================

        /// <summary>
        /// Construct with dependencies.
        /// </summary>
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion
        // ======================================================================================
        #region AuthenticationController > Private Helpers
        // ======================================================================================

        private string _getClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            return null;
        }

        #endregion
        // ======================================================================================
        #region AuthenticationController > API Calls
        // ======================================================================================

        [Route("login")]
        [HttpPost]
        [ResponseType(typeof(LoginResponseModel))]
        public IHttpActionResult Login(LoginRequestModel requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var encryptDecrypt = new EncryptDecrypt();

                var userModel = _userService.Login(new UserLoginViewModel
                {
                    Username = requestDto.Username,
                    Password = requestDto.Password,
                    IPAddress = _getClientIp()
                });

                if (userModel.objErrorModel.Count > 0)
                {
                    return ResponseErrorResult(ApiException.ErrorCode.FailExecute, userModel.objErrorModel[0].ErrorMessage);
                }

                var loginModel = new LoginResponseModel
                {
                    Username = requestDto.Username,
                    Id = userModel.UserId,
                    ApiKey = encryptDecrypt.Encrypt(userModel.UserId.ToString()),
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    Phone = userModel.Phone,
                    DefaultRegionId = userModel.DefaultRegionId,
                    LoginDateTime = DateTime.Now,
                    ProfileId = userModel.UserId
                };
                loginModel.Token = encryptDecrypt.Encrypt(JsonConvert.SerializeObject(loginModel));
                var responseModel = new DetailedLoginResponseModel(loginModel)
                {
                    Address = new AddressModel { Address1 = userModel.Addres, City = userModel.City, ZipCode = userModel.Zipcode }
                };
                responseModel.UpdateRegions(userModel.Regions);
                responseModel.UpdateRoles(userModel.Roles);
                responseModel.UpdatePeriods(userModel.lstPeriodAccess);

                return ResponseSuccessResult(responseModel);
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