using JKApi.Service.Helper;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using JKViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Service.Helper.Extension;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Franchisee;
using JK.Repository.Uow;
using JKApi.Data.DTOObject;
using JKViewModels.Franchisee;
using JKApi.Service.ServiceContract.JKControl;
using JKViewModels.Administration.Region;

namespace JKApi.Service.Service.Region
{
    public class RegionService : BaseService, IRegionService
    {


        #region ConstructorCalls

        public RegionService(IJKEfUow uow)
        {
            Uow = uow;
        }

        public RegionService()
        {
        }

        #endregion

        #region Service Calls


        #region Region
        public IEnumerable<mstrRegion> GetmstrRegion()
        {
            using (var context = new JKControlMasterEntities())
            {
                var result = context.mstrRegions.ToList();
                return result;
            }

            //var qry = _uow.mstrRegion.GetAll();
            //return qry;
        }

        public mstrRegion GetmstrRegionById(int id)
        {
            return Uow.mstrRegion.GetById(id);
        }

        public mstrRegion SavemstrRegion(mstrRegion mstrRegion)
        {

            var ID = mstrRegion.RegionId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.mstrRegion.Add(mstrRegion);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.mstrRegion.Update(mstrRegion);
                Uow.Commit();
            }

            return mstrRegion;
        }

        public mstrRegion DeletemstrRegion(int id)
        {
            var entity = Uow.mstrRegion.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.mstrRegion.Update(entity);
            Uow.Commit();
            return entity;
        }

        //IQueryable<mstrRegion> IRegionService.GetmstrRegion()
        //{
        //    throw new NotImplementedException();
        //}







        #endregion
































        #endregion






    }
}