using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.Region
{
   public class MstrRegionViewModel
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DBConnectionString { get; set; }
        public string LogoBase64 { get; set; }
        public string ActionURL { get; set; }
        public Nullable<Guid> RegionUniqueID { get; set; }
        public Nullable<bool> IsEnable { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public string DNS { get; set; }
        public string Server { get; set; }
        public string DataPath { get; set; }
        public string DataFolder { get; set; }
        public string RegionNumber { get; set; }
        public string Director { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string AhPhone { get; set; }
        public string Fax { get; set; }
        public string Corpregion { get; set; }
        public string International { get; set; }
        public string Email { get; set; }
        public string WebAddress { get; set; }
        public string Image { get; set; }
        public string TestData { get; set; }
        public string WebAccess { get; set; }
        public string ApdataPath { get; set; }
        public string MacAddress { get; set; }
        public string IPAddress { get; set; }
        public string AcctServer { get; set; }
        public string AcctPWD { get; set; }
        public string AcctDB { get; set; }
        public string Redirect { get; set; }
        public string Country { get; set; }
        public string Active { get; set; }
        public string DSN { get; set; }

    }
}
