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
    public class tbl_C_noteRepository : BaseRepository<tbl_C_note> , Itbl_C_noteRepository
    {
        public tbl_C_noteRepository(DbContext context) : base(context){}
    }
}
