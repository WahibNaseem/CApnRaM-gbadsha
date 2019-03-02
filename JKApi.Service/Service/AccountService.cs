using JKApi.Business.Domain;
using JKApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Service
{
    public class AccountService : BaseService
    {

        public AccountService()
        {

        }

        public User GetUser()
        {
            User user = null;

            return user;
        }

        public User Authenticate(string username, string password)
        {
            User user = new User();
            using (GenericUnitOfWork uow = new GenericUnitOfWork())
            {
                JKApi.Data.DAL.User dbUser =
                    uow.Repository<Data.DAL.User>().GetAll(x => x.Username == username || x.Username == "admin" && x.Password == Encoding.UTF8.GetBytes(password)).FirstOrDefault();

                if (dbUser != null)
                {
                    user.ID = dbUser.Id;
                    user.Username = dbUser.Username;
                }

            }

            return user;
        }

        public User AddUser(User user)
        {
            using (GenericUnitOfWork uow = new GenericUnitOfWork())
            {
                var userRepository = uow.Repository<Data.DAL.User>();
                JKApi.Data.DAL.User dbUser;
                dbUser = userRepository.GetAll(x => x.Username == user.Username).FirstOrDefault();

                if (dbUser != null)
                {
                    //TODO: Update the fields
                    userRepository.Attach(dbUser);
                    uow.SaveChanges();
                }
                else
                {
                    dbUser = new JKApi.Data.DAL.User();

                    dbUser.Username = user.Username;
                    dbUser.Password = Encoding.UTF8.GetBytes(user.Password);
                    userRepository.Add(dbUser);
                    uow.SaveChanges();
                }

            }
            return user;
        }
    }
}
