using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Business.Domain
{
    public class IDomainBase
    {
        int? ID { get; set; }

        bool IsActive { get; set; }
    }
}
