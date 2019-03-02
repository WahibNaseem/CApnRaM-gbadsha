using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI.Models
{
    public class Authorize
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string TransactionID { get; set; }
        public string CardNumber { get; set; }
        public string CardType { get; set; }
    }
}