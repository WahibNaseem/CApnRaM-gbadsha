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
    public class tbl_Sys_CountyTaxAuthorityRepository : BaseRepository<tbl_Sys_CountyTaxAuthority> , Itbl_Sys_CountyTaxAuthorityRepository
    {
        public tbl_Sys_CountyTaxAuthorityRepository(DbContext context) : base(context){}
    }
}
