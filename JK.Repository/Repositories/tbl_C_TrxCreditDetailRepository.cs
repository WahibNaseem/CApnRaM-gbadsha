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
    public class tbl_C_TrxCreditDetailRepository : BaseRepository<tbl_C_TrxCreditDetail> , Itbl_C_TrxCreditDetailRepository
    {
        public tbl_C_TrxCreditDetailRepository(DbContext context) : base(context){}
    }
}
