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
    public class tbl_AR_CreditFTrxRepository : BaseRepository<tbl_AR_CreditFTrx> , Itbl_AR_CreditFTrxRepository
    {
        public tbl_AR_CreditFTrxRepository(DbContext context) : base(context){}
    }
}
