using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
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
    public class MessageController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                   ().DeclaringType);
        [Dependency]
        public IEmployeeDetailRepository Employee { get; set; }
        [Dependency]
        public IUserMessageThreadRepository UserMessageThreadRepo { get; set; }
        [Dependency]
        public IUserMessageRepository UserMessageRepo { get; set; }
        [Dependency]
        public IEmployeeJobRepository EmployeeJobRepo { get; set; }

        // GET: Employee/Message/EmployeeMessageList
        [ValidateInput(false)]
        public ActionResult EmployeeMessageList()
        {
            try
            {
                using (Employee)
                {
                    string Searchstring = Request.QueryString["searchkeyword"];
                    Guid LoggedId = Guid.Parse(base.GetUserId);
                    Nullable<Guid> empRole = string.IsNullOrEmpty(Request.QueryString["Role"]) ? Guid.Empty :
                                               Guid.Parse(Request.QueryString["Role"]);

                    var employeerList = Employee.GetEmployeeDetailWithUnreadMessage(LoggedId.ToString(), Searchstring).AsEnumerable();
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);
                    var role = "";
                    if (empRole != Guid.Empty)
                    {
                        role = Employee.getUserRole(empRole.ToString()).FirstOrDefault().Name;
                    }

                    if (role != "")
                    {
                        employeerList = employeerList.Where(m => (m.Role != null && m.Role == role.ToString()));
                    }
                    Guid UserId = Guid.Parse(base.GetUserId);
                    employeerList = employeerList.Where(m => m.EmployeeId != UserId);

                    if (!string.IsNullOrEmpty(Searchstring))
                    {
                        employeerList = employeerList.Where(customer =>
                        (customer.Name != null && customer.Name.ToString().ToLower().Contains(Searchstring.ToLower())) ||
                        (customer.UserName != null && customer.UserName.ToLower().Contains(Searchstring.ToLower())) ||
                        (customer.EID != null && customer.EID.ToLower().Contains(Searchstring.ToLower())) ||
                        (customer.Message != null && customer.Message.ToLower().Contains(Searchstring.ToLower()))
                        );
                    }

                    CommonMapper<EmployeeDetailViewModelCore, EmpDetailListViewModel> mapper = new CommonMapper<EmployeeDetailViewModelCore, EmpDetailListViewModel>();
                    List<EmpDetailListViewModel> empDetailListViewModel = mapper.MapToList(employeerList.ToList());

                    var employeeDetailListViewModel = new EmployeeDetailListViewModel
                    {
                        EmployeeDetailList = empDetailListViewModel,
                        EmployeeDetailInfo = new EmployeeDetailSearchViewModel
                        {
                            FirstName = string.IsNullOrEmpty(Searchstring) ? "" : Searchstring,
                            GetUserRoles = this.GetUserRoles,
                            PageSize = PageSize,
                            Role = empRole.ToString()
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
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EmployeeMessageList(EmployeeDetailSearchViewModel employeeDetailSearchViewModel)
        {
            try
            {
                using (Employee)
                {
                    employeeDetailSearchViewModel.GetUserRoles = this.GetUserRoles;
                    Guid UserId = Guid.Parse(base.GetUserId);
                    Guid? RoleId = Guid.Empty;
                    if (employeeDetailSearchViewModel.Role != null)
                    {
                        RoleId = Guid.Parse(employeeDetailSearchViewModel.Role);
                    }

                    if (employeeDetailSearchViewModel.FirstName == null)
                        employeeDetailSearchViewModel.FirstName = "";

                    var employeerList = Employee.GetEmployeeDetailWithUnreadMessage(UserId.ToString(), employeeDetailSearchViewModel.FirstName).OrderByDescending(m => m.EID).AsEnumerable();
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                              Convert.ToInt32(Request.QueryString["page_size"]);
                    employeeDetailSearchViewModel.PageSize = PageSize;
                    var role = "";
                    if (RoleId != Guid.Empty)
                    {
                        role = Employee.getUserRole(RoleId.ToString()).FirstOrDefault().Name;
                    }
                    if (role != "")
                    {
                        employeerList = employeerList.Where(m => (m.Role != null && m.Role == role.ToString()));
                    }

                    employeerList = employeerList.Where(m => m.EmployeeId != UserId);
                    string Searchstring = employeeDetailSearchViewModel.FirstName;

                    if (!string.IsNullOrEmpty(Searchstring))
                    {
                        employeerList = employeerList.Where(customer =>
                        (customer.Name != null && customer.Name.ToString().ToLower().Contains(Searchstring.ToLower())) ||
                        (customer.UserName != null && customer.UserName.ToLower().Contains(Searchstring.ToLower())) ||
                        (customer.EID != null && customer.EID.ToLower().Contains(Searchstring.ToLower())) ||
                        (customer.Message != null && customer.Message.ToLower().Contains(Searchstring.ToLower()))
                        );
                    }

                    CommonMapper<EmployeeDetailViewModelCore, EmpDetailListViewModel> mapper = new CommonMapper<EmployeeDetailViewModelCore, EmpDetailListViewModel>();
                    List<EmpDetailListViewModel> empDetailListViewModel = mapper.MapToList(employeerList.ToList());

                    var employeeDetailListViewModel = new EmployeeDetailListViewModel
                    {
                        EmployeeDetailList = empDetailListViewModel,
                        EmployeeDetailInfo = employeeDetailSearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets list of employee message.");

                    return View(employeeDetailListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Employee/Message/EmployeeMessageList
        [ValidateInput(false)]
        [HttpGet]
        public ActionResult _EmployeeMessageChat(string EmployeeId)
        {
            try
            {
                UserMessageViewModel userMessageViewModel = new UserMessageViewModel();
                Guid LoggedId = Guid.Parse(base.GetUserId);
                Guid employeeId = Guid.Parse(EmployeeId);

                userMessageViewModel.From_Id = LoggedId;
                userMessageViewModel.To_Id = employeeId;
                if (LoggedId != null && employeeId != null)
                {
                    //Get user Thread Id 
                    var GetThread = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == LoggedId && i.ToMessageUser == employeeId).FirstOrDefault();
                    if (GetThread != null)
                    {
                        string loggeduserid = (GetThread.LoggedInUser).ToString();
                        Guid logid = Guid.Parse(loggeduserid);
                        Guid GetThreadId = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == logid && i.ToMessageUser == employeeId).FirstOrDefault().ID;
                        var MessageList = UserMessageRepo.GetMessageList(GetThreadId.ToString()).OrderBy(m => m.CreatedDate).ToList();

                        CommonMapper<UserMessageCoreViewModel, UserMessageViewModel> mapper = new CommonMapper<UserMessageCoreViewModel, UserMessageViewModel>();
                        List<UserMessageViewModel> messageViewModel = mapper.MapToList(MessageList.ToList());

                        var messageListViewModel = new MessageListViewModel
                        {
                            UserMessageForCoreViewModel = messageViewModel,
                            UserMessageViewModel = userMessageViewModel
                        };
                        return PartialView(messageListViewModel);
                    }
                }

                var messageListsViewModel = new MessageListViewModel
                {
                    UserMessageForCoreViewModel = new List<UserMessageViewModel>(),
                    UserMessageViewModel = userMessageViewModel

                };
                return PartialView(messageListsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _EmployeeMessageChat(string ToId, string FromId, string MessageBox)
        {
            Helper helper = new Helper();
            try
            {
                Guid fromId = Guid.Parse(FromId);
                Guid toId = Guid.Parse(ToId);
                var isOTRW = EmployeeJobRepo.GetOTRWUser().Where(m => m.Id == ToId).FirstOrDefault();

                var ThreadList = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == toId).FirstOrDefault();
                var ThreadReverseList = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == toId && i.ToMessageUser == fromId).FirstOrDefault();
                if (ThreadList == null)
                {
                    UserMessageThreadviewModel userMessageThreadviewModel = new UserMessageThreadviewModel();
                    userMessageThreadviewModel.ID = Guid.NewGuid();
                    userMessageThreadviewModel.LoggedInUser = fromId;
                    userMessageThreadviewModel.ToMessageUser = toId;
                    userMessageThreadviewModel.CreatedDate = DateTime.Now;
                    userMessageThreadviewModel.CreatedBy = Guid.Parse(base.GetUserId);
                    userMessageThreadviewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                    userMessageThreadviewModel.ModifiedDate = DateTime.Now;
                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageThreadviewModel, UserMessageThread> mapper = new CommonMapper<UserMessageThreadviewModel, UserMessageThread>();
                    UserMessageThread userMessageThread = mapper.Mapper(userMessageThreadviewModel);
                    UserMessageThreadRepo.Add(userMessageThread);
                    UserMessageThreadRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, toId, userMessageThreadviewModel.ID, "Message");
                    }
                }
                else
                {
                    Guid threadId = ThreadList.ID;
                    var ThreadDetail = UserMessageThreadRepo.FindBy(i => i.ID == threadId).FirstOrDefault();

                    ThreadDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                    ThreadDetail.ModifiedDate = DateTime.Now;

                    UserMessageThreadRepo.Edit(ThreadDetail);
                    UserMessageThreadRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, toId, threadId, "Message");
                    }
                }

                if (ThreadReverseList == null)
                {
                    UserMessageThreadviewModel userMessageThreadReverseviewModel = new UserMessageThreadviewModel();
                    userMessageThreadReverseviewModel.ID = Guid.NewGuid();
                    userMessageThreadReverseviewModel.LoggedInUser = toId;
                    userMessageThreadReverseviewModel.ToMessageUser = fromId;
                    userMessageThreadReverseviewModel.CreatedDate = DateTime.Now;
                    userMessageThreadReverseviewModel.CreatedBy = Guid.Parse(base.GetUserId);
                    userMessageThreadReverseviewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                    userMessageThreadReverseviewModel.ModifiedDate = DateTime.Now;

                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageThreadviewModel, UserMessageThread> mapper = new CommonMapper<UserMessageThreadviewModel, UserMessageThread>();
                    UserMessageThread userMessageThread = mapper.Mapper(userMessageThreadReverseviewModel);
                    UserMessageThreadRepo.Add(userMessageThread);
                    UserMessageThreadRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, toId, userMessageThreadReverseviewModel.ID, "Message");
                    }
                }
                else
                {
                    Guid threadReverseId = ThreadReverseList.ID;
                    var ThreadReverseDetail = UserMessageThreadRepo.FindBy(i => i.ID == threadReverseId).FirstOrDefault();

                    ThreadReverseDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                    ThreadReverseDetail.ModifiedDate = DateTime.Now;

                    UserMessageThreadRepo.Edit(ThreadReverseDetail);
                    UserMessageThreadRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, toId, threadReverseId, "Message");
                    }
                }

                var ThreadUserMessage = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == toId).FirstOrDefault();
                var ThreadUserReverseMessage = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == toId && i.ToMessageUser == fromId).FirstOrDefault();

                if (ThreadUserMessage != null)
                {
                    UserMessageViewModel userMessageViewModel = new UserMessageViewModel();
                    userMessageViewModel.ID = Guid.NewGuid();
                    userMessageViewModel.MessageThreadID = ThreadUserMessage.ID;
                    userMessageViewModel.From_Id = fromId;
                    userMessageViewModel.To_Id = toId;
                    userMessageViewModel.Message = MessageBox;
                    userMessageViewModel.IsMessageRead = true;
                    userMessageViewModel.CreatedDate = DateTime.Now;
                    userMessageViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageViewModel, UserMessage> mapper = new CommonMapper<UserMessageViewModel, UserMessage>();
                    UserMessage userMessage = mapper.Mapper(userMessageViewModel);
                    UserMessageRepo.Add(userMessage);
                    UserMessageRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, toId, userMessageViewModel.ID, "Message");
                    }
                }
                if (ThreadUserReverseMessage != null)
                {
                    UserMessageViewModel userMessageReverseViewModel = new UserMessageViewModel();
                    userMessageReverseViewModel.ID = Guid.NewGuid();
                    userMessageReverseViewModel.MessageThreadID = ThreadUserReverseMessage.ID;
                    userMessageReverseViewModel.From_Id = fromId;
                    userMessageReverseViewModel.To_Id = toId;
                    userMessageReverseViewModel.Message = MessageBox;
                    userMessageReverseViewModel.IsMessageRead = false;
                    userMessageReverseViewModel.CreatedDate = DateTime.Now;
                    userMessageReverseViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageViewModel, UserMessage> mappers = new CommonMapper<UserMessageViewModel, UserMessage>();
                    UserMessage userReverseMessage = mappers.Mapper(userMessageReverseViewModel);
                    UserMessageRepo.Add(userReverseMessage);
                    UserMessageRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, toId, userMessageReverseViewModel.ID, "Message");
                    }
                }
                //Get thread Id for user
                var GetThread = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == toId).FirstOrDefault();
                string loggeduserid = (GetThread.LoggedInUser).ToString();
                Guid logid = Guid.Parse(loggeduserid);

                var GetThreadId = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == logid && i.ToMessageUser == toId).FirstOrDefault().ID;
                var MessageList = UserMessageRepo.GetMessageList(GetThreadId.ToString()).OrderBy(m => m.CreatedDate).ToList();

                CommonMapper<UserMessageCoreViewModel, UserMessageViewModel> mapperList = new CommonMapper<UserMessageCoreViewModel, UserMessageViewModel>();
                List<UserMessageViewModel> messageViewModel = mapperList.MapToList(MessageList.ToList());

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(messageViewModel);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " chat with employee.");

                return Json(new { list = json, length = messageViewModel.Count() });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult DeleteMessageChat(string FromId, string ToId)
        {
            try
            {
                Guid fromId = Guid.Parse(FromId);
                Guid toId = Guid.Parse(ToId);

                Guid ThreadId = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == toId).FirstOrDefault().ID;
                List<UserMessage> ThreadList = UserMessageRepo.FindBy(i => i.MessageThreadID == ThreadId).ToList();

                if (ThreadList != null)
                {
                    foreach (var item in ThreadList)
                    {
                        UserMessageRepo.Delete(item);
                        UserMessageRepo.Save();
                    }
                }

                var MessageList = UserMessageRepo.GetMessageList(ThreadId.ToString()).OrderBy(m => m.CreatedDate).ToList();

                CommonMapper<UserMessageCoreViewModel, UserMessageViewModel> mapper = new CommonMapper<UserMessageCoreViewModel, UserMessageViewModel>();
                List<UserMessageViewModel> messageViewModel = mapper.MapToList(MessageList.ToList());

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(messageViewModel);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted message chat.");

                return Json(new { list = json, length = messageViewModel.Count() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult _ViewInboxMessageChat()
        {
            try
            {
                using (Employee)
                {
                    var employeerList = Employee.GetEmployeeDetail().AsEnumerable().OrderBy(m => m.UserName);
                    employeerList = employeerList.Where(m => m.IsDelete == false && m.IsActive == true).AsEnumerable().OrderBy(m => m.UserName);

                    EmployeeNameIdViewModel employeeNameIdViewModel = new EmployeeNameIdViewModel();
                    List<EmployeeNameDetail> li = new List<EmployeeNameDetail>();


                    foreach (var i in employeerList)
                    {
                        EmployeeNameDetail obj = new EmployeeNameDetail();
                        obj.EmployeeId = i.EmployeeId;
                        obj.UserName = i.UserName;
                        li.Add(obj);
                    }

                    employeeNameIdViewModel.employeeDetail = li;
                    employeeNameIdViewModel.FromId = Guid.Parse(base.GetUserId);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed inbox message list.");

                    return PartialView(employeeNameIdViewModel);
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _AllEmployeeMessageSend(string EmployeeName, string ToId, string FromId, string MessageBox)
        {
            Helper helper = new Helper();
            try
            {
                Guid ToUserId = Guid.Parse(ToId);
                var employeerList = Employee.GetEmployeeDetail().AsEnumerable().OrderBy(m => m.UserName);
                if (EmployeeName == "(All_Admin)")
                {
                    employeerList = employeerList.Where(m => m.Role == "Admin").AsEnumerable().OrderBy(m => m.UserName);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " sent message to all admin.");
                }
                if (EmployeeName == "(All_OTRW)")
                {
                    employeerList = employeerList.Where(m => m.Role == "OTRW").AsEnumerable().OrderBy(m => m.UserName);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " sent message to all otrw.");

                }
                Guid fromId = Guid.Parse(FromId);
                //Guid toId = Guid.Parse(ToId);
                var isOTRW = EmployeeJobRepo.GetOTRWUser().Where(m => m.Id == ToId).FirstOrDefault();

                foreach (var Emp in employeerList)
                {
                    Guid toId = Emp.EmployeeId;
                    var ThreadList = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == ToUserId).FirstOrDefault();
                    //var ThreadList = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == toId).FirstOrDefault();
                    var ThreadReverseList = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == toId && i.ToMessageUser == fromId).FirstOrDefault();
                    if (ThreadList == null)
                    {
                        UserMessageThreadviewModel userMessageThreadviewModel = new UserMessageThreadviewModel();
                        userMessageThreadviewModel.ID = Guid.NewGuid();
                        userMessageThreadviewModel.LoggedInUser = fromId;
                        userMessageThreadviewModel.ToMessageUser = ToUserId;
                        userMessageThreadviewModel.CreatedDate = DateTime.Now;
                        userMessageThreadviewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        userMessageThreadviewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                        userMessageThreadviewModel.ModifiedDate = DateTime.Now;
                        // mapping <viewmodel> to <entity>
                        CommonMapper<UserMessageThreadviewModel, UserMessageThread> mapper = new CommonMapper<UserMessageThreadviewModel, UserMessageThread>();
                        UserMessageThread userMessageThread = mapper.Mapper(userMessageThreadviewModel);
                        UserMessageThreadRepo.Add(userMessageThread);
                        UserMessageThreadRepo.Save();

                        if (isOTRW != null)
                        {
                            helper.SaveAndSendNotification(fromId, ToUserId, userMessageThreadviewModel.ID, "Message");
                        }
                    }
                    else
                    {
                        Guid threadId = ThreadList.ID;
                        var ThreadDetail = UserMessageThreadRepo.FindBy(i => i.ID == threadId).FirstOrDefault();

                        ThreadDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                        ThreadDetail.ModifiedDate = DateTime.Now;

                        UserMessageThreadRepo.Edit(ThreadDetail);
                        UserMessageThreadRepo.Save();

                        if (isOTRW != null)
                        {
                            helper.SaveAndSendNotification(fromId, toId, threadId, "Message");
                        }
                    }

                    if (ThreadReverseList == null)
                    {
                        UserMessageThreadviewModel userMessageThreadReverseviewModel = new UserMessageThreadviewModel();
                        userMessageThreadReverseviewModel.ID = Guid.NewGuid();
                        userMessageThreadReverseviewModel.LoggedInUser = toId;
                        userMessageThreadReverseviewModel.ToMessageUser = fromId;
                        userMessageThreadReverseviewModel.CreatedDate = DateTime.Now;
                        userMessageThreadReverseviewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        userMessageThreadReverseviewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                        userMessageThreadReverseviewModel.ModifiedDate = DateTime.Now;

                        // mapping <viewmodel> to <entity>
                        CommonMapper<UserMessageThreadviewModel, UserMessageThread> mapper = new CommonMapper<UserMessageThreadviewModel, UserMessageThread>();
                        UserMessageThread userMessageThread = mapper.Mapper(userMessageThreadReverseviewModel);
                        UserMessageThreadRepo.Add(userMessageThread);
                        UserMessageThreadRepo.Save();

                        if (isOTRW != null)
                        {
                            helper.SaveAndSendNotification(fromId, toId, userMessageThreadReverseviewModel.ID, "Message");
                        }
                    }
                    else
                    {
                        Guid threadReverseId = ThreadReverseList.ID;
                        var ThreadReverseDetail = UserMessageThreadRepo.FindBy(i => i.ID == threadReverseId).FirstOrDefault();

                        ThreadReverseDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                        ThreadReverseDetail.ModifiedDate = DateTime.Now;

                        UserMessageThreadRepo.Edit(ThreadReverseDetail);
                        UserMessageThreadRepo.Save();

                        if (isOTRW != null)
                        {
                            helper.SaveAndSendNotification(fromId, toId, threadReverseId, "Message");
                        }
                    }

                    var ThreadUserReverseMessage = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == toId && i.ToMessageUser == fromId).FirstOrDefault();

                    if (ThreadUserReverseMessage != null)
                    {
                        UserMessageViewModel userMessageReverseViewModel = new UserMessageViewModel();
                        userMessageReverseViewModel.ID = Guid.NewGuid();
                        userMessageReverseViewModel.MessageThreadID = ThreadUserReverseMessage.ID;
                        userMessageReverseViewModel.From_Id = fromId;
                        userMessageReverseViewModel.To_Id = toId;
                        userMessageReverseViewModel.Message = MessageBox;
                        userMessageReverseViewModel.IsMessageRead = false;
                        userMessageReverseViewModel.CreatedDate = DateTime.Now;
                        userMessageReverseViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                        // mapping <viewmodel> to <entity>
                        CommonMapper<UserMessageViewModel, UserMessage> mappers = new CommonMapper<UserMessageViewModel, UserMessage>();
                        UserMessage userReverseMessage = mappers.Mapper(userMessageReverseViewModel);
                        UserMessageRepo.Add(userReverseMessage);
                        UserMessageRepo.Save();

                        if (isOTRW != null)
                        {
                            helper.SaveAndSendNotification(fromId, toId, userMessageReverseViewModel.ID, "Message");
                        }
                    }
                }
                var ThreadUserMessage = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == ToUserId).FirstOrDefault();
                if (ThreadUserMessage != null)
                {
                    UserMessageViewModel userMessageViewModel = new UserMessageViewModel();
                    userMessageViewModel.ID = Guid.NewGuid();
                    userMessageViewModel.MessageThreadID = ThreadUserMessage.ID;
                    userMessageViewModel.From_Id = fromId;
                    userMessageViewModel.To_Id = ToUserId;
                    userMessageViewModel.Message = MessageBox;
                    userMessageViewModel.IsMessageRead = true;
                    userMessageViewModel.CreatedDate = DateTime.Now;
                    userMessageViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageViewModel, UserMessage> mapper = new CommonMapper<UserMessageViewModel, UserMessage>();
                    UserMessage userMessage = mapper.Mapper(userMessageViewModel);
                    UserMessageRepo.Add(userMessage);
                    UserMessageRepo.Save();

                    if (isOTRW != null)
                    {
                        helper.SaveAndSendNotification(fromId, ToUserId, userMessageViewModel.ID, "Message");
                    }
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(1);
                return Json(new { list = json, length = 1 });
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}