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
    public class tbl_ContactRepository : BaseRepository<tbl_Contact> , Itbl_ContactRepository
    {
        public tbl_ContactRepository(DbContext context) : base(context){}
    }
}
