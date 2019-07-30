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
    public class CustomerBillingAddressRepository : GenericRepository<FsmContext, CustomerBillingAddress>, ICustomerBillingAddressRepository
    {
        public IQueryable<CustomerBillingAddressCoreViewModel> GetBillingAddressList(Guid CustomerGeneralInfoId, string keyword)
        {
            if (keyword != null)
            {
                keyword = keyword.Replace("'", "''");
            }
            string sql = @"select * from(select BillingAddressId,CustomerGeneralInfoId,ISNULL(FirstName,'')+' '+ISNULL(LastName,'') Name ,PhoneNo1,EmailId,ISNULL(NULLIF(StreetNo, '') + ', ', '')+ISNULL(NULLIF(StreetName, '') + ', ', '')+ISNULL(NULLIF(Suburb, '') + ', ', '')+ISNULL(NULLIF(State, '') + ', ', '')+ CONVERT(varchar(10), PostalCode) as BillingAddress
                            from CustomerBillingAddress where IsDelete=0)t
                            Where CustomerGeneralInfoId='" + CustomerGeneralInfoId + "' and(t.name like '%" + keyword + "%' or t.PhoneNo1 like '%" + keyword + "%' or t.EmailId like '%" + keyword + "%' or t.BillingAddress like '%" + keyword + "%')";

            var CustomerSiteList = Context.Database.SqlQuery<CustomerBillingAddressCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public void UpdateDefaultAddress(Guid customerGeneralInfoId)
        {
            try
            {
                string sql = @"update CustomerBillingAddress set isdefault=0 where customergeneralinfoid='" + customerGeneralInfoId + "'";
                Context.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
