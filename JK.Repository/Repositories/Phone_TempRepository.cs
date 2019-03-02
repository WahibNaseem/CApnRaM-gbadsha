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
    public class Phone_TempRepository : BaseRepository<Phone_Temp>, IPhone_TempRepository
    {
        public Phone_TempRepository(DbContext context) : base(context){ }
    }
}
