using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data.DTOObject
{
    public class MstrRegionCollection
    {
        public FranchiseeFeeList FranchiseeFeeList { get; set; }

        public FeeRateTypeList FeeRateTypeList { get; set; }
    }

    public class MstrFeeListCollection
    {
        public FranchiseeFee Fee { get; set; }

        public FranchiseeFeeList FranchiseeFeeList { get; set; }

        public FeeRateTypeList FeeRateTypeList { get; set; }
       
    }

    public class MstrFeeListCollection_Temp
    {
        public FranchiseeFee_Temp Fee_Temp { get; set; }

        public FranchiseeFeeList FranchiseeFeeList { get; set; }

        public FeeRateTypeList FeeRateTypeList { get; set; }        
    }
}
