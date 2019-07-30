using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using FSM.Infrastructure.Repository;
using FSM.Core.Interface;
using FSM.Web.Controllers;

namespace FSM.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());
            container.RegisterType<ICustomerGeneralInfoRepository, CustomerGeneralInfoRepository>();
            container.RegisterType<IEmployeeDetailRepository, EmployeeDetailRepository>();
            container.RegisterType<IAspNetRolesRepository, AspNetRolesRepository>();
            container.RegisterType<IAspNetUserRolesRepository, AspNetUserRolesRepository>();
            container.RegisterType<ICustomerSiteDetailRepository, CustomerSiteDetailRepository>();
            container.RegisterType<ICustomerBillingAddressRepository, CustomerBillingAddressRepository>();
            container.RegisterType<ICustomerResidenceDetailRepository, CustomerResidenceDetailRepository>();
            container.RegisterType<ICustomerConditionReportRepository, CustomerConditionReportRepository>();
            container.RegisterType<ICustomerContactLogRepository, CustomerContactLogRepository>();
            container.RegisterType<IEmployeeRatesRepository, EmployeeRatesRepository>();
            container.RegisterType<IAspNetUsersRepository, AspNetUsersRepository>();
            container.RegisterType<IStockRepository, StockRepository>();
            container.RegisterType<IEmployeeJobRepository, EmployeeJobRepository>();
            container.RegisterType<IEmployeeJobDocumentRepository, EmployeeJobDocumentRepository>();
            container.RegisterType<ICustomerSiteDocumentsRepository, CustomerSiteDocumentsRepository>();
            container.RegisterType<ICustomerContactsRepository, CustomerContactsRepository>();
            container.RegisterType<IOTRWStockRepository, OTRWSTockRepository>();
            container.RegisterType<IJobStockRepository, JobStockRepository>();
            container.RegisterType<IUserTimeSheetRepository, UserTimeSheetRepository>();
            container.RegisterType<IPurchaseorderItemsStock, PurchaseorderItemsStockRepository>();
            container.RegisterType<IPurchaseOrderByStock, PurchaseOrderByStockRepository>();
            container.RegisterType<ISupplier, SupplierRepository>();
            container.RegisterType<IiNoviceRepository, iNvoiceRepository>();
            container.RegisterType<IPurchaseOrderByJobRepository, PurchaseorderjobRepository>();
            container.RegisterType<IPurchaseorderItemJobRepository, PurchaseorderItemJobRepository>();
            container.RegisterType<IVacationRepository, VacationRepository>();
            container.RegisterType<IUserMessageRepository, UserMessageRepository>();
            container.RegisterType<IUserMessageThreadRepository, UserMessageThreadRepository>();
            container.RegisterType<ISupportJobRepository, SupportJobRepository>();
            container.RegisterType<IPublicHolidayRepository, PublicHolidayRepository>();
            container.RegisterType<ISupportdojobMapping, SupportdojobMappingRepository>();
            container.RegisterType<IModule_MasterRepository, Module_MasterRepository>();
            container.RegisterType<IAction_MasterRepository, Action_MasterRepository>();
            container.RegisterType<IRoleModuleActionMappingRepository, RoleModuleActionMappingRepository>();
            container.RegisterType<IRoleModuleMappingRepository, RoleModuleMappingRepository>();
            container.RegisterType<IParentAction_MasterRepository, ParentAction_MasterRepository>();
            container.RegisterType<IRoleParentActionMappingRepository, RoleParentActionMappingRepository>();
            container.RegisterType<IModule_ActionDefaultMaster, ModuleActionMasterRepository>();
            container.RegisterType<IiNvoiceItemsRepository, iNvoiceItemsRepository>();
            container.RegisterType<IimportantDocuments, ImportantDocumentsRepository>();
            container.RegisterType<IWebNotificationRepository, WebNotificationRepository>();
            container.RegisterType<IEmployeeDetailTempRepository, EmployeeDetailTempRepository>();
            container.RegisterType<IJobAssignToMappingRepository, JobAssignToMappingRepository>();
            container.RegisterType<IRateSubCategoryRepository, RateSubCategoryRepository>();
            container.RegisterType<IRateCategoryRepository, RateCategoryRepository>();
            container.RegisterType<ICustomerReminderRepository, CustomerReminderRepository>();


            container.RegisterType<IViewEmployeeRatesRepository, ViewEmployeeRatesRepository>();
            container.RegisterType<IJCLRepository, JCLRepository>();
            container.RegisterType<IEmployeeWorkTypeRepository, EmployeeWorkTypeRepository>();
            container.RegisterType<IWorkTypeRepository, WorkTypeRepository>();
            container.RegisterType<IJCLItemInvoiceMappingRepository, JCLItemInvoiceMappingRepository>();
            container.RegisterType<IJobRiskAssessmentRepository, JobRiskAssessmentRepository>();
            container.RegisterType<IJCLColor_MappingRepository, JCLColor_MappingRepository>();
            container.RegisterType<IJCLSize_MappingRepository, JCLSize_MappingRepository>();
            container.RegisterType<IJCLProducts_MappingRepository, JCLProducts_MappingRepository>();
            container.RegisterType<IinvoicedJCLItemMappingRepository, InvoicedJCLItemMappingRepository>();
            container.RegisterType<IiNvoicePaymentRepository, InvoicePaymentRepository>();
            container.RegisterType<IAspNetUsersIsDeleteRepository, AspNetUsersIsDeleteRepository>();
            container.RegisterType<IJClExtraproduct_mappingRepositories, JclExtraProduct_MappingRepository>();
            container.RegisterType<ILogRepository, LogRepository>();
            container.RegisterType<IInvoiceAssignToMappingRepository, InvoiceAssignToMappingRepository>();
            container.RegisterType<IAssetManagementRepository, AssetManagementRepository>();
            container.RegisterType<ICustomerJobReminderMapping, CustomerJobReminderMappingRepository>();
            container.RegisterType<IScheduleReminderRepository, ScheduleReminderRepository>();
            container.RegisterType<IContactLogSiteContactsMappingRepository, ContactLogSiteContactsMappingRepository>();
            container.RegisterType<IRoastedOffRepository, RoastedOffRepository>();
            container.RegisterType<IRoastedOffWeekMappingRepository, RoastedOffWeekMappingRepository>();
            container.RegisterType<IJobsReasonMappingRepository, JobsReasonMappingRepository>();
            container.RegisterType<IJobTaskMappingRepository, JobTaskMappingRepository>();
            container.RegisterType<ICompulsaryTaskRepository, CompulsaryTaskRepository>();
        }
    }
}
