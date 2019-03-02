using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class UserLoginViewModel
    {
        //[RegularExpression(RegularExpression.EMAIL_PATTERN, ErrorMessageResourceName = RegularExpression.EMAIL_INVALID, ErrorMessageResourceType = typeof(RegularExpressionResource))]
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string IPAddress { get; set; }

        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }
    }
}
