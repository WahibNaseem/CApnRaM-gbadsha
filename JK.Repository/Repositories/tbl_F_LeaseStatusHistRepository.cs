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
    public class tbl_F_LeaseStatusHistRepository : BaseRepository<tbl_F_LeaseStatusHist> , Itbl_F_LeaseStatusHistRepository
    {
        public tbl_F_LeaseStatusHistRepository(DbContext context) : base(context){}
    }
}
