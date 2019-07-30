using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Common
{
    public class Constants
    {
        public enum JobType
        {
            Quote =1,
            Do = 2,
            Support = 3,
            [Display(Name = "Non Revenue")]
            NonRevenue = 4,
            [Display(Name = "Call Back")]
            CallBack = 5
        }
        
        public enum JobStatus
        {
            Created = 1,
            [Display(Name = "Not Created")]
            NotCreated = 2,
            Booked = 3,
            [Display(Name = "Not Booked")]
            NotBooked = 4,
            Assigned = 5,
            [Display(Name = "Not Assigned")]
            NotAssigned = 6,
            Confirmed = 7,
            [Display(Name = "Not Confirmed")]
            NotConfirmed = 8,
            [Display(Name = "En-Route")]
            EnRoute = 9,
            [Display(Name = "Not En-Route")]
            NotEnRoute = 10,
            [Display(Name = "In-Progress")]
            InProgress = 11,
            [Display(Name = "Not In-Progress")]
            NotInProgress = 12,
            [Display(Name = "On-Hold")]
            OnHold = 13,
            [Display(Name = "Not On-Hold")]
            NotOnHold = 14,
            Completed = 15,
            [Display(Name = "Not Completed")]
            NotCompleted = 16
        }

        public enum PreferredTime
        {
            Anytime = 1,
            [Display(Name = "Early Morning")]
            EarlyMorning = 2,
            Morning = 3,
            Noon = 4,
            Afternoon = 5,
            [Display(Name ="Late Afternoon")]
            LateAfternoon = 6,
            Evening = 7
        }

        private static readonly ReadOnlyCollection<string> _jobperform =
        new ReadOnlyCollection<string>(new[]
        {
            "Lunch",
            "Personal",
            "Travelling",
            "ByAdmin",
            "JobPause",
            "End",
            "JobStart",
            "JobResume",
            "JobEnd"
        });
        public static ReadOnlyCollection<string> JobPerform
        {
            get { return _jobperform; }
        }
        public enum VacationType
        {
            [Display(Name = "Pending Approval")]
            PendingApproval = 1,
            Approved=2,
            [Display(Name = "Not Approved")]
            NotApproved=3
        }

        public enum InvoiceSentStatus
        {
            Sent = 1,
            Unsent = 2
        }

        public enum InvoiceStatus
        {
            Pending = 0,
            Submitted = 1,
        }
        public enum CustomerType
        {

            Commercial = 2,
            Domestic = 3,
            [Display(Name = "Property Manager")]
            PropertyManager = 7,
            [Display(Name = "Real Estate")]
            RealState = 4,
            Strata = 5,
            Unknown = 1,
            [Display(Name = "Women's Housing")]
            WomensHousing = 6,
        }
        public enum LeadType
        {
            [Display(Name = "Existing Customer,New Work")]
            ExistingCustomerNewWork = 1,
            Reminders = 2,
            Contracted = 3,
            [Display(Name = "Contracted Customer Extra")]
            ContractedCustomerExtra = 4,
            Google = 5,
            [Display(Name = "True Local")]
            TrueLocal = 6,
            [Display(Name = "Yellow Pages")]
            YellowPages = 7,
            [Display(Name = "Newspaper Advertising")]
            NewspaperAdvertising = 8,
            Flyers = 9,
            Referral = 10,
            [Display(Name = "Marketing Emails/Letters")]
            MarketingEmailsLetters = 11
        }
        public enum Terms
        {
            [Display(Name = "Account(14 Day)")]
            Account14Day = 1,
            [Display(Name = "Cash On Completion")]
            CashOnCompletion = 2
        }
        public enum EmployeeLeaveType
        {
            Sick = 1,
            Annual = 2,
            Unpaid = 3
        }
    }
}