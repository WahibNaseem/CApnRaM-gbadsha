using System.ComponentModel.DataAnnotations;
using JKApi.WebAPI.Models;
using Newtonsoft.Json;

namespace JKApi.WebAPI.Dtos.CRM
{

    public class CheckCompanynameExitsRequestModel : IModelBase
    {

        [Required]
        [JsonProperty(PropertyName = "CompanyName")]
        public string CompanyName { get; set; }


        [Required]
        [JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }

        public string SelectedRegionId { get;set;}
    }

    public class CheckCompanynameExitsResponseModel : IModelBase
    {

    }

}