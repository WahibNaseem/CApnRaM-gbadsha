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
    public class tbl_F_LeaseRepository : BaseRepository<tbl_F_Lease> , Itbl_F_LeaseRepository
    {
        public tbl_F_LeaseRepository(DbContext context) : base(context){}
    }
}
