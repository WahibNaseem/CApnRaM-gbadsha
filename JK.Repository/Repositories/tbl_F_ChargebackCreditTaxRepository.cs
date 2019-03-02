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
    public class tbl_F_ChargebackCreditTaxRepository : BaseRepository<tbl_F_ChargebackCreditTax> , Itbl_F_ChargebackCreditTaxRepository
    {
        public tbl_F_ChargebackCreditTaxRepository(DbContext context) : base(context){}
    }
}
