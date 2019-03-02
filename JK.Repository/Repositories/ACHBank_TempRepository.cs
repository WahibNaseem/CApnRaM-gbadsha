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
    public class ACHBank_TempRepository : BaseRepository<ACHBank_Temp>, IACHBank_TempRepository
    {
        public ACHBank_TempRepository(DbContext context) : base(context){ }
    }
}
