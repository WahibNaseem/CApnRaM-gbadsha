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
    public class tbl_ContactCommRepository : BaseRepository<tbl_ContactComm> , Itbl_ContactCommRepository
    {
        public tbl_ContactCommRepository(DbContext context) : base(context){}
    }
}
