using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Linq;

namespace FSM.Infrastructure.Repository
{
    public class PurchaseorderjobRepository : GenericRepository<FsmContext, PurchaseOrderByJob>, IPurchaseOrderByJobRepository
    {
        public IQueryable<PurchaserOrderByJobCoreViewModel> GetjobPurchaseOrders(string searchkeyword)
        {
            if (searchkeyword != null)
            {
                searchkeyword = searchkeyword.Replace("'", "''");
            }
            if (!string.IsNullOrEmpty(searchkeyword))
            {
                string purchaseno = "";
                if (searchkeyword.ToLower().Contains("po-"))
                {
                    purchaseno = searchkeyword.ToLower().Replace("po-", "");
                }
                else
                {
                    purchaseno = searchkeyword.ToLower();
                }
                string sql = @"SELECT * FROM(	select distinct j.JobId jobidnumeric,j.JobNo,p.JobID as JobID, inv.InvoiceNo InvoiceNo,inv.id InvoiceId,
                              p.ID,p.PurchaseOrderNo   PurchaseOrderNo,'PO-'+convert(nvarchar(50), p.PurchaseOrderNo )  
                              PurchaseOrderNoformated,p.IsApprove, p.supplierID,p.Description,p.Cost,s.Name SupplierName from  PurchaseOrderByJob p
                            left join Supplier s on s.Id=p.supplierID 
							left join jobs j on j.id= p.jobid
							left join Invoice inv on inv.EmployeeJobId=p.JobID WHERE p.IsDelete=0 or p.IsDelete Is Null) t
                            Where t.PurchaseOrderNo Like '%" + purchaseno + "%'Or t.JobNo Like '%" + searchkeyword + "%'Or t.cost Like '%" + searchkeyword + "%'  Or t.Description Like '%"
                            + searchkeyword + "%' Or t.SupplierName Like '%" + searchkeyword + "%' Or t.InvoiceNo Like '%" + searchkeyword +
                            "%' Or t.jobidnumeric Like '%" + searchkeyword + "%'";

                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaserOrderByJobCoreViewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
            else
            {
                string sql = @"	select distinct j.JobId jobidnumeric,j.JobNo,p.JobID as JobID, inv.InvoiceNo InvoiceNo,inv.id InvoiceId,
                                p.ID,p.PurchaseOrderNo   PurchaseOrderNo,'PO-'+convert(nvarchar(50), p.PurchaseOrderNo )  
                                PurchaseOrderNoformated,p.IsApprove, p.supplierID,p.Description,p.Cost,s.Name SupplierName from  PurchaseOrderByJob p
                            inner join Supplier s on s.Id=p.supplierID 
							Left join jobs j on j.id= p.jobid
							left join Invoice inv on inv.id=p.invoiceid
                             where p.IsDelete=0";


                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaserOrderByJobCoreViewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
        }
        public IQueryable<PurchaserOrderByJobCoreViewModel> GetjobPurchaseOrdersByJobId(string JobId)
        {
            if (!string.IsNullOrEmpty(JobId))
            {
                string purchaseno = "";
                if (JobId.ToLower().Contains("po-"))
                {
                    purchaseno = JobId.ToLower().Replace("po-", "");
                }
                else
                {
                    purchaseno = JobId.ToLower();
                }
                string sql = @"SELECT * FROM(	select distinct j.JobNo,p.JobID as JobID, inv.InvoiceNo InvoiceNo,inv.id InvoiceId,
                              p.ID,p.PurchaseOrderNo   PurchaseOrderNo,'PO-'+convert(nvarchar(50), p.PurchaseOrderNo )  PurchaseOrderNoformated, p.supplierID,p.Description,p.Cost,s.Name SupplierName from  PurchaseOrderByJob p
                            left join Supplier s on s.Id=p.supplierID 
							left join jobs j on j.id= p.jobid
							left join Invoice inv on inv.EmployeeJobId=p.JobID where isnull(p.IsDelete,0)=0) t
                            Where t.Jobid= '" + JobId + "'";

                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaserOrderByJobCoreViewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
            else
            {
                string sql = @"	select distinct j.JobNo jobidnumeric,p.JobID as JobID, inv.InvoiceNo InvoiceNo,inv.id InvoiceId, p.ID,p.PurchaseOrderNo   PurchaseOrderNo,'PO-'+convert(nvarchar(50), p.PurchaseOrderNo )  PurchaseOrderNoformated, p.supplierID,p.Description,p.Cost,s.Name SupplierName from  PurchaseOrderByJob p
                            inner join Supplier s on s.Id=p.supplierID 
							inner join jobs j on j.id= p.jobid
							left join Invoice inv on inv.id=p.invoiceid";


                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaserOrderByJobCoreViewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
        }
        public int GetMaxPurchaseNo()
        {
            string sql = "SELECT MAX(PurchaseOrderNo) FROM dbo.PurchaseOrderByJob";
            return (Convert.ToInt32(Context.Database.SqlQuery<int>(sql).FirstOrDefault()) + 1);
        }
    }
}