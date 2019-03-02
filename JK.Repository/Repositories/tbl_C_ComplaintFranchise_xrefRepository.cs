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
    public class tbl_C_ComplaintFranchise_xrefRepository : BaseRepository<tbl_C_ComplaintFranchise_xref> , Itbl_C_ComplaintFranchise_xrefRepository
    {
        public tbl_C_ComplaintFranchise_xrefRepository(DbContext context) : base(context){}
    }
}
