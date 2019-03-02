using System;
using JKApi.Business.Domain;
using JKApi.WebAPI.Dtos.Account;
using JKApi.WebAPI.Models;

namespace JKApi.WebAPI.Mapper
{
    public class AccountMapper : IMapper
    {
        public IModelBase DomainToModel(IDomainBase domainObj)
        {
            if (domainObj == null)
                return null;
            if (!(domainObj is User))
                throw new Exception();
            var user = domainObj as User;
            var loginResponseModel = new LoginResponseModel
            {
                Id = user.ID.Value,
                Username = user.Username,
                ApiKey = user.Password
            };
            return loginResponseModel;
        }

        public IDomainBase ModelToDomain(IModelBase modelObj)
        {
            if (modelObj == null)
                return null;
            if (!(modelObj is LoginResponseModel))
                throw new Exception(); //TODO: Update this for better error handling
            LoginResponseModel loginResponseModel = modelObj as LoginResponseModel;
            User user = new User();
            user.Username = loginResponseModel.Username;
            user.Password = loginResponseModel.ApiKey;
            user.ID = loginResponseModel.Id;
            return user;
        }
    }
}