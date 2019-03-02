using JK.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace JK.Repository.Repositories
{
    public class AuthDepartmentRepository : BaseRepository<JKApi.Data.DAL.AuthDepartment>, IAuthDepartmentRepository    
    {
        public AuthDepartmentRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
