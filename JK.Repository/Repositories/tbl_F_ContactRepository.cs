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
    public class tbl_F_ContactRepository : BaseRepository<tbl_F_Contact> , Itbl_F_ContactRepository
    {
        public tbl_F_ContactRepository(DbContext context) : base(context){}
    }
}
