using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FSM.Web.Areas.Employee.Controllers
{
    [Authorize]
    public class JCLController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                       ().DeclaringType);
        [Dependency]
        public IJCLRepository JCLRepo { get; set; }

        [Dependency]
        public IJCLColor_MappingRepository JCLColorRepo { get; set; }

        [Dependency]
        public IJCLProducts_MappingRepository JCLProductRepo { get; set; }

        [Dependency]
        public IJClExtraproduct_mappingRepositories JCLExtraProductRepo { get; set; }


        [Dependency]
        public IJCLSize_MappingRepository JCLSizeRepo { get; set; }

        [Dependency]
        public IinvoicedJCLItemMappingRepository InvoiceJclItemRepo { get; set; }
        [Dependency]   
        public IEmployeeDetailRepository EmpDetailRepo { get; set; }
        // GET: Employee/JCL

        //GET:Employee/JCL/GETJCLList
        /// <summary>
        /// Get All JCL List 
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult GETJCLList()
        {
            try
            {
                using (JCLRepo)
                {
                    var jclList = JCLRepo.GetAll().Where(m => m.IsDelete == false).OrderByDescending(m => m.ModifiedDate).AsEnumerable();
                    string keyword = string.IsNullOrEmpty(Request.QueryString["Keyword"]) ? "" :
                                                (Request.QueryString["Keyword"]);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        jclList = jclList.Where(customer =>
                        (customer.ItemName != null && customer.ItemName.ToLower().Contains(keyword.ToLower())) ||
                        (customer.DefaultQty != null && customer.DefaultQty.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.Price != null && customer.Price.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.Description != null && customer.Description.ToLower().Contains(keyword.ToLower()))
                        );
                    }
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    var jclDetailListViewModel = jclList.Select(m => new JCLDetailListViewModel
                    {
                        ItemName = m.ItemName,
                        DefaultQty = m.DefaultQty,
                        Price = m.Price,
                        Description = m.Description,
                        JCLId = m.JCLId
                    }).ToList();

                    var jclListViewModel = new JCLListViewModel
                    {
                        jCLDetailListViewModel = jclDetailListViewModel,
                        jCLSearchViewModel = new JCLSearchViewModel
                        {
                            Keyword = keyword,
                            PageSize = PageSize
                        }
                    };

                    return View(jclListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST: Employee/JCL/GETJCLList
        /// <summary>
        /// View  JCL lists 
        /// </summary>
        /// <param name="jclSearchViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GETJCLList(JCLSearchViewModel jclSearchViewModel)
        {
            try
            {
                using (JCLRepo)
                {
                    var jclList = JCLRepo.GetAll().Where(m => m.IsDelete == false).OrderByDescending(m => m.ModifiedDate).AsEnumerable();

                    string Keyword = jclSearchViewModel.Keyword;
                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        jclList = jclList.Where(customer =>
                        (customer.ItemName != null && customer.ItemName.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.DefaultQty != null && customer.DefaultQty.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (customer.Price != null && customer.Price.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (customer.Description != null && customer.Description.ToLower().Contains(Keyword.ToLower()))
                        );
                    }

                    var jclDetailListViewModel = jclList.Select(m => new JCLDetailListViewModel
                    {
                        ItemName = m.ItemName,
                        DefaultQty = m.DefaultQty,
                        Price = m.Price,
                        Description = m.Description,
                        JCLId = m.JCLId
                    }).ToList();

                    var jclListViewModel = new JCLListViewModel
                    {
                        jCLDetailListViewModel = jclDetailListViewModel,
                        jCLSearchViewModel = jclSearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed JCL list.");

                    return View(jclListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/JCL/AddJCL
        /// <summary>
        /// Register for new jcl
        /// </summary>
        /// <returns>Model</returns>

        [HttpGet]
        public ActionResult AddJCL()
        {
            JCLDetailViewModel jclDetail = new JCLDetailViewModel();
            jclDetail.ApplyBonus = true;
            jclDetail.JClColorList = null;
            jclDetail.JCLSizeList = null;
            jclDetail.JCLProductsList = null;
            return View(jclDetail);
        }
        //POST:Employee/JCL/AddJCL
        /// <summary>
        /// Record saved
        /// </summary>
        /// <param name="employeeDetailViewModel"></param>
        /// <param name="ProfilePicture"></param>
        /// <param name="SignaturePicture"></param>
        /// <returns>Redirect GETJCLList Page</returns>

        [HttpPost, ValidateInput(false)]
        public ActionResult AddJCL(JCLDetailViewModel jCLDetailViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    using (JCLRepo)
                    {
                        jCLDetailViewModel.JCLId = Guid.NewGuid();
                        jCLDetailViewModel.IsDelete = false;
                        jCLDetailViewModel.CreatedDate = DateTime.Now;
                        jCLDetailViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        jCLDetailViewModel.ModifiedDate = DateTime.Now;

                        // mapping viewmodel to entity
                        CommonMapper<JCLDetailViewModel, JCL> mapper = new CommonMapper<JCLDetailViewModel, JCL>();
                        JCL jclEntity = mapper.Mapper(jCLDetailViewModel);

                        JCLRepo.Add(jclEntity);
                        JCLRepo.Save();
                        TempData["Message"] = 1;
                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " added new JCL.");

                        return Json(new { data = jCLDetailViewModel.JCLId }, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("GETJCLList");
                    }
                }
                return View(jCLDetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddJCLItems(List<JCLColor> JClColorList, List<JCLSize> JCLSizeList, List<JCLProducts> JCLProductsList, List<JCLExtraProducts> JclExtrproductList)
        {
            try
            {
                //JCL Color Save In Table
                foreach (JCLColor color in JClColorList)
                {
                    if (color.ColorId != Guid.Empty)
                    {
                        var ob = JCLColorRepo.FindBy(i => i.ColorId == color.ColorId).FirstOrDefault();
                        ob.ModifiedDate = DateTime.Now;
                        ob.ModifiedBy = Guid.Parse(base.GetUserId);
                        JCLColorRepo.Edit(ob);
                        JCLColorRepo.Save();
                    }
                    else
                    {
                        JCLColor_Mapping ob = new JCLColor_Mapping();
                        ob.ColorId = Guid.NewGuid();
                        ob.ColorName = color.ColorName;
                        ob.JCLId = color.JCLId;
                        ob.CreatedBy = Guid.Parse(base.GetUserId);
                        ob.CreatedDate = DateTime.Now;
                        ob.IsDelete = false;
                        JCLColorRepo.Add(ob);
                        JCLColorRepo.Save();
                    }
                }
                // mapping viewmodel to entity
                //JCL Size Save In Table

                foreach (JCLSize size in JCLSizeList)
                {
                    if (size.SizeId != Guid.Empty)
                    {
                        var ob = JCLSizeRepo.FindBy(i => i.SizeId == size.SizeId).FirstOrDefault();
                        ob.ModifiedDate = DateTime.Now;
                        ob.ModifiedBy = Guid.Parse(base.GetUserId);
                        JCLSizeRepo.Edit(ob);
                        JCLSizeRepo.Save();
                    }
                    else
                    {
                        JCLSize_Mapping ob = new JCLSize_Mapping();
                        ob.SizeId = Guid.NewGuid();
                        ob.JCLId = size.JCLId;
                        ob.Size = size.Size;
                        ob.CreatedBy = Guid.Parse(base.GetUserId);
                        ob.CreatedDate = DateTime.Now;
                        ob.IsDelete = false;
                        JCLSizeRepo.Add(ob);
                        JCLSizeRepo.Save();
                    }
                }

                // mapping viewmodel to entity
                //JCL Products Save In Table

                foreach (JCLProducts product in JCLProductsList)
                {
                    if (product.ProductId != Guid.Empty)
                    {
                        var ob = JCLProductRepo.FindBy(i => i.ProductId == product.ProductId).FirstOrDefault();
                        ob.ModifiedDate = DateTime.Now;
                        ob.ModifiedBy = Guid.Parse(base.GetUserId);
                        JCLProductRepo.Edit(ob);
                        JCLProductRepo.Save();
                    }
                    else
                    {
                        JCLProducts_Mapping ob = new JCLProducts_Mapping();
                        ob.ProductId = Guid.NewGuid();
                        ob.ProductName = product.ProductName;
                        ob.JCLId = product.JCLId;
                        ob.CreatedBy = Guid.Parse(base.GetUserId);
                        ob.CreatedDate = DateTime.Now;
                        ob.IsDelete = false;
                        JCLProductRepo.Add(ob);
                        JCLProductRepo.Save();
                    }
                }

                // mapping viewmodel to entity
                //JCL Extra Products Save In Table

                foreach (JCLExtraProducts product in JclExtrproductList)
                {
                    if (product.ProductId != Guid.Empty)
                    {
                        var ob = JCLExtraProductRepo.FindBy(i => i.ProductId == product.ProductId).FirstOrDefault();
                        ob.ModifiedDate = DateTime.Now;
                        ob.ModifiedBy = Guid.Parse(base.GetUserId);
                        JCLExtraProductRepo.Edit(ob);
                        JCLExtraProductRepo.Save();
                    }
                    else
                    {
                        JCLExtraProducts_Mapping ob = new JCLExtraProducts_Mapping();
                        ob.ProductId = Guid.NewGuid();
                        ob.ProductName = product.ProductName;
                        ob.JCLId = product.JCLId;
                        ob.CreatedBy = Guid.Parse(base.GetUserId);
                        ob.CreatedDate = DateTime.Now;
                        ob.IsDelete = false;
                        JCLExtraProductRepo.Add(ob);
                        JCLExtraProductRepo.Save();
                    }
                }
                // mapping viewmodel to entity
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added new JCL item");
                return Json(new { data = "1" },
                           JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET:Employee/JCL/EditJCL
        /// <summary>
        /// Update Employee Record
        /// </summary>
        /// <param name="JCLId"></param>
        /// <returns>Model</returns>

        [HttpGet]
        public ActionResult EditJCL(Guid JCLId)
        {
            try
            {
                using (JCLRepo)
                {
                    JCL jclEntity = JCLRepo.FindBy(m => m.JCLId == JCLId).FirstOrDefault();       //Get Jcl 
                    var jclColorList = JCLColorRepo.FindBy(m => m.JCLId == JCLId).ToList();       // Get Jcl Color using by Jcl Id
                    var jclSizeList = JCLSizeRepo.FindBy(m => m.JCLId == JCLId).ToList();         // Get Jcl Size using by Jcl Id
                    var jclProductList = JCLProductRepo.FindBy(m => m.JCLId == JCLId).ToList();   // Get Jcl Products using by Jcl Id
                    var jclextraproductList = JCLExtraProductRepo.FindBy(m => m.JCLId == JCLId).ToList();   // Get Jcl Extra Products using by Jcl Id

                    //Mapping color entity to model
                    CommonMapper<JCLColor_Mapping, JCLColor> mapperColor = new CommonMapper<JCLColor_Mapping, JCLColor>();
                    List<JCLColor> colorList = mapperColor.MapToList(jclColorList).ToList();

                    //Mapping size entity to model
                    CommonMapper<JCLSize_Mapping, JCLSize> mapperSize = new CommonMapper<JCLSize_Mapping, JCLSize>();
                    List<JCLSize> sizeList = mapperSize.MapToList(jclSizeList).ToList();

                    //Mapping product entity to model
                    CommonMapper<JCLProducts_Mapping, JCLProducts> mapperProducts = new CommonMapper<JCLProducts_Mapping, JCLProducts>();
                    List<JCLProducts> productList = mapperProducts.MapToList(jclProductList).ToList();
                    //Mapping Extraproduct entity to model
                    CommonMapper<JCLExtraProducts_Mapping, JCLExtraProducts> mapperextraProducts = new CommonMapper<JCLExtraProducts_Mapping, JCLExtraProducts>();
                    List<JCLExtraProducts> extraproductList = mapperextraProducts.MapToList(jclextraproductList).ToList();


                    // mapping entity to viewmodel
                    CommonMapper<JCL, JCLDetailViewModel> mapperJCL = new CommonMapper<JCL, JCLDetailViewModel>();
                    JCLDetailViewModel jclDetailViewModel = mapperJCL.Mapper(jclEntity);

                    var userName = "";
                    if (jclDetailViewModel.ModifiedBy == null)
                    {
                        userName = EmpDetailRepo.FindBy(m => m.EmployeeId == jclDetailViewModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = EmpDetailRepo.FindBy(m => m.EmployeeId == jclDetailViewModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (jclDetailViewModel.ModifiedDate == null)
                    {
                        jclDetailViewModel.CreatedDate = jclDetailViewModel.CreatedDate;
                    }
                    else
                    {
                        jclDetailViewModel.ModifiedDate = jclDetailViewModel.ModifiedDate;
                    }
                    jclDetailViewModel.UserName = userName;
                    jclDetailViewModel.JClColorList = colorList;
                    jclDetailViewModel.JCLSizeList = sizeList;
                    jclDetailViewModel.JCLProductsList = productList;
                    jclDetailViewModel.JCLExtraProductsList = extraproductList;

                    return View(jclDetailViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Employee/JCL/EditJCL
        /// <summary>
        /// Record saved
        /// </summary>
        /// <param name="employeeDetailViewModel"></param>
        /// <param name="ProfilePicture"></param>
        /// <param name="SignaturePicture"></param>
        /// <returns>Redirect GETJCLList Page</returns>

        [HttpPost, ValidateInput(false)]
        public ActionResult EditJCL(JCLDetailViewModel jCLDetailViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (JCLRepo)
                    {
                        jCLDetailViewModel.ModifiedDate = DateTime.Now;
                        jCLDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                        jCLDetailViewModel.IsDelete = false;

                        // mapping viewmodel to entity
                        CommonMapper<JCLDetailViewModel, JCL> mapper = new CommonMapper<JCLDetailViewModel, JCL>();
                        JCL jclEntity = mapper.Mapper(jCLDetailViewModel);

                        JCLRepo.Edit(jclEntity);
                        JCLRepo.Save();

                        TempData["Message"] = 2;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated JCL");

                        return Json(new { data = jCLDetailViewModel.JCLId }, JsonRequestBehavior.AllowGet);
                    }
                }
                return View(jCLDetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Employee/JCL/DeleteJCL
        /// <summary>
        /// Delete Employee Record
        /// </summary>
        /// <param name="JCLId"></param>
        /// <returns>Redirect JCL List</returns>
        public ActionResult DeleteJCL(Guid JCLId)
        {
            try
            {
                //Check Jcl Id Exist Other Job Or Not
                var InvoiceJclMapping = InvoiceJclItemRepo.FindBy(m => m.JCLItemID == JCLId).FirstOrDefault();
                if (InvoiceJclMapping != null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (JCLRepo)
                    {
                        JCL jclEntity = JCLRepo.FindBy(i => i.JCLId == JCLId).FirstOrDefault();
                        JCLRepo.DeleteJcLITem(JCLId);
                        jclEntity.IsDelete = true;
                        jclEntity.ModifiedBy= Guid.Parse(base.GetUserId);
                        jclEntity.ModifiedDate = DateTime.Now;
                        JCLRepo.Edit(jclEntity);
                        JCLRepo.Save();

                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }




        }

        /// <summary>
        /// Delete JCL ITems
        /// </summary>
        /// <param name="JCLId"></param>
        /// <returns></returns>
        public ActionResult DeleteJCLITems(Guid ItemId, string Type)
        {
            try
            {
                string Status = "";
                switch (Type.ToString().ToLower())
                {
                    case "jclcolor":
                        var ob = JCLColorRepo.FindBy(i => i.ColorId == ItemId).FirstOrDefault();
                        ob.IsDelete = true;
                        JCLColorRepo.Edit(ob);
                        JCLColorRepo.Save();
                        Status = "1";
                        break;
                    case "jclsize":
                        var sizeob = JCLSizeRepo.FindBy(i => i.SizeId == ItemId).FirstOrDefault();
                        sizeob.IsDelete = true;
                        JCLSizeRepo.Edit(sizeob);
                        JCLSizeRepo.Save();
                        Status = "1";
                        break;
                    case "jclproduct":
                        var productob = JCLProductRepo.FindBy(i => i.ProductId == ItemId).FirstOrDefault();
                        productob.IsDelete = true;
                        JCLProductRepo.Edit(productob);
                        JCLProductRepo.Save();
                        Status = "1";
                        break;

                    case "jclExtraproduct":
                        var productExtraob = JCLExtraProductRepo.FindBy(i => i.ProductId == ItemId).FirstOrDefault();
                        productExtraob.IsDelete = true;
                        JCLExtraProductRepo.Edit(productExtraob);
                        JCLExtraProductRepo.Save();
                        Status = "1";
                        break;
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted JCL items");
                return Json(new { status = Status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Employee/JCL/GETJCLItemList
        /// <summary>
        /// Get Jcl Item
        /// </summary>
        /// <param name=""></param>
        /// <returns>return json</returns>
        public ActionResult GETJCLItemList()
        {
            try
            {
                using (JCLRepo)
                {
                    JCLItems ob = new JCLItems();
                    ob.JCLItemList = JCLRepo.GetAll().Where(m => m.IsDelete == false).Select(m => new SelectListItem()
                    {
                        Text = m.ItemName,
                        Value = m.JCLId.ToString()
                    }).OrderBy(k => k.Text).ToList();

                    ob.JCLInfo = null;
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(ob);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of JCL items.");

                    return Json(new { JCLList = jsonSerialiser.Serialize(ob.JCLItemList), Sizlist = jsonSerialiser.Serialize(ob.jclSizeList), Productlist = jsonSerialiser.Serialize(ob.jclProductlist), Colorlist = jsonSerialiser.Serialize(ob.jclcolorlist), JclList = jsonSerialiser.Serialize(ob.JCLInfo) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}