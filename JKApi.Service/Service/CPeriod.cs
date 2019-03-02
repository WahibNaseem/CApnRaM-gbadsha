using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Service;
using JKApi.Service.ServiceContract;

namespace JKApi.Service
{
    class CPeriod
    {
       
        int _CPeriodId;
        public int CPeriodId
        {
            get
            {
                return _CPeriodId;
            }

            set
            {
                _CPeriodId = value;
            }
        }


        int _CRegionId;
        public int CRegionId
        {
            get
            {
                return _CRegionId;
            }

            set
            {
                _CRegionId = value;
            }
        }

        
    }
}
