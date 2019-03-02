using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class AccountTypeListRepository : BaseRepository<AccountTypeList> , IAccountTypeListRepository
    {
        public AccountTypeListRepository(DbContext context) : base(context){}
    }
}
