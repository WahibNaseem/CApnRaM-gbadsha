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
    public class FranchiseeFee_TempRepository : BaseRepository<FranchiseeFee_Temp>, IFranchiseeFee_TempRepository
    {
        public FranchiseeFee_TempRepository(DbContext context) : base(context){ }
    }
}
