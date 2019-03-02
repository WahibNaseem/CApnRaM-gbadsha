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
    public class tbl_C_TrxRecurringFeeRepository : BaseRepository<tbl_C_TrxRecurringFee> , Itbl_C_TrxRecurringFeeRepository
    {
        public tbl_C_TrxRecurringFeeRepository(DbContext context) : base(context){}
    }
}
