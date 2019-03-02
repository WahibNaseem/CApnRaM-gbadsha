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
    public class Franchisee_TempRepository : BaseRepository<Franchisee_Temp>, IFranchisee_TempRepository
    {
        public Franchisee_TempRepository(DbContext context) : base(context){ }
    }
}
