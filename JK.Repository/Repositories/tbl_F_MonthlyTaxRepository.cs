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
    public class tbl_F_MonthlyTaxRepository : BaseRepository<tbl_F_MonthlyTax> , Itbl_F_MonthlyTaxRepository
    {
        public tbl_F_MonthlyTaxRepository(DbContext context) : base(context){}
    }
}
