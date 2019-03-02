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
    public class tbl_Sys_LettersRepository : BaseRepository<tbl_Sys_Letters> , Itbl_Sys_LettersRepository
    {
        public tbl_Sys_LettersRepository(DbContext context) : base(context){}
    }
}
