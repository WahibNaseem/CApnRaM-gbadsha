
using System;
using System.Collections.Generic;
using JKViewModels.Common;

namespace JKViewModels.Customer
{
    public class AccountWalkThruFormModel : BaseEntityModel
    {
        public IList<AccountWalkThruItemModel> LocationCheckList { get; set; }
        public IList<AccountWalkThruItemModel> SecurityCheckList { get; set; }
        public IList<AccountWalkThruItemModel> PaperworkCheckList { get; set; }
        public DateTime? WalkThruDate { get; set; }

        public AccountWalkThruFormModel()
        {
            LocationCheckList = new List<AccountWalkThruItemModel>();
            SecurityCheckList = new List<AccountWalkThruItemModel>();
            PaperworkCheckList = new List<AccountWalkThruItemModel>();
        }

        public AccountWalkThruFormModel(IEnumerable<AccountWalkThruItemModel> items)
        {
            LocationCheckList = new List<AccountWalkThruItemModel>();
            SecurityCheckList = new List<AccountWalkThruItemModel>();
            PaperworkCheckList = new List<AccountWalkThruItemModel>();

            foreach (var item in items)
            {
                if (WalkThruDate == null)
                {
                    WalkThruDate = item.ModifiedDate;
                    IsEnable = true;
                    IsDelete = false;
                    CreatedBy = item.CreatedBy;
                    CreatedDate = item.CreatedDate;
                    ModifiedBy = item.ModifiedBy;
                    ModifiedDate = item.ModifiedDate;
                }
                switch (item.AccountWalkThruType)
                {
                    case AccountWalkThruType.LightSwitches:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.BreakerPanel:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.ContactOffice:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.StorageArea:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.WaterSource:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.TrashDisposal:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.Recycling:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.AccountSupplies:
                        LocationCheckList.Add(item);
                        break;
                    case AccountWalkThruType.Entry:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.AlarmSystem:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.RestroomPaperDispeners:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.SecurityProcedures:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.EmergencyNamesPhones1:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.EmergencyNamesPhones2:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.ProblemConcernComments:
                        SecurityCheckList.Add(item);
                        break;
                    case AccountWalkThruType.SignedByMaintenance:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.SignedPricePage:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.SignedCleaningSchedule:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.AnalysisOfAccount:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.AccountBidSheet:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.UploadDocument:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.EmailToCustomerService:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.BusinessCardAttached:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.FranchiseAccounting:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.AeSignatures:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.OpSignatures:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.RdSignatures:
                        PaperworkCheckList.Add(item);
                        break;
                    case AccountWalkThruType.FoSignatures:
                        PaperworkCheckList.Add(item);
                        break;
                }
            }
        }
    }

    public class AccountWalkThruItemModel : BaseEntityModel
    {
        public int AccountWalkThruItemId { get; set; }
        public AccountWalkThruType AccountWalkThruType { get; set; }
        public int CustomerId { get; set; }
        public int FranchiseeId { get; set; }
        public string Title { get; set; }
        public bool FieldValue { get; set; }
        public string FieldText { get; set; }
        public string FileUrl { get; set; }
    }
}
