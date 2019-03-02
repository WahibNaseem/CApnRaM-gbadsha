namespace JKViewModels.CRM
{
    using System;
    using System.Web;

    public class CRMDocumentViewModel
    {
        public int CRM_DocumentId { get; set; } 
        public int CRM_AccountId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public string File_Title { get; set; }
        public string File_Name { get; set; }
        public string Description { get; set; }
        public bool IsWorkBook { get; set; }
        public string Document_FilePath { get; set; }
        public HttpPostedFileBase document { get; set; }
        public string Content_Type { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        

        public int? FileTypeListId { get; set; }

        public string FiletypeListName { get; set; }

        public int RegionId { get; set; }

        public bool IsViewToFranchisee { get; set; }

        public Nullable<bool> IsCustomerRequired { get; set; }
        public Nullable<int> CustomerOrderBy { get; set; }
        public Nullable<bool> IsFranchiseeRequired { get; set; }
        public Nullable<int> FranchiseeOrderBy { get; set; }

    }
}
