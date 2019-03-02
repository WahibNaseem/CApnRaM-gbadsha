using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JKViewModels.Management
{
    public class StarecapViewModel
    {
        [DisplayName("Bill Month/Year")]
        [Required(ErrorMessage = "Month is Required")]
        public string billMonth { get; set; }
        [DisplayName("Year")]
        [Required(ErrorMessage = "{0} is Required")]
        public string billYear { get; set; }
    }

    public class StarecapListItem
    {
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Balance { get; set; }
        public string grossrevenue { get; set; }
        public string groupby { get; set; }
        public string orderby { get; set; }
    }
}
