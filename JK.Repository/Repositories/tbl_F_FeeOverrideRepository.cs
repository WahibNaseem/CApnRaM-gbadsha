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
    public class tbl_F_FeeOverrideRepository : BaseRepository<tbl_F_FeeOverride> , Itbl_F_FeeOverrideRepository
    {
        public tbl_F_FeeOverrideRepository(DbContext context) : base(context){}
    }
}
