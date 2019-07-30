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
    public class JCLItemInvoiceMappingRepository : GenericRepository<FsmContext, JCLItemInvoiceMapping>, IJCLItemInvoiceMappingRepository
    {
        public IQueryable<JCLItemInvoiceCoreViewModel> GetJCLInvoiceList(Guid invoiceid)
        {
            string sql = @"select JIM.*,j.BonusPerItem,j.ItemName,j.BonusPerItem,j.Category,j.DefaultQty from JCLItemInvoiceMapping JIM
                           inner join Jcl j on JIM.JCLId=j.JCLId
                           where JIM.InvoiceId='" + invoiceid+"'";

            var CustomerjobStockList = Context.Database.SqlQuery<JCLItemInvoiceCoreViewModel>(sql).AsQueryable();
            return CustomerjobStockList;
        }
    }
}
