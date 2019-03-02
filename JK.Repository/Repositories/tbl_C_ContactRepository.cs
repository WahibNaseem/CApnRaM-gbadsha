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
    public class tbl_C_ContactRepository : BaseRepository<tbl_C_Contact> , Itbl_C_ContactRepository
    {
        public tbl_C_ContactRepository(DbContext context) : base(context){}
    }
}
