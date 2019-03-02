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
    public class tbl_C_TrxFeeRepository : BaseRepository<tbl_C_TrxFee> , Itbl_C_TrxFeeRepository
    {
        public tbl_C_TrxFeeRepository(DbContext context) : base(context){}
    }
}
