using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure
{
    public class FsmContext : DbContext
    {
        public FsmContext()
           : base("name=FsmConnectionString")
        {
            var a = Database.Connection.ConnectionString;
        }
        public DbSet<CustomerGeneralInfo> CustomerGeneralInfo { get; set; }
        public DbSet<EmployeeDetail> EmployeeDetail { get; set; }
        public DbSet<CustomerSiteDetail> CustomerSiteDetail { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<CustomerBillingAddress> CustomerBillingAddress { get; set; }
        public DbSet<CustomerResidenceDetail> CustomerResidenceDetail { get; set; }
        public DbSet<CustomerConditionReport> CustomerConditionReport { get; set; }
        public DbSet<CustomerContactLog> CustomerContactLog { get; set; }
        public DbSet<CustomerContacts> CustomerContacts { get; set; }
        public DbSet<EmployeeRates> EmployeeRates { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<JobDocuments> JobDocuments { get; set; }
        public DbSet<FSM.Core.Entities.Jobs> Jobs { get; set; }
        public DbSet<CustomerSitesDocuments> CustomerSiteDocuments { get; set; }
        public DbSet<JobStock> JobStock { get; set; }
        public DbSet<OTRWStock> OTRWStock { get; set; }
        public DbSet<UserTimeSheet> UserTimeSheet { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<PurchaseorderItemsStock> PurchaseorderItemsStock { get; set; }
        public DbSet<PurchaseOrderByStock> PurchaseOrderByStock { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<PurchaseOrderByJob> PurchaseOrderByJob { get; set; }
        public DbSet<PurchaseorderItemJob> PurchaseorderItemJob { get; set; }
        public DbSet<Vacation> Vacation { get; set; }
        public DbSet<SupportJob> SupportJob { get; set; }
        public DbSet<UserMessage> UserMessage { get; set; }
        public DbSet<UserMessageThread> UserMessageThread { get; set; }
        public DbSet<PublicHoliday> PublicHoliday { get; set; }
        public DbSet<SupportdojobMapping> SupportdojobMapping { get; set; }
        public DbSet<RoleModuleMapping> RoleModuleMapping { get; set; }
        public DbSet<RoleModuleActionMapping> RoleModuleActionMapping { get; set; }
        public DbSet<ParentAction_Master> ParentAction_Master { get; set; }
        public DbSet<RoleParentActionMapping> RoleParentActionMapping { get; set; }
        public DbSet<UserDeviceTokenMapping> UserDeviceTokenMappings { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Module_ActionDefaultMaster> Module_ActionDefaultMaster { get; set; }
        public DbSet<InvoiceItems> InvoiceItems { get; set; }
        public DbSet<ImportantDocuments> ImportantDocuments { get; set; }
        public DbSet<WebNotifications> WebNotifications { get; set; }
        public DbSet<EmployeeDetailTemp> EmployeeDetailTemp { get; set; }
        public DbSet<JobAssignToMapping> JobAssignToMapping { get; set; }
        public DbSet<RateSubCategory> RateSubCategory { get; set; }
        public DbSet<RateCategory> RateCategory { get; set; }
        public DbSet<JCL> JCL { get; set; }
        public DbSet<EmployeeWorkType> EmployeeWorkType { get; set; }
        public DbSet<WorkType> WorkType { get; set; }
        public DbSet<CustomerReminder> CustomerReminder { get; set; }
        public DbSet<JCLItemInvoiceMapping> JCLItemInvoiceMapping { get; set; }
        public DbSet<JobRiskAssessment> JobRiskAssessment { get; set; }
        public DbSet<JCLColor_Mapping> JCLColor_Mapping { get; set; }
        public DbSet<JCLSize_Mapping> JCLSize_Mapping { get; set; }
        public DbSet<JCLProducts_Mapping> JCLProducts_Mapping { get; set; }
        public DbSet<InvoicedJCLItemMapping> InvoicedJCLItemMapping { get; set; }
        public DbSet<InvoicePayment> InvoicePayment { get; set; }
        public DbSet<AspNetUsersIsDelete> AspNetUsersIsDelete { get; set; }
        public DbSet<JCLExtraProducts_Mapping> JcLExtraproductMapping { get; set; }
        public DbSet<InvoiceAssignToMapping> InvoiceAssignToMapping { get; set; }

        public DbSet<CustomerJobReminderMapping> CustomerJobReminderMapping { get; set; }
        public DbSet<ScheduleReminder> ScheduleReminder { get; set; }
        public DbSet<ContactLogSiteContactsMapping> ContactLogSiteContactsMapping { get; set; }
        public DbSet<RoastedOff> RoastedOff { get; set; }
        public DbSet<RoastedOffWeekMapping> RoastedOffWeekMapping { get; set; }
        public DbSet<JobsReasonMapping> JobsReasonMapping { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<AssetManagement> AssetManagement { get; set; }
        public DbSet<CompulsaryTask> CompulsaryTask { get; set; }
        public DbSet<JobTaskMapping> JobTaskMapping { get; set; }

        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
