using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JKApi.Data.DAL;

namespace JK.FMS.MVC.Areas.Portal.Models
{
    public class FranchiseeFeeDefinition
    {
        public int FeeRateType { get; set; }
        public decimal Amount { get; set; }
        public int FeeId { get; set; }

        public static FranchiseeFeeDefinition FromFeeData(FranchiseeFee fee)
        {
            FranchiseeFeeDefinition def = new FranchiseeFeeDefinition();
            def.FeeRateType = (int)fee.FeeRateType;
            def.Amount = (decimal)fee.Amount;
            def.FeeId = (int)fee.FeesId;
            return def;
        }

        public static FranchiseeFeeDefinition FromFeeData(DistributionFee fee)
        {
            FranchiseeFeeDefinition def = new FranchiseeFeeDefinition();
            def.FeeRateType = (int)fee.FeeRateTypeListId;
            def.Amount = (decimal)fee.Amount;
            def.FeeId = Convert.ToInt32(fee.FeeId);
            return def;
        }
    }
}