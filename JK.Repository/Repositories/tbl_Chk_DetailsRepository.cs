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
    public class tbl_Chk_DetailsRepository : BaseRepository<tbl_Chk_Details> , Itbl_Chk_DetailsRepository
    {
        public tbl_Chk_DetailsRepository(DbContext context) : base(context){}
    }
}
