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
    public class ValidationRepository : BaseRepository<Validation>, IValidationRepository
    {
        public ValidationRepository(DbContext context) : base(context){ }
    }
}
