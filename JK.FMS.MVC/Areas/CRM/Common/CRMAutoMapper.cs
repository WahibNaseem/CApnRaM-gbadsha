using AutoMapper;
using JKApi.Data.DAL;
using JKViewModels;
using JKViewModels.CRM;
using System.Collections.Generic;

namespace JK.FMS.MVC.Areas.CRM.Common
{
    public static class CRMAutoMapper
    {
        public static List<CRMAccountViewModel> CrmAccountEntitiesToViewModels(List<CRM_Account> accountEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Account, CRMAccountViewModel>();
            });
            var mapper = config.CreateMapper();
            var accountViewModels = mapper.Map<IEnumerable<CRM_Account>, IEnumerable<CRMAccountViewModel>>(accountEntities) as List<CRMAccountViewModel>;
            return accountViewModels;
        }

        public static CRMAccountViewModel CrmAccountEntityToViewModel(CRM_Account accountEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Account, CRMAccountViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Account, CRMAccountViewModel>(accountEntity);
        }

        public static CRM_Account CrmAccountViewModelToEntity(CRMAccountViewModel accountViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMAccountViewModel, CRM_Account>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMAccountViewModel, CRM_Account>(accountViewModel);
        }

        public static CRMAccountCustomerDetailViewModel CrmAccountCustomerEntityToViewModel(CRM_AccountCustomerDetail accountCustomerDetailEntity)
        {
            var config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<CRM_AccountCustomerDetail, CRMAccountCustomerDetailViewModel>();
             });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_AccountCustomerDetail, CRMAccountCustomerDetailViewModel>(accountCustomerDetailEntity);
        }  
       
        public static CRM_AccountCustomerDetail CrmAccountCustomerViewModelToEntity(CRMAccountCustomerDetailViewModel accountCustomerDetailViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMAccountCustomerDetailViewModel, CRM_AccountCustomerDetail>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMAccountCustomerDetailViewModel, CRM_AccountCustomerDetail>(accountCustomerDetailViewModel);
        }

        public static CRMAccountFranchiseDetailViewModel CrmAccountFranchiseDetailEntityToViewModel(CRM_AccountFranchiseDetail accountFranchiseDetailEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_AccountFranchiseDetail, CRMAccountFranchiseDetailViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_AccountFranchiseDetail, CRMAccountFranchiseDetailViewModel>(accountFranchiseDetailEntity);
            
        }  

        public static List<CRMActivityViewModel> CrmActivityEntitiesToViewModels(List<CRM_Activity> activityEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Activity, CRMActivityViewModel>();
            });
            var mapper = config.CreateMapper();
            var activityViewModels = mapper.Map<IEnumerable<CRM_Activity>, IEnumerable<CRMActivityViewModel>>(activityEntities) as List<CRMActivityViewModel>;
            return activityViewModels;
        }

        public static List<CRMDocumentViewModel> CrmDocumentEntitiesToViewModels(List<CRM_Document> documentEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Document, CRMDocumentViewModel>();
            });
            var mapper = config.CreateMapper();
            var documentViewModels = mapper.Map<IEnumerable<CRM_Document>, IEnumerable<CRMDocumentViewModel>>(documentEntities) as List<CRMDocumentViewModel>;
            return documentViewModels;
        }

        public static List<CRMInitialCommunicationViewModel> CrmInitialEntitiesToViewModel(List<CRM_InitialCommunication> initialEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_InitialCommunication, CRMInitialCommunicationViewModel>();
            });
            var mapper = config.CreateMapper();
            var initialViewModels = mapper.Map<IEnumerable<CRM_InitialCommunication>, IEnumerable<CRMInitialCommunicationViewModel>>(initialEntities) as List<CRMInitialCommunicationViewModel>;
            return initialViewModels;
        }

        public static List<CRMFvPresentationViewModel> CrmfvPresentationEntitiesToViewModels(List<CRM_FvPresentation> fvpresentationEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FvPresentation, CRMFvPresentationViewModel>();
            });
            var mapper = config.CreateMapper();
            var fvpresentationViewModels = mapper.Map<IEnumerable<CRM_FvPresentation>, IEnumerable<CRMFvPresentationViewModel>>(fvpresentationEntities) as List<CRMFvPresentationViewModel>;
            return fvpresentationViewModels;
        }

        public static List<CRMBiddingViewModel> CrmBiddingEntityToViewModel(List<CRM_Bidding> biddingEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Bidding, CRMBiddingViewModel>();
            });
            var mapper = config.CreateMapper();
            var biddingViewModels = mapper.Map<IEnumerable<CRM_Bidding>, IEnumerable<CRMBiddingViewModel>>(biddingEntities) as List<CRMBiddingViewModel>;
            return biddingViewModels;
        }

        public static List<CRMNoteViewModel> CrmNoteEntitiesToViewModels(List<CRM_Note> noteEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Note, CRMNoteViewModel>();
            });
            var mapper = config.CreateMapper();
            var activityViewModels = mapper.Map<IEnumerable<CRM_Note>, IEnumerable<CRMNoteViewModel>>(noteEntities) as List<CRMNoteViewModel>;
            return activityViewModels;
        }

        public static List<CRMScheduleViewModel> CrmScheduleEntitiesToViewModels(List<CRM_Schedule> scheduleEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Schedule, CRMScheduleViewModel>();
            });
            var mapper = config.CreateMapper();
            var scheduleViewModels = mapper.Map<IEnumerable<CRM_Schedule>, IEnumerable<CRMScheduleViewModel>>(scheduleEntities) as List<CRMScheduleViewModel>;
            return scheduleViewModels;

        }

        public static List<CRMStageStatusScheduleViewModel> CrmStageStatusScheduleEntitiesToViewModels(List<CRM_StageStatusSchedule> stagestatusscheduleEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_StageStatusSchedule, CRMStageStatusScheduleViewModel>();
            });
            var mapper = config.CreateMapper();
            var scheduleViewModels = mapper.Map<IEnumerable<CRM_StageStatusSchedule>, IEnumerable<CRMStageStatusScheduleViewModel>>(stagestatusscheduleEntities) as List<CRMStageStatusScheduleViewModel>;
            return scheduleViewModels;

        }

        public static CRM_Schedule CrmScheduleViewModelToEntity(CRMScheduleViewModel scheduleViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMScheduleViewModel, CRM_Schedule>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMScheduleViewModel, CRM_Schedule>(scheduleViewModel);
        }

        public static CRM_Note CrmNoteViewModelToEntity(CRMNoteViewModel noteViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMNoteViewModel, CRM_Note>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMNoteViewModel, CRM_Note>(noteViewModel);
        } 

        public static CRMNoteViewModel CrmNoteEntityToViewModel(CRM_Note noteEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Note, CRMNoteViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Note, CRMNoteViewModel>(noteEntity);
        } 

        public static CRMScheduleViewModel CrmScheduleEntityToViewModel(CRM_Schedule scheduleEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Schedule, CRMScheduleViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Schedule, CRMScheduleViewModel>(scheduleEntity);
        }

        public static CRM_Activity CrmActivityEntityToViewModel(CRMActivityViewModel activityViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMActivityViewModel, CRM_Activity>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMActivityViewModel, CRM_Activity>(activityViewModel);
        } 

        public static CRMActivityViewModel CrmActivityViewModelToEntity(CRM_Activity activityEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Activity, CRMActivityViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Activity, CRMActivityViewModel>(activityEntity);
        }

        public static CRM_InitialCommunication CrmInitialViewModelToEntity(CRMInitialCommunicationViewModel initialViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMInitialCommunicationViewModel, CRM_InitialCommunication>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMInitialCommunicationViewModel, CRM_InitialCommunication>(initialViewModel);
        }

        public static CRM_FvPresentation CrmFvPresentationViewModelToEntity(CRMFvPresentationViewModel fvpresentationViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMFvPresentationViewModel, CRM_FvPresentation>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMFvPresentationViewModel, CRM_FvPresentation>(fvpresentationViewModel);
        }

        public static CRMFvPresentationViewModel CrmFvPresentationEntityToViewModel(CRM_FvPresentation fvpresentationEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FvPresentation, CRMFvPresentationViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_FvPresentation, CRMFvPresentationViewModel>(fvpresentationEntity);
        }

        public static CRM_Bidding CrmBiddingViewModelToEntity(CRMBiddingViewModel biddingViewViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMBiddingViewModel, CRM_Bidding>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMBiddingViewModel, CRM_Bidding>(biddingViewViewModel);
        }

        public static CRMBiddingViewModel CrmBiddingEntityToViewModel(CRM_Bidding biddingEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Bidding, CRMBiddingViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Bidding, CRMBiddingViewModel>(biddingEntity);
        }

        public static CRMInitialCommunicationViewModel CrmInitialEntityToViewModel(CRM_InitialCommunication initialEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_InitialCommunication, CRMInitialCommunicationViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_InitialCommunication, CRMInitialCommunicationViewModel>(initialEntity);
        }

        public static CRM_PdAppointment CrmPdAppointmentViewModelToEntity(CRMPdAppointmentViewModel pdViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMPdAppointmentViewModel, CRM_PdAppointment>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMPdAppointmentViewModel, CRM_PdAppointment>(pdViewModel);
        }

        public static CRMPdAppointmentViewModel CrmPdAppointmentEntityToViewModel(CRM_PdAppointment pdEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_PdAppointment, CRMPdAppointmentViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_PdAppointment, CRMPdAppointmentViewModel>(pdEntity);
        }

        public static CRM_FollowUp CrmFollowupViewModelToEntity(CRMFollowUpViewModel followViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMFollowUpViewModel, CRM_FollowUp>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMFollowUpViewModel, CRM_FollowUp>(followViewModel);
        }

        public static CRMFollowUpViewModel CrmFollowupEntityToViewModel(CRM_FollowUp followEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FollowUp, CRMFollowUpViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_FollowUp, CRMFollowUpViewModel>(followEntity);
        }
        
        public static CRM_StageStatusSchedule CrmStageStatusScheduleViewModelToEntity(CRMStageStatusScheduleViewModel stagestatusscheduleViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMStageStatusScheduleViewModel, CRM_StageStatusSchedule>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMStageStatusScheduleViewModel, CRM_StageStatusSchedule>(stagestatusscheduleViewModel);
        }

        public static CRMStageStatusScheduleViewModel CrmStageStatusScheduleEntityToViewModel(CRM_StageStatusSchedule stagestatusscheduleEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_StageStatusSchedule, CRMStageStatusScheduleViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_StageStatusSchedule, CRMStageStatusScheduleViewModel>(stagestatusscheduleEntity);
        }

        public static CRM_Document CrmDocumentViewModelToEntity(CRMDocumentViewModel documentViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMDocumentViewModel, CRM_Document>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMDocumentViewModel, CRM_Document>(documentViewModel);
        }

        public static CRMDocumentViewModel CrmDocumentEntityToViewModel(CRM_Document documentEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Document, CRMDocumentViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Document, CRMDocumentViewModel>(documentEntity);
        }

        public static List<CRMDocumentViewModel> CrmDocumentsEntityToViewModel(List<CRM_Document> documentEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Document, CRMDocumentViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map< IEnumerable<CRM_Document>, IEnumerable<CRMDocumentViewModel>>(documentEntities) as List<CRMDocumentViewModel>;
        }

        public static CRM_LeadGeneration CrmLeadGenViewModelToEntity(CRMLeadGenerationViewModel leadgenViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMLeadGenerationViewModel, CRM_LeadGeneration>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMLeadGenerationViewModel, CRM_LeadGeneration>(leadgenViewModel);
        }

        public static CRM_FranchiseContract CrmFranchiseContractViewModelToEntity(CRMFranchiseContractViewModel franchisecontractViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMFranchiseContractViewModel, CRM_FranchiseContract>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMFranchiseContractViewModel, CRM_FranchiseContract>(franchisecontractViewModel);
        }

        public static CRMLeadGenerationViewModel CrmLeadGenEntityToViewModel(CRM_LeadGeneration leadgenEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_LeadGeneration, CRMLeadGenerationViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_LeadGeneration, CRMLeadGenerationViewModel>(leadgenEntity);
        }

        public static CRMFranchiseContractViewModel CrmFranchiseContractEntityToViewModel(CRM_FranchiseContract franchiseContractEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FranchiseContract, CRMFranchiseContractViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_FranchiseContract, CRMFranchiseContractViewModel>(franchiseContractEntity);
        }

        public static CRM_Contact CrmContactViewModelToEntity(CRMContactViewModel crmContactViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMContactViewModel, CRM_Contact>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMContactViewModel, CRM_Contact>(crmContactViewModel);
        }

        public static CRMContactViewModel CrmContactEntityToViewModel(CRM_Contact crmContactEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Contact, CRMContactViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Contact, CRMContactViewModel>(crmContactEntity);
        }

        public static CRMCloseViewModel CrmCloseEntityToViewModel(CRM_Close  crmCloseEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Close, CRMCloseViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Close, CRMCloseViewModel>(crmCloseEntity);
        } 
        public static CRM_Close CrmCloseViewModelToEntity(CRMCloseViewModel crmCloseViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMCloseViewModel, CRM_Close>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMCloseViewModel, CRM_Close>(crmCloseViewModel);
        }

        public static CRMFranchiseFollowUpViewModel CrmFranchiseFollowUpEntityToViewModel(CRM_FranchiseFollowUp crmFranchiseFollowUpEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FranchiseFollowUp, CRMFranchiseFollowUpViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_FranchiseFollowUp, CRMFranchiseFollowUpViewModel>(crmFranchiseFollowUpEntity);
        }

        public static CRM_FranchiseFollowUp CrmFranchiseFollowUpViewModelToEntity(CRMFranchiseFollowUpViewModel crmFranchiseFollowUpViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMFranchiseFollowUpViewModel, CRM_FranchiseFollowUp>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMFranchiseFollowUpViewModel, CRM_FranchiseFollowUp>(crmFranchiseFollowUpViewModel);
        }

        public static CRMSignAgreementViewModel CrmSignAgreementEntityToViewModel(CRM_SignAgreement crmSignAgreementEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_SignAgreement, CRMSignAgreementViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_SignAgreement, CRMSignAgreementViewModel>(crmSignAgreementEntity);
        }

        public static CRM_SignAgreement CrmSignAgreementViewModelToEntity(CRMSignAgreementViewModel crmSignAgreementViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMSignAgreementViewModel, CRM_SignAgreement>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMSignAgreementViewModel, CRM_SignAgreement>(crmSignAgreementViewModel);
        }

        public static CRMCallLogViewModel CrmCallLogEntityToViewModel(CRM_CallLog crmCallLogViewEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_CallLog, CRMCallLogViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_CallLog, CRMCallLogViewModel>(crmCallLogViewEntity);
        }

        public static CRM_CallLog CrmCallLogViewModelToEntity(CRMCallLogViewModel crmCallLogViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMCallLogViewModel, CRM_CallLog>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMCallLogViewModel, CRM_CallLog>(crmCallLogViewModel);
        }
    }

}