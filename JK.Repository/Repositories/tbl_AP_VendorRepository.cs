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
    public class tbl_AP_VendorRepository : BaseRepository<tbl_AP_Vendor> , Itbl_AP_VendorRepository
    {
        public tbl_AP_VendorRepository(DbContext context) : base(context){}
    }
}
