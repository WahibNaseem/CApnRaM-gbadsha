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
    public class tbl_ContactTypeRepository : BaseRepository<tbl_ContactType> , Itbl_ContactTypeRepository
    {
        public tbl_ContactTypeRepository(DbContext context) : base(context){}
    }
}
