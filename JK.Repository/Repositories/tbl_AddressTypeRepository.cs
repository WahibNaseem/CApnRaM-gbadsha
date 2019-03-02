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
    public class tbl_AddressTypeRepository : BaseRepository<tbl_AddressType> , Itbl_AddressTypeRepository
    {
        public tbl_AddressTypeRepository(DbContext context) : base(context){}
    }
}
