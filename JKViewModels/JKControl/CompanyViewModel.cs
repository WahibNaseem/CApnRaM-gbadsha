using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class RegionInfoViewModel : BaseModel
    {
        public int RegionId { get; set; }

        public string Name { get; set; }

        public string Acronym { get; set; }

        public int? Corporate { get; set; }

        public int? Status { get; set; }

        public int? Test { get; set; }

        public string Displayname { get; set; }

        public string ReportName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public int? RemitSameAsMain { get; set; }

        public string Phone { get; set; }

        public int? International { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string AhPhone { get; set; }

        public string Fax { get; set; }

        public string Director { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string DBname { get; set; }

        public string LockboxId { get; set; }

        public int? Number { get; set; }

    }

    public class RegionViewListModel : PaggingModel
    {
        public string SelectedRegionId { get; set; }
        public List<RegionInfoViewModel> CompnayList { get; set; }
    }
}
