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
    
    public partial class NotificationMessageForDashboard
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string EntrySource { get; set; }
        public Nullable<int> MID { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> HeaderId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> NotificationTypeListId { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
    }
}
