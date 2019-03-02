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
    public class tbl_F_CustomerxrefRepository : BaseRepository<tbl_F_Customerxref> , Itbl_F_CustomerxrefRepository
    {
        public tbl_F_CustomerxrefRepository(DbContext context) : base(context){}
    }
}
