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
    public class tbl_C_PhoneRepository : BaseRepository<tbl_C_Phone> , Itbl_C_PhoneRepository
    {
        public tbl_C_PhoneRepository(DbContext context) : base(context){}
    }
}
