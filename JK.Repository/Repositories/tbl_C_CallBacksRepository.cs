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
    public class tbl_C_CallBacksRepository : BaseRepository<tbl_C_CallBacks> , Itbl_C_CallBacksRepository
    {
        public tbl_C_CallBacksRepository(DbContext context) : base(context){}
    }
}
