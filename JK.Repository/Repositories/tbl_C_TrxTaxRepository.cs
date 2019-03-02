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
    public class tbl_C_TrxTaxRepository : BaseRepository<tbl_C_TrxTax> , Itbl_C_TrxTaxRepository
    {
        public tbl_C_TrxTaxRepository(DbContext context) : base(context){}
    }
}
