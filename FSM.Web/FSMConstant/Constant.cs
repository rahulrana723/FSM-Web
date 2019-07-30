using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FSM.Web.FSMConstant
{
    public class Constant
    {
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
        public enum CustomerSearchType
        {
            Blacklisted = 8,
            Commercial = 2,
            Contracted = 9,
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
        public enum Title
        {
            [Display(Name = "Mr.")]
            Mr = 1,
            [Display(Name = "Mrs.")]
            Mrs = 2
        }
        public enum SRAS
        {
            None = 0,
            Partial = 1,
            Full = 2
        }
        public enum HomeAddressTitle
        {
            [Display(Name = "Mr.")]
            Mr = 1,
            [Display(Name = "Mrs.")]
            Mrs = 2
        }
        public enum MailingAddressTitle
        {
            [Display(Name = "Mr.")]
            Mr = 1,
            [Display(Name = "Mrs.")]
            Mrs = 2
        }
        public enum HomeAddressStreetType
        {
            ESP = 1,
            FLAT = 2,
            FWAY = 3,
            FRNT = 4,
            GDNS = 5,
            GLEN = 6,
            GRN = 7,
            GR = 8,
            HTS = 9,
            HWY = 10,
            LANE = 11,
            LINK = 12,
            LOOP = 13,
            MALL = 14,
            MEWS = 15,
            PCKT = 16,
            PDE = 17,
            PARK = 18,
            PKWY = 19,
            PL = 20,
            PROM = 21,
            RES = 22,
            RDGE = 23,
            RISE = 24,
            RD = 25,
            ROW = 26,
            SQ = 27,
            ST = 28,
            STRP = 29,
            TARN = 30,
            TCE = 31,
            TFRE = 32,
            TRAC = 33,
            TWAY = 34,
            VIEW = 35,
            VSTA = 36,
            WALK = 37,
            WAY = 38,
            WWAY = 39,
            YARD = 40,
            AVE = 41
        }

        public enum MailingAddressSteetType
        {
            ESP = 1,
            FLAT = 2,
            FWAY = 3,
            FRNT = 4,
            GDNS = 5,
            GLEN = 6,
            GRN = 7,
            GR = 8,
            HTS = 9,
            HWY = 10,
            LANE = 11,
            LINK = 12,
            LOOP = 13,
            MALL = 14,
            MEWS = 15,
            PCKT = 16,
            PDE = 17,
            PARK = 18,
            PKWY = 19,
            PL = 20,
            PROM = 21,
            RES = 22,
            RDGE = 23,
            RISE = 24,
            RD = 25,
            ROW = 26,
            SQ = 27,
            ST = 28,
            STRP = 29,
            TARN = 30,
            TCE = 31,
            TFRE = 32,
            TRAC = 33,
            TWAY = 34,
            VIEW = 35,
            VSTA = 36,
            WALK = 37,
            WAY = 38,
            WWAY = 39,
            YARD = 40,
            AVE = 41
        }
        public enum PrefTimeOfDay
        {
            Anytime = 1,
            [Display(Name = "Early Morning")]
            EarlyMorning = 2,
            Morning = 3,
            Noon = 4,
            Afternoon = 5,
            [Display(Name = "Late Afternoon")]
            LateAfternoon = 6,
            Evening = 7
        }
        public enum UploadFiles
        {
            Signature = 1,
            Profile = 2,
            Driving = 3,
            Bank = 4,
            Insurance = 5
        }


        public enum Frequency
        {
            [Display(Name = "1 Month")]
            Monthly = 1,
            [Display(Name = "2 Months")]
            TWOMONTHLY = 2,
            [Display(Name = "3 Months")]
            THREEMONTHLY = 3,
            [Display(Name = "4 Months")]
            FOURMONTHLY = 4,
            [Display(Name = "6 Months")]
            SIXMONTHLY = 5,
            [Display(Name = "1 Year")]
            TwelveMONTHLY = 6,
            [Display(Name = "Not Set")]
            NotSet = 7
        }
        public enum ContactsType
        {

            Accounts = 1,
            Owner = 2,
            [Display(Name = "Real Estate")]
            RealEstate = 3,
            [Display(Name = "Site Contact")]
            SiteContact = 4,
            [Display(Name = "Strata Manager")]
            StrataManager = 5,




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

        public enum ResidenceType
        {
            House = 1,
            [Display(Name = "T/House")]
            THouse = 2,
            Units = 3,
            Factory = 4,
            Commercial = 5,
            [Display(Name = "Un-Defined")]
            Undefined = 6,
            Terrace = 7
        }
        public enum ResidenceHeight
        {
            [Display(Name = "1")]
            One = 1,
            [Display(Name = "2")]
            Two = 2,
            [Display(Name = "3")]
            Three = 3,
            [Display(Name = "4")]
            Four = 4,
            Other = 5
        }
        //public enum RoofType
        //{
        //    Unknown = 0,
        //    Good = 1,
        //    Fair = 2,
        //    [Display(Name = "Needs Repair")]
        //    NeedRepair = 3,
        //    [Display(Name = "Needs Replacing")]
        //    NeedReplacement = 4,
        //    [Display(Name = "Not Applicable")]
        //    NotApplicable = 5
        //}


        public enum RoofType
        {
            [Display(Name = "Conc Tile")]
            ConcTile = 0,
            [Display(Name = "T/Cotta")]
            TCotta = 1,
            [Display(Name = "Glazed T/Cotta")]
            GlazedTCotta = 2,
            Shingle = 3,
            Slate = 4,
            Metal = 5,
            [Display(Name = "G/guard")]
            Gguard = 6,
            Asbestos = 7,
            Concrete = 8,
            [Display(Name = "UN-DEFINED")]
            Undefined = 9
        }
        public enum GutarGuard
        {
            Unknown = 0,
            [Display(Name = "PGG Metal")]
            PGGMetal = 1,
            pvc = 2,
            Standard = 3
        }
        public enum RoofPitch
        {
            [Display(Name = "22.5")]
            One = 1,
            [Display(Name = "30")]
            Two = 2,
            [Display(Name = "35")]
            Three = 3,
            Flat = 4,
            Other = 5
        }
        public enum DownPipe
        {
            Unknown = 0,
            Good = 1,
            Fair = 2,
            [Display(Name = "Needs Repair")]
            NeedRepair = 3,
            [Display(Name = "Needs Replacing")]
            NeedReplacement = 4,
            [Display(Name = "Not Applicable")]
            NotApplicable = 5
        }

        public enum ConditionRoof
        {
            Unknown = 0,
            Good = 1,
            Fair = 2,
            [Display(Name = "Needs Repair")]
            NeedRepair = 3,
            [Display(Name = "Needs Replacing")]
            NeedReplacement = 4,
            [Display(Name = "Not Applicable")]
            NotApplicable = 5
        }
        public enum BargeCappings
        {
            Unknown = 0,
            Good = 1,
            Fair = 2,
            [Display(Name = "Needs Repair")]
            NeedRepair = 3,
            [Display(Name = "Needs Replacing")]
            NeedReplacement = 4,
            [Display(Name = "Not Applicable")]
            NotApplicable = 5
        }
        public enum RidgeCappings
        {
            Unknown = 0,
            Good = 1,
            Fair = 2,
            [Display(Name = "Needs Repair")]
            NeedRepair = 3,
            [Display(Name = "Needs Replacing")]
            NeedReplacement = 4,
            [Display(Name = "Not Applicable")]
            NotApplicable = 5
        }

        public enum Valleys
        {
            Unknown = 0,
            Good = 1,
            Fair = 2,
            [Display(Name = "Needs Repair")]
            NeedRepair = 3,
            [Display(Name = "Needs Replacing")]
            NeedReplacement = 4,
            [Display(Name = "Not Applicable")]
            NotApplicable = 5
        }
        public enum flashings
        {
            Unknown = 0,
            Good = 1,
            Fair = 2,
            [Display(Name = "Needs Repair")]
            NeedRepair = 3,
            [Display(Name = "Needs Replacing")]
            NeedReplacement = 4,
            [Display(Name = "Not Applicable")]
            NotApplicable = 5
        }
        public enum EmployeeType
        {
            OTRW = 1,
            Other = 2
        }

        public enum JobType
        {
            Quote = 1,
            Do = 2,
            Support = 3,
            [Display(Name = "Non Revenue")]
            NonRevenue = 4,
            [Display(Name = "Call Back")]
            CallBack = 5,
            [Display(Name = "Re-Quote")]
            ReQuote = 6,
            [Display(Name = "Check Measure")]
            CheckMeasure = 7
        }
        public enum OTRWRequired
        {
            [Display(Name = "1")]
            One = 1,
            [Display(Name = "2")]
            Two = 2,
            [Display(Name = "3")]
            Three = 3,
            [Display(Name = "4")]
            Four = 4,
            [Display(Name = "5")]
            Five = 5,
            [Display(Name = "6")]
            Six = 6,
            [Display(Name = "7")]
            Seven = 7,
            [Display(Name = "8")]
            Eight = 8,
            [Display(Name = "9")]
            Nine = 9,
            [Display(Name = "10")]
            Ten = 10
            //[Display(Name = "11")]
            //Eleven = 11,
            //[Display(Name = "12")]
            //Tweleve = 12,
            //[Display(Name = "13")]
            //Thirteen = 13,
            //[Display(Name = "14")]
            //Forteen = 14,
            //[Display(Name = "15")]
            //Fifteen = 15,
            //[Display(Name = "16")]
            //Sixteen = 16,
            //[Display(Name = "17")]
            //Seventeen = 17,
            //[Display(Name = "18")]
            //Eighteen = 18,
            //[Display(Name = "19")]
            //Nineteen = 19,
            //[Display(Name = "20")]
            //Twenty = 20
              
        }
        public enum SendJobEmail
        {

            [Display(Name = "Send Confirmation Email")]
            SendConfirmationEmail = 1,
            [Display(Name = "No Confirmation Email")]
            NoConfirmationEmail = 2
        }
        public enum ReSendJobEmail
        {

            [Display(Name = "Resend Confirmation Email")]
            ResendConfirmationEmail = 1,
            [Display(Name = "Confirmation Sent ")]
            ConfirmationSent = 2
        }
        public enum WorkType
        {
            Roofing = 1,
            Cleaning = 2,
            Office = 3
        }

        public enum AccountJobType
        {
            Do = 2,
            Support = 3,
            [Display(Name = "Non Revenue")]
            NonRevenue = 4
        }
        public enum ReportType
        {
            [Display(Name = "Detail Report")]
            DetailReport = 1,
            [Display(Name = "Aggregate Report")]
            AggregateReport = 2
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
            [Display(Name = "Stand-By")]
            OnHold = 13,
            [Display(Name = "Not On-Hold")]
            NotOnHold = 14,
            Completed = 15,
            [Display(Name = "Not Completed")]
            NotCompleted = 16,
            [Display(Name = "Rain")]
            Rain = 17

        }
        public enum InvoiceJobStatus
        {
            Created = 1,
            //[Display(Name = "Not Created")]
            //NotCreated = 2,
            Booked = 3,
            //[Display(Name = "Not Booked")]
            //NotBooked = 4,
            Assigned = 5,
            //[Display(Name = "Not Assigned")]
            //NotAssigned = 6,
            Confirmed = 7,
            //[Display(Name = "Not Confirmed")]
            //NotConfirmed = 8,
            [Display(Name = "En-Route")]
            EnRoute = 9,
            //[Display(Name = "Not En-Route")]
            //NotEnRoute = 10,
            [Display(Name = "In-Progress")]
            InProgress = 11,
            //[Display(Name = "Not In-Progress")]
            //NotInProgress = 12,
            [Display(Name = "Stand-By")]
            OnHold = 13,
            //[Display(Name = "Not On-Hold")]
            //NotOnHold = 14,
            Completed = 15
            //[Display(Name = "Not Completed")]
            //NotCompleted = 16

        }

        public enum ActionStatus
        {
            Completed = 1,
            Cancel = 2,
            [Display(Name = "In-Complete")]
            InComplete=3
        }

        public enum JobStatusForDashboard
        {
            Created = 1,
            Booked = 3,
            [Display(Name = "Not Booked")]
            NotBooked = 4,
            NotAssigned = 6,
        }

        public enum InvoiceStatus
        {
            Pending = 0,
            Submitted = 1,
        }

        public enum VacationType
        {
            [Display(Name = "Pending Approval")]
            PendingApproval = 1,
            Approved = 2,
            [Display(Name = "Not Approved")]
            NotApproved = 3
        }
        public enum InvoiceSentStatus
        {
            Sent = 1,
            Unsent = 2,
            Unpaid=3,
            Paid=4
        }
        public enum RelationShip
        {
            Relative = 1,
            Partner = 2,
            Friend = 3
        }
        public enum JCLCategory
        {
            [Display(Name = "Roof Plumbing")]
            RoofPlumbing = 1,
            [Display(Name = "Roof Tiling")]
            RoofTiling = 2,
            [Display(Name = "Gutter Cleaning - New Customers")]
            GutterCleaningNewCustomers = 3,
            [Display(Name = "Gutter cleaning - Existing Customers")]
            GuttercleaningExistingCustomers = 4,
            [Display(Name = "Gutter cleaning - Contracted")]
            GuttercleaningContracted = 5,
            Miscellaneous = 6,
            Stormwater = 7,
            [Display(Name = "Anchor Point & SRAS")]
            AnchorPointSRAS = 8,
            [Display(Name = "Gutter Guard")]
            GutterGuard = 9,
            [Display(Name = "Tree Trimming")]
            TreeTrimming = 10,
            [Display(Name = "Customer Discounts ")]
            CustomerDiscounts = 11
        }
        public enum MessageType
        {
            Reminder = 1,
            [Display(Name = "Rain Message")]
            Rain_Message = 2
        }
        public enum TemplateMessage
        {
            [Display(Name = "Due to Rain Job is Postponed to tomorrow")]
            Due_to_Rain_Job_is_Postponed_to_tomorrow = 1,
            [Display(Name = "Payment is Pending")]
            Payment_is_Pending = 2,
            [Display(Name = "Please Provide your feedback on the Job")]
            Please_Provide_your_feedback_on_the_Job = 3,
            [Display(Name = "Domestic Payment Reminder")]
            Domestic_Payment_Reminder = 4,
            [Display(Name = "Confirmation Appointment Strata Realestate")]
            ConfirmationAppointmentStrataRealestate = 5,
            [Display(Name = "Confirmation Appointment Domestic Customer")]
            ConfirmationAppointmentDomesticCustomer = 6,
            [Display(Name = "Due to Rain Job is Postponed to tomorrow")]
            DueToRainJobPostponed = 7,
            [Display(Name = "Unavailable OTRW Due To BadHealth")]
            UnavailableOTRWDueToBadHealth = 8
        }

        public enum DashboardTemplateMessage
        {
            [Display(Name = "Confirmation Appointment Strata Realestate")]
            ConfirmationAppointmentStrataRealestate = 1,
            [Display(Name = "Confirmation Appointment Domestic Customer")]
            ConfirmationAppointmentDomesticCustomer = 2,
            [Display(Name = "Due to Rain Job is Postponed to tomorrow")]
            DueToRainJobPostponed = 3,
            [Display(Name = "Unavailable OTRW Due To BadHealth")]
            UnavailableOTRWDueToBadHealth = 4,
            [Display(Name = "Reminder")]
            CustomerReminder= 5,
            [Display(Name = "Rain")]
            ReminderForRain = 6,
            [Display(Name = "Sick")]
            ReminderForSick = 7,
        }

        public enum CustomerJobTemplateMessage
        {
            [Display(Name = "Confirmation Appointment Strata Realestate")]
            ConfirmationAppointmentStrataRealestate = 1,
            [Display(Name = "Confirmation Appointment Domestic Customer")]
            ConfirmationAppointmentDomesticCustomer = 2,
            [Display(Name = "Due to Rain Job is Postponed to tomorrow")]
            DueToRainJobPostponed = 3,
            [Display(Name = "Unavailable OTRW Due To BadHealth")]
            UnavailableOTRWDueToBadHealth = 4,
            [Display(Name = "Price Increase for Gutter Clean Contract")]
            ContractedGutterCleanPriceIncrease =5,
            [Display(Name = "SRAS Needed for Gutter Clean Contract")]
            ContractedGutterCleanSRASNeeded =6,
            [Display(Name = "Reminder")]
            CustomerReminder = 7,
            [Display(Name = "Rain")]
            ReminderForRain = 8,
            [Display(Name = "Sick")]
            ReminderForSick = 9,
        }


        public enum EmailTemplates
        {
            [Display(Name = "invoice domestic clean")]
            invoice_domestic_clean = 1,
            [Display(Name = "invoice domestic roofing")]
            invoice_domestic_roofing = 2,
            [Display(Name = "invoice  other")]
            invoice_other = 3,
            [Display(Name = "Invoice  overdue payment")]
            Invoice_overdue_payment = 4,
            [Display(Name = "Quote Other Work Found")]
            Quote_Other_Work_Found = 5,
            [Display(Name = "Quote Requested")]
            Quote_Requested = 6,
            [Display(Name = "Confirmation of Appointment email Domestic customer")]
            Confirmation_of_Appointment_email_Domestic_customer = 7,
            [Display(Name = "Confirmation of Appointment email Strata or Real estate")]
            Confirmation_of_Appointment_email_Strata_or_Real_estate = 8,
            [Display(Name = "Deposit Request")]
            Deposit_Request = 9,
            [Display(Name = "Domestic Outstanding Invoice Stage 1")]
            Invoice_Domestic_OutStanding_Stage1,
            [Display(Name = "Domestic Outstanding Invoice Stage 2")]
            Invoice_Domestic_OutStanding_Stage2,
            [Display(Name = "Strata OutStanding Invoice Stage 2")]
            Invoice_Strata_OutStanding_Stage2,
            [Display(Name = "Strata OutStanding Invoice Stage 3")]
            Invoice_Strata_OutStanding_Stage3,
            [Display(Name = "Strata OutStanding Invoice Stage 1")]
            Invoice_Strata_OutStanding_Stage1,
            [Display(Name = "Contracted Gutter Clean SRAS Needed")]
            Contracted_Gutter_Clean_SRAS_Needed,
            [Display(Name = "Contracted Gutter Clean Price Increase")]
            Contracted_Gutter_Clean_Price_Increase
        }

        public enum JobCategory
        {
            Booked = 1,
            StandBy = 2
        }
        public enum AmountPaid
        {
            [Display(Name = "10%")]
            Ten = 10,
            [Display(Name = "20%")]
            Twenty = 20,
            [Display(Name = "30%")]
            Thirty = 30,
            [Display(Name = "40%")]
            Fourty = 40,
            [Display(Name = "50%")]
            Fifty = 50
        }
        public enum PaymentMethod
        {
            [Display(Name = "Credit Card")]
            CreditCard = 1,
            [Display(Name = "Debit Card")]
            DebitCard = 2,
            Cash = 3,
            [Display(Name = "Net Banking")]
            NetBanking = 4,
            [Display(Name = "Cheque")]
            Cheque = 5,
            PayPal=6,
            [Display(Name = "DIRECT DEPOSIT MULTIPLE ")]
            DirectDepositMultiple=7

        }
        public enum FromEmail
        {
            [Display(Name = "invoicing@sydneyroofandgutter.com.au")]
            invoicingsydneyroofandgutter = 0,
            [Display(Name = "admin@sydneyroofandgutter.com.au")]
            adminsydneyroofandgutter = 1,
            [Display(Name = "info@sydneyroofandgutter.com.au")]
            infosydneyroofandgutter = 2,
            [Display(Name = "Frontdesk@sydneyroofandgutter.com.au")]
            Frontdesksydneyroofandgutter = 3,
            [Display(Name = "accounts@sydneyroofandgutter.com.au ")]
            accountssydneyroofandgutter = 4,
        }

        public enum JobNotificationType
        {
            Email = 0,
            [Display(Name = "Text Message")]
            TextMessage = 1,
            Both = 2
        }

        public enum TimesheetReportType
        {
            [Display(Name = "Labour Cost Per Hour")]
            LabourCostPerHour = 1,
            [Display(Name = "Labour TimeSheet")]
            LabourTimeSheet = 2,
            [Display(Name = "Sales Bonus Report")]
            SalesbonusReport = 3,
            [Display(Name = "Unpaid Invoice Report")]
            UnpaidInvoice = 4,
           [Display(Name = "Operational Report")]
            OpertationalReport = 5,
            [Display(Name = "Performance Bonus Reoprt")]
            PerformanceBonusReport = 6,
            [Display(Name = "Sales Invoice Report")]
            SalesInvoiceReport = 7,
            [Display(Name = "PAOLH Report")]
            PaolhReport = 8
        }

        public enum AssetType
        {
            Vehicle = 1,
            [Display(Name = "iPhone 6 Plus")]
            iPhone_6_Plus = 2,
            [Display(Name = "Visa Debit Card")]
            Visa_Debit_Card = 3,
            [Display(Name = "Bunnings Card")]
            Bunnings_Card = 4,
            Blower = 5,
            GPS = 6,
            [Display(Name = "Safety Kit")]
            Safety_Kit = 7,
            Tools = 8,
        }
        public enum AssetAssignStatus
        {
            Assigned = 1,
            [Display(Name = "Not Assigned")]
            Not_Assigned = 2,
        }

        public enum OperationalReportType
        {
            [Display(Name = "Weekly")]
            Weekly = 1,
            [Display(Name = "Monthly")]
            Monthly = 2,
            [Display(Name = "Quaterly")]
            QUARTER = 3,
            [Display(Name = "Yearly")]
            YEARLY = 4
        }

        public enum EmployeeLeaveType
        {
            Sick = 1,
            Annual = 2,
            Unpaid = 3
        }
        public enum EmployeeRoastdDays
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday=4,
            Friday=5,
            Saturday=6,
            Sunday=7
        }
        public enum EmployeeRoastdWeeks
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Fifth = 5
        }
    }
}