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
    public class tbl_F_NoteStatusHistRepository : BaseRepository<tbl_F_NoteStatusHist> , Itbl_F_NoteStatusHistRepository
    {
        public tbl_F_NoteStatusHistRepository(DbContext context) : base(context){}
    }
}
