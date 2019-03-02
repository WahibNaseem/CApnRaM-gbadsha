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
    public class tbl_GL_FinancialTransactionRepository : BaseRepository<tbl_GL_FinancialTransaction> , Itbl_GL_FinancialTransactionRepository
    {
        public tbl_GL_FinancialTransactionRepository(DbContext context) : base(context){}
    }
}
