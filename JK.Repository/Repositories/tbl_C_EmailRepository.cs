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
    public class tbl_C_EmailRepository : BaseRepository<tbl_C_Email> , Itbl_C_EmailRepository
    {
        public tbl_C_EmailRepository(DbContext context) : base(context){}
    }
}
