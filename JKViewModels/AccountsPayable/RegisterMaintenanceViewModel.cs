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
    public class RegisterMaintenanceViewModel
    {
        [DisplayName("Types")]
        [Required(ErrorMessage = "{0} is Required")]
        public string Type { get; set; }
        [DisplayName("Adjustment")]
        public bool Adjustment { get; set; }
        [DisplayName("Date")]
        [Required(ErrorMessage = "{0} is Required")]
        public string Date { get; set; }
        [DisplayName("Amount")]
        [Required(ErrorMessage = "{0} is Required")]
        public string Amount { get; set; }
        [DisplayName("Comments")]
        public string Comments { get; set; }
    }
}
