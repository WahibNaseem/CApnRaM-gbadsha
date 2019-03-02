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
    public class tbl_C_AddressRepository : BaseRepository<tbl_C_Address> , Itbl_C_AddressRepository
    {
        public tbl_C_AddressRepository(DbContext context) : base(context){}
    }
}
