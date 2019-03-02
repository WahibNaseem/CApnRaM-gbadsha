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
    public class tbl_F_ChargebackTaxRepository : BaseRepository<tbl_F_ChargebackTax> , Itbl_F_ChargebackTaxRepository
    {
        public tbl_F_ChargebackTaxRepository(DbContext context) : base(context){}
    }
}
