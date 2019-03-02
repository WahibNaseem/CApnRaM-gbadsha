using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JKViewModels;
using JKViewModels.Common;

namespace JK.FMS.MVC.Models.Users
{
    public class AuthDto
    {
    }


    public class LoginResponseModel 
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int DefaultRegionId { get; set; }
        public DateTime LoginDateTime { get; set; }
        public string Token { get; set; }
        public int ProfileId { get; set; }
    }

    public class DetailedLoginResponseModel : LoginResponseModel
    {
        public AddressModel Address { get; set; }
        public List<RoleModel> Roles { get; set; }
        public List<RegionInfoViewModel> Regions { get; set; }

        public DetailedLoginResponseModel()
        {
            Roles = new List<RoleModel>();
            Regions = new List<RegionInfoViewModel>();
        }

        public DetailedLoginResponseModel(LoginResponseModel loginModel)
        {
            Username = loginModel.Username;
            Id = loginModel.Id;
            ApiKey = loginModel.ApiKey;
            FirstName = loginModel.FirstName;
            LastName = loginModel.LastName;
            Email = loginModel.Email;
            Phone = loginModel.Phone;
            DefaultRegionId = loginModel.DefaultRegionId;
            LoginDateTime = loginModel.LoginDateTime;
            Token = loginModel.Token;
            ProfileId = loginModel.ProfileId;
            Roles = new List<RoleModel>();
            Regions = new List<RegionInfoViewModel>();
        }
    }
}