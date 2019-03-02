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
    public class tbl_F_ChargebackFeeRepository : BaseRepository<tbl_F_ChargebackFee> , Itbl_F_ChargebackFeeRepository
    {
        public tbl_F_ChargebackFeeRepository(DbContext context) : base(context){}
    }
}
