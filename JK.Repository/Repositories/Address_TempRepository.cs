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
    public class Address_TempRepository : BaseRepository<Address_Temp>, IAddress_TempRepository
    {
        public Address_TempRepository(DbContext context) : base(context){ }
    }
}
