using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JKViewModels.AccountsPayable
{
    public class CheckGenerateViewModel
    {
        [DisplayName("Select")]
        [Required(ErrorMessage = "Required")]
        public string generateSelect { get; set; }
        [DisplayName("Date")]
        [Required(ErrorMessage = "{0} is Required")]
        public string date { get; set; }
    }
}
