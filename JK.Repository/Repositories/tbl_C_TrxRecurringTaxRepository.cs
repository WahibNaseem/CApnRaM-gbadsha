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
    public class tbl_C_TrxRecurringTaxRepository : BaseRepository<tbl_C_TrxRecurringTax> , Itbl_C_TrxRecurringTaxRepository
    {
        public tbl_C_TrxRecurringTaxRepository(DbContext context) : base(context){}
    }
}
