using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JK.FMS.MVC.Areas.CRM.Common
{
    public class DataTableParams
    {
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iColumns { get; set; }
        public string sSearch { get; set; }
        public bool bEscapeRegex { get; set; }
        public int iSortingCols { get; set; }
        public int sEcho { get; set; }
        public List<string> sColumnsNames { get; set; }
        public List<bool> bSortable { get; set; }
        public List<bool> bSearchable { get; set; }
        public List<string> sSearchValues { get; set; }
        public List<int> iSortCol { get; set; }
        public List<string> sSortDir { get; set; }
        public List<bool> bEscapeRegexColumns { get; set; }

        public DataTableParams()
        {
            sColumnsNames = new List<string>();
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchValues = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();
        }
    }
}