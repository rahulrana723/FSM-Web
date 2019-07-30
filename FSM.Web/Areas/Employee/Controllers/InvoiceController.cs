using FSM.Core.Interface;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSM.Core.ViewModels;
using FSM.Web.Common;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Core.Entities;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Web.Script.Serialization;
using Rotativa;
using FSM.Web.FSMConstant;
using FSM.Web.Areas.Customer.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using TransmitSms;
using static FSM.Web.FSMConstant.Constant;
using Microsoft.Ajax.Utilities;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Services;
using MYOB.AccountRight.SDK.Services.GeneralLedger;
using MYOB.AccountRight.SDK.Services.Contact;
using System.Windows.Forms;
using System.Drawing;
using MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger;
using MYOB.AccountRight.SDK.Contracts.Version2.Contact;
using MYOB.AccountRight.SDK.Services.Purchase;
using MYOB.AccountRight.SDK.Contracts.Version2.Purchase;
using MYOB.AccountRight.SDK.Services.Sale;
using MYOB.AccountRight.SDK.Contracts.Version2.Sale;
using System.Threading;

namespace FSM.Web.Areas.Employee.Controllers
{
    [Authorize]
    public class InvoiceController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                        ().DeclaringType);
        [Dependency]
        public ICustomerSiteDocumentsRepository CustomerSitesDocumentsRepo { get; set; }
        [Dependency]
        public ICustomerBillingAddressRepository CustomerBilling { get; set; }
        [Dependency]
        public ICustomerSiteDetailRepository CustomerSiteDetailRepo { get; set; }
        [Dependency]
        public IEmployeeJobDocumentRepository EmployeejobDocument { get; set; }
        [Dependency]
        public ICustomerGeneralInfoRepository CustomerGeneralInfo { get; set; }
        [Dependency]
        public IPurchaseOrderByJobRepository JobPurchaseOrder { get; set; }
        [Dependency]
        public IPurchaseorderItemJobRepository JobPurchaseOrderitem { get; set; }
        [Dependency]
        public IJobAssignToMappingRepository JobAssignToMappingRepo { get; set; }
        [Dependency]
        public ICustomerConditionReportRepository ConditionReport { get; set; }
        [Dependency]
        public IStockRepository Stock { get; set; }
        [Dependency]
        public IJobStockRepository JobStock { get; set; }
        [Dependency]
        public IEmployeeJobRepository Employeejob { get; set; }
        [Dependency]
        public ISupportdojobMapping SupportjobMapping { get; set; }
        [Dependency]
        public IiNoviceRepository InvoiceRep { get; set; }
        [Dependency]
        public IEmployeeDetailRepository Employee { get; set; }
        [Dependency]
        public ISupplier Supplier { get; set; }
        [Dependency]
        public IiNvoiceItemsRepository Invoiceitem { get; set; }
        [Dependency]
        public ICustomerContactsRepository Customercontacts { get; set; }
        [Dependency]
        public ICustomerReminderRepository CustomerReminderRepo { get; set; }
        [Dependency]
        public ICustomerResidenceDetailRepository CustomerResidence { get; set; }
        [Dependency]
        public ICustomerContactLogRepository CustomercontactLogRepo { get; set; }
        [Dependency]
        public IContactLogSiteContactsMappingRepository ContactLogSiteContactsMappingRepo { get; set; }
        [Dependency]
        public IimportantDocuments ImpDocsRepo { get; set; }
        [Dependency]
        public ICustomerSiteDocumentsRepository CustSiteDocRepo { get; set; }
        [Dependency]
        public IJCLItemInvoiceMappingRepository JCLItemInvoiceRepo { get; set; }
        [Dependency]
        public IScheduleReminderRepository ScheduleReminderRepo { get; set; }

        [Dependency]
        public IJCLRepository JCLRepository { get; set; }
        [Dependency]
        public IJCLRepository JCLRepo { get; set; }
        [Dependency]
        public IinvoicedJCLItemMappingRepository invoiceJCLItemRepo { get; set; }

        [Dependency]
        public IJCLColor_MappingRepository JCLColorRepo { get; set; }

        [Dependency]
        public IJCLProducts_MappingRepository JCLProductRepo { get; set; }

        [Dependency]
        public IInvoiceAssignToMappingRepository InvoiceAssignMapping { get; set; }

        [Dependency]
        public IJCLSize_MappingRepository JCLSizeRepo { get; set; }
        [Dependency]
        public IiNvoicePaymentRepository InvoicePaymentRepo { get; set; }
        [Dependency]
        public IEmployeeJobRepository JobRepository { get; set; }
        [Dependency]
        public IJobAssignToMappingRepository JobAssignMapping { get; set; }


        public Guid SyncInvoiceId;

        public string _Apiode = "";
        public bool issinglerecord = true;
        public string redirecturl = "";
        //GET:Employee/Invoice/CreateInvoice
        /// <summary>
        /// Create Invoice 
        /// </summary>
        /// <param name="empjobid"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult CreateInvoice(string empjobid)
        {
            try
            {
                using (InvoiceRep)
                {
                    CreateInvoiceCoreViewModel createInvoiceCoreViewModel = new CreateInvoiceCoreViewModel();
                    var Customerinvoice = InvoiceRep.GetCreateInvoiceList(empjobid).FirstOrDefault();
                    var TotalInvoiceItemPrice = InvoiceRep.GetStockItemTotalPrice(empjobid);
                    var SUBTotalStockJCLPrice = InvoiceRep.GetStockAndJCLItemTotalPrice(empjobid);

                    CommonMapper<CreateInvoiceCoreViewModel, CreateInvoiceViewModel> mapper = new CommonMapper<CreateInvoiceCoreViewModel, CreateInvoiceViewModel>();
                    CreateInvoiceViewModel EmployeeinvoiceViewModel = mapper.Mapper(Customerinvoice);

                    if (Customerinvoice != null)
                    {
                        Guid jobid = Guid.Parse(empjobid);
                        var job = JobRepository.FindBy(i => i.Id == jobid).FirstOrDefault();
                        Customerinvoice.JobNotes = !string.IsNullOrEmpty(job.JobNotes) ? Regex.Replace(job.JobNotes, "<.*?>", String.Empty) : "";
                    }

                    //Get stockItem of a job
                    var stockJCLItemList = InvoiceRep.GetStockItemListByJobId(empjobid).ToList();
                    Guid JobId = Guid.Parse(empjobid);
                    var workType = Employeejob.FindBy(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();
                    var jobtype = Employeejob.FindBy(m => m.Id == JobId).Select(m => m.JobType).FirstOrDefault();
                    if (jobtype == 1)
                    {
                        EmployeeinvoiceViewModel.InvoiceType = "Quote";
                    }
                    else
                    {
                        EmployeeinvoiceViewModel.InvoiceType = "Invoice";
                    }

                    if (EmployeeinvoiceViewModel.SupportJobId != null && EmployeeinvoiceViewModel.SupportJobId.ToString() != Guid.Empty.ToString())
                    {
                        var SupportJob = Employeejob.FindBy(job => job.Id == EmployeeinvoiceViewModel.SupportJobId).FirstOrDefault();
                        if (SupportJob != null)
                        {
                            EmployeeinvoiceViewModel.SupportJId = Convert.ToInt32(SupportJob.JobId);
                            EmployeeinvoiceViewModel.SupportCustName = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == SupportJob.CustomerGeneralInfoId).FirstOrDefault().CustomerLastName;
                            if (SupportJob.JobType != null)
                            {
                                EmployeeinvoiceViewModel.SupportjobType = (FSM.Web.FSMConstant.Constant.JobType)SupportJob.JobType;
                            }
                            if (SupportJob.Status != null)
                            {
                                EmployeeinvoiceViewModel.SupportjobStatus = (FSM.Web.FSMConstant.Constant.JobStatus)SupportJob.Status;
                            }
                            EmployeeinvoiceViewModel.SupportjobDateBooked = SupportJob.DateBooked;
                            if (SupportJob.AssignTo != null)
                            {
                                EmployeeinvoiceViewModel.SupportOTRW = Guid.Parse(Convert.ToString(SupportJob.AssignTo).ToLower());
                            }
                        }
                    }

                    JCLItems jclob = new JCLItems();
                    var JobQuote = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Quote").FirstOrDefault();
                    var Invoice = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Invoice").FirstOrDefault();
                    if (JobQuote != null && Invoice == null)
                    {
                        List<JcLViewModel> jclitemslist = new List<JcLViewModel>();
                        var QuoteJclitems = invoiceJCLItemRepo.FindBy(i => i.JobID == JobQuote.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);

                        foreach (var i in QuoteJclitems)
                        {


                            JcLViewModel item = new JcLViewModel();
                            var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                            item.Description = i.Description;
                            item.DefaultQty = i.Quantity;
                            item.Price = i.Price;
                            item.GCLID = jclitemifo.JCLId;
                            item.Colorid = i.ColorID;
                            item.Productid = i.ProductStyleID;
                            item.Sizeid = i.SizeID;

                            item.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                            {
                                Text = m.ItemName,
                                Value = m.JCLId.ToString(),
                                Selected = m.JCLId == item.GCLID
                            }).OrderBy(k => k.Text).ToList();

                            item.jclcolorlist = JCLColorRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                            {
                                Text = m.ColorName,
                                Value = m.ColorId.ToString(),
                                Selected = m.ColorId == item.Colorid
                            }).OrderBy(k => k.Text).ToList();

                            item.jclProductlist = JCLProductRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                            {
                                Text = m.ProductName,
                                Value = m.ProductId.ToString(),
                                Selected = m.ProductId == item.Productid
                            }).OrderBy(k => k.Text).ToList();

                            item.jclSizeList = JCLSizeRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                            {
                                Text = m.Size,
                                Value = m.SizeId.ToString(),
                                Selected = m.SizeId == item.Sizeid
                            }).OrderBy(k => k.Text).ToList();
                            item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                            jclitemslist.Add(item);
                        }


                        jclob.JCLInfo = jclitemslist;
                        //subtotal
                        decimal subtotal = Math.Round(Convert.ToDecimal(jclitemslist.Sum(i => i.TotalPrice)), 2);

                        jclob.JCLInfo.ForEach(i => i.SubTotal = subtotal);
                        //GST
                        decimal GST = Math.Round(Convert.ToDecimal((subtotal * 10) / 100), 2);
                        jclob.JCLInfo.ForEach(i => i.GST = GST);
                        //GrandTotal
                        decimal GrandTotal = Math.Round((subtotal + GST), 2);
                        jclob.JCLInfo.ForEach(i => i.GrandTotal = GrandTotal);
                        jclob.JCLInfo.ForEach(i => i.JobId = Guid.Parse(JobQuote.EmployeeJobId.ToString()));

                    }
                    else
                    {



                        jclob.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                        {
                            Text = m.ItemName,
                            Value = m.JCLId.ToString()
                        }).ToList();

                        jclob.jclcolorlist = JCLColorRepo.GetAll().Select(m => new SelectListItem()
                        {
                            Text = m.ColorName,
                            Value = m.ColorId.ToString()
                        }).ToList();

                        jclob.jclProductlist = JCLProductRepo.GetAll().Select(m => new SelectListItem()
                        {
                            Text = m.ProductName,
                            Value = m.ProductId.ToString()
                        }).ToList();

                        jclob.jclSizeList = JCLSizeRepo.GetAll().Select(m => new SelectListItem()
                        {
                            Text = m.Size,
                            Value = m.SizeId.ToString()
                        }).ToList();

                        List<JcLViewModel> items = new List<JcLViewModel>();
                        jclob.JCLInfo = items;
                        jclob.JCLInfo.ForEach(i => i.JobId = Guid.Parse(empjobid));
                    }
                    //payment Grid Data Binding 
                    List<InvoicePaymentList> Invoicepaymentviewmodel = new List<InvoicePaymentList>();
                    if (TempData["PaymentsInvoice"] != null)
                    {
                        List<InvoicePayment> invoicePayments = (List<InvoicePayment>)TempData["PaymentsInvoice"];
                        InvoicePaymentList item = new InvoicePaymentList();
                        foreach (var i in invoicePayments)
                        {

                            var paymetrecord = InvoicePaymentRepo.FindBy(k => i.Id == item.Id).FirstOrDefault();
                            item.InvoiceId = paymetrecord.InvoiceId;
                            item.PaymentDate = paymetrecord.PaymentDate;
                            item.PaymentAmount = paymetrecord.PaymentAmount;
                            item.PaymentMethod = (FSMConstant.Constant.PaymentMethod)(paymetrecord.PaymentMethod);
                            item.Reference = paymetrecord.Reference;
                            item.PaymentNote = paymetrecord.PaymentNote;
                            Invoicepaymentviewmodel.Add(item);
                        }

                    }
                    else
                    {
                        //check if Job have any Quote
                        InvoicePaymentList item = new InvoicePaymentList();
                        var Quote = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Quote").FirstOrDefault();
                        var Invoicedata = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Invoice").FirstOrDefault();
                        //check for quote balance amount and paid amount during quote
                        if (Quote != null && Invoicedata == null)
                        {
                            EmployeeinvoiceViewModel.BalanceafterQuoteAmount = Quote.Due;
                            EmployeeinvoiceViewModel.QuotePaidAmount = Quote.Paid;
                        }
                        if (Quote != null && Invoicedata == null)
                        {
                            var QuotePaymetrecord = InvoicePaymentRepo.FindBy(k => k.InvoiceId == Quote.Id);//find the quote record in payment

                            foreach (var i in QuotePaymetrecord)
                            {
                                item = new InvoicePaymentList();
                                item.InvoiceId = i.InvoiceId;
                                item.PaymentDate = i.PaymentDate;
                                item.PaymentAmount = i.PaymentAmount;
                                item.PaymentMethod = (FSMConstant.Constant.PaymentMethod)(i.PaymentMethod);
                                item.Reference = i.Reference;
                                item.PaymentNote = i.PaymentNote;
                                Invoicepaymentviewmodel.Add(item);

                            }
                        }
                    }
                    var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
                    {
                        JclMappingViewModel = jclob,
                        getStockItemListViewModel = stockJCLItemList,
                        createInvoiceViewModel = EmployeeinvoiceViewModel
                    };
                    var JoBAssign2 = JobAssignMapping.FindBy(m => m.JobId == JobId).Select(m => m.AssignTo).ToList();

                    var DefaultBillingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == EmployeeinvoiceViewModel.CustomerGeneralInfoId && m.IsDefault == true).Select(m => m.BillingAddressId).FirstOrDefault();
                    if (DefaultBillingaddress != null && DefaultBillingaddress != Guid.Empty)
                    {
                        EmployeeinvoiceViewModel.BillingAddressId = DefaultBillingaddress;
                    }
                    else
                    {
                        DefaultBillingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == EmployeeinvoiceViewModel.CustomerGeneralInfoId).OrderByDescending(m => m.CreatedDate).Select(m => m.BillingAddressId).FirstOrDefault();
                        if (DefaultBillingaddress != null)
                            EmployeeinvoiceViewModel.BillingAddressId = DefaultBillingaddress;
                    }
                    var BillingAddressList = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == EmployeeinvoiceViewModel.CustomerGeneralInfoId).Select(m => new SelectListItem()
                    {
                        Text = m.FirstName + " " + m.LastName,
                        Value = m.BillingAddressId.ToString(),
                        Selected = m.BillingAddressId == EmployeeinvoiceViewModel.BillingAddressId
                    }).ToList();

                    var OTRWList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(workType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id
                    }).ToList();

                    custoemrInvoiceListViewmodel.InvoicePaymentViewModel = Invoicepaymentviewmodel;
                    EmployeeinvoiceViewModel.OTRWListselect = OTRWList;
                    EmployeeinvoiceViewModel.OTRWAssignedList = JoBAssign2;
                    EmployeeinvoiceViewModel.BillingAddressList = BillingAddressList;
                    EmployeeinvoiceViewModel.InvoiceNo = InvoiceRep.GetMaxinvoiceNo();
                    EmployeeinvoiceViewModel.Price = TotalInvoiceItemPrice;
                    EmployeeinvoiceViewModel.SubTotal = SUBTotalStockJCLPrice;
                    EmployeeinvoiceViewModel.Total = SUBTotalStockJCLPrice + (SUBTotalStockJCLPrice * 10) / 100;
                    return View(custoemrInvoiceListViewmodel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Invoice/CreateInvoice
        /// <summary>
        /// Invoice create  
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirect InvoiceList</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateInvoice(CustoemrInvoiceListViewmodel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (InvoiceRep)
                    {
                        CommonMapper<CreateInvoiceViewModel, FSM.Core.Entities.Invoice> mapperdoc = new CommonMapper<CreateInvoiceViewModel, FSM.Core.Entities.Invoice>();
                        if (model.createInvoiceViewModel.InvoiceDate == null)
                        {
                            model.createInvoiceViewModel.InvoiceDate = DateTime.Now;
                        }
                        model.createInvoiceViewModel.IsDelete = false;

                        model.createInvoiceViewModel.CreatedDate = DateTime.Now;
                        model.createInvoiceViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        model.createInvoiceViewModel.InvoiceStatus = FSMConstant.Constant.InvoiceStatus.Submitted;

                        model.createInvoiceViewModel.Id = Guid.NewGuid();
                        FSM.Core.Entities.Invoice customerInvoice = mapperdoc.Mapper(model.createInvoiceViewModel);
                        InvoiceRep.Add(customerInvoice);
                        InvoiceRep.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " created invoice");

                        //check for job having purchaseorder
                        var purchaseorder = JobPurchaseOrder.FindBy(i => i.JobID == model.createInvoiceViewModel.EmployeeJobId).FirstOrDefault();
                        if (purchaseorder != null)
                        {
                            purchaseorder.InvoiceId = model.createInvoiceViewModel.Id;
                            JobPurchaseOrder.Edit(purchaseorder);
                            JobPurchaseOrder.Save();
                        }
                        //Get Supportjob of a job
                        if (model.createInvoiceViewModel.SupportJobId != null && model.createInvoiceViewModel.SupportJobId.ToString() != Guid.Empty.ToString())
                        {
                            var SupportJob = Employeejob.FindBy(job => job.Id == model.createInvoiceViewModel.SupportJobId).FirstOrDefault();
                            if (SupportJob != null)
                            {
                                SupportJob.AssignTo = model.createInvoiceViewModel.SupportOTRW;
                                SupportJob.Status = Convert.ToInt32(model.createInvoiceViewModel.SupportjobStatus);
                                if (SupportJob.DateBooked != null)
                                    SupportJob.DateBooked = Convert.ToDateTime(model.createInvoiceViewModel.DateBooked);
                                SupportJob.ModifiedBy = Guid.Parse(base.GetUserId);
                                SupportJob.ModifiedDate = DateTime.Now;
                                Employeejob.Edit(SupportJob);
                                Employeejob.Save();
                            }
                        }
                        //invoice Assigned jop otrw 
                        InvoiceAssignToMappingViewModel InvoiceAssignViewModel = new InvoiceAssignToMappingViewModel();
                        if (model.createInvoiceViewModel.OTRWAssignedList != null)
                        {

                            foreach (var value in model.createInvoiceViewModel.OTRWAssignedList)
                            {
                                InvoiceAssignViewModel.Id = Guid.NewGuid();
                                InvoiceAssignViewModel.JobId = model.createInvoiceViewModel.EmployeeJobId;
                                InvoiceAssignViewModel.InvoiceId = model.createInvoiceViewModel.Id;
                                InvoiceAssignViewModel.AssignTo = value;
                                InvoiceAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                                InvoiceAssignViewModel.CreatedDate = DateTime.Now;

                                CommonMapper<InvoiceAssignToMappingViewModel, InvoiceAssignToMapping> Assignmapper = new CommonMapper<InvoiceAssignToMappingViewModel, InvoiceAssignToMapping>();
                                InvoiceAssignToMapping invoiceAssignToMapping = Assignmapper.Mapper(InvoiceAssignViewModel);

                                InvoiceAssignMapping.Add(invoiceAssignToMapping);
                                InvoiceAssignMapping.Save();
                            }


                        }

                        if (model.getStockItemListViewModel != null)
                        {
                            CommonMapper<InvoiceStockJCLItemCoreViewModel, InvoiceItems> mapperitems = new CommonMapper<InvoiceStockJCLItemCoreViewModel, InvoiceItems>();
                            foreach (var item in model.getStockItemListViewModel)
                            {
                                item.ID = Guid.NewGuid();
                                item.InvoiceId = model.createInvoiceViewModel.Id;
                                item.Items = item.Items;
                                item.Description = item.Description;
                                item.Quantity = item.Quantity;
                                item.Price = item.Price;
                                item.CreatedDate = DateTime.Now;
                                item.CreatedBy = Guid.Parse(base.GetUserId);
                                InvoiceItems customerInvoiceitems = mapperitems.Mapper(item);
                                Invoiceitem.Add(customerInvoiceitems);
                                Invoiceitem.Save();
                            }
                        }
                        if (TempData["JcLITemInvoice"] != null)
                        {
                            List<InvoicedJCLItemMapping> jclitems = (List<InvoicedJCLItemMapping>)TempData["JcLITemInvoice"];
                            jclitems.ForEach(i => i.InvoiceId = model.createInvoiceViewModel.Id);
                            SaveJcLData(jclitems);
                        }
                        if (TempData["PaymentsInvoice"] != null)
                        {
                            List<InvoicePayment> invoicePayments = (List<InvoicePayment>)TempData["PaymentsInvoice"];
                            Guid InvoiceID = model.createInvoiceViewModel.Id;

                            SaveInvoicePayment(invoicePayments, InvoiceID);
                            if (invoicePayments.Count > 0)
                            {
                                InvoiceRep.UpdateInvoicePaymentDetail(invoicePayments.Sum(i => i.PaymentAmount), InvoiceID);
                            }
                        }



                        TempData["Message"] = 1;
                        return RedirectToAction("SaveInvoiceInfo", new { id = model.createInvoiceViewModel.Id.ToString(), activetab = "Invoice Detail", success = "ok" });
                        //return RedirectToAction("EditInvoice", new { id = model.createInvoiceViewModel.Id.ToString() });
                    }
                }
                model.createInvoiceViewModel.OTRWList = GetOtrwEmployeesList();
                model.createInvoiceViewModel.BillingAddressList = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == model.createInvoiceViewModel.CustomerGeneralInfoId).Select(m => new SelectListItem()
                {
                    Text = m.FirstName + " " + m.LastName,
                    Value = m.BillingAddressId.ToString()
                }).ToList();

                JCLItems jclob = new JCLItems();
                List<JcLViewModel> jclitemlist = new List<JcLViewModel>();
                if (TempData["JcLITemInvoice"] != null)
                {
                    List<InvoicedJCLItemMapping> jclitems = (List<InvoicedJCLItemMapping>)TempData["JcLITemInvoice"];

                    foreach (var i in jclitems)
                    {
                        JcLViewModel item = new JcLViewModel();
                        var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                        item.Description = i.Description;
                        item.DefaultQty = i.Quantity;
                        item.Price = i.Price;
                        item.GCLID = jclitemifo.JCLId;
                        item.Id = i.ID;
                        item.Colorid = i.ColorID;
                        item.Productid = i.ProductStyleID;
                        item.Sizeid = i.SizeID;

                        item.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                        {
                            Text = m.ItemName,
                            Value = m.JCLId.ToString(),
                            Selected = m.JCLId == item.GCLID
                        }).OrderBy(k => k.Text).ToList();

                        item.jclcolorlist = JCLColorRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                        {
                            Text = m.ColorName,
                            Value = m.ColorId.ToString(),
                            Selected = m.ColorId == item.Colorid
                        }).OrderBy(k => k.Text).ToList();

                        item.jclProductlist = JCLProductRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                        {
                            Text = m.ProductName,
                            Value = m.ProductId.ToString(),
                            Selected = m.ProductId == item.Productid
                        }).OrderBy(k => k.Text).ToList();

                        item.jclSizeList = JCLSizeRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                        {
                            Text = m.Size,
                            Value = m.SizeId.ToString(),
                            Selected = m.SizeId == item.Sizeid
                        }).OrderBy(k => k.Text).ToList();
                        item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                        jclitemlist.Add(item);
                    }


                    jclob.JCLInfo = jclitemlist;
                    //subtotal
                    decimal subtotal = Math.Round(Convert.ToDecimal(jclitemlist.Sum(i => i.TotalPrice)), 2);
                    jclob.JCLInfo.ForEach(i => i.SubTotal = subtotal);
                    //GST
                    decimal GST = Math.Round(Convert.ToDecimal((subtotal * 10) / 100), 2);
                    jclob.JCLInfo.ForEach(i => i.GST = GST);
                    //GrandTotal
                    decimal GrandTotal = Math.Round((subtotal + GST), 2);
                    jclob.JCLInfo.ForEach(i => i.GrandTotal = GrandTotal);
                    jclob.JCLInfo.ForEach(i => i.JobId = Guid.Parse(model.createInvoiceViewModel.EmployeeJobId.ToString()));

                }

                else
                {
                    JcLViewModel item = new JcLViewModel();

                    item.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.ItemName,
                        Value = m.JCLId.ToString()
                    }).ToList();

                    item.jclcolorlist = JCLColorRepo.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.ColorName,
                        Value = m.ColorId.ToString()
                    }).ToList();

                    item.jclProductlist = JCLProductRepo.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.ProductName,
                        Value = m.ProductId.ToString()
                    }).ToList();

                    item.jclSizeList = JCLSizeRepo.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.Size,
                        Value = m.SizeId.ToString()
                    }).ToList();

                    jclitemlist.Add(item);
                    jclob.JCLInfo = jclitemlist;
                    //subtotal
                    decimal subtotal = Convert.ToDecimal(jclitemlist.Sum(i => i.TotalPrice));
                    jclob.JCLInfo.ForEach(i => i.SubTotal = subtotal);
                    //GST
                    decimal GST = Math.Round(Convert.ToDecimal((subtotal * 10) / 100), 2);
                    jclob.JCLInfo.ForEach(i => i.GST = GST);
                    //GrandTotal
                    decimal GrandTotal = Math.Round((subtotal + GST), 2);
                    jclob.JCLInfo.ForEach(i => i.GrandTotal = GrandTotal);
                    jclob.JCLInfo.ForEach(i => i.JobId = Guid.Parse(model.createInvoiceViewModel.EmployeeJobId.ToString()));

                }


                model.JclMappingViewModel = jclob;
                //payment Grid Data Binding 
                List<InvoicePaymentList> Invoicepaymentviewmodel = new List<InvoicePaymentList>();
                if (TempData["PaymentsInvoice"] != null)
                {
                    List<InvoicePayment> invoicePayments = (List<InvoicePayment>)TempData["PaymentsInvoice"];

                    foreach (var i in invoicePayments)
                    {
                        InvoicePaymentList item = new InvoicePaymentList();
                        var paymetrecord = InvoicePaymentRepo.FindBy(k => i.Id == item.Id).FirstOrDefault();
                        item.InvoiceId = paymetrecord.InvoiceId;
                        item.PaymentDate = paymetrecord.PaymentDate;
                        item.PaymentAmount = paymetrecord.PaymentAmount;
                        item.PaymentMethod = (FSMConstant.Constant.PaymentMethod)(paymetrecord.PaymentMethod);
                        item.Reference = paymetrecord.Reference;
                        item.PaymentNote = paymetrecord.PaymentNote;

                        Invoicepaymentviewmodel.Add(item);
                    }

                }

                Guid? JobId = model.createInvoiceViewModel.EmployeeJobId;
                var workType = Employeejob.FindBy(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();
                var OTRWList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(workType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();
                model.createInvoiceViewModel.OTRWListselect = OTRWList;
                model.InvoicePaymentViewModel = Invoicepaymentviewmodel;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveInvoicePayment(List<InvoicePayment> invoicePayments, Guid InvoiceID)
        {
            try
            {
                foreach (InvoicePayment item in invoicePayments)
                {
                    var record = InvoicePaymentRepo.FindBy(i => i.Id == item.Id).FirstOrDefault();
                    if (record != null)
                    {
                        record.InvoiceId = InvoiceID;
                        record.PaymentDate = item.PaymentDate;
                        record.PaymentAmount = item.PaymentAmount;
                        record.PaymentMethod = item.PaymentMethod;
                        record.Reference = item.Reference;
                        record.PaymentNote = item.PaymentNote;
                        record.ModifiedBy = Guid.Parse(base.GetUserId);
                        record.ModifiedDate = DateTime.Now;
                        InvoicePaymentRepo.Edit(record);
                        InvoicePaymentRepo.Save();
                    }
                    else
                    {
                        item.Id = Guid.NewGuid();
                        item.InvoiceId = InvoiceID;
                        InvoicePaymentRepo.Add(item);
                        InvoicePaymentRepo.Save();
                    }

                }


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved invoice data");
                TempData["PaymentsInvoice"] = null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void SaveJcLData(List<InvoicedJCLItemMapping> jclitems)
        {
            try
            {
                foreach (InvoicedJCLItemMapping item in jclitems)
                {
                    var record = invoiceJCLItemRepo.FindBy(i => i.ID == item.ID).FirstOrDefault();
                    if (record != null)
                    {
                        record.JCLItemID = item.JCLItemID;
                        record.ColorID = item.ColorID;
                        record.SizeID = item.SizeID;
                        record.ProductStyleID = item.ProductStyleID;
                        record.JobID = item.JobID;
                        record.Quantity = item.Quantity;
                        record.Price = item.Price;
                        record.TotalPrice = item.TotalPrice;
                        record.Description = item.Description;
                        record.ModifiedBy = Guid.Parse(base.GetUserId);
                        record.InvoiceId = item.InvoiceId;
                        record.ModifiedDate = DateTime.Now;
                        record.OrderNo = item.OrderNo;
                        invoiceJCLItemRepo.Edit(record);

                        invoiceJCLItemRepo.Save();
                    }
                    else
                    {
                        item.ID = Guid.NewGuid();
                        invoiceJCLItemRepo.Add(item);
                        invoiceJCLItemRepo.Save();
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " saved jcl data");
                }
                TempData["JcLITemInvoice"] = null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        ///GET:Employee/Invoice/EditInvoice
        /// <summary>
        /// Update Invoice Record
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult EditInvoice(string Id)
        {
            try
            {
                Guid InvoicId = Guid.Parse(Id);
                using (InvoiceRep)
                {
                    CreateInvoiceViewModel createInvoiceViewModel = new CreateInvoiceViewModel();
                    var CustomerCreateinvoiceGridList = InvoiceRep.FindBy(i => i.Id == InvoicId && i.IsDelete == false).FirstOrDefault();
                    var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == CustomerCreateinvoiceGridList.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                    string invoiceTypeId = "";
                    if (customerGeneralInfoId != null)
                    {
                        var customerData = CustomerGeneralInfo.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
                        CustomerCreateinvoiceGridList.CustomerLastName = customerData.CustomerLastName;
                        invoiceTypeId = CustomerCreateinvoiceGridList.InvoiceType;
                    }
                    Jobs jobData = new Jobs();
                    if (invoiceTypeId == "Invoice")
                    {
                        jobData = JobRepository.FindBy(m => m.JobNo == CustomerCreateinvoiceGridList.JobId && m.IsDelete == false && m.JobType == 2).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                    }
                    else
                    {
                        jobData = JobRepository.FindBy(m => m.JobNo == CustomerCreateinvoiceGridList.JobId && m.IsDelete == false && m.JobType == 1).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                    }

                    if (invoiceTypeId == "Invoice" && jobData == null)
                    {
                        jobData = JobRepository.FindBy(m => m.JobNo == CustomerCreateinvoiceGridList.JobId && m.IsDelete == false).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                        TempData["JobType"] = "DO type job not created.";
                    }
                    else
                    {
                        jobData = JobRepository.FindBy(m => m.JobNo == CustomerCreateinvoiceGridList.JobId && m.IsDelete == false && m.JobType == 2).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                    }
                    if (jobData == null)
                    {
                        jobData = JobRepository.FindBy(m => m.JobNo == CustomerCreateinvoiceGridList.JobId && m.IsDelete == false).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                    }

                    if (jobData != null)
                    {
                        CustomerCreateinvoiceGridList.JobId = jobData.JobNo;
                        CustomerCreateinvoiceGridList.JobType = jobData.JobType;
                        CustomerCreateinvoiceGridList.JobStatus = jobData.Status;
                        CustomerCreateinvoiceGridList.DateBooked = jobData.DateBooked;
                        CustomerCreateinvoiceGridList.OTRWNotes = jobData.OTRWjobNotes;
                        CustomerCreateinvoiceGridList.JobNotes = jobData.JobNotes;
                        CustomerCreateinvoiceGridList.OperationNotes = jobData.OperationNotes;

                    }

                    var siteData = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == jobData.SiteId).FirstOrDefault();
                    if (siteData != null)
                    {
                        CustomerCreateinvoiceGridList.SiteStreetAddress = siteData.Street;
                        CustomerCreateinvoiceGridList.SiteUnit = siteData.Unit;
                        CustomerCreateinvoiceGridList.SiteStreetName = siteData.StreetName;
                        CustomerCreateinvoiceGridList.SiteState = siteData.State;
                        CustomerCreateinvoiceGridList.SiteSuburb = siteData.Suburb;
                        CustomerCreateinvoiceGridList.SitePostalCode = siteData.PostalCode;
                    }

                    var userName = "";
                    if (CustomerCreateinvoiceGridList.ModifiedBy == null)
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == CustomerCreateinvoiceGridList.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == CustomerCreateinvoiceGridList.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }

                    var TotalInvoiceItemPrice = CustomerCreateinvoiceGridList.Price;
                    var jobDetailId = Employeejob.FindBy(m => m.JobNo == CustomerCreateinvoiceGridList.JobId).Select(m => m.Id).FirstOrDefault();
                    if (CustomerCreateinvoiceGridList.ModifiedDate == null)
                    {
                        CustomerCreateinvoiceGridList.ModifiedDate = CustomerCreateinvoiceGridList.CreatedDate;
                    }
                    else
                    {
                        CustomerCreateinvoiceGridList.ModifiedDate = CustomerCreateinvoiceGridList.ModifiedDate;
                    }

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
                    CreateInvoiceViewModel EmployeeinvoiceViewModel = mapper.Mapper(CustomerCreateinvoiceGridList);
                    if (string.IsNullOrEmpty(EmployeeinvoiceViewModel.JobNotes))
                    {
                        if (jobData != null)
                        {
                            EmployeeinvoiceViewModel.JobNotes = !string.IsNullOrEmpty(jobData.JobNotes) ? Regex.Replace(jobData.JobNotes, "<.*?>", String.Empty) : "";
                        }
                    }
                    if (EmployeeinvoiceViewModel.SentStatus == 1 && EmployeeinvoiceViewModel.SentStatus != null)
                    {
                        EmployeeinvoiceViewModel.DisplaySentStatus = "Sent";
                    }
                    else
                    {
                        EmployeeinvoiceViewModel.DisplaySentStatus = "UnSent";
                    }
                    EmployeeinvoiceViewModel.ApproveStatus = EmployeeinvoiceViewModel.IsApproved != null ?
                            EmployeeinvoiceViewModel.IsApproved == true ? "[ Approved ]" : "[ Not Approved ]" : string.Empty;

                    //Check if their is supportjob for job
                    var supportjob = SupportjobMapping.FindBy(i => i.LinkedJobId == EmployeeinvoiceViewModel.EmployeeJobId).FirstOrDefault();
                    //Get Supportjob of a job
                    if (supportjob != null)
                    {
                        if (supportjob.SupportJobId != null)
                        {
                            var SupportJob = Employeejob.FindBy(job => job.Id == supportjob.SupportJobId).FirstOrDefault();
                            if (SupportJob != null)
                            {
                                EmployeeinvoiceViewModel.SupportJobId = supportjob.SupportJobId;
                                EmployeeinvoiceViewModel.SupportJId = Convert.ToInt32(SupportJob.JobId);
                                EmployeeinvoiceViewModel.SupportCustName = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == SupportJob.CustomerGeneralInfoId).FirstOrDefault().CustomerLastName;
                                EmployeeinvoiceViewModel.SupportjobType = (FSM.Web.FSMConstant.Constant.JobType)SupportJob.JobType;
                                EmployeeinvoiceViewModel.SupportjobStatus = (FSM.Web.FSMConstant.Constant.JobStatus)SupportJob.Status;
                                EmployeeinvoiceViewModel.SupportjobDateBooked = SupportJob.DateBooked;
                                EmployeeinvoiceViewModel.SupportOTRW = SupportJob.AssignTo;
                            }
                        }
                    }




                    var billingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                    CustomerBillingAddress BillingAddress;
                    if (billingaddress != null)
                    {
                        BillingAddress = billingaddress;
                        EmployeeinvoiceViewModel.BillingAddressId = BillingAddress.BillingAddressId;
                    }
                    else
                    {
                        BillingAddress = CustomerBilling.GetAll().Where(i => i.CustomerGeneralInfoId == customerGeneralInfoId).OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                        if (BillingAddress != null)
                            EmployeeinvoiceViewModel.BillingAddressId = BillingAddress.BillingAddressId;
                    }

                    if (EmployeeinvoiceViewModel.BillingAddressId != null)
                    {
                        var BillingAddressList = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).Select(m => new SelectListItem()
                        {
                            Text = m.FirstName + " " + m.LastName,
                            Value = m.BillingAddressId.ToString(),
                            Selected = m.BillingAddressId == EmployeeinvoiceViewModel.BillingAddressId
                        }).ToList();
                        EmployeeinvoiceViewModel.BillingAddressList = BillingAddressList;

                    }
                    else
                    {
                        var BillingAddressList = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).Select(m => new SelectListItem()
                        {
                            Text = m.FirstName + " " + m.LastName,
                            Value = m.BillingAddressId.ToString(),
                        }).ToList();
                        EmployeeinvoiceViewModel.BillingAddressList = BillingAddressList;
                    }
                    Guid? JobId = EmployeeinvoiceViewModel.EmployeeJobId;
                    var workType = Employeejob.FindBy(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();
                    var OTRWList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(workType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id
                    }).ToList();

                    var invoiceAssign2 = JobAssignMapping.FindBy(m => m.JobId == JobId && m.IsDelete == false).Select(m => m.AssignTo).ToList();



                    EmployeeinvoiceViewModel.OTRWListselect = OTRWList;
                    EmployeeinvoiceViewModel.OTRWAssignedList = invoiceAssign2;
                    EmployeeinvoiceViewModel.UserName = userName;
                    var InvoiceItemList = Invoiceitem.GetAll().Where(i => i.InvoiceId == InvoicId).ToList();
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<InvoiceItems, InvoiceStockJCLItemCoreViewModel> mapperItems = new CommonMapper<InvoiceItems, InvoiceStockJCLItemCoreViewModel>();
                    List<InvoiceStockJCLItemCoreViewModel> invoiceItemCoreViewModel = mapperItems.MapToList(InvoiceItemList.ToList());
                    EmployeeinvoiceViewModel.Price = TotalInvoiceItemPrice;
                    EmployeeinvoiceViewModel.SubTotal = TotalInvoiceItemPrice;
                    EmployeeinvoiceViewModel.Total = TotalInvoiceItemPrice + (TotalInvoiceItemPrice * 10) / 100;

                    Guid Jobid = Guid.Parse(CustomerCreateinvoiceGridList.EmployeeJobId.ToString());

                    JCLItems jclob = new JCLItems();

                    var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoicId).ToList();
                    var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == Jobid && i.InvoiceId == InvoicId).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == Jobid).ToList().OrderBy(i => i.OrderNo);


                    List<JcLViewModel> items = new List<JcLViewModel>();
                    foreach (var i in InvoiceJCLItemlist)
                    {
                        JcLViewModel item = new JcLViewModel();
                        var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                        if (jclitemifo != null)
                        {
                            item.Description = i.Description;
                            item.DefaultQty = i.Quantity;
                            item.Price = i.Price;
                            item.GCLID = jclitemifo.JCLId;
                            item.Id = i.ID;
                            item.Colorid = i.ColorID;
                            item.Productid = i.ProductStyleID;
                            item.Sizeid = i.SizeID;

                            item.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                            {
                                Text = m.ItemName,
                                Value = m.JCLId.ToString(),
                                Selected = m.JCLId == item.GCLID
                            }).OrderBy(k => k.Text).ToList();

                            item.jclcolorlist = JCLColorRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(k => new SelectListItem()
                            {
                                Text = k.ColorName,
                                Value = k.ColorId.ToString(),
                                Selected = k.ColorId == item.Colorid
                            }).OrderBy(k => k.Text).ToList();



                            item.jclProductlist = JCLProductRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                            {
                                Text = m.ProductName,
                                Value = m.ProductId.ToString(),
                                Selected = m.ProductId == item.Productid
                            }).OrderBy(k => k.Text).ToList();

                            item.jclSizeList = JCLSizeRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                            {
                                Text = m.Size,
                                Value = m.SizeId.ToString(),
                                Selected = m.SizeId == item.Sizeid
                            }).OrderBy(k => k.Text).ToList();
                            item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                            items.Add(item);
                        }
                    }
                    // find if Job have any Quote

                    var Quote = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId).FirstOrDefault();
                    var Invoice = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Invoice").FirstOrDefault();
                    if (EmployeeinvoiceViewModel.InvoiceType != "Quote" && Quote != null && Invoice == null)
                    {
                        if (Quote != null && TempData["JcLITemInvoice"] == null)
                        {
                            EmployeeinvoiceViewModel.BalanceafterQuoteAmount = Quote.Due;
                            EmployeeinvoiceViewModel.QuotePaidAmount = Quote.Paid;
                            List<InvoicedJCLItemMapping> jclitems = new List<InvoicedJCLItemMapping>();
                            var JcLitemsQuote = invoiceJCLItemRepo.FindBy(i => i.JobID == Quote.EmployeeJobId).ToList();
                            foreach (var i in JcLitemsQuote)
                            {
                                JcLViewModel item = new JcLViewModel();
                                var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                                item.Description = i.Description;
                                item.DefaultQty = i.Quantity;
                                item.Price = i.Price;
                                item.GCLID = jclitemifo.JCLId;
                                item.Id = i.ID;
                                item.Colorid = i.ColorID;
                                item.Productid = i.ProductStyleID;
                                item.Sizeid = i.SizeID;

                                item.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                                {
                                    Text = m.ItemName,
                                    Value = m.JCLId.ToString(),
                                    Selected = m.JCLId == item.GCLID
                                }).OrderBy(k => k.Text).ToList();

                                item.jclcolorlist = JCLColorRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                                {
                                    Text = m.ColorName,
                                    Value = m.ColorId.ToString(),
                                    Selected = m.ColorId == item.Colorid
                                }).OrderBy(k => k.Text).ToList();

                                item.jclProductlist = JCLProductRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                                {
                                    Text = m.ProductName,
                                    Value = m.ProductId.ToString(),
                                    Selected = m.ProductId == item.Productid
                                }).OrderBy(k => k.Text).ToList();

                                item.jclSizeList = JCLSizeRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                                {
                                    Text = m.Size,
                                    Value = m.SizeId.ToString(),
                                    Selected = m.SizeId == item.Sizeid
                                }).OrderBy(k => k.Text).ToList();

                                item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                                items.Add(item);
                            }

                        }
                    }



                    jclob.JCLInfo = items;
                    //subtotal
                    decimal subtotal = Math.Round(Convert.ToDecimal(items.Sum(i => i.TotalPrice)), 2);
                    jclob.JCLInfo.ForEach(i => i.SubTotal = subtotal);
                    //GST
                    decimal GST = Math.Round((Convert.ToDecimal((subtotal * 10) / 100)), 2);
                    jclob.JCLInfo.ForEach(i => i.GST = GST);
                    //GrandTotal
                    decimal GrandTotal = Math.Round((subtotal + GST), 2);
                    jclob.JCLInfo.ForEach(i => i.GrandTotal = GrandTotal);

                    jclob.JCLInfo.ForEach(i => i.JobId = Jobid);

                    //Get Invoice Payment List
                    var PaymentList = InvoicePaymentRepo.FindBy(i => i.InvoiceId == InvoicId).ToList();

                    CommonMapper<InvoicePayment, InvoicePaymentList> mapperpayment = new CommonMapper<InvoicePayment, InvoicePaymentList>();
                    List<InvoicePaymentList> paymentmodel = mapperpayment.MapToList(PaymentList.OrderBy(x => x.PaymentDate).ToList());

                    //check if Job have any Quote

                    Quote = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Quote").FirstOrDefault();
                    var Invoicedata = InvoiceRep.FindBy(i => i.EmployeeJobId == JobId && i.InvoiceType == "Invoice").FirstOrDefault();
                    //check for quote balance amount and paid amount during quote
                    if (Quote != null && EmployeeinvoiceViewModel.InvoiceType != "Quote" && Invoicedata == null)
                    {
                        EmployeeinvoiceViewModel.BalanceafterQuoteAmount = Quote.Due;
                        EmployeeinvoiceViewModel.QuotePaidAmount = Quote.Paid;
                    }
                    if (Quote != null && EmployeeinvoiceViewModel.InvoiceType != "Quote" && Invoicedata == null)
                    {
                        var QuotePaymetrecord = InvoicePaymentRepo.FindBy(k => k.InvoiceId == InvoicId);//find the quote record in payment
                        foreach (var i in QuotePaymetrecord)
                        {
                            InvoicePaymentList Pitem = new InvoicePaymentList();
                            Pitem.Id = i.Id;
                            Pitem.InvoiceId = i.InvoiceId;
                            Pitem.PaymentDate = i.PaymentDate;
                            Pitem.PaymentAmount = i.PaymentAmount;
                            if (i.PaymentMethod != null)
                                Pitem.PaymentMethod = (FSMConstant.Constant.PaymentMethod)(i.PaymentMethod);
                            Pitem.Reference = i.Reference;
                            Pitem.PaymentNote = i.PaymentNote;


                            paymentmodel.Add(Pitem);
                        }
                    }



                    var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
                    {
                        JclMappingViewModel = jclob,
                        getStockItemListViewModel = invoiceItemCoreViewModel,
                        createInvoiceViewModel = EmployeeinvoiceViewModel,
                        UserRole = base.GetUserRoles[0],
                        InvoicePaymentViewModel = paymentmodel
                    };


                    return View(custoemrInvoiceListViewmodel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ChangeInvoiceStatus()
        {
            try
            {
                var invoiceId = !string.IsNullOrEmpty(Request.QueryString["InvoiceId"]) ?
                                       Guid.Parse(Request.QueryString["InvoiceId"]) : Guid.Empty;
                var status = Request.QueryString["InvoiceStatus"];
                var invoiceStatus = string.Empty;

                var invoice = InvoiceRep.FindBy(m => m.Id == invoiceId).FirstOrDefault();
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "Approve")
                    {
                        invoice.IsApproved = true;
                        invoiceStatus = "[ Approved ]";
                    }
                    else
                    {
                        invoice.IsApproved = false;
                        invoiceStatus = "[ Not Approved ]";
                    }
                }
                invoice.ApprovedBy = Guid.Parse(base.GetUserId);
                InvoiceRep.Edit(invoice);
                InvoiceRep.Save();


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " changed invoice status");

                return Json(new { invoicestatus = invoiceStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Employee/Invoice/EditInvoice
        /// <summary>
        /// Update Invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirect InvoiceList</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult EditInvoice(CustoemrInvoiceListViewmodel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (InvoiceRep)
                    {
                        CommonMapper<CreateInvoiceViewModel, FSM.Core.Entities.Invoice> mapperdoc = new CommonMapper<CreateInvoiceViewModel, FSM.Core.Entities.Invoice>();
                        if (model.createInvoiceViewModel.InvoiceDate == null)
                        {
                            model.createInvoiceViewModel.InvoiceDate = DateTime.Now;
                        }
                        model.createInvoiceViewModel.Due = (model.createInvoiceViewModel.Price) - (model.createInvoiceViewModel.Paid);
                        model.createInvoiceViewModel.ModifiedDate = DateTime.Now;
                        model.createInvoiceViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                        model.createInvoiceViewModel.InvoiceStatus = FSMConstant.Constant.InvoiceStatus.Submitted;
                        FSM.Core.Entities.Invoice customerInvoice = mapperdoc.Mapper(model.createInvoiceViewModel);
                        customerInvoice.IsmyobSynced = null;
                        InvoiceRep.Edit(customerInvoice);
                        InvoiceRep.Save();

                        FSM.Core.Entities.Jobs jobData = JobRepository.FindBy(m => m.JobNo == model.createInvoiceViewModel.JobId).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                        if (jobData != null)
                        {
                            jobData.OTRWjobNotes = model.createInvoiceViewModel.OTRWNotes;
                            jobData.JobNotes = model.createInvoiceViewModel.JobNotes;
                            jobData.OperationNotes = model.createInvoiceViewModel.OperationNotes;
                            JobRepository.Edit(jobData);
                            JobRepository.Save();
                        }

                        FSM.Core.Entities.CustomerSiteDetail siteData = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == jobData.SiteId).FirstOrDefault();
                        if (siteData != null)
                        {
                            siteData.Street = model.createInvoiceViewModel.SiteStreetAddress;
                            siteData.Unit = model.createInvoiceViewModel.SiteUnit;
                            siteData.StreetName = model.createInvoiceViewModel.SiteStreetName;
                            siteData.State = model.createInvoiceViewModel.SiteState;
                            siteData.Suburb = model.createInvoiceViewModel.SiteSuburb;
                            siteData.PostalCode = model.createInvoiceViewModel.SitePostalCode;
                            CustomerSiteDetailRepo.Edit(siteData);
                            CustomerSiteDetailRepo.Save();
                        }

                        if (model.createInvoiceViewModel.SupportJobId != null && model.createInvoiceViewModel.SupportJobId.ToString() != Guid.Empty.ToString())
                        {
                            var SupportJob = Employeejob.FindBy(job => job.Id == model.createInvoiceViewModel.SupportJobId).FirstOrDefault();
                            if (SupportJob != null)
                            {
                                SupportJob.AssignTo = model.createInvoiceViewModel.SupportOTRW;
                                SupportJob.Status = Convert.ToInt32(model.createInvoiceViewModel.SupportjobStatus);
                                if (SupportJob.DateBooked != null)
                                    SupportJob.DateBooked = Convert.ToDateTime(model.createInvoiceViewModel.SupportjobDateBooked);
                                SupportJob.ModifiedBy = Guid.Parse(base.GetUserId);
                                SupportJob.ModifiedDate = DateTime.Now;
                                Employeejob.Edit(SupportJob);
                                Employeejob.Save();
                            }
                        }
                        var invoiceAssignmapper = InvoiceAssignMapping.GetAll().Where(m => m.InvoiceId == model.createInvoiceViewModel.Id).OrderByDescending(i => i.CreatedDate).ToList();
                        foreach (var item in invoiceAssignmapper)
                        {
                            if (item != null)
                            {

                                InvoiceAssignMapping.Delete(item);
                                InvoiceAssignMapping.Save();
                            }
                        }
                        InvoiceAssignToMappingViewModel InvoiceAssignViewModel = new InvoiceAssignToMappingViewModel();
                        if (model.createInvoiceViewModel.OTRWAssignedList != null)
                        {

                            foreach (var value in model.createInvoiceViewModel.OTRWAssignedList)
                            {
                                InvoiceAssignViewModel.Id = Guid.NewGuid();
                                InvoiceAssignViewModel.JobId = model.createInvoiceViewModel.EmployeeJobId;
                                InvoiceAssignViewModel.InvoiceId = model.createInvoiceViewModel.Id;
                                InvoiceAssignViewModel.AssignTo = value;
                                InvoiceAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                                InvoiceAssignViewModel.CreatedDate = DateTime.Now;

                                CommonMapper<InvoiceAssignToMappingViewModel, InvoiceAssignToMapping> Assignmapper = new CommonMapper<InvoiceAssignToMappingViewModel, InvoiceAssignToMapping>();
                                InvoiceAssignToMapping invoiceAssignToMapping = Assignmapper.Mapper(InvoiceAssignViewModel);

                                InvoiceAssignMapping.Add(invoiceAssignToMapping);
                                InvoiceAssignMapping.Save();
                            }


                        }

                        if (model.getStockItemListViewModel != null)
                        {
                            CommonMapper<InvoiceStockJCLItemCoreViewModel, InvoiceItems> mapperitems = new CommonMapper<InvoiceStockJCLItemCoreViewModel, InvoiceItems>();
                            foreach (var item in model.getStockItemListViewModel)
                            {
                                item.ID = item.ID;
                                item.InvoiceId = item.InvoiceId;
                                item.Items = item.Items;
                                item.Description = item.Description;
                                item.Quantity = item.Quantity;
                                item.Price = item.Price;
                                item.ModifiedDate = DateTime.Now;
                                item.ModifiedBy = Guid.Parse(base.GetUserId);
                                InvoiceItems customerInvoiceitems = mapperitems.Mapper(item);
                                Invoiceitem.Edit(customerInvoiceitems);
                                Invoiceitem.Save();
                            }
                        }
                        if (TempData["JcLITemInvoice"] != null)
                        {
                            List<InvoicedJCLItemMapping> jclitems = (List<InvoicedJCLItemMapping>)TempData["JcLITemInvoice"];
                            jclitems.ForEach(i => i.InvoiceId = model.createInvoiceViewModel.Id);
                            SaveJcLData(jclitems);
                        }
                        if (TempData["PaymentsInvoice"] != null)
                        {
                            List<InvoicePayment> invoicePayments = (List<InvoicePayment>)TempData["PaymentsInvoice"];
                            Guid InvoiceID = model.createInvoiceViewModel.Id;
                            SaveInvoicePayment(invoicePayments, InvoiceID);
                            if (invoicePayments.Count > 0)
                            {
                                InvoiceRep.UpdateInvoicePaymentDetail(invoicePayments.Sum(i => i.PaymentAmount), InvoiceID);
                            }
                        }
                        TempData["Message"] = 2;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated invoice");


                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                    }
                }

                Guid? JobId = model.createInvoiceViewModel.EmployeeJobId;
                var workType = Employeejob.FindBy(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();
                var OTRWList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(workType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();

                var invoiceAssign2 = InvoiceAssignMapping.FindBy(m => m.InvoiceId == model.createInvoiceViewModel.Id).Select(m => m.AssignTo).ToList();


                var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                return Json(new { status = "failure", errors = errCollection });
                model.createInvoiceViewModel.BillingAddressList = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == model.createInvoiceViewModel.CustomerGeneralInfoId).Select(m => new SelectListItem()
                {
                    Text = m.FirstName + " " + m.LastName,
                    Value = m.BillingAddressId.ToString()
                }).ToList();
                model.createInvoiceViewModel.OTRWListselect = OTRWList;
                model.createInvoiceViewModel.OTRWAssignedList = invoiceAssign2;
                Guid Jobid = Guid.Parse(model.createInvoiceViewModel.EmployeeJobId.ToString());
                if (TempData["JcLITemInvoice"] != null)
                {
                    JCLItems jclob = new JCLItems();
                    List<InvoicedJCLItemMapping> jclitems = (List<InvoicedJCLItemMapping>)TempData["JcLITemInvoice"];
                    List<JcLViewModel> items = new List<JcLViewModel>();
                    foreach (var i in jclitems)
                    {
                        JcLViewModel item = new JcLViewModel();
                        var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();

                        item.Description = i.Description;
                        item.DefaultQty = i.Quantity;
                        item.Price = i.Price;
                        item.GCLID = jclitemifo.JCLId;
                        item.Id = i.ID;
                        item.Colorid = i.ColorID;
                        item.Productid = i.ProductStyleID;
                        item.Sizeid = i.SizeID;

                        item.JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                        {
                            Text = m.ItemName,
                            Value = m.JCLId.ToString(),
                            Selected = m.JCLId == item.GCLID
                        }).OrderBy(k => k.Text).ToList();

                        item.jclcolorlist = JCLColorRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                        {
                            Text = m.ColorName,
                            Value = m.ColorId.ToString(),
                            Selected = m.ColorId == item.Colorid
                        }).OrderBy(k => k.Text).ToList();

                        item.jclProductlist = JCLProductRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                        {
                            Text = m.ProductName,
                            Value = m.ProductId.ToString(),
                            Selected = m.ProductId == item.Productid
                        }).OrderBy(k => k.Text).ToList();

                        item.jclSizeList = JCLSizeRepo.FindBy(s => s.JCLId == jclitemifo.JCLId).Select(m => new SelectListItem()
                        {
                            Text = m.Size,
                            Value = m.SizeId.ToString(),
                            Selected = m.SizeId == item.Sizeid
                        }).OrderBy(k => k.Text).ToList();

                        item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                        items.Add(item);
                    }

                    jclob.JCLInfo = items;
                    //subtotal
                    decimal subtotal = Math.Round(Convert.ToDecimal(items.Sum(i => i.TotalPrice)), 2);
                    jclob.JCLInfo.ForEach(i => i.SubTotal = subtotal);
                    //GST
                    decimal GST = Math.Round(Convert.ToDecimal((subtotal * 10) / 100), 2);
                    jclob.JCLInfo.ForEach(i => i.GST = GST);
                    //GrandTotal
                    decimal GrandTotal = Math.Round((subtotal + GST), 2);
                    jclob.JCLInfo.ForEach(i => i.GrandTotal = GrandTotal);

                    jclob.JCLInfo.ForEach(i => i.JobId = Jobid);
                    model.JclMappingViewModel = jclob;

                    var PaymentList = InvoicePaymentRepo.FindBy(i => i.InvoiceId == model.createInvoiceViewModel.Id).ToList();
                    CommonMapper<InvoicePayment, InvoicePaymentList> mapperpayment = new CommonMapper<InvoicePayment, InvoicePaymentList>();
                    List<InvoicePaymentList> invoiceItemCoreViewModel1 = mapperpayment.MapToList(PaymentList.OrderBy(x => x.PaymentDate).ToList());
                    model.InvoicePaymentViewModel = invoiceItemCoreViewModel1;

                }
                return View(model);

            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET:Employee/Invoice/InvoiceList
        /// <summary>
        /// Get All Invoice List 
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult InvoiceList()
        {
            try
            {
                using (InvoiceRep)
                {
                    string JobID = "";
                    if (Request.UrlReferrer != null)
                    {
                        var QueryStringJobId = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
                        JobID = QueryStringJobId["jid"];
                    }

                    InvoiceSearchViewModel invoiceSearchViewModelPageSize = new InvoiceSearchViewModel();
                    string Searchstring = string.IsNullOrEmpty(Request.QueryString["searchkeyword"]) ? "" :
                                                (Request.QueryString["searchkeyword"]);
                    if (!string.IsNullOrEmpty(Searchstring))
                    {
                        Searchstring = Searchstring.Trim();
                    }
                    Nullable<int> CutomerType = string.IsNullOrEmpty(Request.QueryString["CustomerType"]) ? (int?)0 :
                                            Convert.ToInt32(Request.QueryString["CustomerType"]);
                    if (CutomerType > 0)
                    {

                        invoiceSearchViewModelPageSize.CustomerType = (Constant.CustomerType)Convert.ToInt32(Request.QueryString["CustomerType"]);
                    }
                    else
                    {
                        invoiceSearchViewModelPageSize.CustomerType = 0;
                    }
                    var invoice = InvoiceRep.GetinvoiceListBySearchKeyword(Searchstring, Convert.ToInt32(invoiceSearchViewModelPageSize.CustomerType)).ToList();
                    invoice = invoice.DistinctBy(m => m.EmployeeJobId).OrderByDescending(m => m.CreatedDate).ToList();

                    var JobId = Request.QueryString["jid"] != null ?
                                     Guid.Parse(Request.QueryString["jid"].ToString()) : Guid.Empty;
                    if (JobId == Guid.Empty)
                    {
                        if (!string.IsNullOrEmpty(JobID))
                        {
                            JobId = Guid.Parse(JobID);
                        }
                    }
                    if (JobId != Guid.Empty)
                    {
                        var jobNo = Employeejob.FindBy(m => m.Id == JobId).Select(m => m.JobNo).FirstOrDefault();
                        invoice = invoice.Where(m => m.JobId == jobNo).ToList();
                        var Jobs = Employeejob.FindBy(m => m.Id == JobId).FirstOrDefault();
                        invoiceSearchViewModelPageSize.searchkeyword = Jobs.JobNo.ToString();
                        invoiceSearchViewModelPageSize.JobId = Jobs.Id;
                    }


                    DateTime? StartDate = Request.QueryString["StartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["StartDate"].ToString()) ?
                                  DateTime.Parse(Request.QueryString["StartDate"]) : (DateTime?)null : (DateTime?)null;
                    DateTime? EndDate = Request.QueryString["EndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["EndDate"].ToString()) ?
                               DateTime.Parse(Request.QueryString["EndDate"]) : (DateTime?)null : (DateTime?)null;
                    Nullable<int> SentStatus = string.IsNullOrEmpty(Request.QueryString["SentStatus"]) ? (int?)0 :
                                                Convert.ToInt32(Request.QueryString["SentStatus"]);



                    if (SentStatus > 0 && SentStatus != Convert.ToInt32(Constant.InvoiceSentStatus.Unpaid) && SentStatus != Convert.ToInt32(Constant.InvoiceSentStatus.Paid))
                    {
                        invoice = invoice.Where(inv => inv.SentStatus == SentStatus).ToList();
                    }
                    else if (SentStatus == Convert.ToInt32(Constant.InvoiceSentStatus.Unpaid))
                    {
                        invoice = invoice.Where(inv => inv.Due > 0 || inv.Paid == 0 || inv.Paid == null).OrderBy(i => i.CustomerType).ToList();
                    }
                    else if (SentStatus == Convert.ToInt32(Constant.InvoiceSentStatus.Paid))
                    {
                        invoice = invoice.Where(inv => inv.Due <= 0).OrderBy(i => i.CustomerType).ToList();
                    }
                    if (StartDate.HasValue && EndDate.HasValue)
                    {
                        invoice = invoice.Where(m => (m.InvoiceSearchDate != null && m.InvoiceSearchDate >= StartDate && m.InvoiceSearchDate <= EndDate)).ToList();
                        invoiceSearchViewModelPageSize.StartDate = Convert.ToDateTime(StartDate.Value.ToShortDateString());
                        invoiceSearchViewModelPageSize.EndDate = Convert.ToDateTime(EndDate.Value.ToShortDateString());
                    }

                    else if (StartDate.HasValue)
                    {
                        invoice = invoice.Where(m => (m.InvoiceSearchDate != null && m.InvoiceSearchDate >= StartDate)).ToList();
                        invoiceSearchViewModelPageSize.StartDate = Convert.ToDateTime(StartDate.Value.ToShortDateString()); ;
                    }
                    else if (EndDate.HasValue)
                    {
                        invoice = invoice.Where(m => (m.InvoiceSearchDate != null && m.InvoiceSearchDate <= EndDate)).ToList();
                        invoiceSearchViewModelPageSize.EndDate = Convert.ToDateTime(EndDate.Value.ToShortDateString()); ;
                    }

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<CreateInvoiceCoreViewModel, CreateInvoiceViewModel> mapper = new CommonMapper<CreateInvoiceCoreViewModel, CreateInvoiceViewModel>();
                    List<CreateInvoiceViewModel> invoicelist = mapper.MapToList(invoice.ToList());
                    if (invoicelist.Count > 0)
                    {
                        invoicelist = mapper.MapToList(invoice.ToList());
                        for (int i = 0; i < invoicelist.Count; i++)
                        {
                            Guid? InvJobId = invoicelist[i].EmployeeJobId;
                            var EmployeeForjob = JobAssignMapping.FindBy(m => m.JobId == InvJobId && m.IsDelete == false).Select(m => m.AssignTo).Distinct().ToList();
                            if (EmployeeForjob != null)
                            {
                                string OtrwListforjob = "";
                                foreach (var assignTo in EmployeeForjob.ToList())
                                {
                                    string EmployeeName = Employee.FindBy(k => k.EmployeeId == assignTo).Select(j => j.FirstName + " " + j.LastName).FirstOrDefault();
                                    OtrwListforjob = OtrwListforjob + EmployeeName + ",";
                                }
                                invoicelist[i].OtrwAssignedName = OtrwListforjob.TrimEnd(',');
                            }


                            invoicelist[i].CurrentJobType = invoicelist[i].Type != null ? (Constant.JobType)(invoicelist[i].Type) : Constant.JobType.Do;
                            if (invoicelist[i].Type == 1)
                            {
                                invoicelist[i].DisplayType = "Quote";
                            }
                            else
                            {
                                invoicelist[i].DisplayType = "Invoice";
                            }
                            if (invoicelist[i].SentStatus == 1)
                            {
                                invoicelist[i].DisplaySentStatus = "Sent";
                            }
                            else
                            {
                                invoicelist[i].DisplaySentStatus = "UnSent";
                            }
                            if (invoicelist[i].Due <= 0)
                            {
                                invoicelist[i].paidStatus = "Paid";
                            }
                            else
                            {
                                invoicelist[i].paidStatus = "Unpaid";
                            }
                        }
                    }


                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 30 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);

                    invoiceSearchViewModelPageSize.PageSize = PageSize;
                    invoiceSearchViewModelPageSize.TotalCount = invoice.Count > 0 ? invoice.FirstOrDefault().TotalCount : 0;
                    if (invoiceSearchViewModelPageSize.searchkeyword == null)
                    {
                        invoiceSearchViewModelPageSize.searchkeyword = string.IsNullOrEmpty(Searchstring) ? "" : Searchstring;
                    }
                    invoiceSearchViewModelPageSize.SentStatus = (FSMConstant.Constant.InvoiceSentStatus)(SentStatus);

                    var invoiceListViewModel = new InvoiceListViewModel
                    {
                        createInvoiceViewModel = invoicelist.OrderByDescending(i => i.InvoiceDate),
                        invoiceSearchViewModel = invoiceSearchViewModelPageSize
                    };
                    return View(invoiceListViewModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //POST:Employee/Invoice/InvoiceList
        /// <summary>
        /// Search Record 
        /// </summary>
        /// <param name="invoiceSearchViewModel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult InvoiceList(InvoiceSearchViewModel invoiceSearchViewModel)
        {
            try
            {
                using (InvoiceRep)
                {
                    if (!string.IsNullOrEmpty(invoiceSearchViewModel.searchkeyword))
                    {
                        invoiceSearchViewModel.searchkeyword = invoiceSearchViewModel.searchkeyword.Trim();
                    }
                    var Invoices = InvoiceRep.GetinvoiceListBySearchKeyword(invoiceSearchViewModel.searchkeyword, Convert.ToInt32(invoiceSearchViewModel.CustomerType));
                    //     Invoices = Invoices.DistinctBy(m => m.EmployeeJobId).OrderByDescending(m => m.CreatedDate).AsQueryable();

                    //if (invoiceSearchViewModel.SentStatus != Constant.InvoiceSentStatus.Unpaid && invoiceSearchViewModel.SentStatus != Constant.InvoiceSentStatus.Paid)
                    //{
                    //    Invoices = (int)invoiceSearchViewModel.SentStatus > 0 ? Invoices.Where(invoice => invoice.SentStatus ==
                    //                                      (int)invoiceSearchViewModel.SentStatus) : Invoices;
                    //}


                    if (invoiceSearchViewModel.StartDate != null && invoiceSearchViewModel.EndDate != null)
                    {
                        //Invoices = (DateTime)invoiceSearchViewModel.StartDate != null ? Invoices.Where(m => m.InvoiceSearchDate >=
                        //                 (DateTime)invoiceSearchViewModel.StartDate && m.InvoiceSearchDate <= (DateTime)invoiceSearchViewModel.EndDate) : Invoices;
                        Invoices = Invoices.Where(m => (m.InvoiceSearchDate != null && m.InvoiceSearchDate >= invoiceSearchViewModel.StartDate && m.InvoiceSearchDate <= invoiceSearchViewModel.EndDate));
                    }

                    if (invoiceSearchViewModel.StartDate == null && invoiceSearchViewModel.EndDate != null)
                    {
                        Invoices = (DateTime)invoiceSearchViewModel.EndDate != null ? Invoices.Where(customer => customer.InvoiceSearchDate
                                    <= (DateTime)invoiceSearchViewModel.EndDate) : Invoices;
                    }
                    if (invoiceSearchViewModel.StartDate != null && invoiceSearchViewModel.EndDate == null)
                    {
                        Invoices = (DateTime)invoiceSearchViewModel.StartDate != null ? Invoices.Where(customer => customer.InvoiceSearchDate
                                 >= (DateTime)invoiceSearchViewModel.StartDate) : Invoices;
                    }
                    if (invoiceSearchViewModel.CustomerType > 0)
                    {
                        Invoices = (int)invoiceSearchViewModel.CustomerType > 0 ? Invoices.Where(invoice => invoice.CustomerType ==
                                                          (int)invoiceSearchViewModel.CustomerType) : Invoices;
                    }

                    if (invoiceSearchViewModel.SentStatus == FSMConstant.Constant.InvoiceSentStatus.Unpaid)
                    {
                        Invoices = Invoices.Where(i => i.Due > 0 || i.Paid == 0 || i.Paid == null || i.Due == null).OrderBy(i => i.CustomerType);
                    }
                    else if (invoiceSearchViewModel.SentStatus == Constant.InvoiceSentStatus.Paid)
                    {
                        Invoices = Invoices.Where(inv => inv.Due <= 0).OrderBy(inv => inv.CustomerType);
                    }
                    if (invoiceSearchViewModel.SentStatus == Constant.InvoiceSentStatus.Paid)
                    {
                        Invoices = Invoices.Where(inv => inv.Due <= 0).OrderBy(inv => inv.CustomerType); ;
                    }
                    invoiceSearchViewModel.TotalCount = Invoices.Count() > 0 ? Invoices.FirstOrDefault().TotalCount : 0;
                    CommonMapper<CreateInvoiceCoreViewModel, CreateInvoiceViewModel> mapper = new CommonMapper<CreateInvoiceCoreViewModel, CreateInvoiceViewModel>();
                    List<CreateInvoiceViewModel> invoiceViewModel = mapper.MapToList(Invoices.ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 30 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    invoiceSearchViewModel.PageSize = PageSize;

                    foreach (var inv in invoiceViewModel)
                    {
                        inv.CurrentJobType = (inv.Type) != null ? (Constant.JobType)(inv.Type) : Constant.JobType.Do;


                        if (inv.SentStatus == 1)
                        {
                            inv.DisplaySentStatus = "Sent";
                        }
                        else
                        {
                            inv.DisplaySentStatus = "UnSent";
                        }
                        if (inv.Due <= 0)
                        {
                            inv.paidStatus = "Paid";
                        }
                        else
                        {
                            inv.paidStatus = "Unpaid";
                        }

                        Guid? InvJobId = inv.EmployeeJobId;
                        var EmployeeForjob = JobAssignMapping.FindBy(m => m.JobId == InvJobId && m.IsDelete == false).Select(m => m.AssignTo).Distinct().ToList();
                        if (EmployeeForjob != null)
                        {
                            string OtrwListforjob = "";
                            foreach (var assignTo in EmployeeForjob.ToList())
                            {
                                string EmployeeName = Employee.FindBy(k => k.EmployeeId == assignTo).Select(j => j.FirstName + " " + j.LastName).FirstOrDefault();
                                OtrwListforjob = OtrwListforjob + EmployeeName + ",";
                            }
                            inv.OtrwAssignedName = OtrwListforjob.TrimEnd(',');
                        }
                    }


                    var invoiceListViewModel = new InvoiceListViewModel
                    {
                        createInvoiceViewModel = invoiceViewModel.OrderByDescending(i => i.InvoiceDate),
                        invoiceSearchViewModel = invoiceSearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets list of invoice");

                    return View(invoiceListViewModel);

                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        //POST:Employee/Invoice/DeleteInvoice
        /// <summary>
        /// Delete Invoice Record
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Redirect InvoiceList</returns>
        public ActionResult DeleteInvoice(string Id)
        {
            try
            {
                using (InvoiceRep)
                {
                    Guid InvoiceId = Guid.Parse(Id);
                    FSM.Core.Entities.Invoice invoice = InvoiceRep.FindBy(i => i.Id == InvoiceId).FirstOrDefault();
                    invoice.IsDelete = true;
                    InvoiceRep.Edit(invoice);
                    InvoiceRep.Save();

                    TempData["Message"] = 3;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted invoice");
                    return RedirectToAction("InvoiceList");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<OTRWEmployeeInvoice> GetOtrwEmployeesList()
        {
            List<OTRWEmployeeInvoice> list = new List<OTRWEmployeeInvoice>();
            using (Employee)
            {
                var role = Guid.Parse("31cf918d-b8fe-4490-b2d7-27324bfe89b4");
                var employee = Employee.FindBy(i => i.Role == role && i.IsDelete == false);
                foreach (var emp in employee)
                {
                    OTRWEmployeeInvoice obj = new OTRWEmployeeInvoice();
                    obj.EmployeeId = emp.EmployeeId;
                    obj.EmployeeName = emp.FirstName;
                    list.Add(obj);
                }
            }

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " gets otrw employee list.");

            return list;
        }
        //GET:Employee/Invoice/SendCustomerEmail
        /// <summary>
        /// Send Invoice 
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult SendCustomerEmail()
        {
            try
            {
                CustomerEmailSendViewModel customerEmailSendViewModel = new CustomerEmailSendViewModel();
                string subject = "";
                Guid? Id = Guid.Parse(Request.RequestContext.RouteData.Values["Id"].ToString());
                ViewBag.InvoiceId = Id.ToString();

                var invoices = InvoiceRep.FindBy(i => i.Id == Id).FirstOrDefault();
                Jobs jobData = new Jobs();
                if (invoices.InvoiceType == "Invoice")
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoices.JobId && m.IsDelete == false && m.JobType == 2).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }
                else
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoices.JobId && m.IsDelete == false && m.JobType == 1).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }

                if (invoices.InvoiceType == "Invoice" && jobData == null)
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoices.JobId && m.IsDelete == false).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }
                else
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoices.JobId && m.IsDelete == false).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }
                var jobid = jobData.Id;
                var SiteId = Employeejob.FindBy(i => i.Id == jobid).FirstOrDefault().SiteId;


                var jobtype = Employeejob.FindBy(i => i.Id == jobid).FirstOrDefault().JobType;

                if (invoices.InvoiceType.ToLower() == "invoice")
                {
                    subject = "Invoice No #" + invoices.InvoiceNo;
                }
                else
                {
                    subject = "Quote No #" + invoices.InvoiceNo;
                }
                if (jobid != null)
                {
                    var customergeneralInfoId = JobRepository.FindBy(i => i.Id == jobid).FirstOrDefault().CustomerGeneralInfoId;
                    var CustomerBillings = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customergeneralInfoId && m.IsDelete == false).ToList();
                    var Billinglist = CustomerBillings.Select(m => new SelectListItem()
                    {
                        Text = !String.IsNullOrEmpty(m.EmailId) ? m.EmailId : "",
                        Value = m.CustomerGeneralInfoId.ToString()
                    }).Where(m => m.Text != String.Empty && m.Text != null).Take(20).OrderBy(m => m.Text).ToList();
                    customerEmailSendViewModel.BillingContactList = Billinglist;
                    customerEmailSendViewModel.CustomerGeneralInfoId = customergeneralInfoId;
                }
                //end
                using (EmployeejobDocument)
                {
                    var jobDocuments = EmployeejobDocument.FindBy(i => i.JobId == jobid).Where(i => i.IsDelete == false); ;
                    var importantDocuments = ImpDocsRepo.GetAll().Where(i => i.IsDelete == false);
                    var siteDocuments = CustSiteDocRepo.FindBy(i => i.SiteId == SiteId).Where(i => i.IsDelete == false);
                    var customergeneralInfoId = InvoiceRep.FindBy(m => m.Id == Id).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                    var CustomerSiteFilename = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == SiteId).FirstOrDefault();
                    var CustomerBillings = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customergeneralInfoId).ToList();

                    //Customer Billing address dropdown

                    List<BillingContactList> BillingContacts = new List<BillingContactList>();
                    foreach (var item in CustomerBillings)
                    {
                        BillingContactList ob = new BillingContactList();
                        ob.Id = item.CustomerGeneralInfoId;
                        if (item.EmailId == null)
                            ob.Emailid = "";
                        else
                            ob.Emailid = item.EmailId;
                        BillingContacts.Add(ob);
                    }
                    customerEmailSendViewModel.BillingContacts = BillingContacts;
                    if (customerEmailSendViewModel.BillingContacts.Count == 1)
                    {
                        customerEmailSendViewModel.BillingEmail = customerEmailSendViewModel.BillingContacts.FirstOrDefault().Emailid;

                    }
                    if (CustomerSiteFilename != null)

                        customerEmailSendViewModel.Subject = subject + " ," + Convert.ToString(CustomerSiteFilename.Street + " " + CustomerSiteFilename.StreetName + " " + CustomerSiteFilename.Suburb + " " + CustomerSiteFilename.State + " " + CustomerSiteFilename.PostalCode);
                    else
                        customerEmailSendViewModel.Subject = subject;
                    //JobDocument Entity To View model
                    CommonMapper<JobDocuments, EmployeeJobDocumentViewModel> mapper = new CommonMapper<JobDocuments, EmployeeJobDocumentViewModel>();
                    List<EmployeeJobDocumentViewModel> customerGeneralInfoViewModel = mapper.MapToList(jobDocuments.ToList());

                    //ImportantDocument Entity To View model
                    CommonMapper<ImportantDocuments, ImportantDocumentViewModel> mapper1 = new CommonMapper<ImportantDocuments, ImportantDocumentViewModel>();
                    List<ImportantDocumentViewModel> customerImportntDocsViewModel = mapper1.MapToList(importantDocuments.ToList());

                    //SiteDocument Entity To View model
                    CommonMapper<CustomerSitesDocuments, CustomerSitesDocumentsViewModel> mapper2 = new CommonMapper<CustomerSitesDocuments, CustomerSitesDocumentsViewModel>();
                    List<CustomerSitesDocumentsViewModel> customerSiteDocsViewModel = mapper2.MapToList(siteDocuments.ToList());


                    var customerEmailSendListViewModel = new CustomerEmailSendListViewModel
                    {
                        CustomerEmailsendviewModel = customerEmailSendViewModel,
                        displayEmployeeJobDocumentViewModel = customerGeneralInfoViewModel,
                        displayImportantDocumentViewModel = customerImportntDocsViewModel,
                        displaySiteDocumentViewModel = customerSiteDocsViewModel,
                        EmployeeJobId = jobid
                    };

                    return View(customerEmailSendListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        //POST:Employee/Invoice/SendCustomerEmail
        /// <summary>
        /// Send Invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Json</returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SendCustomerEmail(List<CustomerEmailSendViewModel> model)
        {
            try
            {
                // for from
                var From = Request.Form.Get("from");
                var FromEmail = Request.Form.Get("fromEmail");
                var To = Request.Form.Get("to");
                var Subject = Request.Form.Get("subject");
                var Docid = Request.Form.Get("docid");
                var SiteDocid = Request.Form.Get("Sitedocid");
                var ImpDocid = Request.Form.Get("Impdocid");
                var CC = Request.Form.Get("Cc");
                var BCC = Request.Form.Get("Bcc");
                var Templatedata = Request.Form.Get("TemplateData");
                string InvoiceId = Request.Form.Get("INVOICEID");
                var Template = Request.Form.Get("Template");
                Guid invoiceid = Guid.Parse(InvoiceId);
                var employeeJobId = Request.Form.Get("EmployeeJobId");
                Guid jobId = Guid.Parse(employeeJobId);
                string[] namesArray = Docid.Split(',');
                string[] namesArraySite = SiteDocid.Split(',');
                string[] namesArrayImp = ImpDocid.Split(',');

                FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == invoiceid).FirstOrDefault();
                bool invoiceAlreadySent = InvoiceData.SentStatus != null ? InvoiceData.SentStatus == 1 ? true : false : false;

                var employeeJob = Employeejob.FindBy(m => m.Id == jobId).FirstOrDefault();
                bool bookNextContractJob = false;
                var customer = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == employeeJob.SiteId).FirstOrDefault();
                if ((customer.Contracted != null && customer.Contracted != 0) && employeeJob.JobType == 2)
                {
                    bookNextContractJob = true;
                }

                // mapping entity to viewmodel
                CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
                CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
                invoiceViewModel = mapper.Mapper(InvoiceData);

                //Invoice File Export Path Get
                var root = Server.MapPath("~/InvoiceEmailPdf/");
                var pdfname = "";
                if (InvoiceData.InvoiceType == "Quote")
                {
                    pdfname = String.Format("{0}.pdf", "Quote No #" + InvoiceData.InvoiceNo);
                }
                else
                {
                    pdfname = String.Format("{0}.pdf", "Invoice No #" + InvoiceData.InvoiceNo);
                }
                var path = Path.Combine(root, pdfname);
                path = Path.GetFullPath(path);

                //For jobcheckbox file
                var idget = namesArray.ToList();
                List<string> attachfiles = new List<string>();
                if (Docid != null && Docid != "")
                {
                    using (EmployeejobDocument)
                    {
                        foreach (var doc in idget)
                        {
                            if (doc != "")
                            {
                                Guid docid = Guid.Parse(doc);
                                var file = EmployeejobDocument.FindBy(i => i.Id == docid).FirstOrDefault();
                                attachfiles.Add("/Images/JobDocuments/" + file.JobId + '/' + file.SaveDocName);
                            }
                        }
                    }
                }

                //For Sitecheckbox file
                var siteDocidget = namesArraySite.ToList();
                List<string> attachfilesSite = new List<string>();
                if (SiteDocid != null && SiteDocid != "")
                {
                    using (CustSiteDocRepo)
                    {
                        foreach (var sitedoc in siteDocidget)
                        {
                            if (sitedoc != "")
                            {
                                Guid docid = Guid.Parse(sitedoc);
                                var file = CustSiteDocRepo.FindBy(i => i.DocumentId == docid).FirstOrDefault();
                                attachfilesSite.Add("/Images/CustomerDocs/" + docid + '/' + file.DocumentName);
                            }
                        }
                    }
                }

                //For Importantcheckbox file
                var ImpDocidget = namesArrayImp.ToList();
                List<string> attachfilesImportnt = new List<string>();
                if (ImpDocid != null && ImpDocid != "")
                {
                    using (ImpDocsRepo)
                    {
                        foreach (var impdoc in ImpDocidget)
                        {
                            if (impdoc != "")
                            {
                                Guid docid = Guid.Parse(impdoc);
                                var file = ImpDocsRepo.FindBy(i => i.Id == docid).FirstOrDefault();
                                attachfilesImportnt.Add("/" + file.FilePath);
                            }
                        }
                    }
                }

                string myString = "";
                int Templateused = Convert.ToInt32(Template);
                myString = GetSelectedTemplateData(Templateused);
                string body = Convert.ToString(Templatedata);
                using (MailMessage mm = new MailMessage())
                {
                    mm.Subject = Subject;
                    mm.IsBodyHtml = true;
                    mm.Body = body;
                    mm.From = new MailAddress(FromEmail);
                    string[] TOId = To.Split(',');
                    foreach (string ToEmail in TOId)
                    {
                        mm.To.Add(new MailAddress(ToEmail)); //Adding Multiple To email Id
                    }
                    string[] CCId = CC.Split(',');

                    if (CCId[0] != null && CCId[0] != "")
                    {
                        foreach (string CCEmail in CCId)
                        {
                            mm.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                        }
                    }


                    //For File Uploade
                    if (Request.Files != null && Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            HttpPostedFileBase file = Request.Files[i];
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var attachment = new Attachment(file.InputStream, fileName);
                                mm.Attachments.Add(attachment);
                            }
                        }
                    }


                    if (Docid != null && Docid != "")
                    {
                        foreach (string fileName in attachfiles)
                        {
                            if (System.IO.File.Exists(Request.PhysicalApplicationPath + fileName))
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(Request.PhysicalApplicationPath + fileName);
                                attachment.Name = fileName.Split('/').Last();
                                string extensiong = fileName.Split('.').Last();
                                attachment.Name = attachment.Name.Remove(attachment.Name.LastIndexOf('_')) + '.' + extensiong;
                                //attachment.Name = fileName.Split('/').Last();  // set name here
                                mm.Attachments.Add(attachment);
                            }
                        }
                    }

                    //Invoice Attachment
                    mm.Attachments.Add(new Attachment(path));

                    if (SiteDocid != null && SiteDocid != "")
                    {
                        foreach (string fileName in attachfilesSite)
                        {
                            if (System.IO.File.Exists(Request.PhysicalApplicationPath + fileName))
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(Request.PhysicalApplicationPath + fileName);
                                attachment.Name = fileName.Split('/').Last();  // set name here
                                mm.Attachments.Add(attachment);
                            }
                        }
                    }
                    if (ImpDocid != null && ImpDocid != "")
                    {
                        foreach (string fileName in attachfilesImportnt)
                        {
                            if (System.IO.File.Exists(Request.PhysicalApplicationPath + fileName))
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(Request.PhysicalApplicationPath + fileName);
                                attachment.Name = fileName.Split('/').Last();
                                string extensiong = fileName.Split('.').Last();
                                attachment.Name = attachment.Name.Remove(attachment.Name.LastIndexOf('_')) + '.' + extensiong;
                                // set name here
                                mm.Attachments.Add(attachment);
                            }
                        }
                    }
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                        smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                        smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                        smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        //  Email copy saving section  Kamal
                        string FilePath = "";
                        if (smtp.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                        {
                            string root1 = AppDomain.CurrentDomain.BaseDirectory;
                            string pickupRoot = smtp.PickupDirectoryLocation.Replace("~/", root1);
                            pickupRoot = root1 + pickupRoot;
                            pickupRoot = pickupRoot.Replace("/", @"\");
                            FilePath = pickupRoot;
                            smtp.PickupDirectoryLocation = pickupRoot;
                        }
                        await smtp.SendMailAsync(mm);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtp.SendMailAsync(mm);
                        using (InvoiceRep)
                        {
                            var invoice = InvoiceRep.FindBy(i => i.Id == invoiceid).FirstOrDefault();
                            invoice.SentStatus = (int)(FSMConstant.Constant.InvoiceSentStatus.Sent);
                            invoice.SentDate = DateTime.Now.Date;
                            InvoiceRep.Edit(invoice);
                            InvoiceRep.Save();
                        }

                        //  getting latest added .eml file  Kamal
                        var directory = new DirectoryInfo(FilePath);
                        var myFile = (from f in directory.GetFiles("*.eml")
                                      orderby f.LastWriteTime descending
                                      select f).First();


                        //invoice sent successfully save data in contact log
                        var employeejob = Employeejob.FindBy(m => m.Id == jobId).FirstOrDefault();   //get job data
                        var customerInfo = CustomerGeneralInfo.FindBy(m => m.CustomerGeneralInfoId == employeejob.CustomerGeneralInfoId).FirstOrDefault();  //get customer data

                        CustomerContactLog customerContactLog = new CustomerContactLog();
                        customerContactLog.CustomerContactId = Guid.NewGuid();
                        customerContactLog.CustomerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                        customerContactLog.CustomerId = (customerInfo.CTId).ToString();
                        customerContactLog.JobId = employeejob.Id.ToString();

                        if (employeeJob.JobType == 1)


                            customerContactLog.Note = "<a href='http://www.srag-portal.com/InvoiceEmailPdf/" + myFile + "' target=_blank>Quote sent successfully !" + myFile + "</a>";
                        else
                            customerContactLog.Note = "<a href='http://www.srag-portal.com/InvoiceEmailPdf/" + myFile + "' target=_blank>Invoice sent successfully !" + myFile + "</a>";
                        customerContactLog.LogDate = DateTime.Now;
                        customerContactLog.IsDelete = false;
                        customerContactLog.CreatedDate = DateTime.Now;
                        customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);

                        CustomercontactLogRepo.Add(customerContactLog);
                        CustomercontactLogRepo.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " sent email to a cutomer for invoice");
                        return Json(new { success = true, responseText = "Your message successfuly sent!", invoiceAlreadySent = invoiceAlreadySent, jobId = employeeJobId, bookNextContractJob = bookNextContractJob }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                InvoiceRep.Dispose();
            }
        }

        private string GetSelectedTemplateData(int Templateused)
        {
            try
            {
                string myString = "";
                StreamReader reader = null;
                string readFile = "";
                if (Templateused != 0)
                {

                    Constant.EmailTemplates template = (Constant.EmailTemplates)Templateused;
                    switch (template)
                    {
                        case Constant.EmailTemplates.invoice_domestic_clean:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceDomesticClean.html"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;

                        case Constant.EmailTemplates.invoice_domestic_roofing:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceDomesticRoofing.html"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;

                        case Constant.EmailTemplates.invoice_other:

                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceOther.html"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Invoice_overdue_payment:

                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceOverduePayment.html"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Quote_Other_Work_Found:

                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/QuoteOtherWorkFound.html"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Quote_Requested:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/QuoteRequested.html"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Confirmation_of_Appointment_email_Domestic_customer:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ConfirmationAppointment(DomesticCustomer).htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Confirmation_of_Appointment_email_Strata_or_Real_estate:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ConfirmationAppointment(StrataRealestate).htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Deposit_Request:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/DepositRequest.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Invoice_Domestic_OutStanding_Stage1:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceDomesticOutStandingStage1.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Invoice_Domestic_OutStanding_Stage2:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceDomesticOutStandingStage2.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Invoice_Strata_OutStanding_Stage2:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceStrataOutStandingStage2.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Invoice_Strata_OutStanding_Stage3:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceStrataOutStandingStage3.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Invoice_Strata_OutStanding_Stage1:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceStrataOutStandingStage1.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Contracted_Gutter_Clean_SRAS_Needed:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ContractedGutterCleanSRASNeeded.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                        case Constant.EmailTemplates.Contracted_Gutter_Clean_Price_Increase:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ContractedGutterCleanPriceIncrease.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            break;
                    }

                }
                return myString;

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult _SaveNextContractJob(string JobId)
        {
            try
            {
                Guid id;
                ContractJobScheduleViewModel contractJobScheduleViewModel = new ContractJobScheduleViewModel();
                if (!string.IsNullOrEmpty(JobId))
                {
                    id = Guid.Parse(JobId);
                    contractJobScheduleViewModel.JobId = id;
                    contractJobScheduleViewModel.NextContractScheduleDate = getNextContractDueDate(id);
                }
                return PartialView(contractJobScheduleViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult _SaveNextContractJob(ContractJobScheduleViewModel contractJobScheduleViewModel)
        {
            try
            {
                var employeejob = Employeejob.FindBy(m => m.Id == contractJobScheduleViewModel.JobId).FirstOrDefault();

                // jobviewmodel to jobentity
                CommonMapper<Jobs, JobsViewModel> mapper1 = new CommonMapper<Jobs, JobsViewModel>();
                var jobsViewModel = mapper1.Mapper(employeejob);
                jobsViewModel.Id = Guid.NewGuid();
                jobsViewModel.JobId = JobRepository.GetMaxJobNo();
                jobsViewModel.JobNo = JobRepository.GetMaxJobNo();
                jobsViewModel.Status = JobStatus.Booked;
                jobsViewModel.OperationNotes = "";
                jobsViewModel.OTRWjobNotes = "";
                jobsViewModel.DateBooked = contractJobScheduleViewModel.NextContractScheduleDate;
                jobsViewModel.NotificationType = (int)contractJobScheduleViewModel.JobNotificationType;
                jobsViewModel.CreatedDate = DateTime.Now;
                jobsViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                // modified viewmodel to entity
                CommonMapper<JobsViewModel, Jobs> mapper2 = new CommonMapper<JobsViewModel, Jobs>();
                var jobs = mapper2.Mapper(jobsViewModel);

                Employeejob.Add(jobs);
                Employeejob.Save();


                CustomerContactLog customerContactLog = new CustomerContactLog();
                customerContactLog.CustomerContactId = Guid.NewGuid();
                customerContactLog.CustomerGeneralInfoId = jobsViewModel.CustomerGeneralInfoId;
                customerContactLog.CustomerId = Guid.Parse(base.GetUserId).ToString();
                customerContactLog.JobId = jobsViewModel.Id.ToString();

                customerContactLog.LogDate = DateTime.Now;
                customerContactLog.Note = "Contracted job: " + jobsViewModel.JobNo + " created successfully";
                customerContactLog.IsDelete = false;
                customerContactLog.IsReminder = true;
                customerContactLog.IsScheduler = true;
                customerContactLog.CreatedDate = DateTime.Now;
                customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);
                CustomercontactLogRepo.Add(customerContactLog);
                CustomercontactLogRepo.Save();


                return Json(new { success = "Next contract job created successfully" });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult _CancelNextContractJob(string JobId)
        {
            try
            {
                Guid id;
                CancelJobScheduleViewModel contractJobScheduleViewModel = new CancelJobScheduleViewModel();
                if (!string.IsNullOrEmpty(JobId))
                {
                    id = Guid.Parse(JobId);
                    contractJobScheduleViewModel.JobId = id;
                }
                return PartialView(contractJobScheduleViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult _CancelNextContractJob(CancelJobScheduleViewModel cancelJobScheduleViewModel)
        {
            try
            {
                CancelJobScheduleViewModel contractJobScheduleViewModel = new CancelJobScheduleViewModel();
                var employeejob = Employeejob.FindBy(m => m.Id == cancelJobScheduleViewModel.JobId).FirstOrDefault();

                CustomerContactLog customerContactLog = new CustomerContactLog();
                customerContactLog.CustomerContactId = Guid.NewGuid();
                customerContactLog.CustomerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                customerContactLog.JobId = employeejob.Id.ToString();
                customerContactLog.Note = cancelJobScheduleViewModel.Reason;
                customerContactLog.LogDate = DateTime.Now;
                customerContactLog.CreatedDate = DateTime.Now;
                customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);

                CustomercontactLogRepo.Add(customerContactLog);
                CustomercontactLogRepo.Save();

                return Json(new { success = "Cancel reason saved successfully" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Nullable<DateTime> getNextContractDueDate(Guid id)
        {
            var employeejob = Employeejob.FindBy(m => m.Id == id).FirstOrDefault();
            var jobContract = employeejob.CustomerSiteDetail.Contracted;
            var jobBookedDate = employeejob.DateBooked;
            Nullable<DateTime> nextContractDate = jobBookedDate;

            // adding months in next due date
            switch (jobContract)
            {
                case 1:
                    nextContractDate = nextContractDate.Value.AddMonths(1);
                    break;
                case 2:
                    nextContractDate = nextContractDate.Value.AddMonths(2);
                    break;
                case 3:
                    nextContractDate = nextContractDate.Value.AddMonths(3);
                    break;
                case 4:
                    nextContractDate = nextContractDate.Value.AddMonths(4);
                    break;
                case 5:
                    nextContractDate = nextContractDate.Value.AddMonths(6);
                    break;
                default:
                    nextContractDate = nextContractDate.Value.AddMonths(12);
                    break;
            }

            // adding days in next due date
            var nextContractDateDay = nextContractDate.Value.ToString("dddd");
            if (nextContractDateDay == "Friday")
            {
                nextContractDate = nextContractDate.Value.AddDays(3);
            }
            else if (nextContractDateDay == "Saturday")
            {
                nextContractDate = nextContractDate.Value.AddDays(2);
            }
            else
            {
                nextContractDate = nextContractDate.Value.AddDays(1);
            }

            return nextContractDate;
        }

        public ActionResult GetExportFile(Guid InvoiceId)
        {
            int CustomerType = 0;
            string CustomerName = "";
            List<InvoicePaymentList> QuotepaymentInfo = new List<InvoicePaymentList>();
            FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == InvoiceId).FirstOrDefault();
            var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
            // mapping entity to viewmodel
            CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
            CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
            invoiceViewModel = mapper.Mapper(InvoiceData);
            invoiceViewModel.InvcDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : string.Empty;

            var customer = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

            invoiceViewModel.TradeName = "";
            if (customer != null)
            {
                CustomerType = customer.CustomerType.HasValue ? Convert.ToInt32(customer.CustomerType) : 0;
            }

            var jobdetail = JobRepository.FindBy(m => m.JobNo == InvoiceData.JobId && m.IsDelete == false).OrderByDescending(m => m.CreatedDate).FirstOrDefault();
            var sitedetail = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
            if (jobdetail != null)
            {
                invoiceViewModel.WorkOrderNumber = !String.IsNullOrEmpty(jobdetail.WorkOrderNumber) ? jobdetail.WorkOrderNumber : "";
            }
            else
            {
                invoiceViewModel.WorkOrderNumber = "";
            }
            dynamic billingdetail = null;
            var BillingAddressId = InvoiceData.BillingAddressId;
            if (BillingAddressId != null)
            {
                if (BillingAddressId != Guid.Empty)
                {
                    billingdetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                }
                else
                {
                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                }
            }
            else
            {
                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
            }

            if (billingdetail == null)
            {

                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

            }
            if (customer != null)
            {
                var TradingName = customer.TradingName;
                if (!string.IsNullOrEmpty(TradingName))
                {
                    invoiceViewModel.TradeName = !String.IsNullOrEmpty(customer.TradingName) ? customer.TradingName : "";

                }
                CustomerName = !String.IsNullOrEmpty(customer.CustomerLastName) ? customer.CustomerLastName : "";
            }

            invoiceViewModel.DisplaysiteAddress =
                                               (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
            if (billingdetail == null)
            {

                invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else
            {

                invoiceViewModel.DisplayBillingAddress =
                                                        (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                        (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                        (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                        (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                        (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                        (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");

            }
            if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Domestic))
            {
                if (billingdetail == null)
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.RealState) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Strata) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Commercial))
            {
                invoiceViewModel.DisplaysiteAddress =

                                             (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                if (billingdetail == null)
                {
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else
                {
                    invoiceViewModel.DisplayBillingAddress = (!string.IsNullOrEmpty(invoiceViewModel.TradeName) ? invoiceViewModel.TradeName + "\n" : "") +
                         (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                    invoiceViewModel.DisplaysiteAddress = (!string.IsNullOrEmpty(sitedetail.StrataPlan) ? "SP:" + sitedetail.StrataPlan + "\n" : "") +
                                                                invoiceViewModel.DisplaysiteAddress;
                }
            }



            if (InvoiceData.CreatedBy != null)
            {
                invoiceViewModel.PreparedBy = Employee.FindBy(i => i.EmployeeId == InvoiceData.CreatedBy).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault();
            }
            //Get Jcl Item Total Price
            JCLItems jclob = new JCLItems();
            var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoiceData.Id).ToList();
            var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId && i.InvoiceId == InvoiceData.Id).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);
            List<JcLViewModel> items = new List<JcLViewModel>();
            foreach (var i in InvoiceJCLItemlist)
            {
                JcLViewModel item = new JcLViewModel();
                var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                item.DefaultQty = i.Quantity;
                item.Description = i.Description;
                item.Price = i.Price;
                item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                items.Add(item);
            }
            jclob.JCLInfo = items;
            //subtotal
            decimal subtotal = Convert.ToDecimal(items.Sum(i => i.TotalPrice));
            invoiceViewModel.Price = subtotal;
            invoiceViewModel.PriceWithGst = (subtotal + (subtotal * 10) / 100);

            if (invoiceViewModel.Paid != null)
            {
                if (invoiceViewModel.Paid > 0)
                {
                    invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst - invoiceViewModel.Paid;
                }
                else
                {
                    invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
                }
            }
            else
            {
                invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
            }
            invoiceViewModel.AmountPay = InvoiceData.AmountPaid;
            invoiceViewModel.GST = Math.Round((Convert.ToDecimal(subtotal * 10) / 100), 2);
            if (invoiceViewModel.Paid != null)
            {
                if (invoiceViewModel.Paid > 0)
                {
                    invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst - invoiceViewModel.Paid;
                }
                else
                {
                    invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
                }
            }
            else
            {
                invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
            }
            //Quote payment history

            var QuotePaymenthistory = InvoicePaymentRepo.FindBy(i => i.InvoiceId == InvoiceId).ToList();
            foreach (var payment in QuotePaymenthistory)
            {
                InvoicePaymentList paymentinfo = new InvoicePaymentList();
                paymentinfo.Id = payment.Id;
                paymentinfo.PaymentDate = payment.PaymentDate;
                paymentinfo.payment_Date = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                paymentinfo.PaymentAmount = payment.PaymentAmount;
                if (payment.PaymentMethod != null)
                {
                    Constant.PaymentMethod Durations = (Constant.PaymentMethod)payment.PaymentMethod;
                    var displaynamee = Durations.GetAttribute<DisplayAttribute>();
                    if (displaynamee != null)
                        paymentinfo.payment_Method = displaynamee.Name.ToString();
                }
                paymentinfo.Reference = payment.Reference;
                paymentinfo.PaymentNote = payment.PaymentNote;
                QuotepaymentInfo.Add(paymentinfo);

            }

            var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
            {
                JclMappingViewModel = jclob,
                createInvoiceViewModel = invoiceViewModel,
                InvoicePaymentViewModel = QuotepaymentInfo
            };

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " export pdf of invoice");

            //invoice Root Path
            var root = Server.MapPath("~/InvoiceEmailPdf/");
            var pdfname = "";
            if (InvoiceData.InvoiceType == "Quote")
            {
                pdfname = String.Format("{0}.pdf", "Quote No #" + InvoiceData.InvoiceNo);
            }
            else
            {
                pdfname = String.Format("{0}.pdf", "Invoice No #" + InvoiceData.InvoiceNo);
            }

            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            if (jobdetail.JobType == 1)
            {
                var Quote = new Rotativa.ViewAsPdf("QuoteExportPreview", custoemrInvoiceListViewmodel)
                {
                    FileName = "JobQuote.pdf",
                    SaveOnServerPath = path,
                };
                return Quote;
            }

            var something = new Rotativa.ViewAsPdf("InvoiceExportPreview", custoemrInvoiceListViewmodel)
            {
                FileName = "JobInvoice.pdf",
                SaveOnServerPath = path,
            };

            return something;

        }
        public ActionResult DownloadFile(string Id)
        {
            try
            {
                Guid docId = Guid.Parse(Id);
                var jobid = EmployeejobDocument.FindBy(i => i.Id == docId).FirstOrDefault().JobId;
                var ImageName = EmployeejobDocument.FindBy(i => i.Id == docId).FirstOrDefault();
                //var FileVirtualPath = "/Images/EmployeeJobs/" + jobid + '/' + ImageName.SaveDocName;
                var FileVirtualPath = "/Images/JobDocuments/" + jobid + '/' + ImageName.SaveDocName;
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download a employee job document");

                return Json(FileVirtualPath, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult DownloadImportantDocsFile(string Id)
        {
            try
            {
                Guid docId = Guid.Parse(Id);
                var ImageList = ImpDocsRepo.FindBy(i => i.Id == docId).FirstOrDefault();
                string ImageName = Path.GetFileNameWithoutExtension(ImageList.FileName);
                string extension = Path.GetExtension(ImageList.FileName);
                var FileVirtualPath = "/" + ImageList.FilePath;
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download important document");
                return Json(FileVirtualPath, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult DownloadSiteDocsFile(string Id)
        {
            try
            {
                Guid docId = Guid.Parse(Id);
                var ImageList = CustSiteDocRepo.FindBy(i => i.DocumentId == docId).FirstOrDefault();
                string ImageName = Path.GetFileNameWithoutExtension(ImageList.DocumentName);
                string extension = Path.GetExtension(ImageList.DocumentName);
                var FileVirtualPath = "/Images/CustomerDocs/" + docId + '/' + ImageName + extension;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download customer document");

                return Json(FileVirtualPath, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Add Purchase Order 
        /// </summary>
        /// <param name="Jobid"></param>
        /// <param name="InvoiceId"></param>
        public ActionResult AddPurchaseOrder(string JobId, string InvoiceId)
        {
            try
            {
                using (Employeejob)
                {
                    Guid id = Guid.Parse(JobId);
                    var employeerJobList = Employeejob.FindBy(i => i.Id == id).AsEnumerable();

                    GetJobViewModel getJobViewModel = new GetJobViewModel();
                    List<EmployeeJobDetail> li = new List<EmployeeJobDetail>();
                    foreach (var i in employeerJobList)
                    {
                        EmployeeJobDetail obj = new EmployeeJobDetail();
                        obj.EmployeeJobId = i.Id;
                        obj.JobNo = i.JobNo;
                        obj.Description = "JobNo_" + obj.JobNo;
                        li.Add(obj);
                    }
                    getJobViewModel.employeeJobDetail = li;
                    if (!string.IsNullOrEmpty(JobId))
                    {
                        TempData["jobid"] = JobId;
                        TempData.Keep("jobid");
                    }

                    if (!string.IsNullOrEmpty(InvoiceId))
                    {
                        TempData["Invoiceid"] = InvoiceId;
                        TempData.Keep("Invoiceid");
                    }


                    getJobViewModel.PurchaseOrderNo = JobPurchaseOrder.GetMaxPurchaseNo();

                    return View(getJobViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult AddPurchaseOrder(List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values)
        {
            SavePurchaseDetail(values, "");
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public void SavePurchaseDetail(List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values, string InvoiceId)
        {
            try
            {
                string Purchaseid = values[0].purchaseId;
                PurchaseOrderByJobviewmodel purchaseOrderByJobviewmodel = new PurchaseOrderByJobviewmodel();
                if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                {
                    Guid id = Guid.Parse(Purchaseid);
                    var purchasestock = JobPurchaseOrder.FindBy(i => i.ID == id).FirstOrDefault();
                    purchasestock.SupplierID = Guid.Parse(values[0].Supplierid);
                    purchasestock.Description = values[0].Description;
                    if (!String.IsNullOrEmpty(InvoiceId))
                    {
                        purchasestock.InvoiceId = Guid.Parse(InvoiceId);
                    }
                    purchasestock.Cost = Convert.ToDecimal(values[0].Cost);
                    if (!String.IsNullOrEmpty(values[0].JobId))
                        purchasestock.JobID = Guid.Parse(values[0].JobId);
                    purchasestock.ModifiedBy = Guid.Parse(base.GetUserId);
                    purchasestock.ModifiedDate = DateTime.Now;
                    JobPurchaseOrder.Edit(purchasestock);
                    JobPurchaseOrder.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated purchage stock");

                    TempData["Message"] = 2;
                }

                else
                {
                    purchaseOrderByJobviewmodel.ID = Guid.NewGuid();
                    purchaseOrderByJobviewmodel.SupplierID = Guid.Parse(values[0].Supplierid);
                    purchaseOrderByJobviewmodel.Description = values[0].Description;
                    if (!(String.IsNullOrEmpty(InvoiceId)))
                    {
                        purchaseOrderByJobviewmodel.InvoiceId = Guid.Parse(InvoiceId);
                    }
                    purchaseOrderByJobviewmodel.Cost = Convert.ToDecimal(values[0].Cost);
                    if (!String.IsNullOrEmpty(values[0].JobId))
                        purchaseOrderByJobviewmodel.JobID = Guid.Parse(values[0].JobId);
                    purchaseOrderByJobviewmodel.CreatedBy = Guid.Parse(base.GetUserId);
                    purchaseOrderByJobviewmodel.CreatedDate = DateTime.Now;
                    CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob> mapper = new CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob>();
                    PurchaseOrderByJob purchasejobinfo = mapper.Mapper(purchaseOrderByJobviewmodel);
                    JobPurchaseOrder.Add(purchasejobinfo);
                    JobPurchaseOrder.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added new purchage stock");



                    TempData["Message"] = 1;
                }
                //delete item if not exist while updating
                Guid purchaseitemId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                var pitems = JobPurchaseOrderitem.FindBy(i => i.PurchaseOrderID == purchaseitemId).ToList();
                foreach (var item in pitems)
                {
                    string iteminfoid = Convert.ToString(item.ID.ToString());
                    bool pos = Array.Exists(values.ToArray(), element => element.StockId == iteminfoid);
                    if (!pos)
                    {
                        if (!String.IsNullOrEmpty(iteminfoid))
                        {
                            var item_id = Guid.Parse(iteminfoid);
                            var itemtodelete = JobPurchaseOrderitem.FindBy(i => i.ID == item_id && i.PurchaseOrderID == purchaseitemId).FirstOrDefault();
                            JobPurchaseOrderitem.Delete(itemtodelete);
                            JobPurchaseOrderitem.Save();
                        }
                    }
                }

                foreach (var stockitem in values)
                {
                    if (stockitem.StockId == null)
                    {
                        stockitem.StockId = Guid.Empty.ToString();
                    }
                    Guid itemorderId = Guid.Empty;
                    if (stockitem.ItemId != null)
                    {
                        itemorderId = Guid.Parse(stockitem.ItemId.ToString());

                    }
                    Guid itempurchasdId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                    var checkExist = JobPurchaseOrderitem.FindBy(i => i.ID == itemorderId && i.PurchaseOrderID == itempurchasdId).FirstOrDefault();
                    if (checkExist != null)
                    {
                        var itemtoupdate = checkExist;
                        itemtoupdate.PurchaseItem = Convert.ToString(stockitem.PurchaseItem);
                        itemtoupdate.UnitOfMeasure = Convert.ToString(stockitem.UnitMeasure);
                        itemtoupdate.Price = Convert.ToDecimal(stockitem.Price);
                        itemtoupdate.Quantity = Convert.ToInt32(stockitem.Quantity);
                        itemtoupdate.ModifiedBy = Guid.Parse(base.GetUserId);
                        itemtoupdate.ModifiedDate = DateTime.Now;
                        JobPurchaseOrderitem.Edit(itemtoupdate);
                        JobPurchaseOrderitem.Save();
                    }
                    else
                    {
                        PurchaseorderItemJobViewModel purchaseOrderITemByjobViewModel = new PurchaseorderItemJobViewModel();
                        purchaseOrderITemByjobViewModel.ID = Guid.NewGuid();
                        if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                        {
                            purchaseOrderITemByjobViewModel.PurchaseOrderID = Guid.Parse(Purchaseid);
                        }
                        else { purchaseOrderITemByjobViewModel.PurchaseOrderID = purchaseOrderByJobviewmodel.ID; }
                        purchaseOrderITemByjobViewModel.StockID = Guid.Parse(stockitem.StockId);
                        purchaseOrderITemByjobViewModel.PurchaseItem = stockitem.PurchaseItem;
                        purchaseOrderITemByjobViewModel.UnitOfMeasure = stockitem.UnitMeasure;
                        purchaseOrderITemByjobViewModel.Quantity = Convert.ToInt32(stockitem.Quantity);
                        purchaseOrderITemByjobViewModel.Price = Convert.ToDecimal(stockitem.Price);
                        purchaseOrderITemByjobViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        purchaseOrderITemByjobViewModel.CreatedDate = DateTime.Now;
                        CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob> mapper = new CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob>();
                        PurchaseorderItemJob purchasejobiteminfo = mapper.Mapper(purchaseOrderITemByjobViewModel);
                        JobPurchaseOrderitem.Add(purchasejobiteminfo);
                        JobPurchaseOrderitem.Save();
                    }
                }

                TempData["PurchaseOrderDetail"] = null;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                JobPurchaseOrderitem.Dispose();
            }

        }
        [HttpGet]
        public ActionResult GetInvoicePurchaseItemByJobId(string jobid)
        {
            try
            {
                string Purchaseorderid = "";
                using (JobPurchaseOrder)
                {
                    Guid jobId = Guid.Parse(jobid);
                    var purchaseorder = JobPurchaseOrder.FindBy(i => i.JobID == jobId).FirstOrDefault();
                    if (purchaseorder != null)
                    {
                        Purchaseorderid = Convert.ToString(purchaseorder.ID);
                    }
                }
                using (JobPurchaseOrderitem)
                {
                    if (!String.IsNullOrEmpty(Purchaseorderid))
                    {
                        Guid Id = Guid.Parse(Purchaseorderid);
                        var purchaseitem = JobPurchaseOrderitem.GetPurchaseorderItemJobs(Id.ToString());
                        CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel>();
                        List<PurchaseDatajobviewModel> purchaseitemviewmodel = mapper.MapToList(purchaseitem.ToList());
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(purchaseitemviewmodel);
                        return Json(new { list = json, length = purchaseitemviewmodel.Count() }, JsonRequestBehavior.AllowGet);
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets invoice item by job id");


                    return Json(new { list = false, length = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult InvoiceExport(string id)
        {
            int CustomerType = 0;
            string CustomerName = "";
            string siteAddress = "";
            Guid? Id = Guid.Parse(id);
            FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == Id).FirstOrDefault();
            var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
            //mapping entity to viewmodel
            CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
            CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
            invoiceViewModel = mapper.Mapper(InvoiceData);
            invoiceViewModel.InvcDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : string.Empty;

            var jobdetail = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).FirstOrDefault();
            var sitedetail = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
            if (jobdetail != null)
            {
                invoiceViewModel.WorkOrderNumber = !String.IsNullOrEmpty(jobdetail.WorkOrderNumber) ? jobdetail.WorkOrderNumber : "";
            }
            else
            {
                invoiceViewModel.WorkOrderNumber = "";
            }

            var customer = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();   //get customer detail

            invoiceViewModel.TradeName = "";
            if (customer != null)
            {
                CustomerType = customer.CustomerType.HasValue ? Convert.ToInt32(customer.CustomerType) : 0;
            }
            dynamic billingdetail = null;
            var BillingAddressId = InvoiceData.BillingAddressId;
            if (BillingAddressId != null)
            {
                if (BillingAddressId != Guid.Empty)
                {
                    billingdetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                }
                else
                {
                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                }
            }
            else
            {
                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
            }

            if (billingdetail == null)
            {

                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

            }
            if (customer != null)
            {
                var TradingName = customer.TradingName;
                if (!string.IsNullOrEmpty(TradingName))
                {
                    invoiceViewModel.TradeName = !String.IsNullOrEmpty(customer.TradingName) ? customer.TradingName : "";

                }
                CustomerName = !String.IsNullOrEmpty(customer.CustomerLastName) ? customer.CustomerLastName : "";
            }

            invoiceViewModel.DisplaysiteAddress =
                                               (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
            if (billingdetail == null)
            {

                invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else
            {

                invoiceViewModel.DisplayBillingAddress =

                                                         (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");

            }
            if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Domestic))
            {
                if (billingdetail == null)
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.RealState) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Strata) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Commercial))
            {
                invoiceViewModel.DisplaysiteAddress =

                                             (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");

                if (billingdetail == null)
                {
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else
                {
                    invoiceViewModel.DisplayBillingAddress = (!string.IsNullOrEmpty(invoiceViewModel.TradeName) ? invoiceViewModel.TradeName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                }
                invoiceViewModel.DisplaysiteAddress = (!string.IsNullOrEmpty(sitedetail.StrataPlan) ? "SP:" + sitedetail.StrataPlan + "\n" : "") +
                                                          (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
            }


            //Get Jcl Item Total Price
            JCLItems jclob = new JCLItems();
            var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoiceData.Id).ToList();
            var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId && i.InvoiceId == InvoiceData.Id).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);
            List<JcLViewModel> items = new List<JcLViewModel>();
            foreach (var i in InvoiceJCLItemlist)
            {
                JcLViewModel item = new JcLViewModel();
                var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                item.DefaultQty = i.Quantity;
                item.Description = i.Description;
                item.Price = i.Price;
                item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                items.Add(item);
            }
            jclob.JCLInfo = items;
            var invoicePaymenthistory = InvoicePaymentRepo.FindBy(i => i.InvoiceId == Id).ToList();
            List<InvoicePaymentList> invoicepaymentInfo = new List<InvoicePaymentList>();
            foreach (var payment in invoicePaymenthistory)
            {
                InvoicePaymentList paymentinfo = new InvoicePaymentList();
                paymentinfo.Id = payment.Id;
                paymentinfo.PaymentDate = payment.PaymentDate;
                paymentinfo.payment_Date = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                paymentinfo.PaymentAmount = payment.PaymentAmount;
                if (payment.PaymentMethod != null)
                {
                    Constant.PaymentMethod Durations = (Constant.PaymentMethod)payment.PaymentMethod;
                    var displaynamee = Durations.GetAttribute<DisplayAttribute>();
                    if (displaynamee != null)
                        paymentinfo.payment_Method = displaynamee.Name.ToString();
                }
                paymentinfo.Reference = payment.Reference;
                paymentinfo.PaymentNote = payment.PaymentNote;
                invoicepaymentInfo.Add(paymentinfo);

            }
            //subtotal

            decimal subtotal = Convert.ToDecimal(items.Sum(i => i.TotalPrice));
            invoiceViewModel.Price = Math.Round(subtotal, 2);
            invoiceViewModel.PriceWithGst = Math.Round(subtotal + (subtotal * 10) / 100, 2);
            invoiceViewModel.DepositRequired = ((invoiceViewModel.PriceWithGst * InvoiceData.AmountPaid) / 100);
            invoiceViewModel.DepositRequired = Math.Round(Convert.ToDecimal(invoiceViewModel.DepositRequired), 2);
            invoiceViewModel.GST = Math.Round((subtotal * 10) / 100, 2);

            if (invoiceViewModel.Paid != null)
            {
                if (invoiceViewModel.Paid > 0)
                {
                    invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst - invoiceViewModel.Paid), 2);

                }
                else
                {
                    invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst), 2);
                }
            }
            else
            {
                invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
            }
            var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
            {
                JclMappingViewModel = jclob,
                createInvoiceViewModel = invoiceViewModel,
                InvoicePaymentViewModel = invoicepaymentInfo
            };

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " export invoice");


            return View(custoemrInvoiceListViewmodel);
        }
        [HttpGet]
        public ActionResult QuoteExport(string id)
        {
            //Declaration 
            int CustomerType = 0;
            string CustomerName = "";
            List<JcLViewModel> items = new List<JcLViewModel>();
            List<InvoicePaymentList> QuotepaymentInfo = new List<InvoicePaymentList>();
            JCLItems jclob = new JCLItems();
            Guid? Id = Guid.Parse(id);
            FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == Id).FirstOrDefault();
            var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
            //mapping entity to viewmodel
            CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
            CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
            invoiceViewModel = mapper.Mapper(InvoiceData);
            var customer = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
            invoiceViewModel.TradeName = "";
            if (customer != null)
            {
                CustomerType = customer.CustomerType.HasValue ? Convert.ToInt32(customer.CustomerType) : 0;
            }

            var jobdetail = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).FirstOrDefault();
            var sitedetail = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
            if (jobdetail != null)
            {
                invoiceViewModel.WorkOrderNumber = !String.IsNullOrEmpty(jobdetail.WorkOrderNumber) ? jobdetail.WorkOrderNumber : "";
            }
            else
            {
                invoiceViewModel.WorkOrderNumber = "";
            }
            dynamic billingdetail = null;
            var BillingAddressId = InvoiceData.BillingAddressId;
            if (BillingAddressId != null)
            {
                if (BillingAddressId != Guid.Empty)
                {
                    billingdetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                }
                else
                {
                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                }
            }
            else
            {
                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
            }

            if (billingdetail == null)
            {

                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

            }
            if (customer != null)
            {
                var TradingName = customer.TradingName;
                if (!string.IsNullOrEmpty(TradingName))
                {
                    invoiceViewModel.TradeName = !String.IsNullOrEmpty(customer.TradingName) ? customer.TradingName : "";

                }
                CustomerName = !String.IsNullOrEmpty(customer.CustomerLastName) ? customer.CustomerLastName : "";
            }

            invoiceViewModel.DisplaysiteAddress =
                                               (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
            if (billingdetail == null)
            {

                invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else
            {

                invoiceViewModel.DisplayBillingAddress =
                                                         (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");

            }
            if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Domestic))
            {
                if (billingdetail == null)
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.RealState) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Strata) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Commercial))
            {
                invoiceViewModel.DisplaysiteAddress =
                                             (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                if (billingdetail == null)
                {
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else
                {
                    invoiceViewModel.DisplayBillingAddress = (!string.IsNullOrEmpty(invoiceViewModel.TradeName) ? invoiceViewModel.TradeName + "\n" : "") +
                                                        (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                }
                invoiceViewModel.DisplaysiteAddress = (!string.IsNullOrEmpty(sitedetail.StrataPlan) ? "SP:" + sitedetail.StrataPlan + "\n" : "") +
                                                            invoiceViewModel.DisplaysiteAddress;
            }



            if (InvoiceData.CreatedBy != null)
            {
                invoiceViewModel.PreparedBy = Employee.FindBy(i => i.EmployeeId == InvoiceData.CreatedBy).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault();
            }
            invoiceViewModel.InvcDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : string.Empty;

            if (InvoiceData.CreatedBy != null)
            {
                invoiceViewModel.PreparedBy = Employee.FindBy(i => i.EmployeeId == InvoiceData.CreatedBy).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault();
            }
            //Get Jcl Item Total Price
            var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoiceData.Id).ToList();
            var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId && i.InvoiceId == InvoiceData.Id).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);
            foreach (var i in InvoiceJCLItemlist)
            {
                JcLViewModel item = new JcLViewModel();
                var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                item.DefaultQty = i.Quantity;
                item.Description = i.Description;
                item.Price = i.Price;
                item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                items.Add(item);
            }
            jclob.JCLInfo = items;


            //subtotal
            decimal subtotal = Convert.ToDecimal(items.Sum(i => i.TotalPrice));
            invoiceViewModel.Price = Math.Round(subtotal, 2);
            invoiceViewModel.PriceWithGst = Math.Round((subtotal + (subtotal * 10) / 100), 2);
            invoiceViewModel.BalanceDue = (invoiceViewModel.PriceWithGst) - (invoiceViewModel.PriceWithGst * (InvoiceData.AmountPaid)) / 100;
            invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.BalanceDue), 2);
            invoiceViewModel.AmountPay = InvoiceData.AmountPaid;
            invoiceViewModel.DepositRequired = Math.Round(Convert.ToDecimal((invoiceViewModel.PriceWithGst * InvoiceData.AmountPaid) / 100), 2);
            invoiceViewModel.GST = Math.Round(Convert.ToDecimal((subtotal * 10) / 100), 2);
            if (invoiceViewModel.Paid != null)
            {
                if (invoiceViewModel.Paid > 0)
                {
                    invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst - invoiceViewModel.Paid), 2);
                }
                else
                {
                    invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst), 2);
                }
            }
            else
            {
                invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst), 2);
            }

            var QuotePaymenthistory = InvoicePaymentRepo.FindBy(i => i.InvoiceId == Id).ToList();
            foreach (var payment in QuotePaymenthistory)
            {
                InvoicePaymentList paymentinfo = new InvoicePaymentList();
                paymentinfo.Id = payment.Id;
                paymentinfo.PaymentDate = payment.PaymentDate;
                paymentinfo.payment_Date = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                paymentinfo.PaymentAmount = payment.PaymentAmount;
                if (payment.PaymentMethod != null)
                {
                    Constant.PaymentMethod Durations = (Constant.PaymentMethod)payment.PaymentMethod;
                    var displaynamee = Durations.GetAttribute<DisplayAttribute>();
                    if (displaynamee != null)
                        paymentinfo.payment_Method = displaynamee.Name.ToString();
                }
                paymentinfo.Reference = payment.Reference;
                paymentinfo.PaymentNote = payment.PaymentNote;
                QuotepaymentInfo.Add(paymentinfo);

            }
            var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
            {
                JclMappingViewModel = jclob,
                createInvoiceViewModel = invoiceViewModel,
                InvoicePaymentViewModel = QuotepaymentInfo
            };
            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " export quote");

            return View(custoemrInvoiceListViewmodel);
        }
        [HttpGet]
        public ActionResult JobInvoiceExport(string Jobid, string InvoiceId)
        {
            try
            {
                Guid Id = !string.IsNullOrEmpty(Jobid) ? Guid.Parse(Jobid) : Guid.Empty;
                var job = Employeejob.FindBy(m => m.Id == Id).FirstOrDefault();
                CommonMapper<Jobs, JobsViewModel> mapper = new CommonMapper<Jobs, JobsViewModel>();
                var jobsViewModel = mapper.Mapper(job);

                var customerList = CustomerGeneralInfo.GetAll().Select(m => new SelectListItem()
                {
                    Text = m.CustomerLastName,
                    Value = m.CustomerGeneralInfoId.ToString()
                }).ToList();

                var OTRWList = Employeejob.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();

                var linkJobList = Employeejob.FindBy(m => m.JobType != 3).Select(m => new SelectListItem()
                {
                    Text = "Job_" + m.JobId.ToString(),
                    Value = m.Id.ToString()
                }).ToList();

                var SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == jobsViewModel.CustomerGeneralInfoId).
                               Select(m => new SelectListItem()
                               {
                                   Text = m.StreetName,
                                   Value = m.SiteDetailId.ToString()
                               }).ToList();

                jobsViewModel.CustomerList = customerList;
                jobsViewModel.SiteList = SiteList;
                jobsViewModel.OTRWList = OTRWList;
                jobsViewModel.LinkJobList = linkJobList;
                jobsViewModel.CustomerInfoId = jobsViewModel.CustomerGeneralInfoId.ToString();
                jobsViewModel.tempAssignTo = jobsViewModel.AssignTo.ToString();
                jobsViewModel.tempSiteId = jobsViewModel.SiteId.ToString();

                var linkJob = SupportjobMapping.FindBy(m => m.SupportJobId == Id).FirstOrDefault();
                jobsViewModel.LinkJobId = linkJob != null ? linkJob.LinkedJobId.ToString() : string.Empty;
                ViewBag.ShowMsg = TempData["ShowMsg"] != null ? TempData["ShowMsg"].ToString() : string.Empty;
                ViewBag.InvoiceId = InvoiceId;
                return View(jobsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerGeneralInfo.Dispose();
                Employeejob.Dispose();
                CustomerSiteDetailRepo.Dispose();
                SupportjobMapping.Dispose();
            }
        }


        public ActionResult Export()
        {
            try
            {
                int CustomerType = 0;
                string CustomerName = "";
                Guid? Id = Guid.Parse(Request.RequestContext.RouteData.Values["Id"].ToString());
                FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == Id).FirstOrDefault();
                List<InvoicePaymentList> inovicepaymentInfo = new List<InvoicePaymentList>();
                // mapping entity to viewmodel
                CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
                CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
                invoiceViewModel = mapper.Mapper(InvoiceData);

                invoiceViewModel.InvcDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : string.Empty;
                var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();

                var jobdetail = JobRepository.FindBy(m => m.JobNo == InvoiceData.JobId && m.IsDelete == false).OrderByDescending(m => m.CreatedDate).FirstOrDefault();
                var sitedetail = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
                if (jobdetail != null)
                {
                    invoiceViewModel.WorkOrderNumber = !String.IsNullOrEmpty(jobdetail.WorkOrderNumber) ? jobdetail.WorkOrderNumber : "";
                }
                else
                {
                    invoiceViewModel.WorkOrderNumber = "";
                }
                var customer = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();   //get customer detail

                invoiceViewModel.TradeName = "";
                if (customer != null)
                {
                    CustomerType = customer.CustomerType.HasValue ? Convert.ToInt32(customer.CustomerType) : 0;
                }
                dynamic billingdetail = null;
                var BillingAddressId = InvoiceData.BillingAddressId;
                if (BillingAddressId != null)
                {
                    if (BillingAddressId != Guid.Empty)
                    {
                        billingdetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                    }
                    else
                    {
                        billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                    }
                }
                else
                {
                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                }

                if (billingdetail == null)
                {

                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

                }
                if (customer != null)
                {
                    var TradingName = customer.TradingName;
                    if (!string.IsNullOrEmpty(TradingName))
                    {
                        invoiceViewModel.TradeName = !String.IsNullOrEmpty(customer.TradingName) ? customer.TradingName : "";

                    }
                    CustomerName = !String.IsNullOrEmpty(customer.CustomerLastName) ? customer.CustomerLastName : "";
                }

                invoiceViewModel.DisplaysiteAddress =
                                                 (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                                (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                                (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                                (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                                (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                if (billingdetail == null)
                {

                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else
                {

                    invoiceViewModel.DisplayBillingAddress =

                                                         (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +

                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");

                }
                if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Domestic))
                {
                    if (billingdetail == null)
                        invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.RealState) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Strata) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Commercial))
                {
                    invoiceViewModel.DisplaysiteAddress =
                                             (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                            (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                            (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                    if (billingdetail == null)
                    {
                        invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                    }
                    else
                    {
                        invoiceViewModel.DisplayBillingAddress = (!string.IsNullOrEmpty(invoiceViewModel.TradeName) ? invoiceViewModel.TradeName + "\n" : "") +
                                                               (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                             (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                    }
                    invoiceViewModel.DisplaysiteAddress = (!string.IsNullOrEmpty(sitedetail.StrataPlan) ? "SP:" + sitedetail.StrataPlan + "\n" : "") +
                                                                invoiceViewModel.DisplaysiteAddress;
                }

                if (InvoiceData.CreatedBy != null)
                {
                    invoiceViewModel.PreparedBy = Employee.FindBy(i => i.EmployeeId == InvoiceData.CreatedBy).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault();
                }
                JCLItems jclob = new JCLItems();
                var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoiceData.Id).ToList();

                var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId && i.InvoiceId == InvoiceData.Id).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);
                // var InvoiceJCLItemlist = invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId && i.InvoiceId == InvoiceData.Id).ToList().OrderBy(i => i.OrderNo);
                List<JcLViewModel> items = new List<JcLViewModel>();
                foreach (var i in InvoiceJCLItemlist)
                {
                    JcLViewModel item = new JcLViewModel();
                    var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                    item.DefaultQty = i.Quantity;
                    item.Description = i.Description;
                    item.Price = i.Price;
                    item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                    items.Add(item);
                }
                jclob.JCLInfo = items;
                //subtotal
                decimal subtotal = Math.Round(Convert.ToDecimal(items.Sum(i => i.TotalPrice)), 2);
                invoiceViewModel.Price = Math.Round(subtotal, 2);
                invoiceViewModel.GST = Math.Round(Convert.ToDecimal(subtotal * 10), 2);
                invoiceViewModel.PriceWithGst = Math.Round((subtotal + (subtotal * 10) / 100), 2);
                invoiceViewModel.BalanceDue = (invoiceViewModel.PriceWithGst) - (invoiceViewModel.PriceWithGst * (InvoiceData.AmountPaid)) / 100;
                invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.BalanceDue), 2);
                invoiceViewModel.AmountPay = InvoiceData.AmountPaid;
                invoiceViewModel.DepositRequired = ((invoiceViewModel.PriceWithGst * InvoiceData.AmountPaid) / 100);
                invoiceViewModel.GST = Math.Round(((subtotal * 10) / 100), 2);
                if (invoiceViewModel.Paid != null)
                {
                    if (invoiceViewModel.Paid > 0)
                    {
                        invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst - invoiceViewModel.Paid), 2);
                    }
                    else
                    {
                        invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst), 2);
                    }
                }
                else
                {
                    invoiceViewModel.BalanceDue = Math.Round(Convert.ToDecimal(invoiceViewModel.PriceWithGst), 2);
                }
                var QuotePaymenthistory = InvoicePaymentRepo.FindBy(i => i.InvoiceId == Id).ToList();
                foreach (var payment in QuotePaymenthistory)
                {
                    InvoicePaymentList paymentinfo = new InvoicePaymentList();
                    paymentinfo.Id = payment.Id;
                    paymentinfo.PaymentDate = payment.PaymentDate;
                    paymentinfo.payment_Date = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    paymentinfo.PaymentAmount = payment.PaymentAmount;
                    if (payment.PaymentMethod != null)
                    {
                        Constant.PaymentMethod Durations = (Constant.PaymentMethod)payment.PaymentMethod;
                        var displaynamee = Durations.GetAttribute<DisplayAttribute>();
                        if (displaynamee != null)
                            paymentinfo.payment_Method = displaynamee.Name.ToString();
                    }
                    paymentinfo.Reference = payment.Reference;
                    paymentinfo.PaymentNote = payment.PaymentNote;
                    inovicepaymentInfo.Add(paymentinfo);

                }
                var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
                {
                    JclMappingViewModel = jclob,
                    createInvoiceViewModel = invoiceViewModel,
                    InvoicePaymentViewModel = inovicepaymentInfo
                };

                if (InvoiceData.InvoiceType.ToLower() == "quote")
                {
                    return new ViewAsPdf("QuoteExportPreview", custoemrInvoiceListViewmodel)
                    {
                        FileName = "JobQuote.pdf"
                    };
                }
                else
                {
                    return new ViewAsPdf("InvoiceExportPreview", custoemrInvoiceListViewmodel)
                    {
                        FileName = "JobInvoice.pdf"
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult InvoiceExportPreview(string id)
        {
            Guid? Id = Guid.Parse(id);
            FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == Id).FirstOrDefault();
            //mapping entity to viewmodel
            CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
            CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
            invoiceViewModel = mapper.Mapper(InvoiceData);
            invoiceViewModel.InvcDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : string.Empty;
            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " invoice export preview");

            return View(invoiceViewModel);
        }
        //GET: Employee/Invoice/ManageStockinfo
        /// <summary>
        /// StockInfo by jobid
        /// </summary>
        /// <param name="Jobid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageStockInfo(string Jobid, string invoiceid)
        {
            try
            {
                using (JobStock)
                {
                    Guid jid = Guid.Parse(Jobid);
                    var StockList = Stock.GetAll();
                    //StockList = StockList.Where(i => i.Available > 0);
                    var JobStockList = JobStock.GetJobStockList().Where(i => i.Jobid == jid).ToList();
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 5 :
                                   Convert.ToInt32(Request.QueryString["page_size"]);
                    DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                    if (!string.IsNullOrEmpty(invoiceid))
                    {
                        displayJobStocksViewModel.InvoiceId = invoiceid;
                    }
                    else
                    {
                        displayJobStocksViewModel.InvoiceId = "";
                    }
                    displayJobStocksViewModel.JobId = jid;
                    List<StockDetail> li = new List<StockDetail>();
                    foreach (var i in StockList)
                    {
                        StockDetail obj = new StockDetail();
                        obj.StockID = i.ID;
                        obj.Label = i.Label;
                        li.Add(obj);
                    }
                    displayJobStocksViewModel.stockDetail = li;
                    displayJobStocksViewModel.PageSize = PageSize;

                    var displayJobStocksListViewModel = new DisplayJobStocksListViewModel
                    {
                        DisplayJobStocksViewModel = displayJobStocksViewModel,
                        DisplayJobStocksList = JobStockList,
                        JobStock = new JobStock()
                    };



                    return View(displayJobStocksListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Invoice/ManageStockInfo
        /// <summary>
        ///Save stockinfo 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirect ManageStockInfo</returns>
        [HttpPost]
        public ActionResult ManageStockInfo(DisplayJobStocksListViewModel model)
        {
            if (model != null)
            {
                try
                {
                    DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                    if (ModelState.IsValid)
                    {
                        displayJobStocksViewModel.JobId = model.DisplayJobStocksViewModel.JobId;
                        displayJobStocksViewModel.StockID = model.DisplayJobStocksViewModel.StockID;
                        displayJobStocksViewModel.UnitMeasure = model.DisplayJobStocksViewModel.UnitMeasure;
                        displayJobStocksViewModel.Price = model.DisplayJobStocksViewModel.Price;
                        displayJobStocksViewModel.Quantity = model.DisplayJobStocksViewModel.Quantity;

                        if (!string.IsNullOrEmpty(model.DisplayJobStocksViewModel.InvoiceId))
                        {
                            displayJobStocksViewModel.InvoiceId = model.DisplayJobStocksViewModel.InvoiceId;
                        }
                        else
                        {
                            displayJobStocksViewModel.InvoiceId = "";
                        }
                        int available = (Convert.ToInt32(model.DisplayJobStocksViewModel.AvailableQuantity) - Convert.ToInt32(displayJobStocksViewModel.Quantity));
                        using (Stock)
                        {
                            var stock = Stock.FindBy(i => i.ID == displayJobStocksViewModel.StockID).FirstOrDefault();
                            stock.Available = available;
                            stock.ModifiedBy = Guid.Parse(base.GetUserId);
                            stock.ModifiedDate = DateTime.Now;
                            Stock.Edit(stock);
                            Stock.Save();
                        }
                        if (model.DisplayJobStocksViewModel.ID == Guid.Empty)
                        {
                            displayJobStocksViewModel.CreatedDate = DateTime.Now;
                            displayJobStocksViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            displayJobStocksViewModel.ID = Guid.NewGuid();
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            JobStock.Add(jobStock);
                            JobStock.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added manage stock info");

                        }
                        else
                        {
                            displayJobStocksViewModel.ID = model.DisplayJobStocksViewModel.ID;
                            displayJobStocksViewModel.ModifiedDate = DateTime.Now;

                            displayJobStocksViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            JobStock.Edit(jobStock);
                            JobStock.Save();


                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " updated manage stock info");
                        }

                        return RedirectToAction("ManageStockInfo", new { Jobid = displayJobStocksViewModel.JobId, invoiceid = displayJobStocksViewModel.InvoiceId });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("ManageStockInfo", new { Jobid = model.DisplayJobStocksViewModel.JobId, invoiceid = "" });
        }

        //POST: Employee/Invoice/EditManageStockInfo
        /// <summary>
        /// EditManageStockInfo by stockid 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditManageStockInfo(string Id, string stockID, string invoiceid = "")
        {
            try
            {
                using (JobStock)
                {
                    Guid JobStockID = Guid.Parse(Id);
                    Guid StockID = Guid.Parse(stockID);

                    var stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    int available = Convert.ToInt32(stock.Available);

                    JobStock jobStock = JobStock.FindBy(m => m.ID == JobStockID).FirstOrDefault();
                    int quantityauail = Convert.ToInt32(jobStock.Quantity);

                    // mapping entity to viewmodel
                    CommonMapper<JobStock, DisplayJobStocksViewModel> mapper = new CommonMapper<JobStock, DisplayJobStocksViewModel>();
                    DisplayJobStocksViewModel displayJobStocksViewModel = mapper.Mapper(jobStock);
                    if (!String.IsNullOrEmpty(invoiceid))
                    {
                        displayJobStocksViewModel.InvoiceId = invoiceid;
                    }
                    displayJobStocksViewModel.AvailableQuantity = available + quantityauail;

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(displayJobStocksViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated manage stock info");

                    return Json(new { json, JsonRequestBehavior.AllowGet });

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/Job/DeleteStockInfo
        /// <summary>
        /// DeleteStockInfo By stockid
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns></returns>
        public ActionResult DeleteManageStockInfo(string Id, string stockID, string invoiceid = "")
        {
            try
            {
                int newavailablestock;
                Guid Jobid;
                Guid jobStockID = Guid.Parse(Id);
                Guid StockID = Guid.Parse(stockID);

                using (JobStock)
                {
                    JobStock jobStock = JobStock.FindBy(m => m.ID == jobStockID).FirstOrDefault();
                    int quantityauail = Convert.ToInt32(jobStock.Quantity);
                    newavailablestock = quantityauail;
                    Jobid = jobStock.JobId;

                    JobStock stockDelete = JobStock.FindBy(i => i.ID == jobStockID).FirstOrDefault();
                    stockDelete.IsDelete = true;
                    JobStock.Edit(stockDelete);
                    JobStock.Save();


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated job stock info");
                }
                using (Stock)
                {
                    var stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    int available = Convert.ToInt32(stock.Available);

                    stock.Available = available + newavailablestock;
                    Stock.Edit(stock);
                    Stock.Save();


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated stock info");

                }

                return RedirectToAction("ManageStockInfo", new { Jobid = Jobid, invoiceid = invoiceid });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// _Invoice JobPurchase Order
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _InvoiceJobPurchaseOrder()
        {
            try
            {
                return PartialView("_InvoiceJobPurchaseOrder");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult DeleteJobPurchaseitem(string Jobid, string stockid)
        {
            try
            {
                Guid JobId = Guid.Parse(Jobid);
                Guid StockId = Guid.Parse(stockid);
                var PurchaseOrderId = JobPurchaseOrder.FindBy(i => i.JobID == JobId).FirstOrDefault();
                if (PurchaseOrderId != null)
                {
                    var purchaseitem = JobPurchaseOrderitem.FindBy(i => i.StockID == StockId && i.PurchaseOrderID == PurchaseOrderId.ID).FirstOrDefault();
                    if (purchaseitem != null)
                    {
                        JobPurchaseOrderitem.Delete(purchaseitem);
                        JobPurchaseOrderitem.Save();
                    }
                }


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted job purchage item");

                return Json(true);

            }
            catch (Exception)
            {
                return Json(true);
                throw;
            }
        }
        [HttpGet]
        public ActionResult ViewJobsInvoicePurchaseOrder(string Jobid, string InvoiceId)
        {
            try
            {
                using (JobPurchaseOrder)
                {
                    var purchaseorders = JobPurchaseOrder.GetjobPurchaseOrdersByJobId(Jobid);
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = "" }
                    };
                    ViewBag.JobId = Jobid;
                    ViewBag.InvoiceId = InvoiceId;



                    return View(model);
                }
            }

            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult ViewJobsInvoicePurchaseOrder(PurchaseOrderjobsearchviewModel purchaseOrderjobsearchviewModel)
        {
            try
            {
                using (JobPurchaseOrder)
                {
                    string jobId = (purchaseOrderjobsearchviewModel.JobId).ToString();
                    string Searchstring = purchaseOrderjobsearchviewModel.SearchKeyword;
                    //var stocks =Stock.GetAll();
                    var purchaseorders = JobPurchaseOrder.GetjobPurchaseOrders(Searchstring);
                    purchaseorders = string.IsNullOrEmpty(jobId) ? purchaseorders : purchaseorders.Where(i => (i.JobID).ToString().Contains(jobId));
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };
                    ViewBag.JobId = jobId;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job invoice purchase order.");

                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Invoice/DeleteJobPurchaseorder
        /// <summary>
        /// Delete JobPurchaseOrder Record
        /// </summary>
        /// <param name="jobid"></param>
        /// <returns>Redirect ViewJobsInvoicePurchaseOrder</returns>
        public ActionResult DeleteJobPurchaseorder(string jobid)
        {
            try
            {
                Guid Jobid = Guid.Parse(jobid);
                string PurchaseorderId = "";

                var purchaseorder = JobPurchaseOrder.FindBy(i => i.JobID == Jobid).FirstOrDefault();
                if (purchaseorder != null)
                {
                    PurchaseorderId = purchaseorder.ID.ToString();
                }

                if (!string.IsNullOrEmpty(PurchaseorderId))
                {
                    Guid purchaseId = Guid.Parse(PurchaseorderId);
                    var purchaseorderitems = JobPurchaseOrderitem.FindBy(i => i.PurchaseOrderID == purchaseId).ToList();
                    if (purchaseorderitems != null && purchaseorderitems.Count > 0)
                    {
                        foreach (var pitem in purchaseorderitems)
                        {
                            JobPurchaseOrderitem.Delete(pitem);
                            JobPurchaseOrderitem.Save();
                        }
                    }
                    var porder = JobPurchaseOrder.FindBy(i => i.ID == purchaseId).FirstOrDefault();
                    JobPurchaseOrder.Delete(porder);
                    JobPurchaseOrder.Save();
                    TempData["Message"] = "3";
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted job purchase order.");

                return RedirectToAction("ViewJobspurchaseOrder");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                JobPurchaseOrder.Dispose();
            }
        }
        [HttpGet]
        public ActionResult EditJobInvoicePurchaseOrder(string Purchaseorderid, string JobId, string InvoiceId)
        {
            List<SupplierJobItem> SupplierJobList = new List<SupplierJobItem>();
            using (Supplier)
            {
                var suppliers = Supplier.GetAll();
                foreach (var supplier in suppliers)
                {
                    SupplierJobItem obj = new SupplierJobItem();
                    obj.ID = supplier.ID;
                    obj.Name = supplier.Name;
                    SupplierJobList.Add(obj);
                }
            }

            List<EmployeeJobDetail> empjobdetail = new List<EmployeeJobDetail>();
            using (Employeejob)
            {
                Guid id = Guid.Parse(JobId);
                var employeerJobList = Employeejob.FindBy(i => i.Id == id).AsEnumerable();

                GetJobViewModel getJobViewModel = new GetJobViewModel();
                foreach (var i in employeerJobList)
                {
                    EmployeeJobDetail obj = new EmployeeJobDetail();
                    obj.EmployeeJobId = i.Id;
                    obj.JobNo = i.JobNo;
                    obj.Description = "JobNo_" + obj.JobNo;
                    empjobdetail.Add(obj);
                }
            }

            using (Stock)
            {
                var stocks = Stock.GetAll();
                List<StockJobList> StockJobList = new List<StockJobList>();
                foreach (var stock in stocks)
                {
                    StockJobList sitem = new StockJobList();
                    sitem.StockId = stock.ID;
                    sitem.StockName = stock.Label;
                    StockJobList.Add(sitem);
                }

                JobPurChaseViewModel model = new JobPurChaseViewModel
                {
                    PurchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel(),
                    PurchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel(),
                    getjobviewmodel = new GetJobViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    Guid id = Guid.Parse(JobId);
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                    model.PurchaseOrderByJobViewModel.PurchaseOrderNo = JobPurchaseOrder.FindBy(i => i.JobID == id && i.ID == model.PurchaseOrderByJobViewModel.ID).FirstOrDefault().PurchaseOrderNo;
                }

                model.PurchaseOrderITemByJobViewModel.StockJoblist = StockJobList.OrderBy(i => i.StockName).ToList();
                model.PurchaseOrderByJobViewModel.SupplierJobList = SupplierJobList.OrderBy(i => i.Name).ToList();
                model.getjobviewmodel.employeeJobDetail = empjobdetail.OrderBy(i => i.EmployeeJobId).ToList();
                ViewBag.JobId = JobId;
                ViewBag.InvoiceId = InvoiceId;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " updated job invoice purchase order.");

                return View(model);
            }

        }
        [HttpPost]
        public ActionResult SaveJobPurchaseData(List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values)
        {
            try
            {
                Dictionary<Guid, int> dictionaryjob = new Dictionary<Guid, int>();
                Dictionary<Guid, string> dictionaryItemstoJobpurchase = new Dictionary<Guid, string>();

                string Purchaseid = values[0].purchaseId;
                PurchaseOrderByJobviewmodel purchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel();
                #region Add-Edit Purchaseorderstock
                using (JobPurchaseOrder)
                {
                    if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                    {
                        Guid id = Guid.Parse(Purchaseid);
                        var purchasestock = JobPurchaseOrder.FindBy(i => i.ID == id).FirstOrDefault();
                        purchasestock.SupplierID = Guid.Parse(values[0].Supplierid);
                        if (values[0].JobId != null)
                        {
                            purchasestock.JobID = Guid.Parse(values[0].JobId);
                        }
                        purchasestock.Description = values[0].Description;
                        purchasestock.Cost = Convert.ToDecimal(values[0].Cost);
                        purchasestock.ModifiedBy = Guid.Parse(base.GetUserId);
                        purchasestock.ModifiedDate = DateTime.Now;
                        JobPurchaseOrder.Edit(purchasestock);
                        JobPurchaseOrder.Save();
                        TempData["Message"] = 2;
                    }
                    else
                    {
                        purchaseOrderByJobViewModel.ID = Guid.NewGuid();
                        purchaseOrderByJobViewModel.SupplierID = Guid.Parse(values[0].Supplierid);
                        purchaseOrderByJobViewModel.Description = values[0].Description;
                        if (values[0].JobId != null)
                        {
                            purchaseOrderByJobViewModel.JobID = Guid.Parse(values[0].JobId);
                        }
                        purchaseOrderByJobViewModel.Cost = Convert.ToDecimal(values[0].Cost);
                        purchaseOrderByJobViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        purchaseOrderByJobViewModel.CreatedDate = DateTime.Now;
                        CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob> mapper = new CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob>();
                        PurchaseOrderByJob purchasestockinfo = mapper.Mapper(purchaseOrderByJobViewModel);
                        JobPurchaseOrder.Add(purchasestockinfo);
                        JobPurchaseOrder.Save();
                        TempData["Message"] = 1;
                    }
                }
                #endregion

                #region Add-Edit Purchaseitemstock
                #region Delete item on update 
                Guid itemId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;

                var items = JobPurchaseOrderitem.FindBy(i => i.PurchaseOrderID == itemId).ToList();

                foreach (var item in items)
                {
                    dictionaryItemstoJobpurchase.Add((item.ID), item.Quantity.ToString());
                    string iteminfoid = Convert.ToString(item.ID.ToString());
                    //int pos = Array.IndexOf(values.ToArray(), itemid);
                    bool pos = Array.Exists(values.ToArray(), element => element.ItemId == Guid.Parse(iteminfoid));
                    if (!pos)
                    {
                        var itemdetailid = Convert.ToString(item.ID.ToString());
                        if (!String.IsNullOrEmpty(itemdetailid))
                        {
                            var item_id = Guid.Parse(itemdetailid);
                            dictionaryjob.Add(item_id, Convert.ToInt32(item.Quantity));
                            var itemtodelete = JobPurchaseOrderitem.FindBy(i => i.ID == item_id).FirstOrDefault();
                            JobPurchaseOrderitem.Delete(itemtodelete);
                            JobPurchaseOrderitem.Save();
                        }
                    }
                }
                #endregion
                using (JobPurchaseOrderitem)
                {
                    foreach (var stockitem in values)
                    {
                        if (stockitem.StockId == null)
                        {
                            stockitem.StockId = Guid.Empty.ToString();
                        }
                        Guid? itemorderId = stockitem.ItemId;
                        Guid itempurchasdId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                        var checkExist = JobPurchaseOrderitem.FindBy(i => i.ID == itemorderId && i.PurchaseOrderID == itempurchasdId).FirstOrDefault();
                        if (checkExist != null)
                        {
                            var itemtoupdate = checkExist;
                            itemtoupdate.PurchaseItem = Convert.ToString(stockitem.PurchaseItem);
                            itemtoupdate.UnitOfMeasure = Convert.ToString(stockitem.UnitMeasure);
                            itemtoupdate.Price = Convert.ToDecimal(stockitem.Price);
                            itemtoupdate.Quantity = Convert.ToInt32(stockitem.Quantity);
                            itemtoupdate.ModifiedBy = Guid.Parse(base.GetUserId);
                            itemtoupdate.ModifiedDate = DateTime.Now;
                            JobPurchaseOrderitem.Edit(itemtoupdate);
                            JobPurchaseOrderitem.Save();
                        }
                        else
                        {
                            PurchaseorderItemJobViewModel purchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel();
                            purchaseOrderITemByJobViewModel.ID = Guid.NewGuid();
                            if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                            {
                                purchaseOrderITemByJobViewModel.PurchaseOrderID = Guid.Parse(Purchaseid);
                            }
                            else { purchaseOrderITemByJobViewModel.PurchaseOrderID = purchaseOrderByJobViewModel.ID; }
                            purchaseOrderITemByJobViewModel.StockID = Guid.Parse(stockitem.StockId);
                            purchaseOrderITemByJobViewModel.PurchaseItem = stockitem.PurchaseItem;
                            purchaseOrderITemByJobViewModel.UnitOfMeasure = stockitem.UnitMeasure;
                            purchaseOrderITemByJobViewModel.Quantity = Convert.ToInt32(stockitem.Quantity);
                            purchaseOrderITemByJobViewModel.Price = Convert.ToDecimal(stockitem.Price);
                            purchaseOrderITemByJobViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            purchaseOrderITemByJobViewModel.CreatedDate = DateTime.Now;
                            CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob> mapper = new CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob>();
                            PurchaseorderItemJob purchasestockiteminfo = mapper.Mapper(purchaseOrderITemByJobViewModel);
                            JobPurchaseOrderitem.Add(purchasestockiteminfo);
                            JobPurchaseOrderitem.Save();
                        }
                    }

                }

                #endregion

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved job purchase data.");

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                TempData["Message"] = 0;
                throw;
            }
        }

        public ActionResult DeleteinvoiceItem(string ItemId)
        {
            try
            {
                Guid Id = Guid.Parse(ItemId);
                var InvoiceItems = Invoiceitem.FindBy(i => i.Id == Id).FirstOrDefault();

                if (InvoiceItems != null)
                {
                    Invoiceitem.Delete(InvoiceItems);
                    Invoiceitem.Save();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted invoice item.");

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region tabs
        public ActionResult SaveInvoiceInfo(string id, string activetab, string success, string pagenum)
        {
            try
            {
                InvoiceTabPanelViewModel objInvoiceTabPanelViewModel = new InvoiceTabPanelViewModel();

                if (!string.IsNullOrEmpty(id))
                {
                    FSM.Core.Entities.Invoice Invoice = InvoiceRep.FindBy(m => m.Id == new Guid(id)).FirstOrDefault();
                    objInvoiceTabPanelViewModel.InvoiceId = id;
                    objInvoiceTabPanelViewModel.ActiveTab = activetab;
                    objInvoiceTabPanelViewModel.Success = success;
                    objInvoiceTabPanelViewModel.PageNum = pagenum;
                    Jobs Jobs = Employeejob.FindBy(m => m.Id == Invoice.EmployeeJobId).OrderByDescending(m => m.CreatedDate).FirstOrDefault();
                    objInvoiceTabPanelViewModel.JobId = Jobs.Id;
                    objInvoiceTabPanelViewModel.JobNo = Jobs.JobNo;
                    CustomerSiteDetail Site = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == Jobs.SiteId).FirstOrDefault();
                    objInvoiceTabPanelViewModel.SiteDocumentsCount = CustomerSitesDocumentsRepo.FindBy(m => m.SiteId == Jobs.SiteId).Count().ToString();
                    var billingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == Site.CustomerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                    CustomerBillingAddress BillingAddress;
                    if (billingaddress != null)
                        BillingAddress = billingaddress;
                    else
                    {
                        BillingAddress = CustomerBilling.GetAll().Where(i => i.CustomerGeneralInfoId == Site.CustomerGeneralInfoId).OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                    }
                    if (Site != null)
                    {
                        objInvoiceTabPanelViewModel.SiteContactId = Site.ContactId.ToString();
                        objInvoiceTabPanelViewModel.CustomerGeneralInfoId = Site.CustomerGeneralInfoId.ToString();
                        objInvoiceTabPanelViewModel.CustomerSiteDetailId = Site.SiteDetailId.ToString();
                    }

                    if (BillingAddress != null)
                    {
                        objInvoiceTabPanelViewModel.BillingAddressId = BillingAddress.BillingAddressId.ToString();
                    }
                    if (Invoice != null)
                    {
                        objInvoiceTabPanelViewModel.InvoiceNo = Invoice.InvoiceNo;
                        objInvoiceTabPanelViewModel.JobType = Invoice.JobType;
                        objInvoiceTabPanelViewModel.InvoiceType = Invoice.InvoiceType;
                    }
                    if (Jobs != null)
                    {
                        objInvoiceTabPanelViewModel.JobType = Jobs.JobType;
                    }
                }

                TempData["ShowMsg"] = !string.IsNullOrEmpty(Request.QueryString["showmsg"]) ? "Yes" : string.Empty;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved invoice info.");

                return View(objInvoiceTabPanelViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Employeejob.Dispose();
            }
        }

        #endregion
        [HttpGet]
        public ActionResult EditJob(string id, string InvoiceId)
        {
            try
            {
                Guid Id = !string.IsNullOrEmpty(id) ? Guid.Parse(id) : Guid.Empty;
                var job = Employeejob.FindBy(m => m.Id == Id).FirstOrDefault();
                CommonMapper<Jobs, JobsViewModel> mapper = new CommonMapper<Jobs, JobsViewModel>();
                var jobsViewModel = mapper.Mapper(job);
                var userName = "";
                if (job.ModifiedBy == null)
                {
                    userName = Employee.FindBy(m => m.EmployeeId == job.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                }
                else
                {
                    userName = Employee.FindBy(m => m.EmployeeId == job.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                }
                if (job.ModifiedDate == null)
                {
                    job.CreatedDate = job.CreatedDate;
                }
                else
                {
                    job.ModifiedDate = job.ModifiedDate;
                }

                var customerList = CustomerGeneralInfo.GetAll().Where(m => m.IsDelete == false).Select(m => new SelectListItem()
                {
                    Text = m.CustomerLastName,
                    Value = m.CustomerGeneralInfoId.ToString()
                }).ToList();

                var OTRWList = Employeejob.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();

                var JoBAssign2 = JobAssignMapping.FindBy(m => m.JobId == Id).Select(m => m.AssignTo).ToList();

                var linkJobList = Employeejob.FindBy(m => m.JobType != 3).Select(m => new SelectListItem()
                {
                    Text = "Job_" + m.JobId.ToString(),
                    Value = m.Id.ToString()
                }).ToList();

                var SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == jobsViewModel.CustomerGeneralInfoId).
                               Select(m => new SelectListItem()
                               {
                                   Text = m.StreetName,
                                   Value = m.SiteDetailId.ToString()
                               }).ToList();

                jobsViewModel.CustomerList = customerList;
                jobsViewModel.SiteList = SiteList;
                jobsViewModel.OTRWList = OTRWList;
                jobsViewModel.UserName = userName;
                jobsViewModel.tempAssignTo2 = JoBAssign2;
                jobsViewModel.LinkJobList = linkJobList;
                jobsViewModel.CustomerInfoId = jobsViewModel.CustomerGeneralInfoId.ToString();
                jobsViewModel.tempAssignTo = jobsViewModel.AssignTo.ToString();
                jobsViewModel.tempSiteId = jobsViewModel.SiteId.ToString();

                var linkJob = SupportjobMapping.FindBy(m => m.SupportJobId == Id).FirstOrDefault();
                jobsViewModel.LinkJobId = linkJob != null ? linkJob.LinkedJobId.ToString() : string.Empty;
                ViewBag.ShowMsg = TempData["ShowMsg"] != null ? TempData["ShowMsg"].ToString() : string.Empty;
                ViewBag.InvoiceId = InvoiceId;
                return View(jobsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerGeneralInfo.Dispose();
                Employeejob.Dispose();
                CustomerSiteDetailRepo.Dispose();
                SupportjobMapping.Dispose();
            }
        }

        [HttpPost]
        public ActionResult EditJob(JobsViewModel jobsViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (jobsViewModel.Status == 0)
                    {
                        ModelState.AddModelError("Status", "Status is required!");
                        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        return Json(new { status = "failure", errors = errCollection });
                    }
                    else if (jobsViewModel.Status == Constant.JobStatus.Assigned)
                    {
                        if (jobsViewModel.tempAssignTo2.Count <= 0)
                        {
                            ModelState.AddModelError("tempAssignTo", "Job not assigned!");
                            var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                            return Json(new { status = "failure", errors = errCollection });
                        }
                    }
                    ModelState.Clear();

                    jobsViewModel.CustomerGeneralInfoId = !string.IsNullOrEmpty(jobsViewModel.CustomerInfoId) ?
                                                           Guid.Parse(jobsViewModel.CustomerInfoId) : Guid.Empty;
                    jobsViewModel.SiteId = !string.IsNullOrEmpty(jobsViewModel.tempSiteId) ?
                                            Guid.Parse(jobsViewModel.tempSiteId) : Guid.Empty;
                    jobsViewModel.AssignTo = !string.IsNullOrEmpty(jobsViewModel.tempAssignTo) ?
                                              Guid.Parse(jobsViewModel.tempAssignTo) : (Nullable<Guid>)null;
                    jobsViewModel.BookedBy = Guid.Parse(base.GetUserId);
                    jobsViewModel.JobNotes = jobsViewModel.Job_Notes;
                    jobsViewModel.OperationNotes = jobsViewModel.Operation_Notes;


                    CommonMapper<JobsViewModel, Jobs> mapper = new CommonMapper<JobsViewModel, Jobs>();
                    Jobs jobs = mapper.Mapper(jobsViewModel);
                    jobs.ModifiedDate = DateTime.Now;
                    jobs.ModifiedBy = Guid.Parse(base.GetUserId);

                    Employeejob.DeAttach(jobs);
                    Employeejob.Edit(jobs);
                    Employeejob.Save();

                    // saving supoort job
                    SaveSupportJob(jobsViewModel);

                    var jobAssignmapper = JobAssignMapping.GetAll().Where(m => m.JobId == jobsViewModel.Id).OrderByDescending(i => i.CreatedDate).ToList();
                    foreach (var item in jobAssignmapper)
                    {
                        if (item != null)
                        {

                            JobAssignMapping.Delete(item);
                            JobAssignMapping.Save();
                        }
                    }

                    JobAssignToMappingViewModel jobAssignViewModel = new JobAssignToMappingViewModel();
                    if (jobsViewModel.tempAssignTo2 != null)
                    {

                        foreach (var value in jobsViewModel.tempAssignTo2)
                        {
                            jobAssignViewModel.Id = Guid.NewGuid();
                            jobAssignViewModel.JobId = jobsViewModel.Id;
                            jobAssignViewModel.AssignTo = value;
                            jobAssignViewModel.DateBooked = jobsViewModel.DateBooked;
                            jobAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            jobAssignViewModel.CreatedDate = DateTime.Now;
                            //jobAssignViewModel.StartTime = jobsViewModel.StartTime;
                            jobAssignViewModel.Status = jobsViewModel.Status;
                            CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper = new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                            JobAssignToMapping jobAssignToMapping = Assignmapper.Mapper(jobAssignViewModel);

                            JobAssignMapping.Add(jobAssignToMapping);
                            JobAssignMapping.Save();
                        }
                    }


                    // saving job documents
                    //SaveJobDocs(jobsViewModel.JobDocs, jobsViewModel.Id);
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated job.");


                    return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Employeejob.Dispose();
            }
        }

        private void SaveSupportJob(JobsViewModel jobsViewModel)
        {
            try
            {
                Guid linkJobId = !string.IsNullOrEmpty(jobsViewModel.LinkJobId) ? Guid.Parse(jobsViewModel.LinkJobId) : Guid.Empty;
                var supportdojobMapping = SupportjobMapping.FindBy(m => m.SupportJobId == jobsViewModel.Id).FirstOrDefault();

                if (linkJobId != Guid.Empty)
                {
                    if (supportdojobMapping != null)
                    {
                        supportdojobMapping.LinkedJobId = Guid.Parse(jobsViewModel.LinkJobId);
                        supportdojobMapping.ModifiedBy = Guid.Parse(base.GetUserId);
                        supportdojobMapping.ModifiedDate = DateTime.Now;

                        SupportjobMapping.Edit(supportdojobMapping);
                        SupportjobMapping.Save();
                    }
                    else
                    {
                        SupportdojobMapping supportdojobEntity = new SupportdojobMapping();
                        supportdojobEntity.ID = Guid.NewGuid();
                        supportdojobEntity.SupportJobId = jobsViewModel.Id;

                        supportdojobEntity.LinkedJobId = linkJobId;
                        supportdojobEntity.CreatedBy = Guid.Parse(base.GetUserId);
                        supportdojobEntity.CreatedDate = DateTime.Now;

                        SupportjobMapping.Add(supportdojobEntity);
                        SupportjobMapping.Save();

                    }
                }
                else
                {
                    if (supportdojobMapping != null)
                    {
                        SupportjobMapping.Delete(supportdojobMapping);
                        SupportjobMapping.Save();
                    }
                }
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved support job.");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SupportjobMapping.Dispose();
            }
        }
        public void SaveJobDocs(List<HttpPostedFileBase> JobDocs, Guid Id)
        {
            try
            {
                for (int i = 0; i < JobDocs.Count(); i++)
                {
                    var File = JobDocs[i];
                    if (File != null && File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(File.FileName);
                        string Jobid = Id.ToString();
                        string DateTimeForDoc = DateTime.Now.ToString("MM-dd-yyyy-hh.mm.ss.ffffff");
                        Directory.CreateDirectory(Server.MapPath("~/Images/EmployeeJobs/" + Jobid));
                        File.SaveAs(Path.Combine(Server.MapPath("~/Images/EmployeeJobs/" + Jobid), DateTimeForDoc + "_" + fileName));

                        EmployeeJobDocumentViewModel employeeJobDocumentViewModel = new EmployeeJobDocumentViewModel();
                        employeeJobDocumentViewModel.Id = Guid.NewGuid();
                        employeeJobDocumentViewModel.JobId = Id;
                        employeeJobDocumentViewModel.DocName = fileName.ToString();
                        employeeJobDocumentViewModel.SaveDocName = DateTimeForDoc + "_" + fileName.ToString();
                        employeeJobDocumentViewModel.CreatedDate = DateTime.Now;
                        employeeJobDocumentViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                        CommonMapper<EmployeeJobDocumentViewModel, JobDocuments> mapperdoc = new CommonMapper<EmployeeJobDocumentViewModel, JobDocuments>();
                        JobDocuments employeeJobDocuments = mapperdoc.Mapper(employeeJobDocumentViewModel);
                        EmployeejobDocument.Add(employeeJobDocuments);
                        EmployeejobDocument.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " saved employee job document.");

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EmployeejobDocument.Dispose();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult SiteListByCustomer()
        {
            try
            {
                ModelState.Clear();

                JobsViewModel jobsViewModel = new JobsViewModel();
                Guid CustomerGeneralInfoId = !string.IsNullOrEmpty(Request.QueryString["CustomerInfoId"]) ?
                                              Guid.Parse(Request.QueryString["CustomerInfoId"]) : Guid.Empty;

                var SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).
                         Select(m => new SelectListItem()
                         {
                             Text = m.StreetName,
                             Value = m.SiteDetailId.ToString()
                         }).ToList();

                jobsViewModel.SiteList = SiteList;

                return PartialView("_SiteList", jobsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerSiteDetailRepo.Dispose();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CustomerByJobId()
        {
            try
            {
                ModelState.Clear();

                Guid JobId = !string.IsNullOrEmpty(Request.QueryString["LinkJobId"]) ? Guid.Parse(Request.QueryString["LinkJobId"]) : Guid.Empty;

                var customer = Employeejob.FindBy(m => m.Id == JobId).Select(m => new
                {
                    m.CustomerGeneralInfoId,
                    m.CustomerGeneralInfo.CustomerLastName,
                    m.SiteId,
                    m.CustomerSiteDetail.StreetName
                }).FirstOrDefault();

                return Json(new { Customer = customer }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Employeejob.Dispose();
            }
        }
        public ActionResult GetJobDocuments()
        {
            try
            {
                int JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? int.Parse(Request.QueryString["JobId"]) : 0;
                var employeeJobDocs = EmployeejobDocument.FindBy(m => m.Jobs.JobId == JobId).Select(m => new
                {
                    m.DocName,
                    m.SaveDocName,
                    m.Id
                }).ToList();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " gets list of job document.");

                return Json(new { JobDocs = employeeJobDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EmployeejobDocument.Dispose();
            }
        }
        public ActionResult DownloadDocuments(string Id)
        {
            try
            {
                Guid DocId = Guid.Parse(Id);

                var file = EmployeejobDocument.FindBy(i => i.Id == DocId).FirstOrDefault();
                var FileVirtualPath = Server.MapPath("~/Images/EmployeeJobs/" + file.JobId + '/' + file.SaveDocName);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted document.");

                return File(FileVirtualPath, MimeMapping.GetMimeMapping(FileVirtualPath), file.DocName);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                EmployeejobDocument.Dispose();
            }
        }
        [HttpGet]
        public ActionResult DeletejobDocumentByDocId()
        {
            try
            {
                Guid DocId = !string.IsNullOrEmpty(Request.QueryString["DocId"]) ? Guid.Parse(Request.QueryString["DocId"]) : Guid.Empty;
                int JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? int.Parse(Request.QueryString["JobId"]) : 0;

                var docs = EmployeejobDocument.FindBy(i => i.Id == DocId).FirstOrDefault();
                EmployeejobDocument.Delete(docs);
                EmployeejobDocument.Save();
                if ((System.IO.File.Exists(Server.MapPath("~/Images/EmployeeJobs/" + docs.JobId + '/' + docs.DocName))))
                {
                    System.IO.File.Delete(Server.MapPath("~/Images/EmployeeJobs/" + docs.JobId + '/' + docs.DocName));
                }

                var employeeJobDocs = EmployeejobDocument.FindBy(m => m.Jobs.JobId == JobId).Select(m => new
                {
                    m.DocName,
                    m.SaveDocName,
                    m.Id
                }).ToList();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted job document.");

                return Json(new { JobDocs = employeeJobDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EmployeejobDocument.Dispose();
            }
        }

        [HttpGet]
        public ActionResult UpdateCustomerDetails(string id)
        {
            try
            {
                using (CustomerGeneralInfo)
                {
                    Guid CustomerGeneralInfoId;
                    if (!string.IsNullOrEmpty(id))
                    {
                        Guid.TryParse(id, out CustomerGeneralInfoId);
                    }
                    else
                    {
                        int jobId = (Employeejob.GetMaxJobID() - 1);
                        var jobs = Employeejob.FindBy(m => m.JobId == jobId).FirstOrDefault();
                        CustomerGeneralInfoId = jobs.CustomerGeneralInfoId;
                    }
                    CustomerGeneralInfo customerGeneralInfo = CustomerGeneralInfo.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId)
                                                              .FirstOrDefault();
                    var userName = "";
                    if (customerGeneralInfo.ModifiedBy == null)
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == customerGeneralInfo.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == customerGeneralInfo.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (customerGeneralInfo.ModifiedDate == null)
                    {
                        customerGeneralInfo.CreatedDate = customerGeneralInfo.CreatedDate;
                    }
                    else
                    {
                        customerGeneralInfo.ModifiedDate = customerGeneralInfo.ModifiedDate;
                    }
                    // mapping entity to viewmodel
                    CommonMapper<CustomerGeneralInfo, CustomerGeneralInfoViewModel> mapper = new CommonMapper<CustomerGeneralInfo, CustomerGeneralInfoViewModel>();
                    CustomerGeneralInfoViewModel customerGeneralInfoViewModel = mapper.Mapper(customerGeneralInfo);
                    customerGeneralInfoViewModel.UserName = userName;
                    return View(customerGeneralInfoViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult UpdateCustomerDetails(CustomerGeneralInfoViewModel customerGeneralInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (CustomerGeneralInfo)
                    {
                        customerGeneralInfoViewModel.ModifiedDate = DateTime.Now;
                        customerGeneralInfoViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                        // mapping viewmodel to entity
                        CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo>();
                        CustomerGeneralInfo customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);

                        CustomerGeneralInfo.Edit(customerGeneralInfo);
                        CustomerGeneralInfo.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated customer details.");

                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                    }
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Add New Customer Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="customercontactid"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public ActionResult UpdateSiteContact(string id)
        {
            try
            {
                CustomerContactsViewModel model = new CustomerContactsViewModel();
                using (Customercontacts)
                {
                    Nullable<Guid> custContactid;
                    if (!string.IsNullOrEmpty(id))
                    {
                        custContactid = Guid.Parse(id);
                    }
                    else
                    {
                        int jobId = (Employeejob.GetMaxJobID() - 1);
                        var jobs = Employeejob.FindBy(m => m.JobId == jobId).FirstOrDefault();
                        var siteId = jobs != null ? jobs.SiteId : Guid.Empty;

                        var custometSiteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteId).FirstOrDefault();
                        custContactid = custometSiteDetail != null ? custometSiteDetail.ContactId : Guid.Empty;
                    }

                    CustomerContacts cotactlog = Customercontacts.FindBy(i => i.ContactId == custContactid).FirstOrDefault();

                    if (cotactlog != null)
                    {
                        CommonMapper<CustomerContacts, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContacts, CustomerContactsViewModel>();
                        model = mapper.Mapper(cotactlog);

                        model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.SiteDetailId == cotactlog.SiteId).Select(m =>
                       new SelectListItem { Text = m.StreetName, Value = m.SiteDetailId.ToString() }).ToList();
                        model.SiteList.OrderBy(m => m.Text);
                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Post: Add New Customer Contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects AddCustomerInfo</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateSiteContact(CustomerContactsViewModel model)
        {
            try
            {
                using (Customercontacts)
                {
                    Guid custGeneralinfoid = model.CustomerGeneralInfoId;
                    var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();

                    CommonMapper<CustomerContactsViewModel, CustomerContacts> mapper = new CommonMapper<CustomerContactsViewModel, CustomerContacts>();
                    if (ModelState.IsValid)
                    {
                        if (model.IsBillingContact == true)
                        {
                            foreach (var i in CustomerContactsList)
                            {
                                CustomerContacts contact = Customercontacts.FindBy(m => m.ContactId == i.ContactId).FirstOrDefault();
                                contact.IsBillingContact = false;
                                Customercontacts.Edit(contact);
                                Customercontacts.Save();
                            }
                        }
                        if (model.ContactId != Guid.Empty)
                        {
                            model.ModifiedDate = DateTime.Now;
                            model.ModifiedBy = Guid.Parse(base.GetUserId);
                            CustomerContacts customerContact = Customercontacts.FindBy(m => m.ContactId == model.ContactId).FirstOrDefault();
                            Customercontacts.DeAttach(customerContact);
                            customerContact = mapper.Mapper(model);
                            Customercontacts.Edit(customerContact);
                            Customercontacts.Save();
                        }

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated site contact.");

                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !<strong>" });
                    }
                    else
                    {
                        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        return Json(new { status = "failure", errors = errCollection });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET:Manage Customer Sites
        /// <summary>
        /// Manage Customer Sites Using SiteDetailId
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult UpdateCustomerSiteDetail(string id)
        {
            try
            {
                CustomerSitesViewModel customerSitesViewModel = new CustomerSitesViewModel();
                Nullable<Guid> siteDetailId;
                if (!string.IsNullOrEmpty(id))
                {
                    siteDetailId = Guid.Parse(id);
                }
                else
                {
                    int jobId = (Employeejob.GetMaxJobID() - 1);
                    var jobs = Employeejob.FindBy(m => m.JobId == jobId).FirstOrDefault();
                    siteDetailId = jobs.SiteId;
                }

                CustomerSiteDetail customerSiteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel> mapsitedetail = new CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel>();
                CustomerSiteDetailViewModel customerSiteDetailViewModel = mapsitedetail.Mapper(customerSiteDetail);

                var userName = "";
                if (customerSiteDetailViewModel.ModifiedBy == null)
                {
                    userName = Employee.FindBy(m => m.EmployeeId == customerSiteDetailViewModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                }
                else
                {
                    userName = Employee.FindBy(m => m.EmployeeId == customerSiteDetailViewModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                }
                if (customerSiteDetailViewModel.ModifiedDate == null)
                {
                    customerSiteDetailViewModel.CreatedDate = customerSiteDetailViewModel.CreatedDate;
                }
                else
                {
                    customerSiteDetailViewModel.ModifiedDate = customerSiteDetailViewModel.ModifiedDate;
                }

                CustomerResidenceDetail customerResidenceDetail = CustomerResidence.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                if (customerResidenceDetail != null)
                {
                    CommonMapper<CustomerResidenceDetail, CustomerResidenceDetailViewModel> mapresidencedetail = new CommonMapper<CustomerResidenceDetail, CustomerResidenceDetailViewModel>();
                    CustomerResidenceDetailViewModel customerResidenceDetailViewModel = mapresidencedetail.Mapper(customerResidenceDetail);
                    customerSitesViewModel.CustomerResidenceDetailViewModel = customerResidenceDetailViewModel;
                    customerSitesViewModel.ResidenceDetailId = customerResidenceDetail.ResidenceDetailId;
                }

                CustomerConditionReport customerConditionReport = ConditionReport.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                if (customerConditionReport != null)
                {
                    if (customerConditionReport.ConditionRoof == null)
                    {
                        customerConditionReport.ConditionRoof = 0;
                    }
                }
                if (customerConditionReport != null)
                {
                    CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel> mapconditiondetail = new CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel>();
                    CustomerConditionReportViewModel customerConditionReportViewModel = mapconditiondetail.Mapper(customerConditionReport);
                    customerSitesViewModel.CustomerConditionReportViewModel = customerConditionReportViewModel;
                    customerSitesViewModel.ConditionReportId = customerConditionReport.ConditionReportId;
                }
                customerSitesViewModel.CustomerSiteDetailViewModel = customerSiteDetailViewModel;
                customerSitesViewModel.UserName = userName;
                customerSitesViewModel.ModifiedDate = customerSiteDetailViewModel.ModifiedDate;
                customerSitesViewModel.CustomerGeneralInfoId = customerSiteDetailViewModel.CustomerGeneralInfoId;
                customerSitesViewModel.SiteDetailId = (Guid)siteDetailId;

                // binding drodownlist
                customerSitesViewModel.ContactList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId).Select(m =>
                    new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.ContactList.OrderBy(m => m.Text);

                //binding Strata manager drodownlist
                customerSitesViewModel.StrataManagerList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId && m.IsStrataManager == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.StrataManagerList.OrderBy(m => m.Text);

                //binding Real Estate drodownlist
                customerSitesViewModel.RealEstateList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId && m.IsRealEstate == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.RealEstateList.OrderBy(m => m.Text);
                ViewBag.JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? Guid.Parse(Request.QueryString["JobId"]) : Guid.Empty;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " updated customer site details.");

                return View("_InvoiceSiteForm", customerSitesViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Save Customer 
        /// <summary>
        /// Save List Of Customer
        /// </summary>
        /// <param name="customerSitesViewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveCustomerList(CustomerSitesViewModel customerSitesViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerSiteDetailViewModel customerSiteDetailViewModel = customerSitesViewModel.CustomerSiteDetailViewModel;
                    customerSiteDetailViewModel.SiteDetailId = customerSitesViewModel.SiteDetailId;
                    customerSiteDetailViewModel.ModifiedDate = DateTime.Now;
                    customerSiteDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    CustomerResidenceDetailViewModel customerResidenceDetailViewModel = customerSitesViewModel.CustomerResidenceDetailViewModel;
                    customerResidenceDetailViewModel.ResidenceDetailId = customerSitesViewModel.ResidenceDetailId;
                    customerResidenceDetailViewModel.ModifiedDate = DateTime.Now;
                    customerResidenceDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    CustomerConditionReportViewModel customerConditionReportViewModel = customerSitesViewModel.CustomerConditionReportViewModel;
                    customerConditionReportViewModel.ConditionReportId = customerSitesViewModel.ConditionReportId;
                    customerConditionReportViewModel.ModifiedDate = DateTime.Now;
                    customerConditionReportViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    // saving sitedetail info
                    using (CustomerSiteDetailRepo)
                    {
                        CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail>();
                        CustomerSiteDetail customerSiteDetail = mapsitedetail.Mapper(customerSiteDetailViewModel);
                        customerSiteDetail.CustomerGeneralInfoId = customerSitesViewModel.CustomerGeneralInfoId;

                        // getting lattitude and longitude
                        string address = customerSiteDetailViewModel.Street + " " + customerSiteDetailViewModel.StreetName + " " +
                                        customerSiteDetailViewModel.Suburb + " " + customerSiteDetailViewModel.State + " " + customerSiteDetailViewModel.PostalCode;
                        string[] arrLatLong = { };

                        for (int Index = 0; Index < 4; Index++)
                        {
                            arrLatLong = GetLatitudeLongitude(address);
                            if (arrLatLong[2] == "OVER_QUERY_LIMIT")
                            {
                                System.Threading.Thread.Sleep(2000);
                                if (Index < 3)
                                {
                                    continue;
                                }
                                else
                                {
                                    ModelState.AddModelError("SRASError", @"Query limit exceeded for google map. Please try again on tommorow.");
                                    return Json(ModelState.Values.SelectMany(m => m.Errors));
                                }
                            }
                            break;
                        }

                        if (arrLatLong[0] == "" || arrLatLong[1] == "" || arrLatLong[0] == "0" || arrLatLong[0] == "0")
                        {
                            ModelState.AddModelError("SRASError", @"Site address is not correct!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }

                        customerSiteDetail.Latitude = !string.IsNullOrEmpty(arrLatLong[0]) ? double.Parse(arrLatLong[0]) : (double?)null;
                        customerSiteDetail.Longitude = !string.IsNullOrEmpty(arrLatLong[1]) ? double.Parse(arrLatLong[1]) : (double?)null;

                        CustomerSiteDetailRepo.Edit(customerSiteDetail);
                        CustomerSiteDetailRepo.Save();
                    }

                    // saving residencedetail info
                    using (CustomerResidence)
                    {
                        CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                        CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(customerResidenceDetailViewModel);
                        customerResidenceDetail.SiteDetailId = customerSiteDetailViewModel.SiteDetailId;
                        CustomerResidence.Edit(customerResidenceDetail);
                        CustomerResidence.Save();
                    }

                    // saving conditionreport info
                    using (ConditionReport)
                    {
                        CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport> mapper = new CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport>();
                        CustomerConditionReport customerConditionReport = mapper.Mapper(customerConditionReportViewModel);
                        customerConditionReport.SiteDetailId = customerSiteDetailViewModel.SiteDetailId;
                        ConditionReport.Edit(customerConditionReport);
                        ConditionReport.Save();
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " saved customer list.");

                    return Json(new { status = "saved", msg = "<strong>Record updated successfully !</stong>" });
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string[] GetLatitudeLongitude(string address)
        {
            string url = "http://maps.googleapis.com/maps/api/geocode/json?address=" + address + "&sensor=false";
            string[] arrLatLong = new string[3];
            string result = string.Empty; //Get geocode response 
            WebClient Client = new WebClient();
            using (Stream strm = Client.OpenRead(url))
            {
                StreamReader sr = new StreamReader(strm);
                result = sr.ReadToEnd();
            }
            //Deserialize into .Net object 
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MyGeoCodeResponse _MyGeoCodeResponse = ser.Deserialize<MyGeoCodeResponse>(result);
            string _latitude = _MyGeoCodeResponse.results.Length > 0 ? _MyGeoCodeResponse.results[0].geometry.location.lat : string.Empty;
            string _longitude = _MyGeoCodeResponse.results.Length > 0 ? _MyGeoCodeResponse.results[0].geometry.location.lng : string.Empty;
            arrLatLong[0] = _latitude;
            arrLatLong[1] = _longitude;
            arrLatLong[2] = _MyGeoCodeResponse.status;
            return arrLatLong;
        }
        //GET: Customer/Customer/EditBillingAddress
        /// <summary>
        /// Edit BillingAddress of Customer
        /// </summary>
        /// <param name="BillingAddressId"></param>
        /// <returns></returns>
        public ActionResult UpdateBillingAddress(string id, string customerid)
        {
            try
            {
                CustomerBillingAddressViewModel model = new CustomerBillingAddressViewModel();

                using (CustomerBilling)
                {
                    Guid custbilId = Guid.Empty;
                    CustomerBillingAddress CustomerBillingAdderss = null;

                    Guid customerGeneralInfoId = Guid.Parse(customerid);
                    model.CustomerGeneralInfoId = customerGeneralInfoId;
                    var contacts = Customercontacts.FindBy(m => m.IsBillingContact == true && m.CustomerGeneralInfoId ==
                                   customerGeneralInfoId).FirstOrDefault();


                    if (!string.IsNullOrEmpty(id))
                    {
                        custbilId = Guid.Parse(id);
                        CustomerBillingAdderss = CustomerBilling.FindBy(i => i.BillingAddressId == custbilId).FirstOrDefault();
                    }
                    else
                    {
                        var billingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                        CustomerBillingAddress BillingAddress;
                        if (billingaddress != null)
                            BillingAddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
                        else
                        {
                            BillingAddress = CustomerBilling.GetAll().Where(i => i.CustomerGeneralInfoId == customerGeneralInfoId).OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                        }

                    }

                    if (CustomerBillingAdderss != null)
                    {
                        //mapping entity to viewmodel
                        CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel> mapper = new CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel>();
                        model = mapper.Mapper(CustomerBillingAdderss);
                        var userName = "";
                        if (CustomerBillingAdderss.ModifiedBy == null)
                        {
                            userName = Employee.FindBy(m => m.EmployeeId == CustomerBillingAdderss.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                        }
                        else
                        {
                            userName = Employee.FindBy(m => m.EmployeeId == CustomerBillingAdderss.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                        }
                        if (CustomerBillingAdderss.ModifiedDate == null)
                        {
                            CustomerBillingAdderss.CreatedDate = CustomerBillingAdderss.CreatedDate;
                        }
                        else
                        {
                            CustomerBillingAdderss.ModifiedDate = CustomerBillingAdderss.ModifiedDate;
                        }
                        model.UserName = userName;
                        model.ModifiedDate = CustomerBillingAdderss.ModifiedDate;
                        model.CreatedDate = CustomerBillingAdderss.CreatedDate;
                        return View(model);
                    }

                    if (contacts != null)
                    {
                        model.CustomerTitle = contacts.Title != null ? (Constant.Title)contacts.Title : 0;
                        model.FirstName = contacts.FirstName;
                        model.LastName = contacts.LastName;
                        model.PhoneNo1 = contacts.PhoneNo1;
                        model.PhoneNo2 = contacts.PhoneNo2;
                        model.PhoneNo3 = contacts.PhoneNo3;
                        model.Spare1 = contacts.Spare1;
                        model.Spare2 = contacts.Spare2;
                        model.EmailId = contacts.EmailId;
                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Customer/Customer/EditBillingAddress
        /// <summary>
        /// Edit Billing Address of customer
        /// </summary>
        /// <param name="CustomerBillingAddressModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateBillingAddress(CustomerBillingAddressViewModel customerBillingAddressModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (customerBillingAddressModel.PO == false)
                    {
                        if (customerBillingAddressModel.StreetNo == null)
                        {
                            ModelState.AddModelError("StreetNo", "Street Number is required!");
                        }
                        if (customerBillingAddressModel.StreetName == null)
                        {
                            ModelState.AddModelError("StreetName", "Street Name is required!");
                        }

                        if (customerBillingAddressModel.Suburb == null)
                        {
                            ModelState.AddModelError("Suburb", "Suburb is required!");
                        }
                        if (customerBillingAddressModel.State == null)
                        {
                            ModelState.AddModelError("State", "State is required!");
                        }
                        if (customerBillingAddressModel.PostalCode == null)
                        {
                            ModelState.AddModelError("PostalCode", "Postal Code is required!");
                        }
                        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    }
                    else
                    {
                        if (customerBillingAddressModel.POAddress == null)
                        {
                            ModelState.AddModelError("PoAddress", "PO Address is required!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }
                    }
                    using (CustomerBilling)
                    {
                        if (customerBillingAddressModel.IsDefault)
                        {
                            //  var CustomersAddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerBillingAddressModel.CustomerGeneralInfoId).ToList();
                            CustomerBilling.UpdateDefaultAddress(customerBillingAddressModel.CustomerGeneralInfoId);
                        }

                        var CustomerBillingAdderss = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId ==
                                                     customerBillingAddressModel.CustomerGeneralInfoId).Select(m => m.BillingAddressId)
                                                     .FirstOrDefault();

                        //mapping viewmodel to entity
                        CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress> mapper = new CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress>();
                        CustomerBillingAddress customerBillingAdderss = mapper.Mapper(customerBillingAddressModel);


                        //CustomerBilling.DeleteState(customerBillingAdderss);
                        if (CustomerBillingAdderss == null || CustomerBillingAdderss == Guid.Empty)
                        {
                            customerBillingAdderss.BillingAddressId = Guid.NewGuid();
                            CustomerBilling.Add(customerBillingAdderss);
                        }
                        else
                        {
                            customerBillingAddressModel.ModifiedDate = DateTime.Now;
                            customerBillingAddressModel.ModifiedBy = Guid.Parse(base.GetUserId);
                            customerBillingAdderss.BillingAddressId = CustomerBillingAdderss;
                            CustomerBilling.DeleteState(customerBillingAdderss);
                            CustomerBilling.Edit(customerBillingAdderss);
                        }
                        CustomerBilling.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated billing address.");

                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                    }
                }
                var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                return Json(new { status = "failure", errors = errCollection });
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult ViewCustomerContactLog(string id)
        {
            try
            {
                using (CustomercontactLogRepo)
                {
                    Guid jobGeneralinfoid = Guid.Parse(id);
                    Guid Customercontactid = Guid.Empty;
                    int? jobNo = JobRepository.FindBy(m => m.Id == jobGeneralinfoid).Select(m => m.JobNo).FirstOrDefault();
                    //var CustomerContactList = CustomercontactLog.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid);
                    var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, jobGeneralinfoid.ToString()).ToList();
                    CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
                    List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.OrderByDescending(m => m.LogDate).ToList());

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]); ; // default page size;
                    var customerContractListViewModel = new CustomerContractListViewModel
                    {
                        CustomerContactList = CustomerContactListing.ToList(),
                        PageSize = PageSize,
                        CustomerContactLog = new CustomerContactLog()
                    };
                    customerContractListViewModel.CustomerContactLog.JobId = jobGeneralinfoid.ToString();

                    return View(customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult ViewCustomerContactLogPartial(string JobId, String Keyword)
        {
            try
            {
                using (CustomercontactLogRepo)
                {
                    Guid jobGeneralinfoid = Guid.Parse(JobId);
                    Guid Customercontactid = Guid.Empty;
                    int? jobNo = JobRepository.FindBy(m => m.Id == jobGeneralinfoid).Select(m => m.JobNo).FirstOrDefault();
                    var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, jobGeneralinfoid.ToString(), Keyword).ToList();
                    CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
                    List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.OrderByDescending(m => m.LogDate).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                              Convert.ToInt32(Request.QueryString["page_size"]);

                    var customerContractListViewModel = new CustomerContractListViewModel
                    {
                        CustomerContactList = CustomerContactListing.ToList(),
                        PageSize = PageSize,
                        CustomerContactLog = new CustomerContactLog()
                    };
                    customerContractListViewModel.CustomerContactLog.JobId = jobGeneralinfoid.ToString();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed customer contact log.");

                    return PartialView("_InvoiceJobContactLogList", customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomercontactLogRepo.Dispose();
            }
        }
        //POST:  /Customer/Customer/CustomerContactLogSearch
        /// <summary>
        /// Search Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="Keyword"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContactLogSearch(string JobGeneralinfoid, string Keyword, string PageNum)
        {
            try
            {
                using ((Customercontacts))
                {
                    if (Keyword == null)
                        Keyword = "";

                    Guid Customerid = Guid.Parse(JobGeneralinfoid);
                    int? jobNo = JobRepository.FindBy(m => m.Id == Customerid).Select(m => m.JobNo).FirstOrDefault();
                    var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, JobGeneralinfoid, Keyword).ToList();
                    CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
                    List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);
                    var customerContractListViewModel = new CustomerContractListViewModel
                    {
                        CustomerContactList = CustomerContactListing.ToList(),
                        PageSize = PageSize,
                        CustomerContactLog = new CustomerContactLog()
                    };
                    customerContractListViewModel.CustomerContactLog.JobId = JobGeneralinfoid;

                    return PartialView("_InvoiceJobContactLogList", customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PartialViewResult _CustomercontactAddEdit(string JobId, string Customercontactid, string PageNum, string SiteName)
        {
            try
            {
                CustomerContactLogViewModel model = new CustomerContactLogViewModel();

                Guid jobGeneralInfoId = Guid.Parse(JobId);
                var employeejob = Employeejob.FindBy(i => i.Id == jobGeneralInfoId).FirstOrDefault();
                var Customerjobs = GetCustomerjobsByCustomerid(JobId);

                model.PageNum = PageNum;
                if (!string.IsNullOrEmpty(Customercontactid))
                {
                    using (CustomercontactLogRepo)
                    {
                        Guid custContactid = Guid.Parse(Customercontactid);
                        CustomerContactLog cotactlog = CustomercontactLogRepo.FindBy(i => i.CustomerContactId == custContactid).FirstOrDefault();

                        if (cotactlog != null)
                        {
                            //mapping of entity to viewmodel 
                            CommonMapper<CustomerContactLog, CustomerContactLogViewModel> mapper = new CommonMapper<CustomerContactLog, CustomerContactLogViewModel>();
                            model = mapper.Mapper(cotactlog);
                            model.Customerjobs = Customerjobs;
                            model.SiteName = SiteName;
                            if (cotactlog.LogDate == null)
                            {
                                model.LogDate = model.LogDate = DateTime.Now.Date;
                            }
                            return PartialView(model);
                        }
                        else
                        {
                            return PartialView(model);
                        }
                    }
                }
                else
                {
                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (CustomerGeneralInfo)
                    {
                        Guid customerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                        CustomerGeneralinfo = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
                    }
                    Guid custContactid = Guid.Parse(JobId);
                    model = new CustomerContactLogViewModel();
                    model.Customerjobs = Customerjobs;
                    model.CustomerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                    model.CustomerId = CustomerGeneralinfo.CTId.ToString();
                    model.SiteName = SiteName;
                    model.LogDate = model.LogDate = DateTime.Now.Date;
                    return PartialView(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<CustomerJobs> GetCustomerjobsByCustomerid(string Customerinfoid)
        {
            try
            {
                using (Employeejob)
                {
                    Guid Customerid = Guid.Parse(Customerinfoid);
                    List<CustomerJobs> joblist = new List<CustomerJobs>();
                    var jobs = Employeejob.FindBy(i => i.Id == Customerid).ToList();
                    foreach (var job in jobs)
                    {
                        CustomerJobs obj = new CustomerJobs();
                        obj.CustJobId = job.Id;
                        obj.Jobtext = job.JobNo.ToString();
                        joblist.Add(obj);
                    }
                    return joblist;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult DeleteCustomerContactLog(string Customercontactid, string PageNum)
        {
            try
            {
                using (CustomercontactLogRepo)
                {
                    Guid contactid = Guid.Parse(Customercontactid);
                    CustomerContactLog logtodelete = CustomercontactLogRepo.FindBy(i => i.CustomerContactId == contactid).FirstOrDefault();
                    logtodelete.IsDelete = true;
                    CustomercontactLogRepo.Edit(logtodelete);
                    CustomercontactLogRepo.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted customer contact log.");

                    return RedirectToAction("ViewCustomerContactLogPartial", "CustomerJob", new { JobId = logtodelete.JobId });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult AddJobDocuments(string id)
        {
            try
            {
                var keyWordSearch = "";
                Guid Jobid = Guid.Parse(id);
                var jobSiteDetaillist = Employeejob.FindBy(m => m.Id == Jobid).FirstOrDefault();
                string siteId = (jobSiteDetaillist.SiteId).ToString();
                var customerSiteDetaillist = CustomerSitesDocumentsRepo.GetSiteCountForJob(Guid.Parse(siteId)).ToList();
                var customerSiteDocumentsist = CustomerSitesDocumentsRepo.JobsSiteDocumentList(keyWordSearch, Guid.Parse(siteId)).ToList();
                ViewBag.GridCount = customerSiteDocumentsist.Count();

                CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                List<Customer.ViewModels.SiteDetail> li = new List<Customer.ViewModels.SiteDetail>();
                foreach (var i in customerSiteDetaillist)
                {
                    Customer.ViewModels.SiteDetail obj = new Customer.ViewModels.SiteDetail();
                    obj.SiteDetailId = i.SiteDetailId;
                    obj.SiteAddress = i.SiteAddress;
                    li.Add(obj);
                }
                customersitemodel.siteDetail = li;

                var customerSiteDocumentsListViewModel = new CustomerSiteDocumentsListViewModel
                {
                    CustomerSiteDocumentsCoreViewModelList = customerSiteDocumentsist,
                    SiteCountviewModel = customersitemodel,
                    CustomerSiteDocuments = new CustomerSitesDocuments()
                };
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.SiteId = Guid.Parse(siteId);
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.CustomerGeneralInfoId = jobSiteDetaillist.CustomerGeneralInfoId;

                var invoiceid = InvoiceRep.FindBy(i => i.EmployeeJobId == Jobid).FirstOrDefault().Id;
                ViewBag.invoiceid = invoiceid;
                return View(customerSiteDocumentsListViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult AddJobDocuments(CustomerSiteDocumentsListViewModel model, IEnumerable<HttpPostedFileBase> filename)
        {
            if (model != null)
            {
                try
                {
                    CustomerSitesDocumentsViewModel customerSiteDocumentsViewModel = new CustomerSitesDocumentsViewModel();

                    if (ModelState.IsValid)
                    {
                        using (CustomerSitesDocumentsRepo)
                        {
                            customerSiteDocumentsViewModel.IsDelete = false;
                            customerSiteDocumentsViewModel.CreatedDate = DateTime.Now;
                            customerSiteDocumentsViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                            for (int i = 0; i < filename.Count(); i++)
                            {
                                var File = Request.Files[i];
                                if (File != null && File.ContentLength > 0)
                                {
                                    customerSiteDocumentsViewModel.DocumentId = Guid.NewGuid();
                                    var fileName = Path.GetFileName(File.FileName);
                                    string extension = Path.GetExtension(fileName).ToLower();
                                    string docId = customerSiteDocumentsViewModel.DocumentId.ToString();
                                    Directory.CreateDirectory(Server.MapPath("~/Images/CustomerDocs/" + docId));
                                    File.SaveAs(Path.Combine(Server.MapPath("~/Images/CustomerDocs/" + docId), fileName));

                                    customerSiteDocumentsViewModel.SiteId = model.SiteCountviewModel.SiteDetailId;
                                    customerSiteDocumentsViewModel.CustomerGeneralInfoId = model.CustomerSiteDocuments.CustomerGeneralInfoId;
                                    customerSiteDocumentsViewModel.DocumentName = fileName.ToString();
                                    customerSiteDocumentsViewModel.DocType = GetDocumentType(extension);
                                    CommonMapper<CustomerSitesDocumentsViewModel, CustomerSitesDocuments> mapperdoc = new CommonMapper<CustomerSitesDocumentsViewModel, CustomerSitesDocuments>();
                                    CustomerSitesDocuments customerSiteDocuments = mapperdoc.Mapper(customerSiteDocumentsViewModel);
                                    CustomerSitesDocumentsRepo.Add(customerSiteDocuments);
                                    CustomerSitesDocumentsRepo.Save();
                                }
                            }
                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added job document.");

                            return RedirectToAction("AddDocumentsPartial", "CustomerJob", new { siteID = model.CustomerSiteDocuments.SiteId });
                        }
                    }
                    else
                    {
                        return View(model);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View(model);
        }
        private static string GetDocumentType(string extension)
        {
            string documentType;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".png":
                    documentType = "Image";
                    break;
                case ".doc":
                case ".docx":
                    documentType = "Word Document";
                    break;
                case ".xls":
                case ".xlsx":
                    documentType = "Word Excel";
                    break;
                case ".pdf":
                    documentType = "Pdf";
                    break;
                case ".text":
                case ".txt":
                    documentType = "Text File";
                    break;
                default:
                    documentType = "Unknown";
                    break;
            }

            return documentType;
        }
        public ActionResult AddDocumentsPartial(string siteID)
        {
            try
            {
                var keyWordSearch = "";
                Guid siteId = Guid.Parse(siteID);
                var customerSiteDetaillist = CustomerSitesDocumentsRepo.GetSiteCountForJob(siteId).ToList();
                var customerSiteDocumentsist = CustomerSitesDocumentsRepo.JobsSiteDocumentList(keyWordSearch, siteId).ToList();


                CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                List<Customer.ViewModels.SiteDetail> li = new List<Customer.ViewModels.SiteDetail>();
                foreach (var i in customerSiteDetaillist)
                {
                    Customer.ViewModels.SiteDetail obj = new Customer.ViewModels.SiteDetail();
                    obj.SiteDetailId = i.SiteDetailId;
                    obj.SiteAddress = i.SiteAddress;
                    li.Add(obj);
                }
                customersitemodel.siteDetail = li;

                var customerSiteDocumentsListViewModel = new CustomerSiteDocumentsListViewModel
                {
                    CustomerSiteDocumentsCoreViewModelList = customerSiteDocumentsist,
                    SiteCountviewModel = customersitemodel,
                    CustomerSiteDocuments = new CustomerSitesDocuments()
                };
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.SiteId = siteId;
                return PartialView("_JobSiteDocumentList", customerSiteDocumentsListViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //GET: Employee/Job/Stockinfo
        /// <summary>
        /// StockInfo by jobid
        /// </summary>
        /// <param name="Jobid"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult StockInfo(string Jobid, string InvoiceId)
        {
            try
            {
                Guid jid = Guid.Parse(Jobid);
                var StockList = Stock.GetAll().Where(m => m.IsDelete == false);
                StockList = StockList.Where(i => i.Available > 0 && i.IsDelete == false);
                var JobStockList = JobStock.GetJobStockList().Where(i => i.Jobid == jid).ToList();
                int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 5 :
                               Convert.ToInt32(Request.QueryString["page_size"]);
                DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                displayJobStocksViewModel.JobId = jid;
                List<StockDetail> li = new List<StockDetail>();
                foreach (var i in StockList)
                {
                    StockDetail obj = new StockDetail();
                    obj.StockID = i.ID;
                    obj.Label = i.Label;
                    li.Add(obj);
                }
                displayJobStocksViewModel.stockDetail = li;
                displayJobStocksViewModel.PageSize = PageSize;

                var displayJobStocksListViewModel = new DisplayJobStocksListViewModel
                {
                    DisplayJobStocksViewModel = displayJobStocksViewModel,
                    DisplayJobStocksList = JobStockList,
                    JobStock = new JobStock()
                };
                ViewBag.InvoiceId = InvoiceId;



                return View(displayJobStocksListViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/job/StockInfo
        /// <summary>
        ///Save stockinfo 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects StockInfo</returns>
        [HttpPost]
        public ActionResult StockInfo(DisplayJobStocksListViewModel model)
        {
            if (model != null)
            {
                try
                {
                    DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                    if (ModelState.IsValid)
                    {
                        displayJobStocksViewModel.JobId = model.DisplayJobStocksViewModel.JobId;
                        displayJobStocksViewModel.StockID = model.DisplayJobStocksViewModel.StockID;
                        displayJobStocksViewModel.UnitMeasure = model.DisplayJobStocksViewModel.UnitMeasure;
                        displayJobStocksViewModel.Price = model.DisplayJobStocksViewModel.Price;
                        displayJobStocksViewModel.Quantity = model.DisplayJobStocksViewModel.Quantity;
                        int available = (Convert.ToInt32(model.DisplayJobStocksViewModel.AvailableQuantity) - Convert.ToInt32(displayJobStocksViewModel.Quantity));
                        using (Stock)
                        {
                            var stock = Stock.FindBy(i => i.ID == displayJobStocksViewModel.StockID).FirstOrDefault();
                            stock.Available = available;
                            stock.ModifiedBy = Guid.Parse(base.GetUserId);
                            stock.ModifiedDate = DateTime.Now;
                            Stock.Edit(stock);
                            Stock.Save();
                        }
                        if (model.DisplayJobStocksViewModel.ID == Guid.Empty)
                        {
                            displayJobStocksViewModel.CreatedDate = DateTime.Now;
                            displayJobStocksViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            displayJobStocksViewModel.ID = Guid.NewGuid();
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            jobStock.IsDelete = false;
                            JobStock.Add(jobStock);
                            JobStock.Save();
                        }
                        else
                        {
                            displayJobStocksViewModel.ID = model.DisplayJobStocksViewModel.ID;
                            displayJobStocksViewModel.ModifiedDate = DateTime.Now;
                            displayJobStocksViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            JobStock.Edit(jobStock);
                            JobStock.Save();
                        }

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " stock info.");

                        return RedirectToAction("StockInfo", new { Jobid = displayJobStocksViewModel.JobId });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("StockInfo", new { Jobid = model.DisplayJobStocksViewModel.JobId });
        }

        //POST: Employee/Job/EditStockInfo
        /// <summary>
        /// EditStockInfo by stockid 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditStockInfo(string Id, string stockID)
        {
            try
            {
                using (JobStock)
                {
                    Guid JobStockID = Guid.Parse(Id);
                    Guid StockID = Guid.Parse(stockID);

                    var stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    int available = Convert.ToInt32(stock.Available);

                    JobStock jobStock = JobStock.FindBy(m => m.ID == JobStockID).FirstOrDefault();
                    int quantityauail = Convert.ToInt32(jobStock.Quantity);

                    // mapping entity to viewmodel
                    CommonMapper<JobStock, DisplayJobStocksViewModel> mapper = new CommonMapper<JobStock, DisplayJobStocksViewModel>();
                    DisplayJobStocksViewModel displayJobStocksViewModel = mapper.Mapper(jobStock);

                    displayJobStocksViewModel.AvailableQuantity = available + quantityauail;

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(displayJobStocksViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated stock info.");

                    return Json(new { json, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/Job/DeleteStockInfo
        /// <summary>
        /// DeleteStockInfo By stockid
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns>Redirects StockInfo</returns>
        public ActionResult DeleteStockInfo(string Id, string stockID)
        {
            try
            {
                int newavailablestock;
                Guid Jobid;
                Guid jobStockID = Guid.Parse(Id);
                Guid StockID = Guid.Parse(stockID);
                using (JobStock)
                {
                    JobStock jobStock = JobStock.FindBy(m => m.ID == jobStockID).FirstOrDefault();
                    int quantityauail = Convert.ToInt32(jobStock.Quantity);
                    newavailablestock = quantityauail;
                    Jobid = jobStock.JobId;

                    JobStock stockDelete = JobStock.FindBy(i => i.ID == jobStockID).FirstOrDefault();
                    stockDelete.IsDelete = true;
                    JobStock.Edit(stockDelete);
                    JobStock.Save();
                }
                using (Stock)
                {
                    var stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    int available = Convert.ToInt32(stock.Available);
                    stock.Available = available + newavailablestock;
                    Stock.Edit(stock);
                    Stock.Save();
                }
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted stock info.");

                return Json(Jobid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// GetStockData By Stockid
        /// </summary>
        /// <param name="StockId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStockData(string StockId)
        {
            try
            {
                using (Stock)
                {
                    Guid StockID = Guid.Parse(StockId);
                    Stock stock = Stock.FindBy(i => i.ID == StockID && i.IsDelete == false).FirstOrDefault();
                    CommonMapper<Stock, FSM.Web.Areas.Employee.ViewModels.StockViewModel> mapper = new CommonMapper<Stock, FSM.Web.Areas.Employee.ViewModels.StockViewModel>();
                    FSM.Web.Areas.Employee.ViewModels.StockViewModel displayStocksViewModel = mapper.Mapper(stock);
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(displayStocksViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets stock info.");

                    return Json(new { json, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult ViewJobspurchaseOrder(string Jobid, string InvoiceId)
        {
            try
            {
                using (JobPurchaseOrder)
                {
                    var purchaseorders = JobPurchaseOrder.GetjobPurchaseOrdersByJobId(Jobid);
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = "" }
                    };
                    ViewBag.JobId = Jobid;
                    ViewBag.InvoiceId = InvoiceId;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed purchase order.");

                    return View(model);
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ViewJobspurchaseOrder(PurchaseOrderjobsearchviewModel purchaseOrderjobsearchviewModel)
        {
            try
            {
                using (JobPurchaseOrder)
                {
                    string jobId = (purchaseOrderjobsearchviewModel.JobId).ToString();
                    string Searchstring = purchaseOrderjobsearchviewModel.SearchKeyword;
                    //var stocks =Stock.GetAll();
                    var purchaseorders = JobPurchaseOrder.GetjobPurchaseOrders(Searchstring);
                    purchaseorders = string.IsNullOrEmpty(jobId) ? purchaseorders : purchaseorders.Where(i => (i.JobID).ToString().Contains(jobId));
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };
                    ViewBag.JobId = jobId;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job purchase order.");

                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]

        public ActionResult AddJobPurchaseOrder(string JobId, string InvoiceId)
        {
            try
            {
                Guid id = Guid.Parse(JobId);
                var employeerJobList = Employeejob.FindBy(i => i.Id == id).AsEnumerable();
                GetJobViewModel getJobViewModel = new GetJobViewModel();
                List<EmployeeJobDetail> li = new List<EmployeeJobDetail>();
                foreach (var i in employeerJobList)
                {
                    EmployeeJobDetail obj = new EmployeeJobDetail();
                    obj.EmployeeJobId = i.Id;
                    obj.JobNo = i.JobNo;
                    obj.Description = "JobNo_" + obj.JobNo;
                    li.Add(obj);
                }
                getJobViewModel.employeeJobDetail = li;
                ViewBag.JobId = JobId;
                ViewBag.InvoiceId = InvoiceId;
                getJobViewModel.SupplierList = Supplier.GetAll().Select(m => new SelectListItem()
                {
                    Text = m.Name,
                    Value = m.ID.ToString()
                }).ToList();

                getJobViewModel.PurchaseOrderNo = JobPurchaseOrder.GetMaxPurchaseNo();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added job purchase order.");

                return View(getJobViewModel);

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public ActionResult AddEditJobPurchaseOrder(string Purchaseorderid, string JobId, string InvoiceId)
        {
            List<SupplierJobItem> SupplierJobList = new List<SupplierJobItem>();
            using (Supplier)
            {
                var suppliers = Supplier.GetAll();
                foreach (var supplier in suppliers)
                {
                    SupplierJobItem obj = new SupplierJobItem();
                    obj.ID = supplier.ID;
                    obj.Name = supplier.Name;
                    SupplierJobList.Add(obj);
                }
            }

            List<EmployeeJobDetail> empjobdetail = new List<EmployeeJobDetail>();
            using (Employeejob)
            {
                Guid id = Guid.Parse(JobId);
                var employeerJobList = Employeejob.FindBy(i => i.Id == id).AsEnumerable();

                GetJobViewModel getJobViewModel = new GetJobViewModel();
                foreach (var i in employeerJobList)
                {
                    EmployeeJobDetail obj = new EmployeeJobDetail();
                    obj.EmployeeJobId = i.Id;
                    obj.JobNo = i.JobNo;
                    obj.Description = "JobNo_" + obj.JobNo;
                    empjobdetail.Add(obj);
                }
            }

            using (Stock)
            {
                var stocks = Stock.GetAll();
                List<StockJobList> StockJobList = new List<StockJobList>();
                foreach (var stock in stocks)
                {
                    StockJobList sitem = new StockJobList();
                    sitem.StockId = stock.ID;
                    sitem.StockName = stock.Label;
                    StockJobList.Add(sitem);
                }

                JobPurChaseViewModel model = new JobPurChaseViewModel
                {
                    PurchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel(),
                    PurchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel(),
                    getjobviewmodel = new GetJobViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                }
                model.PurchaseOrderITemByJobViewModel.StockJoblist = StockJobList.OrderBy(i => i.StockName).ToList();
                model.PurchaseOrderByJobViewModel.SupplierJobList = SupplierJobList.OrderBy(i => i.Name).ToList();
                model.getjobviewmodel.employeeJobDetail = empjobdetail.OrderBy(i => i.EmployeeJobId).ToList();
                ViewBag.JobId = JobId;
                ViewBag.InvoiceId = InvoiceId;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added job purchase order.");

                return View(model);
            }
        }
        [HttpGet]
        public ActionResult JobDocuments(string InvoiceId)
        {
            try
            {
                Guid invoiceId = Guid.Parse(InvoiceId);
                var invoiceData = InvoiceRep.FindBy(m => m.Id == invoiceId).FirstOrDefault();
                Jobs jobData = new Jobs();
                if (invoiceData.InvoiceType == "Invoice")
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoiceData.JobId && m.IsDelete == false && m.JobType == 2).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }
                else
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoiceData.JobId && m.IsDelete == false && m.JobType == 1).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }

                if (invoiceData.InvoiceType == "Invoice" && jobData == null)
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoiceData.JobId && m.IsDelete == false).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }
                else
                {
                    jobData = JobRepository.FindBy(m => m.JobNo == invoiceData.JobId && m.IsDelete == false).OrderByDescending(i => i.CreatedDate).FirstOrDefault();   //getNextContractDueDate job data
                }
                var jobDocList = EmployeejobDocument.FindBy(m => m.JobId == jobData.Id && m.IsDelete == false).ToList();
                CommonMapper<JobDocuments, JobDocumentList> mapper = new CommonMapper<JobDocuments, JobDocumentList>();
                var jobdocs = mapper.MapToList(jobDocList); // job docs list

                JobDocViewModel jobDocViewModel = new JobDocViewModel();
                jobDocViewModel.JobId = jobData.Id;
                jobDocViewModel.jobDocumentList = jobdocs;

                return View(jobDocViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ViewInvoiceContacts(string siteId, string customerGeneralInfoId, string invoiceId)
        {
            try
            {
                using (Customercontacts)
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    string Searchstring = Request.QueryString["searchkeyword"];
                    int ContactType = 0;
                    Guid sitedetailId = Guid.Parse(siteId);
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralInfoId);
                    Guid Customercontactid = Guid.Empty;
                    var CustomerContactsList = Customercontacts.GetJobContactsInfo(custGeneralinfoid, sitedetailId, ContactType, Searchstring).ToList();
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactsList.ToList());

                    List<CustomerContactsViewModel> customerSiteCollection =
                            customerContactsCoreViewModel.Select(m => new CustomerContactsViewModel
                            {

                                FirstName = m.FirstName,
                                LastName = m.LastName,
                                PhoneNo1 = m.PhoneNo1,
                                EmailId = m.EmailId,
                                SiteAddress = m.SiteAddress,
                                ContactId = m.ContactId,
                                CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                                DisplayContactsType = (int)m.ContactsType != 0 ? m.ContactsType.GetAttribute<DisplayAttribute>() != null ? m.ContactsType.GetAttribute<DisplayAttribute>().Name : m.ContactsType.ToString() : string.Empty
                            }).ToList();


                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);  // default page size;
                    contactsSearchViewModel.PageSize = PageSize;

                    CustomerContactsViewModel model = new CustomerContactsViewModel();


                    if (CustomerContactsList.Count == 1)
                    {
                        var ContactId = CustomerContactsList[0].ContactId;
                        CustomerContacts customerContacts = Customercontacts.FindBy(i => i.ContactId == ContactId).FirstOrDefault();
                        CommonMapper<CustomerContacts, CustomerContactsViewModel> mapper1 = new CommonMapper<CustomerContacts, CustomerContactsViewModel>();
                        model = mapper1.Mapper(customerContacts);
                    }


                    model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == custGeneralinfoid).Select(m =>
                    new SelectListItem { Text = m.StreetName, Value = m.SiteDetailId.ToString() }).ToList();
                    model.SiteList.OrderBy(m => m.Text);

                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = model,
                        ContactCount = CustomerContactsList.Count
                    };
                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = custGeneralinfoid;
                    customerContactsListViewModel.CustomerContacts.SiteId = sitedetailId;
                    customerContactsListViewModel.customerContactsViewModel.InvoiceId = invoiceId;
                    customerContactsListViewModel.customerContactsViewModel.UserName = base.GetUserName;
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job contact.");

                    return View(customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:  /Employee/Invoice/ViewInvoiceContacts
        /// <summary>
        /// Search Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="name"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewInvoiceContacts(string CustomerGeneralinfoid, string SiteId, int ContactType, string name, string PageNum)
        {
            try
            {
                using ((Customercontacts))
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    if (name == null)
                        name = "";

                    Guid Customerid = Guid.Parse(CustomerGeneralinfoid);
                    Guid siteDetailId = Guid.Parse(SiteId);
                    var CustomerContactsList = Customercontacts.GetJobContactsInfo(Customerid, siteDetailId, ContactType, name).ToList();
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactsList.ToList());

                    List<CustomerContactsViewModel> customerSiteCollection =
                            customerContactsCoreViewModel.Select(m => new CustomerContactsViewModel
                            {

                                FirstName = m.FirstName,
                                LastName = m.LastName,
                                PhoneNo1 = m.PhoneNo1,
                                EmailId = m.EmailId,
                                SiteAddress = m.SiteAddress,
                                ContactId = m.ContactId,
                                CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                                DisplayContactsType = (int)m.ContactsType != 0 ? m.ContactsType.GetAttribute<DisplayAttribute>() != null ? m.ContactsType.GetAttribute<DisplayAttribute>().Name : m.ContactsType.ToString() : string.Empty
                            }).ToList();

                    contactsSearchViewModel.PageSize = Request.QueryString["page_size"] == null ? 10 : Convert.ToInt32(Request.QueryString["page_size"]); ;
                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = new CustomerContactsViewModel()
                    };

                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = Customerid;
                    customerContactsListViewModel.CustomerContacts.SiteId = siteDetailId;
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed invoice contact.");
                    return PartialView("_InvoiceContactsList", customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        ///Post: View All Customer Contacts
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewInvoiceContactsPartial(string SiteId, string customerGeneralinfoid, int ContactType, string Keyword)
        {
            try
            {
                using (Customercontacts)
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    Guid siteDetailId = Guid.Parse(SiteId);
                    Guid Customercontactid = Guid.Empty;
                    //var CustomerContactList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();
                    var CustomerContactsList = Customercontacts.GetJobContactsInfo(custGeneralinfoid, siteDetailId, ContactType, Keyword).ToList();
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactsList.ToList());

                    List<CustomerContactsViewModel> customerSiteCollection =
                            customerContactsCoreViewModel.Select(m => new CustomerContactsViewModel
                            {

                                FirstName = m.FirstName,
                                LastName = m.LastName,
                                PhoneNo1 = m.PhoneNo1,
                                EmailId = m.EmailId,
                                SiteAddress = m.SiteAddress,
                                ContactId = m.ContactId,
                                CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                                DisplayContactsType = (int)m.ContactsType != 0 ? m.ContactsType.GetAttribute<DisplayAttribute>() != null ? m.ContactsType.GetAttribute<DisplayAttribute>().Name : m.ContactsType.ToString() : string.Empty
                            }).ToList();

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);  // default page size;
                    contactsSearchViewModel.PageSize = PageSize;
                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = new CustomerContactsViewModel()
                    };
                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = custGeneralinfoid;
                    customerContactsListViewModel.CustomerContacts.SiteId = siteDetailId;
                    return PartialView("_InvoiceContactsList", customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Add New Customer Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="customercontactid"></param>
        /// <param name="SiteId"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public PartialViewResult _UpdateInvoiceContactsAddEdit(string CustomerGeneralinfoid, string SiteId, string InvoiceId, string customercontactid, string PageNum)
        {
            try
            {
                CustomerContactsViewModel model = new CustomerContactsViewModel();
                Guid customerGeneralinfoid = Guid.Parse(CustomerGeneralinfoid);
                Guid siteDetailId = Guid.Parse(SiteId);
                model.PageNum = PageNum;
                if (!string.IsNullOrEmpty(customercontactid))
                {
                    using (Customercontacts)
                    {
                        Guid custContactid = Guid.Parse(customercontactid);
                        CustomerContacts cotactlog = Customercontacts.FindBy(i => i.ContactId == custContactid).FirstOrDefault();

                        if (cotactlog != null)
                        {
                            CommonMapper<CustomerContacts, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContacts, CustomerContactsViewModel>();
                            model = mapper.Mapper(cotactlog);
                            model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.SiteDetailId == siteDetailId).Select(m =>
                            new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).ToList();
                            model.SiteList.OrderBy(m => m.Text);
                            model.InvoiceId = InvoiceId;
                            return PartialView(model);
                        }
                        else
                        {

                            return PartialView(model);
                        }
                    }
                }
                else
                {
                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (CustomerGeneralInfo)
                    {
                        CustomerGeneralinfo = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralinfoid).FirstOrDefault();
                    }
                    Guid custContactid = Guid.Parse(CustomerGeneralinfoid);
                    model = new CustomerContactsViewModel();
                    model.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);

                    model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.SiteDetailId == siteDetailId).Select(m =>
                       new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).ToList();
                    model.SiteList.OrderBy(m => m.Text);
                    model.HideAddContacts = Request.QueryString["HideAddContacts"];
                    model.InvoiceId = InvoiceId;
                    return PartialView(model);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Post: Add New Customer Contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects AddCustomerInfo</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult _UpdateJobContactsAddEdit(CustomerContactsViewModel model)
        {
            if (model != null)
            {
                try
                {
                    using (Customercontacts)
                    {
                        Guid custGeneralinfoid = model.CustomerGeneralInfoId;
                        var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();

                        CommonMapper<CustomerContactsViewModel, CustomerContacts> mapper = new CommonMapper<CustomerContactsViewModel, CustomerContacts>();
                        if (ModelState.IsValid)
                        {
                            if (model.ContactId != Guid.Empty)
                            {
                                model.ModifiedDate = DateTime.Now;
                                model.ModifiedBy = Guid.Parse(base.GetUserId);
                                CustomerContacts customerContact = Customercontacts.FindBy(m => m.ContactId == model.ContactId).FirstOrDefault();
                                Customercontacts.DeAttach(customerContact);
                                customerContact = mapper.Mapper(model);
                                Customercontacts.Edit(customerContact);
                                Customercontacts.Save();

                                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                log.Info(base.GetUserName + " updated job contact.");

                                return RedirectToAction("SaveInvoiceInfo", new { id = model.InvoiceId, activetab = "Contact", success = "ok", pagenum = model.PageNum });
                            }
                            else
                            {
                                model.IsDelete = false;
                                model.CreatedDate = DateTime.Now;
                                model.CreatedBy = Guid.Parse(base.GetUserId);
                                model.ContactId = Guid.NewGuid();
                                CustomerContacts customerContacts = mapper.Mapper(model);
                                Customercontacts.Add(customerContacts);
                                Customercontacts.Save();

                                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                log.Info(base.GetUserName + " added job contact.");

                                return RedirectToAction("SaveInvoiceInfo", new { id = model.InvoiceId, activetab = "Site Contact", success = "ok", pagenum = model.PageNum });
                            }
                        }
                        else
                        {
                            return PartialView(model);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return PartialView(model);
        }
        //POST:  /Employee/Invoice/DeleteInvoiceContacts
        /// <summary>
        /// Delete customer Contact 
        /// </summary>
        /// <param name="Customercontactid"></param>
        /// <returns></returns>
        public ActionResult DeleteInvoiceContacts(string Customercontactid, string InvoiceId)
        {
            try
            {
                using (Customercontacts)
                {
                    Guid contactid = Guid.Parse(Customercontactid);
                    CustomerContacts contactdelete = Customercontacts.FindBy(i => i.ContactId == contactid).FirstOrDefault();
                    contactdelete.IsDelete = true;
                    Customercontacts.Edit(contactdelete);
                    Customercontacts.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted invoice contact.");

                    return Json(InvoiceId, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GetBillingDetail(Guid BillingAddressId)
        {
            try
            {
                ModelState.Clear();

                CustomerBillingAddressViewModel billingDetailViewModel = new CustomerBillingAddressViewModel();

                var BillingAddressDetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId && (m.IsDelete == false || m.IsDelete == null)).FirstOrDefault();
                if (BillingAddressDetail != null)
                {
                    billingDetailViewModel.EmailId = BillingAddressDetail.EmailId;
                    billingDetailViewModel.PhoneNo1 = BillingAddressDetail.PhoneNo1;
                }


                return Json(billingDetailViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerSiteDetailRepo.Dispose();
            }
        }
        //GET: Employee/Invoice/ManageStockinfo
        /// <summary>
        /// Invoice JCL Info by Invoice 
        /// </summary>
        /// <param name="Jobid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageJCLItem(Guid invoiceid)
        {
            try
            {
                using (JCLItemInvoiceRepo)
                {
                    var JCLInvoiceList = JCLItemInvoiceRepo.GetJCLInvoiceList(invoiceid).ToList();
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 5 :
                                   Convert.ToInt32(Request.QueryString["page_size"]);
                    DisplayJCLItemInvoiceViewModel displayJclItemViewModel = new DisplayJCLItemInvoiceViewModel();
                    displayJclItemViewModel.InvoiceId = invoiceid;

                    var JCLItemList = JCLRepo.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.ItemName,
                        Value = m.JCLId.ToString()
                    }).ToList();

                    displayJclItemViewModel.ItemNameList = JCLItemList;
                    displayJclItemViewModel.PageSize = PageSize;

                    CommonMapper<JCLItemInvoiceCoreViewModel, DisplayJCLItemInvoiceViewModel> mapper = new CommonMapper<JCLItemInvoiceCoreViewModel, DisplayJCLItemInvoiceViewModel>();
                    List<DisplayJCLItemInvoiceViewModel> JCLItemListList = mapper.MapToList(JCLInvoiceList);

                    List<DisplayJCLItemInvoiceViewModel> JCLInvoiceCollection =
                        JCLItemListList.Select(m => new DisplayJCLItemInvoiceViewModel
                        {
                            Id = m.Id,
                            JCLId = m.JCLId,
                            InvoiceId = m.InvoiceId,
                            ItemName = m.ItemName,
                            BonusPerItem = m.BonusPerItem,
                            DisplayCategoryName = m.Category != null ? (int)m.Category != 0 ? m.Category.GetAttribute<DisplayAttribute>() != null ? m.Category.GetAttribute<DisplayAttribute>().Name : m.Category.ToString() : string.Empty : string.Empty,
                            DefaultQty = m.DefaultQty
                        }).ToList();

                    var displayJCLItemInvoiceListViewModel = new DisplayJCLItemInvoiceListViewModel
                    {
                        displayJCLItemInvoiceViewModel = displayJclItemViewModel,
                        DisplayJCLInvoiceList = JCLInvoiceCollection
                    };
                    return View(displayJCLItemInvoiceListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Invoice/ManageJCLItem
        /// <summary>
        ///Save JCL Item 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirect ManageJCLItem</returns>
        [HttpPost]
        public ActionResult ManageJCLItem(DisplayJCLItemInvoiceListViewModel model)
        {
            if (model != null)
            {

                try
                {
                    DisplayJCLItemInvoiceViewModel displayJCLItemInvoiceViewModel = new DisplayJCLItemInvoiceViewModel();
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    if (ModelState.IsValid)
                    {
                        displayJCLItemInvoiceViewModel.InvoiceId = model.displayJCLItemInvoiceViewModel.InvoiceId;
                        displayJCLItemInvoiceViewModel.JCLId = model.displayJCLItemInvoiceViewModel.JCLId;

                        if (model.displayJCLItemInvoiceViewModel.Id == Guid.Empty)
                        {
                            displayJCLItemInvoiceViewModel.CreatedDate = DateTime.Now;
                            displayJCLItemInvoiceViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            displayJCLItemInvoiceViewModel.Id = Guid.NewGuid();
                            CommonMapper<DisplayJCLItemInvoiceViewModel, JCLItemInvoiceMapping> mapperdoc = new CommonMapper<DisplayJCLItemInvoiceViewModel, JCLItemInvoiceMapping>();
                            JCLItemInvoiceMapping jclEntity = mapperdoc.Mapper(displayJCLItemInvoiceViewModel);
                            JCLItemInvoiceRepo.Add(jclEntity);
                            JCLItemInvoiceRepo.Save();
                        }
                        else
                        {
                            displayJCLItemInvoiceViewModel.Id = model.displayJCLItemInvoiceViewModel.Id;
                            displayJCLItemInvoiceViewModel.ModifiedDate = DateTime.Now;

                            displayJCLItemInvoiceViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                            CommonMapper<DisplayJCLItemInvoiceViewModel, JCLItemInvoiceMapping> mapperdoc = new CommonMapper<DisplayJCLItemInvoiceViewModel, JCLItemInvoiceMapping>();
                            JCLItemInvoiceMapping jclEntity = mapperdoc.Mapper(displayJCLItemInvoiceViewModel);
                            JCLItemInvoiceRepo.Edit(jclEntity);
                            JCLItemInvoiceRepo.Save();
                        }

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " managed JCL items.");

                        return RedirectToAction("ManageJCLItem", new { invoiceid = displayJCLItemInvoiceViewModel.InvoiceId });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("ManageJCLItem", new { invoiceid = model.displayJCLItemInvoiceViewModel.InvoiceId });
        }
        //POST: Employee/Invoice/GetJCLData
        /// <summary>
        /// GetJCLData By JCLId
        /// </summary>
        /// <param name="JCLId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetJCLData(Guid JCLId)
        {
            try
            {
                using (JCLRepo)
                {
                    JCL jcl = JCLRepo.FindBy(i => i.JCLId == JCLId).FirstOrDefault();
                    CommonMapper<JCL, JCLDetailViewModel> mapper = new CommonMapper<JCL, JCLDetailViewModel>();
                    JCLDetailViewModel jclDetailViewModel = mapper.Mapper(jcl);
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(jclDetailViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets JCL data.");

                    return Json(new { json, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST: Employee/Invoice/EditManageJCLItem
        /// <summary>
        /// EditManageJCLItem by JCLId 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditManageJCLItem(Guid ID, Guid JCLId, Guid invoiceid)
        {
            try
            {
                using (JobStock)
                {
                    var JCLData = JCLRepo.FindBy(i => i.JCLId == JCLId).FirstOrDefault();


                    // mapping entity to viewmodel
                    CommonMapper<JCL, DisplayJCLItemInvoiceViewModel> mapper = new CommonMapper<JCL, DisplayJCLItemInvoiceViewModel>();
                    DisplayJCLItemInvoiceViewModel displayJCLItemInvoice = mapper.Mapper(JCLData);

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(displayJCLItemInvoice);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated manage JCL items.");

                    return Json(new { json, JsonRequestBehavior.AllowGet });

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET: Employee/Invoice/DeleteJCLInvoice
        /// <summary>
        /// DeleteJCLInvoice By id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Redirects json</returns>
        public ActionResult DeleteJCLInvoice(Guid Id)
        {
            try
            {
                using (JCLItemInvoiceRepo)
                {
                    JCLItemInvoiceMapping jclInvoiceMapping = JCLItemInvoiceRepo.FindBy(m => m.Id == Id).FirstOrDefault();
                    JCLItemInvoiceRepo.Delete(jclInvoiceMapping);
                    JCLItemInvoiceRepo.Save();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted JCL invoice.");

                return Json(Id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get GCLI tem Detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetGCLItemDetail(Guid JCLId)
        {
            try
            {
                if (JCLId != Guid.Empty)
                {
                    JCLItems ob = new JCLItems();
                    var JCLInfo = JCLRepository.FindBy(i => i.JCLId == JCLId).ToList();
                    List<JcLViewModel> items = new List<JcLViewModel>();
                    foreach (var i in JCLInfo)
                    {
                        JcLViewModel item = new JcLViewModel();
                        item.Description = i.Description;
                        item.DefaultQty = i.DefaultQty;
                        item.Price = i.Price;
                        item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                        items.Add(item);
                    }
                    ob.JCLInfo = items;
                    ob.JCLItemList = JCLRepo.FindBy(i => i.JCLId == JCLId).Select(m => new SelectListItem()
                    {
                        Text = m.ItemName,
                        Value = m.JCLId.ToString()
                    }).ToList();
                    ob.jclProductlist = JCLProductRepo.FindBy(i => i.JCLId == JCLId).Select(m => new SelectListItem()
                    {
                        Text = m.ProductName,
                        Value = m.ProductId.ToString()
                    }).ToList();
                    ob.jclcolorlist = JCLColorRepo.FindBy(i => i.JCLId == JCLId).Select(m => new SelectListItem()
                    {
                        Text = m.ColorName,
                        Value = m.ColorId.ToString()
                    }).ToList();
                    ob.jclSizeList = JCLSizeRepo.FindBy(i => i.JCLId == JCLId).Select(m => new SelectListItem()
                    {
                        Text = m.Size,
                        Value = m.SizeId.ToString()
                    }).ToList();
                    ob.JCLInfo = items;
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(ob);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets JCL item details.");

                    return Json(new { JCLList = jsonSerialiser.Serialize(ob.JCLItemList), Sizlist = jsonSerialiser.Serialize(ob.jclSizeList), Productlist = jsonSerialiser.Serialize(ob.jclProductlist), Colorlist = jsonSerialiser.Serialize(ob.jclcolorlist), JcLinfo = jsonSerialiser.Serialize(ob.JCLInfo) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Status = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Delete JclI Tem From Invoice
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult DeleteJclITemFromInvoice(Guid InvoiceJckMappingid)
        {
            try
            {
                if (InvoiceJckMappingid != Guid.Empty)
                {
                    var jclitem = invoiceJCLItemRepo.FindBy(i => i.ID == InvoiceJckMappingid).FirstOrDefault();   //get data before delete

                    if (jclitem != null)
                    {
                        invoiceJCLItemRepo.Delete(jclitem);
                        invoiceJCLItemRepo.Save();


                        var jclItems = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == jclitem.InvoiceId).ToList();   //get data after delete

                        if (jclItems.Count > 0)
                        {
                            var invoiceData = InvoiceRep.FindBy(m => m.Id == jclitem.InvoiceId).FirstOrDefault();   //Get invoice data
                            if (invoiceData != null)
                            {
                                var totalPriceGST = Convert.ToDecimal(jclItems.Sum(i => i.TotalPrice) * 10 / Convert.ToDecimal(100));
                                totalPriceGST = totalPriceGST + Convert.ToDecimal(jclItems.Sum(i => i.TotalPrice));
                                invoiceData.Price = Convert.ToDecimal(totalPriceGST);
                                InvoiceRep.Edit(invoiceData);
                                InvoiceRep.Save();                                                                  //update invoice price
                            }
                        }

                        return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = false }, JsonRequestBehavior.AllowGet);
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted JCL items from invoice.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpGet]
        public ActionResult DeletePaymentFromInvoice(Guid InvoicePaymentid)
        {
            try
            {
                if (InvoicePaymentid != Guid.Empty)
                {
                    var invoicitem = InvoicePaymentRepo.FindBy(i => i.Id == InvoicePaymentid).FirstOrDefault();
                    if (invoicitem != null)
                    {
                        InvoicePaymentRepo.Delete(invoicitem);
                        InvoicePaymentRepo.Save();
                        return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = false }, JsonRequestBehavior.AllowGet);
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted payment from invoice.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]

        public ActionResult InsertJclITemForInvoice(List<DisplayinvoiceJCLITemModel> jclitems)
        {
            try
            {
                if (jclitems != null)
                {
                    TempData["JcLITemInvoice"] = null;
                    List<InvoicedJCLItemMapping> objtemp = new List<InvoicedJCLItemMapping>();
                    int i = 1;
                    foreach (DisplayinvoiceJCLITemModel item in jclitems)
                    {

                        InvoicedJCLItemMapping obj = new InvoicedJCLItemMapping();
                        if (String.IsNullOrEmpty(item.Id))
                        {
                            obj.ID = Guid.Empty;
                        }
                        else
                        {
                            obj.ID = Guid.Parse(item.Id);
                        }
                        obj.JCLItemID = Guid.Parse(item.GCLID);
                        obj.JobID = Guid.Parse(item.JobId);
                        if (!string.IsNullOrEmpty(item.Colorid))
                            obj.ColorID = Guid.Parse(item.Colorid);
                        if (!string.IsNullOrEmpty(item.Sizeid))
                            obj.SizeID = Guid.Parse(item.Sizeid);
                        if (!string.IsNullOrEmpty(item.Productid))
                            obj.ProductStyleID = Guid.Parse(item.Productid);
                        if (String.IsNullOrEmpty(item.Price))
                            item.Price = "0";
                        if (item.Price.ToLower() == "null")
                            item.Price = "0";
                        if (!string.IsNullOrEmpty(item.Price))
                            obj.Price = Convert.ToDecimal(item.Price);
                        if (String.IsNullOrEmpty(item.DefaultQty))
                            item.DefaultQty = "0";
                        if (item.DefaultQty.ToLower() == "null")
                            item.DefaultQty = "0";
                        if (!string.IsNullOrEmpty(item.DefaultQty))
                            obj.Quantity = Convert.ToInt32(item.DefaultQty);
                        if (String.IsNullOrEmpty(item.TotalPrice))
                            item.TotalPrice = "0";
                        if (item.TotalPrice.ToLower() == "null")
                            item.TotalPrice = "0";
                        if (!string.IsNullOrEmpty(item.TotalPrice))
                            obj.TotalPrice = Convert.ToDecimal(item.TotalPrice);
                        if (!string.IsNullOrEmpty(item.Description))
                            obj.Description = Convert.ToString(item.Description);
                        obj.CreateDate = DateTime.Now;
                        obj.CreatedBy = Guid.Parse(base.GetUserId);
                        obj.InvoiceId = Guid.Empty;
                        obj.OrderNo = i;
                        i++;
                        objtemp.Add(obj);
                    }
                    TempData["JcLITemInvoice"] = objtemp;
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " inserted JCL item for invoice.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Rename Site Document
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        public ActionResult RenameSiteDocument(string Id, string Newfilename)
        {
            try
            {
                bool status = true;
                string Message = "";
                string filename = "";
                if (Newfilename.Length > 100)
                {
                    status = false;
                    Message = "File name length can't be greater than 100";
                    filename = "";
                }
                else
                {
                    try
                    {
                        using (CustomerSitesDocumentsRepo)
                        {
                            Guid docId = Guid.Parse(Id);
                            CustomerSitesDocuments ImageList = CustomerSitesDocumentsRepo.FindBy(i => i.DocumentId == docId).FirstOrDefault();
                            string ImageName = Path.GetFileNameWithoutExtension(ImageList.DocumentName);
                            string extension = Path.GetExtension(ImageList.DocumentName);
                            var FileVirtualPath = "/Images/CustomerDocs/" + docId + '/' + ImageName + extension;
                            var sourcePath = Path.Combine(Server.MapPath("~/Images/CustomerDocs/" + docId + '/' + ImageName + extension));
                            var destinationPath = Path.Combine(Server.MapPath("~/Images/CustomerDocs/" + docId + '/' + Newfilename + extension));
                            System.IO.File.Move(sourcePath, destinationPath);
                            status = true;
                            Message = "File renamed successfully";
                            ImageList.DocumentName = Newfilename + extension;
                            filename = Newfilename + extension;
                            ImageList.ModifiedBy = Guid.Parse(base.GetUserId);
                            CustomerSitesDocumentsRepo.Edit(ImageList);
                            CustomerSitesDocumentsRepo.Save();
                        }
                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " renamed site document.");
                    }
                    catch (Exception ex)
                    {
                        status = false;
                        Message = "Error Occured while Renaming the file";
                        throw ex;
                    }

                }
                return Json(new { Status = status, message = Message, file_name = filename }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Employee/Invoice/GETPaymentMethod
        /// <summary>
        /// Get Jcl Item
        /// </summary>
        /// <param name=""></param>
        /// <returns>return json</returns>
        public ActionResult GETPaymentMethod()
        {
            try
            {

                InvoicePaymentList Model = new InvoicePaymentList();
                List<KeyValuePair<string, string>> lstMethod = new List<KeyValuePair<string, string>>();
                Array method = Enum.GetValues(typeof(Constant.PaymentMethod));
                foreach (Constant.PaymentMethod val in method)
                {
                    lstMethod.Add(new KeyValuePair<string, string>(val.ToString(), ((int)val).ToString()));
                }
                var jsonSerialiser = new JavaScriptSerializer();
                return Json(new { data = jsonSerialiser.Serialize(lstMethod) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]

        public ActionResult InsertPaymentForInvoice(List<DisplayInvoicePaymentsViewModel> PaymentList)
        {
            try
            {
                if (PaymentList != null)
                {
                    TempData["PaymentsInvoice"] = null;
                    List<InvoicePayment> objtemp = new List<InvoicePayment>();
                    foreach (DisplayInvoicePaymentsViewModel item in PaymentList)
                    {
                        InvoicePayment obj = new InvoicePayment();
                        if (String.IsNullOrEmpty(item.Id))
                        {
                            obj.Id = Guid.Empty;
                        }
                        else
                        {
                            obj.Id = Guid.Parse(item.Id);
                        }
                        if (!string.IsNullOrEmpty(item.InvoiceId))
                            obj.InvoiceId = Guid.Parse(item.InvoiceId);
                        if (!string.IsNullOrEmpty(item.PaymentDate))
                            obj.PaymentDate = Convert.ToDateTime(item.PaymentDate);
                        if (!string.IsNullOrEmpty(item.PaymentAmount))
                            obj.PaymentAmount = Convert.ToDecimal(item.PaymentAmount);
                        if (item.PaymentMethod != null)
                            obj.PaymentMethod = item.PaymentMethod;
                        if (!string.IsNullOrEmpty(item.Reference))
                            obj.Reference = item.Reference;
                        if (!string.IsNullOrEmpty(item.PaymentNote))
                            obj.PaymentNote = item.PaymentNote;
                        obj.CreatedDate = DateTime.Now;
                        obj.CreatedBy = Guid.Parse(base.GetUserId);
                        if (item.PaymentDate != null && !String.IsNullOrEmpty(item.PaymentAmount))
                            objtemp.Add(obj);
                    }
                    TempData["PaymentsInvoice"] = objtemp;
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " inserted payment for invoice.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]

        public ActionResult UpdateInvoiceStatus(Guid InvoiceId, String StatusName, string StatusValue)
        {
            try
            {
                var invoice = InvoiceRep.FindBy(i => i.Id == InvoiceId).FirstOrDefault();
                bool isactive;
                if (invoice != null)
                {
                    switch (StatusName.ToString().ToLower())
                    {
                        case "photos":
                            if (!string.IsNullOrEmpty(StatusValue))
                            {
                                isactive = Convert.ToBoolean(StatusValue);
                                invoice.Photos = isactive;
                                InvoiceRep.Edit(invoice);
                                InvoiceRep.Save();


                            }

                            break;
                        case "docs":
                            if (!string.IsNullOrEmpty(StatusValue))
                            {
                                isactive = Convert.ToBoolean(StatusValue);
                                invoice.RequiredDocs = isactive;
                                InvoiceRep.Edit(invoice);
                                InvoiceRep.Save();


                            }
                            break;
                        case "paid":
                            if (!string.IsNullOrEmpty(StatusValue))
                            {
                                isactive = Convert.ToBoolean(StatusValue);
                                invoice.IsPaid = isactive;
                                InvoiceRep.Edit(invoice);
                                InvoiceRep.Save();


                            }
                            break;
                        case "stock":
                            if (!string.IsNullOrEmpty(StatusValue))
                            {
                                isactive = Convert.ToBoolean(StatusValue);
                                invoice.Stock = isactive;
                                InvoiceRep.Edit(invoice);
                                InvoiceRep.Save();


                            }
                            break;
                        case "material":
                            if (!string.IsNullOrEmpty(StatusValue))
                            {
                                isactive = Convert.ToBoolean(StatusValue);
                                invoice.Material = isactive;
                                InvoiceRep.Edit(invoice);
                                InvoiceRep.Save();
                            }
                            break;
                        case "approved":
                            if (!string.IsNullOrEmpty(StatusValue))
                            {
                                isactive = Convert.ToBoolean(StatusValue);
                                invoice.IsApproved = isactive;
                                invoice.ApprovedBy = Guid.Parse(base.GetUserId);
                                invoice.SentStatus = 1;
                                InvoiceRep.Edit(invoice);
                                InvoiceRep.Save();
                            }
                            break;
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpGet]
        public ActionResult GetQuickViewInvoiceData(Guid InvoiceId)
        {
            try
            {
                var JobId = InvoiceRep.FindBy(m => m.Id == InvoiceId).Select(i => i.EmployeeJobId).FirstOrDefault();
                var CustomerGeneralinfoID = JobRepository.FindBy(m => m.Id == JobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                var QuickData = InvoiceRep.GetInvoiceQuickViewData(InvoiceId).FirstOrDefault();

                //check if photos for the job exist or not
                var isJobphotoExist = EmployeejobDocument.FindBy(i => i.JobId == JobId).Where(i => i.DocType.ToLower() == "image" && (i.IsDelete == false || i.IsDelete == null)).ToList();
                var isJobDocumentExist = EmployeejobDocument.FindBy(i => i.JobId == JobId).Where(i => i.DocType.ToLower() != "image" && (i.IsDelete == false || i.IsDelete == null)).ToList();
                var jobStock = JobStock.FindBy(m => m.JobId == JobId && (m.IsDelete == false || m.IsDelete == null)).FirstOrDefault();
                if (isJobphotoExist.Count > 0)
                {
                    QuickData.Photos = true;
                }
                if (isJobDocumentExist.Count > 0)
                {
                    QuickData.RequiredDocs = true;
                }
                if (QuickData.Due <= 0)
                {
                    QuickData.IsPaid = true;
                }
                if (jobStock != null)
                {
                    QuickData.Stock = true;
                }
                Constant.JobType enumDisplaytype = (Constant.JobType)QuickData.JobType;
                if (QuickData.SentStatus == null)
                {
                    QuickData.SentStatus = 2;
                }
                Constant.InvoiceSentStatus Invsentsatus = (Constant.InvoiceSentStatus)QuickData.SentStatus;

                if (QuickData.IsApproved == null)
                {
                    QuickData.IsApproved = false;
                }
                QuickData.DisplayType = enumDisplaytype.ToString();
                if (QuickData.CustomerNotes == null)
                {
                    QuickData.CustomerNotes = "Not Available";
                }
                // var CustomerGeneralinfo = InvoiceRep.FindBy(i => i.Id == InvoiceId).FirstOrDefault();
                string Billingaddress = "";

                if (CustomerGeneralinfoID != null)
                {

                    Guid customerid = Guid.Parse(CustomerGeneralinfoID.ToString());
                    var customerdefaultbillingaddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerid && i.IsDefault == true).FirstOrDefault();
                    if (customerdefaultbillingaddress == null)
                    {
                        customerdefaultbillingaddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerid).OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                        if (customerdefaultbillingaddress != null)
                        {
                            QuickData.BillingNotes = customerdefaultbillingaddress.Spare1;
                        }
                        else
                        {
                            QuickData.BillingNotes = "Not Available";
                        }
                    }
                    else
                    {
                        QuickData.BillingNotes = customerdefaultbillingaddress.Spare1;
                    }

                    if (customerdefaultbillingaddress != null)
                    {
                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.FirstName))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.FirstName + " ";
                        }

                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.LastName))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.LastName + ",";
                        }
                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.PhoneNo1))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.PhoneNo1 + ",";
                        }
                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.StreetNo))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.StreetNo + ",";
                        }
                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.StreetName))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.StreetName + ",";
                        }
                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.Suburb))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.Suburb + ",";
                        }
                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.State))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.State + ",";
                        }

                        if (!string.IsNullOrEmpty(customerdefaultbillingaddress.PostalCode))
                        {
                            Billingaddress = Billingaddress + customerdefaultbillingaddress.PostalCode;
                        }

                    }
                }


                if (String.IsNullOrEmpty(Billingaddress))
                {
                    QuickData.BillingContact = "Not Available";
                }
                else
                {
                    QuickData.BillingContact = Billingaddress;

                }

                if (String.IsNullOrEmpty(QuickData.BillingNotes))
                {
                    QuickData.BillingNotes = "Not Available";
                }
                if (QuickData.OTRWNotes == null)
                {
                    QuickData.OTRWNotes = "Not Available";
                }
                if (QuickData.OperationNotes == null)
                {
                    QuickData.OperationNotes = "Not Available";
                }
                if (QuickData.JobNotes == null)
                {
                    QuickData.JobNotes = "Not Available";
                }
                if (QuickData.InvoicePrice == null)
                {
                    QuickData.InvoicePrice = 0;
                }
                if (QuickData.Paid == null)
                {
                    QuickData.Paid = 0;
                }
                if (QuickData.ApprovedBy == null)
                {
                    QuickData.ApprovedBy = "Not Available";
                }

                if (string.IsNullOrEmpty(QuickData.ApprovedByName))
                {
                    QuickData.ApprovedByName = "Not Available";
                }
                if (QuickData.TimeTaken == null)
                {
                    QuickData.TimeTaken = "Not Available";
                }
                return Json(QuickData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PartialViewResult SendEmailTemppartial()
        {
            try
            {
                CustomerEmailSendViewModel model = new CustomerEmailSendViewModel();
                List<SelectListItem> Billinglist = new List<SelectListItem>();
                string readFile = "";
                string myString = "";
                //Get Jobid from Invoice
                Guid? Id = Guid.Parse(Request.RequestContext.RouteData.Values["Id"].ToString());
                var invoices = InvoiceRep.FindBy(i => i.Id == Id).FirstOrDefault();
                var jobid = invoices.EmployeeJobId;
                if (jobid != null)
                {
                    var customergeneralInfoId = JobRepository.FindBy(i => i.Id == jobid).FirstOrDefault().CustomerGeneralInfoId;
                    var CustomerBillings = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customergeneralInfoId).ToList();
                    Billinglist = CustomerBillings.Select(m => new SelectListItem()
                    {
                        Text = !string.IsNullOrEmpty(m.EmailId) ? m.EmailId : "",
                        Value = m.CustomerGeneralInfoId.ToString()
                    }).Where(m => m.Text != String.Empty && m.Text != null).Take(20).OrderBy(m => m.Text).ToList();
                }
                //end

                StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/InvoiceDomesticClean.html"));
                readFile = reader.ReadToEnd();
                myString = readFile;
                model.TemplateSelected = myString;
                model.BillingContactList = Billinglist;
                return PartialView("~/Views/Invoice/SendEmailTemppartial.cshtml", model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult GetTemplate(string Templateid)
        {
            try
            {
                string temlatedata = "";
                if (!string.IsNullOrEmpty(Templateid))
                {
                    EmailSendViewModel model = new EmailSendViewModel();
                    temlatedata = GetSelectedTemplateData(Convert.ToInt32(Templateid));
                    return Json(new { invoicestatus = temlatedata }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { invoicestatus = temlatedata }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult _JobReminderCreate()
        {
            return PartialView();
        }
        public PartialViewResult _InvoiceReminderCreate(Guid JobId, string CustomerReminderId, string PageNum, string SiteName)
        {
            try
            {
                CustomerReminderVM model = new CustomerReminderVM();
                string strJobId = Convert.ToString(JobId);
                string strJobIds = string.Join("','", strJobId); // separating each element by "','"
                strJobIds = strJobIds.Insert(0, "'"); // putting "'" at 0 index
                strJobIds = strJobIds.Insert(strJobIds.Count(), "'"); // putting "'" at last index

                var Jobs = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                model.DisplayJobNo = Jobs.JobNo.ToString();
                model.JobId = JobId;
                model.SiteName = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == Jobs.SiteId).Select(m => m.SiteFileName).FirstOrDefault();

                if (!string.IsNullOrEmpty(CustomerReminderId))
                {
                    Guid contactLogId = Guid.Parse(CustomerReminderId);
                    var customercontactLogInfo = CustomercontactLogRepo.FindBy(m => m.CustomerContactId == contactLogId).
                                                Select(m => new { m.Note, m.CustomerId }).FirstOrDefault();



                    model.TemplateMessageId = GetCustomerJobTemplate(customercontactLogInfo.Note);
                    model.CustomerId = customercontactLogInfo.CustomerId;

                    var scheduleReminder = ScheduleReminderRepo.FindBy(m => m.CustomerContactLogId == contactLogId).Select(m => new
                    {
                        m.ScheduleDate,
                        m.EmailTemplate,
                        m.PhoneTemplate,
                        m.Schedule,
                        m.FromEmail
                    }).FirstOrDefault();

                    if (scheduleReminder != null)
                    {
                        model.FromEmail = (Constant.FromEmail)scheduleReminder.FromEmail;
                        model.Schedule = scheduleReminder.Schedule;
                        model.ReminderDate = scheduleReminder.ScheduleDate;
                        if (string.IsNullOrEmpty(scheduleReminder.EmailTemplate) && string.IsNullOrEmpty(scheduleReminder.PhoneTemplate))
                        {
                            model.Note = string.Empty;
                        }
                        else if (string.IsNullOrEmpty(scheduleReminder.EmailTemplate))
                        {
                            model.Note = scheduleReminder.PhoneTemplate;
                            model.Note = model.Note.Replace("<p>", System.Environment.NewLine).
                                     Replace("</p>", System.Environment.NewLine);
                        }
                        else if (string.IsNullOrEmpty(scheduleReminder.PhoneTemplate))
                        {
                            model.Note = scheduleReminder.EmailTemplate;
                            model.Note = model.Note.Replace("<p>", System.Environment.NewLine).
                                     Replace("</p>", System.Environment.NewLine);
                        }
                    }

                    model.JobId2 = CustomercontactLogRepo.GetJobByContactLog(CustomerReminderId).Select(m =>
                                   new JobDataVM
                                   {
                                       Id = m.Id
                                   }).Select(m => m.Id).ToList();

                    model.ContactList = Customercontacts.GetJobContactList(strJobIds).Select(m => new
                    {
                        m.ContactId,
                        m.EmailId,
                        m.FirstName,
                        m.LastName,
                        m.Phone,
                        m.SiteFileName
                    }).Distinct().Select(m => new SelectListItem
                    {
                        Text = m.FirstName + " " + m.LastName + " (" + m.SiteFileName + ")",
                        Value = m.ContactId.ToString()
                    }).ToList();
                    model.ContactListIds = ContactLogSiteContactsMappingRepo.FindBy(m => m.ContactLogId == contactLogId)
                                           .Select(m => m.ContactId).ToList();
                    model.ReminderId = Guid.Parse(CustomerReminderId);
                    return PartialView(model);
                }
                else
                {
                    Guid CustomerGeneralinfoid = Jobs.CustomerGeneralInfoId;

                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (CustomerGeneralInfo)
                    {
                        CustomerGeneralinfo = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == CustomerGeneralinfoid).FirstOrDefault();
                    }
                    //model = new CustomerReminderVM();
                    model.CustomerGeneralInfoId = CustomerGeneralinfoid;


                    model.ContactList = new List<SelectListItem>();


                    var jobContacts = Customercontacts.GetJobContactList(strJobIds).Select(m => new
                    {
                        m.ContactId,
                        m.EmailId,
                        m.FirstName,
                        m.LastName,
                        m.Phone,
                        m.SiteFileName
                    }).Distinct();
                    model.ContactList = jobContacts.Select(m => new SelectListItem
                    {
                        Text = m.FirstName + " " + m.LastName + " (" + m.SiteFileName + ")",
                        Value = m.ContactId.ToString()
                    }).ToList();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + "  created customer Reminder.");

                    return PartialView(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public async Task<ActionResult> InvoiceSendReminder(DateTime? ReminderDate, Guid ReminderId, string[] JobId, string[] ContactListIds, Nullable<FSMConstant.Constant.CustomerJobTemplateMessage> TempMsgId, string Note, bool HasSMS, bool HasEmail, bool hasSchedule, string fromEmail, string fromEmailVal)
        {

            Customer.ViewModels.CustomerReminderViewModel model = new Customer.ViewModels.CustomerReminderViewModel();

            Guid firstJobId = Guid.Parse(JobId[0]);
            Guid InvoiceId = InvoiceRep.FindBy(m => m.EmployeeJobId == firstJobId && m.IsDelete == false).Select(m => m.Id).FirstOrDefault();

            var CustomerID = Guid.Empty;

            #region update particular contact log and its related data
            if (ReminderId != Guid.Empty)
            {
                Guid jobId;
                var hasJobId = Guid.TryParse(JobId[0], out jobId);

                #region update contact log for a particular job
                // update contact log for a particular job
                var contactLog = CustomercontactLogRepo.FindBy(m => m.CustomerContactId == ReminderId).FirstOrDefault();
                contactLog.LogDate = DateTime.Now;
                contactLog.Note = TempMsgId.Value > 0 ? TempMsgId.GetAttribute<DisplayAttribute>().Name : TempMsgId.Value.ToString();
                contactLog.JobId = JobId.Count() > 0 ? JobId[0] : string.Empty;
                contactLog.ModifiedDate = DateTime.Now;
                contactLog.ModifiedBy = Guid.Parse(base.GetUserId);
                CustomercontactLogRepo.Edit(contactLog);
                CustomercontactLogRepo.Save();
                #endregion

                #region delete ContactLogSiteContactsMapping for a particular contact log
                // delete ContactLogSiteContactsMapping for a particular contact log
                var contactLogSiteContactsMappingList = ContactLogSiteContactsMappingRepo.FindBy(m => m.ContactLogId ==
                                                        contactLog.CustomerContactId).ToList();
                foreach (var contactLogSiteContactsMapping in contactLogSiteContactsMappingList)
                {
                    ContactLogSiteContactsMappingRepo.Delete(contactLogSiteContactsMapping);
                    ContactLogSiteContactsMappingRepo.Save();
                }
                #endregion

                #region delete SchedulerReminder for a particular contact log
                // delete SchedulerReminder for a particular contact log
                var scheduleReminderList = ScheduleReminderRepo.FindBy(m => m.CustomerContactLogId ==
                                           contactLog.CustomerContactId).ToList();
                foreach (var scheduleReminder in scheduleReminderList)
                {
                    ScheduleReminderRepo.Delete(scheduleReminder);
                    ScheduleReminderRepo.Save();
                }
                #endregion

                #region adding schduler reminder and ContactLogSiteContactsMapping for a particular contact
                // adding schduler reminder and ContactLogSiteContactsMapping for a particular contact
                if (ContactListIds.Count() > 0)
                {
                    // find job contacts
                    var jobContacts = Customercontacts.GetJobContactList("'" + jobId + "'");
                    foreach (var contact in jobContacts)
                    {
                        if (Array.IndexOf(ContactListIds, contact.ContactId.ToString()) >= 0)
                        {
                            // saving in ContactLogSiteContactsMapping for each contact
                            ContactLogSiteContactsMapping contactLogSiteContactsMapping = new ContactLogSiteContactsMapping();
                            contactLogSiteContactsMapping.Id = Guid.NewGuid();
                            contactLogSiteContactsMapping.ContactLogId = contactLog.CustomerContactId;
                            contactLogSiteContactsMapping.JobId = jobId;
                            contactLogSiteContactsMapping.ContactId = contact.ContactId;
                            contactLogSiteContactsMapping.FirstName = contact.FirstName;
                            contactLogSiteContactsMapping.LastName = contact.LastName;
                            contactLogSiteContactsMapping.CreatedDate = DateTime.Now;
                            contactLogSiteContactsMapping.CreatedBy = Guid.Parse(base.GetUserId);
                            ContactLogSiteContactsMappingRepo.Add(contactLogSiteContactsMapping);
                            ContactLogSiteContactsMappingRepo.Save();

                            // saving in ScheduleReminder for each contact
                            ScheduleReminder scheduleReminder = new ScheduleReminder();
                            scheduleReminder.Id = Guid.NewGuid();
                            scheduleReminder.CustomerContactLogId = contactLog.CustomerContactId;
                            scheduleReminder.Schedule = hasSchedule;
                            scheduleReminder.FromEmail = Convert.ToInt32(fromEmailVal);
                            scheduleReminder.ScheduleDate = ReminderDate;
                            scheduleReminder.CreatedDate = DateTime.Now;
                            scheduleReminder.CreatedBy = Guid.Parse(base.GetUserId);
                            var jobs = JobRepository.FindBy(i => i.Id == jobId).FirstOrDefault();
                            string msgBody = Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);
                            if (jobs != null)
                            {
                                if (jobs.DateBooked.HasValue)
                                {

                                    if (jobs.DateBooked >= DateTime.Now.Date)
                                    {
                                        if (jobs.DateBooked.HasValue)
                                            msgBody = msgBody.Replace("{{DateBooked}}", jobs.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                        else
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                        string startDate = (JobAssignMapping.FindBy(m => m.JobId == jobId && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= jobs.DateBooked).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        //string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == jobId && m.IsDelete == false).OrderBy(m => m.StartTime).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        if (!string.IsNullOrEmpty(startDate))
                                        {
                                            DateTime getDateTime = Convert.ToDateTime(startDate);
                                            string getDate = getDateTime.ToString("dd/MM/yyyy");
                                            DateTime endDate = DateTime.Now;
                                            if (getDateTime != null)
                                            {
                                                endDate = getDateTime.AddMinutes(+30);
                                            }
                                            TimeSpan startTime = new TimeSpan(getDateTime.Hour, getDateTime.Minute, getDateTime.Second);
                                            TimeSpan endTime = new TimeSpan(endDate.Hour, endDate.Minute, endDate.Second);
                                            msgBody = msgBody.Replace("{{DateSend}}", getDate);
                                            msgBody = msgBody.Replace("{{StartEndTime}}", startTime + " - " + endTime);
                                        }

                                        if (HasSMS == true || (HasSMS == false && HasEmail == false))
                                        {
                                            // replacing img tag with alt text
                                            var pattern = @"<img.*?alt='(.*?)'[^\>]*>";
                                            var replacePattern = @"$1";
                                            var phoneTemplate = Regex.Replace(msgBody, pattern, replacePattern, RegexOptions.IgnoreCase);
                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                scheduleReminder.Phone = contact.Phone;
                                                scheduleReminder.PhoneTemplate = phoneTemplate;
                                            }

                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                int errorCounter = Regex.Matches(contact.Phone, @"[a-zA-Z]").Count;
                                                if (errorCounter == 0)
                                                {
                                                    contact.Phone = "+61" + contact.Phone;
                                                    string[] to = { contact.Phone };
                                                    string sendFrom = "+61414363865";
                                                    TransmitSmsWrapper manager = new TransmitSmsWrapper("23dac442668a809eeaa7d9aaad5f91c7", "clientapisecret", "https://api.transmitsms.com");
                                                    var response = manager.SendSms("" + msgBody + "", to, sendFrom, null, null, "", "", 0);
                                                }
                                            }

                                        }
                                        if (HasEmail == true)
                                        {
                                            msgBody = msgBody.Replace("\n", "<p>");

                                            if (!string.IsNullOrEmpty(contact.EmailId))
                                            {
                                                scheduleReminder.Email = contact.EmailId;
                                                scheduleReminder.EmailTemplate = msgBody;
                                            }

                                            if (!string.IsNullOrEmpty(contact.EmailId))
                                            {
                                                // sending mail
                                                MailMessage mmm = new MailMessage();
                                                mmm.Subject = "Reminding You";
                                                mmm.IsBodyHtml = true;
                                                mmm.To.Add(contact.EmailId);
                                                mmm.From = new MailAddress(fromEmail, "Sydney Roof and Gutter");
                                                //mmm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                                                string Body = Convert.ToString(msgBody);
                                                mmm.Body = Body;
                                                SmtpClient Smtp = new SmtpClient();
                                                Smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                                Smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                                Smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                                Smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                                await Smtp.SendMailAsync(mmm);
                                            }
                                        }
                                    }
                                }
                            }
                            ScheduleReminderRepo.Add(scheduleReminder);
                            ScheduleReminderRepo.Save();
                        }
                    }
                }
                #endregion

                CustomerID = contactLog.CustomerGeneralInfoId;
            }
            #endregion

            #region add contact log and its related data
            else
            {
                foreach (var job in JobId)
                {
                    Guid Job_id = Guid.Parse(job);
                    CustomerID = JobRepository.FindBy(m => m.Id == Job_id).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                    var customerInfo = JobRepository.FindBy(m => m.Id == Job_id).Select(m => new
                    {
                        m.CustomerGeneralInfo.CustomerGeneralInfoId,
                        m.CustomerGeneralInfo.CTId
                    }).FirstOrDefault();

                    var JobDetail = JobRepository.FindBy(m => m.Id == Job_id).FirstOrDefault();
                    string SiteAddress = CustomerSiteDetailRepo.GetSiteAddress(JobDetail.SiteId);

                    #region save contactlog for each job
                    // save contactlog for each job
                    CustomerContactLog customerContactLog = new CustomerContactLog();
                    customerContactLog.CustomerContactId = Guid.NewGuid();
                    customerContactLog.CustomerGeneralInfoId = customerInfo.CustomerGeneralInfoId;
                    customerContactLog.CustomerId = customerInfo.CTId.ToString();
                    customerContactLog.JobId = job;
                    customerContactLog.LogDate = DateTime.Now;
                    customerContactLog.Note = TempMsgId.Value > 0 ? TempMsgId.GetAttribute<DisplayAttribute>().Name : TempMsgId.Value.ToString();
                    customerContactLog.IsDelete = false;
                    customerContactLog.IsReminder = true;
                    customerContactLog.IsScheduler = true;
                    customerContactLog.CreatedDate = DateTime.Now;
                    customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);
                    CustomercontactLogRepo.Add(customerContactLog);
                    CustomercontactLogRepo.Save();
                    #endregion
                    // find job contacts
                    var jobContacts = Customercontacts.GetJobContactList("'" + JobDetail.Id + "'");
                    #region adding ContactLogSiteContactsMapping and scheduler reminder data for each contact 
                    foreach (var contact in jobContacts)
                    {
                        if (Array.IndexOf(ContactListIds, contact.ContactId.ToString()) >= 0)
                        {
                            // saving in ContactLogSiteContactsMapping for each contact
                            ContactLogSiteContactsMapping contactLogSiteContactsMapping = new ContactLogSiteContactsMapping();
                            contactLogSiteContactsMapping.Id = Guid.NewGuid();
                            contactLogSiteContactsMapping.ContactLogId = customerContactLog.CustomerContactId;
                            contactLogSiteContactsMapping.JobId = Job_id;
                            contactLogSiteContactsMapping.ContactId = contact.ContactId;
                            contactLogSiteContactsMapping.FirstName = contact.FirstName;
                            contactLogSiteContactsMapping.LastName = contact.LastName;
                            contactLogSiteContactsMapping.CreatedDate = DateTime.Now;
                            contactLogSiteContactsMapping.CreatedBy = Guid.Parse(base.GetUserId);
                            ContactLogSiteContactsMappingRepo.Add(contactLogSiteContactsMapping);
                            ContactLogSiteContactsMappingRepo.Save();

                            // saving in ScheduleReminder for each contact
                            ScheduleReminder scheduleReminder = new ScheduleReminder();
                            scheduleReminder.Id = Guid.NewGuid();
                            scheduleReminder.CustomerContactLogId = customerContactLog.CustomerContactId;
                            scheduleReminder.Schedule = hasSchedule;
                            scheduleReminder.FromEmail = Convert.ToInt32(fromEmailVal);
                            scheduleReminder.ScheduleDate = ReminderDate;
                            scheduleReminder.CreatedDate = DateTime.Now;
                            scheduleReminder.CreatedBy = Guid.Parse(base.GetUserId);
                            var jobs = JobRepository.FindBy(i => i.Id == Job_id).FirstOrDefault();
                            string msgBody = Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);

                            if (jobs != null)
                            {
                                if (jobs.DateBooked.HasValue)
                                {

                                    if (jobs.DateBooked >= DateTime.Now.Date)
                                    {
                                        if (jobs.DateBooked.HasValue)
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", jobs.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        else
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                        string startDate = (JobAssignMapping.FindBy(m => m.JobId == Job_id && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= jobs.DateBooked).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        //  string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false).OrderBy(m => m.StartTime).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        if (!string.IsNullOrEmpty(startDate))
                                        {
                                            DateTime getDateTime = Convert.ToDateTime(startDate);
                                            string getDate = getDateTime.ToString("dd/MM/yyyy");
                                            DateTime endDate = DateTime.Now;
                                            if (getDateTime != null)
                                            {
                                                endDate = getDateTime.AddMinutes(+30);
                                            }
                                            TimeSpan startTime = new TimeSpan(getDateTime.Hour, getDateTime.Minute, getDateTime.Second);
                                            TimeSpan endTime = new TimeSpan(endDate.Hour, endDate.Minute, endDate.Second);
                                            msgBody = msgBody.Replace("{{DateSend}}", getDate);
                                            msgBody = msgBody.Replace("{{StartEndTime}}", startTime + " - " + endTime);
                                        }

                                        if (HasSMS == true || (HasSMS == false && HasEmail == false))
                                        {
                                            // replacing img tag with alt text
                                            var pattern = @"<img.*?alt='(.*?)'[^\>]*>";
                                            var replacePattern = @"$1";
                                            var phoneTemplate = Regex.Replace(msgBody, pattern, replacePattern, RegexOptions.IgnoreCase);
                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                scheduleReminder.Phone = contact.Phone;
                                                scheduleReminder.PhoneTemplate = phoneTemplate;
                                            }

                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                int errorCounter = Regex.Matches(contact.Phone, @"[a-zA-Z]").Count;
                                                if (errorCounter == 0)
                                                {
                                                    contact.Phone = "+61" + contact.Phone;
                                                    string[] to = { contact.Phone };
                                                    string sendFrom = "+61414363865";
                                                    TransmitSmsWrapper manager = new TransmitSmsWrapper("23dac442668a809eeaa7d9aaad5f91c7", "clientapisecret", "https://api.transmitsms.com");
                                                    var response = manager.SendSms("" + msgBody + "", to, sendFrom, null, null, "", "", 0);
                                                }
                                            }
                                        }
                                        if (HasEmail == true)
                                        {
                                            msgBody = msgBody.Replace("\n", "<p>");

                                            if (!string.IsNullOrEmpty(contact.EmailId))
                                            {
                                                scheduleReminder.Email = contact.EmailId;
                                                scheduleReminder.EmailTemplate = msgBody;
                                            }

                                            if (!string.IsNullOrEmpty(contact.EmailId))
                                            {
                                                // sending mail
                                                MailMessage mmm = new MailMessage();
                                                mmm.Subject = "Reminding You";
                                                mmm.IsBodyHtml = true;
                                                mmm.To.Add(contact.EmailId);
                                                mmm.From = new MailAddress(fromEmail, "Sydney Roof and Gutter");
                                                //mmm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                                                string Body = Convert.ToString(msgBody);
                                                mmm.Body = Body;
                                                SmtpClient Smtp = new SmtpClient();
                                                Smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                                Smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                                Smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                                Smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                                await Smtp.SendMailAsync(mmm);
                                            }
                                        }
                                    }
                                }
                            }
                            ScheduleReminderRepo.Add(scheduleReminder);
                            ScheduleReminderRepo.Save();
                        }
                    }
                    #endregion
                }
            }
            #endregion
            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + "  send Reminder to a customer by email.");
            return Json(new { success = InvoiceId }, JsonRequestBehavior.AllowGet);
        }




        private Constant.CustomerJobTemplateMessage GetCustomerJobTemplate(string customercontactLogInfo)
        {
            Constant.CustomerJobTemplateMessage TemplateMessageId = 0;
            switch (customercontactLogInfo)
            {
                case "Confirmation Appointment Strata Realestate":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ConfirmationAppointmentStrataRealestate;
                    break;
                case "Confirmation Appointment Domestic Customer":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ConfirmationAppointmentDomesticCustomer;
                    break;
                case "Due to Rain Job is Postponed to tomorrow":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.DueToRainJobPostponed;
                    break;
                case "Unavailable OTRW Due To BadHealth":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.UnavailableOTRWDueToBadHealth;
                    break;
                case "Price Increase for Gutter Clean Contract":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ContractedGutterCleanPriceIncrease;
                    break;
                case "SRAS Needed for Gutter Clean Contract":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ContractedGutterCleanSRASNeeded;
                    break;
                case "Reminder":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.CustomerReminder;
                    break;
                case "Rain":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ReminderForRain;
                    break;
                case "Sick":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ReminderForSick;
                    break;
            }
            return TemplateMessageId;
        }

        [HttpPost]
        public ActionResult GetCustomerEmailId(string Customerid)
        {
            List<selctemail> li = new List<selctemail>();
            var customerinfoid = Guid.Parse(Customerid);
            var CustomerBillings = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerinfoid && (m.IsDelete == false || m.IsDelete == null)).ToList();
            if (CustomerBillings.Count > 0)
            {
                foreach (var contact in CustomerBillings)
                {
                    if (!string.IsNullOrEmpty(contact.EmailId))
                    {
                        selctemail mail = new selctemail();
                        mail.Email = contact.EmailId;
                        mail.id = contact.CustomerGeneralInfoId.ToString();
                        li.Add(mail);
                    }
                }
            }
            var jsonSerialiser = new JavaScriptSerializer();
            //  var json = jsonSerialiser.Serialize(li);
            return Json(li, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult Syncmyob(string Id, string From)
        {
            var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlMyob"];
            if (!string.IsNullOrEmpty(Id))
            {
                if (Guid.Parse(Id) != Guid.Empty)
                {
                    SyncInvoiceId = Guid.Parse(Id);
                    Session["invoiceid"] = SyncInvoiceId;
                }
            }
            _Apiode = !string.IsNullOrEmpty(Request.QueryString["code"]) ? Request.QueryString["code"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(_Apiode))
            {

            }
            else
            {
                string res = FSMMyob1(); //Live Sydney roof and gutter file
            }
            return RedirectToAction("SaveInvoiceInfo", new { id = Guid.Parse(Session["invoiceid"].ToString()) });


        }


        [HttpGet]
        public ActionResult SynctoMyobInvoiceList()
        {
            var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlInvoiceList"];
            _Apiode = !string.IsNullOrEmpty(Request.QueryString["code"]) ? Request.QueryString["code"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(_Apiode))
            {
                //return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var t = new Thread(SyncAllInvoices);
                t.SetApartmentState(ApartmentState.STA);
                if (t.IsAlive)
                {
                    // Thread has not finished
                }
                else
                {
                    t.Start();
                    // Finished
                }

            }
            return RedirectToAction("InvoiceList");
        }


        [HttpGet]
        public ActionResult Syncmyoburchase(string Id, string From)
        {
            Session["Ispurchase"] = true;
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        #region purchase order
        public string SyncPurchaseordermyob()
        {
            try
            {
                #region GetCompanyFile
                CustomerGeneralInfo customerData = new Core.Entities.CustomerGeneralInfo();
                var developerKey = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperKey"]; //"jgnsyvj7brdpw7kb4ftpz3rk";
                var developerSecret = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperSecret"];// "PqbpPYmxr2UcCfScHxrkEYSz";
                var UserName = System.Configuration.ConfigurationManager.AppSettings["CompanyUserName"];
                var Password = System.Configuration.ConfigurationManager.AppSettings["CompanyPassword"];
                var CompanyFilename = System.Configuration.ConfigurationManager.AppSettings["CompanyFileName"];
                var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlMyob"];
                CompanyFileCredentials _CompanyFile = new CompanyFileCredentials(UserName, Password);
                var configuration = new ApiConfiguration(developerKey, developerSecret, RedirectUrl);
                var oauthService = new OAuthService(configuration);
                string code = _Apiode;
                var tokens = oauthService.GetTokens(code);
                tokens.ExpiresIn = 1;
                var mytoken = tokens;
                if (tokens.HasExpired)
                {
                    mytoken = oauthService.RenewTokens(tokens);
                }
                var keystore = new SimpleOAuthKeyService();
                keystore.OAuthResponse = mytoken;
                //Get Company Files
                var cfService = new CompanyFileService(configuration, null, keystore);
                var companyFiles = cfService.GetRange();
                companyFiles = companyFiles.Where(i => i.Name == CompanyFilename).ToArray();
                #endregion

                #region GetITemService (From My Ob)
                var itemservice = new MYOB.AccountRight.SDK.Services.Inventory.ItemService(configuration, null, keystore); // invoice service
                #endregion
                var supService = new SupplierService(configuration, null, keystore);
                // purchase order service
                var poService = new ItemPurchaseOrderService(configuration, null, keystore);
                var taxcodes = new TaxCodeService(configuration, null, keystore);
                var taxcodeservices = taxcodes.GetRange(companyFiles[0], null, _CompanyFile);
                var tax_Gstcodes = taxcodeservices.Items.ToList().Where(i => i.Code == "GST").FirstOrDefault();
                var Purchaseorders = InvoiceRep.GetallPurchaseOrderformyob();
                List<ItemPurchaseOrderLine> purchaseline = new List<ItemPurchaseOrderLine>();
                foreach (var order in Purchaseorders)
                {
                    string jobno = "";
                    string filtername = order.Name.ToString();
                    if (filtername.ToLower().Contains('&'))
                    {
                        filtername = filtername.Replace("&", "'&'");
                    }
                    string pageFilter = String.Format("$filter=DisplayID eq '{0}' or CompanyName eq '{1}' or LastName eq '{2}' ", "USS0" + filtername, filtername, filtername);
                    var supList = supService.GetRange(companyFiles[0], pageFilter, _CompanyFile);
                    if (supList != null)
                    {
                        var itemservicepurchase = new MYOB.AccountRight.SDK.Services.Inventory.ItemService(configuration, null, keystore); // invoice service
                        var itemfilter = String.Format("$filter=Number eq '{0}'", "MATERIALS");//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                        var itemss = itemservice.GetRange(companyFiles[0], itemfilter, _CompanyFile);
                        //Get Total items of the purchaseorder
                        var orderitems = JobPurchaseOrderitem.FindBy(i => i.PurchaseOrderID == order.ID).ToList();
                        if (orderitems.Count > 0)
                        {
                            purchaseline = new List<ItemPurchaseOrderLine>();
                            foreach (var item in orderitems)
                            {
                                ItemPurchaseOrderLine line = new ItemPurchaseOrderLine();
                                line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                {
                                    UID = itemss.Items[0].UID,
                                    URI = itemss.Items[0].URI,
                                    Name = itemss.Items[0].Name,
                                    Number = itemss.Items[0].Number
                                };
                                line.Description = item.PurchaseItem;
                                line.TaxCode = new TaxCodeLink() { Code = tax_Gstcodes.Code, UID = tax_Gstcodes.UID };
                                line.UnitPrice = 0;
                                line.ReceivedQuantity = 1;

                                line.BillQuantity = 1;
                                line.Total = 0;
                                //line.Account = new AccountLink() { UID= UndepositedFunds1.UID, DisplayID= UndepositedFunds1.DisplayID };
                                line.Type = OrderLineType.Transaction;
                                line.Description = order.Description;
                                purchaseline.Add(line);
                                var job = JobRepository.FindBy(i => i.Id == item.PurchaseOrderByJob.JobID).ToList();
                                if (job.Count() > 0)
                                {
                                    jobno = job.FirstOrDefault().JobNo.ToString();
                                }

                                else
                                {
                                    var invoice = InvoiceRep.FindBy(i => i.Id == item.PurchaseOrderByJob.InvoiceId).ToList();
                                    if (invoice.Count() > 0)
                                    {
                                        jobno = invoice.FirstOrDefault().JobId.ToString();
                                    }
                                }
                            }
                        }

                        // save purchase order
                        var servicePO = new ItemPurchaseOrder()
                        {
                            Comment = "Adding purchase order",
                            Created = order.CreatedDate,
                            Date = Convert.ToDateTime(order.CreatedDate).Date,
                            OrderType = OrderLayoutType.Service,
                            IsTaxInclusive = false,
                            JournalMemo = "JobNo#" + jobno,
                            LastModified = order.ModifiedDate,
                            Lines = purchaseline.ToArray(),
                            Number = order.Purchaseorderno.ToString(),
                            ShippingMethod = "By Air",
                            ShipToAddress = "Mohali, Seasia Infotech",
                            Supplier = new SupplierLink()
                            {
                                UID = supList.Items[0].UID
                            },
                            SupplierInvoiceNumber = "",
                            TotalAmount = 0,
                            Status = PurchaseOrderStatus.Open,
                            TotalTax = 0,
                            UID = Guid.NewGuid(),
                            IsReportable = false,


                        };
                        poService.Insert(companyFiles[0], servicePO, _CompanyFile);
                        // get purchase order
                        string pageFilters = String.Format("$filter=Number eq '{0}'", order.Purchaseorderno.ToString());
                        var purchaseOrdersitem = poService.GetRange(companyFiles[0], pageFilters, _CompanyFile);
                    }
                }
                return "1";

            }

            catch (Exception ex)
            {
                return ex.InnerException.ToString();
                throw ex;
            }
        }

        #endregion

        //Function for myob on Live Sydney Gutter clean file
        public string FSMMyob1()
        {
            try
            {
                string invoicessyncedtotal = "";
                #region GetCompanyFile
                if (Session["invoiceid"] != null)
                {
                    SyncInvoiceId = Guid.Parse(Session["invoiceid"].ToString());
                }
                CustomerGeneralInfo customerData = new Core.Entities.CustomerGeneralInfo();
                var developerKey = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperKey"]; //"jgnsyvj7brdpw7kb4ftpz3rk";
                var developerSecret = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperSecret"];// "PqbpPYmxr2UcCfScHxrkEYSz";
                var UserName = System.Configuration.ConfigurationManager.AppSettings["CompanyUserName"];
                var Password = System.Configuration.ConfigurationManager.AppSettings["CompanyPassword"];
                var CompanyFilename = System.Configuration.ConfigurationManager.AppSettings["CompanyFileName"];
                var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlMyob"];
                CompanyFileCredentials _CompanyFile = new CompanyFileCredentials(UserName, Password);
                var configuration = new ApiConfiguration(developerKey, developerSecret, RedirectUrl);
                var oauthService = new OAuthService(configuration);
                string code = _Apiode;
                var tokens = oauthService.GetTokens(code);
                tokens.ExpiresIn = 1;
                var mytoken = tokens;
                if (tokens.HasExpired)
                {
                    mytoken = oauthService.RenewTokens(tokens);
                }
                var keystore = new SimpleOAuthKeyService();
                keystore.OAuthResponse = mytoken;

                //Get Company Files
                var cfService = new CompanyFileService(configuration, null, keystore);
                var companyFiles = cfService.GetRange();

                //GEt Locked date of the myob
                companyFiles = companyFiles.Where(i => i.Name == CompanyFilename).ToArray();
                MYOB.AccountRight.SDK.Services.Company.CompanyPreferencesService companypreference = new MYOB.AccountRight.SDK.Services.Company.CompanyPreferencesService(configuration, null, keystore);
                var companyPreference = companypreference.Get(companyFiles[0], _CompanyFile, null);
                DateTime? lockDatetime = companyPreference.System.LockPeriodPriorTo;
                #endregion

                #region GetITemService (From My Ob)
                var itemservice = new MYOB.AccountRight.SDK.Services.Inventory.ItemService(configuration, null, keystore); // invoice service
                var items = itemservice.GetRange(companyFiles[0], null, _CompanyFile);
                #endregion
                var supService = new SupplierService(configuration, null, keystore);
                #region GetAllInvoices From FSM Which are not synced
                var invoiceRange = InvoiceRep.GetALLInvoices(SyncInvoiceId);//if only one id than parameter otherwise not 
                #endregion
                #region GetCustomerInfo from Invoiceid
                foreach (var Currentinvoice in invoiceRange)
                {
                    SyncInvoiceId = Guid.Parse(Currentinvoice.ID.ToString());
                    FSM.Core.Entities.Invoice invoicedata = InvoiceRep.FindBy(i => i.Id == SyncInvoiceId).FirstOrDefault();
                    if (invoicedata != null)
                    {
                        if (invoicedata.CustomerGeneralInfoId != null)
                        {
                            if (invoicedata.CustomerGeneralInfoId != Guid.Empty)

                            {
                                customerData = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == invoicedata.CustomerGeneralInfoId).FirstOrDefault();

                            }
                            else
                            {

                                var Customer = JobRepository.FindBy(i => i.Id == invoicedata.EmployeeJobId).FirstOrDefault();
                                customerData = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == Customer.CustomerGeneralInfoId).FirstOrDefault();
                            }
                        }
                        else
                        {
                            var Customer = JobRepository.FindBy(i => i.Id == invoicedata.EmployeeJobId).FirstOrDefault();
                            customerData = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == Customer.CustomerGeneralInfoId).FirstOrDefault();
                        }
                    }
                    #endregion
                    var GetCustomerViaInvoice = new InvoiceService(configuration, null, keystore); // invoice service

                    #region GetCustomer Service and Various Codes in company File
                    var custService = new CustomerService(configuration, null, keystore); // customer service
                    string filtername = customerData.CustomerLastName.ToString();
                    if (filtername.ToLower().Contains('&'))
                    {
                        filtername = filtername.Replace("&", "'&'");
                    }
                    string pageFilter = String.Format("$filter=DisplayID eq '{0}' or CompanyName eq '{1}' or LastName eq '{2}' ", "USS0" + customerData.CTId.ToString(), filtername, filtername);
                    var custinfo = custService.GetRange(companyFiles[0], pageFilter, _CompanyFile);
                    // tax code service
                    var tcService = new TaxCodeService(configuration, null, keystore);
                    var tcList = tcService.GetRange(companyFiles[0], null, _CompanyFile);
                    #region GET The Account Linked to the Myob
                    AccountService accnt = new AccountService(configuration, null, keystore);
                    string accountfilter = String.Format("$filter=DisplayID eq '{0}' or DisplayID eq '{1}' ", "1-1180", "1-1111");//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                    var Account_Service = accnt.GetRange(companyFiles[0], accountfilter, _CompanyFile);
                    var UndepositedFunds = Account_Service.Items.ToList().Where(i => i.Number == 1180).FirstOrDefault();
                    var BankAccountFunds = Account_Service.Items.ToList().Where(i => i.Number == 1111).FirstOrDefault();
                    #endregion
                    #region get tax code items
                    var taxcode = new TaxCodeService(configuration, null, keystore);
                    var taxcodeservice = taxcode.GetRange(companyFiles[0], null, _CompanyFile);
                    var tax_Gstcode = taxcodeservice.Items.ToList().Where(i => i.Code == "GST").FirstOrDefault();
                    MYOB.AccountRight.SDK.Contracts.Version2.Contact.Customer cust1 = new MYOB.AccountRight.SDK.Contracts.Version2.Contact.Customer();
                    #endregion
                    #endregion
                    #region GetInvoicePayment from FSM
                    var invoicepayment = InvoicePaymentRepo.FindBy(i => i.InvoiceId == invoicedata.Id).ToList();
                    var InvoicePaidAmount = Convert.ToDecimal(invoicepayment.Sum(i => i.PaymentAmount));
                    DateTime? LastPaymentDate = null;
                    if (invoicepayment.Count > 0)
                    {
                        var lastpayment = invoicepayment.OrderByDescending(i => i.PaymentDate).Select(i => i.PaymentDate).FirstOrDefault();
                        LastPaymentDate = lastpayment.Value;
                    }

                    #endregion

                    #region Customer Add
                    var customerid = Guid.Parse(customerData.CustomerGeneralInfoId.ToString());
                    //Check if customer Exist or not
                    if (custinfo.Items.Count() == 0)
                    {
                        BillingAddressForMyob ob = GetCustomerBillingAddress(SyncInvoiceId.ToString());
                        var CustomerBillingAddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerid).FirstOrDefault();
                        string Email = "";
                        string Phone1 = "";
                        string Phone2 = "";
                        string StreetNo = "";
                        string streetName = "";
                        string postalcode = "";
                        string Suburb = "";
                        string state = "";
                        string firstName = "";
                        String LastName = "";
                        if (CustomerBillingAddress != null)
                        {
                            firstName = !string.IsNullOrEmpty(CustomerBillingAddress.FirstName) ? CustomerBillingAddress.FirstName : "";
                            LastName = !string.IsNullOrEmpty(CustomerBillingAddress.LastName) ? CustomerBillingAddress.LastName : "";
                            StreetNo = !string.IsNullOrEmpty(CustomerBillingAddress.StreetNo) ? CustomerBillingAddress.StreetName : "";
                            streetName = !string.IsNullOrEmpty(CustomerBillingAddress.StreetName) ? CustomerBillingAddress.StreetName : "";
                            postalcode = !string.IsNullOrEmpty(CustomerBillingAddress.PostalCode) ? CustomerBillingAddress.PostalCode : "";
                            state = !string.IsNullOrEmpty(CustomerBillingAddress.State) ? CustomerBillingAddress.State : "";
                            Suburb = !string.IsNullOrEmpty(CustomerBillingAddress.Suburb) ? CustomerBillingAddress.Suburb : "";
                            Email = CustomerBillingAddress.EmailId;
                            Phone1 = CustomerBillingAddress.PhoneNo1;
                            Phone2 = CustomerBillingAddress.PhoneNo2;
                        }
                        if (customerData.CustomerType == Convert.ToInt32(CustomerType.Domestic))
                        {
                            cust1.Addresses = new List<Address>() { new Address() { ContactName = "", Street = firstName + " " + LastName + " ," + StreetNo + " " + streetName + " " + Suburb, Country = "Australia", Email = Email, Fax = "", Phone1 = Phone1, Phone2 = Phone2, State = state, PostCode = postalcode } };
                        }
                        else
                        {
                            cust1.Addresses = new List<Address>() { new Address() { ContactName = "", Street = customerData.TradingName + " \n" + StreetNo + " " + streetName + " " + Suburb, Country = "Australia", Email = Email, Fax = "", Phone1 = Phone1, Phone2 = Phone2, State = state, PostCode = postalcode } };
                        }
                        cust1.CompanyName = !string.IsNullOrEmpty(customerData.CustomerLastName) ? customerData.CustomerLastName : "";//"CompanyName";
                        if (cust1.CompanyName.Length > 50)
                        {
                            cust1.CompanyName = cust1.CompanyName.Substring(0, 49);
                        }
                        cust1.CurrentBalance = Convert.ToDecimal(invoicedata.Due);
                        cust1.IsIndividual = false;
                        cust1.DisplayID = "USS0" + customerData.CTId.ToString(); //"USS0+CTId.Tostring()";//
                        cust1.IsActive = true;
                        cust1.Notes = !string.IsNullOrEmpty(customerData.CustomerNotes) ? customerData.CustomerNotes : "";
                        if (!string.IsNullOrEmpty(customerData.CustomerNotes))
                        {
                            if (cust1.Notes.Length > 250)
                            {
                                cust1.Notes = cust1.Notes.Substring(0, 249);
                            }
                        }
                        cust1.UID = Guid.Parse(customerData.CustomerGeneralInfoId.ToString());
                        CustomerSellingDetails sellingdetail = new CustomerSellingDetails();
                        sellingdetail.Credit = new CustomerCredit
                        {
                            Limit = 0,
                            Available = 0,
                            PastDue = 0,
                            OnHold = false
                        };
                        cust1.Type = ContactType.Customer;
                        sellingdetail.PrintedForm = "Pre-Printed Invoice";//"Pre-Printed Invoice";
                        sellingdetail.ItemPriceLevel = "Level A";//"";
                        sellingdetail.IncomeAccount = null;
                        sellingdetail.TaxCode = new TaxCodeLink { Code = tax_Gstcode.Code, UID = tax_Gstcode.UID, URI = tax_Gstcode.URI };
                        sellingdetail.FreightTaxCode = new TaxCodeLink { Code = tax_Gstcode.Code, UID = tax_Gstcode.UID, URI = tax_Gstcode.URI };
                        sellingdetail.ReceiptMemo = "Thank you";//"Thank you";
                        sellingdetail.SalesPerson = null;
                        sellingdetail.SaleComment = "We appreciate your business.";//"We appreciate your business.";
                        sellingdetail.ShippingMethod = "Freight";//"Freight";
                        sellingdetail.HourlyBillingRate = 0;
                        sellingdetail.UseCustomerTaxCode = false;
                        cust1.SellingDetails = sellingdetail;
                        cust1.PaymentDetails = null;
                        var myCredentials = _CompanyFile;
                        custService.Insert(companyFiles.FirstOrDefault(), cust1, myCredentials, ErrorLevel.WarningsAsErrors);
                    }
                    else
                    {
                        //Update Customer
                        var CustomerBillingAddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerid).FirstOrDefault();
                        cust1 = new MYOB.AccountRight.SDK.Contracts.Version2.Contact.Customer();
                        cust1 = custinfo.Items.ToList().FirstOrDefault();
                        var myCredentials = _CompanyFile;

                    }

                    #endregion

                    #region insert invoice
                    custinfo = custService.GetRange(companyFiles[0], pageFilter, _CompanyFile);
                    var invService = new ItemInvoiceService(configuration, null, keystore); // invoice service
                    var invoices = invService.GetRange(companyFiles[0], "$filter = Number eq '" + invoicedata.InvoiceNo.ToString() + "'", _CompanyFile);
                    var isInvoicexist = invoices.Items.Where(i => i.Number == invoicedata.InvoiceNo.ToString()).FirstOrDefault();
                    System.Collections.Generic.List<ItemInvoiceLine> Lines = new List<ItemInvoiceLine>();
                    #region GetInvoiceDetail
                    var invoicejclitemmapping = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == invoicedata.Id).OrderBy(i => i.OrderNo).ToList();
                    if (invoicejclitemmapping.Count() == 0)
                    {
                        invoicejclitemmapping = invoiceJCLItemRepo.FindBy(i => i.JobID == invoicedata.EmployeeJobId).OrderBy(i => i.OrderNo).ToList();
                    }

                    if (isInvoicexist == null && invoicedata.InvoiceDate >= Convert.ToDateTime(lockDatetime))
                    {
                        decimal subtotal = Convert.ToDecimal(invoicejclitemmapping.Sum(i => i.TotalPrice));
                        decimal TotalTax = Convert.ToDecimal((subtotal * 10) / 100);
                        foreach (var item in invoicejclitemmapping)
                        {
                            ItemInvoiceLine line = new ItemInvoiceLine();
                            line.Type = InvoiceLineType.Transaction;
                            line.ShipQuantity = item.Quantity;
                            line.UnitPrice = item.Price;
                            line.Total = item.TotalPrice;
                            if (line.UnitPrice < 0)
                            {
                                line.UnitPrice = -1 * (item.Price);
                                line.ShipQuantity = -1 * (line.ShipQuantity);
                                line.Total = item.TotalPrice;
                            }

                            if (!string.IsNullOrEmpty(item.Description))
                            {
                                if (item.Description.Length <= 250)
                                {
                                    line.Description = item.Description;
                                }
                                else
                                {
                                    line.Description = item.Description.Substring(0, 249);
                                }
                            }
                            else
                            {
                                line.Description = String.Empty;
                            }
                            var Saleitem = invoiceJCLItemRepo.FindBy(i => i.JCLItemID == item.JCLItemID && i.InvoiceId == SyncInvoiceId).FirstOrDefault();
                            if (Saleitem == null)
                            {
                                var jobid = invoicejclitemmapping.FirstOrDefault().JobID;
                                Saleitem = invoiceJCLItemRepo.FindBy(i => i.JCLItemID == item.JCLItemID && i.JobID == jobid).FirstOrDefault();
                            }
                            var itemname = JCLRepo.FindBy(i => i.JCLId == Saleitem.JCLItemID).FirstOrDefault();
                            var saleitem = itemname.ItemName;
                            if (saleitem == "00 - Gutter Cleaning - Repeat")
                            {
                                saleitem = "00 CLEAN - REPEAT CUSTOMER";
                            }
                            if (saleitem.Trim() == "Gutter Cleaning")
                            {
                                saleitem = "Remove all debris";
                            }
                            if (saleitem == "00 - Gutter Cleaning - Initial")
                            {
                                saleitem = "Remove all debris";
                            }
                            if (saleitem == "00 - Roof Plumbing Blank")
                            {
                                saleitem = "0 BLANK ROOF PLUMBING";
                            }
                            if (saleitem == "SRAS - WITH HATCH")
                            {
                                saleitem = "SRAS HATCH";
                            }
                            if (saleitem.Trim() == "PGG - HDPE")
                            {
                                saleitem = "PGG PLASTIC";
                            }
                            if (saleitem.Trim() == "PGG")
                            {
                                saleitem = "PGG PLASTIC";
                            }
                            if (saleitem == "PVG METAL VALLEY GUARD")
                            {
                                saleitem = "VALLEY PGG METAL";
                            }
                            if (saleitem == "00 - Gutter Cleaning - Contracted")
                            {
                                saleitem = "00 CLEAN - CONTRACTED";
                            }
                            if (saleitem == "RECERT FREE")
                            {
                                saleitem = "RE-CERT FREE";
                            }
                            if (saleitem == "00 - 6m Option Gutter Clean Offer")
                            {
                                saleitem = "6 Monthly";
                            }
                            if (saleitem == "00 - Roof Tiling Blank")
                            {
                                saleitem = "0 BLANK ITEM TILING";
                            }
                            if (saleitem == "REP VALLEY (TILED ROOF)")
                            {
                                saleitem = "REP VALLEY TILE";
                            }

                            if (saleitem == "REP VALLEY (METAL ROOF)")
                            {
                                saleitem = "REP VALLEYS METAL";
                            }

                            if(saleitem== "PBGG - BOX GUTTER GUARD")
                            {
                                saleitem = "PBG - PREMIUM BOX GUTTER GUARD";
                            }
                            var description = Saleitem.Description;
                            string filter = "";
                            if (itemname.Category == 6)
                            {
                                filter = String.Format("$filter=Number eq '{0}' or Name eq '{1}' ", "0 BLANK ITEM MISCELLANEOUS", "0 BLANK ITEM MISCELLANEOUS");//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                            }
                            else
                            {
                                filter = String.Format("$filter=Number eq '{0}' or Name eq '{1}' ", saleitem.ToString(), saleitem.ToString());//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                            }
                            var myobsaleitem = itemservice.GetRange(companyFiles[0], filter, _CompanyFile);
                            if (myobsaleitem.Items.Count() == 0)
                            {
                                string desc = description.Split(':')[0];
                                filter = String.Format("$filter=Number eq '{0}' or Name eq '{1}' ", desc.ToString(), desc.ToString());//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                                myobsaleitem = itemservice.GetRange(companyFiles[0], filter, _CompanyFile);
                                if (myobsaleitem.Items.Count() == 0)
                                {
                                    var items_account = itemservice.GetRange(companyFiles[0], null, _CompanyFile);
                                    var myo = items_account.Items.ToList().Where(i => i.Description == desc).ToList();
                                    myo = items_account.Items.ToList().Where(i => i.Description.Contains(desc)).ToList();
                                    myo = items_account.Items.ToList().Where(i => i.Description.Contains(saleitem)).ToList();
                                    if (myo.Count > 0)
                                    {
                                        line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                        {
                                            Name = myo.FirstOrDefault().Name,
                                            Number = myo.FirstOrDefault().Number,
                                            URI = myo.FirstOrDefault().URI,
                                            UID = myo.FirstOrDefault().UID
                                        };
                                    }
                                }
                                else
                                {
                                    line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                    {
                                        Name = myobsaleitem.Items[0].Name,
                                        Number = myobsaleitem.Items[0].Number,
                                        URI = myobsaleitem.Items[0].URI,
                                        UID = myobsaleitem.Items[0].UID
                                    };
                                }
                            }
                            else
                            {
                                line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                {
                                    Name = myobsaleitem.Items[0].Name,
                                    Number = myobsaleitem.Items[0].Number,
                                    URI = myobsaleitem.Items[0].URI,
                                    UID = myobsaleitem.Items[0].UID
                                };
                            }

                            var taxcodelnk = new TaxCodeLink();
                            taxcodelnk.UID = tax_Gstcode.UID;
                            line.TaxCode = taxcodelnk;
                            Lines.Add(line);
                        }
                        InvoiceTerms terms = new InvoiceTerms { MonthlyChargeForLatePayment = 0 };
                        var Invoice = new ItemInvoice()
                        {
                            Date = Convert.ToDateTime(invoicedata.InvoiceDate),
                            Customer = new CardLink()
                            {
                                DisplayID = "USS0" + customerData.CTId,
                                Name = customerData.CustomerLastName,
                                UID = custinfo.Items.FirstOrDefault().UID,
                                URI = custinfo.Items.FirstOrDefault().URI
                            },
                            UID = SyncInvoiceId,
                            Number = invoicedata.InvoiceNo.ToString(),
                            Lines = Lines,
                            Terms = terms,
                            TotalTax = TotalTax,
                            TotalAmount = subtotal + TotalTax,
                            BalanceDueAmount = Convert.ToDecimal(InvoicePaidAmount - (subtotal + TotalTax)),
                            IsTaxInclusive = false,
                            InvoiceType = InvoiceLayoutType.Item,
                            LastPaymentDate = LastPaymentDate != null ? LastPaymentDate : null
                        };
                        invService.Insert(companyFiles[0], Invoice, _CompanyFile);
                        invoicessyncedtotal = invoicessyncedtotal + Invoice.Number + ",";
                    }
                    else
                    {
                        #endregion
                    }

                    #endregion

                    #region CustomerPayment
                    var invpaymentservice = new CustomerPaymentService(configuration, null, keystore); // invoice service
                    invoicepayment = invoicepayment.Where(i => i.IsSynctoMyob == false || i.IsSynctoMyob == null).ToList();
                    if (invoicepayment.Count > 0)
                    {
                        foreach (var invoice in invoicepayment)
                        {
                            CustomerPayment CustomerPayment = new CustomerPayment();
                            CustomerPayment.Account = new AccountLink { UID = Account_Service.Items.FirstOrDefault().UID };
                            CustomerPayment.AmountReceived = InvoicePaidAmount;
                            CustomerPayment.Customer = new CardLink { UID = custinfo.Items.FirstOrDefault().UID };
                            CustomerPayment.DepositTo = 0;//Account or undeposited funds
                            var payments = invoicepayment.Select(i => i.PaymentMethod).FirstOrDefault();
                            CustomerPayment.Date = Convert.ToDateTime(invoice.PaymentDate);
                            CustomerPayment.Memo = "Payment;" + (string.IsNullOrEmpty(invoice.Reference) ? "" : invoice.Reference);

                            switch (invoice.PaymentMethod)
                            {
                                case 1:
                                    CustomerPayment.PaymentMethod = "Credit Card";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                                case 2:
                                    CustomerPayment.PaymentMethod = "Debit Card";
                                    CustomerPayment.Account = new AccountLink { UID = BankAccountFunds.UID };
                                    break;
                                case 3:
                                    CustomerPayment.PaymentMethod = "Cash";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                                case 4:
                                    CustomerPayment.PaymentMethod = "Net Banking";
                                    CustomerPayment.Account = new AccountLink { UID = BankAccountFunds.UID };
                                    break;
                                case 5:
                                    CustomerPayment.PaymentMethod = "Cheque";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                                case 6:
                                    CustomerPayment.PaymentMethod = "PayPal";
                                    CustomerPayment.Account = new AccountLink { UID = BankAccountFunds.UID };
                                    break;
                                case 7:
                                    CustomerPayment.PaymentMethod = "DIRECT DEPOSIT MULTIPLE";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                            }
                            if (string.IsNullOrEmpty(CustomerPayment.PaymentMethod))
                            { CustomerPayment.PaymentMethod = "Debit Card"; }
                            List<CustomerPaymentLine> listofpayment = new List<CustomerPaymentLine>();
                            invoices = invService.GetRange(companyFiles[0], "$filter = Number eq +'" + invoicedata.InvoiceNo.ToString() + "'", _CompanyFile);
                            isInvoicexist = invoices.Items.Where(i => i.Number == invoicedata.InvoiceNo.ToString()).FirstOrDefault();
                            CustomerPaymentLine line = new CustomerPaymentLine();
                            line.AmountApplied = Convert.ToDecimal(invoice.PaymentAmount);
                            line.Number = invoicedata.InvoiceNo.ToString();
                            line.Type = 0;//invoice
                            line.UID = isInvoicexist.UID;
                            listofpayment.Add(line);
                            var paymentpaid = invoicepayment.Where(i => (i.IsSynctoMyob == false || i.IsSynctoMyob == null) && i.InvoiceId == invoicedata.Id).ToList();
                            if (listofpayment.Count > 0)
                            {
                                CustomerPayment.Invoices = listofpayment;
                                invpaymentservice.Insert(companyFiles[0], CustomerPayment, _CompanyFile);
                            }
                        }
                    }
                    //Update the Status of the payments
                    foreach (var value in invoicepayment)
                    {
                        var statusofinvoice = InvoicePaymentRepo.FindBy(i => i.Id == value.Id).FirstOrDefault();
                        statusofinvoice.IsSynctoMyob = true;
                        InvoicePaymentRepo.Save();
                    }
                    #endregion
                    InvoiceRep.SaveInvoiceSyncstatus(SyncInvoiceId);
                }

                return "1";

            }

            catch (Exception ex)

            {
                return ex.Message + " : " + ex.InnerException + "" + SyncInvoiceId;
                throw ex;

            }
        }



        //Get Customer Billing Detail for the my ob
        public BillingAddressForMyob GetCustomerBillingAddress(string invoicdID)
        {
            int CustomerType = 0;
            string CustomerName = "";
            Guid? Id = Guid.Parse(invoicdID);
            FSM.Core.Entities.Invoice InvoiceData = InvoiceRep.FindBy(m => m.Id == Id).FirstOrDefault();
            var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
            //mapping entity to viewmodel
            CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<FSM.Core.Entities.Invoice, CreateInvoiceViewModel>();
            CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
            invoiceViewModel = mapper.Mapper(InvoiceData);
            BillingAddressForMyob obj = new BillingAddressForMyob();
            var jobdetail = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).FirstOrDefault();
            var sitedetail = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
            if (jobdetail != null)
            {
                invoiceViewModel.WorkOrderNumber = !String.IsNullOrEmpty(jobdetail.WorkOrderNumber) ? jobdetail.WorkOrderNumber : "";
            }
            else
            {
                invoiceViewModel.WorkOrderNumber = "";
            }

            var customer = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();   //get customer detail

            invoiceViewModel.TradeName = "";
            if (customer != null)
            {
                CustomerType = customer.CustomerType.HasValue ? Convert.ToInt32(customer.CustomerType) : 0;
            }

            dynamic billingdetail = null;
            var BillingAddressId = InvoiceData.BillingAddressId;
            if (BillingAddressId != null)
            {
                if (BillingAddressId != Guid.Empty)
                {
                    billingdetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                }
                else
                {
                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                }
            }
            else
            {
                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
            }

            if (billingdetail == null)
            {

                billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

            }
            if (customer != null)
            {
                var TradingName = customer.TradingName;
                if (!string.IsNullOrEmpty(TradingName))
                {
                    invoiceViewModel.TradeName = !String.IsNullOrEmpty(customer.TradingName) ? customer.TradingName : "";

                }
                CustomerName = !String.IsNullOrEmpty(customer.CustomerLastName) ? customer.CustomerLastName : "";
            }


            if (billingdetail == null)
            {

                invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                obj.StreetNo = sitedetail.Street;
                obj.StreetName = sitedetail.StreetName;
                obj.Suburb = sitedetail.Suburb;
                obj.Suburb = sitedetail.Suburb;
                obj.State = sitedetail.State;
                obj.PostalCode = sitedetail.PostalCode.ToString();
            }
            else
            {

                obj.FirstName = billingdetail.FirstName;
                obj.LastName = billingdetail.LastName;
                obj.StreetNo = billingdetail.StreetNo;
                obj.StreetName = billingdetail.StreetName;
                obj.Suburb = billingdetail.Suburb;
                obj.Suburb = billingdetail.Suburb;
                obj.State = billingdetail.State;
                obj.PostalCode = sitedetail.PostalCode.ToString();

            }
            if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Domestic))
            {
                if (billingdetail == null)
                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
            }
            else if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.RealState) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Strata) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Commercial))
            {

                if (billingdetail == null)
                {

                    obj.StreetNo = sitedetail.Street;
                    obj.StreetName = sitedetail.StreetName;
                    obj.Suburb = sitedetail.Suburb;
                    obj.Suburb = sitedetail.Suburb;
                    obj.State = sitedetail.State;
                    obj.PostalCode = sitedetail.PostalCode.ToString();
                }
                else
                {

                    obj.FirstName = billingdetail.FirstName;
                    obj.LastName = billingdetail.LastName;
                    obj.StreetNo = billingdetail.StreetNo;
                    obj.StreetName = billingdetail.StreetName;
                    obj.Suburb = billingdetail.Suburb;
                    obj.Suburb = billingdetail.Suburb;
                    obj.State = billingdetail.State;
                    obj.PostalCode = sitedetail.PostalCode.ToString();
                }

            }

            obj.TradeName = String.IsNullOrEmpty(obj.TradeName) ? "" : obj.TradeName;
            obj.FirstName = String.IsNullOrEmpty(obj.FirstName) ? "" : obj.FirstName;
            obj.LastName = String.IsNullOrEmpty(obj.LastName) ? "" : obj.LastName;
            obj.StreetNo = String.IsNullOrEmpty(obj.StreetNo) ? "" : obj.StreetNo;
            obj.StreetName = String.IsNullOrEmpty(obj.StreetName) ? "" : obj.StreetName;
            obj.Suburb = String.IsNullOrEmpty(obj.Suburb) ? "" : obj.Suburb;
            obj.State = String.IsNullOrEmpty(obj.State) ? "" : obj.State;
            obj.PostalCode = String.IsNullOrEmpty(obj.PostalCode) ? "" : obj.PostalCode;

            return obj;
        }



        public void SyncAllInvoices()
        {
            //try
            //{
            string invoicessyncedtotal = "";
            #region GetCompanyFile
            CustomerGeneralInfo customerData = new Core.Entities.CustomerGeneralInfo();
            var developerKey = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperKey"]; //"jgnsyvj7brdpw7kb4ftpz3rk";
            var developerSecret = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperSecret"];// "PqbpPYmxr2UcCfScHxrkEYSz";
            var UserName = System.Configuration.ConfigurationManager.AppSettings["CompanyUserName"];
            var Password = System.Configuration.ConfigurationManager.AppSettings["CompanyPassword"];
            var CompanyFilename = System.Configuration.ConfigurationManager.AppSettings["CompanyFileName"];
            var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlInvoiceList"];
            CompanyFileCredentials _CompanyFile = new CompanyFileCredentials(UserName, Password);
            var configuration = new ApiConfiguration(developerKey, developerSecret, RedirectUrl);
            var oauthService = new OAuthService(configuration);
            string code = _Apiode;
            var tokens = oauthService.GetTokens(code);
            tokens.ExpiresIn = 1;
            var mytoken = tokens;
            if (tokens.HasExpired)
            {
                mytoken = oauthService.RenewTokens(tokens);
            }
            var keystore = new SimpleOAuthKeyService();
            keystore.OAuthResponse = mytoken;

            //Get Company Files
            var cfService = new CompanyFileService(configuration, null, keystore);
            var companyFiles = cfService.GetRange();

            //GEt Locked date of the myob
            companyFiles = companyFiles.Where(i => i.Name == CompanyFilename).ToArray();
            MYOB.AccountRight.SDK.Services.Company.CompanyPreferencesService companypreference = new MYOB.AccountRight.SDK.Services.Company.CompanyPreferencesService(configuration, null, keystore);
            var companyPreference = companypreference.Get(companyFiles[0], _CompanyFile, null);
            DateTime? lockDatetime = companyPreference.System.LockPeriodPriorTo;
            #endregion

            #region GetITemService (From My Ob)
            var itemservice = new MYOB.AccountRight.SDK.Services.Inventory.ItemService(configuration, null, keystore); // invoice service
            var items = itemservice.GetRange(companyFiles[0], null, _CompanyFile);
            #endregion
            var supService = new SupplierService(configuration, null, keystore);
            #region GetAllInvoices From FSM Which are not synced
            var invoiceRange = InvoiceRep.GetALLInvoices(Guid.Empty);//if only one id than parameter otherwise not 
            #endregion
            #region GetCustomerInfo from Invoiceid
            foreach (var Currentinvoice in invoiceRange)
            {
                try
                {
                    SyncInvoiceId = Guid.Parse(Currentinvoice.ID.ToString());
                    FSM.Core.Entities.Invoice invoicedata = InvoiceRep.FindBy(i => i.Id == SyncInvoiceId).FirstOrDefault();
                    if (invoicedata != null)
                    {
                        if (invoicedata.CustomerGeneralInfoId != null)
                        {
                            if (invoicedata.CustomerGeneralInfoId != Guid.Empty)

                            {
                                customerData = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == invoicedata.CustomerGeneralInfoId).FirstOrDefault();

                            }
                            else
                            {

                                var Customer = JobRepository.FindBy(i => i.Id == invoicedata.EmployeeJobId).FirstOrDefault();
                                customerData = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == Customer.CustomerGeneralInfoId).FirstOrDefault();
                            }
                        }
                        else
                        {
                            var Customer = JobRepository.FindBy(i => i.Id == invoicedata.EmployeeJobId).FirstOrDefault();
                            customerData = CustomerGeneralInfo.FindBy(i => i.CustomerGeneralInfoId == Customer.CustomerGeneralInfoId).FirstOrDefault();
                        }
                    }
                    #endregion
                    var GetCustomerViaInvoice = new InvoiceService(configuration, null, keystore); // invoice service

                    #region GetCustomer Service and Various Codes in company File
                    var custService = new CustomerService(configuration, null, keystore); // customer service
                    string filtername = customerData.CustomerLastName.ToString();
                    if (filtername.ToLower().Contains('&'))
                    {
                        filtername = filtername.Replace("&", "'&'");
                    }
                    string pageFilter = String.Format("$filter=DisplayID eq '{0}' or CompanyName eq '{1}' or LastName eq '{2}' ", "USS0" + customerData.CTId.ToString(), filtername, filtername);
                    var custinfo = custService.GetRange(companyFiles[0], pageFilter, _CompanyFile);
                    // tax code service
                    var tcService = new TaxCodeService(configuration, null, keystore);
                    var tcList = tcService.GetRange(companyFiles[0], null, _CompanyFile);
                    #region GET The Account Linked to the Myob
                    AccountService accnt = new AccountService(configuration, null, keystore);
                    string accountfilter = String.Format("$filter=DisplayID eq '{0}' or DisplayID eq '{1}' ", "1-1180", "1-1111");//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                    var Account_Service = accnt.GetRange(companyFiles[0], accountfilter, _CompanyFile);
                    var UndepositedFunds = Account_Service.Items.ToList().Where(i => i.Number == 1180).FirstOrDefault();
                    var BankAccountFunds = Account_Service.Items.ToList().Where(i => i.Number == 1111).FirstOrDefault();
                    #endregion
                    #region get tax code items
                    var taxcode = new TaxCodeService(configuration, null, keystore);
                    var taxcodeservice = taxcode.GetRange(companyFiles[0], null, _CompanyFile);
                    var tax_Gstcode = taxcodeservice.Items.ToList().Where(i => i.Code == "GST").FirstOrDefault();
                    MYOB.AccountRight.SDK.Contracts.Version2.Contact.Customer cust1 = new MYOB.AccountRight.SDK.Contracts.Version2.Contact.Customer();
                    #endregion
                    #endregion
                    #region GetInvoicePayment from FSM
                    var invoicepayment = InvoicePaymentRepo.FindBy(i => i.InvoiceId == invoicedata.Id).ToList();
                    var InvoicePaidAmount = Convert.ToDecimal(invoicepayment.Sum(i => i.PaymentAmount));
                    DateTime? LastPaymentDate = null;
                    if (invoicepayment.Count > 0)
                    {
                        var lastpayment = invoicepayment.OrderByDescending(i => i.PaymentDate).Select(i => i.PaymentDate).FirstOrDefault();
                        LastPaymentDate = lastpayment.Value;
                    }

                    #endregion

                    #region Customer Add
                    var customerid = Guid.Parse(customerData.CustomerGeneralInfoId.ToString());
                    //Check if customer Exist or not
                    if (custinfo.Items.Count() == 0)
                    {
                        BillingAddressForMyob ob = GetCustomerBillingAddress(SyncInvoiceId.ToString());
                        var CustomerBillingAddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerid).FirstOrDefault();
                        string Email = "";
                        string Phone1 = "";
                        string Phone2 = "";
                        string StreetNo = "";
                        string streetName = "";
                        string postalcode = "";
                        string Suburb = "";
                        string state = "";
                        string firstName = "";
                        String LastName = "";
                        if (CustomerBillingAddress != null)
                        {
                            firstName = !string.IsNullOrEmpty(CustomerBillingAddress.FirstName) ? CustomerBillingAddress.FirstName : "";
                            LastName = !string.IsNullOrEmpty(CustomerBillingAddress.LastName) ? CustomerBillingAddress.LastName : "";
                            StreetNo = !string.IsNullOrEmpty(CustomerBillingAddress.StreetNo) ? CustomerBillingAddress.StreetName : "";
                            streetName = !string.IsNullOrEmpty(CustomerBillingAddress.StreetName) ? CustomerBillingAddress.StreetName : "";
                            postalcode = !string.IsNullOrEmpty(CustomerBillingAddress.PostalCode) ? CustomerBillingAddress.PostalCode : "";
                            state = !string.IsNullOrEmpty(CustomerBillingAddress.State) ? CustomerBillingAddress.State : "";
                            Suburb = !string.IsNullOrEmpty(CustomerBillingAddress.Suburb) ? CustomerBillingAddress.Suburb : "";
                            Email = CustomerBillingAddress.EmailId;
                            Phone1 = CustomerBillingAddress.PhoneNo1;
                            Phone2 = CustomerBillingAddress.PhoneNo2;
                        }
                        if (customerData.CustomerType == Convert.ToInt32(CustomerType.Domestic))
                        {
                            cust1.Addresses = new List<Address>() { new Address() { ContactName = "", Street = firstName + " " + LastName + " ," + StreetNo + " " + streetName + " " + Suburb, Country = "Australia", Email = Email, Fax = "", Phone1 = Phone1, Phone2 = Phone2, State = state, PostCode = postalcode } };
                        }
                        else
                        {
                            cust1.Addresses = new List<Address>() { new Address() { ContactName = "", Street = customerData.TradingName + " \n" + StreetNo + " " + streetName + " " + Suburb, Country = "Australia", Email = Email, Fax = "", Phone1 = Phone1, Phone2 = Phone2, State = state, PostCode = postalcode } };
                        }
                        cust1.CompanyName = !string.IsNullOrEmpty(customerData.CustomerLastName) ? customerData.CustomerLastName : "";//"CompanyName";
                        if (cust1.CompanyName.Length > 50)
                        {
                            cust1.CompanyName = cust1.CompanyName.Substring(0, 49);
                        }
                        cust1.CurrentBalance = Convert.ToDecimal(invoicedata.Due);
                        cust1.IsIndividual = false;
                        cust1.DisplayID = "USS0" + customerData.CTId.ToString(); //"USS0+CTId.Tostring()";//
                        cust1.IsActive = true;
                        cust1.Notes = !string.IsNullOrEmpty(customerData.CustomerNotes) ? customerData.CustomerNotes : "";
                        if (!string.IsNullOrEmpty(customerData.CustomerNotes))
                        {
                            if (cust1.Notes.Length > 250)
                            {
                                cust1.Notes = cust1.Notes.Substring(0, 249);
                            }
                        }
                        cust1.UID = Guid.Parse(customerData.CustomerGeneralInfoId.ToString());
                        CustomerSellingDetails sellingdetail = new CustomerSellingDetails();
                        sellingdetail.Credit = new CustomerCredit
                        {
                            Limit = 0,
                            Available = 0,
                            PastDue = 0,
                            OnHold = false
                        };
                        cust1.Type = ContactType.Customer;
                        sellingdetail.PrintedForm = "Pre-Printed Invoice";//"Pre-Printed Invoice";
                        sellingdetail.ItemPriceLevel = "Level A";//"";
                        sellingdetail.IncomeAccount = null;
                        sellingdetail.TaxCode = new TaxCodeLink { Code = tax_Gstcode.Code, UID = tax_Gstcode.UID, URI = tax_Gstcode.URI };
                        sellingdetail.FreightTaxCode = new TaxCodeLink { Code = tax_Gstcode.Code, UID = tax_Gstcode.UID, URI = tax_Gstcode.URI };
                        sellingdetail.ReceiptMemo = "Thank you";//"Thank you";
                        sellingdetail.SalesPerson = null;
                        sellingdetail.SaleComment = "We appreciate your business.";//"We appreciate your business.";
                        sellingdetail.ShippingMethod = "Freight";//"Freight";
                        sellingdetail.HourlyBillingRate = 0;
                        sellingdetail.UseCustomerTaxCode = false;
                        cust1.SellingDetails = sellingdetail;
                        cust1.PaymentDetails = null;
                        var myCredentials = _CompanyFile;
                        custService.Insert(companyFiles.FirstOrDefault(), cust1, myCredentials, ErrorLevel.WarningsAsErrors);
                    }
                    else
                    {
                        //Update Customer
                        var CustomerBillingAddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerid).FirstOrDefault();
                        cust1 = new MYOB.AccountRight.SDK.Contracts.Version2.Contact.Customer();
                        cust1 = custinfo.Items.ToList().FirstOrDefault();
                        var myCredentials = _CompanyFile;

                    }

                    #endregion

                    #region insert invoice
                    custinfo = custService.GetRange(companyFiles[0], pageFilter, _CompanyFile);
                    var invService = new ItemInvoiceService(configuration, null, keystore); // invoice service
                    var invoices = invService.GetRange(companyFiles[0], "$filter = Number eq '" + invoicedata.InvoiceNo.ToString() + "'", _CompanyFile);
                    var isInvoicexist = invoices.Items.Where(i => i.Number == invoicedata.InvoiceNo.ToString()).FirstOrDefault();
                    System.Collections.Generic.List<ItemInvoiceLine> Lines = new List<ItemInvoiceLine>();
                    #region GetInvoiceDetail
                    var invoicejclitemmapping = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == invoicedata.Id).OrderBy(i => i.OrderNo).ToList();
                    if (invoicejclitemmapping.Count() == 0)
                    {
                        invoicejclitemmapping = invoiceJCLItemRepo.FindBy(i => i.JobID == invoicedata.EmployeeJobId).OrderBy(i => i.OrderNo).ToList();
                    }

                    if (isInvoicexist == null && invoicedata.InvoiceDate >= Convert.ToDateTime(lockDatetime))
                    {
                        decimal subtotal = Convert.ToDecimal(invoicejclitemmapping.Sum(i => i.TotalPrice));
                        decimal TotalTax = Convert.ToDecimal((subtotal * 10) / 100);
                        foreach (var item in invoicejclitemmapping)
                        {
                            ItemInvoiceLine line = new ItemInvoiceLine();
                            line.Type = InvoiceLineType.Transaction;
                            line.ShipQuantity = item.Quantity;
                            line.UnitPrice = item.Price;
                            line.Total = item.TotalPrice;
                            if (line.UnitPrice < 0)
                            {
                                line.UnitPrice = -1 * (item.Price);
                                line.ShipQuantity = -1 * (line.ShipQuantity);
                                line.Total = item.TotalPrice;
                            }

                            if (!string.IsNullOrEmpty(item.Description))
                            {
                                if (item.Description.Length <= 250)
                                {
                                    line.Description = item.Description;
                                }
                                else
                                {
                                    line.Description = item.Description.Substring(0, 249);
                                }
                            }
                            else
                            {
                                line.Description = String.Empty;
                            }
                            var Saleitem = invoiceJCLItemRepo.FindBy(i => i.JCLItemID == item.JCLItemID && i.InvoiceId == SyncInvoiceId).FirstOrDefault();
                            if (Saleitem == null)
                            {
                                var jobid = invoicejclitemmapping.FirstOrDefault().JobID;
                                Saleitem = invoiceJCLItemRepo.FindBy(i => i.JCLItemID == item.JCLItemID && i.JobID == jobid).FirstOrDefault();
                            }
                            var itemname = JCLRepo.FindBy(i => i.JCLId == Saleitem.JCLItemID).FirstOrDefault();
                            var saleitem = itemname.ItemName;
                            if (saleitem == "00 - Gutter Cleaning - Repeat")
                            {
                                saleitem = "00 CLEAN - REPEAT CUSTOMER";
                            }
                            if (saleitem.Trim() == "Gutter Cleaning")
                            {
                                saleitem = "Remove all debris";
                            }
                            if (saleitem == "00 - Gutter Cleaning - Initial")
                            {
                                saleitem = "Remove all debris";
                            }
                            if (saleitem == "00 - Roof Plumbing Blank")
                            {
                                saleitem = "0 BLANK ROOF PLUMBING";
                            }
                            if (saleitem == "SRAS - WITH HATCH")
                            {
                                saleitem = "SRAS HATCH";
                            }
                            if (saleitem.Trim() == "PGG - HDPE")
                            {
                                saleitem = "PGG PLASTIC";
                            }
                            if (saleitem.Trim() == "PGG")
                            {
                                saleitem = "PGG PLASTIC";
                            }
                            if (saleitem == "PVG METAL VALLEY GUARD")
                            {
                                saleitem = "VALLEY PGG METAL";
                            }
                            if (saleitem == "00 - Gutter Cleaning - Contracted")
                            {
                                saleitem = "00 CLEAN - CONTRACTED";
                            }
                            if (saleitem == "RECERT FREE")
                            {
                                saleitem = "RE-CERT FREE";
                            }
                            if (saleitem == "00 - 6m Option Gutter Clean Offer")
                            {
                                saleitem = "6 Monthly";
                            }
                            if (saleitem == "00 - Roof Tiling Blank")
                            {
                                saleitem = "0 BLANK ITEM TILING";
                            }
                            if (saleitem == "REP VALLEY (TILED ROOF)")
                            {
                                saleitem = "REP VALLEY TILE";
                            }

                            if (saleitem == "REP VALLEY (METAL ROOF)")
                            {
                                saleitem = "REP VALLEYS METAL";
                            }
                            var description = Saleitem.Description;
                            string filter = "";
                            if (itemname.Category == 6)
                            {
                                filter = String.Format("$filter=Number eq '{0}' or Name eq '{1}' ", "0 BLANK ITEM MISCELLANEOUS", "0 BLANK ITEM MISCELLANEOUS");//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                            }
                            else
                            {
                                filter = String.Format("$filter=Number eq '{0}' or Name eq '{1}' ", saleitem.ToString(), saleitem.ToString());//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                            }
                            var myobsaleitem = itemservice.GetRange(companyFiles[0], filter, _CompanyFile);
                            if (myobsaleitem.Items.Count() == 0)
                            {
                                string desc = description.Split(':')[0];
                                filter = String.Format("$filter=Number eq '{0}' or Name eq '{1}' ", desc.ToString(), desc.ToString());//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                                myobsaleitem = itemservice.GetRange(companyFiles[0], filter, _CompanyFile);
                                if (myobsaleitem.Items.Count() == 0)
                                {
                                    var items_account = itemservice.GetRange(companyFiles[0], null, _CompanyFile);
                                    var myo = items_account.Items.ToList().Where(i => i.Description == desc).ToList();
                                    myo = items_account.Items.ToList().Where(i => i.Description.Contains(desc)).ToList();
                                    myo = items_account.Items.ToList().Where(i => i.Description.Contains(saleitem)).ToList();
                                    if (myo.Count > 0)
                                    {
                                        line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                        {
                                            Name = myo.FirstOrDefault().Name,
                                            Number = myo.FirstOrDefault().Number,
                                            URI = myo.FirstOrDefault().URI,
                                            UID = myo.FirstOrDefault().UID
                                        };
                                    }
                                }
                                else
                                {
                                    line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                    {
                                        Name = myobsaleitem.Items[0].Name,
                                        Number = myobsaleitem.Items[0].Number,
                                        URI = myobsaleitem.Items[0].URI,
                                        UID = myobsaleitem.Items[0].UID
                                    };
                                }
                            }
                            else
                            {
                                line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                {
                                    Name = myobsaleitem.Items[0].Name,
                                    Number = myobsaleitem.Items[0].Number,
                                    URI = myobsaleitem.Items[0].URI,
                                    UID = myobsaleitem.Items[0].UID
                                };
                            }

                            var taxcodelnk = new TaxCodeLink();
                            taxcodelnk.UID = tax_Gstcode.UID;
                            line.TaxCode = taxcodelnk;
                            Lines.Add(line);
                        }
                        InvoiceTerms terms = new InvoiceTerms { MonthlyChargeForLatePayment = 0 };
                        var Invoice = new ItemInvoice()
                        {
                            Date = Convert.ToDateTime(invoicedata.InvoiceDate),
                            Customer = new CardLink()
                            {
                                DisplayID = "USS0" + customerData.CTId,
                                Name = customerData.CustomerLastName,
                                UID = custinfo.Items.FirstOrDefault().UID,
                                URI = custinfo.Items.FirstOrDefault().URI
                            },
                            UID = SyncInvoiceId,
                            Number = invoicedata.InvoiceNo.ToString(),
                            Lines = Lines,
                            Terms = terms,
                            TotalTax = TotalTax,
                            TotalAmount = subtotal + TotalTax,
                            BalanceDueAmount = Convert.ToDecimal(InvoicePaidAmount - (subtotal + TotalTax)),
                            IsTaxInclusive = false,
                            InvoiceType = InvoiceLayoutType.Item,
                            LastPaymentDate = LastPaymentDate != null ? LastPaymentDate : null
                        };
                        invService.Insert(companyFiles[0], Invoice, _CompanyFile);
                        invoicessyncedtotal = invoicessyncedtotal + Invoice.Number + ",";
                    }
                    else
                    {
                        #endregion
                    }

                    #endregion

                    #region CustomerPayment
                    var invpaymentservice = new CustomerPaymentService(configuration, null, keystore); // invoice service
                    invoicepayment = invoicepayment.Where(i => i.IsSynctoMyob == false || i.IsSynctoMyob == null).ToList();
                    if (invoicepayment.Count > 0)
                    {
                        foreach (var invoice in invoicepayment)
                        {
                            CustomerPayment CustomerPayment = new CustomerPayment();
                            CustomerPayment.Account = new AccountLink { UID = Account_Service.Items.FirstOrDefault().UID };
                            CustomerPayment.AmountReceived = InvoicePaidAmount;
                            CustomerPayment.Customer = new CardLink { UID = custinfo.Items.FirstOrDefault().UID };
                            CustomerPayment.DepositTo = 0;//Account or undeposited funds
                            var payments = invoicepayment.Select(i => i.PaymentMethod).FirstOrDefault();
                            CustomerPayment.Date = Convert.ToDateTime(invoice.PaymentDate);
                            CustomerPayment.Memo = "Payment;" + (string.IsNullOrEmpty(invoice.Reference) ? "" : invoice.Reference);

                            switch (invoice.PaymentMethod)
                            {
                                case 1:
                                    CustomerPayment.PaymentMethod = "Credit Card";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                                case 2:
                                    CustomerPayment.PaymentMethod = "Debit Card";
                                    CustomerPayment.Account = new AccountLink { UID = BankAccountFunds.UID };
                                    break;
                                case 3:
                                    CustomerPayment.PaymentMethod = "Cash";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                                case 4:
                                    CustomerPayment.PaymentMethod = "Net Banking";
                                    CustomerPayment.Account = new AccountLink { UID = BankAccountFunds.UID };
                                    break;
                                case 5:
                                    CustomerPayment.PaymentMethod = "Cheque";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                                case 6:
                                    CustomerPayment.PaymentMethod = "PayPal";
                                    CustomerPayment.Account = new AccountLink { UID = BankAccountFunds.UID };
                                    break;
                                case 7:
                                    CustomerPayment.PaymentMethod = "DIRECT DEPOSIT MULTIPLE";
                                    CustomerPayment.Account = new AccountLink { UID = UndepositedFunds.UID };
                                    break;
                            }
                            if (string.IsNullOrEmpty(CustomerPayment.PaymentMethod))
                            { CustomerPayment.PaymentMethod = "Debit Card"; }
                            List<CustomerPaymentLine> listofpayment = new List<CustomerPaymentLine>();
                            invoices = invService.GetRange(companyFiles[0], "$filter = Number eq +'" + invoicedata.InvoiceNo.ToString() + "'", _CompanyFile);
                            isInvoicexist = invoices.Items.Where(i => i.Number == invoicedata.InvoiceNo.ToString()).FirstOrDefault();
                            CustomerPaymentLine line = new CustomerPaymentLine();
                            line.AmountApplied = Convert.ToDecimal(invoice.PaymentAmount);
                            line.Number = invoicedata.InvoiceNo.ToString();
                            line.Type = 0;//invoice
                            line.UID = isInvoicexist.UID;
                            listofpayment.Add(line);
                            var paymentpaid = invoicepayment.Where(i => (i.IsSynctoMyob == false || i.IsSynctoMyob == null) && i.InvoiceId == invoicedata.Id).ToList();
                            if (listofpayment.Count > 0)
                            {
                                CustomerPayment.Invoices = listofpayment;
                                invpaymentservice.Insert(companyFiles[0], CustomerPayment, _CompanyFile);
                            }
                        }
                    }
                    //Update the Status of the payments
                    foreach (var value in invoicepayment)
                    {
                        var statusofinvoice = InvoicePaymentRepo.FindBy(i => i.Id == value.Id).FirstOrDefault();
                        statusofinvoice.IsSynctoMyob = true;
                        InvoicePaymentRepo.Save();
                    }
                    #endregion
                    InvoiceRep.SaveInvoiceSyncstatus(SyncInvoiceId);
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
                    continue;
                }
            }
            //}

            //catch (Exception ex)

            //{
            //    return ex.Message + " : " + ex.InnerException + "" + SyncInvoiceId;
            //    throw ex;

            //}
        }


        private void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = Server.MapPath("~/ErrorLog/ErrorLogInvoiceMyob.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
    internal static class OAuthLogin
    {
        private const string CsOAuthServer = "https://secure.myob.com/oauth2/account/authorize/";
        // private const string CsOAuthServer = "http://localhost:8080/accountright/";
        //private const string CsOAuthServer = "http://192.168.16.4/accountright/";

        private const string CsOAuthScope = "CompanyFile";

        /// <summary>
        /// Function to return the OAuth code
        /// </summary>
        /// <param name="config">Contains the API configuration such as ClientId and Redirect URL</param>
        /// <returns>OAuth code</returns>
        /// <remarks></remarks>
        public static string GetAuthorizationCode(IApiConfiguration config)
        {
            //Format the URL so  User can login to OAuth server
            string url = string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}&response_type=code", CsOAuthServer,
                                       config.ClientId, HttpUtility.UrlEncode(config.RedirectUrl), CsOAuthScope);

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://secure.myob.com/oauth2/account/authorize?client_id=jgnsyvj7brdpw7kb4ftpz3rk&redirect_uri=http://www.srag-portal.com/LibraryBrowser/OAuthcallback&response_type=code&scope=CompanyFile");
            //request.AllowAutoRedirect = true;

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream dataStream = response.GetResponseStream();

            //string url = response.ResponseUri.ToString();


            // response.Close();

            // Create a new form with a web browser to display OAuth login page
            var frm = new Form();
            var webB = new WebBrowser();
            frm.Controls.Add(webB);
            webB.Dock = DockStyle.Fill;

            // Add a handler for the web browser to capture content change 
            webB.DocumentTitleChanged += WebBDocumentTitleChanged;

            // navigat to url and display form
            //webB.Navigate(url);
            frm.Size = new Size(800, 600);
            frm.ShowDialog();

            //  System.Web.HttpContext.Current.Response.Redirect(url);

            //Retrieve the code from the returned HTML
            return ExtractSubstring(webB.DocumentText, "code=", "<");
        }



        /// <summary>
        /// Handler that is called when HTML title is changed in browser (i.e. content is reloaded)
        /// Once user has signed in to OAth page and authorised this app the OAuth code is returned in the HTML content 
        /// </summary>
        /// <param name="sender">The web browser control</param>
        /// <param name="e">The event</param>
        /// <remarks>This assumes redirect URL is http://desktop</remarks>
        private static void WebBDocumentTitleChanged(object sender, EventArgs e)
        {
            var webB = (WebBrowser)sender;
            var frm = (Form)webB.Parent;

            //Check if OAuth code is returned
            if (webB.DocumentText.Contains("code="))
            {
                frm.Close();
            }
        }

        /// <summary>
        /// Function to retrieve content from a string based on begining and ending pattern
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="startsWith">start pattern</param>
        /// <param name="endsWith">end pattern</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string ExtractSubstring(string input, string startsWith, string endsWith)
        {
            Match match = Regex.Match(input, startsWith + "(.*)" + endsWith);
            string code = match.Groups[1].Value;
            return code;
        }


    }

    public class selctemail
    {
        public string Email { get; set; }
        public string id { get; set; }
    }
}