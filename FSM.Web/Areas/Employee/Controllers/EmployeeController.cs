using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSM.Core.Entities;
using FSM.Infrastructure;
using Microsoft.Practices.Unity;
using FSM.Core.Interface;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using FSM.Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.ComponentModel.DataAnnotations;
using FSM.Web.FSMConstant;
using System.Web.Routing;
using System.Text;
using System.Text.RegularExpressions;
using FSM.Core.ViewModels;
using log4net;
using Rotativa;
using Rotativa.Options;
using FSM.Infrastructure.Repository;
using static FSM.Web.FSMConstant.Constant;
using System.Web.UI.WebControls;
using System.Globalization;

namespace FSM.Web.Areas.Employee.Controllers
{
    [Authorize]
    public class EmployeeController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                         ().DeclaringType);

        // GET: Employee/Employee
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [Dependency]
        public IEmployeeDetailRepository Employee { get; set; }

        [Dependency]
        public IEmployeeJobRepository EmployeeJobRepo { get; set; }

        [Dependency]
        public IEmployeeDetailTempRepository EmployeeDetailTempRepo { get; set; }

        [Dependency]
        public IPublicHolidayRepository IHoliday { get; set; }


        [Dependency]
        public IAspNetUserRolesRepository UserRoles { get; set; }

        [Dependency]
        public IAspNetUsersRepository NetUsers { get; set; }
        [Dependency]
        public IAspNetUsersIsDeleteRepository NetUsersIsDeleteRepo { get; set; }

        [Dependency]
        public IEmployeeRatesRepository EmployeeRates { get; set; }

        [Dependency]
        public IVacationRepository VacationRepo { get; set; }
        [Dependency]
        public IAspNetRolesRepository AspNetRolesRepo { get; set; }
        [Dependency]
        public IRateCategoryRepository RateCategoryRepo { get; set; }
        [Dependency]
        public IRateSubCategoryRepository RateSubCategoryRepo { get; set; }

        [Dependency]
        public IViewEmployeeRatesRepository ViewEmployeeRates { get; set; }
        [Dependency]
        public IWorkTypeRepository WorktypeRepo { get; set; }

        [Dependency]
        public IEmployeeWorkTypeRepository EmpWorktypeRepo { get; set; }


        [Dependency]
        public IUserTimeSheetRepository TimeSheetRepository { get; set; }

        [Dependency
           ]
        public IEmployeeJobRepository EmployeeJobRep { get; set; }

        [Dependency
        ]
        public iNvoiceRepository EmployeeInvoiceRep { get; set; }

        [Dependency]
        public ILogRepository logRepo { get; set; }
        [Dependency]
        public IRoastedOffRepository RoastefOffRepo { get; set; }
        [Dependency]
        public IRoastedOffWeekMappingRepository RoastedOffWeekMappingRepo { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        //GET:Employee/Employee/EmployeeDetails
        /// <summary>
        /// Show all Employee Details
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult EmployeeDetails()
        {
            try
            {
                using (Employee)
                {
                    var employeerList = Employee.GetEmployeeDetail().AsEnumerable();
                    employeerList = employeerList.Where(m => m.IsDelete == false).AsEnumerable();

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    var empDetailListViewModel = employeerList.Select(m => new EmpDetailListViewModel
                    {
                        Name = m.Name,
                        EmailAddress = m.EmailAddress,
                        Mobile = m.Mobile,
                        UserName = m.UserName,
                        EID = int.Parse(m.EID),
                        Role = m.Role,
                        EmployeeId = m.EmployeeId,
                        CreatedDate = m.CreatedDate
                    }).OrderByDescending(m => m.CreatedDate).ToList();

                    var employeeDetailListViewModel = new EmployeeDetailListViewModel
                    {
                        EmployeeDetailList = empDetailListViewModel,
                        EmployeeDetailInfo = new EmployeeDetailSearchViewModel
                        {
                            FirstName = "",
                            GetUserRoles = this.GetUserRoles,
                            PageSize = PageSize
                        }
                    };

                    return View(employeeDetailListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Employee/EmployeeDetails
        /// <summary>
        /// Search Record 
        /// </summary>
        /// <param name="employeeDetailSearchViewModel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult EmployeeDetails(EmployeeDetailSearchViewModel employeeDetailSearchViewModel)
        {
            try
            {
                using (Employee)
                {
                    employeeDetailSearchViewModel.GetUserRoles = this.GetUserRoles;

                    if (employeeDetailSearchViewModel.FirstName == null)
                        employeeDetailSearchViewModel.FirstName = "";

                    var employeerList = Employee.GetEmployeeDetail(employeeDetailSearchViewModel.FirstName).AsEnumerable();
                    employeerList = employeerList.Where(m => m.IsDelete == false).AsEnumerable();

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    employeeDetailSearchViewModel.PageSize = PageSize;

                    var empDetailListViewModel = employeerList.Select(m => new EmpDetailListViewModel
                    {
                        Name = m.Name,
                        EmailAddress = m.EmailAddress,
                        Mobile = m.Mobile,
                        UserName = m.UserName,
                        EID = int.Parse(m.EID),
                        Role = m.Role,
                        EmployeeId = m.EmployeeId,
                        CreatedDate = m.CreatedDate
                    }).OrderByDescending(m => m.CreatedDate).ToList();

                    var employeeDetailListViewModel = new EmployeeDetailListViewModel
                    {
                        EmployeeDetailList = empDetailListViewModel,
                        EmployeeDetailInfo = employeeDetailSearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed employee list.");

                    return View(employeeDetailListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/Employee/AddEmployee
        /// <summary>
        /// Register for new employee
        /// </summary>
        /// <returns>Model</returns>

        [HttpGet]
        public ActionResult AddEmployee()
        {
            EmployeeDetailViewModel employeeDetail = new EmployeeDetailViewModel();
            employeeDetail.GetUserRoles = this.GetUserRoles;
            var userName = "";
            if (employeeDetail.ModifiedBy == null)
            {
                userName = Employee.FindBy(m => m.EmployeeId == employeeDetail.CreatedBy).Select(m => m.UserName).FirstOrDefault();

            }
            else
            {
                userName = Employee.FindBy(m => m.EmployeeId == employeeDetail.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
            }
            if (employeeDetail.ModifiedDate == null)
            {
                employeeDetail.CreatedDate = employeeDetail.CreatedDate;
            }
            else
            {
                employeeDetail.ModifiedDate = employeeDetail.ModifiedDate;
            }
            employeeDetail.UserName = userName;
            //employeeDetail.CreatedDate =CreatedBy;
            employeeDetail.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            //Get Rate Category List
            employeeDetail.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
             new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

            //Get Rate sub Category List
            employeeDetail.RateSubCategoryList = RateSubCategoryRepo.GetAll().Select(m =>
             new SelectListItem { Text = m.SubCategoryName, Value = m.SubCategoryId.ToString() }).ToList();

            employeeDetail.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
            {
                Text = x.WorkTypeName,
                Value = x.Value.ToString()
            }).ToList();
            return View(employeeDetail);
        }

        //POST:Employee/Employee/AddEmployee
        /// <summary>
        /// Record saved
        /// </summary>
        /// <param name="employeeDetailViewModel"></param>
        /// <param name="ProfilePicture"></param>
        /// <param name="SignaturePicture"></param>
        /// <returns>Redirect EmployeeDetailsPage</returns>

        [HttpPost]
        public async Task<ActionResult> AddEmployee(EmployeeDetailViewModel employeeDetailViewModel, HttpPostedFileBase ProfilePicture, HttpPostedFileBase SignaturePicture)
        {
            try
            {
                if (employeeDetailViewModel.Role == new Guid("cde8f045-239f-4531-aa17-d8aecb0fa732"))
                {
                    if (employeeDetailViewModel.HourlyRate == null)
                    {
                        ModelState.AddModelError("HourlyRate", "Hourly rate required");
                    }
                }
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = employeeDetailViewModel.UserName, Email = employeeDetailViewModel.Email };
                    var result = await UserManager.CreateAsync(user, employeeDetailViewModel.Password);
                    if (result.Succeeded)
                    {
                        using (Employee)
                        {
                            employeeDetailViewModel.EmployeeId = user.Id;
                            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/"));
                            if (di.Exists)
                            {
                                di.CreateSubdirectory(user.Id);
                            }
                            else
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/"));
                                DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("~/EmployeeImages/"));
                                di.CreateSubdirectory(user.Id);
                            }


                            if (employeeDetailViewModel.ProfilePicture != null)
                            {
                                //var fileName = Path.GetFileName(file.FileName);
                                string profilepic = Path.GetFileNameWithoutExtension(ProfilePicture.FileName);
                                string extension = Path.GetExtension(ProfilePicture.FileName);
                                var pathprofile = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                                  profilepic + "ProfilePic" + extension);

                                ProfilePicture.SaveAs(pathprofile);

                                employeeDetailViewModel.ProfilePicture = profilepic + extension;
                            }



                            if (employeeDetailViewModel.SignaturePicture != null)
                            {
                                string signaturepic = Path.GetFileNameWithoutExtension(SignaturePicture.FileName);
                                string extension = Path.GetExtension(SignaturePicture.FileName);
                                var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                               signaturepic + "SignaturePic" + extension);

                                SignaturePicture.SaveAs(pathsign);

                                employeeDetailViewModel.SignaturePicture = signaturepic + extension;
                            }
                            if (employeeDetailViewModel.DrivingLicense != null)
                            {
                                string drivingLicense = Path.GetFileNameWithoutExtension(employeeDetailViewModel.DrivingLicense.FileName);
                                string extension = Path.GetExtension(employeeDetailViewModel.DrivingLicense.FileName);
                                var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                               drivingLicense + "DrivingLicense" + extension);

                                employeeDetailViewModel.DrivingLicense.SaveAs(pathsign);

                                employeeDetailViewModel.DrivingLicenseImg = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                                            drivingLicense + "DrivingLicense" + extension);
                            }
                            if (employeeDetailViewModel.BankDetail != null)
                            {
                                string bankDetail = Path.GetFileNameWithoutExtension(employeeDetailViewModel.BankDetail.FileName);
                                string extension = Path.GetExtension(employeeDetailViewModel.BankDetail.FileName);
                                var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                               bankDetail + "BankDetail" + extension);

                                employeeDetailViewModel.BankDetail.SaveAs(pathsign);

                                employeeDetailViewModel.BankDetailDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                                        bankDetail + "BankDetail" + extension);
                            }
                            if (employeeDetailViewModel.Insurance != null)
                            {
                                string insurance = Path.GetFileNameWithoutExtension(employeeDetailViewModel.Insurance.FileName);
                                string extension = Path.GetExtension(employeeDetailViewModel.Insurance.FileName);
                                var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                               insurance + "Insurance" + extension);

                                employeeDetailViewModel.Insurance.SaveAs(pathsign);

                                employeeDetailViewModel.InsuranceDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                                        insurance + "Insurance" + extension);
                            }

                            string maxEmployeeId = Employee.GetMaxEmployeeId();
                            if (string.IsNullOrEmpty(maxEmployeeId))
                            {
                                employeeDetailViewModel.EID = "1";
                            }
                            else
                            {
                                employeeDetailViewModel.EID = Convert.ToString(Convert.ToInt16(maxEmployeeId) + 1);
                            }
                            Guid Role = Guid.Parse("31cf918d-b8fe-4490-b2d7-27324bfe89b4");
                            if (employeeDetailViewModel.Role == Role)
                            {
                                int? MaxOTRWoOrderNo = Employee.FindBy(m => m.Role == Role && m.IsDelete == false && m.IsActive == true).Select(m => m.OTRW_Order).Max();
                                employeeDetailViewModel.OTRW_Order = MaxOTRWoOrderNo + 1;
                            }

                            employeeDetailViewModel.IsDelete = false;
                            employeeDetailViewModel.CreatedDate = DateTime.Now;
                            employeeDetailViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                            // mapping viewmodel to entity
                            CommonMapper<EmployeeDetailViewModel, EmployeeDetail> mapper = new CommonMapper<EmployeeDetailViewModel, EmployeeDetail>();
                            EmployeeDetail employeeDetail = mapper.Mapper(employeeDetailViewModel);
                            AspNetUserRoles aspNetUserRoles = new AspNetUserRoles();

                            aspNetUserRoles.UserId = (employeeDetailViewModel.EmployeeId).ToString();
                            aspNetUserRoles.RoleId = (employeeDetailViewModel.Role).ToString();

                            Employee.Add(employeeDetail);
                            Employee.Save();

                            UserRoles.Add(aspNetUserRoles);
                            UserRoles.Save();

                            if (employeeDetailViewModel.WorkTypeId != null)
                            {

                                foreach (var key in employeeDetailViewModel.WorkTypeId)
                                {
                                    EmployeeWorkType empWorkType = new EmployeeWorkType();
                                    empWorkType.Id = Guid.NewGuid();
                                    empWorkType.EmployeeId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                                    empWorkType.WorkType = Convert.ToInt32(key);

                                    EmpWorktypeRepo.Add(empWorkType);
                                    EmpWorktypeRepo.Save();
                                }
                            }
                            TempData["Message"] = 1;
                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added new employee " + employeeDetailViewModel.UserName);
                            return RedirectToAction("EmployeeDetails", new { id = employeeDetailViewModel.EmployeeId.ToString() });
                        }
                    }
                }
                employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                //Get Rate Category List
                employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                //Get Rate sub Category List
                employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.SubCategoryName, Value = m.SubCategoryId.ToString() }).ToList();


                employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.WorkTypeName,
                    Value = x.Value.ToString()
                }).ToList();
                employeeDetailViewModel.GetUserRoles = this.GetUserRoles;
                ViewBag.Status = "0";

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added new employee " + employeeDetailViewModel.UserName);

                return View(employeeDetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/Employee/EditEmployee
        /// <summary>
        /// Update Employee Record
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns>Model</returns>

        [HttpGet]
        public ActionResult EditEmployee(Guid EmployeeId)
        {
            try
            {
                using (Employee)
                {

                    EmployeeDetail employeedetailInfo = Employee.FindBy(m => m.EmployeeId == EmployeeId).FirstOrDefault();
                    employeedetailInfo.GetUserRoles = this.GetUserRoles;
                    var EmployeeWorkTypeId = EmpWorktypeRepo.FindBy(m => m.EmployeeId == EmployeeId).Select(m => m.WorkType).ToList();

                    // mapping entity to viewmodel
                    CommonMapper<EmployeeDetail, EmployeeDetailViewModel> mapper = new CommonMapper<EmployeeDetail, EmployeeDetailViewModel>();
                    EmployeeDetailViewModel employeeDetailViewModel = mapper.Mapper(employeedetailInfo);
                    employeeDetailViewModel.ProfilePictureTemp = employeeDetailViewModel.ProfilePicture;
                    employeeDetailViewModel.SignaturePictureTemp = employeeDetailViewModel.SignaturePicture;
                    employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                    //Get Rate Category List
                    employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                     new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                    //Get Rate sub Category List


                    employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.FindBy(m => m.CategoryId == employeeDetailViewModel.CategoryId).Select(m => new SelectListItem()
                    {
                        Text = m.SubCategoryName,
                        Value = m.SubCategoryId.ToString()
                    }).OrderBy(m => m.Text).ToList();

                    employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.WorkTypeName,
                        Value = x.Value.ToString()
                    }).ToList();
                    employeeDetailViewModel.WorkTypeId = EmployeeWorkTypeId;
                    var userName = "";
                    if (employeeDetailViewModel.ModifiedBy == null)
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == employeeDetailViewModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == employeeDetailViewModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (employeeDetailViewModel.ModifiedDate == null)
                    {
                        employeeDetailViewModel.CreatedDate = employeeDetailViewModel.CreatedDate;
                    }
                    else
                    {
                        employeeDetailViewModel.ModifiedDate = employeeDetailViewModel.ModifiedDate;
                    }
                    employeeDetailViewModel.ModifyUserName = userName;
                    return View(employeeDetailViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Employee/EditEmployee
        /// <summary>
        /// Update Record
        /// </summary>
        /// <param name="employeeDetailViewModel"></param>
        /// <returns>Redirect EmployeeDetails</returns>

        [HttpPost]
        public async Task<ActionResult> EditEmployee(EmployeeDetailViewModel employeeDetailViewModel)
        {
            try
            {
                if (employeeDetailViewModel.Role == new Guid("cde8f045-239f-4531-aa17-d8aecb0fa732"))
                {
                    if (employeeDetailViewModel.HourlyRate == null)
                    {
                        ModelState.AddModelError("HourlyRate", "Hourly rate required");
                    }
                }

                // checks added when user don't want to update pics data and also bind pics in case of bad model
                if (employeeDetailViewModel.SignatureDoc != null)
                {
                    employeeDetailViewModel.SignaturePicture = employeeDetailViewModel.SignatureDoc.FileName;
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/"), employeeDetailViewModel.SignaturePicture);
                    employeeDetailViewModel.SignatureDoc.SaveAs(pathsign);
                }
                if (employeeDetailViewModel.ProfileDoc != null)
                {
                    employeeDetailViewModel.ProfilePicture = employeeDetailViewModel.ProfileDoc.FileName;
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/"), employeeDetailViewModel.ProfilePicture);
                    employeeDetailViewModel.ProfileDoc.SaveAs(pathsign);
                }
                if (employeeDetailViewModel.DrivingLicense != null)
                {
                    string drivingLicense = Path.GetFileNameWithoutExtension(employeeDetailViewModel.DrivingLicense.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.DrivingLicense.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   drivingLicense + "DrivingLicense" + extension);

                    employeeDetailViewModel.DrivingLicense.SaveAs(pathsign);

                    employeeDetailViewModel.DrivingLicenseImg = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                                drivingLicense + "DrivingLicense" + extension);
                }
                if (employeeDetailViewModel.BankDetail != null)
                {
                    string bankDetail = Path.GetFileNameWithoutExtension(employeeDetailViewModel.BankDetail.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.BankDetail.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   bankDetail + "BankDetail" + extension);

                    employeeDetailViewModel.BankDetail.SaveAs(pathsign);

                    employeeDetailViewModel.BankDetailDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                            bankDetail + "BankDetail" + extension);
                }
                if (employeeDetailViewModel.Insurance != null)
                {
                    string insurance = Path.GetFileNameWithoutExtension(employeeDetailViewModel.Insurance.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.Insurance.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   insurance + "Insurance" + extension);

                    employeeDetailViewModel.Insurance.SaveAs(pathsign);

                    employeeDetailViewModel.InsuranceDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                            insurance + "Insurance" + extension);
                }

                if (ModelState.IsValid)
                {
                    using (Employee)
                    {
                        employeeDetailViewModel.ModifiedDate = DateTime.Now;
                        employeeDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                        //Update Also in AspnetUsers
                        AspNetUsers aspnet = NetUsers.FindBy(m => m.Id == employeeDetailViewModel.EmployeeId).FirstOrDefault();
                        if (aspnet != null)
                        {
                            //String hashedNewPassword = UserManager.PasswordHasher.HashPassword(employeeDetailViewModel.Password);
                            aspnet.Email = employeeDetailViewModel.Email;
                            aspnet.UserName = employeeDetailViewModel.UserName;
                            //aspnet.PasswordHash = hashedNewPassword;

                            NetUsers.Edit(aspnet);
                            NetUsers.Save();
                        }
                        else
                        {
                            var user = new ApplicationUser { Id = employeeDetailViewModel.EmployeeId, UserName = employeeDetailViewModel.UserName, Email = employeeDetailViewModel.Email };
                            var result1 = await UserManager.CreateAsync(user, employeeDetailViewModel.Password);
                        }

                        AspNetUserRoles aspNetUserRoles = UserRoles.FindBy(m => m.UserId == employeeDetailViewModel.EmployeeId).FirstOrDefault();
                        if (aspNetUserRoles != null)
                        {
                            aspNetUserRoles.RoleId = (employeeDetailViewModel.Role).ToString();

                            UserRoles.Edit(aspNetUserRoles);
                            UserRoles.Save();
                        }
                        else
                        {
                            aspNetUserRoles = new AspNetUserRoles();
                            aspNetUserRoles.UserId = employeeDetailViewModel.EmployeeId;
                            aspNetUserRoles.RoleId = employeeDetailViewModel.Role.ToString();

                            UserRoles.Add(aspNetUserRoles);
                            UserRoles.Save();
                        }

                        Guid Role = Guid.Parse("31cf918d-b8fe-4490-b2d7-27324bfe89b4");
                        if (employeeDetailViewModel.Role == Role && employeeDetailViewModel.OTRW_Order == null)
                        {
                            int? MaxOTRWoOrderNo = Employee.FindBy(m => m.Role == Role && m.IsDelete == false && m.IsActive == true).Select(m => m.OTRW_Order).Max();
                            employeeDetailViewModel.OTRW_Order = MaxOTRWoOrderNo + 1;
                        }


                        // mapping viewmodel to entity
                        CommonMapper<EmployeeDetailViewModel, EmployeeDetail> mapper = new CommonMapper<EmployeeDetailViewModel, EmployeeDetail>();
                        EmployeeDetail employeeDetailInfo = mapper.Mapper(employeeDetailViewModel);
                        Employee.Edit(employeeDetailInfo);
                        Employee.Save();
                        string code = await UserManager.GeneratePasswordResetTokenAsync(employeeDetailViewModel.EmployeeId);
                        var result = await UserManager.ResetPasswordAsync(employeeDetailViewModel.EmployeeId, code, employeeDetailViewModel.Password);


                        //Delete Work Type Assign Already
                        Guid EmpId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                        var EmployeeWorkType = EmpWorktypeRepo.FindBy(m => m.EmployeeId == EmpId).ToList();
                        if (EmployeeWorkType != null)
                        {
                            foreach (var assign in EmployeeWorkType)
                            {
                                EmpWorktypeRepo.Delete(assign);
                                EmpWorktypeRepo.Save();
                            }
                        }

                        if (employeeDetailViewModel.WorkTypeId != null)
                        {

                            foreach (var key in employeeDetailViewModel.WorkTypeId)
                            {
                                EmployeeWorkType empWorkType = new EmployeeWorkType();
                                empWorkType.Id = Guid.NewGuid();
                                empWorkType.EmployeeId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                                empWorkType.WorkType = Convert.ToInt32(key);

                                EmpWorktypeRepo.Add(empWorkType);
                                EmpWorktypeRepo.Save();
                            }
                        }

                        //Update Also in AspnetUsers
                        //AspNetUsers aspnet = NetUsers.FindBy(m => m.Id == employeeDetailViewModel.EmployeeId).FirstOrDefault();
                        //if (aspnet != null)
                        //{
                        //    //String hashedNewPassword = UserManager.PasswordHasher.HashPassword(employeeDetailViewModel.Password);
                        //    aspnet.Email = employeeDetailViewModel.Email;
                        //    aspnet.UserName = employeeDetailViewModel.UserName;
                        //    //aspnet.PasswordHash = hashedNewPassword;

                        //    NetUsers.Edit(aspnet);
                        //    NetUsers.Save();
                        //}
                        //else
                        //{
                        //    var user = new ApplicationUser {Id=employeeDetailViewModel.EmployeeId, UserName = employeeDetailViewModel.UserName, Email = employeeDetailViewModel.Email };
                        //    var result1 = await UserManager.CreateAsync(user, employeeDetailViewModel.Password);
                        //}

                        TempData["Message"] = 2;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated an existing employee " + employeeDetailViewModel.UserName);


                        return RedirectToAction("EmployeeDetails", new { id = employeeDetailViewModel.EmployeeId.ToString() });
                    }
                }
                employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                //Get Rate Category List
                employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                //Get Rate sub Category List
                employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.SubCategoryName, Value = m.SubCategoryId.ToString() }).ToList();
                employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.WorkTypeName,
                    Value = x.Value.ToString()
                }).ToList();
                employeeDetailViewModel.GetUserRoles = this.GetUserRoles;
                ViewBag.Status = "0";

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " updated an existing employee " + employeeDetailViewModel.UserName);

                return View(employeeDetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeProfile(Guid EmployeeId)
        {
            try
            {
                using (Employee)
                {
                    EmployeeDetail employeedetailInfo = Employee.FindBy(m => m.EmployeeId == EmployeeId).FirstOrDefault();
                    employeedetailInfo.GetUserRoles = this.GetUserRoles;
                    var EmployeeWorkTypeId = EmpWorktypeRepo.FindBy(m => m.EmployeeId == EmployeeId).Select(m => m.WorkType).ToList();

                    // mapping entity to viewmodel
                    CommonMapper<EmployeeDetail, EmployeeDetailViewModel> mapper = new CommonMapper<EmployeeDetail, EmployeeDetailViewModel>();
                    EmployeeDetailViewModel employeeDetailViewModel = mapper.Mapper(employeedetailInfo);
                    employeeDetailViewModel.ProfilePictureTemp = employeeDetailViewModel.ProfilePicture;
                    employeeDetailViewModel.SignaturePictureTemp = employeeDetailViewModel.SignaturePicture;
                    employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                    employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.WorkTypeName,
                        Value = x.Value.ToString()
                    }).ToList();
                    employeeDetailViewModel.WorkTypeId = EmployeeWorkTypeId;

                    //Get Rate Category List
                    employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                     new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                    //Get Rate sub Category List
                    employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.FindBy(m => m.CategoryId == employeeDetailViewModel.CategoryId).Select(m => new SelectListItem()
                    {
                        Text = m.SubCategoryName,
                        Value = m.SubCategoryId.ToString()
                    }).OrderBy(m => m.Text).ToList();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed profile of " + employeedetailInfo.UserName);

                    return View(employeeDetailViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Employee/EditEmployee
        /// <summary>
        /// Update Record
        /// </summary>
        /// <param name="employeeDetailViewModel"></param>
        /// <returns>Redirect EmployeeDetails</returns>

        [HttpPost]
        public async Task<ActionResult> EmployeeProfile(EmployeeDetailViewModel employeeDetailViewModel)
        {
            try
            {
                if (employeeDetailViewModel.Role == new Guid("cde8f045-239f-4531-aa17-d8aecb0fa732"))
                {
                    if (employeeDetailViewModel.HourlyRate == null)
                    {
                        ModelState.AddModelError("HourlyRate", "Hourly rate required");
                    }
                }

                // checks added when user don't want to update pics data and also bind pics in case of bad model
                if (employeeDetailViewModel.SignatureDoc != null)
                {
                    employeeDetailViewModel.SignaturePicture = employeeDetailViewModel.SignatureDoc.FileName;
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/"), employeeDetailViewModel.SignaturePicture);
                    employeeDetailViewModel.SignatureDoc.SaveAs(pathsign);
                }
                if (employeeDetailViewModel.ProfileDoc != null)
                {
                    employeeDetailViewModel.ProfilePicture = employeeDetailViewModel.ProfileDoc.FileName;
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/"), employeeDetailViewModel.ProfilePicture);
                    employeeDetailViewModel.ProfileDoc.SaveAs(pathsign);
                }
                if (employeeDetailViewModel.DrivingLicense != null)
                {
                    string drivingLicense = Path.GetFileNameWithoutExtension(employeeDetailViewModel.DrivingLicense.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.DrivingLicense.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   drivingLicense + "DrivingLicense" + extension);

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    if (!di.Exists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    }

                    employeeDetailViewModel.DrivingLicense.SaveAs(pathsign);

                    employeeDetailViewModel.DrivingLicenseImg = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                                drivingLicense + "DrivingLicense" + extension);
                }
                if (employeeDetailViewModel.BankDetail != null)
                {
                    string bankDetail = Path.GetFileNameWithoutExtension(employeeDetailViewModel.BankDetail.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.BankDetail.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   bankDetail + "BankDetail" + extension);

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    if (!di.Exists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    }

                    employeeDetailViewModel.BankDetail.SaveAs(pathsign);

                    employeeDetailViewModel.BankDetailDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                            bankDetail + "BankDetail" + extension);
                }
                if (employeeDetailViewModel.Insurance != null)
                {
                    string insurance = Path.GetFileNameWithoutExtension(employeeDetailViewModel.Insurance.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.Insurance.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   insurance + "Insurance" + extension);

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    if (!di.Exists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    }

                    employeeDetailViewModel.Insurance.SaveAs(pathsign);

                    employeeDetailViewModel.InsuranceDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                            insurance + "Insurance" + extension);
                }

                if (ModelState.IsValid)
                {
                    //using (Employee)
                    //{
                    employeeDetailViewModel.ModifiedDate = DateTime.Now;
                    employeeDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    AspNetUserRoles aspNetUserRoles = UserRoles.FindBy(m => m.UserId == employeeDetailViewModel.EmployeeId).FirstOrDefault();
                    aspNetUserRoles.RoleId = (employeeDetailViewModel.Role).ToString();

                    UserRoles.Edit(aspNetUserRoles);
                    UserRoles.Save();

                    // mapping viewmodel to entity
                    CommonMapper<EmployeeDetailViewModel, EmployeeDetail> mapper = new CommonMapper<EmployeeDetailViewModel, EmployeeDetail>();
                    EmployeeDetail employeeDetailInfo = mapper.Mapper(employeeDetailViewModel);
                    Employee.Edit(employeeDetailInfo);
                    Employee.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated profile of " + employeeDetailViewModel.UserName);

                    string code = await UserManager.GeneratePasswordResetTokenAsync(employeeDetailViewModel.EmployeeId);
                    var result = await UserManager.ResetPasswordAsync(employeeDetailViewModel.EmployeeId, code, employeeDetailViewModel.Password);

                    //Delete Work Type Assign Already
                    Guid EmpId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                    var EmployeeWorkType = EmpWorktypeRepo.FindBy(m => m.EmployeeId == EmpId).ToList();
                    if (EmployeeWorkType != null)
                    {
                        foreach (var assign in EmployeeWorkType)
                        {
                            EmpWorktypeRepo.Delete(assign);
                            EmpWorktypeRepo.Save();
                        }
                    }

                    if (employeeDetailViewModel.WorkTypeId != null)
                    {

                        foreach (var key in employeeDetailViewModel.WorkTypeId)
                        {
                            EmployeeWorkType empWorkType = new EmployeeWorkType();
                            empWorkType.Id = Guid.NewGuid();
                            empWorkType.EmployeeId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                            empWorkType.WorkType = Convert.ToInt32(key);

                            EmpWorktypeRepo.Add(empWorkType);
                            EmpWorktypeRepo.Save();
                        }
                    }
                    return RedirectToAction("Dashboard", "Dashboard", new { @area = "admin" });
                    //}
                }
                employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                //Get Rate Category List
                employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                //Get Rate sub Category List
                employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.SubCategoryName, Value = m.SubCategoryId.ToString() }).ToList();

                employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.WorkTypeName,
                    Value = x.Value.ToString()
                }).ToList();
                employeeDetailViewModel.GetUserRoles = this.GetUserRoles;
                ViewBag.Status = "0";
                return View(employeeDetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,OPERATIONS")]
        public ActionResult ApproveEmployee(Guid EmployeeTempId)
        {
            try
            {
                using (Employee)
                {
                    EmployeeDetailTemp employeedetailInfo = EmployeeDetailTempRepo.FindBy(m => m.Id == EmployeeTempId).FirstOrDefault();
                    employeedetailInfo.GetUserRoles = this.GetUserRoles;
                    var EmployeeWorkTypeId = EmpWorktypeRepo.FindBy(m => m.EmployeeId == EmployeeTempId).Select(m => m.WorkType).ToList();

                    // mapping entity to viewmodel
                    CommonMapper<EmployeeDetailTemp, EmployeeDetailViewModel> mapper = new CommonMapper<EmployeeDetailTemp, EmployeeDetailViewModel>();
                    EmployeeDetailViewModel employeeDetailViewModel = mapper.Mapper(employeedetailInfo);
                    employeeDetailViewModel.ProfilePictureTemp = employeeDetailViewModel.ProfilePicture;
                    employeeDetailViewModel.SignaturePictureTemp = employeeDetailViewModel.SignaturePicture;
                    employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                    employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.WorkTypeName,
                        Value = x.Value.ToString()
                    }).ToList();
                    employeeDetailViewModel.WorkTypeId = EmployeeWorkTypeId;

                    //Get Rate Category List
                    employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                     new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                    //Get Rate sub Category List
                    employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.FindBy(m => m.CategoryId == employeeDetailViewModel.CategoryId).Select(m => new SelectListItem()
                    {
                        Text = m.SubCategoryName,
                        Value = m.SubCategoryId.ToString()
                    }).OrderBy(m => m.Text).ToList();




                    return View(employeeDetailViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Employee/EditEmployee
        /// <summary>
        /// Update Record
        /// </summary>
        /// <param name="employeeDetailViewModel"></param>
        /// <returns>Redirect EmployeeDetails</returns>

        [HttpPost]
        [Authorize(Roles = "Admin,OPERATIONS")]
        public async Task<ActionResult> ApproveEmployee(EmployeeDetailViewModel employeeDetailViewModel)
        {
            try
            {
                if (employeeDetailViewModel.Role == new Guid("cde8f045-239f-4531-aa17-d8aecb0fa732"))
                {
                    if (employeeDetailViewModel.HourlyRate == null)
                    {
                        ModelState.AddModelError("HourlyRate", "Hourly rate required");
                    }
                }

                // checks added when user don't want to update pics data and also bind pics in case of bad model
                if (employeeDetailViewModel.SignatureDoc != null)
                {
                    employeeDetailViewModel.SignaturePicture = employeeDetailViewModel.SignatureDoc.FileName;
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/"), employeeDetailViewModel.SignaturePicture);
                    employeeDetailViewModel.SignatureDoc.SaveAs(pathsign);
                }
                if (employeeDetailViewModel.ProfileDoc != null)
                {
                    employeeDetailViewModel.ProfilePicture = employeeDetailViewModel.ProfileDoc.FileName;
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/"), employeeDetailViewModel.ProfilePicture);
                    employeeDetailViewModel.ProfileDoc.SaveAs(pathsign);
                }
                if (employeeDetailViewModel.DrivingLicense != null)
                {
                    string drivingLicense = Path.GetFileNameWithoutExtension(employeeDetailViewModel.DrivingLicense.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.DrivingLicense.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   drivingLicense + "DrivingLicense" + extension);

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    if (!di.Exists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    }

                    employeeDetailViewModel.DrivingLicense.SaveAs(pathsign);

                    employeeDetailViewModel.DrivingLicenseImg = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                                drivingLicense + "DrivingLicense" + extension);
                }
                if (employeeDetailViewModel.BankDetail != null)
                {
                    string bankDetail = Path.GetFileNameWithoutExtension(employeeDetailViewModel.BankDetail.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.BankDetail.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   bankDetail + "BankDetail" + extension);

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    if (!di.Exists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    }

                    employeeDetailViewModel.BankDetail.SaveAs(pathsign);

                    employeeDetailViewModel.BankDetailDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                            bankDetail + "BankDetail" + extension);
                }
                if (employeeDetailViewModel.Insurance != null)
                {
                    string insurance = Path.GetFileNameWithoutExtension(employeeDetailViewModel.Insurance.FileName);
                    string extension = Path.GetExtension(employeeDetailViewModel.Insurance.FileName);
                    var pathsign = Path.Combine(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId),
                                   insurance + "Insurance" + extension);

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    if (!di.Exists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/EmployeeImages/" + employeeDetailViewModel.EmployeeId));
                    }


                    employeeDetailViewModel.Insurance.SaveAs(pathsign);

                    employeeDetailViewModel.InsuranceDoc = Path.Combine(employeeDetailViewModel.EmployeeId,
                                                            insurance + "Insurance" + extension);
                }

                if (ModelState.IsValid)
                {
                    //using (Employee)
                    //{
                    employeeDetailViewModel.ModifiedDate = DateTime.Now;
                    employeeDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    AspNetUserRoles aspNetUserRoles = UserRoles.FindBy(m => m.UserId == employeeDetailViewModel.EmployeeId).FirstOrDefault();
                    aspNetUserRoles.RoleId = (employeeDetailViewModel.Role).ToString();

                    UserRoles.Edit(aspNetUserRoles);
                    UserRoles.Save();

                    // mapping viewmodel to entity
                    CommonMapper<EmployeeDetailViewModel, EmployeeDetail> mapper = new CommonMapper<EmployeeDetailViewModel, EmployeeDetail>();
                    EmployeeDetail employeeDetailInfo = mapper.Mapper(employeeDetailViewModel);
                    Employee.Edit(employeeDetailInfo);
                    Employee.Save();
                    string code = await UserManager.GeneratePasswordResetTokenAsync(employeeDetailViewModel.EmployeeId);
                    var result = await UserManager.ResetPasswordAsync(employeeDetailViewModel.EmployeeId, code, employeeDetailViewModel.Password);

                    //Delete Work Type Assign Already
                    Guid EmpId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                    var EmployeeWorkType = EmpWorktypeRepo.FindBy(m => m.EmployeeId == EmpId).ToList();
                    if (EmployeeWorkType != null)
                    {
                        foreach (var assign in EmployeeWorkType)
                        {
                            EmpWorktypeRepo.Delete(assign);
                            EmpWorktypeRepo.Save();
                        }
                    }

                    if (employeeDetailViewModel.WorkTypeId != null)
                    {

                        foreach (var key in employeeDetailViewModel.WorkTypeId)
                        {
                            EmployeeWorkType empWorkType = new EmployeeWorkType();
                            empWorkType.Id = Guid.NewGuid();
                            empWorkType.EmployeeId = Guid.Parse(employeeDetailViewModel.EmployeeId);
                            empWorkType.WorkType = Convert.ToInt32(key);

                            EmpWorktypeRepo.Add(empWorkType);
                            EmpWorktypeRepo.Save();
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " approved " + employeeDetailInfo.UserName);

                    return RedirectToAction("Dashboard", "Dashboard", new { area = "Admin", Module = "Dashboard" });
                    //}
                }
                employeeDetailViewModel.RoleList = AspNetRolesRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                //Get Rate Category List
                employeeDetailViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                //Get Rate sub Category List
                employeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.GetAll().Select(m =>
                 new SelectListItem { Text = m.SubCategoryName, Value = m.SubCategoryId.ToString() }).ToList();

                employeeDetailViewModel.WorkTypeList = WorktypeRepo.GetAll().Select(x => new SelectListItem
                {
                    Text = x.WorkTypeName,
                    Value = x.Value.ToString()
                }).ToList();
                employeeDetailViewModel.GetUserRoles = this.GetUserRoles;
                ViewBag.Status = "0";
                return View(employeeDetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Employee/DeleteEmployee
        /// <summary>
        /// Delete Employee Record
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Redirect EmployeeDetails</returns>

        public ActionResult DeleteEmployee(string employeeId)
        {
            try
            {
                Guid employeeid = Guid.Parse(employeeId);

                //delete data in AspNetUserRoles
                AspNetUserRoles aspRoles = UserRoles.FindBy(m => m.UserId == employeeId).FirstOrDefault();
                UserRoles.Delete(aspRoles);
                UserRoles.Save();

                //delete data in AspNetUsers
                AspNetUsers aspNetUser = NetUsers.FindBy(m => m.Id == employeeId).FirstOrDefault();
                NetUsers.Delete(aspNetUser);
                NetUsers.Save();

                //delete Employee In Employeedetail

                EmployeeDetail logtodelete = Employee.FindBy(i => i.EmployeeId == employeeid).FirstOrDefault();
                logtodelete.IsDelete = true;
                Employee.Edit(logtodelete);
                Employee.Save();
                string UserID = Convert.ToString(base.GetUserId);
                var resultDeleteRelation = Employee.DeleteAllReleatedDataByEmployeeid(Convert.ToString(employeeid), UserID);


                Guid otrwRole = Guid.Parse("31cf918d-b8fe-4490-b2d7-27324bfe89b4");
                if (logtodelete.Role == otrwRole)
                {
                    var insertVacation = Employee.UpdateOtrwOrder(logtodelete.OTRW_Order);

                }
                //Get Emplyee Detail In AspNetUsers Table
                //AspNetUsers aspNetUser = NetUsers.FindBy(i => i.Id == EmployeeId).FirstOrDefault();

                // mapping entity to entity
                //CommonMapper<AspNetUsers, AspNetUsersIsDelete> mapper = new CommonMapper<AspNetUsers, AspNetUsersIsDelete>();
                //AspNetUsersIsDelete aspNetUserDelete = mapper.Mapper(aspNetUser);

                //NetUsersIsDeleteRepo.Add(aspNetUserDelete);
                //NetUsersIsDeleteRepo.Save();
                ////Delete Data into AspNetUsers Table
                //NetUsers.Delete(aspNetUser);
                //NetUsers.Save();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted " + logtodelete.UserName);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/Employee/AddEmployeeRates
        /// <summary>
        /// Add Employee Rates
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEmployeeRates()
        {
            ViewEmployeeRatesViewModel model = new ViewEmployeeRatesViewModel();
            try
            {
                using (ViewEmployeeRates)
                {
                    //List<string> EmployeeIDS = Employee.GetNoRateEmployeesByEID();
                    //EmployeeIDS.Insert(0, "Select");
                    //Get Rate Category List
                    model.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                     new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                    //Get Rate sub Category List
                    //model.RateSubCategoryList = RateSubCategoryRepo.GetAll().Select(m =>
                    // new SelectListItem { Text = m.SubCategoryName, Value = m.SubCategoryId.ToString() }).ToList();
                    //model.EmployeeIDList = EmployeeIDS;
                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Bind Employeee EID
        /// </summary>
        /// <returns></returns>
        public List<string> BindEmployeesEid()
        {
            try
            {
                using (Employee)
                {
                    var EmployeeList = Employee.GetAll();
                    List<string> EmployeeIDList = new List<string>();
                    EmployeeIDList.Insert(0, "Select");
                    foreach (var i in EmployeeList)
                        if (!string.IsNullOrEmpty(i.EID))
                            EmployeeIDList.Add(i.EID);
                    return EmployeeIDList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Employee/AddEmployeeRates
        /// <summary>
        /// Add Employee Rates
        /// </summary>
        /// <param name="employeeRatesViewModel"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult AddEmployeeRates(ViewEmployeeRatesViewModel ViewemployeeRatesViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RateSubCategoryRepo)
                    {
                        //ViewemployeeRatesViewModel.CategoryId = Guid.NewGuid();
                        ViewemployeeRatesViewModel.SubCategoryId = Guid.NewGuid();
                        ViewemployeeRatesViewModel.Payroll_Inc_Cost = (ViewemployeeRatesViewModel.BaseRate * ViewemployeeRatesViewModel.S_WC * ViewemployeeRatesViewModel.AL_PH * ViewemployeeRatesViewModel.TAFE * ViewemployeeRatesViewModel.Payroll);
                        ViewemployeeRatesViewModel.Cont_MV_EQ_Cost = (ViewemployeeRatesViewModel.ActualRate - ViewemployeeRatesViewModel.BaseRate);
                        ViewemployeeRatesViewModel.Gross_Labour_Cost = (ViewemployeeRatesViewModel.Payroll_Inc_Cost + ViewemployeeRatesViewModel.Cont_MV_EQ_Cost + ViewemployeeRatesViewModel.Emp_MV_Cost + ViewemployeeRatesViewModel.Equip_Cost + ViewemployeeRatesViewModel.Emp_Mob_Ph_Cost);
                        ViewemployeeRatesViewModel.PERF_B_PAR = (ViewemployeeRatesViewModel.Gross_Labour_Cost + 70);
                        ViewemployeeRatesViewModel.GP_Hour_PAR = (ViewemployeeRatesViewModel.PERF_B_PAR - ViewemployeeRatesViewModel.Gross_Labour_Cost);
                        ViewemployeeRatesViewModel.IsDelete = false;
                        CommonMapper<ViewEmployeeRatesViewModel, RateSubCategory> mapper = new CommonMapper<ViewEmployeeRatesViewModel, RateSubCategory>();
                        RateSubCategory viewRateSubCategory = mapper.Mapper(ViewemployeeRatesViewModel);
                        RateSubCategoryRepo.Add(viewRateSubCategory);
                        RateSubCategoryRepo.Save();
                        TempData["Message"] = 1;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " added employee's rate.");

                        return RedirectToAction("ViewEmployeesRates");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            ViewemployeeRatesViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                   new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();
            return View(ViewemployeeRatesViewModel);
        }

        //GET:Employee/Employee/EditEmployeeRates
        /// <summary>
        /// Edit Employee Rates
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult EditEmployeeRates(Guid SubcategoryId)
        {
            try
            {
                using (ViewEmployeeRates)
                {
                    // var EmployeeIDList = EmployeeRates.GetAll();
                    //Guid RateId = Guid.Parse(Rateid);
                    var employeeRates = RateSubCategoryRepo.FindBy(i => i.SubCategoryId == SubcategoryId).FirstOrDefault();
                    CommonMapper<RateSubCategory, ViewEmployeeRatesViewModel> mapper = new CommonMapper<RateSubCategory, ViewEmployeeRatesViewModel>();
                    ViewEmployeeRatesViewModel employeeRatesViewModel = mapper.Mapper(employeeRates);
                    //employeeRatesViewModel.EmployeeIDList = EmployeeIDList.Select(i => i.EID).ToList();

                    //Get Rate Category List
                    employeeRatesViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                     new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();

                    //Get Rate sub Category List
                    employeeRatesViewModel.RateSubCategoryList = RateSubCategoryRepo.FindBy(m => m.CategoryId == employeeRatesViewModel.CategoryId).Select(m => new SelectListItem()
                    {
                        Text = m.SubCategoryName,
                        Value = m.SubCategoryId.ToString()
                    }).OrderBy(m => m.Text).ToList();

                    return View(employeeRatesViewModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //POST: Employee/Employee/EditEmployeeRates
        /// <summary>
        /// Edit Employees Rates
        /// </summary>
        /// <param name="employeeRatesViewModel"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult EditEmployeeRates(ViewEmployeeRatesViewModel employeeRatesViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RateSubCategoryRepo)
                    {
                        employeeRatesViewModel.Payroll_Inc_Cost = (employeeRatesViewModel.BaseRate * employeeRatesViewModel.S_WC * employeeRatesViewModel.AL_PH * employeeRatesViewModel.TAFE * employeeRatesViewModel.Payroll);
                        employeeRatesViewModel.Cont_MV_EQ_Cost = (employeeRatesViewModel.ActualRate - employeeRatesViewModel.BaseRate);
                        employeeRatesViewModel.Gross_Labour_Cost = (employeeRatesViewModel.Payroll_Inc_Cost + employeeRatesViewModel.Cont_MV_EQ_Cost + employeeRatesViewModel.Emp_MV_Cost + employeeRatesViewModel.Equip_Cost + employeeRatesViewModel.Emp_Mob_Ph_Cost);
                        employeeRatesViewModel.PERF_B_PAR = (employeeRatesViewModel.Gross_Labour_Cost + 70);
                        employeeRatesViewModel.GP_Hour_PAR = (employeeRatesViewModel.PERF_B_PAR - employeeRatesViewModel.Gross_Labour_Cost);
                        CommonMapper<ViewEmployeeRatesViewModel, RateSubCategory> mapper = new CommonMapper<ViewEmployeeRatesViewModel, RateSubCategory>();
                        RateSubCategory viewEmployeeSubCat = mapper.Mapper(employeeRatesViewModel);
                        RateSubCategoryRepo.Edit(viewEmployeeSubCat);
                        RateSubCategoryRepo.Save();
                        TempData["Message"] = 2;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated employee's rate.");


                        return RedirectToAction("ViewEmployeesRates");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            employeeRatesViewModel.RateCategoryList = RateCategoryRepo.GetAll().Select(m =>
                  new SelectListItem { Text = m.CategoryName, Value = m.CategoryId.ToString() }).ToList();
            return View(employeeRatesViewModel);
        }

        //GET: Employee/Employee/GetEmployeeDetailByEID
        /// <summary>
        /// Get Employee Detail By EID
        /// </summary>
        /// <param name="EID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEmployeeDetailByEID(string EID)
        {
            try
            {
                using (Employee)
                {
                    EmployeeDetail employeeDetail = Employee.FindBy(i => i.EID == EID).FirstOrDefault();

                    // mapping entity to viewmodel
                    CommonMapper<EmployeeDetail, EmployeeDetailViewModel> mapper = new CommonMapper<EmployeeDetail, EmployeeDetailViewModel>();
                    EmployeeDetailViewModel EmployeeDetailViewModel = mapper.Mapper(employeeDetail);
                    return Json(EmployeeDetailViewModel, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/Employee/ViewEmployeesRates
        /// <summary>
        /// View Employees Rates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewEmployeesRates()
        {
            try
            {
                using (ViewEmployeeRates)
                {
                    var viewEmployeeRates = ViewEmployeeRates.ViewEmployeeRateDetail().OrderByDescending(m => m.CreatedDate).ToList();
                    string keyword = string.IsNullOrEmpty(Request.QueryString["Keyword"]) ? "" :
                                                (Request.QueryString["Keyword"]);

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                          Convert.ToInt32(Request.QueryString["page_size"]);

                    ViewEmployeeRatesSearchViewModel employeeRatesSearchViewModel = new ViewEmployeeRatesSearchViewModel
                    {
                        Keyword = keyword,
                        PageSize = PageSize
                    };

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        viewEmployeeRates = viewEmployeeRates.Where(rates =>
                       (rates.SubCategoryName != null && rates.SubCategoryName.ToLower().Contains(keyword.ToLower())) ||
                        (rates.ActualRate != 0 && rates.ActualRate.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (rates.BaseRate != 0 && rates.BaseRate.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (rates.Gross_Labour_Cost != 0 && rates.Gross_Labour_Cost.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (rates.PERF_B_PAR != 0 && rates.PERF_B_PAR.ToString().ToLower().Contains(keyword.ToLower())) ||
                         (rates.GP_Hour_PAR != 0 && rates.GP_Hour_PAR.ToString().ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                    }

                    CommonMapper<ViewEmployeeRatesCoreViewModel, ViewEmployeeRatesViewModel> mapper = new CommonMapper<ViewEmployeeRatesCoreViewModel, ViewEmployeeRatesViewModel>();
                    List<ViewEmployeeRatesViewModel> employeeRateslist = mapper.MapToList(viewEmployeeRates.OrderByDescending(i => i.CategoryName).ToList());

                    var rateslistviewmodel = new ViewEmployeeRatesListViewModel
                    {
                        Rateslist = employeeRateslist,
                        RatesListsearchmodel = employeeRatesSearchViewModel,
                    };
                    var view_EemployeeRates = employeeRateslist.ToList();
                    viewEmployeeRates.ForEach(i => i.PageSize = PageSize);
                    ViewBag.PageSize = PageSize;



                    return View(rateslistviewmodel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult ViewEmployeesRates(ViewEmployeeRatesSearchViewModel rateListearchviewmodel)
        {
            try
            {
                using (ViewEmployeeRates)
                {
                    var viewEmployeeRates = ViewEmployeeRates.ViewEmployeeRateDetail();
                    string Keyword = rateListearchviewmodel.Keyword;
                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        viewEmployeeRates = viewEmployeeRates.Where(rates =>
                        (rates.SubCategoryName != null && rates.SubCategoryName.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (rates.ActualRate != 0 && rates.ActualRate.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (rates.BaseRate != 0 && rates.BaseRate.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (rates.Gross_Labour_Cost != 0 && rates.Gross_Labour_Cost.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (rates.PERF_B_PAR != 0 && rates.PERF_B_PAR.ToString().ToLower().Contains(Keyword.ToLower())) ||
                         (rates.GP_Hour_PAR != 0 && rates.GP_Hour_PAR.ToString().ToLower().Contains(Keyword.ToLower()))
                            );
                    }
                    CommonMapper<ViewEmployeeRatesCoreViewModel, ViewEmployeeRatesViewModel> mapper = new CommonMapper<ViewEmployeeRatesCoreViewModel, ViewEmployeeRatesViewModel>();
                    List<ViewEmployeeRatesViewModel> employeeRateslist = mapper.MapToList(viewEmployeeRates.OrderByDescending(i => i.CategoryName).ToList());
                    string keyword = string.IsNullOrEmpty(Request.QueryString["Keyword"]) ? "" :
                                                (Request.QueryString["Keyword"]);

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                          Convert.ToInt32(Request.QueryString["page_size"]);
                    ViewEmployeeRatesSearchViewModel employeeRatesSearchViewModel = new ViewEmployeeRatesSearchViewModel
                    {
                        Keyword = keyword,
                        PageSize = PageSize
                    };
                    var rateslistviewmodel = new ViewEmployeeRatesListViewModel
                    {
                        Rateslist = employeeRateslist,
                        RatesListsearchmodel = employeeRatesSearchViewModel,
                    };


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed employee's rate.");

                    return View(rateslistviewmodel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //POST:Delete Employee Rates
        /// <summary>
        /// Delete Employee Rates
        /// </summary>
        /// <param name="Rateid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteEmployeeRate(Guid SubCategoryId)
        {
            try
            {
                //Guid categoryId = Guid.Parse(CategoryId);
                var employeeRates = RateSubCategoryRepo.FindBy(i => i.SubCategoryId == SubCategoryId).FirstOrDefault();
                var empName = EmployeeRates.FindBy(m => m.SubCategoryId == SubCategoryId).Select(m => m.EmployeeName).FirstOrDefault();
                employeeRates.IsDelete = true;
                RateSubCategoryRepo.Edit(employeeRates);
                RateSubCategoryRepo.Save();

                TempData["Message"] = 3;


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted rate employee rate");

                return RedirectToAction("ViewEmployeesRates");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //GET: checking Employee UserName
        /// <summary>
        /// Check UserName Exist
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public ActionResult CheckUserNameExist(string UserName, string CurrentUserName)
        {
            try
            {
                using (NetUsers)
                {
                    int result = 0;
                    //AspNetUsers EmpDetail = NetUsers.FindBy(user => user.UserName == UserName).FirstOrDefault();
                    EmployeeDetail EmpDetail = Employee.FindBy(user => user.UserName == UserName && user.IsDelete == false && user.UserName != CurrentUserName).FirstOrDefault();
                    if (EmpDetail != null)
                    {
                        result = 0;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = 1;
                        return Json(result = 1, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: checking Employee Email ID for Existence
        /// <summary>
        /// Check EmployeeEmail Exist
        /// </summary>
        /// <param name="EmpEmail"></param>
        /// <returns></returns>
        public ActionResult CheckEmployeeEmailExist(string EmpEmail, string CurrentEmail)
        {
            try
            {
                using (NetUsers)
                {
                    int result = 0;
                    //AspNetUsers EmpDetail = NetUsers.FindBy(user => user.Email == EmpEmail).FirstOrDefault();
                    EmployeeDetail EmpDetail = Employee.FindBy(user => user.Email == EmpEmail && user.IsDelete == false && user.Email != CurrentEmail).FirstOrDefault();
                    if (EmpDetail != null)
                    {
                        result = 0;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = 1;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Upload Files
        /// <summary>
        /// Upload files
        /// </summary>
        /// <param name=" "></param>
        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    string fname = string.Empty;

                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }

                    // Get the complete folder path and store the file inside it.  
                    fname = Path.Combine(Server.MapPath("~/EmployeeImages/TempImages"), fname);
                    file.SaveAs(fname);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " uploaded an employee image");

                    // Returns filename to be displayed  
                    return Json(file.FileName);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpGet]
        public ActionResult GetVacations()
        {
            EmployeeVacationModel employeeVacationModel = new EmployeeVacationModel();
            try
            {
                var EmployeeVacations = VacationRepo.GetAll().Where(m => m.IsDelete == false && m.RoastedOffId == null);
                int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;

                // mapping list< entity > to list<viewmodel>
                CommonMapper<Vacation, VacationViewModel> mapper = new CommonMapper<Vacation, VacationViewModel>();
                var vacationViewModel = mapper.MapToList(EmployeeVacations.ToList());

                var vacationList = vacationViewModel.Select(m => new VacationViewModel
                {
                    Id = m.Id,
                    EmpId = int.Parse(m.EmployeeDetail.EID),
                    EmployeeDetail = m.EmployeeDetail,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Reason = m.Reason,
                    CreatedDate = m.CreatedDate,
                    DisplayStatus = (int)m.Status != 0 ? m.Status.GetAttribute<DisplayAttribute>() != null ?
                                    m.Status.GetAttribute<DisplayAttribute>().Name : m.Status.ToString() : string.Empty,
                    DisplayLeaveType = m.LeaveType.HasValue && (int)m.LeaveType != 0 ? m.LeaveType.GetAttribute<DisplayAttribute>() != null ?
                                    m.LeaveType.GetAttribute<DisplayAttribute>().Name : m.LeaveType.ToString() : string.Empty
                });

                employeeVacationModel.VacationList = vacationList.OrderByDescending(m => m.CreatedDate);
                employeeVacationModel.PageSize = PageSize;

                return View(employeeVacationModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        [ValidateInput(false)]
        public ActionResult GetVacationsPartial()
        {
            EmployeeVacationModel employeeVacationModel = new EmployeeVacationModel();
            try
            {
                var EmployeeVacations = VacationRepo.GetAll().Where(m => m.IsDelete == false && m.RoastedOffId == null);

                string EmpKeyword = ControllerContext.HttpContext.Request.Unvalidated.QueryString["EmployeeKeyword"] != null ?
                                    ControllerContext.HttpContext.Request.Unvalidated.QueryString["EmployeeKeyword"] : string.Empty;

                DateTime StartDate;
                bool validStartDate = DateTime.TryParse(Request.QueryString["StartDate"], out StartDate);

                DateTime EndDate;
                bool validEndDate = DateTime.TryParse(Request.QueryString["EndDate"], out EndDate);

                int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;

                if (!string.IsNullOrEmpty(EmpKeyword))
                {
                    EmployeeVacations = EmployeeVacations.Where(m => (m.EmployeeDetail.EID != null && m.EmployeeDetail.EID.ToLower()
                                    .Contains(EmpKeyword.ToLower())) ||
                                    (m.EmployeeDetail.UserName != null && m.EmployeeDetail.UserName.ToLower().Contains(EmpKeyword.ToLower())));
                }

                if (validStartDate && validEndDate)
                {
                    EmployeeVacations = EmployeeVacations.Where(m => m.StartDate >= StartDate && m.EndDate <= EndDate);
                }
                else if (validStartDate)
                {
                    EmployeeVacations = EmployeeVacations.Where(m => m.StartDate >= StartDate);
                }
                else if (validEndDate)
                {
                    EmployeeVacations = EmployeeVacations.Where(m => m.EndDate <= EndDate);
                }

                // mapping list< entity > to list<viewmodel>
                CommonMapper<Vacation, VacationViewModel> mapper = new CommonMapper<Vacation, VacationViewModel>();
                var vacationViewModel = mapper.MapToList(EmployeeVacations.ToList());

                var vacationList = vacationViewModel.Select(m => new VacationViewModel
                {
                    Id = m.Id,
                    EmpId = int.Parse(m.EmployeeDetail.EID),
                    EmployeeDetail = m.EmployeeDetail,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Reason = m.Reason,
                    DisplayStatus = (int)m.Status != 0 ? m.Status.GetAttribute<DisplayAttribute>() != null ?
                                    m.Status.GetAttribute<DisplayAttribute>().Name : m.Status.ToString() : string.Empty,
                    ModifiedDate = m.ModifiedDate,
                    DisplayLeaveType = m.LeaveType.HasValue && (int)m.LeaveType != 0 ? m.LeaveType.GetAttribute<DisplayAttribute>() != null ?
                                    m.LeaveType.GetAttribute<DisplayAttribute>().Name : m.LeaveType.ToString() : string.Empty
                });

                employeeVacationModel.VacationList = vacationList.OrderByDescending(m => m.ModifiedDate);
                employeeVacationModel.PageSize = PageSize;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed list of all vacations of employee.");

                return PartialView("_EmployeeVacation", employeeVacationModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        public ActionResult ApproveVacation()
        {
            try
            {
                string id = Request.QueryString["id"];
                string grid_page = Request.QueryString["grid-page"];
                string grid_column = Request.QueryString["grid-column"];
                string grid_dir = Request.QueryString["grid-dir"];
                string EmpKeyword = !string.IsNullOrEmpty(Request.QueryString["EmployeeKeyword"]) ? Request.QueryString["EmployeeKeyword"]
                                    : string.Empty;
                Nullable<DateTime> StartDate = !string.IsNullOrEmpty(Request.QueryString["StartDate"]) ? DateTime.Parse(Request.QueryString["StartDate"])
                                : (Nullable<DateTime>)null;
                Nullable<DateTime> EndDate = !string.IsNullOrEmpty(Request.QueryString["EndDate"]) ? DateTime.Parse(Request.QueryString["EndDate"])
                                : (Nullable<DateTime>)null;
                int PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ? int.Parse(Request.QueryString["PageSize"]) : 10;



                if (!string.IsNullOrEmpty(id))
                {
                    Guid LoginuserId = Guid.Parse(base.GetUserId);
                    var FromEmployeeList = Employee.FindBy(m => m.EmployeeId == LoginuserId).FirstOrDefault();
                    string FromMailUser = FromEmployeeList.Email;

                    Guid Id = Guid.Parse(id);
                    Vacation vacation = VacationRepo.FindBy(m => m.Id == Id).FirstOrDefault();

                    // if employee already booked or not
                    var employeeHasJob = EmployeeJobRepo.EmployeeHasJob(vacation.EmployeeId.ToString(),
                                         vacation.StartDate, vacation.EndDate);
                    if (employeeHasJob)
                    {
                        return Json(new { alreadyBooked = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Guid ToEmployeeId = Guid.Parse(vacation.EmployeeId.ToString());
                        var ToEmployeeList = Employee.FindBy(m => m.EmployeeId == ToEmployeeId).FirstOrDefault();
                        string ToMailUser = ToEmployeeList.Email;
                        vacation.Status = (int)Constant.VacationType.Approved;
                        vacation.ModifiedDate = DateTime.Now;
                        vacation.ModifiedBy = Guid.Parse(base.GetUserId);

                        VacationRepo.Edit(vacation);
                        VacationRepo.Save();

                        var vacationstatus = vacation.Status.Value != 2 ? ((Constant.VacationType)vacation.Status).
                                            GetAttribute<DisplayAttribute>().Name : ((Constant.VacationType)vacation.Status).ToString();

                        SendEmail sendEmail = new SendEmail();
                        StringBuilder body = new StringBuilder();
                        body.Append("<p>Dear " + vacation.EmployeeDetail.UserName + ",</p>");
                        body.Append("<p>This is to inform you that your leave has been approved by management. Please consider below details:-</p>");
                        body.Append("<p><b>Start Date : </b>" + vacation.StartDate.Value.Date.ToShortDateString() + "</p>");
                        body.Append("<p><b>End Date : </b>" + vacation.EndDate.Value.Date.ToShortDateString() + "</p>");
                        body.Append("<p><b>Hours </b>: " + vacation.Hours + "</p>");
                        body.Append("<p><b>Status : </b>" + vacationstatus + "</p>");
                        body.Append("<div>Sincerly,</div>");
                        body.Append("<div>Admin</div>");
                        sendEmail.Send("Leave Approved", body.ToString(), FromMailUser, ToMailUser);

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " approved a vacation of " + vacation.EmployeeDetail.UserName);
                    }
                }

                var routeValues = new RouteValueDictionary();
                routeValues.Add("grid-page", grid_page);
                routeValues.Add("grid-column", grid_column);
                routeValues.Add("grid-dir", grid_dir);
                routeValues.Add("PageSize", PageSize);
                routeValues.Add("EmployeeKeyword", EmpKeyword);
                routeValues.Add("StartDate", StartDate);
                routeValues.Add("EndDate", EndDate);



                return RedirectToAction("GetVacationsPartial", routeValues);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        public ActionResult RejectVacation()
        {
            try
            {
                string id = Request.QueryString["id"];
                string grid_page = Request.QueryString["grid-page"];
                string grid_column = Request.QueryString["grid-column"];
                string grid_dir = Request.QueryString["grid-dir"];
                string EmpKeyword = !string.IsNullOrEmpty(Request.QueryString["EmployeeKeyword"]) ? Request.QueryString["EmployeeKeyword"]
                                    : string.Empty;
                Nullable<DateTime> StartDate = !string.IsNullOrEmpty(Request.QueryString["StartDate"]) ? DateTime.Parse(Request.QueryString["StartDate"])
                                : (Nullable<DateTime>)null;
                Nullable<DateTime> EndDate = !string.IsNullOrEmpty(Request.QueryString["EndDate"]) ? DateTime.Parse(Request.QueryString["EndDate"])
                                : (Nullable<DateTime>)null;
                int PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ? int.Parse(Request.QueryString["PageSize"]) : 10;

                if (!string.IsNullOrEmpty(id))
                {
                    Guid LoginuserId = Guid.Parse(base.GetUserId);
                    var FromEmployeeList = Employee.FindBy(m => m.EmployeeId == LoginuserId).FirstOrDefault();
                    string FromMailUser = FromEmployeeList.Email;

                    Guid Id = Guid.Parse(id);
                    Vacation vacation = VacationRepo.FindBy(m => m.Id == Id).FirstOrDefault();
                    Guid ToEmployeeId = Guid.Parse(vacation.EmployeeId.ToString());
                    var ToEmployeeList = Employee.FindBy(m => m.EmployeeId == ToEmployeeId).FirstOrDefault();
                    string ToMailUser = ToEmployeeList.Email;
                    vacation.Status = (int)Constant.VacationType.NotApproved;
                    vacation.ModifiedDate = DateTime.Now;
                    vacation.ModifiedBy = Guid.Parse(base.GetUserId);

                    VacationRepo.Edit(vacation);
                    VacationRepo.Save();

                    var vacationstatus = vacation.Status.Value != 2 ? ((Constant.VacationType)vacation.Status).
                                        GetAttribute<DisplayAttribute>().Name : ((Constant.VacationType)vacation.Status).ToString();

                    SendEmail sendEmail = new SendEmail();
                    StringBuilder body = new StringBuilder();
                    body.Append("<p>Dear " + vacation.EmployeeDetail.UserName + ",</p>");
                    body.Append("<p>This is to inform you that your leave has not been approved by management. Please consider below details:-</p>");
                    body.Append("<p><b>Start Date : </b>" + vacation.StartDate.Value.Date.ToShortDateString() + "</p>");
                    body.Append("<p><b>End Date : </b>" + vacation.EndDate.Value.Date.ToShortDateString() + "</p>");
                    body.Append("<p><b>Hours </b>: " + vacation.Hours + "</p>");
                    body.Append("<p><b>Status : </b>" + vacationstatus + "</p>");
                    body.Append("<div>Sincerly,</div>");
                    body.Append("<div>Admin</div>");
                    sendEmail.Send("Leave Not Approved", body.ToString(), FromMailUser, ToMailUser);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " rejected a vacation of " + vacation.EmployeeDetail.UserName);

                }

                var routeValues = new RouteValueDictionary();
                routeValues.Add("grid-page", grid_page);
                routeValues.Add("grid-column", grid_column);
                routeValues.Add("grid-dir", grid_dir);
                routeValues.Add("PageSize", PageSize);
                routeValues.Add("EmployeeKeyword", EmpKeyword);
                routeValues.Add("StartDate", StartDate);
                routeValues.Add("EndDate", EndDate);

                return RedirectToAction("GetVacationsPartial", routeValues);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        [ValidateInput(false)]
        public ActionResult ManageVacactions()
        {
            try
            {
                ManageVacactionViewModel manageVacactionViewModel = new ManageVacactionViewModel();
                Guid userId = Guid.Parse(base.GetUserId);
                Nullable<DateTime> StartDate = !string.IsNullOrEmpty(Request.QueryString["StartDate"]) ?
                                               DateTime.Parse(Request.QueryString["StartDate"]) : (DateTime?)null;
                Nullable<DateTime> EndDate = !string.IsNullOrEmpty(Request.QueryString["EndDate"]) ?
                                               DateTime.Parse(Request.QueryString["EndDate"]) : (DateTime?)null;
                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";

                var vacationList = VacationRepo.FindBy(m => m.EmployeeId == userId && m.IsDelete == false);
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    manageVacactionViewModel.SearchStartDate = StartDate;
                    manageVacactionViewModel.SearchEndDate = EndDate;
                    vacationList = vacationList.Where(m => m.StartDate >= StartDate && m.EndDate <= EndDate);
                }
                else if (StartDate.HasValue && !EndDate.HasValue)
                {
                    manageVacactionViewModel.SearchStartDate = StartDate;
                    vacationList = vacationList.Where(m => m.StartDate >= StartDate);
                }
                else if (EndDate.HasValue && !StartDate.HasValue)
                {
                    manageVacactionViewModel.SearchEndDate = EndDate;
                    vacationList = vacationList.Where(m => m.EndDate <= EndDate);
                }

                CommonMapper<Vacation, VacationViewModel> mapper = new CommonMapper<Vacation, VacationViewModel>();
                List<VacationViewModel> vacationViewModel = mapper.MapToList(vacationList.ToList());

                var listVacations = vacationViewModel.Select(m => new VacationViewModel()
                {
                    Id = m.Id,
                    EmployeeId = m.EmployeeId,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Hours = m.Hours,
                    Reason = m.Reason,
                    DisplayStatus = m.Status != 0 ? m.Status.GetAttribute<DisplayAttribute>() != null ?
                                        m.Status.GetAttribute<DisplayAttribute>().Name : m.Status.ToString() : string.Empty,
                    CreatedDate = m.CreatedDate,
                    CreatedBy = m.CreatedBy,
                    ModifiedDate = m.ModifiedDate,
                    ModifiedBy = m.ModifiedBy,
                    IsVisible = m.Status == Constant.VacationType.PendingApproval ? "visible" : "hidden"
                });

                manageVacactionViewModel.listVacations = listVacations.OrderByDescending(m => m.CreatedDate).ThenByDescending(m => m.ModifiedDate);
                manageVacactionViewModel.PageSize = int.Parse(PageSize);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed a vacation of employee.");

                return View(manageVacactionViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        public ActionResult ManageVacactionsPartial()
        {
            try
            {
                ManageVacactionViewModel manageVacactionViewModel = new ManageVacactionViewModel();
                Guid userId = Guid.Parse(base.GetUserId);
                Nullable<DateTime> StartDate = !string.IsNullOrEmpty(Request.QueryString["StartDate"]) ?
                                               DateTime.Parse(Request.QueryString["StartDate"]) : (DateTime?)null;
                Nullable<DateTime> EndDate = !string.IsNullOrEmpty(Request.QueryString["EndDate"]) ?
                                               DateTime.Parse(Request.QueryString["EndDate"]) : (DateTime?)null;
                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";

                var vacationList = VacationRepo.FindBy(m => m.EmployeeId == userId && m.IsDelete == false);
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    vacationList = vacationList.Where(m => m.StartDate >= StartDate && m.EndDate <= EndDate);
                }
                else if (StartDate.HasValue && !EndDate.HasValue)
                {
                    vacationList = vacationList.Where(m => m.StartDate >= StartDate);
                }
                else if (EndDate.HasValue && !StartDate.HasValue)
                {
                    vacationList = vacationList.Where(m => m.EndDate <= EndDate);
                }


                CommonMapper<Vacation, VacationViewModel> mapper = new CommonMapper<Vacation, VacationViewModel>();
                List<VacationViewModel> vacationViewModel = mapper.MapToList(vacationList.ToList());

                var listVacations = vacationViewModel.Select(m => new VacationViewModel()
                {
                    Id = m.Id,
                    EmployeeId = m.EmployeeId,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Hours = m.Hours,
                    Reason = m.Reason,
                    DisplayStatus = m.Status != 0 ? m.Status.GetAttribute<DisplayAttribute>() != null ?
                                        m.Status.GetAttribute<DisplayAttribute>().Name : m.Status.ToString() : string.Empty,
                    CreatedDate = m.CreatedDate,
                    CreatedBy = m.CreatedBy,
                    ModifiedDate = m.ModifiedDate,
                    ModifiedBy = m.ModifiedBy,
                    IsVisible = m.Status == Constant.VacationType.PendingApproval ? "visible" : "hidden"
                });

                manageVacactionViewModel.listVacations = listVacations.OrderByDescending(m => m.CreatedDate).ThenByDescending(m => m.ModifiedDate);
                manageVacactionViewModel.PageSize = int.Parse(PageSize);

                return PartialView("_ManageVacationsGrid", manageVacactionViewModel);
            }
            catch (Exception)
            {

                throw;
            }

            finally
            {
                VacationRepo.Dispose();
            }
        }

        [HttpGet]
        public ActionResult AddNewVacation()
        {
            try
            {
                ModelState.Clear();
                VacationViewModel vacationViewModel = new VacationViewModel();
                vacationViewModel.Id = Guid.NewGuid();
                vacationViewModel.EmployeeId = Guid.Parse(base.GetUserId);
                vacationViewModel.Status = Constant.VacationType.PendingApproval;
                vacationViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                return PartialView("_AddNewVacation", vacationViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddNewVacation(VacationViewModel vacationViewModel)
        {
            try
            {
                List<ModelError> errorList = new List<ModelError>();

                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";

                if (ModelState.IsValid)
                {

                    var IsVacationExist = VacationRepo.FindBy(m => m.StartDate == vacationViewModel.StartDate &&
                                          m.EndDate == vacationViewModel.EndDate).FirstOrDefault();

                    if (IsVacationExist != null)
                    {
                        errorList.Add(new ModelError("Vacation already exist for mentioned date !"));
                        return Json(new { errorList = errorList });
                    }

                    if (!vacationViewModel.StartDate.HasValue || !vacationViewModel.EndDate.HasValue)
                    {
                        errorList.Add(new ModelError("Start and End dates are required !"));
                    }

                    if (vacationViewModel.StartDate > vacationViewModel.EndDate)
                    {
                        errorList.Add(new ModelError("Start date can't be greater than End date !"));
                    }

                    if (vacationViewModel.Hours.HasValue)
                    {
                        if (vacationViewModel.Hours > 1500)
                        {
                            errorList.Add(new ModelError("Hours specified are too large !"));
                        }
                        else if (vacationViewModel.Hours <= 0)
                        {
                            errorList.Add(new ModelError("Invalid hrs !"));
                        }
                    }
                    else
                    {
                        errorList.Add(new ModelError("Hours are required !"));
                    }

                    if (!string.IsNullOrEmpty(vacationViewModel.Reason))
                    {
                        // allowed alphanumeric, space, and #& chars
                        var regex = new Regex("^[#&\\w\\s]*$");
                        if (!regex.IsMatch(vacationViewModel.Reason))
                        {
                            errorList.Add(new ModelError("Reason data not valid !"));
                        }
                    }
                    else
                    {
                        errorList.Add(new ModelError("Reason is required !"));
                    }


                    if (errorList.Count > 0)
                    {
                        return Json(new { errorList = errorList });
                    }

                    CommonMapper<VacationViewModel, Vacation> mapper = new CommonMapper<VacationViewModel, Vacation>();
                    Vacation vacation = mapper.Mapper(vacationViewModel);
                    vacation.CreatedDate = DateTime.Now;
                    vacation.IsDelete = false;

                    VacationRepo.DeAttach(vacation);
                    VacationRepo.Add(vacation);
                    VacationRepo.Save();

                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("PageSize", PageSize);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added new vacation.");
                    return RedirectToAction("ManageVacactionsPartial", routeValues);
                }
                else
                {
                    var errorCollection = ModelState.Where(m => m.Value.Errors.Count > 0);
                    foreach (var item in errorCollection)
                    {
                        if (item.Key == "StartDate")
                        {
                            errorList.Add(new ModelError("Start date is not valid !"));
                        }
                        else if (item.Key == "EndDate")
                        {
                            errorList.Add(new ModelError("End date is not valid !"));
                        }
                        else if (item.Key == "Hours")
                        {
                            errorList.Add(new ModelError("Hours are invalid !"));
                        }
                    }
                    return Json(new { errorList = errorList });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        [HttpGet]
        public ActionResult EditVacation(string Id)
        {
            try
            {
                ModelState.Clear();

                Guid id = Guid.Parse(Id);

                var Vacation = VacationRepo.FindBy(m => m.Id == id).FirstOrDefault();
                CommonMapper<Vacation, VacationViewModel> mapper = new CommonMapper<Vacation, VacationViewModel>();
                VacationViewModel vacationViewModel = mapper.Mapper(Vacation);
                vacationViewModel.OldStartDate = vacationViewModel.StartDate;
                vacationViewModel.OldEndDate = vacationViewModel.EndDate;

                return PartialView("_EditVacation", vacationViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult EditVacation(VacationViewModel vacationViewModel)
        {
            try
            {
                List<ModelError> errorList = new List<ModelError>();

                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";

                if (ModelState.IsValid)
                {
                    if (vacationViewModel.StartDate != vacationViewModel.OldStartDate ||
                        vacationViewModel.EndDate != vacationViewModel.OldEndDate)
                    {
                        var IsVacationExist = VacationRepo.FindBy(m => m.StartDate == vacationViewModel.StartDate &&
                                          m.EndDate == vacationViewModel.EndDate).FirstOrDefault();

                        if (IsVacationExist != null)
                        {
                            errorList.Add(new ModelError("Vacation already exist for mentioned date !"));
                            return Json(new { errorList = errorList });
                        }
                    }

                    if (!vacationViewModel.StartDate.HasValue || !vacationViewModel.EndDate.HasValue)
                    {
                        errorList.Add(new ModelError("Start and End dates are required !"));
                    }

                    if (vacationViewModel.StartDate > vacationViewModel.EndDate)
                    {
                        errorList.Add(new ModelError("Start date can't be greater than End date !"));
                    }



                    if (vacationViewModel.Hours.HasValue)
                    {
                        if (vacationViewModel.Hours > 1500)
                        {
                            errorList.Add(new ModelError("Hours specified are too large !"));
                        }
                        else if (vacationViewModel.Hours <= 0)
                        {
                            errorList.Add(new ModelError("Invalid hrs !"));
                        }
                    }
                    else
                    {
                        errorList.Add(new ModelError("Hours are required !"));
                    }

                    if (!string.IsNullOrEmpty(vacationViewModel.Reason))
                    {
                        // allowed alphanumeric, space, and #& chars
                        var regex = new Regex("^[#&\\w\\s]*$");
                        if (!regex.IsMatch(vacationViewModel.Reason))
                        {
                            errorList.Add(new ModelError("Reason data not valid !"));
                        }
                    }
                    else
                    {
                        errorList.Add(new ModelError("Reason is required !"));
                    }

                    if (errorList.Count > 0)
                    {
                        return Json(new { errorList = errorList });
                    }

                    CommonMapper<VacationViewModel, Vacation> mapper = new CommonMapper<VacationViewModel, Vacation>();
                    Vacation vacation = mapper.Mapper(vacationViewModel);
                    vacation.CreatedDate = DateTime.Now;
                    vacation.IsDelete = false;

                    VacationRepo.DeAttach(vacation);
                    VacationRepo.Edit(vacation);
                    VacationRepo.Save();

                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("PageSize", PageSize);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated a vacation of employee.");

                    return RedirectToAction("ManageVacactionsPartial", routeValues);
                }
                else
                {
                    var errorCollection = ModelState.Where(m => m.Value.Errors.Count > 0);
                    foreach (var item in errorCollection)
                    {
                        if (item.Key == "StartDate")
                        {
                            errorList.Add(new ModelError("Start date is not valid !"));
                        }
                        else if (item.Key == "EndDate")
                        {
                            errorList.Add(new ModelError("End date is not valid !"));
                        }
                        else if (item.Key == "Hours")
                        {
                            errorList.Add(new ModelError("Hours are invalid !"));
                        }
                    }
                    return Json(new { errorList = errorList });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        [HttpGet]
        public ActionResult DeleteVacation(string Id)
        {
            try
            {
                ModelState.Clear();

                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";
                Guid id = Guid.Parse(Id);

                var VacationHoliday = VacationRepo.FindBy(m => m.Id == id).FirstOrDefault();
                VacationHoliday.IsDelete = true;
                VacationRepo.Edit(VacationHoliday);
                VacationRepo.Save();

                var routeValues = new RouteValueDictionary();
                routeValues.Add("PageSize", PageSize);


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted a vacation of employee.");


                return RedirectToAction("ManageVacactionsPartial", routeValues);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET: Employee/Employee/GetPublicHoliday
        /// <summary>
        /// GetPublicHoliday
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult GetPublicHoliday()
        {
            try
            {
                using (IHoliday)
                {
                    HolidaySearchViewModel SearchViewModel = new HolidaySearchViewModel();
                    string Searchstring = ControllerContext.HttpContext.Request.Unvalidated.QueryString["searchkeyword"] != null ?
                                    ControllerContext.HttpContext.Request.Unvalidated.QueryString["searchkeyword"] : string.Empty;
                    var Holidaylist = IHoliday.GetAll().Where(m => m.IsDelete == false);
                    DateTime? StartDate = Request.QueryString["StartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["StartDate"].ToString()) ?
                                  DateTime.Parse(Request.QueryString["StartDate"]) : (DateTime?)null : (DateTime?)null;
                    DateTime? EndDate = Request.QueryString["EndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["EndDate"].ToString()) ?
                               DateTime.Parse(Request.QueryString["EndDate"]) : (DateTime?)null : (DateTime?)null;
                    Holidaylist = string.IsNullOrEmpty(Searchstring) ? Holidaylist :
                               Holidaylist.Where(i => i.Reason.ToLower().Contains(Searchstring.ToLower()));
                    if (StartDate.HasValue && EndDate.HasValue)
                    {
                        Holidaylist = Holidaylist.Where(m => (m.Date != null && m.Date >= StartDate && m.Date <= EndDate));
                        SearchViewModel.StartDate = StartDate;
                        SearchViewModel.EndDate = EndDate;
                    }
                    else if (StartDate.HasValue)
                    {
                        Holidaylist = Holidaylist.Where(m => (m.Date != null && m.Date >= StartDate));
                        SearchViewModel.StartDate = StartDate;
                    }
                    else if (EndDate.HasValue)
                    {
                        Holidaylist = Holidaylist.Where(m => (m.Date != null && m.Date <= EndDate));
                        SearchViewModel.EndDate = EndDate;
                    }
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<PublicHoliday, PublicHolidayViewModel> mapper = new CommonMapper<PublicHoliday, PublicHolidayViewModel>();
                    List<PublicHolidayViewModel> publicHolidayViewModel = mapper.MapToList(Holidaylist.OrderBy(m => m.Date).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);
                    SearchViewModel.PageSize = PageSize;
                    SearchViewModel.SearchText = string.IsNullOrEmpty(Searchstring) ? "" : Searchstring;
                    var ViewModel = new PublicHoidaysListViewModel
                    {
                        PublicHolidayViewModel = publicHolidayViewModel,
                        HolidaySearchViewModel = SearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets list of public holiday.");


                    return View(ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST: Employee/Employee/GetPublicHoliday
        /// <summary>
        /// GetPublicHoliday
        /// </summary>
        /// <param name="SearchViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetPublicHoliday(HolidaySearchViewModel SearchViewModel)
        {
            try
            {
                using (IHoliday)
                {
                    var Holidaylist = IHoliday.GetAll().Where(m => m.IsDelete == false);
                    Holidaylist = string.IsNullOrEmpty(SearchViewModel.SearchText) ? Holidaylist :
                                  Holidaylist.Where(i => i.Reason.ToLower().Contains(SearchViewModel.SearchText.ToLower()));
                    if (SearchViewModel.StartDate != null && SearchViewModel.EndDate != null)
                    {
                        Holidaylist = (DateTime)SearchViewModel.StartDate != null ? Holidaylist.Where(customer => customer.Date >=
                                         (DateTime)SearchViewModel.StartDate && customer.Date <= (DateTime)SearchViewModel.EndDate) : Holidaylist;
                    }
                    if (SearchViewModel.StartDate == null && SearchViewModel.EndDate != null)
                    {
                        Holidaylist = (DateTime)SearchViewModel.EndDate != null ? Holidaylist.Where(customer => customer.Date
                                    <= (DateTime)SearchViewModel.EndDate) : Holidaylist;
                    }
                    if (SearchViewModel.StartDate != null && SearchViewModel.EndDate == null)
                    {
                        Holidaylist = (DateTime)SearchViewModel.StartDate != null ? Holidaylist.Where(customer => customer.Date
                                 >= (DateTime)SearchViewModel.StartDate) : Holidaylist;
                    }
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<PublicHoliday, PublicHolidayViewModel> mapper = new CommonMapper<PublicHoliday, PublicHolidayViewModel>();
                    List<PublicHolidayViewModel> publicHolidayViewModel = mapper.MapToList(Holidaylist.OrderBy(m => m.Date).ToList());
                    SearchViewModel.SearchText = string.IsNullOrEmpty(SearchViewModel.SearchText) ? "" : SearchViewModel.SearchText;
                    var ViewModel = new PublicHoidaysListViewModel
                    {
                        PublicHolidayViewModel = publicHolidayViewModel,
                        HolidaySearchViewModel = SearchViewModel
                    };


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets public holiday.");


                    return View(ViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///partial view  _AddEditHolidays
        /// <summary>
        /// _AddEditHolidays
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult _AddHolidays(string Id = "")
        {
            try
            {
                PublicHolidayViewModel publicHolidayViewModel = new PublicHolidayViewModel();

                return PartialView("_AddHolidays", publicHolidayViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }


        ///partial view  _AddEditHolidays
        /// <summary>
        /// _AddEditHolidays
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult _EditHolidays(string Id = "")
        {
            try
            {
                Guid ID = Guid.Parse(Id);
                var Holiday = IHoliday.FindBy(i => i.Id == ID).FirstOrDefault();
                CommonMapper<PublicHoliday, PublicHolidayViewModel> mapper = new CommonMapper<PublicHoliday, PublicHolidayViewModel>();
                PublicHolidayViewModel publicHolidayViewModel = mapper.Mapper(Holiday);



                return PartialView("_EditHolidays", publicHolidayViewModel);

            }
            catch (Exception)
            {

                throw;
            }
        }

        //post:partial view  _AddEditHolidays
        /// <summary>
        /// _AddEditHolidays
        /// </summary>
        /// <param name="publicHolidayViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _AddHolidays(PublicHolidayViewModel publicHolidayViewModel)
        {
            try
            {
                Guid id = Guid.Parse(publicHolidayViewModel.Id.ToString());
                if (ModelState.IsValid)
                {
                    publicHolidayViewModel.IsDelete = false;
                    publicHolidayViewModel.Id = Guid.NewGuid();
                    publicHolidayViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                    publicHolidayViewModel.CreatedDate = DateTime.Now;
                    CommonMapper<PublicHolidayViewModel, PublicHoliday> mapper = new CommonMapper<PublicHolidayViewModel, PublicHoliday>();
                    PublicHoliday Holiday = mapper.Mapper(publicHolidayViewModel);
                    IHoliday.Add(Holiday);
                    IHoliday.Save();
                    TempData["Message"] = 1;
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added employee leave.");
                    return RedirectToAction("GetPublicHoliday");
                }
                else
                {
                    return PartialView(publicHolidayViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //post:partial view  _AddEditHolidays
        /// <summary>
        /// _AddEditHolidays
        /// </summary>
        /// <param name="publicHolidayViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _EditHolidays(PublicHolidayViewModel publicHolidayViewModel)
        {
            try
            {
                Guid id = Guid.Parse(publicHolidayViewModel.Id.ToString());
                if (ModelState.IsValid)
                {
                    var Holiday = IHoliday.FindBy(i => i.Id == id).FirstOrDefault();
                    Holiday.ModifiedBy = Guid.Parse(base.GetUserId);
                    Holiday.ModifiedDate = DateTime.Now;
                    Holiday.Reason = publicHolidayViewModel.Reason;
                    Holiday.Date = publicHolidayViewModel.Date;
                    IHoliday.Edit(Holiday);
                    IHoliday.Save();
                    TempData["Message"] = 2;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated employee leave.");

                    return RedirectToAction("GetPublicHoliday");
                }
                else
                {
                    return PartialView(publicHolidayViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET : Employee/Employee/DeleteHoliday
        /// <summary>
        /// DeleteHoliday
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteHoliday(string id)
        {
            try
            {
                using (IHoliday)
                {
                    var Id = Guid.Parse(id);
                    var Holiday = IHoliday.FindBy(i => i.Id == Id).FirstOrDefault();
                    Holiday.IsDelete = true;
                    IHoliday.Edit(Holiday);
                    IHoliday.Save();
                    TempData["Message"] = 3;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted employee leave.");

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET : Employee/Employee/CheckExistingHolidays
        /// <summary>
        /// Check Existing Holidays
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckExistingHolidays(DateTime? Date, string Id)
        {
            try
            {
                DateTime date = Convert.ToDateTime(Date);
                Guid ID = Guid.Parse(Id);
                var Holidays = IHoliday.GetAll();
                var ExistingRecord = (DateTime)Date != null ? Holidays.Where(Holidate => Holidate.Date ==
                                    (DateTime)Date).FirstOrDefault() : null;
                if (ExistingRecord != null && ID != Guid.Empty)
                {
                    if (ExistingRecord.Id == ID)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (ExistingRecord != null)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult EditVacationAdmin(string Id)
        {
            try
            {
                ModelState.Clear();

                Guid id = Guid.Parse(Id);

                var Vacation = VacationRepo.FindBy(m => m.Id == id).FirstOrDefault();
                CommonMapper<Vacation, VacationViewModel> mapper = new CommonMapper<Vacation, VacationViewModel>();
                VacationViewModel vacationViewModel = mapper.Mapper(Vacation);
                vacationViewModel.OldStartDate = vacationViewModel.StartDate;
                vacationViewModel.OldEndDate = vacationViewModel.EndDate;

                return PartialView("_EditManageVacationAdmin", vacationViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult EditVacationAdmin(VacationViewModel vacationViewModel)
        {
            try
            {
                List<ModelError> errorList = new List<ModelError>();

                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";

                if (ModelState.IsValid)
                {
                    if (vacationViewModel.StartDate != vacationViewModel.OldStartDate ||
                        vacationViewModel.EndDate != vacationViewModel.OldEndDate)
                    {
                        //var IsVacationExist = VacationRepo.FindBy(m => m.StartDate == vacationViewModel.StartDate &&
                        //                  m.EndDate == vacationViewModel.EndDate).FirstOrDefault();

                        var IsVacationExist = VacationRepo.GetExistVacationDate(Convert.ToDateTime(vacationViewModel.StartDate), Convert.ToDateTime(vacationViewModel.EndDate), vacationViewModel.Id, vacationViewModel.EmployeeId).FirstOrDefault();



                        if (IsVacationExist != null)
                        {
                            errorList.Add(new ModelError("Vacation already exist for mentioned date !"));
                            return Json(new { errorList = errorList });
                        }
                    }

                    if (!vacationViewModel.StartDate.HasValue || !vacationViewModel.EndDate.HasValue)
                    {
                        errorList.Add(new ModelError("Start and End dates are required !"));
                    }

                    if (vacationViewModel.StartDate > vacationViewModel.EndDate)
                    {
                        errorList.Add(new ModelError("Start date can't be greater than End date !"));
                    }

                    if (errorList.Count > 0)
                    {
                        return Json(new { errorList = errorList });
                    }

                    CommonMapper<VacationViewModel, Vacation> mapper = new CommonMapper<VacationViewModel, Vacation>();
                    Vacation vacation = mapper.Mapper(vacationViewModel);
                    vacation.ModifiedBy = Guid.Parse(base.GetUserId);
                    vacation.ModifiedDate = DateTime.Now;

                    VacationRepo.DeAttach(vacation);
                    VacationRepo.Edit(vacation);
                    VacationRepo.Save();

                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("PageSize", PageSize);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated vacation");

                    return RedirectToAction("GetVacations", routeValues);
                }
                else
                {
                    var errorCollection = ModelState.Where(m => m.Value.Errors.Count > 0);
                    foreach (var item in errorCollection)
                    {
                        if (item.Key == "StartDate")
                        {
                            errorList.Add(new ModelError("Start date is not valid !"));
                        }
                        else if (item.Key == "EndDate")
                        {
                            errorList.Add(new ModelError("End date is not valid !"));
                        }
                        else if (item.Key == "Hours")
                        {
                            errorList.Add(new ModelError("Hours are invalid !"));
                        }
                    }
                    return Json(new { errorList = errorList });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }
        //GET: Employee/Employee/GetCategoryDetailByCategoryId
        /// <summary>
        /// Get Category Detail By CategoryId
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCategoryDetailBySubCategoryId(Guid SubCategoryId)
        {
            try
            {
                using (RateSubCategoryRepo)
                {
                    RateSubCategory categoryDetail = RateSubCategoryRepo.FindBy(i => i.SubCategoryId == SubCategoryId).FirstOrDefault();

                    // mapping entity to viewmodel
                    CommonMapper<RateSubCategory, EmployeeRatesViewModel> mapper = new CommonMapper<RateSubCategory, EmployeeRatesViewModel>();
                    EmployeeRatesViewModel EmployeeDetailViewModel = mapper.Mapper(categoryDetail);
                    return Json(EmployeeDetailViewModel, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/Employee/GetCategoryDetailBySubCategoryId
        /// <summary>
        /// Get Category Detail By CategoryId
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public ActionResult GetCategoryDetailByCategoryId(Guid CategoryId)
        {
            try
            {
                using (RateCategoryRepo)
                {
                    RateCategory categoryDetail = RateCategoryRepo.FindBy(i => i.CategoryId == CategoryId).FirstOrDefault();

                    // mapping entity to viewmodel
                    CommonMapper<RateCategory, EmployeeRatesViewModel> mapper = new CommonMapper<RateCategory, EmployeeRatesViewModel>();
                    EmployeeRatesViewModel EmployeeDetailViewModel = mapper.Mapper(categoryDetail);

                    EmployeeDetailViewModel.RateSubCategoryList = RateSubCategoryRepo.FindBy(m => m.CategoryId == CategoryId).Select(m => new SelectListItem()
                    {
                        Text = m.SubCategoryName,
                        Value = m.SubCategoryId.ToString()
                    }).OrderBy(m => m.Text).ToList();

                    return Json(EmployeeDetailViewModel.RateSubCategoryList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        ///partial view  _AddEmployeeHolidays
        /// <summary>
        /// _AddEditHolidays
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult _AddEmployeeHolidays()
        {
            try
            {
                VacationViewModel employeeHolidayViewModel = new VacationViewModel();
                employeeHolidayViewModel.OTRWList = NetUsers.GetOTRWUser().Select(m => new SelectListItem { Text = m.UserName, Value = m.Id }).ToList();
                return PartialView("_AddEmployeeHolidays", employeeHolidayViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //post:partial view  _AddEditHolidays
        /// <summary>
        /// _AddEditHolidays
        /// </summary>
        /// <param name="publicHolidayViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _AddEmployeeHolidays(VacationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    model.IsDelete = false;
                    model.CreatedBy = Guid.Parse(base.GetUserId);
                    model.CreatedDate = DateTime.Now;
                    model.Status = Constant.VacationType.Approved;
                    CommonMapper<VacationViewModel, Vacation> mapper = new CommonMapper<VacationViewModel, Vacation>();
                    Vacation vacation = mapper.Mapper(model);
                    VacationRepo.Add(vacation);
                    VacationRepo.Save();
                    TempData["Message"] = 1;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added employee leave.");

                    return RedirectToAction("GetVacations");
                }
                else
                {
                    return PartialView(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET : Employee/Employee/DeleteEmployeeHoliday
        /// <summary>
        /// DeleteHoliday
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteEmployeeHoliday(string id)
        {
            try
            {
                using (IHoliday)
                {
                    var Id = Guid.Parse(id);
                    var Holiday = VacationRepo.FindBy(i => i.Id == Id).FirstOrDefault();
                    VacationRepo.Delete(Holiday);
                    VacationRepo.Save();
                    TempData["Message"] = 3;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted employee leave.");

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GenerateReports()
        {
            try
            {
                ReportsViewModel model = new ReportsViewModel();
                model.EmployeeList = EmployeeJobRep.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).OrderBy(m => m.Text).ToList();


                //model.Joblist = EmployeeInvoiceRep.FindBy(m => m.IsDelete == false).Select(m =>
                //  new SelectListItem
                //  {
                //      Text = m.JobId.ToString(),
                //      Value = m.EmployeeJobId.ToString()

                //  }).Take(20).ToList();
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult LabourCost(List<LabourCostViewModel> LabourCost)
        {
            return View(LabourCost);
        }
        [HttpGet]
        public ActionResult LabourCostPerHour(List<FSM.Web.Areas.Employee.ViewModels.LabourCostperhourReportViewModel> HourList)
        {
            return View(HourList);
        }
        [HttpGet]
        public ActionResult PaolhReport(List<FSM.Web.Areas.Employee.ViewModels.PaolhReportViewModel> PaolhReport)
        {
            return View(PaolhReport);
        }
        [HttpGet]
        public ActionResult SalesBonusReport(List<FSM.Web.Areas.Employee.ViewModels.SalesBonusViewModel> Salesbonus)
        {
            return View(Salesbonus);
        }

        [HttpGet]
        public ActionResult UnpaidInvoiceReport(List<UnpaidInvoiceReportViewModel> Unpaidlist)
        {
            return View(Unpaidlist);
        }
        [HttpGet]
        public ActionResult SalesInvoiceReport(List<InvoiceSalesReportViewModel> SalesInvoiceList)
        {
            return View(SalesInvoiceList);
        }

        [HttpGet]
        public ActionResult OperationReport(List<OperationalViewModel> OperationList)
        {
            return View(OperationList);
        }
        [HttpGet]
        public ActionResult PerformanceBonusReport(List<PerformanceBonusReportViewModel> performanceBonuslist)
        {
            return View(performanceBonuslist);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateReports(ReportsViewModel model)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    switch (model.ReportType)
                    {
                        case Constant.TimesheetReportType.LabourTimeSheet:
                            if (model.EmployeeIds == null)
                            {
                                ModelState.AddModelError("EmployeeId", @"select atleast one employee");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.StartDate == null)
                            {
                                ModelState.AddModelError("StartDate", @"select start Date");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.EndDate == null)
                            {
                                ModelState.AddModelError("EndDate", @"select End Date.");
                                model.EmployeeList = GetEmployeeList();
                                //  model.Joblist = GetJobList();
                                return View(model);

                            }

                            if (model.StartDate != null && model.EndDate != null)
                            {
                                if (model.StartDate > model.EndDate)
                                {
                                    ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                    ModelState.AddModelError("StartDate", @"select start Date");
                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);

                                }
                            }
                            var data = TimeSheetRepository.GetLabourCost(model.StartDate, model.EndDate, model.EmployeeIds);
                            List<FSM.Web.Areas.Employee.ViewModels.LabourCostperhourReportViewModel> labourperhour = new List<FSM.Web.Areas.Employee.ViewModels.LabourCostperhourReportViewModel>();
                            if (data != null)
                            {
                                foreach (var emp in data.ToList())
                                {
                                    var isEmpExistforDate = labourperhour.Where(i => i.EID == emp.EID && i.jobDate == emp.jobDate).ToList();
                                    if (isEmpExistforDate.Count == 0)
                                    {
                                        FSM.Web.Areas.Employee.ViewModels.LabourCostperhourReportViewModel item = new ViewModels.LabourCostperhourReportViewModel();
                                        item.EID = emp.EID;
                                        item.EmployeeName = emp.Name;
                                        item.jobDate = emp.jobDate;
                                        item.Reason = emp.Reason;
                                        item.TimeSpent = emp.TimeSpent;
                                        item.ST = emp.ST;
                                        // var hourminutes = employeeReason.TimeSpent;
                                        var fthours = (emp.FTOLHour * 60 * 100) / 100;
                                        var fminutes = fthours;
                                        int fhour = Convert.ToInt32(fminutes / 60);
                                        int fminute = Convert.ToInt32(fminutes % 60);
                                        string ftotal = "00:00:00";
                                        if (fminute > 0)
                                        {
                                            ftotal = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":" + ((fminute < 10) ? "0" + fminute : fminute.ToString()) + ":00";
                                        }
                                        else
                                        {
                                            ftotal = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":00:00";
                                        }
                                        // item.LunchBreak = employeeReason.TimeSpent;
                                        item.FTOLHour = ftotal;
                                        //item.FTOLHour = emp.FTOLHour;



                                        var EmpJobdetails = data.Where(i => i.EID == emp.EID && i.jobDate == emp.jobDate).ToList();
                                        foreach (var employeeReason in EmpJobdetails.ToList())
                                        {

                                            string Hours, minutes, seconds;
                                            switch (employeeReason.Reason.ToString().ToLower())
                                            {
                                                case "lunch":

                                                    Hours = (employeeReason.TimeSpentHour < 10) ? "0" + employeeReason.TimeSpentHour.ToString() : employeeReason.TimeSpentHour.ToString();
                                                    minutes = (employeeReason.TimeSpentMinute < 10) ? "0" + employeeReason.TimeSpentMinute.ToString() : employeeReason.TimeSpentMinute.ToString();
                                                    seconds = (employeeReason.TimeSpentSecond < 10) ? "0" + employeeReason.TimeSpentSecond.ToString() : employeeReason.TimeSpentSecond.ToString();

                                                    if (Convert.ToInt16(Hours) > 0)
                                                    {
                                                        item.Luchhourcal = Hours;
                                                    }

                                                    if (Convert.ToInt16(minutes) > 30)
                                                    {
                                                        item.lunchminutecal = minutes;
                                                    }
                                                    item.Lunch_Break = Hours + ":" + minutes + ":" + seconds;


                                                    if (Convert.ToInt32(item.Luchhourcal) < 1 || Convert.ToInt32(item.lunchminutecal) < 30)
                                                    {
                                                        item.LucnchTime = "OK";
                                                    }
                                                    else
                                                    {
                                                        item.LucnchTime = "Too Long";
                                                    }

                                                    break;
                                                case "personal":
                                                    Hours = (employeeReason.TimeSpentHour < 10) ? "0" + employeeReason.TimeSpentHour.ToString() : employeeReason.TimeSpentHour.ToString();
                                                    minutes = (employeeReason.TimeSpentMinute < 10) ? "0" + employeeReason.TimeSpentMinute.ToString() : employeeReason.TimeSpentMinute.ToString();
                                                    seconds = (employeeReason.TimeSpentSecond < 10) ? "0" + employeeReason.TimeSpentSecond.ToString() : employeeReason.TimeSpentSecond.ToString();
                                                    item.Personal_Time = Hours + ":" + minutes + ":" + seconds;
                                                    break;
                                                case "travelling":
                                                    Hours = (employeeReason.TimeSpentHour < 10) ? "0" + employeeReason.TimeSpentHour.ToString() : employeeReason.TimeSpentHour.ToString();
                                                    minutes = (employeeReason.TimeSpentMinute < 10) ? "0" + employeeReason.TimeSpentMinute.ToString() : employeeReason.TimeSpentMinute.ToString();
                                                    seconds = (employeeReason.TimeSpentSecond < 10) ? "0" + employeeReason.TimeSpentSecond.ToString() : employeeReason.TimeSpentSecond.ToString();

                                                    item.Hours_Traveled = Hours + ":" + minutes + ":" + seconds;
                                                    break;
                                                case "job":
                                                    Hours = (employeeReason.TimeSpentHour < 10) ? "0" + employeeReason.TimeSpentHour.ToString() : employeeReason.TimeSpentHour.ToString();
                                                    minutes = (employeeReason.TimeSpentMinute < 10) ? "0" + employeeReason.TimeSpentMinute.ToString() : employeeReason.TimeSpentMinute.ToString();
                                                    seconds = (employeeReason.TimeSpentSecond < 10) ? "0" + employeeReason.TimeSpentSecond.ToString() : employeeReason.TimeSpentSecond.ToString();
                                                    item.Hours_Worked = Hours + ":" + minutes + ":" + seconds;
                                                    break;
                                            }
                                        }
                                        item.HoursTotal = "";
                                        if (!string.IsNullOrEmpty(item.Hours_Worked) || !string.IsNullOrEmpty(item.Hours_Traveled))
                                        {
                                            int toalhours, minutes, seconds, extraminute = 0, extrahour = 0;
                                            string[] Hourworked = null, Hours_Traveled = null;
                                            if (item.Hours_Worked != null) { Hourworked = item.Hours_Worked.Split(':'); } else { Hourworked = "00:00:00".Split(':'); }
                                            if (item.Hours_Traveled != null) { Hours_Traveled = item.Hours_Traveled.Split(':'); } else { Hours_Traveled = "00:00:00".Split(':'); }

                                            seconds = Convert.ToInt32(Hourworked[2]) + Convert.ToInt32(Hours_Traveled[2]);
                                            if (seconds > 60)
                                            {
                                                int cal = seconds;
                                                seconds = seconds % 60;
                                                extraminute = cal / 60;
                                                // toalhours = toalhours + extrahour;
                                            }

                                            minutes = Convert.ToInt32(Hourworked[1]) + Convert.ToInt32(Hours_Traveled[1]);
                                            minutes = minutes + extraminute;
                                            if (minutes > 60)
                                            {
                                                int calm = seconds;
                                                minutes = minutes % 60;
                                                extrahour = calm / 60;
                                                //toalhours = toalhours + extrahour;
                                            }

                                            toalhours = Convert.ToInt32(Hourworked[0]) + Convert.ToInt32(Hours_Traveled[0]);
                                            toalhours = toalhours + extrahour;
                                            //  toalhours.ToString() =
                                            item.HoursTotal = Convert.ToString((toalhours) < 10 ? "0" + toalhours.ToString() : toalhours.ToString()) + ":" + ((minutes < 10) ? "0" + minutes.ToString() : minutes.ToString()) + ":" + ((seconds < 10) ? "0" + seconds.ToString() : seconds.ToString());
                                        }
                                        else
                                        {
                                            item.HoursTotal = "00:00:00";
                                        }
                                        if (!string.IsNullOrEmpty(item.Hours_Traveled))
                                        {
                                            item.HoursTotal = Convert.ToString(item.Hours_Traveled);
                                        }
                                        if (String.IsNullOrEmpty(item.Lunch_Break))
                                        {
                                            item.Lunch_Break = "00:00:00";
                                        }

                                        if (String.IsNullOrEmpty(item.Hours_Worked))
                                        {
                                            item.Hours_Worked = "00:00:00";
                                        }


                                        if (String.IsNullOrEmpty(item.LucnchTime))
                                        {
                                            item.LucnchTime = "OK";
                                        }

                                        if (String.IsNullOrEmpty(item.Personal_Time))
                                        {
                                            item.Personal_Time = "00:00:00";
                                        }


                                        if (String.IsNullOrEmpty(item.Hours_Traveled))
                                        {
                                            item.Hours_Traveled = "00:00:00";
                                        }


                                        var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                        var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                        item.StartingDate = Startdate;
                                        item.EndingDate = End;


                                        var chours = (emp.FTOLHour * 60 * 100) / 100;
                                        var cminutes = chours;
                                        int chour = Convert.ToInt32(cminutes / 60);
                                        int cminute = Convert.ToInt32(cminutes % 60);

                                        string ctotal = "00:00:00";



                                        if (cminute > 0)
                                        {
                                            //  ctotal = chour + "." + cminute;
                                            ctotal = ((chour < 9) ? "0" + chour.ToString() : chour.ToString()) + ":" + ((cminute < 10) ? "0" + cminute : cminute.ToString()) + ":00";
                                        }
                                        else
                                        {
                                            ctotal = ((chour < 9) ? "0" + chour.ToString() : chour.ToString()) + ":00:00";
                                        }
                                        item.CorrectedTime = ctotal;

                                        if (Convert.ToDecimal(item.lunchminutecal) > Convert.ToDecimal(30))
                                        {
                                            string[] corrected = ctotal.Split(':');
                                            int totalminutes = Convert.ToInt32(corrected[1]);//minutes
                                            int lunchExtraTime = Convert.ToInt32(item.lunchminutecal) - 30;

                                            totalminutes = totalminutes - lunchExtraTime;
                                            if (totalminutes < 0)
                                            {
                                                chour = chour - 1;
                                                totalminutes = Convert.ToInt32(60) + Convert.ToInt32(totalminutes);

                                            }
                                            ctotal = ((chour < 9) ? "0" + chour.ToString() : chour.ToString()) + ":" + ((totalminutes < 10) ? "0" + totalminutes : totalminutes.ToString()) + ":00";
                                            item.CorrectedTime = ctotal;
                                        }
                                        labourperhour.Add(item);
                                    }
                                }
                            }

                            return new ViewAsPdf("LabourCostPerHour", labourperhour)
                            {
                                FileName = "LabourCostReport.pdf"
                            };

                        case Constant.TimesheetReportType.LabourCostPerHour:

                            var timesheetdata = TimeSheetRepository.GetLabourCostPerHour(model.EmployeeIds, model.StartDate, model.EndDate);
                            List<LabourCostViewModel> HourList = new List<LabourCostViewModel>();
                            foreach (var i in timesheetdata)
                            {
                                LabourCostViewModel item = new LabourCostViewModel();
                                item.EID = i.EID;
                                item.EmployeeId = i.EmployeeId;
                                item.Name = i.Name;
                                item.BaseRate = i.BaseRate;
                                item.S_WC = i.S_WC;
                                item.AL_PH = i.AL_PH;
                                item.TAFE = i.TAFE;
                                item.Payroll = i.Payroll;
                                item.Cont_MV_EQ_Cost = i.Cont_MV_EQ_Cost;
                                item.Emp_MV_Cost = i.Emp_MV_Cost;
                                item.Equip_Cost = i.Equip_Cost;
                                item.Emp_Mob_Ph_Cost = i.Emp_Mob_Ph_Cost;
                                item.GrossLabourCost = i.GrossLabourCost;
                                HourList.Add(item);
                            }

                            return new ViewAsPdf("LabourCost", HourList)
                            {
                                FileName = "LabourCostperHourReport.pdf"
                            };

                        case Constant.TimesheetReportType.SalesbonusReport:

                            //if (model.JobIDs == null)
                            //{
                            //    model.EmployeeList = GetEmployeeList();
                            //    model.Joblist = GetJobList();
                            //    ModelState.AddModelError("Jobid", @"Please select Job");
                            //    //ModelState.AddModelError("StartDate", @"select start Date");
                            //    return View(model);
                            //}
                            if (model.EmployeeIds == null)
                            {
                                ModelState.AddModelError("EmployeeId", @"select atleast one employee");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.StartDate == null)
                            {
                                ModelState.AddModelError("StartDate", @"select start Date");

                                model.EmployeeList = GetEmployeeList();

                                return View(model);
                            }
                            if (model.EndDate == null)
                            {
                                ModelState.AddModelError("EndDate", @"select End Date.");
                                model.EmployeeList = GetEmployeeList();
                                return View(model);
                            }

                            if (model.StartDate != null && model.EndDate != null)
                            {
                                if (model.StartDate > model.EndDate)
                                {
                                    ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                    ModelState.AddModelError("StartDate", @"select start Date");
                                    model.EmployeeList = GetEmployeeList();
                                    return View(model);
                                }
                            }
                            var SalesBonusReport = TimeSheetRepository.GetSalesBonusReport(model.EmployeeIds, model.StartDate, model.EndDate);
                            List<SalesBonusViewModel> SalesList = new List<SalesBonusViewModel>();
                            foreach (var i in SalesBonusReport)
                            {
                                SalesBonusViewModel item = new SalesBonusViewModel();
                                item.SaleIncome = i.SaleIncome;
                                item.JSPOCost = i.JSPOCost;
                                item.StockItemCost = i.StockItemCost;
                                item.SalesBonus = i.SalesBonus;
                                item.TotalCost = i.TotalCost;
                                item.LabourIncome = i.LabourIncome;
                                item.LabourCost = i.LabourCost;
                                item.LabourProfit = i.LabourProfit;
                                item.RevHours = i.RevHours;
                                if (String.IsNullOrEmpty(i.SiteFileName))
                                {
                                    item.SiteFileName = "";

                                }
                                else
                                {
                                    item.SiteFileName = i.SiteFileName;
                                }
                                var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                item.StartDate = Startdate;
                                item.EndDate = End;
                                if (item.RevHours.HasValue)
                                {
                                    var hours = (item.RevHours * 60 * 100) / 100;
                                    var minutes = hours;
                                    int fhour = Convert.ToInt32(minutes / 60);
                                    int fminute = Convert.ToInt32(minutes % 60);
                                    string total = "00:00:00";
                                    if (fminute > 0)
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":" + ((fminute < 10) ? "0" + fminute : fminute.ToString()) + ":00";
                                    }
                                    else
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":00:00";
                                    }
                                    item.Rev_Hours = total;

                                }

                                else { item.Rev_Hours = "00:00:00"; }


                                item.NRLHours = i.NRLHours;
                                if (item.NRLHours.HasValue)
                                {
                                    var hours = (item.NRLHours * 60 * 100) / 100;
                                    var minutes = hours;
                                    int fhour = Convert.ToInt32(minutes / 60);
                                    int fminute = Convert.ToInt32(minutes % 60);
                                    string total = "00:00:00";
                                    if (fminute > 0)
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":" + ((fminute < 10) ? "0" + fminute : fminute.ToString()) + ":00";
                                    }
                                    else
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":00:00";
                                    }
                                    item.NRL_Hours = total;

                                }
                                else { item.NRL_Hours = "00:00:00"; }
                                item.InvoiceDate = i.InvoiceDate;
                                item.CustomerLastName = i.CustomerLastName;
                                item.LabourCostPerHour = i.LabourCostPerHour;
                                item.LabourProfitPerHour = i.LabourProfitPerHour;
                                item.LobourIncomePerHour = i.LobourIncomePerHour;
                                item.InvoiceNo = i.InvoiceNo;
                                item.LabourHours = i.LabourHours;
                                item.OTRWName = i.OTRWName;
                                if (item.LabourHours.HasValue)
                                {
                                    var hours = (item.LabourHours * 60 * 100) / 100;
                                    var minutes = hours;
                                    int fhour = Convert.ToInt32(minutes / 60);
                                    int fminute = Convert.ToInt32(minutes % 60);
                                    string total = "00:00:00";
                                    if (fminute > 0)
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":" + ((fminute < 10) ? "0" + fminute : fminute.ToString()) + ":00";
                                    }
                                    else
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":00:00";
                                    }
                                    item.Labour_Hours = total;

                                }
                                else { item.Labour_Hours = "00:00:00"; }



                                SalesList.Add(item);
                            }
                            return new ViewAsPdf("SalesBonusReport", SalesList)
                            {
                                FileName = "SalesBonusReport.pdf",
                                PageOrientation = Rotativa.Options.Orientation.Landscape,
                                PageSize = Rotativa.Options.Size.A4
                            };


                        case Constant.TimesheetReportType.UnpaidInvoice:
                            if (model.StartDate == null)
                            {
                                ModelState.AddModelError("StartDate", @"select start Date");

                                model.EmployeeList = GetEmployeeList();
                                //model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.EndDate == null)
                            {
                                ModelState.AddModelError("EndDate", @"select End Date.");
                                model.EmployeeList = GetEmployeeList();
                                //model.Joblist = GetJobList();
                                return View(model);

                            }
                            if (model.StartDate != null && model.EndDate != null)
                            {
                                if (model.StartDate > model.EndDate)
                                {
                                    ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                    ModelState.AddModelError("StartDate", @"select start Date");
                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);

                                }
                            }
                            var unpaidReports = TimeSheetRepository.GetUnpaidReportList(model.StartDate, model.EndDate);
                            List<UnpaidInvoiceReportViewModel> unpaidlist = new List<UnpaidInvoiceReportViewModel>();
                            decimal? Total = Convert.ToDecimal(0.0);
                            foreach (var value in unpaidReports)
                            {
                                UnpaidInvoiceReportViewModel ob = new UnpaidInvoiceReportViewModel();
                                ob.JobNo = value.JobNo;
                                ob.JobStatus = value.JobStatus;
                                ob.Name = value.Name;
                                ob.SiteFileName = value.SiteFileName;
                                ob.JobType = value.JobType;
                                ob.InvoiceNo = value.InvoiceNo;
                                ob.Amount = value.Amount;
                                var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                ob.StartDate = Startdate;
                                ob.EndDate = End;
                                ob.DateBooked = value.DateBooked.HasValue ? value.DateBooked.Value.ToString("yyyy-MM-dd") : string.Empty;
                                ob.InvoiceDate = value.InvoiceDate.HasValue ? value.InvoiceDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                var EmployeeForjob = EmployeeInvoiceRep.FindBy(k => k.InvoiceNo == value.InvoiceNo);
                                if (EmployeeForjob != null)
                                {
                                    string OtrwListforjob = "";
                                    foreach (var emp in EmployeeForjob.ToList())
                                    {
                                        string EmployeeName = Employee.FindBy(k => k.EmployeeId == emp.OTRWAssigned).Select(j => j.FirstName + " " + j.LastName).FirstOrDefault();
                                        OtrwListforjob = OtrwListforjob + EmployeeName + ",";
                                    }
                                    ob.Name = OtrwListforjob.TrimEnd(',');
                                }
                                Total = Total + value.Amount;
                                unpaidlist.Add(ob);
                            }

                            if (unpaidlist.Count > 0)
                            {
                                UnpaidInvoiceReportViewModel obj = new UnpaidInvoiceReportViewModel();
                                obj.Amount = Total;
                                obj.SiteFileName = "Total";
                                var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                obj.StartDate = Startdate;
                                obj.EndDate = End;
                                unpaidlist.Add(obj);
                            }

                            return new ViewAsPdf("UnpaidInvoiceReport", unpaidlist)
                            {
                                FileName = "UnpaidInvoiceReport.pdf"

                            };

                        case Constant.TimesheetReportType.PerformanceBonusReport:
                            if (model.EmployeeIds == null)
                            {
                                ModelState.AddModelError("EmployeeId", @"select atleast one employee");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.StartDate == null)
                            {
                                ModelState.AddModelError("StartDate", @"select start Date");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.EndDate == null)
                            {
                                ModelState.AddModelError("EndDate", @"select End Date.");
                                model.EmployeeList = GetEmployeeList();
                                //  model.Joblist = GetJobList();
                                return View(model);

                            }

                            if (model.StartDate != null && model.EndDate != null)
                            {
                                if (model.StartDate > model.EndDate)
                                {
                                    ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                    ModelState.AddModelError("StartDate", @"select start Date");
                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);

                                }
                            }
                            var PerformanceReportList = TimeSheetRepository.GetPerformanceBonusReportList(model.StartDate, model.EndDate, model.EmployeeIds);
                            List<PerformanceBonusReportViewModel> performanceBonuslist = new List<PerformanceBonusReportViewModel>();
                            foreach (var value in PerformanceReportList)
                            {
                                PerformanceBonusReportViewModel obPerformnce = new PerformanceBonusReportViewModel();
                                obPerformnce.EmployeeName = value.EmployeeName;
                                obPerformnce.HoursWorked = value.HoursWorked;
                                obPerformnce.LabourProfit = value.LabourProfit;
                                obPerformnce.LabourProfitPerHour = value.LabourProfitPerHour;
                                obPerformnce.PerformanceBonusRate = value.PerformanceBonusRate;
                                obPerformnce.PerformanceBonus = value.PerformanceBonus;

                                var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                obPerformnce.StartDate = Startdate;
                                obPerformnce.EndDate = End;

                                performanceBonuslist.Add(obPerformnce);
                            }
                            return new ViewAsPdf("PerformanceBonusReport", performanceBonuslist)
                            {
                                FileName = "PerformanceBonusReport.pdf"

                            };

                            break;

                        #region Operational Report
                        case Constant.TimesheetReportType.OpertationalReport:

                            if (model.StartDate == null)
                            {
                                ModelState.AddModelError("StartDate", @"select start Date");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.EndDate == null)
                            {
                                ModelState.AddModelError("EndDate", @"select End Date.");
                                model.EmployeeList = GetEmployeeList();
                                //  model.Joblist = GetJobList();
                                return View(model);

                            }
                            if (model.StartDate != null && model.EndDate != null)
                            {
                                if (model.StartDate > model.EndDate)
                                {
                                    ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                    ModelState.AddModelError("StartDate", @"select start Date");
                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);

                                }

                            }

                            var operationreports = TimeSheetRepository.GetoperationalReport(model.StartDate, model.EndDate, Convert.ToInt32(model.Duration));
                            List<OperationalViewModel> operationalList = new List<OperationalViewModel>();
                            foreach (var value in operationreports)
                            {
                                OperationalViewModel ob = new OperationalViewModel();
                                ob.TotalEmployeeTime = value.TotalEmployeeTime;

                                if (ob.TotalEmployeeTime.HasValue)
                                {
                                    var hours = (ob.TotalEmployeeTime * 60 * 100) / 100;
                                    var minutes = hours;
                                    int fhour = Convert.ToInt32(minutes / 60);
                                    int fminute = Convert.ToInt32(minutes % 60);
                                    string total = "00:00:00";
                                    if (fminute > 0)
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":" + ((fminute < 10) ? "0" + fminute : fminute.ToString()) + ":00";
                                    }
                                    else
                                    {
                                        total = ((fhour < 9) ? "0" + fhour.ToString() : fhour.ToString()) + ":00:00";
                                    }
                                    ob.Total_EmployeeTime = total;

                                }
                                else { ob.Total_EmployeeTime = "00:00:00"; }
                                var Startdate = value.JobStartDate.HasValue ? value.JobStartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                var End = value.JobEndDate.HasValue ? value.JobEndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                ob.JobStartDate = Startdate;
                                ob.JobEndDate = End;
                                ob.TimeSpentinDays = value.TimeSpentinDays;
                                ob.Price = value.Price;
                                ob.LabourCost = value.LabourCost;
                                ob.LabourCostPerHour = value.LabourCostPerHour;
                                ob.LabourIncome = value.LabourIncome;
                                // Constant.OperationalReportType Durations = (Constant.OperationalReportType)model.Duration;
                                //  var seasonDisplayName = Durations.GetAttribute<DisplayAttribute>();
                                //ob.OperationReportType = seasonDisplayName.Name;
                                ob.LabourIncomePerHour = value.LabourIncomePerHour;
                                ob.LabourProfit = value.LabourProfit;
                                ob.LabourProfitPerHour = value.LabourProfitPerHour;
                                operationalList.Add(ob);
                            }

                            return new ViewAsPdf("OperationReport", operationalList)
                            {
                                FileName = "OperationalReport.pdf"
                            };
                            break;
                        #endregion


                        #region PaolhReport
                        case Constant.TimesheetReportType.PaolhReport:
                            if (model.EmployeeIds == null)
                            {
                                ModelState.AddModelError("EmployeeId", @"select atleast one employee");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.StartDate == null)
                            {
                                ModelState.AddModelError("StartDate", @"select start Date");

                                model.EmployeeList = GetEmployeeList();
                                // model.Joblist = GetJobList();
                                return View(model);
                            }
                            if (model.EndDate == null)
                            {
                                ModelState.AddModelError("EndDate", @"select End Date.");
                                model.EmployeeList = GetEmployeeList();
                                //  model.Joblist = GetJobList();
                                return View(model);

                            }

                            if (model.StartDate != null && model.EndDate != null)
                            {
                                if (model.StartDate > model.EndDate)
                                {
                                    ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                    ModelState.AddModelError("StartDate", @"select start Date");
                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);

                                }
                            }
                            var dataPaolh = TimeSheetRepository.GetPaolhReport(model.StartDate, model.EndDate, model.EmployeeIds);
                            List<FSM.Web.Areas.Employee.ViewModels.PaolhReportViewModel> paolhReport = new List<FSM.Web.Areas.Employee.ViewModels.PaolhReportViewModel>();
                            if (dataPaolh != null)
                            {
                                TimeSpan ts=new TimeSpan();
                                decimal totalHourAssigned =Convert.ToDecimal(0);
                                Double totalPer = Convert.ToDouble(0);
                                foreach (var emp in dataPaolh.ToList())
                                {
                                    if (emp.WorkedHour == "0")
                                    {
                                    }
                                    else { 
                                    string stringTime = emp.WorkedHour;
                                    string[] values = stringTime.Split(':');

                                    TimeSpan ts1 = new TimeSpan(Convert.ToInt16(values[0]), Convert.ToInt16(values[1]), Convert.ToInt16(values[2]));

                                    ts = ts + ts1;

                                    }

                                    totalHourAssigned = totalHourAssigned + Convert.ToDecimal(emp.HourAssigned);
                                    totalPer= totalPer + Convert.ToDouble(emp.Percentage);
                                }

                                    foreach (var emp in dataPaolh.ToList())
                                {
                                    FSM.Web.Areas.Employee.ViewModels.PaolhReportViewModel item = new ViewModels.PaolhReportViewModel();
                                    item.EID = emp.EID;
                                    item.OTRWName = emp.OTRWName;
                                    item.HourAssigned =Convert.ToDecimal(emp.HourAssigned);
                                    item.WorkedHour = emp.WorkedHour;
                                    item.StartingDate = Convert.ToDateTime(model.StartDate).ToString("yyyy-MM-dd");
                                    item.EndingDate = Convert.ToDateTime(model.EndDate).ToString("yyyy-MM-dd");
                                    item.totalHourAssigned = totalHourAssigned;
                                    item.TotalHourworked = Convert.ToString((int)ts.TotalHours + ts.ToString(@"\:mm\:ss"));
                                    item.TotalPerCen =totalPer;
                                    item.Percentage = emp.Percentage;
                                    paolhReport.Add(item);
                                }
                            }

                            return new ViewAsPdf("PaolhReport", paolhReport)
                            {
                                FileName = "PaolhReport.pdf"
                            };
                            break;
                        #endregion



                        #region Sales invoice Report
                        case Constant.TimesheetReportType.SalesInvoiceReport:
                            {
                                if (model.StartDate == null)
                                {
                                    ModelState.AddModelError("StartDate", @"select start Date");

                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);
                                }
                                if (model.EndDate == null)
                                {
                                    ModelState.AddModelError("EndDate", @"select End Date.");
                                    model.EmployeeList = GetEmployeeList();
                                    //model.Joblist = GetJobList();
                                    return View(model);

                                }
                                if (model.StartDate != null && model.EndDate != null)
                                {
                                    if (model.StartDate > model.EndDate)
                                    {
                                        ModelState.AddModelError("StartDate", @"start date can't be greater than end date");
                                        ModelState.AddModelError("StartDate", @"select start Date");
                                        model.EmployeeList = GetEmployeeList();
                                        //model.Joblist = GetJobList();
                                        return View(model);

                                    }
                                }
                                var GerInvoiceSalesReport = TimeSheetRepository.InvoiceSalesReportDailyOrByRange(model.StartDate, model.EndDate);
                                List<InvoiceSalesReportViewModel> SalesInvoiceList = new List<InvoiceSalesReportViewModel>();
                                decimal? SalesInvoiceListTotal = Convert.ToDecimal(0.0);
                                foreach (var value in GerInvoiceSalesReport)
                                {
                                    InvoiceSalesReportViewModel ob = new InvoiceSalesReportViewModel();
                                    ob.JobNo = value.JobNo;
                                    ob.JobStatus = value.JobStatus;
                                    ob.Name = value.Name;
                                    ob.SiteFileName = value.SiteFileName;
                                    ob.JobType = value.JobType;
                                    ob.InvoiceNo = value.InvoiceNo;
                                    ob.Amount = value.Amount;
                                    var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                    var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                    ob.StartDate = Startdate;
                                    ob.EndDate = End;
                                    ob.DateBooked = value.DateBooked.HasValue ? value.DateBooked.Value.ToString("yyyy-MM-dd") : string.Empty;
                                    ob.InvoiceDate = value.InvoiceDate.HasValue ? value.InvoiceDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                    var EmployeeForjob = EmployeeInvoiceRep.FindBy(k => k.InvoiceNo == value.InvoiceNo);
                                    if (EmployeeForjob != null)
                                    {
                                        string OtrwListforjob = "";
                                        foreach (var emp in EmployeeForjob.ToList())
                                        {
                                            string EmployeeName = Employee.FindBy(k => k.EmployeeId == emp.OTRWAssigned).Select(j => j.FirstName + " " + j.LastName).FirstOrDefault();
                                            OtrwListforjob = OtrwListforjob + EmployeeName + ",";
                                        }
                                        ob.Name = OtrwListforjob.TrimEnd(',');
                                    }
                                    SalesInvoiceList.Add(ob);
                                    SalesInvoiceListTotal = SalesInvoiceListTotal + value.Amount;
                                }

                                if (SalesInvoiceList.Count > 0)
                                {
                                    InvoiceSalesReportViewModel obj = new InvoiceSalesReportViewModel();
                                    obj.Amount = SalesInvoiceListTotal;
                                    obj.SiteFileName = "Total";
                                    var Startdate = model.StartDate.HasValue ? model.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                    var End = model.EndDate.HasValue ? model.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                                    obj.StartDate = Startdate;
                                    obj.EndDate = End;
                                    SalesInvoiceList.Add(obj);
                                }

                                return new ViewAsPdf("SalesInvoiceReport", SalesInvoiceList)
                                {
                                    FileName = "SalesInvoiceReport.pdf"

                                };
                            }
                            #endregion
                    }


                }
                //model.Joblist = GetJobList();
                if (model.ReportType == 0)
                {
                    ModelState.AddModelError("ReportType", @"select Report Type");

                }
                if (model.ReportType == Constant.TimesheetReportType.LabourCostPerHour || model.ReportType == Constant.TimesheetReportType.LabourTimeSheet)
                {
                    if (model.EmployeeIds == null)
                        ModelState.AddModelError("EmployeeId", @"select Employee");
                }
                model.EmployeeList = GetEmployeeList();
                return View(model);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<SelectListItem> GetEmployeeList()
        {
            try
            {
                List<SelectListItem> EmployeeList = EmployeeJobRep.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id.ToString()
                }).ToList();
                return EmployeeList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<SelectListItem> GetJobList()
        {
            try
            {
                List<SelectListItem> Joblist = EmployeeInvoiceRep.FindBy(m => m.IsDelete == false).Select(m =>
                    new SelectListItem
                    {
                        Text = m.JobId.ToString(),
                        Value = m.EmployeeJobId.ToString()

                    }).ToList();
                return Joblist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //GET:Employee/Employee/ViewRoastedOff
        /// <summary>
        /// Get All Roasted Off For Employee
        /// </summary>
        /// <param name=""></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ViewRoastedOff()
        {
            try
            {
                var keyWordSearch = "";
                var RoastedOffList = RoastefOffRepo.GetRoastedOffList(keyWordSearch).ToList();
                int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;

                if (RoastedOffList.Count > 0)
                {
                    foreach (var emp in RoastedOffList)
                    {
                        emp.Days = (int)emp.DayId > 0 ? Convert.ToString((Constant.EmployeeRoastdDays)emp.DayId) : "Not Available";
                    }
                }

                var employeeRoastedOffListViewModel = new EmployeeRoastedOffListViewModel
                {
                    RoastedOffCoreListViewModel = RoastedOffList,
                    PageSize = PageSize
                };

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " View Roasted Off");

                return View(employeeRoastedOffListViewModel);
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);

                throw ex;
            }
        }


        //GET:Employee/Employee/ViewRoastedOffPartial
        /// <summary>
        /// Get All Roasted Off For Employee With Searching
        /// </summary>
        /// <param name=""></param>
        /// <returns>Model</returns>
        [ValidateInput(false)]
        public ActionResult ViewRoastedOffPartial()
        {
            try
            {
                string keyWordSearch = ControllerContext.HttpContext.Request.Unvalidated.QueryString["EmployeeKeyword"] != null ?
                                    ControllerContext.HttpContext.Request.Unvalidated.QueryString["EmployeeKeyword"] : string.Empty;

                var RoastedOffList = RoastefOffRepo.GetRoastedOffList(keyWordSearch).ToList();

                int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;

                if (RoastedOffList.Count > 0)
                {
                    foreach (var emp in RoastedOffList)
                    {
                        emp.Days = (int)emp.DayId > 0 ? Convert.ToString((Constant.EmployeeRoastdDays)emp.DayId) : "Not Available";
                    }
                }

                var employeeRoastedOffListViewModel = new EmployeeRoastedOffListViewModel
                {
                    RoastedOffCoreListViewModel = RoastedOffList,
                    PageSize = PageSize
                };

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed list of all Roasted off.");

                return PartialView("_EmployeeRoastedOffList", employeeRoastedOffListViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        ///GET: Employee/Employee/_AddRoastedOff
        /// <summary>
        ///Add Edit Roasted Off
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _AddRoastedOff(string Id)
        {
            try
            {
                EmployeeRoastedOffViewModel model = new EmployeeRoastedOffViewModel();

                if (Id != null)
                {
                    Guid RoastedId = Guid.Parse(Id);
                    RoastedOff roastedEntity = RoastefOffRepo.FindBy(m => m.ID == RoastedId).FirstOrDefault();

                    CommonMapper<RoastedOff, EmployeeRoastedOffViewModel> mapper = new CommonMapper<RoastedOff, EmployeeRoastedOffViewModel>();
                    model = mapper.Mapper(roastedEntity);

                    var getDayName = Enum.GetName(typeof(DayOfWeek), Convert.ToInt32((model.DayId)));
                    var Weeks = from EmployeeRoastdWeeks e in Enum.GetValues(typeof(EmployeeRoastdWeeks))
                                select new
                                {
                                    Id = (int)e,
                                    Name = e.ToString()
                                };
                    List<SelectListItem> lstEmployeeRoastdWeeks = new List<SelectListItem>();

                    foreach (var item in Weeks)
                    {
                        SelectListItem lstEmp = new SelectListItem();
                        lstEmp.Text = item.Name + " " + getDayName;
                        lstEmp.Value = item.Id.ToString();
                        lstEmployeeRoastdWeeks.Add(lstEmp);
                    }
                    model.Weeks = lstEmployeeRoastdWeeks;

                    model.OTRWList = EmployeeJobRepo.GetOTRWUser().Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id
                    }).OrderBy(m => m.Text).ToList();

                    model.WeekId = RoastedOffWeekMappingRepo.FindBy(m => m.RoastedOffId == RoastedId)
                                           .Select(m => m.WeekID).ToList();

                    return PartialView(model);
                }
                else
                {
                    var Weeks = from EmployeeRoastdWeeks e in Enum.GetValues(typeof(EmployeeRoastdWeeks))
                                select new
                                {
                                    Id = (int)e,
                                    Name = e.ToString()
                                };
                    List<SelectListItem> lstEmployeeRoastdWeeks = new List<SelectListItem>();

                    foreach (var item in Weeks)
                    {
                        SelectListItem lstEmp = new SelectListItem();
                        lstEmp.Text = item.Name;
                        lstEmp.Value = item.Id.ToString();
                        lstEmployeeRoastdWeeks.Add(lstEmp);
                    }
                    model.Weeks = lstEmployeeRoastdWeeks;

                    model.OTRWList = EmployeeJobRepo.GetOTRWUser().Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id
                    }).OrderBy(m => m.Text).ToList();



                    return PartialView(model);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        ///POST: Employee/Employee/_AddRoastedOff
        /// <summary>
        ///Add Edit Roasted Off
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public ActionResult _AddRoastedOff(EmployeeRoastedOffViewModel model)
        {
            try
            {
                string PageSize = !string.IsNullOrEmpty(Request.QueryString["PageSize"]) ?
                                               Request.QueryString["PageSize"] : "10";
                Guid OTRWID = Guid.Parse(model.OTRWId);
                var days = Convert.ToInt32(model.DayId);
                days = days + 1;   //+ 1 add because day start with sunday but this project show first day monday

                if (model.ID != Guid.Empty)
                {

                    //update Data Into RoastedOff
                    CommonMapper<EmployeeRoastedOffViewModel, RoastedOff> mapper = new CommonMapper<EmployeeRoastedOffViewModel, RoastedOff>();
                    RoastedOff roastedOff = mapper.Mapper(model);

                    roastedOff.ModifiedDate = DateTime.Now;
                    roastedOff.ModeifiedBy = Guid.Parse(base.GetUserId);

                    RoastefOffRepo.DeAttach(roastedOff);
                    RoastefOffRepo.Edit(roastedOff);
                    RoastefOffRepo.Save();

                    //Update Data into RoastedOffWeekMapping

                    var WeekMapping = RoastedOffWeekMappingRepo.FindBy(m => m.RoastedOffId == model.ID).ToList();
                    foreach (var deleteWeek in WeekMapping)  //Delete Exist Data
                    {
                        RoastedOffWeekMappingRepo.Delete(deleteWeek);
                        RoastedOffWeekMappingRepo.Save();
                    }

                    //Delete Record which already insert in vacation table using roastedOffId

                    var vacationData = VacationRepo.FindBy(m => m.RoastedOffId == model.ID).ToList();
                    foreach (var vacation in vacationData)
                    {
                        vacation.IsDelete = true;

                        VacationRepo.DeAttach(vacation);
                        VacationRepo.Edit(vacation);
                        VacationRepo.Save();
                    }


                    foreach (var weeks in model.WeekId)   //Insert Data Again
                    {
                        RoastedOffWeekMapping roastedOffWeekMapping = new RoastedOffWeekMapping();
                        roastedOffWeekMapping.ID = Guid.NewGuid();
                        roastedOffWeekMapping.RoastedOffId = model.ID;
                        roastedOffWeekMapping.WeekID = weeks;

                        roastedOffWeekMapping.CreatedBy = Guid.Parse(base.GetUserId);
                        roastedOffWeekMapping.CreatedDate = DateTime.Now;

                        roastedOffWeekMapping.ModifiedDate = DateTime.Now;
                        roastedOffWeekMapping.ModeifiedBy = Guid.Parse(base.GetUserId);

                        RoastedOffWeekMappingRepo.Add(roastedOffWeekMapping);
                        RoastedOffWeekMappingRepo.Save();


                        //Insert data into vacation table
                        int week1 = 0;
                        int week2 = 0;
                        if (weeks == 1)
                        {
                            week1 = 7;
                            week2 = 0;
                        }
                        else if (weeks == 2)
                        {
                            week1 = 14;
                            week2 = 7;
                        }
                        else if (weeks == 3)
                        {
                            week1 = 21;
                            week2 = 14;
                        }
                        else if (weeks == 4)
                        {
                            week1 = 28;
                            week2 = 21;
                        }
                        else if (weeks == 5)
                        {
                            week1 = 35;
                            week2 = 28;
                        }

                        var insertVacation = RoastefOffRepo.InsertDataIntoVacation(model.ID, OTRWID, model.StartDate, model.EndDate, days, week1, week2);
                    }
                    TempData["Message"] = 2;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " Update roasted off");
                }
                else
                {

                    //Save Data Into RoastedOff
                    model.ID = Guid.NewGuid();
                    model.IsDelete = false;
                    CommonMapper<EmployeeRoastedOffViewModel, RoastedOff> mapper = new CommonMapper<EmployeeRoastedOffViewModel, RoastedOff>();
                    RoastedOff roastedOff = mapper.Mapper(model);

                    roastedOff.CreatedDate = DateTime.Now;
                    roastedOff.CreatedBy = Guid.Parse(base.GetUserId);

                    RoastefOffRepo.Add(roastedOff);
                    RoastefOffRepo.Save();

                    //Save Data into RoastedOffWeekMapping

                    foreach (var weeks in model.WeekId)
                    {
                        RoastedOffWeekMapping roastedOffWeekMapping = new RoastedOffWeekMapping();
                        roastedOffWeekMapping.ID = Guid.NewGuid();
                        roastedOffWeekMapping.RoastedOffId = model.ID;
                        roastedOffWeekMapping.WeekID = weeks;

                        roastedOffWeekMapping.CreatedBy = Guid.Parse(base.GetUserId);
                        roastedOffWeekMapping.CreatedDate = DateTime.Now;

                        RoastedOffWeekMappingRepo.Add(roastedOffWeekMapping);
                        RoastedOffWeekMappingRepo.Save();

                        //Insert data into vacation table
                        int week1 = 0;
                        int week2 = 0;
                        if (weeks == 1)
                        {
                            week1 = 7;
                            week2 = 0;
                        }
                        else if (weeks == 2)
                        {
                            week1 = 14;
                            week2 = 7;
                        }
                        else if (weeks == 3)
                        {
                            week1 = 21;
                            week2 = 14;
                        }
                        else if (weeks == 4)
                        {
                            week1 = 28;
                            week2 = 21;
                        }
                        else if (weeks == 5)
                        {
                            week1 = 35;
                            week2 = 28;
                        }

                        var insertVacation = RoastefOffRepo.InsertDataIntoVacation(model.ID, OTRWID, model.StartDate, model.EndDate, days, week1, week2);
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " Add roasted off");
                }
                TempData["Message"] = 1;

                var routeValues = new RouteValueDictionary();
                routeValues.Add("PageSize", PageSize);

                return RedirectToAction("ViewRoastedOff", routeValues);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                VacationRepo.Dispose();
            }
        }

        //GET : Employee/Employee/DeleteRoastedOff
        /// <summary>
        /// Delete Roasted Off
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteRoastedOff(string id)
        {
            try
            {
                var Id = Guid.Parse(id);
                var Roasted = RoastefOffRepo.FindBy(i => i.ID == Id).FirstOrDefault();
                Roasted.IsDelete = true;

                RoastefOffRepo.DeAttach(Roasted);
                RoastefOffRepo.Edit(Roasted);
                RoastefOffRepo.Save();

                // Delete Data in RoastedOffWeekMappings
                var WeekMapping = RoastedOffWeekMappingRepo.FindBy(m => m.RoastedOffId == Roasted.ID).ToList();
                foreach (var deleteWeek in WeekMapping)  //Delete Exist Data
                {
                    RoastedOffWeekMappingRepo.Delete(deleteWeek);
                    RoastedOffWeekMappingRepo.Save();
                }

                //Delete Data in Vacation
                var vacationData = VacationRepo.FindBy(m => m.RoastedOffId == Id).ToList();
                foreach (var vacation in vacationData)
                {
                    vacation.IsDelete = true;

                    VacationRepo.DeAttach(vacation);
                    VacationRepo.Edit(vacation);
                    VacationRepo.Save();
                }

                TempData["Message"] = 3;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted employee leave.");

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GetWeeksUsingByDays(string dayValue)
        {
            ModelState.Clear();
            EmployeeRoastedOffViewModel roastedOffViewModel = new EmployeeRoastedOffViewModel();

            var getDayName = Enum.GetName(typeof(DayOfWeek), Convert.ToInt32((dayValue)));

            var weeks = from EmployeeRoastdWeeks e in Enum.GetValues(typeof(EmployeeRoastdWeeks))
                        select new
                        {
                            Id = (int)e,
                            Name = e.ToString()
                        };
            List<SelectListItem> lstEmployeeRoastdWeeks = new List<SelectListItem>();

            foreach (var item in weeks)
            {
                SelectListItem lstEmp = new SelectListItem();
                lstEmp.Text = item.Name + " " + getDayName;
                lstEmp.Value = item.Id.ToString();
                lstEmployeeRoastdWeeks.Add(lstEmp);
            }

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " Get week name using day id.");

            roastedOffViewModel.Weeks = lstEmployeeRoastdWeeks;
            return Json(roastedOffViewModel.Weeks, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GenerateTimesheetReports(ReportsViewModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            switch (model.ReportType)
        //            {
        //                case Constant.TimesheetReportType.LabourCostPerHour:
        //                    var data = TimeSheetRepository.GetLabourCostPerHour(model.EmployeeIds);
        //                    List<LabourCostPerHourViewModel> HourList = new List<LabourCostPerHourViewModel>();
        //                    foreach (var i in data)
        //                    {
        //                        LabourCostPerHourViewModel item = new LabourCostPerHourViewModel();
        //                        item.EID = i.EID;

        //                        item.EmployeeId = i.EmployeeId;
        //                        item.Name = i.Name;
        //                        item.BaseRate = i.BaseRate;
        //                        item.S_WC = i.S_WC;
        //                        item.AL_PH = i.AL_PH;
        //                        item.TAFE = i.TAFE;
        //                        item.Payroll = i.Payroll;
        //                        item.Cont_MV_EQ_Cost = i.Cont_MV_EQ_Cost;
        //                        item.Emp_MV_Cost = i.Emp_MV_Cost;
        //                        item.Equip_Cost = i.Equip_Cost;
        //                        item.Emp_Mob_Ph_Cost = i.Emp_Mob_Ph_Cost;
        //                        item.GrossLabourCost = i.GrossLabourCost;
        //                        HourList.Add(item);
        //                    }
        //                    return new ViewAsPdf("LabourCostPerHour", HourList)
        //                    {
        //                        FileName = "LabourCostPerHourReport.pdf"
        //                    };

        //                case Constant.TimesheetReportType.LabourTimeSheet:
        //                    TimeSheetRepository.GetLabourTimeSheet(model.StartDate, model.EndDate, model.EmployeeId.ToString());
        //                    break;
        //            }
        //        }
        //        else
        //        {

        //        }
        //        model.EmployeeList = Employee.GetAll().Select(m => new SelectListItem()
        //        {
        //            Text = m.FirstName + " " + m.LastName,
        //            Value = m.EmployeeId.ToString()
        //        }).ToList();
        //        return View(model);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}
