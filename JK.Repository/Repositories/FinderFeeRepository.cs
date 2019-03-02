using JK.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class FinderFeeRepository : BaseRepository<FindersFee>, IFinderFeeRepository
    {
        public FinderFeeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
