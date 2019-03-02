using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JK.Repository.Repositories
{
    public class tbl_F_MonthlyDeductionDetailsRepository : BaseRepository<tbl_F_MonthlyDeductionDetails> , Itbl_F_MonthlyDeductionDetailsRepository
    {
        public tbl_F_MonthlyDeductionDetailsRepository(DbContext context) : base(context){}
    }
}
