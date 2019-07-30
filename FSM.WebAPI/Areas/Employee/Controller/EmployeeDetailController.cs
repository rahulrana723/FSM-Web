using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Infrastructure;
using FSM.WebAPI.App_Start;
using FSM.WebAPI.Areas.Employee.ViewModels;
using FSM.WebAPI.Common;
using FSM.WebAPI.Models;
using log4net;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TransmitSms;

namespace FSM.WebAPI.Areas.Employee.Controller
{
   [Authorize]
    public class EmployeeDetailController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        UnityContainer container = new UnityContainer();

        public IEmployeeDetailRepository EmployeeDetailRepository { get; set; }

        public EmployeeDetailController()
        {
            container = (UnityContainer)UnityConfig.GetConfiguredContainer();
            EmployeeDetailRepository = container.Resolve<IEmployeeDetailRepository>();
        }

        //GET: api/EmployeeDetail/  
        
        public ServiceResponse<EmployeeProfileViewModel> GetEmployeeDetail(Guid id)
        {
            ServiceResponse<EmployeeProfileViewModel> result = new ServiceResponse<EmployeeProfileViewModel>();
            try
            {
                EmployeeDetail employeeDetail = db.EmployeeDetail.Where(m => m.EmployeeId == id && m.IsDelete == false).FirstOrDefault();
                var unreadMessage = db.UserMessage.Where(m => m.To_Id == id && m.IsMessageRead == false).Count();


                var EmployeeWorkType = db.EmployeeWorkType.Where(m => m.EmployeeId == id).Select(m => m.WorkType).ToArray();

                if (employeeDetail == null)
                {
                    result.ResponseCode = 2;
                    result.ResponseErrorMessage = "User not exists";
                    return result;
                }

                employeeDetail.ProfilePicture = employeeDetail.ProfilePicture;
                CommonMapper<EmployeeDetail, EmployeeProfileViewModel> mapper = new CommonMapper<EmployeeDetail, EmployeeProfileViewModel>();
                EmployeeProfileViewModel objEmployeeProfileViewModel = mapper.Mapper(employeeDetail);
                objEmployeeProfileViewModel.IsActive = employeeDetail.IsActive == true ? "1" : "0";
                objEmployeeProfileViewModel.ViewInvoice = employeeDetail.ViewInvoice == true ? "1" : "0";
                objEmployeeProfileViewModel.MakeInvoice = employeeDetail.MakeInvoice == true ? "1" : "0";
                objEmployeeProfileViewModel.ApproveInvoice = employeeDetail.ApproveInvoice == true ? "1" : "0";
                objEmployeeProfileViewModel.SendInvoice = employeeDetail.SendInvoice == true ? "1" : "0";
                objEmployeeProfileViewModel.MYOB = employeeDetail.MYOB == true ? "1" : "0";
                objEmployeeProfileViewModel.WorkType = EmployeeWorkType;
                objEmployeeProfileViewModel.UnreadMsgCount = unreadMessage;


                result.ResponseList = new List<EmployeeProfileViewModel>();
                result.ResponseList.Add(objEmployeeProfileViewModel);
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "null";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get employee details.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }
        [HttpPost]
        [Route("api/EmployeeDetail/UpdateEmployeeDetail")]
        public ServiceResponse<string> UpdateEmployeeDetail(EmployeeDetailVM employeeDetailVM)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<EmployeeDetailVM, EmployeeDetailTemp> empmapper = new CommonMapper<EmployeeDetailVM, EmployeeDetailTemp>();
                var employeeDetailTemp = empmapper.Mapper(employeeDetailVM);
                employeeDetailTemp.Id = Guid.NewGuid();
                db.EmployeeDetailTemp.Add(employeeDetailTemp);
                db.SaveChanges();

                EmployeeDetail employeeDetail = db.EmployeeDetail.Find(employeeDetailVM.EmployeeId);
                // adding notifications
                string sql = @"DECLARE @UserId uniqueidentifier
                                DECLARE @UserName varchar(100) = '" + employeeDetail.FirstName + " "
                                + employeeDetail.LastName + @"'           
                                DECLARE @EmpTempId uniqueidentifier ='" + employeeDetailTemp.Id + @"'
                                DECLARE @EmpNo varchar(50) = " + employeeDetail.EID + @"
                                
                                DECLARE @tmpTableUsers as table
                                (
                                RowId int identity(1,1)
                                ,UserId uniqueidentifier
                                ) 
                                DECLARE @CurrentRowUsers int = 0
                                DECLARE @TotalRowsUsers int = 0 
                                
                                INSERT INTO @tmpTableUsers
                                SELECT EmployeeDetail.EmployeeId
                                from EmployeeDetail
                                INNER JOIN AspNetUserRoles ON AspNetUserRoles.UserId = EmployeeDetail.EmployeeId
                                INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId
                                WHERE AspNetRoles.Name like '%admin%' OR AspNetRoles.Name like '%operation%'
                                
                                SET @TotalRowsUsers = @@ROWCOUNT
                                
                                WHILE (@CurrentRowUsers < @TotalRowsUsers)
                                                              
                                BEGIN
                                    SET @CurrentRowUsers+=1
                                    
                                    SELECT @UserId = UserId FROM @tmpTableUsers WHERE RowId = @CurrentRowUsers
                                    
                                    INSERT INTO WebNotifications
                                    (
                                    Id
                                    ,NotificationMessage
                                    ,NotificationType
                                    ,NotificationTypeId
                                    ,UserId
                                    ,IsRead
                                    ,CreatedDate
                                    )
                                    SELECT 
                                       NEWID()
                                      ,@UserName +' (#'+@EmpNo+') has updated his/her proifle.'
                                      ,'EmployeeProfile'
                                      ,@EmpTempId
                                      ,@UserId
                                      ,0
                                      ,GETDATE()
                                END";


                db.Database.ExecuteSqlCommand(sql);

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " updates employee details.");

                result.ResponseList = new List<string> { "Profile sent for approval succesfully" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;


                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpPost]
        [Route("api/EmployeeDetail/UpdateEmployeeDetailMobile")]
        public ServiceResponse<string> UpdateEmployeeDetailMobile(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                //string homeAddressMobile = "489654";
                EmployeeDetail employeeDetail = db.EmployeeDetail.Find(new Guid(json.Id.Value));
                employeeDetail.HomeAddressMobile = json.HomeAddressMobile.Value;
                //db.EmployeeDetail.Add(employeeDetail);
                db.Entry(employeeDetail).State = EntityState.Modified;
                db.SaveChanges();

                result.ResponseList = new List<string> { "Mobile number updated succesfully" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get mobile number of an employee.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        public ServiceResponse<string> PostFile()
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            string EmpID = "";
            EmpID = HttpContext.Current.Request.Form["Id"] ?? ""; //adding empty string incase no content was 

            var miliSeconds = DateTime.Now.Millisecond;

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body
                // Read the form data and return an async task.
                //   await Request.Content.ReadAsMultipartAsync(provider);

                HttpPostedFile uploadedFile = HttpContext.Current.Request.Files["profilePics"];
                if (uploadedFile == null)
                {
                    // throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                else
                {
                    //retrieve the string with name value...
                    var httpRequest = HttpContext.Current.Request;

                    if (httpRequest.Files.Count > 0)
                    {
                        foreach (string file in httpRequest.Files)
                        {
                            var postedFile = httpRequest.Files[file];

                            var filePath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeImagesPath/" + EmpID + miliSeconds + ".jpg";
                            postedFile.SaveAs(filePath);

                            // NOTE: To store in memory use postedFile.InputStream
                        }
                        //Objtbl_Company.Logo = Company.UserName + ".jpg";
                    }
                }
                // This illustrates how to get the form data.

                //EmployeeDetail ObjEmployeeDetail = new EmployeeDetail();
                //ObjEmployeeDetail = db.EmployeeDetail.Find(Guid.Parse(EmpID));
                //ObjEmployeeDetail.ProfilePicture = EmpID + miliSeconds + ".jpg";
                //db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " post a document.");

                result.ResponseList = new List<string> { EmpID + miliSeconds + ".jpg" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (System.Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpPost]
        [Route("api/EmployeeDetail/AddEmployeeVacation")]
        public ServiceResponse<string> AddEmployeeVacation(VacationViewModel vacationViewModel)
        {
            Nullable<Guid> EmployeeId = vacationViewModel != null ? vacationViewModel.EmployeeId : null;
            Nullable<Guid> CreatedBy = vacationViewModel != null ? vacationViewModel.CreatedBy : null;
            Nullable<DateTime> StartDate = vacationViewModel != null ? vacationViewModel.StartDate : null;
            Nullable<DateTime> EndDate = vacationViewModel != null ? vacationViewModel.EndDate : null;
            Nullable<int> Hours = vacationViewModel != null ? vacationViewModel.Hours : null;

            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (vacationViewModel != null)
                {
                    if (EmployeeId.HasValue && StartDate.HasValue && EndDate.HasValue)
                    {
                        if (StartDate.Value.Date < DateTime.Now.Date || EndDate.Value.Date < DateTime.Now.Date)
                        {
                            result.ResponseCode = 0;
                            result.ResponseErrorMessage = "Start Date and End Date can't be less than current date !";
                            return result;
                        }

                        var vacationExist = db.Vacation.Where(m => m.EmployeeId == EmployeeId && m.StartDate == StartDate
                                            && m.EndDate == EndDate).FirstOrDefault();

                        if (vacationExist == null)
                        {
                            vacationViewModel.Id = Guid.NewGuid();
                            vacationViewModel.Status = 1; // Pending Approval
                            vacationViewModel.CreatedDate = DateTime.Now;
                            vacationViewModel.IsDelete = false;

                            // mapping viewmodel to entity
                            CommonMapper<VacationViewModel, Vacation> mapper = new CommonMapper<VacationViewModel, Vacation>();
                            Vacation vacation = mapper.Mapper(vacationViewModel);

                            db.Vacation.Add(vacation);
                            db.SaveChanges();

                            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                            response = CommonFunctions.GetUserInfoByToken();
                            string userId = response.ResponseList[0].UserId;
                            string userName = response.ResponseList[0].UserName;


                            log4net.ThreadContext.Properties["UserId"] = userId;
                            log.Info(userName + " add employee vacations.");


                            result.ResponseList = new List<string> { "Vacation added for approval !" };
                            result.ResponseCode = 1;
                            result.ResponseErrorMessage = null;
                            return result;
                        }
                        else
                        {
                            result.ResponseCode = 0;
                            result.ResponseErrorMessage = "Vacation for employee already exist on given dates !";
                            return result;
                        }
                    }
                    else
                    {
                        result.ResponseCode = 0;
                        result.ResponseErrorMessage = "Employee Id, Start Date, and End Date are required !";
                        return result;
                    }
                }
                else
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "Invalid data !";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpPost]
        [Route("api/EmployeeDetail/GetEmployeeVacation")]
        public ServiceResponse<Vacation> GetEmployeeVacation(VacationViewModel vacationViewModel)
        {
            Nullable<Guid> EmployeeId = vacationViewModel != null ? vacationViewModel.EmployeeId : null;
            Nullable<DateTime> StartDate = vacationViewModel != null ? vacationViewModel.StartDate : null;
            Nullable<DateTime> EndDate = vacationViewModel != null ? vacationViewModel.EndDate : null;
            ServiceResponse<Vacation> result = new ServiceResponse<Vacation>();
            try
            {
                if (EmployeeId.HasValue)
                {
                    var vacation = from vc in db.Vacation
                                   where vc.EmployeeId == EmployeeId && vc.IsDelete == false
                                   select vc;

                    result.ResponseList = vacation.OrderByDescending(m => m.CreatedDate).ToList();
                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = null;

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " get employee vacations.");

                    return result;
                }
                else
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "Employee Id is required!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpPost]
        [Route("api/EmployeeDetail/SaveUserMessage")]
        public ServiceResponse<string> SaveUserMessage(UserMessageViewModel userMessageViewModel)
        {
            Nullable<Guid> LoggedInUser = userMessageViewModel != null ? userMessageViewModel.LoggedInUser : null;
            Nullable<Guid> ToMessageUser = userMessageViewModel != null ? userMessageViewModel.ToMessageUser : null;
            string Message = userMessageViewModel != null ? userMessageViewModel.Message : null;

            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (LoggedInUser.HasValue && ToMessageUser.HasValue && !string.IsNullOrEmpty(Message))
                {
                    // getting user threads
                    var userThread = db.UserMessageThread.FirstOrDefault(m => m.LoggedInUser == LoggedInUser && m.ToMessageUser == ToMessageUser);
                    var userReverseThread = db.UserMessageThread.FirstOrDefault(m => m.LoggedInUser == ToMessageUser && m.ToMessageUser == LoggedInUser);

                    if (userThread != null)
                    {
                        SaveThreadMessages(LoggedInUser, ToMessageUser, Message, userThread, userReverseThread);

                        // updating user thread
                        userThread.ModifiedDate = DateTime.Now;
                        userThread.ModifiedBy = LoggedInUser;
                        db.SaveChanges();

                        // updating reverse user thread
                        userReverseThread.ModifiedDate = DateTime.Now;
                        userReverseThread.ModifiedBy = LoggedInUser;
                        db.SaveChanges();

                        ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                        response = CommonFunctions.GetUserInfoByToken();
                        string userId = response.ResponseList[0].UserId;
                        string userName = response.ResponseList[0].UserName;


                        log4net.ThreadContext.Properties["UserId"] = userId;
                        log.Info(userName + " save user messages.");

                    }
                    else
                    {
                        SavingUserThreadData(LoggedInUser, ToMessageUser, Message);
                        SavingRevUserThreadData(LoggedInUser, ToMessageUser, Message);
                    }

                    result.ResponseList = new List<string>() { "Message sent successfully !" };
                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = null;
                    return result;
                }
                else
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "LoggedInUser, ToMessageUser and Message are required !";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }
        private void SaveThreadMessages(Guid? LoggedInUser, Guid? ToMessageUser, string Message, UserMessageThread userThread,
                            UserMessageThread userReverseThread)
        {
            PushN objPushN = new PushN();
            List<UserDeviceTokenMapping> lstUserDeviceTokenMapping = db.UserDeviceTokenMappings.Where(m => m.UserId == ToMessageUser).ToList();
            EmployeeDetail objEmployeeDetail = db.EmployeeDetail.Where(m => m.EmployeeId == LoggedInUser).FirstOrDefault();

            // saving user message
            UserMessage userMessage = new UserMessage();
            userMessage.ID = Guid.NewGuid();
            userMessage.MessageThreadID = userThread.ID; // adding foreign key
            userMessage.From_Id = LoggedInUser;
            userMessage.To_Id = ToMessageUser;
            userMessage.Message = Message;
            userMessage.IsMessageRead = true;
            userMessage.CreatedDate = DateTime.Now;
            userMessage.CreatedBy = LoggedInUser;
            db.UserMessage.Add(userMessage);
            db.SaveChanges();

            // saving reverse thread message
            UserMessage userRevMessage = new UserMessage();
            userRevMessage.ID = Guid.NewGuid();
            userRevMessage.MessageThreadID = userReverseThread.ID; // adding reverse thread foreign key
            userRevMessage.From_Id = LoggedInUser;
            userRevMessage.To_Id = ToMessageUser;
            userRevMessage.Message = Message;
            userRevMessage.IsMessageRead = false;
            userRevMessage.CreatedDate = DateTime.Now;
            userRevMessage.CreatedBy = LoggedInUser;

            db.UserMessage.Add(userRevMessage);
            db.SaveChanges();

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " save user thread message.");


            Notifications Notifications = new Notifications();
            Notifications.Id = Guid.NewGuid();
            Notifications.NotificationMessage = objEmployeeDetail.FirstName + " " + objEmployeeDetail.LastName + " sends you a message.";
            Notifications.NotificationType = "Message";
            Notifications.NotificationTypeId = userMessage.ID;
            Notifications.UserId = userMessage.To_Id;
            Notifications.CreatedBy = userMessage.From_Id;
            Notifications.CreatedDate = DateTime.Now;
            db.Notifications.Add(Notifications);
            db.SaveChanges();

            Task.Run(() =>
            {
                foreach (UserDeviceTokenMapping obj in lstUserDeviceTokenMapping)
                {
                    if (obj.DeviceToken != "0")
                    {

                        objPushN.PushToIphone(obj.DeviceToken,
                            objEmployeeDetail.FirstName + " " + objEmployeeDetail.LastName + " send you a message.",
                            System.Configuration.ConfigurationManager.AppSettings["NotificationPassword"],
                            Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsProduction"]),
                            AppDomain.CurrentDomain.BaseDirectory + "IphoneCertificateLocation/Ref/fsmPushNotification.p12");
                    }
                }
            });
        }
        private void SavingUserThreadData(Guid? LoggedInUser, Guid? ToMessageUser, string Message)
        {
            UserMessageThread userMessageThread = SavingUserThread(LoggedInUser, ToMessageUser);

            PushN objPushN = new PushN();
            List<UserDeviceTokenMapping> lstUserDeviceTokenMapping = db.UserDeviceTokenMappings.Where(m => m.UserId == ToMessageUser).ToList();
            EmployeeDetail objEmployeeDetail = db.EmployeeDetail.Where(m => m.EmployeeId == LoggedInUser).FirstOrDefault();

            // saving user message
            UserMessage userMessage = new UserMessage();
            userMessage.ID = Guid.NewGuid();
            userMessage.MessageThreadID = userMessageThread.ID; // adding foreign key
            userMessage.From_Id = LoggedInUser;
            userMessage.To_Id = ToMessageUser;
            userMessage.Message = Message;
            userMessage.IsMessageRead = true;
            userMessage.CreatedDate = DateTime.Now;
            userMessage.CreatedBy = LoggedInUser;
            db.UserMessage.Add(userMessage);
            db.SaveChanges();

            Notifications Notifications = new Notifications();
            Notifications.Id = Guid.NewGuid();
            Notifications.NotificationMessage = objEmployeeDetail.FirstName + " " + objEmployeeDetail.LastName + " sends you a message.";
            Notifications.NotificationType = "Message";
            Notifications.NotificationTypeId = userMessage.ID;
            Notifications.UserId = userMessage.To_Id;
            Notifications.CreatedBy = userMessage.From_Id;
            Notifications.CreatedDate = DateTime.Now;
            db.Notifications.Add(Notifications);
            db.SaveChanges();

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " save user thread data.");


            Task.Run(() =>
            {
                foreach (UserDeviceTokenMapping obj in lstUserDeviceTokenMapping)
                {
                    if (obj.DeviceToken != "0")
                    {
                        objPushN.PushToIphone(obj.DeviceToken,
                            objEmployeeDetail.FirstName + " " + objEmployeeDetail.LastName + " sends you a message.",
                            System.Configuration.ConfigurationManager.AppSettings["NotificationPassword"],
                            Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsProduction"]),
                            AppDomain.CurrentDomain.BaseDirectory + "IphoneCertificateLocation/Ref/fsmPushNotification.p12");
                    }
                }
            });
        }
        private UserMessageThread SavingUserThread(Guid? LoggedInUser, Guid? ToMessageUser)
        {
            // saving user thread
            UserMessageThread userMessageThread = new UserMessageThread();
            userMessageThread.ID = Guid.NewGuid();
            userMessageThread.LoggedInUser = LoggedInUser;
            userMessageThread.ToMessageUser = ToMessageUser;
            userMessageThread.CreatedDate = DateTime.Now;
            userMessageThread.CreatedBy = LoggedInUser;

            db.UserMessageThread.Add(userMessageThread);
            db.SaveChanges();
            return userMessageThread;
        }
        private async void SavingRevUserThreadData(Guid? LoggedInUser, Guid? ToMessageUser, string Message)
        {
            UserMessageThread userRevMessageThread = await SavingRevUserThread(LoggedInUser, ToMessageUser);

            // saving reverse user message
            UserMessage userRevMessage = new UserMessage();
            userRevMessage.ID = Guid.NewGuid();
            userRevMessage.MessageThreadID = userRevMessageThread.ID; // adding reverse thread foreign key
            userRevMessage.From_Id = LoggedInUser;
            userRevMessage.To_Id = ToMessageUser;
            userRevMessage.Message = Message;
            userRevMessage.IsMessageRead = false;
            userRevMessage.CreatedDate = DateTime.Now;
            userRevMessage.CreatedBy = LoggedInUser;

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " save reverse user thread data.");

            db.UserMessage.Add(userRevMessage);
            db.SaveChanges();
        }
        private Task<UserMessageThread> SavingRevUserThread(Guid? LoggedInUser, Guid? ToMessageUser)
        {
            // saving reverse user thread
            UserMessageThread userRevMessageThread = new UserMessageThread();
            userRevMessageThread.ID = Guid.NewGuid();
            userRevMessageThread.LoggedInUser = ToMessageUser;
            userRevMessageThread.ToMessageUser = LoggedInUser;
            userRevMessageThread.CreatedDate = DateTime.Now;
            userRevMessageThread.CreatedBy = LoggedInUser;

            db.UserMessageThread.Add(userRevMessageThread);
            db.SaveChanges();
            return Task.FromResult(userRevMessageThread);
        }


        [HttpPost]
        [Route("api/EmployeeDetail/GetUserMessage")]
        public ServiceResponse<UserMessages> GetUserMessage(UserMessageViewModel userMessageViewModel)
        {
            Nullable<Guid> LoggedInUser = userMessageViewModel != null ? userMessageViewModel.LoggedInUser : null;
            Nullable<Guid> ToMessageUser = userMessageViewModel != null ? userMessageViewModel.ToMessageUser : null;

            ServiceResponse<UserMessages> result = new ServiceResponse<UserMessages>();
            try
            {
                if (LoggedInUser.HasValue && ToMessageUser.HasValue)
                {
                    Guid userThreadId = db.UserMessageThread.Where(m => m.LoggedInUser == LoggedInUser
                                        && m.ToMessageUser == ToMessageUser).Select(m => m.ID).FirstOrDefault();

                    string sql = @"Update dbo.UserMessage Set IsMessageRead = 1 
                                where MessageThreadID = '" + userThreadId + @"'

                                select UM.Message, Usr.UserName from dbo.UserMessage UM
                                join dbo.AspNetUsers Usr on UM.From_Id = Usr.Id
                                where MessageThreadID = '" + userThreadId + @"'
                                order by UM.CreatedDate";
                    var userMessages = db.Database.SqlQuery<UserMessages>(sql).AsQueryable();

                    result.ResponseList = userMessages.ToList();
                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = null;

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " get list of user messages.");

                    return result;

                }
                else
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "LoggedInUser and ToMessageUser are required!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpPost]
        [Route("api/EmployeeDetail/GetUsersByRole")]
        public ServiceResponse<dynamic> GetUsersByRole(dynamic json)
        {
            ServiceResponse<dynamic> result = new ServiceResponse<dynamic>();
            try
            {
                if (!string.IsNullOrEmpty(json.RoleId.Value))
                {
                    string RoleId = json.RoleId.Value;

                    IQueryable<dynamic> resultObject = from U in db.EmployeeDetail
                                                       join UR in db.AspNetUserRoles on U.EmployeeId.ToString() equals UR.UserId
                                                       where UR.RoleId == RoleId && U.IsDelete == false
                                                       select new
                                                       {
                                                           Id = U.EmployeeId,
                                                           UserName = U.UserName
                                                       };
                    result.ResponseList = resultObject.ToList();
                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = null;

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " get user by role.");

                    return result;
                }
                else
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "Role is Required!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpPost]
        [Route("api/EmployeeDetail/GetRecentUser")]
        public ServiceResponse<RecentUsers> GetRecentUser(UserMessageViewModel userMessageViewModel)
        {
            Nullable<Guid> LoggedInUser = userMessageViewModel != null ? userMessageViewModel.LoggedInUser : null;

            ServiceResponse<RecentUsers> result = new ServiceResponse<RecentUsers>();
            try
            {
                if (LoggedInUser.HasValue)
                {
                    string sql = @"SELECT AspNetUsers.Id UserId
                                  ,AspNetUsers.UserName
                                  ,(SELECT COUNT(Id) FROM UserMessage WHERE MessageThreadID = UserMessageThread.Id and IsMessageRead = 0) UnReadMsg
                                  ,UserMessageThread.ModifiedDate 'Date'
                                  FROM UserMessageThread 
                                  INNER JOIN AspNetUsers ON AspNetUsers.Id = UserMessageThread.ToMessageUser  
                                  WHERE UserMessageThread.LoggedInUser = '" + LoggedInUser + @"'
                                  OR UserMessageThread.ToMessageUser = '" + LoggedInUser + "' Order By UserMessageThread.ModifiedDate desc";
                    var recentUsers = db.Database.SqlQuery<RecentUsers>(sql).AsQueryable();

                    result.ResponseList = recentUsers.ToList();
                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = null;

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " get recent user.");

                    return result;
                }
                else
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "LoggedInUser is required!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpGet]
        [Route("api/EmployeeDetail/GetPublicHolidays")]
        public ServiceResponse<dynamic> GetPublicHolidays()
        {
            ServiceResponse<dynamic> result = new ServiceResponse<dynamic>();
            try
            {
                IQueryable<dynamic> resultObject = from Holiday in db.PublicHoliday
                                                   where Holiday.IsDelete == false
                                                   select new
                                                   {
                                                       Id = Holiday.Id,
                                                       Date = Holiday.Date,
                                                       Reason = Holiday.Reason
                                                   };
                result.ResponseList = resultObject.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get public holidats of an employee.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [HttpGet]
        [Route("api/EmployeeDetail/GetNotificationsById")]
        public ServiceResponse<dynamic> GetNotificationsById(Guid? UserId)
        {
            ServiceResponse<dynamic> result = new ServiceResponse<dynamic>();
            try
            {
                IQueryable<dynamic> lstResult = from NTF in db.Notifications
                                                join user in db.AspNetUsers on NTF.CreatedBy.ToString() equals user.Id
                                                where NTF.UserId == UserId
                                                select new
                                                {
                                                    NTF.Id,
                                                    NTF.NotificationMessage,
                                                    NTF.NotificationType,
                                                    NTF.NotificationTypeId,
                                                    NTF.UserId,
                                                    user.UserName,
                                                    NTF.CreatedBy,
                                                    NTF.CreatedDate
                                                };

                result.ResponseList = lstResult.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get notification by user id.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/EmployeeDetail/UpdateOTRWLatitudeLongitude")]
        public ServiceResponse<string> UpdateOTRWLatitudeLongitude(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid UserId = new Guid(json.EmployeeId.Value);
                string Latitude = json.Latitude.Value;
                string Longitude = json.Longitude.Value;

                EmployeeDetail employeeEntity = db.EmployeeDetail.Where(m => m.EmployeeId == UserId).FirstOrDefault();
                employeeEntity.Latitude = float.Parse(Latitude);
                employeeEntity.Longitude = float.Parse(Longitude);

                db.Entry(employeeEntity).State = EntityState.Modified;
                db.SaveChanges();


                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update latitude and longitude of an OTRW.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [Route("api/EmployeeDetail/SendMessage")]
        [HttpPost]
        public ServiceResponse<string> SendMessage(MessageViewModel model)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                //String[] TO = model.sendTo_number.Split(' ');
                //model.sentFrom_number = "7696138820";
                //TransmitSmsWrapper manager = new TransmitSmsWrapper("23dac442668a809eeaa7d9aaad5f91c7", "clientapisecret", "https://api.transmitsms.com");
                //var response = manager.SendSms("This is the Message", TO, model.sentFrom_number, null, null, "", "", 0);
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " send message.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;

            }
        }



        [Route("api/EmployeeDetail/SendEmail")]
        [HttpPost]
        public async Task<HttpResponseMessage> SendEmail()
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                EmailViewModels objemail = new EmailViewModels();
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["Message"]))
                {
                    objemail.Message = Convert.ToString(HttpContext.Current.Request.Form["Message"]);
                    objemail.Subject = Convert.ToString(HttpContext.Current.Request.Form["Subject"]);
                    objemail.Body = Convert.ToString(HttpContext.Current.Request.Form["Body"]);
                    objemail.To = Convert.ToString(HttpContext.Current.Request.Form["To"]);
                    objemail.From = Convert.ToString(HttpContext.Current.Request.Form["From"]);
                }

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " send email to " + objemail.To);

                string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
                List<FileInformation> files = new List<FileInformation>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                List<string> attachfiles = new List<string>();


                foreach (MultipartFileData file in provider.FileData)
                {
                    FileInformation fileinfo = new FileInformation();
                    string filename = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    fileinfo.filename = filename;
                    fileinfo.filepath = HttpContext.Current.Server.MapPath("~/App_Data/" + filename);
                    files.Add(fileinfo);
                    attachfiles.Add("~/App_Data/" + file.LocalFileName);
                }

                using (MailMessage mm = new MailMessage())
                {
                    mm.Subject = objemail.Subject;
                    mm.IsBodyHtml = true;
                    mm.Body = objemail.Body;
                    mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                    mm.To.Add(new MailAddress(objemail.To));
                    for (int i = 0; i < files.Count; i++)
                    {
                        var fileName = files[i].filename;
                        Stream fs = File.OpenRead(files[i].filepath);
                        var attachment = new Attachment(fs, fileName);
                        mm.Attachments.Add(attachment);
                    }

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                        smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                        smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                        smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtp.SendMailAsync(mm);
                    }
                    // Send OK Response along with saved file names to the client.

                }

                Array.ForEach(Directory.GetFiles(HttpContext.Current.Server.MapPath("~/App_Data/")), File.Delete);
                //result.ResponseCode = 1;
                //result.ResponseErrorMessage = null;
                return Request.CreateResponse(HttpStatusCode.OK, "Mail Send Successfully");
            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }



        }
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }


        [Route("api/EmployeeDetail/SendSupportEmail")]
        [HttpPost]
        public async Task<HttpResponseMessage> SendSupportEmail()
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {

                string from = "";
                string msg = "";
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                EmailViewModels objemail = new EmailViewModels();
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["Message"]))
                {
                    objemail.Message = Convert.ToString(HttpContext.Current.Request.Form["Message"]);
                    var Severity = Convert.ToString(HttpContext.Current.Request.Form["Severity"]);
                    objemail.Subject = "FSM :" + Severity;
                    objemail.UserId = Convert.ToString(HttpContext.Current.Request.Form["UserId"]);
                    Guid userid = Guid.Parse(objemail.UserId);
                    EmployeeDetail employeeEntity = db.EmployeeDetail.Where(i => i.EmployeeId == userid).FirstOrDefault();
                    msg = "Hi " + db.EmployeeDetail.Where(i => i.EmployeeId == userid).Select(i => i.UserName).FirstOrDefault() + ",";
                    //  objemail.Body = Convert.ToString(HttpContext.Current.Request.Form["Message"]);
                    string lastname = !String.IsNullOrEmpty(employeeEntity.LastName) ? employeeEntity.LastName : "";
                    objemail.Body = objemail.Message + @"<br/><br/></br>Thanks, <br/>" + employeeEntity.FirstName + " " + lastname + " <br/>" + employeeEntity.Email + "<br/>";
                    objemail.To = "support@srag-portal.com";//"goyaldeepak@seasiainfotech.com";//
                    objemail.From = employeeEntity.Email;
                    from = objemail.From;
                }

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " send support email.");

                string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
                List<FileInformation> files = new List<FileInformation>();
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                List<string> attachfiles = new List<string>();


                foreach (MultipartFileData file in provider.FileData)
                {
                    FileInformation fileinfo = new FileInformation();
                    string filename = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    fileinfo.filename = filename;
                    fileinfo.filepath = HttpContext.Current.Server.MapPath("~/App_Data/" + filename);
                    files.Add(fileinfo);
                    attachfiles.Add("~/App_Data/" + file.LocalFileName);
                }
                using (MailMessage mm = new MailMessage())
                {
                    mm.Subject = objemail.Subject;
                    mm.IsBodyHtml = true;
                    mm.Body = objemail.Body;
                    mm.Bcc.Add(new MailAddress("fsmsupport@seasiainfotech.com"));
                    mm.From = new MailAddress(objemail.From);
                    mm.To.Add(new MailAddress(objemail.To));
                    for (int i = 0; i < files.Count; i++)
                    {
                        var fileName = files[i].filename;
                        Stream fs = File.OpenRead(files[i].filepath);
                        var attachment = new Attachment(fs, fileName);
                        mm.Attachments.Add(attachment);
                    }

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                        smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                        smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                        smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtp.SendMailAsync(mm);


                        using (MailMessage mmrev = new MailMessage())
                        {
                            mmrev.Subject = "Thanks for Contacting us";
                            mmrev.IsBodyHtml = true;
                            mmrev.Body = msg + @"<br/><br/>Thanks for your support request, we will respond to your issue asap.
                                            <br/><br/><br/> 
                                            Thanks, <br/>
                                            Support Team<br/>
                                            <a href='www.srag-portal.com'>www.srag-portal.com<a>";
                            //mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                            //mmrev.From = new MailAddress("goyaldeepak@seasiainfotech.com");//("support@srag-portal.com");
                            mmrev.From = new MailAddress("support@srag-portal.com");//("support@srag-portal.com");
                            mmrev.To.Add(new MailAddress(from)); //Adding Multiple To email Id
                            using (SmtpClient smtprev = new SmtpClient())
                            {
                                smtprev.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                smtprev.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                smtprev.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                smtprev.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                smtprev.DeliveryMethod = SmtpDeliveryMethod.Network;
                                await smtp.SendMailAsync(mmrev);

                            }
                        }
                        // Send OK Response along with saved file names to the client.

                    }
                }

                Array.ForEach(Directory.GetFiles(HttpContext.Current.Server.MapPath("~/App_Data/")), File.Delete);
                //result.ResponseCode = 1;
                //result.ResponseErrorMessage = null;
                return Request.CreateResponse(HttpStatusCode.OK, "Mail Send Successfully");
            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }
    }
}

