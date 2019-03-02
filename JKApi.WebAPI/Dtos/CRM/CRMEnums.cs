using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI
{
    public class CRMEnums
    {

        public enum State
        {
            [Description("Alabama")]
            AL,

            [Description("Alaska")]
            AK,

            [Description("Arkansas")]
            AR,

            [Description("Arizona")]
            AZ,

            [Description("California")]
            CA,

            [Description("Colorado")]
            CO,

            [Description("Connecticut")]
            CT,

            [Description("D.C.")]
            DC,

            [Description("Delaware")]
            DE,

            [Description("Florida")]
            FL,

            [Description("Georgia")]
            GA,

            [Description("Hawaii")]
            HI,

            [Description("Iowa")]
            IA,

            [Description("Idaho")]
            ID,

            [Description("Illinois")]
            IL,

            [Description("Indiana")]
            IN,

            [Description("Kansas")]
            KS,

            [Description("Kentucky")]
            KY,

            [Description("Louisiana")]
            LA,

            [Description("Massachusetts")]
            MA,

            [Description("Maryland")]
            MD,

            [Description("Maine")]
            ME,

            [Description("Michigan")]
            MI,

            [Description("Minnesota")]
            MN,

            [Description("Missouri")]
            MO,

            [Description("Mississippi")]
            MS,

            [Description("Montana")]
            MT,

            [Description("North Carolina")]
            NC,

            [Description("North Dakota")]
            ND,

            [Description("Nebraska")]
            NE,

            [Description("New Hampshire")]
            NH,

            [Description("New Jersey")]
            NJ,

            [Description("New Mexico")]
            NM,

            [Description("Nevada")]
            NV,

            [Description("New York")]
            NY,

            [Description("Oklahoma")]
            OK,

            [Description("Ohio")]
            OH,

            [Description("Oregon")]
            OR,

            [Description("Pennsylvania")]
            PA,

            [Description("Rhode Island")]
            RI,

            [Description("South Carolina")]
            SC,

            [Description("South Dakota")]
            SD,

            [Description("Tennessee")]
            TN,

            [Description("Texas")]
            TX,

            [Description("Utah")]
            UT,

            [Description("Virginia")]
            VA,

            [Description("Vermont")]
            VT,

            [Description("Washington")]
            WA,

            [Description("Wisconsin")]
            WI,

            [Description("West Virginia")]
            WV,

            [Description("Wyoming")]
            WY
        }

        public enum Title
        {
            Dr,
            Ms,
            Mr,
            Mrs,
            Prof
        }

        public enum CallTime
        {
            am = 1,
            pm = 0
        }


        public enum ScheduleLeadPurpose
        {
            [Description("First Visit")]
            FirstVisit = 1,

            [Description("Proposal Delivery")]
            ProposalDelivery = 2,

            [Description("Proposal Follow up")]
            ProposalFollowUp = 3,

            [Description("Walk Thru")]
            WalkThru = 4,

            [Description("Call Back")]
            CallBack = 5,

            [Description("Cold Call")]
            ColdCall = 6,

            [Description("Prospecting Email")]
            ProspectingEmail = 7,

            [Description("Past Proposal")]
            PastProposal = 8,

            [Description("Follow Up")]
            FollowUp = 9

        }

        public enum SchedulePurpose
        {
            [Description("First Visit")]
            FirstVisit = 1,

            [Description("Proposal Delivery")]
            ProposalDelivery = 2,

            [Description("Proposal Follow up")]
            ProposalFollowUp = 3,

            [Description("Walk Thru")]
            WalkThru = 4,

            [Description("Call Back")]
            CallBack = 5,

            [Description("Cold Call")]
            ColdCall = 6,

            [Description("Prospecting Email")]
            ProspectingEmail = 7,

            [Description("Past Proposal")]
            PastProposal = 8,

            [Description("Follow Up")]
            FollowUp = 9,

            [Description("QualifyingLead")]
            QualifyingLead = 10,

            [Description("Meeting")]
            Meeting = 11,

            [Description("Other")]
            Other = 12
        }

        public enum JopType
        {
            [Description("Full Time")]
            fullTime = 1,
            [Description("Part Time")]
            partTime = 2
        }
    }
}