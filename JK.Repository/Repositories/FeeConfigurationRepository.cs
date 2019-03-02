

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
    public class FeeConfigurationRepository : BaseRepository<FeeConfiguration>, IFeeConfigurationRepository
    {
        public FeeConfigurationRepository (DbContext context) : base(context) { }
    }
}



