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
    public class tbl_C_CustomerJKUser_refRepository : BaseRepository<tbl_C_CustomerJKUser_ref> , Itbl_C_CustomerJKUser_refRepository
    {
        public tbl_C_CustomerJKUser_refRepository(DbContext context) : base(context){}
    }
}
