using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using FSM.Web.FSMConstant;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FSM.Web.Areas.Employee.Controllers
{
    public class MaterialController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                        ().DeclaringType);
        [Dependency]
        public IiNoviceRepository InvoiceRep { get; set; }
        [Dependency]
        public IEmployeeJobRepository Employeejob { get; set; }
        [Dependency]
        public IJobAssignToMappingRepository JobAssignMapping { get; set; }
        [Dependency]
        public IEmployeeDetailRepository Employee { get; set; }
        [Dependency]
        public ICustomerGeneralInfoRepository CustomerGeneralInfoRepo { get; set; }
        [Dependency]
        public IinvoicedJCLItemMappingRepository invoiceJCLItemRepo { get; set; }
        [Dependency]
        public IJCLRepository JCLRepo { get; set; }
        [Dependency]
        public IiNvoicePaymentRepository InvoicePaymentRepo { get; set; }

        [Dependency]
        public IJCLColor_MappingRepository JCLColorRepo { get; set; }

        [Dependency]
        public IJCLProducts_MappingRepository JCLProductRepo { get; set; }
        [Dependency]
        public IJCLSize_MappingRepository JCLSizeRepo { get; set; }
        [Dependency]
        public IJClExtraproduct_mappingRepositories JCLExtraRepo { get; set; }


        // GET: Employee/Material
        public ActionResult Index()
        {
            try
            {
                using (InvoiceRep)
                {
                    MaterialSearchViewModel SearchViewModel = new MaterialSearchViewModel();
                    if (Request.QueryString["Keyword"] != null)
                    {
                        SearchViewModel.searchkeyword = Request.QueryString["Keyword"].ToString().Trim();
                    }
                    else
                    {
                        SearchViewModel.searchkeyword = "";
                    }
                    int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;
                    int pageno = Request.QueryString["grid-page"] != null ? int.Parse(Request.QueryString["grid-page"]) : 1;

                    var invoice = InvoiceRep.GetApprovedQuoteListBySearchKeyword(SearchViewModel.searchkeyword, pageno, PageSize).ToList();
                    invoice = invoice.DistinctBy(m => m.EmployeeJobId).OrderByDescending(m => m.CreatedDate).ToList();
                    CommonMapper<CreateInvoiceCoreViewModel, MaterialViewModel> mapper = new CommonMapper<CreateInvoiceCoreViewModel, MaterialViewModel>();
                    List<MaterialViewModel> invoicelist = mapper.MapToList(invoice.ToList());
                    if (invoicelist.Count > 0)
                    {
                        invoicelist = mapper.MapToList(invoice.ToList());
                        for (int i = 0; i < invoicelist.Count; i++)
                        {
                            //var EmployeeForjob = invoice.Where(k => k.EmployeeJobId == invoicelist[i].EmployeeJobId);
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
                    var invoiceListViewModel = new ApprovedQuoteListViewModel
                    {
                        MaterialInvoiceViewModel = invoicelist.OrderByDescending(i => i.InvoiceNo),
                        PageSize = PageSize,
                        searchkeyword = ""
                    };
                    return View(invoiceListViewModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetApprovedQuotematerial()
        {
            try
            {
                using (InvoiceRep)
                {
                    MaterialSearchViewModel SearchViewModel = new MaterialSearchViewModel();


                    if (Request.QueryString["Keyword"] != null)
                    {
                        SearchViewModel.searchkeyword = Request.QueryString["Keyword"].ToString().Trim();
                    }
                    int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;
                    int pageno = Request.QueryString["grid-page"] != null ? int.Parse(Request.QueryString["grid-page"]) : 1;
                    var invoice = InvoiceRep.GetApprovedQuoteListBySearchKeyword(SearchViewModel.searchkeyword, pageno, PageSize).ToList();
                    invoice = invoice.DistinctBy(m => m.EmployeeJobId).OrderByDescending(m => m.CreatedDate).ToList();
                    CommonMapper<CreateInvoiceCoreViewModel, MaterialViewModel> mapper = new CommonMapper<CreateInvoiceCoreViewModel, MaterialViewModel>();
                    List<MaterialViewModel> invoicelist = mapper.MapToList(invoice.ToList());
                    if (invoicelist.Count > 0)
                    {
                        invoicelist = mapper.MapToList(invoice.ToList());
                        for (int i = 0; i < invoicelist.Count; i++)
                        {
                            //var EmployeeForjob = invoice.Where(k => k.EmployeeJobId == invoicelist[i].EmployeeJobId);
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
                    var invoiceListViewModel = new ApprovedQuoteListViewModel
                    {
                        MaterialInvoiceViewModel = invoicelist.OrderByDescending(i => i.InvoiceNo),
                        PageSize = PageSize,
                        searchkeyword = ""
                    };
                    return PartialView("_MaterailList", invoiceListViewModel);

                }
            }
            catch (Exception)
            {
                throw;
            }

        }



        [HttpGet]
        public ActionResult ViewQuoteMaterial(string Id)
        {
            try
            {
                Quote_MaterialViewmodel model = new Quote_MaterialViewmodel();
                model.QuotematerialViewModel = new QuoteMaterialViewModel();
                model.invoiceJclItemListViewModel = new List<InvoiceJCLItemListViewModel>();
                if (!string.IsNullOrEmpty(Id))
                {
                   
                    Guid id = Guid.Parse(Id);
                    var QuoteInfo = InvoiceRep.FindBy(i => i.Id == id).FirstOrDefault();
                    if (QuoteInfo != null)
                    {
                        model.QuotematerialViewModel.QuoteAcceptedDate = DateTime.Now;
                        
                        var jobs = Employeejob.FindBy(i => i.Id == QuoteInfo.EmployeeJobId).FirstOrDefault();
                        Guid? Createdbyid = Guid.Empty;
                        string CreatedBy = "";
                        if (jobs == null)
                        {
                        }
                        else
                        {
                            Createdbyid = jobs.CreatedBy;
                            var Employeedata = Employee.FindBy(i => i.EmployeeId == Createdbyid).FirstOrDefault();
                            if (Employeedata != null)
                            {
                                CreatedBy = Employeedata.FirstName + " " + Employeedata.LastName;
                            }
                            model.QuotematerialViewModel.QuotedBy = CreatedBy;
                            var customer = CustomerGeneralInfoRepo.FindBy(i => i.CustomerGeneralInfoId == jobs.CustomerGeneralInfoId).FirstOrDefault();
                            model.QuotematerialViewModel.CustomerName = customer.CustomerLastName;
                            model.QuotematerialViewModel.SiteAddress = QuoteInfo.SiteUnit + " " + QuoteInfo.SiteStreetName + " " + QuoteInfo.SiteStreetAddress + " " + QuoteInfo.SiteSuburb + " " + QuoteInfo.SiteState + " " + QuoteInfo.SitePostalCode;

                            var jobassign = JobAssignMapping.FindBy(i => i.JobId == jobs.Id && i.IsDelete == false).ToList();

                           // 
                            if (jobassign.Count()>0)
                            {
                                string assigneed = "";
                                foreach(var assignee in jobassign)
                                {
                                    var AssigneedTo = Employee.FindBy(i => i.EmployeeId == assignee.AssignTo).FirstOrDefault();
                                    assigneed = assigneed +  AssigneedTo.FirstName + " " + AssigneedTo.LastName+",";
                                }
                                model.QuotematerialViewModel.JobAssignedto = assigneed;
                            }
                            else
                            {
                                model.QuotematerialViewModel.JobAssignedto = string.Empty;
                            }
                            model.QuotematerialViewModel.JobDateBooked = jobs.DateBooked;

                            //Check if payment for the quote

                            var payment = InvoicePaymentRepo.FindBy(i => i.InvoiceId == QuoteInfo.Id).ToList();
                            if (payment.Count() > 0)
                            {
                                model.QuotematerialViewModel.Totalpaid = payment.ToList().Sum(i => i.PaymentAmount);
                                model.QuotematerialViewModel.DateDepositPaid = payment.ToList().OrderByDescending(i => i.PaymentDate).FirstOrDefault().PaymentDate;
                            }


                        }
                        var jclinfo = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == QuoteInfo.Id).ToList().OrderBy(i => i.OrderNo);
                        if (jclinfo.Count() == 0)
                        {
                            jclinfo = invoiceJCLItemRepo.FindBy(i => i.JobID == QuoteInfo.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);
                        }
                        JCLItems jclob = new JCLItems();
                        List<JcLViewModel> jclitemslist = new List<JcLViewModel>();
                        foreach (var jclitem in jclinfo)
                        {
                            JcLViewModel item = new JcLViewModel();
                            var jclitemifo = JCLRepo.FindBy(i => i.JCLId == jclitem.JCLItemID).FirstOrDefault();
                            item.Description = jclitem.Description;
                            item.DefaultQty = jclitem.Quantity;
                            item.Price = jclitem.Price;
                            item.GCLID = jclitemifo.JCLId;
                            item.ItemName = jclitemifo.ItemName;
                            if (jclitem.ColorID != null)
                            {
                                var color = JCLColorRepo.FindBy(i => i.ColorId == jclitem.ColorID).FirstOrDefault();
                                item.ColorName = color.ColorName;
                            }
                            else
                            {
                                item.ColorName = "NA";
                            }
                            if (jclitem.SizeID != null)
                            {
                                var size = JCLSizeRepo.FindBy(i => i.SizeId == jclitem.SizeID).FirstOrDefault();
                                item.Sizename = size.Size;
                            }
                            else
                            {
                                item.Sizename = "NA";
                            }
                            if (jclitem.ProductStyleID != null)
                            {
                                var product = JCLProductRepo.FindBy(i => i.ProductId == jclitem.ProductStyleID).FirstOrDefault();
                                item.productname = product.ProductName;
                            }
                            else
                            {
                                item.productname ="NA";
                            }

                            if (jclitem.JCLItemID != null)
                            {
                                var jclitems = JCLRepo.FindBy(i => i.JCLId == jclitem.JCLItemID).FirstOrDefault();
                                item.ItemName = jclitems.ItemName;
                            }
                            else
                            {
                                item.ItemName = "NA";
                            }
                            item.Colorid = jclitem.ColorID;
                            item.Productid = jclitem.ProductStyleID;
                            item.Sizeid = jclitem.SizeID;
                            item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                            jclitemslist.Add(item);
                        }

                        //subtotal
                        decimal subtotal = Math.Round(Convert.ToDecimal(jclitemslist.Sum(i => i.TotalPrice)), 2);
                        jclitemslist.ForEach(i => i.SubTotal = subtotal);
                        //GST
                        decimal GST = Math.Round(Convert.ToDecimal((subtotal * 10) / 100), 2);
                        jclitemslist.ForEach(i => i.GST = GST);
                        //GrandTotal
                        decimal GrandTotal = Math.Round((subtotal + GST), 2);

                        model.QuotematerialViewModel.DepositRequested = GrandTotal;
                        jclitemslist.ForEach(i => i.GrandTotal = GrandTotal);
                        jclitemslist.ForEach(i => i.JobId = Guid.Parse(QuoteInfo.EmployeeJobId.ToString()));
                        model.QuotematerialViewModel.JCLInfo = jclitemslist;
                    }
                    TempData["DataFromUnpaidInvoice"] = model;
                    return View(model);
                }
                return View(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
    