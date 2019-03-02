using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JKViewModels.CRM
{
   public class CRMCloseTempDocumentViewModel
    {
        public int CRM_CloseTempDocumentId { get; set; }
        public int? CRM_AccountCustomerDetailId { get; set; }
        public int? TypeListId { get; set; }
        public int? FileTypeListId { get; set; }
        public int? RegionId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
