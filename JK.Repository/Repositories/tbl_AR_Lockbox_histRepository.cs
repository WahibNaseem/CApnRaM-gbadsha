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
    public class tbl_AR_Lockbox_histRepository : BaseRepository<tbl_AR_Lockbox_hist> , Itbl_AR_Lockbox_histRepository
    {
        public tbl_AR_Lockbox_histRepository(DbContext context) : base(context){}
    }
}
