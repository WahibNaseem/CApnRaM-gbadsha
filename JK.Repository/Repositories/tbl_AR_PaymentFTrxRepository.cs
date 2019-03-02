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
    public class tbl_AR_PaymentFTrxRepository : BaseRepository<tbl_AR_PaymentFTrx> , Itbl_AR_PaymentFTrxRepository
    {
        public tbl_AR_PaymentFTrxRepository(DbContext context) : base(context){}
    }
}
