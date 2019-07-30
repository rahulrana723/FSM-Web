using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class iNvoiceRepository : GenericRepository<FsmContext, Invoice>, IiNoviceRepository
    {
        public IQueryable<CreateInvoiceCoreViewModel> GetCreateInvoiceList(string jobId)
        {

            string sql = @"IF NOT EXISTS(select id from  Invoice where JobId = (select JobNo from Jobs where Id = '" + jobId + @"'))
                           BEGIN
                                    select distinct iv.CreatedDate,cc.FirstName as ContactName,sd.unit SiteUnit,sd.StreetName SiteStreetName,sd.Street SiteStreetAddress,
                                    su.SupportJobId,sd.Suburb SiteSuburb,sd.State SiteState,sd.PostalCode SitePostalCode,js.JobNo as JobId,
                                    js.Id EmployeeJobId,js.OTRWjobNotes OTRWNotes,js.JobNotes,js.OperationNotes,js.JobType,js.Status JobStatus,js.DateBooked,Jasm.AssignTo OTRWAssigned,
                                    cg.CustomerGeneralInfoId,cg.CustomerLastName,cb.CustomerTitle BillingTitle,cb.LastName CoLastName,
                                    cb.Unit BillingUnit,cb.StreetName BillingStreetName,cb.StreetNo BillingStreetAddress,cb.Suburb 
                                    BillingSuburb,cb.State BillingState,cb.Postalcode BillingPostalCode,cc.FirstName as ContactName,
                                    iv.InvoiceDate,ISNULL(iv.CheckMeasure,0) CheckMeasure,iv.DesriptionServicesPerformed,iv.InvoiceNo,ISNULL(iv.Id,'00000000-0000-0000-0000-000000000000')Id,iv.ApprovedBy,iv.InvoiceStatus,iv.Paid,iv.Due,empdetail.FirstName+' '+
                                    empdetail.LastName as  OTRWAssignName
                                    from dbo.Jobs js
                                    left join dbo.CustomerSiteDetail sd on sd.SiteDetailId=js.SiteId
                                    left join dbo.CustomerGeneralInfo cg on cg.CustomerGeneralInfoId=js.CustomerGeneralInfoId
                                    left join dbo.CustomerBillingAddress cb on cb.CustomerGeneralInfoId=js.CustomerGeneralInfoId
                                    left join dbo.CustomerContacts cc on cc.ContactId=sd.ContactId
                                    left join dbo.Invoice iv on iv.jobid=js.JobNo
                                    left join dbo.SupportDoJobMapping su on su.LinkedJobId=js.Id
                                    left join dbo.JobAssignToMapping Jasm on Jasm.JobId = js.Id
                                    left join dbo.EmployeeDetail empdetail on empdetail.EmployeeId=Jasm.AssignTo
                                    where js.Id='" + jobId + @"' order by iv.CreatedDate desc
                           END
                           ELSE
                           BEGIN
                                    select ContactName,SiteUnit,SiteStreetName,SiteStreetAddress,su.SupportJobId,SiteSuburb,SiteState
                                    ,SitePostalCode,Invoice.JobId,EmployeeJobId,Invoice.OTRWNotes,OperationNotes,JobType,JobStatus,Invoice.DateBooked,ISNULL(Invoice.CheckMeasure,0) CheckMeasure,Invoice.DesriptionServicesPerformed,Jasm.AssignTo OTRWAssigned,CustomerGeneralInfoId,CustomerLastName,
                                    BillingTitle,CoLastName,PhoneNo,BillingUnit,BillingStreetName,BillingStreetAddress,BillingSuburb,
                                    BillingState,BillingPostalCode,BillingEmail,ContactName,InvoiceDate,InvoiceNo,ISNULL(Invoice.Id,'00000000-0000-0000-0000-000000000000')Id,ApprovedBy,InvoiceStatus,Paid,Due
                                    ,empdetail.FirstName+' '+empdetail.LastName as  OTRWAssignName
                                    from Invoice
                                    left join dbo.SupportDoJobMapping su on su.LinkedJobId=
                                    (select top 1 Id from Jobs where JobNo = Invoice.JobId )
                                    left join dbo.JobAssignToMapping Jasm on Jasm.JobId = (select top 1 Id from jobs where Jobno = Invoice.JobId )
                                    left join dbo.EmployeeDetail empdetail 
                                    on empdetail.EmployeeId=Jasm.AssignTo
           
                                    where Invoice.JobId=(select JobNo from Jobs where Id = '" + jobId + @"') order by Invoice.CreatedDate desc
                           END";

            var CustomerSiteList = Context.Database.SqlQuery<CreateInvoiceCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public int GetMaxinvoiceNo()
        {
            int count = Context.Invoice.Count();
            string sql = "";
            if (count > 0)
            {
                sql = "SELECT IDENT_CURRENT ('invoice')+1 AS Current_Identity";
            }
            else
            {
                sql = "SELECT IDENT_CURRENT ('invoice') AS Current_Identity";
            }
            return Convert.ToInt32(Context.Database.SqlQuery<decimal>(sql).FirstOrDefault());
        }
        public IQueryable<CreateInvoiceCoreViewModel> GetinvoiceListBySearchKeyword(string keyword, int CustomerType = 0)
        {
            if (keyword != null)
            {

                keyword = keyword.Replace("'", "''");
            }
            if (CustomerType == 0)
            {
                string sql = @"Select TOP 1000 *,(SELECT count(*) from Invoice where IsDelete = 0) TotalCount from(select  t.* ,CSD.SiteFileName, Convert(Date,t.InvoiceDate)as InvoiceSearchDate,Jobs.JobType Type,cust.CustomerType,ISNULL(NULLIF(CSD.Street, '') + ' ', '')+ISNULL(NULLIF(CSD.StreetName, '') + ' ', '')+ISNULL(NULLIF(CSD.Suburb, '') + ' ', '')+ISNULL(NULLIF(CSD.State, '') + ' ', '')+ ISNULL(CAST(CSD.PostalCode as nvarchar(20)),'') as SiteAddress
                            from invoice t
                            left Join Jobs on t.EmployeeJobId=Jobs.Id
                            left Join CustomerSiteDetail CSD on Jobs.SiteId=CSD.SiteDetailId
                            left join CustomerGeneralInfo cust on t.CustomerGeneralInfoId=cust.CustomerGeneralInfoId
							left join CustomerType_Master type on cust.CustomerType=type.Id
                            Where (t.IsDelete=0))M
                            WHERE (M.InvoiceNo Like '%" + keyword + "%' Or M.ContactName Like '%" + keyword + "%' Or M.JobId Like '%" + keyword + "%'  Or M.PhoneNo Like '%" + keyword + "%' Or M.SiteAddress Like '%" + keyword + "%'  OR M.SiteFileName like '%" + keyword + "%')order by M.InvoiceDate DESC";
                var CustomerSiteList = Context.Database.SqlQuery<CreateInvoiceCoreViewModel>(sql).AsQueryable();
                return CustomerSiteList;
            }
            else
            {
                string sql = @"Select TOP 1000 *,(SELECT count(*) from Invoice where IsDelete = 0) TotalCount from(select   t.*,CSD.SiteFileName,Convert(Date,t.InvoiceDate)as InvoiceSearchDate,Jobs.JobType Type,cust.CustomerType,ISNULL(NULLIF(CSD.Street, '') + ', ', '')+ISNULL(NULLIF(CSD.StreetName, '') + ' ', '')+ISNULL(NULLIF(CSD.Suburb, '') + ', ', '')+ISNULL(NULLIF(CSD.State, '') + ' ', '')+ ISNULL(CAST(CSD.PostalCode as nvarchar(20)),'') as SiteAddress
                            from invoice t
                            left Join Jobs on t.EmployeeJobId=Jobs.Id
                            left Join CustomerSiteDetail CSD on Jobs.SiteId=CSD.SiteDetailId
                            left join CustomerGeneralInfo cust on t.CustomerGeneralInfoId=cust.CustomerGeneralInfoId
							left join CustomerType_Master type on cust.CustomerType=type.Id
                            Where (t.IsDelete=0) AND (cust.CustomerType= '" + CustomerType + @"'))M 
                            WHERE (M.InvoiceNo Like '%" + keyword + "%' Or M.ContactName Like '%" + keyword + "%' Or M.JobId Like '%" + keyword + "%'  Or M.PhoneNo Like '%" + keyword + "%' Or M.SiteAddress Like '%" + keyword + "%' OR M.SiteFileName like '%" + keyword + "%') order by M.InvoiceDate DESC";
                var CustomerSiteList = Context.Database.SqlQuery<CreateInvoiceCoreViewModel>(sql).AsQueryable();
                return CustomerSiteList;
            }
        }
        public IQueryable<InvoiceStockJCLItemCoreViewModel> GetStockItemListByJobId(string jobId)
        {

            string sql = @"select s.ID as StockId,s.Description, s.Label Items,js.UnitMeasure,js.Price,js.Quantity,js.jobid as Jobid,js.price*js.Quantity AmountAUD,
                ((js.price*js.Quantity)+(js.price*js.Quantity*10)/100) AmountGSTAUD
							from
							dbo.JobStock js
							join dbo.Stock s on s.ID=js.StockID
							where js.JobId='" + jobId + "'";

            var CustomerSiteList = Context.Database.SqlQuery<InvoiceStockJCLItemCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public Decimal GetStockItemTotalPrice(string jobId)
        {
            string sql = @"SELECT sum(Price) AS Price
                 FROM(
                 SELECT
                 (JobStock.Quantity * Stock.Price) Price
                 FROM Stock
                 LEFT JOIN JobStock ON JobStock.StockID = Stock.Id
                 WHERE JobStock.JobId =  '" + jobId + @"'
                 UNION ALL
                 SELECT(PurchaseorderItemJob.Quantity * Stock.Price) Price
                 FROM Stock
                 LEFT JOIN PurchaseorderItemJob ON PurchaseorderItemJob.StockID = Stock.Id
                 LEFT JOIN PurchaseOrderByJob ON PurchaseOrderByJob.ID = PurchaseorderItemJob.PurchaseOrderID
                 WHERE PurchaseOrderByJob.JobId = '" + jobId + "')t";

            return Convert.ToDecimal(Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault());
        }
        public Decimal GetStockAndJCLItemTotalPrice(string jobId)
        {
            string sql = @"select
            SUM(js.price*js.Quantity) SubTotal
			from
			dbo.JobStock js
			join dbo.Stock s on s.ID=js.StockID
			where js.JobId='" + jobId + "'";

            return Convert.ToDecimal(Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault());
        }
        public Decimal GetInvoiceItemTotalPrice(string InvoiceId)
        {
            string sql = @"
                 SELECT sum(AmountAUD) from InvoiceItems
                 WHERE InvoiceId ='" + InvoiceId + "'";

            return Convert.ToDecimal(Context.Database.SqlQuery<decimal?>(sql).FirstOrDefault());
        }

        public IQueryable<InvoiceHistoryCoreVieweModel> GetEmployeeInvoice()
        {

            string sql = @"select distinct inv.id,inv.InvoiceNo,inv.Price,jb.JobNo,jb.JobId,jb.CustomerGeneralInfoId,jb.SiteId,
                               ISNULL(NULLIF(emp.FirstName,'')+' ','')+ (emp.LastName) as CreatedBy
                                                   ,(SELECT UserName = STUFF((
                             SELECT distinct ',' + ANU.UserName
                            from JobAssignToMapping jap
                           INNER JOIN  AspNetUsers ANU on jap.AssignTo=ANU.Id 
                           where jap.JobId = jb.Id
                            FOR XML PATH('')
                            ), 1, 1, '')) AssignUser
                            ,cust.CustomerLastName CustomerName
                            ,ISNULL(NULLIF(custdetail.Street, ''), '') + ISNULL(NULLIF(custdetail.StreetName, '') + ', ', '')+ISNULL(NULLIF(custdetail.Suburb, ''), '')+ISNULL(NULLIF(custdetail.state, ''), '')+ CONVERT(varchar(10), custdetail.PostalCode) as SiteAddress
                            ,(SELECT MAX(JobDate) 
                            from UserTimeSheet where JobId = jb.Id) CompletedDate
                                             from jobs jb 
                            inner join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId
                            inner join Invoice inv on inv.EmployeeJobId=jb.id
                            inner join EmployeeDetail Emp on emp.EmployeeId=inv.CreatedBy
                            inner join CustomerSiteDetail custdetail on jb.SiteId =custdetail.SiteDetailId
                            inner join JobAssignToMapping Jap on jb.id=Jap.JobId
                            inner join AspNetUsers ANU on jap.AssignTo=ANU.Id
                            Where inv.IsDelete=0";

            var CustomerSiteList = Context.Database.SqlQuery<InvoiceHistoryCoreVieweModel>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public IQueryable<QuickViewInvoiceCoreViewModel> GetInvoiceQuickViewData(Guid invoiceId)
        {

            string sql = @"select 
                           job.JobType,inv.Photos,inv.Due,inv.RequiredDocs,inv.IsPaid,inv.Stock,inv.Material,inv.IsApproved,inv.SentStatus,emp.FirstName+' '+emp.LastName as Approvedbyname,
                            Job.JobNotes,job.otrwjobnotes OTRWNotes,job.OperationNotes,
                                                cast (cast(sum(datediff(second,0,UTS.TimeSpent))/3600 as varchar(12)) + ':' + 
                                   right('0' + cast(sum(datediff(second,0,UTS.TimeSpent))/60%60 as varchar(2)),2) +
                                   ':' + right('0' + cast(sum(datediff(second,0,UTS.TimeSpent))%60 as varchar(2)),2)as nvarchar) TimeTaken
                             from Invoice inv
                            left join Jobs job on inv.EmployeeJobId=job.Id
                            left join UserTimeSheet UTS on inv.EmployeeJobId=UTS.JobId
                            left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=inv.CustomerGeneralInfoId
                            left join CustomerBillingAddress billing on inv.CustomerGeneralInfoId=billing.CustomerGeneralInfoId
                            left join EmployeeDetail emp on inv.ApprovedBy =emp.EmployeeId   where inv.Id= 
                            '" + invoiceId + "'GROUP BY job.JobType,UTS.TimeSpent,Job.JobNotes,job.otrwjobnotes,job.operationnotes,inv.Photos,inv.Due,inv.RequiredDocs,inv.IsPaid,inv.Stock,inv.Material,inv.IsApproved,inv.SentStatus,emp.FirstName,emp.LastName";

            var CustomerSiteList = Context.Database.SqlQuery<QuickViewInvoiceCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }


        public int UpdateInvoicePaymentDetail(decimal? amount, Guid invoiceID)
        {
            var db = Context.Invoice.Where(m => m.Id == invoiceID).FirstOrDefault();
            if (db != null)
            {
                db.Paid = amount;
                db.Due = db.Price - amount;
            }
            Context.Entry(db).State = EntityState.Modified;
            Context.SaveChanges();
            return 1;
        }

        public IQueryable<GetInvoiceViewModel> GetALLInvoices(Guid? Invoiceid)
        {
            try
            {
                if ( Invoiceid.ToString()!=Guid.Empty.ToString())
                {
                    string todaydate = DateTime.Now.ToString("yyyy-MM-dd");
                    string sql = "select  id from invoice where id='"+Invoiceid+ "' and InvoiceType = 'Invoice' and cast(Invoicedate as date) between '2018-01-01' and  '" + todaydate + "' and IsmyobSynced is null and isdelete =0  order by invoicedate";
                    //string sql = "select  id from invoice where id='" + Invoiceid + "' and InvoiceType = 'Invoice' and cast(Invoicedate as date) between '2018-01-01' and  '" + todaydate + "' and IsmyobSynced is null and isdelete =0 and invoiceno=2962 order by invoicedate";
                    var CustomerSiteList = Context.Database.SqlQuery<GetInvoiceViewModel>(sql).AsQueryable();
                    return CustomerSiteList;
                }
                else
                {
                    string todaydate = DateTime.Now.ToString("yyyy-MM-dd");
                    //string sql = "select top 50 id from invoice where InvoiceType = 'Invoice' and cast(Invoicedate as date) between '2018-01-08' and  '2018-02-27' and IsmyobSynced is null and isdelete =0 and invoiceno=1072     order by invoicedate";
                    string sql = "select  id from invoice where InvoiceType = 'Invoice' and cast(Invoicedate as date) between '2018-01-01' and  '"+ todaydate+"' and IsmyobSynced is null and isdelete =0  order by invoicedate";
                    var CustomerSiteList = Context.Database.SqlQuery<GetInvoiceViewModel>(sql).AsQueryable();
                    return CustomerSiteList;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IQueryable<GetPurchaseorderViewModel> GetallPurchaseOrders()
        {
            try
            {
                string todaydate = DateTime.Now.ToString("yyyy-MM-dd");
                string sql = @"
                select   Po.PurchaseOrderNo,po.IsSyncedToMyob,po.id,po.invoiceno,Po.CreatedDate,po.ModifiedDate,s.Name,po.Description  from purchaseorderbyjob po
                 inner join Supplier s on po.Supplierid=s.id
                where (po.isdelete=0 or po.isdelete is null) and cast(po.createdDate as date) between '2017-12-31' and  '" + todaydate + "' and IsSyncedToMyob is null";
                var CustomerSiteList = Context.Database.SqlQuery<GetPurchaseorderViewModel>(sql).AsQueryable();
                return CustomerSiteList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IQueryable<GetPurchaseorderViewModel> GetallPurchaseOrderformyob()
        {
            try
            {
                string todaydate = DateTime.Now.ToString("yyyy-MM-dd");
                string sql = @"
                select   Po.PurchaseOrderNo,po.id,Po.CreatedDate,po.InvoiceId,po.ModifiedDate,s.Name,po.Description  from purchaseorderbyjob po
                 inner join Supplier s on po.Supplierid=s.id
                where (po.isdelete is null or po.isdelete=0)  and cast(po.createdDate as date) between '2017-12-31' and  '" + todaydate + "' and IsSyncedToMyob is null";
                var CustomerSiteList = Context.Database.SqlQuery<GetPurchaseorderViewModel>(sql).AsQueryable();
                return CustomerSiteList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public void SaveInvoiceSyncstatus(Guid syncInvoiceId)
        {
            try
            {
                FsmContext db = new FsmContext();
                var invoice = db.Invoice.Where(i => i.Id == syncInvoiceId).FirstOrDefault();
                invoice.IsmyobSynced = true;
                db.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IQueryable<CreateInvoiceCoreViewModel> GetApprovedQuoteListBySearchKeyword(string keyword, int pageno, int pagesize)
        {
            if (keyword != null)
            {
                keyword = keyword.Replace("'", "''");
            }

            string sql = @"Select * from(select t.* ,CSD.SiteFileName, Convert(Date,t.InvoiceDate)as InvoiceSearchDate,Jobs.JobType Type,cust.CustomerType,ISNULL(NULLIF(CSD.Street, '') + ' ', '')+ISNULL(NULLIF(CSD.StreetName, '') + ' ', '')+ISNULL(NULLIF(CSD.Suburb, '') + ' ', '')+ISNULL(NULLIF(CSD.State, '') + ' ', '')+ ISNULL(CAST(CSD.PostalCode as nvarchar(20)),'') as SiteAddress
                            from invoice t
                            left Join Jobs on t.EmployeeJobId=Jobs.Id
                            left Join CustomerSiteDetail CSD on Jobs.SiteId=CSD.SiteDetailId
                            left join CustomerGeneralInfo cust on t.CustomerGeneralInfoId=cust.CustomerGeneralInfoId
							left join CustomerType_Master type on cust.CustomerType=type.Id
                            Where jobs.IsApproved = 1 and (t.IsDelete=0 and (t.jobtype=1 or t.Invoicetype='quote')))M
                            WHERE ( M.CustomerLastName Like '%" + keyword + "%' or M.InvoiceNo Like '%" + keyword + "%' Or M.ContactName Like '%" + keyword + "%' Or M.JobId Like '%" + keyword + "%'  Or M.PhoneNo Like '%" + keyword + "%' Or M.SiteAddress Like '%" + keyword + "%'  OR M.SiteFileName like '%" + keyword + "%')order by M.CreatedDate DESC";
            var CustomerSiteList = Context.Database.SqlQuery<CreateInvoiceCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;

        }

        public void UpdatePurchaseOrderStatus(Guid? iD)
        {
            try
            {
                FsmContext db = new FsmContext();
                var Purchaseorder = db.PurchaseOrderByJob.Where(i => i.ID == iD).FirstOrDefault();
                if(Purchaseorder!=null)
                {
                    Purchaseorder.IsSyncedToMyob = true;
                    db.SaveChanges();
                }
                var purchaseitems = db.PurchaseorderItemJob.Where(i => i.PurchaseOrderID == iD).ToList();
                foreach( var item in purchaseitems)
                {
                    item.IsSyncedToMyob = true;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
