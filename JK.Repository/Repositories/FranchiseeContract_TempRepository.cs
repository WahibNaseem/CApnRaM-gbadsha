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
    public class FranchiseeContract_TempRepository : BaseRepository<FranchiseeContract_Temp>, IFranchiseeContract_TempRepository
    {
        public FranchiseeContract_TempRepository(DbContext context) : base(context){ }
    }
}
