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
    public class CustomerSiteDetailRepository : GenericRepository<FsmContext, CustomerSiteDetail>, ICustomerSiteDetailRepository
    {
        public IQueryable<DisplaySitesViewModel> GetCustomerSiteList(string CustomerGeneralInfoId)
        {
            //string sql = @"select sd.StreetType,sd.Contracted, sd.SiteDetailId,sd.ContactId,sd.StrataManagerId,rd.TypeOfResidence, sd.State,sd.Suburb,sd.StreetName,cr.DownPipes 'DownPipe', rd.RoofType, cc.FirstName+' '+cc.LastName 'Name',sm.FirstName+' '+sm.LastName 'StrataManagerName'
            //                from 
            //                dbo.CustomerSiteDetail sd
            //                left join dbo.CustomerContacts cc on sd.ContactId=cc.ContactId
            //                left join dbo.CustomerContacts sm on sd.StrataManagerId=sm.ContactId
            //                left join dbo.CustomerConditionReport cr on sd.SiteDetailId=cr.SiteDetailId
            //                left join dbo.CustomerResidenceDetail rd on sd.SiteDetailId=rd.SiteDetailId";




            string sql = @" select sd.StreetType,sd.Contracted,sd.SiteFileName,sd.Notes,  
                        sd.SiteDetailId,sd.ContactId,sd.StrataManagerId,rd.TypeOfResidence, sd.Street,
                        sd.State,sd.Suburb,sd.street + ' ' + sd.StreetName + ' ' + stm.streetType + ' ' + Suburb + ' ' + state as StreetName,cr.DownPipes 'DownPipe', rd.RoofType,
                         cc.FirstName + ' ' + cc.LastName 'Name',sm.FirstName + ' ' + sm.LastName 'StrataManagerName',
                         stm.streetType as StreetType
                            from
                            dbo.CustomerSiteDetail sd
                            left
                            join dbo.CustomerContacts cc on sd.ContactId = cc.ContactId
                       left
                            join dbo.CustomerContacts sm on sd.StrataManagerId = sm.ContactId
                       left
                            join dbo.CustomerConditionReport cr on sd.SiteDetailId = cr.SiteDetailId
                       left
                            join dbo.StreetType_Master stm on sd.StreetType = stm.Id
                       left
                            join dbo.CustomerResidenceDetail rd on sd.SiteDetailId = rd.SiteDetailId";

            if (!string.IsNullOrEmpty(CustomerGeneralInfoId))
            {
                sql = sql + " where sd.CustomerGeneralInfoId='" + CustomerGeneralInfoId + "'and sd.IsDelete=0";
            }
            else
            {
                sql = sql + " where sd.CustomerGeneralInfoId is null and sd.IsDelete=0";
            }

            var CustomerSiteList = Context.Database.SqlQuery<DisplaySitesViewModel>(sql).AsQueryable();

            return CustomerSiteList;
        }
        public IQueryable<DisplaySitesViewModel>GetCustomerSiteDetails(string SId)
        {
            string sql = @"SELECT SiteDetailId,StreetName from CustomerSiteDetail";
            var SiteList = Context.Database.SqlQuery<DisplaySitesViewModel>(sql).AsQueryable();
            return SiteList;
        }
        public string GetSiteAddress(Guid SiteId)
        {
            string sql = @"select ISNULL(NULLIF(street, ''), '')+' ' + ISNULL(NULLIF(StreetName, '') + ', ', '')+' '+ISNULL(NULLIF(Suburb, '') , '')+' '+ISNULL(NULLIF(state, '') , '')+' ' + CONVERT(varchar(10),PostalCode) as SiteAddress from CustomerSiteDetail where SiteDetailId='"+SiteId+"' ";
            return Context.Database.SqlQuery<string>(sql).FirstOrDefault();
        }
    }
}
