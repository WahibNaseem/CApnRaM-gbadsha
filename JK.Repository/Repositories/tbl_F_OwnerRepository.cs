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
    public class tbl_F_OwnerRepository : BaseRepository<tbl_F_Owner> , Itbl_F_OwnerRepository
    {
        public tbl_F_OwnerRepository(DbContext context) : base(context){}
    }
}
