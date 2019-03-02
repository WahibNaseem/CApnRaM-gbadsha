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
    public class tbl_AR_PaymentChargebackTaxRepository : BaseRepository<tbl_AR_PaymentChargebackTax> , Itbl_AR_PaymentChargebackTaxRepository
    {
        public tbl_AR_PaymentChargebackTaxRepository(DbContext context) : base(context){}
    }
}
