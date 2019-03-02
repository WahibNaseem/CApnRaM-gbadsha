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
    public class tbl_R_MonthlyDeductionsPaidRepository : BaseRepository<tbl_R_MonthlyDeductionsPaid> , Itbl_R_MonthlyDeductionsPaidRepository
    {
        public tbl_R_MonthlyDeductionsPaidRepository(DbContext context) : base(context){}
    }
}
