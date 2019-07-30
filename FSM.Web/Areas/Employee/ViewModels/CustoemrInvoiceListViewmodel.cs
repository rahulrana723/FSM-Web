using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CustoemrInvoiceListViewmodel
    {
        public List<InvoiceStockJCLItemCoreViewModel> getStockItemListViewModel { get; set; }
        public List<InvoiceJCLItemListViewModel> invoiceJclItemListViewModel { get; set; }
        public CreateInvoiceViewModel createInvoiceViewModel { get; set; }
        public JCLItems JclMappingViewModel { get; set; }
        public List<InvoicePaymentList> InvoicePaymentViewModel { get; set; }
        public string UserRole { get; set; }

    }



    public class Quote_MaterialViewmodel
    {
        public List<InvoiceJCLItemListViewModel> invoiceJclItemListViewModel { get; set; }
        public QuoteMaterialViewModel QuotematerialViewModel { get; set; }

    }

  
        public class InvoicePaymentList
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public Nullable<Decimal> PaymentAmount { get; set; }
        public Nullable<FSMConstant.Constant.PaymentMethod> PaymentMethod { get; set; }
        public string Reference { get; set; }
        public string PaymentNote { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string payment_Date { get; set; }
        public string payment_Method { get; set; }
    }
    public class JCLItems
    {
        public List<SelectListItem> JCLItemList { get; set; }
        public List<SelectListItem> jclcolorlist { get; set; }
        public List<SelectListItem> jclSizeList { get; set; }
        public List<SelectListItem> jclProductlist { get; set; }
        public List<JcLViewModel> JCLInfo { get; set; }
    }

    public class JcLViewModel
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public Nullable<bool> HasBonus { get; set; }
        public Nullable<bool> ApplyBonus { get; set; }
        public Nullable<Guid> GCLID { get; set; }
        public Nullable<Guid> Colorid { get; set; }
        public Nullable<Guid> Sizeid { get; set; }
        public Nullable<Guid> Productid { get; set; }
        public List<SelectListItem> JCLItemList { get; set; }
        public List<SelectListItem> jclcolorlist { get; set; }
        public List<SelectListItem> jclSizeList { get; set; }
        public List<SelectListItem> jclProductlist { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> BonusPerItem { get; set; }
        public Nullable<int> Category { get; set; }
        public string Material { get; set; }
        public Nullable<decimal> BonusMinimum { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Per { get; set; }
        public Nullable<decimal> GroupsOf { get; set; }
        public Nullable<decimal> Minimum { get; set; }
        public Nullable<int> DefaultQty { get; set; }
        public string Description { get; set; }
        public string GeneralQuestion { get; set; }
        public string DiagramQuestion { get; set; }
        public Nullable<bool> Diagram { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }

        public string ColorName { get; set; }
        public string Sizename { get; set; }
        public string productname { get; set; }
        public string Extraproductname { get; set; }

    }


    public class BillingAddressForMyob
    {
        public string TradeName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetNo { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

    }
}