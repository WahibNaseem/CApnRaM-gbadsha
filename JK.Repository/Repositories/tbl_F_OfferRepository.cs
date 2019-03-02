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
    public class tbl_F_OfferRepository : BaseRepository<tbl_F_Offer> , Itbl_F_OfferRepository
    {
        public tbl_F_OfferRepository(DbContext context) : base(context){}
    }
}
