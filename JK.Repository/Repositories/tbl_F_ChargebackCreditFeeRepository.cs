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
    public class tbl_F_ChargebackCreditFeeRepository : BaseRepository<tbl_F_ChargebackCreditFee> , Itbl_F_ChargebackCreditFeeRepository
    {
        public tbl_F_ChargebackCreditFeeRepository(DbContext context) : base(context){}
    }
}
