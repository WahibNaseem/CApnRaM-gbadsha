using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CommonRevenueDistributionDetailViewModel
    {
        public int CustomerId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int StatusListId { get; set; }
        public int StatusReasonListId { get; set; }

        public string RecordType { get; set; }//TRANS/NEW/INVD
        public int? DistributionId { get; set; }
        public int ContractDetailId { get; set; }
        public int FranchiseeId { get; set; }
        public decimal Amount { get; set; }
        public decimal? FeeAmount { get; set; }
        public decimal? TotalAmount { get; set; }

        //FiderFee Detail
        public DateTime? FFStopDate { get; set; }
        public string FFNotes { get; set; }

        //Trouble Fee Detail
        public bool? ApplyTroubleWithFee { get; set; }
        public decimal? ApplyTroubleWithFeeAmount { get; set; }
        public bool? ApplyTroubleWithoutFee { get; set; }
        public bool? ApplyNotATroubleAccoun { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }




    public class CommonRevenueDistributionFeeDetailViewModel
    {
        public string RecordType { get; set; }//TRANS/NEW/INVD
        public int ContractDetailId { get; set; }
        public int FranchiseeId { get; set; }
        public int FeeId { get; set; }
        public decimal Amount { get; set; }

    }


}
