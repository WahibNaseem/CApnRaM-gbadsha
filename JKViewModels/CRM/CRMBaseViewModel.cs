namespace JKViewModels.CRM
{
    public class CRMBaseViewModel : BaseModel
    {
    }

    #region Enums

    public enum AccountType
    {
        Unknown = 0,
        Customer = 1,
        Franchise = 2
    }

    public enum AccountSourceProvider
    {
        Unknown,
        InHouse,
        OutSource
    }

    public enum AccountProviderType
    {
        Unknown,
        Act,
        AhlaDirectory,
        AccuData,
        AvaiatechPPC,
        BizList,
        BusinessJournalBkOfLists,
        CallIn,
        ColdCall,
        Corporate,
        CrissCross,
        Customer,
        DunnAndBradstreet,
        FranchiseAccounting,
        Franchise,
        GoLeadsCom,
        GoldMine,
        Haines,
        Hoovers,
        Imported,
        InfoUsa,
        InsideProspects,
        JaniKingVehicle,
        LeadListService,
        LocalChamber,
        Referral,
        RegionalOffice,
        SaleGenie,
        Sorkins,
        Telemarketing,
        TradeShow,
        WebSite,
        YellowPages,
        InactiveCustomers
    }

    public enum StageType
    {
        Unknown,
        Lead,
        Potential,
        Customer,
        Franchisee
    }

    public enum CallResultType
    {
        other = 1,
        appointment = 2,
        busy = 3,
        contacted = 4,
        leftmessage = 5,
        lettersent = 6,
        noanswer = 7,
        notqualified = 8,
        previouscustomer = 9,
        referredtoops = 10,
        turndownletter = 11,
        wrong = 12,
        callback = 13
    }

    public enum StageStatusType
    {
        Unknown = 0,
        NewLead = 1,
        LeadNeedsToCallOrSetupMeeting = 2,
        UnqualifiedLead = 3,
        JunkLead = 4,
        QualifiedLead = 5,
        NewPotential = 6,
        ReadyForQuotation = 7,
        QuotationSent = 8,
        Negotitation = 9,
        QuotationNotAccepted = 10,
        NeedsRequote = 11,
        QuotationAccepted = 12,
        NewCustomer = 13,
        NeedFollowup = 14,
        InactiveCustomer = 15,
        IntialCommunication = 16,
        NeedAssessment = 17,
        Prestentation = 18,
        Commettobuy = 19,
        Potential = 20,
        FvPresentation = 21,
        Bidding = 22,
        PdAppointment = 23,
        FollowUp = 24,
        Close = 25,
        CallBack = 26,
        LeadGeneration = 27,
        FranchiseContract = 28,
        SignAgreement = 29,
        Sold = 30,
        PotentialInquary = 31,
        SoldToLegal = 32

    }

    public enum LeadAccountType
    {
        Unknown,
        Restaurant,
        RetirementCenter,
        SchoolKThru12,
        ShoppingCenter,
        ShowRoom,
        SkatingRink,
        Spa,
        SportsStadium,
        StadiumEventsOther,
        StorageUnits,
        Surgery,
        Technology,
        TelephoneCompany,
        TheaterArts,
        TimeShare,
        TrainingCenters,
        TransportationCo,
        University,
        Retail
    }

    public enum Title
    {
        Dr,
        Ms,
        Mr,
        Mrs,
        Prof
    }

    public enum Purpose
    {
        CallBack,
        Meeting
    }

    #endregion
}

