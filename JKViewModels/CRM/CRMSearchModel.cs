using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class CRMSearchModel
    {
        public CRMLeadSearchModel CrmLeadSearch { get; set; }
        public List<SearchFieldModel> SearchField { get; set; }
    }
    public class SearchFieldModel
    {
        public string SearchField { get; set; }
        public int Operation { get; set; }
        public string Text { get; set; }
        public bool Condition { get; set; }
    }
    public class CRMLeadSearchModel
    {
        public int AccountType { get; set; }
        public int Territory { get; set; }
        public int Source { get; set; }
        public int Status { get; set; }
        public string SortBy { get; set; }
        public bool LeadOnly { get; set; }
        public bool IncludeCallBackPending { get; set; }
        public bool Exception { get; set; } 
    }
}
