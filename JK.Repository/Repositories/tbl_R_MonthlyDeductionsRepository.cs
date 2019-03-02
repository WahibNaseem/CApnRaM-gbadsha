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
    public class tbl_R_MonthlyDeductionsRepository : BaseRepository<tbl_R_MonthlyDeductions> , Itbl_R_MonthlyDeductionsRepository
    {
        public tbl_R_MonthlyDeductionsRepository(DbContext context) : base(context){}
    }
}
