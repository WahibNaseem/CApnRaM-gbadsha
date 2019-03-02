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
    public class tbl_F_ChargebackCreditRepository : BaseRepository<tbl_F_ChargebackCredit> , Itbl_F_ChargebackCreditRepository
    {
        public tbl_F_ChargebackCreditRepository(DbContext context) : base(context){}
    }
}
