using JK.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class AuthUserLoginRepository : BaseRepository<JKApi.Data.DAL.AuthUserLogin>, IAuthUserLoginRepository
    {
        public AuthUserLoginRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
