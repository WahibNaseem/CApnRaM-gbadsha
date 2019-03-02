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
    public class tbl_F_NegativeDueRepository : BaseRepository<tbl_F_NegativeDue> , Itbl_F_NegativeDueRepository
    {
        public tbl_F_NegativeDueRepository(DbContext context) : base(context){}
    }
}
