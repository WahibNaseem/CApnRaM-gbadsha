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
    public class tbl_R_MonthlyTaxRepository : BaseRepository<tbl_R_MonthlyTax> , Itbl_R_MonthlyTaxRepository
    {
        public tbl_R_MonthlyTaxRepository(DbContext context) : base(context){}
    }
}
