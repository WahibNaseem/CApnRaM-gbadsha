using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JKApi.WebAPI.Models;
using JKViewModels;
using JKViewModels.Common;
using Newtonsoft.Json;

namespace JKApi.WebAPI.Dtos.Account
{
    public class LoginRequestModel : IModelBase
    {
        [Required]
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [Required]
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        public LoginRequestModel(bool fillDummyData)
        {
            Username = "Default";
            Password = "ldr2435jk";
        }

        public LoginRequestModel()
        {
        }
    }

    public class LoginResponseModel : IModelBase
    {
        public string Username { get; set; }
        [JsonProperty(PropertyName = "id")]
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
        public List<LoginRoleModel> Roles { get; set; }
        public List<LoginRegionModel> Regions { get; set; }

        public DetailedLoginResponseModel()
        {
            Roles = new List<LoginRoleModel>();
            Regions = new List<LoginRegionModel>();
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
            Roles = new List<LoginRoleModel>();
            Regions = new List<LoginRegionModel>();
        }

        public void UpdateRegions(List<RegionInfoViewModel> regions)
        {
            Regions.Clear();
            foreach (var region in regions)
            {
                Regions.Add(new LoginRegionModel
                {
                    RegionId = region.RegionId,
                    Name = region.Name,
                    Displayname = region.Displayname
                });
            }
        }

        public void UpdateRoles(List<RoleModel> roles)
        {
            Roles.Clear();
            foreach (var role in roles)
            {
                Roles.Add(new LoginRoleModel
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName
                });
            }
        }

        public void UpdatePeriods(List<PeriodAccessModel> periods)
        {
            foreach (var region in Regions)
            {
                region.Periods.Clear();
                foreach (var period in periods)
                {
                    if (period.RegionId == region.RegionId)
                    {
                        region.Periods.Add(new LoginPeriodModel
                        {
                            PeriodId = period.PeriodId,
                            Month = period.Month,
                            Year = period.Year
                        });
                    }    
                }
            }
        }
    }

    public class LoginRegionModel
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public string Displayname { get; set; }
        public List<LoginPeriodModel> Periods { get; set; }

        public LoginRegionModel()
        {
            Periods = new List<LoginPeriodModel>();
        }
    }

    public class LoginRoleModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class LoginPeriodModel
    {
        public int PeriodId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}