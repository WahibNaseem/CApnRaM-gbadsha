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
    public class tbl_AR_PaymentChargebackDistributionRepository : BaseRepository<tbl_AR_PaymentChargebackDistribution> , Itbl_AR_PaymentChargebackDistributionRepository
    {
        public tbl_AR_PaymentChargebackDistributionRepository(DbContext context) : base(context){}
    }
}
