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
    public class tbl_AR_PaymentChargebackRepository : BaseRepository<tbl_AR_PaymentChargeback> , Itbl_AR_PaymentChargebackRepository
    {
        public tbl_AR_PaymentChargebackRepository(DbContext context) : base(context){}
    }
}
