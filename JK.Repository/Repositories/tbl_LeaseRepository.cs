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
    public class tbl_LeaseRepository : BaseRepository<tbl_Lease> , Itbl_LeaseRepository
    {
        public tbl_LeaseRepository(DbContext context) : base(context){}
    }
}
