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
    public class MasterTrxStatusListRepository : BaseRepository<MasterTrxStatusList> , IMasterTrxStatusListRepository
    {
        public MasterTrxStatusListRepository(DbContext context) : base(context){}
    }
}
