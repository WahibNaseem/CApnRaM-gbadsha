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
    public class FeeConfiguration_TempRepository : BaseRepository<FeeConfiguration_Temp>, IFeeConfiguration_TempRepository
    {
        public FeeConfiguration_TempRepository(DbContext context) : base(context) { }
    }
}
