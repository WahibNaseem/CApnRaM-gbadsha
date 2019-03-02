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
    public class tbl_AddressRepository : BaseRepository<tbl_Address> , Itbl_AddressRepository
    {
        public tbl_AddressRepository(DbContext context) : base(context){}
    }
}
