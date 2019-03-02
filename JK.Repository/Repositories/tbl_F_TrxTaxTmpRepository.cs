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
    public class tbl_F_TrxTaxTmpRepository : BaseRepository<tbl_F_TrxTaxTmp> , Itbl_F_TrxTaxTmpRepository
    {
        public tbl_F_TrxTaxTmpRepository(DbContext context) : base(context){}
    }
}
