using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerUploadDocumentViewModel
    {
        public int FiletypeListId { get; set; }
        public string FiletypeListName { get; set; }
        public Nullable<int> UploadDocumentId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExt { get; set; }
        public Nullable<int> FileSize { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }

        public bool IsViewToFranchisee { get; set; }

        public Nullable<bool> IsCustomerRequired { get; set; }
        public Nullable<int> CustomerOrderBy { get; set; }
        public Nullable<bool> IsFranchiseeRequired { get; set; }
        public Nullable<int> FranchiseeOrderBy { get; set; }
    }
}
