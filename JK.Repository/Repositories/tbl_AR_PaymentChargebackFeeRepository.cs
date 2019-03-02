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
    public class tbl_AR_PaymentChargebackFeeRepository : BaseRepository<tbl_AR_PaymentChargebackFee> , Itbl_AR_PaymentChargebackFeeRepository
    {
        public tbl_AR_PaymentChargebackFeeRepository(DbContext context) : base(context){}
    }
}
