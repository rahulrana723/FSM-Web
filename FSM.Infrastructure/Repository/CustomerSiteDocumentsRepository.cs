using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class CustomerSiteDocumentsRepository : GenericRepository<FsmContext, CustomerSitesDocuments>, ICustomerSiteDocumentsRepository
    {
        public IQueryable<CustomerSitesDocumentsViewModelCore> GetSiteDocumentList(string keywordSearch, Guid Customergeninfoid)
        {

            string sql = @"SELECT * FROM(select sd.SiteDetailId, sd.StreetName,count(csd.SiteId) as filecount,
                           CASE WHEN sd.StrataPlan Is Null
                           THEN sd.SiteFileName
                           ELSE sd.StrataPlan 
                           END SiteAddress
                           -- ,ISNULL(NULLIF(sd.street, '')+'  ', '') + ISNULL(NULLIF(sd.StreetName, '') + ', ', '')+ISNULL(NULLIF(sd.Suburb, '')+'  ', '')+ISNULL(NULLIF(sd.state, '')+'  ', '')+cast(sd.PostalCode as nvarchar) as SiteAddress
                            from 
                            dbo.CustomerSitesDocuments csd
                            join dbo.CustomerSiteDetail sd on sd.SiteDetailId=csd.SiteId
                            where csd.IsDelete=0 AND csd.CustomerGeneralInfoId='" + Customergeninfoid + @"'
                            group by sd.SiteDetailId,sd.StreetName,sd.Street,sd.Suburb,sd.State
							,sd.PostalCode,sd.StrataPlan,sd.SiteFileName)t
                            Where t.SiteAddress Like '%" + keywordSearch + "%'";


            var CustomerSiteList = Context.Database.SqlQuery<CustomerSitesDocumentsViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;

        }
        public IQueryable<CustomerSitesDocumentsViewModelCore> GetJobSiteDocumentList(string keywordSearch, Guid SiteId)
        {

            string sql = @"SELECT * FROM(select sd.SiteDetailId, sd.StreetName,count(csd.SiteId) as filecount
                          ,ISNULL(NULLIF(sd.street, '')+'  ', '') + ISNULL(NULLIF(sd.StreetName, '') + ', ', '')+ISNULL(NULLIF(sd.Suburb, '')+'  ', '')+ISNULL(NULLIF(sd.state, '')+'  ', '')+ CONVERT(varchar(10), sd.PostalCode) as SiteAddress
                            from 
                            dbo.CustomerSitesDocuments csd
                            join dbo.CustomerSiteDetail sd on sd.SiteDetailId=csd.SiteId
                            where csd.SiteId='" + SiteId + @"'
                            group by sd.SiteDetailId,sd.StreetName,sd.Street,sd.Suburb,sd.State
							,sd.PostalCode)t
                            Where t.StreetName Like '%" + keywordSearch + "%'";


            var CustomerSiteList = Context.Database.SqlQuery<CustomerSitesDocumentsViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;

        }

        public IQueryable<CustomerSitesDocumentsViewModelCore> JobsSiteDocumentList(string keywordSearch, Guid SiteId)
        {

            string sql = @"SELECT * FROM(select sd.SiteDetailId,csd.DocumentId,csd.DocumentName,csd.docType, sd.StreetName
                          ,ISNULL(NULLIF(sd.street, '')+'  ', '') + ISNULL(NULLIF(sd.StreetName, '') + ', ', '')+ISNULL(NULLIF(sd.Suburb, '')+'  ', '')+ISNULL(NULLIF(sd.state, ''), '')+ CONVERT(varchar(10), sd.PostalCode) as SiteAddress
                            from 
                            dbo.CustomerSitesDocuments csd
                            join dbo.CustomerSiteDetail sd on sd.SiteDetailId=csd.SiteId
                            where csd.IsDelete=0 and csd.SiteId='" + SiteId + @"'
                            )t";


            var CustomerSiteList = Context.Database.SqlQuery<CustomerSitesDocumentsViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;

        }
        public IQueryable<DisplaySiteDocumentsCountViewModel> GetCustomerSiteDocumentsList(string CustomerGeneralInfoId)
        {
            string sql = @"select DocumentId, DocumentName
                            from 
                            dbo.CustomerSitesDocuments 
                            ";
            if (!string.IsNullOrEmpty(CustomerGeneralInfoId))
            {
                sql = sql + " where IsDelete=0 and CustomerGeneralInfoId='" + CustomerGeneralInfoId + "'";
            }
            else
            {
                sql = sql + " where IsDelete=0 and CustomerGeneralInfoId is null";
            }
            var CustomerSiteList = Context.Database.SqlQuery<DisplaySiteDocumentsCountViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<CustomerSitesDocumentsViewModelCore> GetSiteCountForJob(Guid SiteId)
        {

            string sql = @"select SiteDetailId,ISNULL(NULLIF(street, '')+'  ', '') + ISNULL(NULLIF(StreetName, '') + ', ', '')+ISNULL(NULLIF(Suburb, '')+'  ', '')+ISNULL(NULLIF(state, ''), '')+cast(PostalCode as nvarchar) as SiteAddress
                            from  CustomerSiteDetail 
                            where SiteDetailId='" + SiteId + @"'
                            group by SiteDetailId,StreetName,Street,Suburb,State,PostalCode";


            var CustomerSiteList = Context.Database.SqlQuery<CustomerSitesDocumentsViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;

        }
        public IQueryable<CustomerSitesDocumentsViewModelCore> GetSiteCountForCustomer(Guid CustGeneralInfoId)
        {

            string sql = @"select SiteDetailId,
                           CASE WHEN StrataPlan Is Null
                           THEN SiteFileName
                           ELSE StrataPlan 
                           END SiteAddress
--ISNULL(NULLIF(street, '')+'  ', '') + ISNULL(NULLIF(StreetName, '') + ', ', '')+ISNULL(NULLIF(Suburb, '')+'  ', '')+ISNULL(NULLIF(state, ''), '')+cast(PostalCode as nvarchar) as SiteAddress
                            from  CustomerSiteDetail 
                            where IsDelete=0 AND CustomerGeneralInfoId='" + CustGeneralInfoId + @"'
                            group by SiteDetailId,StreetName,Street,Suburb,State,PostalCode,StrataPlan,SiteFileName";


            var CustomerSiteList = Context.Database.SqlQuery<CustomerSitesDocumentsViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;

        }
    }
}
