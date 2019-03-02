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
    public class tbl_Chk_ManualRepository : BaseRepository<tbl_Chk_Manual> , Itbl_Chk_ManualRepository
    {
        public tbl_Chk_ManualRepository(DbContext context) : base(context){}
    }
}
