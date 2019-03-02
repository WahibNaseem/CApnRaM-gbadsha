using System;
using System.Collections.Generic;

namespace JKViewModels
{
    public class BaseModel
    {
        public BaseModel()
        {
            objErrorModel = new List<ErrorModel>();
        }

        public bool IsEnable { get; set; }
        public bool? IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public List<ErrorModel> objErrorModel { get; set; }
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public int CustomerId { get; set; }
        public int JobId { get; set; }
    }

    public class BaseEntityModel : IDisposable
    {
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsEnable { get; set; }
        public bool IsDelete { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public void Dispose()
        {
            // NO IMPLEMENTATION
        }
    }

    public class ErrorMessage
    {
        public ErrorMessage()
        {
            Messages = new List<ErrorModel>();
        }
        public List<ErrorModel> Messages { get; set; }
    }

    public class ErrorModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ViewBaseModel
    {
        public ViewBaseModel()
        {
            objErrorModel = new List<ErrorModel>();
        }
        public string Message { get; set; }
        public string MessageType { get; set; }

        public List<ErrorModel> objErrorModel { get; set; }

    }

    public class PaggingModel : BaseModel
    {
        public PaggingModel()
        {
            CurrentPage = 1;
            PageSize = 100;
        }
      
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string PagingPrefix { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; } 
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public string SearchBy { get; set; }

        public bool SelectedId { get; set; }
    }

}
