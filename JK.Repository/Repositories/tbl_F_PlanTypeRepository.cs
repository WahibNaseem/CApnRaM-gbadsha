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
    public class tbl_F_PlanTypeRepository : BaseRepository<tbl_F_PlanType> , Itbl_F_PlanTypeRepository
    {
        public tbl_F_PlanTypeRepository(DbContext context) : base(context){}
    }
}
