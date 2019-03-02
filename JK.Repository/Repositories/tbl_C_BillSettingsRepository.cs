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
    public class tbl_C_BillSettingsRepository : BaseRepository<tbl_C_BillSettings> , Itbl_C_BillSettingsRepository
    {
        public tbl_C_BillSettingsRepository(DbContext context) : base(context){}
    }
}
