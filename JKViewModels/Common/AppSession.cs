using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Common
{
    public class AppSession
    {
        public UserViewModel UserInfo { get; set; }

        public AppSession()
        {
            UserInfo = new UserViewModel();
        }
    }
}
