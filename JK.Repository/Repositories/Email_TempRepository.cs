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
    public class Email_TempRepository : BaseRepository<Email_Temp>, IEmail_TempRepository
    {
        public Email_TempRepository(DbContext context) : base(context){ }

    }
}
