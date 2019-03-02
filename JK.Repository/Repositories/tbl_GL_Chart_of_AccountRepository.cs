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
    public class tbl_GL_Chart_of_AccountRepository : BaseRepository<tbl_GL_Chart_of_Account> , Itbl_GL_Chart_of_AccountRepository
    {
        public tbl_GL_Chart_of_AccountRepository(DbContext context) : base(context){}
    }
}
