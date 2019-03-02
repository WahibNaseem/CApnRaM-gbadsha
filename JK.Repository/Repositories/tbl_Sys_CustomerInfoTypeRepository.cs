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
    public class tbl_Sys_CustomerInfoTypeRepository : BaseRepository<tbl_Sys_CustomerInfoType> , Itbl_Sys_CustomerInfoTypeRepository
    {
        public tbl_Sys_CustomerInfoTypeRepository(DbContext context) : base(context){}
    }
}
