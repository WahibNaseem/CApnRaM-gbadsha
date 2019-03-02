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
    public class PaymentRepository : BaseRepository<Payment> , IPaymentRepository
    {
        public PaymentRepository(DbContext context) : base(context){}
    }
}
