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
    public class tbl_Chk_HeaderRepository : BaseRepository<tbl_Chk_Header> , Itbl_Chk_HeaderRepository
    {
        public tbl_Chk_HeaderRepository(DbContext context) : base(context){}
    }
}
