using System;
using System.Collections.Generic;
using JKViewModels;
using JKViewModels.Common;

namespace JKApi.WebAPI.Dtos
{
    public class AccountWalkThruItemListByCustomerRequestDto : IRequestDto
    {
        public int CustomerId { get; set; }
    }

    public class AccountWalkThruUpdateRequestDto : IRequestDto
    {
        public int AccountWalkThruItemId { get; set; }
        public AccountWalkThruType AccountWalkThruType { get; set; }
        public int CustomerId { get; set; }
        public int FranchiseeId { get; set; }
        public bool FieldValue { get; set; }
        public string FieldText { get; set; }
        public string FileUrl { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class AccountWalkThruCheckListUpdateRequestDto : IRequestDto
    {
        public DateTime? WalkThruDate { get; set; }
        public IList<AccountWalkThruUpdateRequestDto> CheckList { get; set; }

        public AccountWalkThruCheckListUpdateRequestDto()
        {
            CheckList = new List<AccountWalkThruUpdateRequestDto>();
        }
        
    }
}