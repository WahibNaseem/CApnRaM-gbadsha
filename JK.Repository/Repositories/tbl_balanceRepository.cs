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
    public class tbl_balanceRepository : BaseRepository<tbl_balance> , Itbl_balanceRepository
    {
        public tbl_balanceRepository(DbContext context) : base(context){}
    }
}
