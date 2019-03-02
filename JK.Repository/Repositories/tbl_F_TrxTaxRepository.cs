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
    public class tbl_F_TrxTaxRepository : BaseRepository<tbl_F_TrxTax> , Itbl_F_TrxTaxRepository
    {
        public tbl_F_TrxTaxRepository(DbContext context) : base(context){}
    }
}
