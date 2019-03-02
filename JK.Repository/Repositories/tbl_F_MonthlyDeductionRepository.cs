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
    public class tbl_F_MonthlyDeductionRepository : BaseRepository<tbl_F_MonthlyDeduction> , Itbl_F_MonthlyDeductionRepository
    {
        public tbl_F_MonthlyDeductionRepository(DbContext context) : base(context){}
    }
}
