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
    public class tbl_C_TrxDescriptionRepository : BaseRepository<tbl_C_TrxDescription> , Itbl_C_TrxDescriptionRepository
    {
        public tbl_C_TrxDescriptionRepository(DbContext context) : base(context){}
    }
}
