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
    public class tbl_F_NoteRepository : BaseRepository<tbl_F_Note> , Itbl_F_NoteRepository
    {
        public tbl_F_NoteRepository(DbContext context) : base(context){}
    }
}
