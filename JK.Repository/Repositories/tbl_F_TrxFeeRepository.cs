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
    public class tbl_F_TrxFeeRepository : BaseRepository<tbl_F_TrxFee> , Itbl_F_TrxFeeRepository
    {
        public tbl_F_TrxFeeRepository(DbContext context) : base(context){}
    }
}
