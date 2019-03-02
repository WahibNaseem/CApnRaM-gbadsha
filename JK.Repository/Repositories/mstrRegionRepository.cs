using JK.Repository.Contracts;
using JKApi.Data.DAL;
using JKApi.Data.MasterDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JK.Repository.Repositories
{
    public class mstrRegionRepository : BaseRepository<mstrRegion> , ImstrRegionRepository
    {
        public mstrRegionRepository(DbContext context) : base(context){}
    }
}
