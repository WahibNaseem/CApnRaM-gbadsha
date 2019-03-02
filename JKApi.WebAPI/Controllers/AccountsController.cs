using JKApi.Service.Service;
using JKApi.WebAPI.Filters;
using JKApi.WebAPI.Mapper;
using JKApi.WebAPI.Models.Account;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JKApi.WebAPI.Controllers
{
    public class AccountsController : ApiController
    {
        private AccountService _service;
        private AccountMapper _mapper;

        public AccountsController()
        {
            _service = new AccountService();
            _mapper = new AccountMapper();
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="request">Login Request</param>
        /// <returns>Login Response</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginResponseModel))]
        [HttpPost]
        [Route("Login")]
        [ValidateModel]
        public HttpResponseMessage Login(LoginRequestModel request)
        {
            Business.Domain.User user = _service.Authenticate(request.Username, request.Password);
            LoginResponseModel response = new LoginResponseModel();
            response = (LoginResponseModel) _mapper.DomainToModel(user);
            return Request.CreateResponse(HttpStatusCode.OK, new LoginResponseModel());
        }
        
        //[HttpGet]
        //[Route("Person/all")]
        //[ValidateModel]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<UserDetailsResponseModel>))]
        //public HttpResponseMessage GetAllUsers()
        //{
            
        //}

    }
}
