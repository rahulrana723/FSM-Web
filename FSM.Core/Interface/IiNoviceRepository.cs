using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
   public interface IiNoviceRepository : IGenericRepository<Invoice> 
    {
        IQueryable<CreateInvoiceCoreViewModel> GetCreateInvoiceList(string jobId); 
        int GetMaxinvoiceNo();
        IQueryable<CreateInvoiceCoreViewModel> GetinvoiceListBySearchKeyword(string keyword,int customertype);
        IQueryable<InvoiceStockJCLItemCoreViewModel> GetStockItemListByJobId(string jobId); 
        Decimal GetStockItemTotalPrice(string jobId); 
        Decimal GetStockAndJCLItemTotalPrice(string jobId);
        Decimal GetInvoiceItemTotalPrice(string InvoiceId);
        IQueryable<InvoiceHistoryCoreVieweModel> GetEmployeeInvoice();
        IQueryable<QuickViewInvoiceCoreViewModel> GetInvoiceQuickViewData(Guid invoiceId);
        int UpdateInvoicePaymentDetail(decimal? v, Guid invoiceID);

        IQueryable<CreateInvoiceCoreViewModel> GetApprovedQuoteListBySearchKeyword(string keyword,int pageno,int pagesize);
        IQueryable<GetInvoiceViewModel> GetALLInvoices(Guid? InvoiceId);
        void SaveInvoiceSyncstatus(Guid syncInvoiceId);
        IQueryable<GetPurchaseorderViewModel> GetallPurchaseOrderformyob();
        void UpdatePurchaseOrderStatus(Guid? iD);
        IQueryable<GetPurchaseorderViewModel> GetallPurchaseOrders();
    }
}
