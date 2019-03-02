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
    public class tbl_F_NegativeDueCreditRepository : BaseRepository<tbl_F_NegativeDueCredit> , Itbl_F_NegativeDueCreditRepository
    {
        public tbl_F_NegativeDueCreditRepository(DbContext context) : base(context){}
    }
}
