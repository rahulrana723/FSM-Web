using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
   public class CustomerContactsRepository: GenericRepository<FsmContext, CustomerContacts>, ICustomerContactsRepository
    {
        public IQueryable<CustomerContactsCoreViewModel> GetCustomerContactsInfo(Guid CustomerGeneralInfoId,string SiteId,int ContactType,string searchKeywords = "")
        {
            if (searchKeywords != null)
            {
                searchKeywords = searchKeywords.Replace("'", "''");
            }
            string sql = @"select * from(select Title,cc.ContactId,cc.CustomerGeneralInfoId,FirstName,LastName,EmailId,PhoneNo1,ContactsType,sd.SiteDetailId
            ,ISNULL(NULLIF(sd.Street, ''), ' ') + ISNULL(NULLIF(sd.StreetName, '') + ', ', '')+ISNULL(NULLIF(sd.Suburb, ''), ' ')+ISNULL(NULLIF(sd.state, ''), ' ')+ CONVERT(varchar(10), sd.PostalCode) as SiteAddress
            from CustomerContacts cc
            inner join CustomerSiteDetail sd on cc.SiteId=sd.SiteDetailId)t
            where t.CustomerGeneralInfoId='" + CustomerGeneralInfoId + "' and(t.FirstName like '%" + searchKeywords + "%'or t.LastName like '%" + searchKeywords + "%'or t.PhoneNo1 like '%" + searchKeywords + "%'or t.EmailId like '%" + searchKeywords + "%' or t.ContactsType like '%" + searchKeywords + "%'or t.SiteAddress like '%" + searchKeywords + "%' )";

            if (SiteId != null && SiteId != "")
            {
                sql = sql + " and t.SiteDetailId='" + SiteId + "'";
            }
            if (ContactType !=0 )
            {
                sql = sql + " and t.ContactsType='" + ContactType + "'";
            }

            var CustomerContactsList = Context.Database.SqlQuery<CustomerContactsCoreViewModel>(sql).AsQueryable();
            return CustomerContactsList;
        }

        public IQueryable<CustomerContactsCountViewModel> GetCustomerContactsList(string CustomerGeneralInfoId)
        {
            string sql = @"select ContactId, EmailId
                            from 
                            dbo.CustomerContacts
                            ";
            if (!string.IsNullOrEmpty(CustomerGeneralInfoId))
            {
                sql = sql + " where IsDelete=0 And CustomerGeneralInfoId='" + CustomerGeneralInfoId + "'";
            }
            else
            {
                sql = sql + " where IsDelete=0 And CustomerGeneralInfoId is null";
            }
            var CustomerContactsList = Context.Database.SqlQuery<CustomerContactsCountViewModel>(sql).AsQueryable();
            return CustomerContactsList;
        }
        public IQueryable<CustomerContactsCoreViewModel> GetContactsInfo(Guid CustomerGeneralInfoId,string searchKeywords="")
        {
            string sql = @"select * from(select Title,cc.ContactId,cc.CustomerGeneralInfoId,FirstName,LastName,EmailId,PhoneNo1,ContactsType
            ,ISNULL(NULLIF(sd.Street, '') , ' ') + ISNULL(NULLIF(sd.StreetName, '') + ', ', '')+ISNULL(NULLIF(sd.Suburb, '') , ' ')+ISNULL(NULLIF(sd.state, '') , ' ')+ CONVERT(varchar(10), sd.PostalCode) as SiteAddress
            from CustomerContacts cc
            inner join CustomerSiteDetail sd on cc.SiteId=sd.SiteDetailId where cc.IsDelete=0)t
            where t.CustomerGeneralInfoId='" + CustomerGeneralInfoId + "' and(t.FirstName like '%" + searchKeywords + "%'or t.LastName like '%" + searchKeywords + "%'or t.PhoneNo1 like '%" + searchKeywords + "%'or t.EmailId like '%" + searchKeywords + "%' or t.ContactsType like '%" + searchKeywords + "%'or t.SiteAddress like '%" + searchKeywords + "%' )";

            var CustomerContactsList = Context.Database.SqlQuery<CustomerContactsCoreViewModel>(sql).AsQueryable();
            return CustomerContactsList;
        }
        public IQueryable<CustomerContactsCoreViewModel> GetJobContactsInfo(Guid CustomerGeneralInfoId,Guid siteId,int ContactType, string searchKeywords = "")
        {
            string sql = @"select * from(select Title,cc.ContactId,SiteId,cc.CustomerGeneralInfoId,FirstName,LastName,EmailId,PhoneNo1,ContactsType
            ,ISNULL(NULLIF(sd.Street, '') , ' ') + ISNULL(NULLIF(sd.StreetName, '') + ', ', '')+ISNULL(NULLIF(sd.Suburb, '') , ' ')+ISNULL(NULLIF(sd.state, '') , ' ')+ CONVERT(varchar(10), sd.PostalCode) as SiteAddress            
            from CustomerContacts cc
            inner join CustomerSiteDetail sd on cc.SiteId=sd.SiteDetailId where cc.IsDelete=0)t
            where t.CustomerGeneralInfoId='" + CustomerGeneralInfoId + "' and t.SiteId='" + siteId+"' and(t.FirstName like '%" + searchKeywords + "%'or t.LastName like '%" + searchKeywords + "%'or t.PhoneNo1 like '%" + searchKeywords + "%'or t.EmailId like '%" + searchKeywords + "%' or t.ContactsType like '%" + searchKeywords + "%'or t.SiteAddress like '%" + searchKeywords + "%' )";

            if (ContactType != 0)
            {
                sql = sql + " and t.ContactsType='" + ContactType + "'";
            }

            var CustomerContactsList = Context.Database.SqlQuery<CustomerContactsCoreViewModel>(sql).AsQueryable();
            return CustomerContactsList;
        }
        public IQueryable<JobContactsVM> GetJobContactList(string jobIds)
        {
            string sql = @"select cc.ContactId, cc.FirstName, cc.LastName, cs.SiteFileName, cc.PhoneNo1 as Phone,
                           cc.EmailId, j.DateBooked from Jobs j
                           join CustomerSiteDetail cs on j.SiteId=cs.SiteDetailId
                           join CustomerContacts cc on cs.SiteDetailId=cc.SiteId
                           where j.Id in (" + jobIds + ")";

            var jobContacts = Context.Database.SqlQuery<JobContactsVM>(sql).AsQueryable();
            return jobContacts;
        }
    }
}
