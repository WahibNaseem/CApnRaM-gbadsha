//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JKApi.Data.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CRMContact
    {
        public int CRMContactId { get; set; }
        public int CRMContactTypeId { get; set; }
        public int UserId { get; set; }
        public int RegionId { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public string Jobtitle { get; set; }
        public string FileAs { get; set; }
        public string Email { get; set; }
        public string DisplayAs { get; set; }
        public string WebPageAddress { get; set; }
        public string IMAddress { get; set; }
        public string BusinessPhone { get; set; }
        public string HomePhone { get; set; }
        public string BusinessFaxPhone { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessAddress { get; set; }
        public bool IsMailingAddress { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual CRMContactType CRMContactType { get; set; }
        public virtual Region Region { get; set; }
    }
}
